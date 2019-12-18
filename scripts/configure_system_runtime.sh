#!/bin/bash

# prepare the system for environment and users
# 1. Add smeiot:smeiot user and group.
# 2. Install the software package we needed (they may break over time)

set -ex

# disable ssh login
sudo useradd --no-create-home --shell /bin/false --groups smeiot smeiot

sudo apt-get install liblttng-ust0 && \
    libcurl4 && \
    libssl1.0.2 && \
    libkrb5-3 && \
    libicu63 && \
    libssl-dev && \
    build-essential && \
    libwrap0-dev && \
    lib-ares-dev && \
    uuid-dev