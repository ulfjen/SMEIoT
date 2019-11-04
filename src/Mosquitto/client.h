#pragma once

#ifndef MOSQUITTO_H_
#define MOSQUITTO_H_

#include <mosquitto.h>

#ifdef _WIN32
#    ifdef LIBRARY_EXPORTS
#        define LIBRARY_API __declspec(dllexport)
#    else
#        define LIBRARY_API __declspec(dllimport)
#    endif
#elif
#    define LIBRARY_API
#endif


typedef void (*CONNECT_CALLBACK)(int);
typedef void (*MESSAGE_CALLBACK)(const struct mosquitto_message*);

// return value 0 represents success. Or ENOMEM, EINVAL when error.
LIBRARY_API int mosq_init();
LIBRARY_API int mosq_set_tls_psk(char* psk, char* identity, char* ciphers);
LIBRARY_API void mosq_set_callback(CONNECT_CALLBACK connect_callback, MESSAGE_CALLBACK message_callback);
LIBRARY_API int mosq_connect(char* host, int port, int keepalive);
LIBRARY_API int mosq_subscribe_topic(char* topic);
LIBRARY_API void mosq_runloop(int timeout, int max_packets, int sleep_on_reconnect);
LIBRARY_API int mosq_destroy();

#endif // MOSQUITTO_H_
