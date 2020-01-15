#!/bin/bash

set -ex

REPO_ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )/.." >/dev/null 2>&1 && pwd )"

cd $REPO_ROOT && echo "Operating at root dir: $REPO_ROOT"
source scripts/bootstrap.sh

mkdir -p $TMP_BOOTSTRAP_DIR
sudo apt-get install -y build-essential \
    libc-ares-dev \
    cmake
build_mosquitto
cd $REPO_ROOT && mkdir -p out && cd out && cmake .. -DCMAKE_BUILD_TYPE=Debug && make -j$(nproc)  && sudo make install
