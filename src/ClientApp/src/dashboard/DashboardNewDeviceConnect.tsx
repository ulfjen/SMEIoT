import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import Skeleton from "@material-ui/lab/Skeleton";
import { Link as ReachLink, RouteComponentProps } from "@reach/router";
import {
  DevicesApi, BasicDeviceApiModelFromJSON
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import DashboardNewDeviceFrame from "./DashboardNewDeviceFrame";
import extractParamFromQuery from "../helpers/extractParamFromQuery";
import BlockCode from "../components/BlockCode";
import LineCode from "../components/LineCode";
import { useAsync, useTitle } from 'react-use';
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import useInterval from "../helpers/useInterval";

const styles = ({ spacing }: Theme) =>
  createStyles({
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    blockCode: {
      whiteSpace: "pre-wrap",
      wordWrap: "break-word"
    },
    actions: {
      marginTop: spacing(1)
    }
  });

export interface IDashboardNewDeviceConnectProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.new.step2.title",
    description: "Used as title in the new device page on the dashboard",
    defaultMessage: "Connect with a new device"
  }
});

const _DashboardNewDeviceConnect: React.FunctionComponent<IDashboardNewDeviceConnectProps> = ({
  classes,
  location,
  navigate
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const api = new DevicesApi(GetDefaultApiConfig());
  const name = extractParamFromQuery(location);
  if (name === null) {
    throw new Error("Device name is required.");
  }
  const [connected, setConnected] = React.useState<boolean>(false);

  const state = useAsync(async () => {
    const res = await api.apiDevicesNameBasicGet({
      name
    });
    setConnected(res.connected || false);

    return res;
  }, [name]);

  const device = state.value || BasicDeviceApiModelFromJSON({});
  
  useInterval(async () => {
    if (!state.loading && state.error === undefined) {

      const res = await api.apiDevicesNameBasicGet({
        name
      });
      setConnected(res.connected || false);
    }
  }, 3000);

  return (
    <DashboardNewDeviceFrame activeStep={1}>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
        {state.loading ? (
            <div><Skeleton variant="text"/><Skeleton variant="text"/><Skeleton variant="text"/></div>
          ) : (
            <div>

            {state.error ?
              (device.name ? 
              <FormattedMessage
                id="dashboard.devices.new.errors.loading_error_without_name"
                description="Error related when we wait for new connection"
                defaultMessage="We can not find your device. Please try to add your new device again."
              /> :
              <FormattedMessage
                id="dashboard.devices.new.errors.loading_error"
                description="Error related when we wait for new connection"
                defaultMessage="We can not find your device {name}. Please try later."
                values={{
                  name: device.name
                }}
              />)
             : 
              <FormattedMessage
                id="dashboard.devices.new.step2.notice"
                description="Notice related when we wait for new connection"
                defaultMessage="Now you can copy the key to your device and start to connect with the broker.
                  Once we receive a new message from the broker, we will prompt you to continue.
                  Your device's name is {name} (PSK identity).
                  Your device's key (PSK key) is shown below. Now the key has {bytes} ({chars} characters).
                  We know that ESP32 only supports 32 bytes (64 characters) key.
                  {code}
                  Please send messages via MQTT to {topic} at {host}:{port}."
                values={{
                  name: <LineCode>{device.name}</LineCode>,
                  code: <BlockCode>{device.preSharedKey}</BlockCode>,
                  topic: <LineCode>{`${device.mqttTopicPrefix}${device.name}/<any_sensor_name>`}</LineCode>,
                  host: <LineCode>{device.mqttHost}</LineCode>,
                  port: <LineCode>{device.mqttPort}</LineCode>,
                  bytes: <b>{device.preSharedKey.length / 2} bytes</b>, 
                  chars: device.preSharedKey.length
                }}
              />
            }
            <div className={classes.actions}>
              <Button
                component={ReachLink}
                to={`../configure_sensors?name=${device.name}`}
                variant="contained"
                color="primary"
                disabled={!connected}
              >
                <FormattedMessage
                  id="dashboard.devices.new.control.next"
                  description="The button text for going to next page"
                  defaultMessage="Next"
                />
              </Button>
            </div>
          </div>
          )}

        </Paper>
      </Grid>
    </DashboardNewDeviceFrame>
  );
};

const DashboardNewDeviceConnect = withStyles(styles)(
  _DashboardNewDeviceConnect
);

export default DashboardNewDeviceConnect;
