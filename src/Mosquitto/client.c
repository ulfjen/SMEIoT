#include "client.h"
#include <stdio.h>
#include <stdlib.h>
#include <errno.h>

#ifdef _WIN32
#include <Windows.h>
#else
#include <unistd.h>
#endif

struct callback_delegates {
    CONNECT_CALLBACK connect_callback_delegate;
    MESSAGE_CALLBACK message_callback_delegate;
};

struct mosquitto* mosq;
int rc;
int run;
struct callback_delegates delegates;

int mosq_init()
{
    mosq = NULL;
    delegates.connect_callback_delegate = NULL;
    delegates.message_callback_delegate = NULL;
    rc = 0;
    run = 1;

    mosquitto_lib_init();
    mosq = mosquitto_new(NULL, 1, &delegates);
    if (mosq == NULL) {
        return errno;
    } else {
        return 0;
    }
}

int mosq_set_tls_psk(char* psk, char* identity, char* ciphers)
{
    return mosquitto_tls_psk_set(mosq, psk, identity, ciphers);
}

void _connect_callback(struct mosquitto* mosq, void* obj, int result)
{
    struct callback_delegates* delegates = obj;
    CONNECT_CALLBACK cb = delegates->connect_callback_delegate;
    printf("connect callback %p invoked with result: %d\n", cb, result);

    cb(result);
}

void _message_callback(struct mosquitto* mosq, void* obj, const struct mosquitto_message* message)
{
    struct callback_delegates* delegates = obj;
    MESSAGE_CALLBACK cb = delegates->message_callback_delegate;
    printf("message callback %p called with a message.\n", cb);

    cb(message->mid, message->topic, message->payload, message->payloadlen, message->qos, message->retain);
}

void mosq_set_callback(CONNECT_CALLBACK connect_callback_delegate, MESSAGE_CALLBACK message_callback_delegate)
{
    delegates.connect_callback_delegate = connect_callback_delegate;
    delegates.message_callback_delegate = message_callback_delegate;
    printf("connect cb delegate %p\n", connect_callback_delegate);
    if (delegates.connect_callback_delegate != NULL) {
        mosquitto_connect_callback_set(mosq, _connect_callback);
    }
    printf("message cb delegate %p\n", message_callback_delegate);
    if (delegates.message_callback_delegate != NULL) {
        mosquitto_message_callback_set(mosq, _message_callback);
    }
}

int mosq_connect(char* host, int port, int keepalive)
{
    printf("connects to %s:%d with keep alive %d\n", host, port, keepalive);
    rc = mosquitto_connect(mosq, host, port, keepalive);
    return rc;
}

void mosq_runloop(int timeout, int max_packets, int sleep_on_reconnect)
{
    while (run) {
        // run message loop
        rc = mosquitto_loop(mosq, timeout, max_packets);
        printf("run loop invoked rc %d\n", rc);
        if (run && rc) {
#ifdef _WIN32
            Sleep(sleep_on_reconnect * 1000); // milliseconds
#else
            sleep(sleep_on_reconnect);
#endif
            mosquitto_reconnect(mosq);
        }
    }
}

int mosq_subscribe_topic(char* topic)
{
    return mosquitto_subscribe(mosq, NULL, topic, 0);
}

int mosq_destroy()
{
    mosquitto_destroy(mosq);
    int res = mosquitto_lib_cleanup();

    delegates.connect_callback_delegate = NULL;
    delegates.message_callback_delegate = NULL;
    run = 0;
    mosq = NULL;
    rc = 0;
    return res;
}
