#!/bin/bash

ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )/.." >/dev/null 2>&1 && pwd )"
echo "Starting at ROOT: $ROOT"

JSROOT="$( cd "$ROOT/src/SMEIoT.Web/ClientApp" >/dev/null 2>&1 && pwd )"
echo "Setting JSROOT: $JSROOT"

APIDOCDIR="$( cd "$JSROOT/../ClientApiSdkGen" >/dev/null 2>&1 && pwd )"
OPENAPIDIR="$ROOT/../openapi-generator"
cd $APIDOCDIR && curl -O --insecure https://localhost:5001/swagger/v1/swagger.json > swagger.json
cd $JSROOT/vendor/smeiot-client && \
  java -jar $OPENAPIDIR/modules/openapi-generator-cli/target/openapi-generator-cli.jar generate \
    -c $APIDOCDIR/config.json \
    -i $APIDOCDIR/swagger.json \
    -g typescript-fetch && \
  npm run prepare
cd $JSROOT && rm -rf node_modules/smeiot-client
cd $JSROOT && npm install
