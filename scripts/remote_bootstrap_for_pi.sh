#!/bin/bash

# The script runs on a workstation and connect via ssh to a Pi and set it up

set -ex

REPO_ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )/.." >/dev/null 2>&1 && pwd )"
REMOTE_USER=root

cd $REPO_ROOT && echo "Operating at root dir: $REPO_ROOT"
source scripts/bootstrap.sh

build_smeiot
prepare_smeiot_migration_script
package_tars

echo << EOF
Do the following:
  scp $BUILD_DIR/*.tar.gz <remote>:/tmp/

Then ssh into the <remote>, do:

  cd /tmp
  sudo mkdir -p /tmp/smeiot_build
  sudo tar xf /tmp/smeiot-config.tar.gz -C /tmp/smeiot_build
  sh -c 'source /tmp/smeiot_build/bootstrap.sh; build_smeiot_with_remote_tars()'
EOF
