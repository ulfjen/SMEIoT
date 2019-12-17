/*
 * The plugin is targeting mosquitto 1.6.8.
 */

#include "auth.h"

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <errno.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/un.h>

// returns fd (file identifier for socket), -1 means the server is down
int mosqauth_connect_socket(const char *socket_path)
{
  struct sockaddr_un addr;
  int i;
  int ret = -1;
  int data_socket;

  /* Create local socket. */
  data_socket = socket(AF_UNIX, SOCK_STREAM, 0);
  if (data_socket == -1)
  {
    return ret;
  }

  /*
    * For portability clear the whole structure, since some
    * implementations have additional (nonstandard) fields in
    * the structure.
    */

  memset(&addr, 0, sizeof(struct sockaddr_un));

  /* Connect socket to socket address */

  addr.sun_family = AF_UNIX;
  strncpy(addr.sun_path, socket_path, sizeof(addr.sun_path) - 1);

  ret = connect(data_socket, (const struct sockaddr *)&addr,
                  sizeof(struct sockaddr_un));
  if (ret < 0) {
    return ret;
  }
  return data_socket;
}

int mosqauth_close_socket(int fd)
{
  return close(fd);
}

const int BUFFER_SIZE = 2048;

// we have a buffer of 2K.
int mosqauth_request(int fd, const char *body, int terminate_request)
{
  char buffer[BUFFER_SIZE];

  int cur = 0;
  int len = strlen(body);

  while (cur < len) {
    int batch_len = len - cur;
    batch_len = batch_len > BUFFER_SIZE ? BUFFER_SIZE : batch_len;
    strncpy(buffer, body + cur, batch_len);
    int ret = write(fd, buffer, batch_len);
    mosquitto_log_printf(MOSQ_LOG_DEBUG, "sending %s %u bytes ret %d\n", body, batch_len, ret);
    if (ret == -1) {
      return ret;
    }
    cur += ret;
  }
  if (terminate_request) {
    strcpy(buffer, "\n"); // we already know the size of this string so we don't use strncpy.
    int ret = write(fd, buffer, strlen(buffer));
    if (ret == -1)
    {
      return ret;
    }
    mosquitto_log_printf(MOSQ_LOG_DEBUG, "sending %s %u bytes ret %d\n", buffer, strlen(buffer), ret);
  }
  return cur + strlen(buffer);
}

const char* PSK_CMD = "GETPSK ";

int mosqauth_request_psk(int fd, const char *identity)
{
  int sent = 0;
  int ret = mosqauth_request(fd, PSK_CMD, 0);
  if (ret == -1) {
    return ret;
  }
  sent += ret;
  ret = mosqauth_request(fd, identity, 1);
  sent += ret;
  return sent;
}

const char* META_CMD = "POSTMETA ";

int mosqauth_request_meta(int fd, int pid)
{
  int sent = 0;
  int ret = mosqauth_request(fd, META_CMD, 0);
  if (ret == -1) {
    return ret;
  }
  sent += ret;

  int version = MOSQ_AUTH_PLUGIN_VERSION;
  char buffer[1024] = {0};
  sprintf(buffer, "{\"mosquittoAuthPluginVersion\":%d,\"pid\":%d}", version, pid);

  ret = mosqauth_request(fd, buffer, 1);
  sent += ret;
  return sent;
}

int mosqauth_receive(int fd, char* buffer, int len_buffer)
{
  int read_bytes = 0;
  while (1) {
    if (len_buffer - read_bytes <= 0) {
      return -1;
    }
    int ret = read(fd, buffer + read_bytes, len_buffer - read_bytes);
    mosquitto_log_printf(MOSQ_LOG_DEBUG, "read() returns = %d\n", ret);
    if (ret == -1)
    {
      return ret;
    }

    read_bytes += ret;
    if (read_bytes > 0 && buffer[read_bytes-1] == '\n') { // we are done reading
      // okay!
      break;
    }
  }
  buffer[read_bytes-1] = 0; // rewrite the terminal
  // mosquitto_log_printf(MOSQ_LOG_DEBUG, "Result = %s (buffer max len = %d)\n", buffer, len_buffer);
  return read_bytes;
}

// 0 means okay. -1 means FAIL, -2 means something else
int mosqauth_receive_ok(int fd)
{
  char buffer[1024];
  mosqauth_receive(fd, buffer, 1023);
  // mosquitto_log_printf(MOSQ_LOG_DEBUG, "Received: %s\n", buffer); // should get an OK.
  if (strncmp(buffer, "OK", 2) == 0) {
    return 0;
  } else if (strncmp(buffer, "FAIL", 4) == 0) {
    return -1;
  }
  return -2;
}

/*
 * Function: mosquitto_auth_plugin_version
 *
 * The broker will call this function immediately after loading the plugin to
 * check it is a supported plugin version. Your code must simply return
 * MOSQ_AUTH_PLUGIN_VERSION.
 */
int mosquitto_auth_plugin_version(void)
{
  return MOSQ_AUTH_PLUGIN_VERSION;
}

/*
 * Function: mosquitto_auth_plugin_init
 *
 * Called after the plugin has been loaded and <mosquitto_auth_plugin_version>
 * has been called. This will only ever be called once and can be used to
 * initialise the plugin.
 *
 * Parameters:
 *
 *	user_data :      The pointer set here will be passed to the other plugin
 *	                 functions. Use to hold connection information for example.
 *	opts :           Pointer to an array of struct mosquitto_opt, which
 *	                 provides the plugin options defined in the configuration file.
 *	opt_count :      The number of elements in the opts array.
 *
 * Return value:
 *	Return 0 on success
 *	Return >0 on failure.
 */
int mosquitto_auth_plugin_init(void **user_data, struct mosquitto_opt *opts, int opt_count)
{
  // we don't need to clean up this as this will die with the mosquitto broker.
  struct mosqauth_aux* aux = malloc(sizeof(struct mosqauth_aux));
  aux->socket_fd = -1;
  aux->pid = -1;
  *user_data = aux;
  mosquitto_log_printf(MOSQ_LOG_DEBUG, "init the auth plugin with a socket struct\n");
  return 0;
}

/*
 * Function: mosquitto_auth_plugin_cleanup
 *
 * Called when the broker is shutting down. This will only ever be called once
 * per plugin.
 * Note that <mosquitto_auth_security_cleanup> will be called directly before
 * this function.
 *
 * Parameters:
 *
 *	user_data :      The pointer provided in <mosquitto_auth_plugin_init>.
 *	opts :           Pointer to an array of struct mosquitto_opt, which
 *	                 provides the plugin options defined in the configuration file.
 *	opt_count :      The number of elements in the opts array.
 *
 * Return value:
 *	Return 0 on success
 *	Return >0 on failure.
 */
int mosquitto_auth_plugin_cleanup(void *user_data, struct mosquitto_opt *opts, int opt_count)
{
  if (user_data != NULL) {
    free(user_data);
  }
  mosquitto_log_printf(MOSQ_LOG_DEBUG, "cleanup the auth plugin with socket\n");
  return 0;
}

/*
 * Function: mosquitto_auth_security_init
 *
 * This function is called in two scenarios:
 *
 * 1. When the broker starts up.
 * 2. If the broker is requested to reload its configuration whilst running. In
 *    this case, <mosquitto_auth_security_cleanup> will be called first, then
 *    this function will be called.  In this situation, the reload parameter
 *    will be true.
 *
 * Parameters:
 *
 *	user_data :      The pointer provided in <mosquitto_auth_plugin_init>.
 *	opts :           Pointer to an array of struct mosquitto_opt, which
 *	                 provides the plugin options defined in the configuration file.
 *	opt_count :      The number of elements in the opts array.
 *	reload :         If set to false, this is the first time the function has
 *	                 been called. If true, the broker has received a signal
 *	                 asking to reload its configuration.
 *
 * Return value:
 *	Return 0 on success
 *	Return >0 on failure.
 */
int mosquitto_auth_security_init(void *user_data, struct mosquitto_opt *opts, int opt_count, bool reload)
{
  struct mosqauth_aux* aux = user_data;
  if (aux->socket_fd != -1) {
    // try to disconnect and ignores what can be wrong
    mosqauth_close_socket(aux->socket_fd);
    aux->socket_fd = -1;
  }
  int remaining_retries = 10;
  int fd = -1;
  while (remaining_retries-- > 0) {
    fd = mosqauth_connect_socket(mosqauth_socket_path);
    if (fd == -1) {
      sleep((10 - remaining_retries) * 2); // wait for a few second to try connect again
    } else {
      break;
    }
  }
  if (fd == -1) { // okay, we can't connect to the server
    mosquitto_log_printf(MOSQ_LOG_INFO, "mosquitto_auth_security_init: Server is not responding on %s. Exit.\n", mosqauth_socket_path);
    errno = ENOTCONN;
    return -1; // returns -1 should exit the mosquitto
  }

  aux->socket_fd = fd;
  aux->pid = getpid();
  mosquitto_log_printf(MOSQ_LOG_INFO, "mosquitto_auth_security_init: Connects to %s\n", mosqauth_socket_path);

  mosqauth_request_meta(aux->socket_fd, aux->pid);
  int ret = mosqauth_receive_ok(aux->socket_fd);
  return ret < 0 ? -ret : 0; // returns errors should exit the mosquitto
}

/* 
 * Function: mosquitto_auth_security_cleanup
 *
 * This function is called in two scenarios:
 *
 * 1. When the broker is shutting down.
 * 2. If the broker is requested to reload its configuration whilst running. In
 *    this case, this function will be called, followed by
 *    <mosquitto_auth_security_init>. In this situation, the reload parameter
 *    will be true.
 *
 * Parameters:
 *
 *	user_data :      The pointer provided in <mosquitto_auth_plugin_init>.
 *	opts :           Pointer to an array of struct mosquitto_opt, which
 *	                 provides the plugin options defined in the configuration file.
 *	opt_count :      The number of elements in the opts array.
 *	reload :         If set to false, this is the first time the function has
 *	                 been called. If true, the broker has received a signal
 *	                 asking to reload its configuration.
 *
 * Return value:
 *	Return 0 on success
 *	Return >0 on failure.
 */
int mosquitto_auth_security_cleanup(void *user_data, struct mosquitto_opt *opts, int opt_count, bool reload)
{
  struct mosqauth_aux* aux = user_data;
  mosqauth_close_socket(aux->socket_fd);
  aux->socket_fd = -1;
  aux->pid = -1;
  mosquitto_log_printf(MOSQ_LOG_DEBUG, "security_cleanup: cleans user_data\n");

  return 0;
}

/*
 * Function: mosquitto_auth_acl_check
 *
 * Called by the broker when topic access must be checked. access will be one
 * of:
 *  MOSQ_ACL_SUBSCRIBE when a client is asking to subscribe to a topic string.
 *                     This differs from MOSQ_ACL_READ in that it allows you to
 *                     deny access to topic strings rather than by pattern. For
 *                     example, you may use MOSQ_ACL_SUBSCRIBE to deny
 *                     subscriptions to '#', but allow all topics in
 *                     MOSQ_ACL_READ. This allows clients to subscribe to any
 *                     topic they want, but not discover what topics are in use
 *                     on the server.
 *  MOSQ_ACL_READ      when a message is about to be sent to a client (i.e. whether
 *                     it can read that topic or not).
 *  MOSQ_ACL_WRITE     when a message has been received from a client (i.e. whether
 *                     it can write to that topic or not).
 *
 * Return:
 *	MOSQ_ERR_SUCCESS if access was granted.
 *	MOSQ_ERR_ACL_DENIED if access was not granted.
 *	MOSQ_ERR_UNKNOWN for an application specific error.
 *	MOSQ_ERR_PLUGIN_DEFER if your plugin does not wish to handle this check.
 */
int mosquitto_auth_acl_check(void *user_data, int access, struct mosquitto *client, const struct mosquitto_acl_msg *msg)
{
  mosquitto_log_printf(MOSQ_LOG_DEBUG, "access: type %d\n", access);
  return MOSQ_ERR_SUCCESS;
}

/*
 * Called by the broker when a client connects to a listener using TLS/PSK.
 * This is used to retrieve the pre-shared-key associated with a client
 * identity.
 *
 * Examine hint and identity to determine the required PSK (which must be a
 * hexadecimal string with no leading "0x") and copy this string into key.
 *
 * Parameters:
 *	user_data :   the pointer provided in <mosquitto_auth_plugin_init>.
 *	hint :        the psk_hint for the listener the client is connecting to.
 *	identity :    the identity string provided by the client
 *	key :         a string where the hex PSK should be copied
 *	max_key_len : the size of key
 *
 * Return value:
 *	Return 0 on success.
 *	Return >0 on failure.
 *	Return MOSQ_ERR_PLUGIN_DEFER if your plugin does not wish to handle this check.
 * ------
 * On success, key must be populated for verification.
 * The function doesn't return until the connection times out (for example, on error).
 * Key validation is handled by OpenSSL so key should not exceed max_key_len.
 * Or it's an error for OpenSSL (On ubuntu 18.04, OpenSSL 1.1.1  11 Sep 2018, it's 1032).
 */
int mosquitto_auth_psk_key_get(void *user_data, struct mosquitto *client, const char *hint, const char *identity, char *key, int max_key_len)
{
  struct mosqauth_aux* aux = user_data;
  mosquitto_log_printf(MOSQ_LOG_DEBUG, "psk request: id %s max_key_len %d\n", identity, max_key_len);

  int ret = mosqauth_request_psk(aux->socket_fd, identity);
  if (ret == -1) {
    return 1; // request failed
  }

  ret = mosqauth_receive(aux->socket_fd, key, max_key_len);
  if (ret == -1) {
    return 2; // received failed
  }
  mosquitto_log_printf(MOSQ_LOG_DEBUG, "key returned val: %d\n", ret);

  if (strncmp(key, "FAIL", 4) == 0) {
    return 3; // server returns fail
  }

  return 0;
}
