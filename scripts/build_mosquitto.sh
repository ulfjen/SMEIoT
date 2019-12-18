#!/bin/bash

set -ex

MOSQUITTO_VER=1.6.8

TMP_DIR=/tmp/mosquitto
mkdir -p $TMP_DIR && cd $TMP_DIR
curl -O -L -C - https://mosquitto.org/files/source/mosquitto-$MOSQUITTO_VER.tar.gz
tar xf mosquitto-$MOSQUITTO_VER.tar.gz
cd mosquitto-$MOSQUITTO_VER
sudo make -j$(nproc) WITH_SRV=yes WITH_WEBSOCKETS=no WITH_DOCS=no