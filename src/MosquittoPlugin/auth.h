#pragma once

#ifndef MOSQUITTO_PLUGIN_AUTH_H_
#define MOSQUITTO_PLUGIN_AUTH_H_

#include <mosquitto.h>
#include <mosquitto_plugin.h>
#include <mosquitto_broker.h>

struct mosqauth_aux {
  int socket_fd;
  int pid;
};

#ifdef ENABLE_DEBUG_MACRO
const char* mosqauth_socket_path = "/tmp/smeiot.auth.broker";
#else
const char* mosqauth_socket_path = "/var/SMEIoT/run/smeiot.auth.broker";
#endif

#endif // MOSQUITTO_PLUGIN_AUTH_H_
