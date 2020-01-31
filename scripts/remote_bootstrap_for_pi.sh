#!/bin/bash

# The script runs on a workstation and then one can connect via ssh to a Pi and set it up
# Dependencies like dotnet, dotnet-ef, node, npm, tsc (typescript) must be installed 

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

If you are upgrading to a new version, change the second line with:
  bash -c 'source /tmp/smeiot_build/scripts/bootstrap.sh; upgrade_smeiot'
"

# To clear the existing project and data
#   bash -c 'source /tmp/smeiot_build/scripts/bootstrap.sh; nuke_smeiot'
