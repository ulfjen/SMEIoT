#pragma once

#ifndef MOSQUITTO_PLUGIN_AUTH_H_
#define MOSQUITTO_PLUGIN_AUTH_H_

#include <mosquitto.h>
#include <mosquitto_plugin.h>
#include <mosquitto_broker.h>

#ifdef _WIN32
#    ifdef LIBRARY_EXPORTS
#        define LIBRARY_API __declspec(dllexport)
#    else
#        define LIBRARY_API __declspec(dllimport)
#    endif
#else
#    define LIBRARY_API
#endif

struct mosqauth_aux {
  int socket_fd;
};

const char* mosqauth_socket_path = "/tmp/smeiot.auth.broker";

#endif // MOSQUITTO_PLUGIN_AUTH_H_
