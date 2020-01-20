/*
* Libmosquitto TLS-PSK example
* 
* Compile:
* gcc pi.c -o pi -lmosquitto
*/

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
#include <mosquitto.h>

const char *device_name = "";
const char *psk = "";
const char *host = "127.0.0.1";
const int port = 4588;
const int reconnect = true;

void connect_callback(struct mosquitto *mosq, void *obj, int result)
{
	printf("connect callback, rc=%d\n", result);
}

void message_callback(struct mosquitto *mosq, void *obj, const struct mosquitto_message *message)
{
	printf("got message '%.*s' for topic '%s'\n", message->payloadlen, (char *)message->payload, message->topic);
}

void copy_topic_buffer(char* buffer, int len_buffer, const char* sensor_name)
{
	strncat(buffer, "iot/", len_buffer);
	strncat(buffer, device_name, len_buffer);
	strncat(buffer, "/", len_buffer);
	strncat(buffer, sensor_name, len_buffer);
}

void reconnect_or_wait(int ret, struct mosquitto *mosq, int reconnect)
{
	if (ret == MOSQ_ERR_NO_CONN && reconnect)
	{
		sleep(5);
		mosquitto_reconnect(mosq);
	}
	else if (ret == MOSQ_ERR_SUCCESS)
	{
		sleep(10);
	}
	else
	{
		printf("error encountered %d\n", ret);
		sleep(10);
	}
}

void prepare_random_num_as_str(char* buffer, int len_buffer)
{
	int num = rand() % 50;
	int len = snprintf(buffer, len_buffer, "%d", num);
}

int main(int argc, char *argv[])
{

	struct mosquitto *mosq;

	// Init lib
	mosquitto_lib_init();

	// Make new instance
	mosq = mosquitto_new(NULL, true, NULL);

	// Set callbacks
	mosquitto_connect_callback_set(mosq, connect_callback);
	mosquitto_message_callback_set(mosq, message_callback);

	// Set PSK and identity
	int psk_ret = mosquitto_tls_psk_set(mosq, psk, device_name, NULL);

	// Connect to broker
	printf("tries to connect %d\n", psk_ret);
	int ret = mosquitto_connect(mosq, host, port, 60);
	if (ret != MOSQ_ERR_SUCCESS)
	{
		exit(1);
	}
	printf("ret: %d\n", ret);
	srand((unsigned)time(&ret));

	const int default_buffer_len = 100;
	while (1)
	{
		puts("sending...");
		char number[default_buffer_len];
		memset(number, 0, sizeof(number));
		prepare_random_num_as_str(number, default_buffer_len);

		char topic_buffer[default_buffer_len];
		memset(topic_buffer, 0, sizeof(topic_buffer));
		copy_topic_buffer(topic_buffer, default_buffer_len, "temp01");

		ret = mosquitto_publish(mosq, NULL, topic_buffer, strlen(number), number, 0, false);

		memset(number, 0, sizeof(number));
		prepare_random_num_as_str(number, default_buffer_len);
		memset(topic_buffer, 0, sizeof(topic_buffer));
		copy_topic_buffer(topic_buffer, default_buffer_len, "temp02");
		ret = mosquitto_publish(mosq, NULL, topic_buffer, strlen(number), number, 0, false);

		memset(number, 0, sizeof(number));
		prepare_random_num_as_str(number, default_buffer_len);
		memset(topic_buffer, 0, sizeof(topic_buffer));
		copy_topic_buffer(topic_buffer, default_buffer_len, "temp03");
		ret = mosquitto_publish(mosq, NULL, topic_buffer, strlen(number), number, 0, false);

		ret = mosquitto_loop(mosq, -1, 1);
		reconnect_or_wait(ret, mosq, reconnect);
	}

	// Free memory
	mosquitto_destroy(mosq);

	// Deinit lib
	mosquitto_lib_cleanup();

	return 0;
}
