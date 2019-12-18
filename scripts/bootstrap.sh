#!/bin/bash

# prepare the system for environment and users
# 1. Add smeiot:smeiot user and group.
# 2. Install the software package we needed (they may break over time)
# 3. setup mosquitto
# 4. get webserver ready
# 5. place systemd unit files and configurations in place

set -ex

MOSQUITTO_VER=1.6.8
MOSQUITTO_SHA256='7df23c81ca37f0e070574fe74414403cf25183016433d07add6134366fb45df6'
TMP_BOOTSTRAP_DIR=/tmp/smeiot_build

SMEIOT_ROOT=/var/SMEIoT
ARCH=linux-arm
SERVER_CONFIG=Release
CLIENT_CONFIG=Release

WEB_ROOT=$REPO_ROOT/src/SMEIoT.Web
BUILD_DIR=$WEB_ROOT/bin/$SERVER_CONFIG/netcoreapp3.1/$ARCH/publish

function create_user {
  # disable ssh login
  sudo useradd --no-create-home --shell /bin/false --groups smeiot smeiot
}

function prepare_syslibs {
  sudo apt-get update && \
  sudo apt-get upgrade && \
  sudo apt-get install -y liblttng-ust0 \
      libcurl4 \
      libssl1.0.2 \
      libkrb5-3 \
      libicu63 \
      libssl-dev \
      build-essential \
      libwrap0-dev \
      libc-ares-dev \
      uuid-dev \
      cmake \
      postgresql && \
  sudo apt-get clean && \
	sudo apt-get autoremove
}

function build_mosquitto {
  cd $TMP_BOOTSTRAP_DIR
  curl -O -L -C - https://mosquitto.org/files/source/mosquitto-$MOSQUITTO_VER.tar.gz
  cd $TMP_BOOTSTRAP_DIR && echo "$MOSQUITTO_SHA256 mosquitto-$MOSQUITTO_VER.tar.gz" | sha256sum -c - && echo "verified mosquitto source package."
  tar xf mosquitto-$MOSQUITTO_VER.tar.gz
  cd mosquitto-$MOSQUITTO_VER
  make -j$(nproc) WITH_SRV=yes WITH_WEBSOCKETS=no WITH_DOCS=no
  sudo make install
}

# We have custom plugins that needed to be configured, but after mosquitto is built and installed
function build_mosquitto_plugins {
  cd /tmp && sudo tar xf smeiot-mosquitto-plugins.tar.gz -C $TMP_BOOTSTRAP_DIR
  mkdir -p $TMP_BOOTSTRAP_DIR/out && cd $TMP_BOOTSTRAP_DIR/out && cmake .. -DCMAKE_BUILD_TYPE=Release && make -j$(nproc)  && sudo make install
}

# we rely on Self-contained deployment that contains .NET itself with us
# so we don't have extra binaries.
#
# command:
#   dotnet publish -c Release -r <RID> --self-contained true
# for Raspbian armv7l (32 bit):
#   dotnet publish -c Release -r linux-arm --self-contained true
function build_smeiot {
  echo " * * * * * * * * * * * * * * * * * * *"
  echo "Building SMEIoT for $ARCH"
  echo ""
  echo "REPO_ROOT=$REPO_ROOT"
  echo "WEB_ROOT=$WEB_ROOT"
  echo "BUILD_DIR=$BUILD_DIR"
  echo ""
  echo ""
  echo "--------------------------------------"

  cd $WEB_ROOT && rm -rf $BUILD_DIR && dotnet publish -c $SERVER_CONFIG -r $ARCH --self-contained true
  cd $WEB_ROOT/ClientApp && npm run build && cp -r build/static/* $BUILD_DIR/wwwroot
}

function prepare_smeiot_migration_script {
  cd $WEB_ROOT && \
      echo "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";" > $BUILD_DIR/db_migrate.sql && \
      dotnet ef migrations script > $BUILD_DIR/db_migrate.sql
}

function setup_db {
  cd $SMEIOT_ROOT
  sudo systemdctl start postgresql && sudo systemdctl enable postgresql
  dropdb smeiot
  su postgres -c 'createdb smeiot -T template1 -U smeiot'
  su postgres -c 'psql smeiot < db_migrate.sql'
}

function setup_smeiot_with_tar {
  sudo mkdir -p $SMEIOT_ROOT
  sudo tar xf $TMP_BOOTSTRAP_DIR/smeiot.tar.gz -C $SMEIOT_ROOT
  sudo chmod -R smeiot:smeiot $SMEIOT_ROOT
}

function package_tars {
  cd $BUILD_DIR && tar caf smeiot.tar.gz *
  cd $REPO_ROOT/samples && tar caf smeiot-config.tar.gz *.sample $REPO_ROOT/scripts/bootstrap.sh && mv smeiot-config.tar.gz $BUILD_DIR
  cd $REPO_ROOT && tar caf smeiot-mosquitto-plugins.tar.gz src/Mosquitto* CMake* && mv smeiot-mosquitto-plugins.tar.gz $BUILD_DIR
}

function configure_system {
  cd /tmp && sudo tar xf smeiot-config.tar.gz -C $TMP_BOOTSTRAP_DIR
  sudo mkdir -p /etc/systemd/system/ && \
  sudo mkdir -p /etc/mosquitto/ && \
  sudo mv $TMP_BOOTSTRAP_DIR/mosquitto.conf.sample /etc/mosquitto/mosquitto.conf && \
  sudo mv $TMP_BOOTSTRAP_DIR/smeiot.service.sample /etc/systemd/system/smeiot.service && \
  sudo mv $TMP_BOOTSTRAP_DIR/mosquitto.service.sample /etc/systemd/system/mosquitto.service && \
  sudo mv $TMP_BOOTSTRAP_DIR/sysctl.conf.sample /etc/sysctl.d/smeiot-sysctl.conf && \
  sudo chmod 644 /etc/mosquitto/mosquitto.conf && \
  sudo chmod 644 /etc/systemd/system/smeiot.service && \
  sudo chmod 644 /etc/systemd/system/mosquitto.service && \
  sudo chmod 644 /etc/sysctl.d/smeiot-sysctl.conf && \
  sudo sysctl -p
  sudo systemctl daemon-reload
  sudo systemctl start smeiot && \
  sudo systemctl start mosquitto && \
  sudo systemctl enable smeiot && \
  sudo systemctl enable mosquitto
}

function build_smeiot_with_remote_tars {
  sudo mkdir -p $TMP_BOOTSTRAP_DIR
  create_user()
  prepare_syslibs()
  build_mosquitto()
  setup_smeiot_with_tar()
  build_mosquitto_plugins()
  setup_db()
  configure_system()
  sudo rm -rf $TMP_BOOTSTRAP_DIR
}
