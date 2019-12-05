
/*
* Libmosquitto TLS-PSK example
*
* 
* Compile:
* gcc pi.c -o pi -lmosquitto
*/

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
#include <mosquitto.h>

static int run = 1;

void connect_callback(struct mosquitto *mosq, void *obj, int result)
{
	printf("connect callback, rc=%d\n", result);
}

void message_callback(struct mosquitto *mosq, void *obj, const struct mosquitto_message *message)
{
	printf("got message '%.*s' for topic '%s'\n", message->payloadlen, (char*) message->payload, message->topic);
}

int main(int argc, char *argv[])
{
	uint8_t reconnect = true;

	struct mosquitto *mosq;
	int rc = 0;
	time_t t;

	// Init lib
	mosquitto_lib_init();

	// Make new instance
	mosq = mosquitto_new(NULL, true, NULL);

	// Set callbacks
	mosquitto_connect_callback_set(mosq, connect_callback);
	mosquitto_message_callback_set(mosq, message_callback);

	// Set PSK and identity
	mosquitto_tls_psk_set(mosq,"<psk>","<identity>",NULL);

	// Connect to broker
	rc = mosquitto_connect(mosq, "<server>", -1, 60);
	srand((unsigned) time(&t));

	while(run) {
		int num = rand() % 50;
		char number[100];
		int len = snprintf(number, 100, "%d", num);

		int ret = mosquitto_publish(mosq, NULL, "<topic>", len, &number, 0, false);
		if (ret == MOSQ_ERR_NO_CONN) {
			sleep(5);
			mosquitto_reconnect(mosq);
		} else if (ret == MOSQ_ERR_SUCCESS) {
			sleep(10);
		} else {
			printf("error encountered %d\n", ret);
			sleep(10);
		}
	}
	// Free memory
  	mosquitto_destroy(mosq);

	// Deinit lib
	mosquitto_lib_cleanup();

	return rc;
}
