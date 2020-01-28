
/*
* Stress test the SMEIoT platform, TLS-PSK example
*
* Ulf Jennehag
* Compile:
* gcc stresstest.c -o stresstest -lmosquitto
*/

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdint.h>
#include <string.h>
#include <sys/time.h>
#include <mosquitto.h>

static int run = 1;

unsigned long getTimeMs()
{
  struct timeval tv;
  gettimeofday(&tv,NULL); // Get current time
  return (unsigned long)(tv.tv_sec)*1000 + (unsigned long)(tv.tv_usec)/1000; // Return seconds*1000 + useconds/1000
}

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
	// argv[1] = ms delay between publlish
	//unsigned long delayTimer=0;
	//unsigned long sendInterval = 10; // Delay between publish in ms
	int counter = 0;
	int delay = atoi(argv[1]);
	int numberofpublish = atoi(argv[2]);

	uint8_t reconnect = true;

	struct mosquitto *mosq;
	int rc = 0;

	// Init lib
	mosquitto_lib_init();

	// Make new instance
	mosq = mosquitto_new(NULL, true, NULL);

	// Set callbacks
	mosquitto_connect_callback_set(mosq, connect_callback);
	mosquitto_message_callback_set(mosq, message_callback);

	// Set PSK and identity
	mosquitto_tls_psk_set(mosq,"CF3100249E6CAAC5BD1EE1074583429952553BD7EC660A66C785E1451F37262840DB888E3C0F845A3BF48455D7D4633FB82C7CB0865C86733EE7589F16969089","stresstest",NULL);

	// Connect to broker
	rc = mosquitto_connect(mosq, "193.10.119.35", 4588, 60);

	// Subscribe to topic
	//mosquitto_subscribe(mosq, NULL, "iot/stresstest2/#", 0);

	//delayTimer = getTimeMs();


	//rc = mosquitto_loop(mosq, -1, 1);

	while(run){
		// run message loop
		rc = mosquitto_loop(mosq, -1, 1);
		if(run && rc){
			printf("connection error!\n");
			usleep(10*1000*1000);
			mosquitto_reconnect(mosq);
		}

		if (counter>numberofpublish) run=0;

		// Delay according to the constraints
		//if ((getTimeMs()-delayTimer)>sendInterval)
		//{
			char number[100];
			char buffer[50];
			int len = snprintf(number, 100, "%lu", getTimeMs());
			//printf("Publish: iot/stresstest/%s%dus -m %s\n",argv[3],delay,number);
			sprintf(buffer,"iot/stresstest/%s%dus",argv[3],delay);
			mosquitto_publish(mosq, NULL, buffer, len, &number, 0, false);

			counter++;

			//delayTimer=getTimeMs();
			usleep(delay);
		//}


	}

	//usleep(5*1000*1000);
	// Free memory
  mosquitto_destroy(mosq);

	// Deinit lib
	mosquitto_lib_cleanup();

	return rc;
}
