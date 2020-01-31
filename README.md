# SMEIoT

[![Build Status](https://travis-ci.com/ulfjen/SMEIoT.svg?branch=master)](https://travis-ci.com/ulfjen/SMEIoT)

SMEIoT offers a web interface to manage IoT sensors that send messages to a MQTT broker on a Raspberry Pi 4
.

## Research

Deliverable related to the Vinnova project "Säkra och Uppkopplade Smarta Ventilationssystem med Radonmätningar".

## Features

- Secure communication with PSK support
- Sensor lifecycle management
- Sensor detection and debug messages
- Stores your sensor data
- Sensor permission

<img alt="Visualizing sensor" src="https://raw.githubusercontent.com/ulfjen/SMEIoT/master/docs/sensor.png" width="720px"/>
<img alt="Sensor management" src="https://raw.githubusercontent.com/ulfjen/SMEIoT/master/docs/manage.png" width="720px"/>
<img alt="Adding sensor wizard" src="https://raw.githubusercontent.com/ulfjen/SMEIoT/master/docs/wizard.gif" width="720px"/>

We use Mosquitto as our broker.

## Setup

Download the packages in the [Releases](https://github.com/ulfjen/SMEIoT/releases).
Then put them under /tmp.

```
cd /tmp && sudo mkdir -p /tmp/smeiot_build && sudo tar xf /tmp/smeiot-config.tar.gz -C /tmp/smeiot_build
bash -c 'source /tmp/smeiot_build/scripts/bootstrap.sh; build_smeiot_with_remote_tars'
```

The setup script should guide you.

MQTT client code sample are available under [samples](https://github.com/ulfjen/SMEIoT/tree/master/samples/mosquitto-device-client) examples. Follows the instruction, take device name (which we uses for PSK identity) and PSK from our wizard on the dashboard.

## Maintenance

- First registered user will be the admin. Don't messed up the admin permission otherwise you lose the access to manage the system.
- Backup your database regularly for data safety.
`pg_dump smeiot > /path/to/dump`
- Setup cron job to renew license
You can trigger a restart to the server by `sudo systemctl restart smeiot`
- In case of emergency with mosquitto
Check its log with `journalctl -au mosquitto` or try to restart the server `sudo systemctl restart smeiot`.
SMEIoT will try to get mosquitto running.
- Secure your ports
The firewall configuration is on you. 80 (HTTP), 443 (HTTPS), 4588(MQTT) must be open to run the server. Usually you need 22 for SSH.
