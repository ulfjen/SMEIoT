#!/bin/bash

# The script runs on a workstation and connect via ssh to a Pi and set it up

set -ex

REPO_ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )/.." >/dev/null 2>&1 && pwd )"
REMOTE_USER=root

cd $REPO_ROOT && echo "Operating at root dir: $REPO_ROOT"
source scripts/bootstrap.sh

build_smeiot
package_tars

echo "==============
Do the following:
  rsync --info=progress2 $BUILD_DIR/*.tar.gz <remote>:/tmp/

Then ssh into the <remote> for deployment, do:

  cd /tmp && sudo mkdir -p /tmp/smeiot_build && sudo tar xf /tmp/smeiot-config.tar.gz -C /tmp/smeiot_build
  bash -c 'source /tmp/smeiot_build/scripts/bootstrap.sh; build_smeiot_with_remote_tars'
"

# export ASPNETCORE_ENVIRONMENT=Staging before the function call to build a staging environment
# bash -c 'source /tmp/smeiot_build/scripts/bootstrap.sh; export ASPNETCORE_ENVIRONMENT=Staging; build_smeiot_with_remote_tars'
