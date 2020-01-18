#!/bin/bash

set -ex

REPO_ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )/.." >/dev/null 2>&1 && pwd )"

cd $REPO_ROOT && echo "Operating at root dir: $REPO_ROOT"
source scripts/bootstrap.sh

mkdir -p $TMP_BOOTSTRAP_DIR
sudo apt-get install -y build-essential \
    libc-ares-dev \
    cmake
install_dotnet_ef
setup_test_db
build_mosquitto
cd $REPO_ROOT && mkdir -p out && cd out && cmake .. -DCMAKE_BUILD_TYPE=Debug && make -j$(nproc)  && sudo make install
cd $REPO_ROOT/src/ClientApp/vendor/smeiot-client && npm install && npm run build
cd $REPO_ROOT/src/ClientApp && npm install && npm run build && cp -r build $REPO_ROOT/src/SMEIoT.Web/wwwroot
