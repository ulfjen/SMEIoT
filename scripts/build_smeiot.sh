#!/bin/bash

# we rely on Self-contained deployment that contains .NET itself with us
# so we don't have extra binaries.
#
# command:
#   dotnet publish -c Release -r <RID> --self-contained true
# for Raspbian armv7l (32 bit):
#   dotnet publish -c Release -r linux-arm --self-contained true

set -ex

ARCH=linux-arm
SERVER_CONFIG=Release
CLIENT_CONFIG=Release

REPO_ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )/.." >/dev/null 2>&1 && pwd )"

WEB_ROOT=$REPO_ROOT/src/SMEIoT.Web
BUILD_DIR=$WEB_ROOT/bin/$SERVER_CONFIG/netcoreapp3.1/$ARCH/publish

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
cd $WEB_ROOT && \
    echo "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";" > $BUILD_DIR/db_migrate.sql && \
    dotnet ef migrations script > $BUILD_DIR/db_migrate.sql

cd $BUILD_DIR && tar caf smeiot.tar.gz *
ls -alh $BUILD_DIR/smeiot.tar.gz