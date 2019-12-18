#!/bin/bash

# Setup the database accordingly

set -e

ENV=production
SMEIOT_ROOT=/var/SMEIOT_ROOT

cd $SMEIOT_ROOT

dropdb smeiot
su postgres createdb smeiot -T template1 -U smeiot
su postgres psql smeiot < db_migrate.sql 