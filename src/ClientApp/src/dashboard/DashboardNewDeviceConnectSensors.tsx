import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import Skeleton from "@material-ui/lab/Skeleton";
import { Link, RouteComponentProps } from "@reach/router";
import { useTitle, useAsync } from 'react-use';
import {
  DevicesApi,
  SensorDetailsApiModelStatusEnum
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import DashboardNewDeviceFrame from "./DashboardNewDeviceFrame";
import extractParamFromQuery from "../helpers/extractParamFromQuery";
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import DashboardDeviceOtherSensors from "./DashboardDeviceOtherSensors";
import useSensorByStatus from "../helpers/useSensorsByStatus";

const styles = ({ spacing }: Theme) =>
  createStyles({
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    actions: {
      marginTop: spacing(1),
      "& > a": {
        marginRight: spacing(1)
      }
    }
  });

export interface IDashboardNewDeviceConnectSensorsProps
  extends RouteComponentProps,
  WithStyles<typeof styles> { }

const messages = defineMessages({
  title: {
    id: "dashboard.devices.new.step3.title",
    description: "Used as title in the connecting device's sensors page on the dashboard",
    defaultMessage: "Connect with sensors for the device"
  }
});

const _DashboardNewDeviceConnectSensors: React.FunctionComponent<IDashboardNewDeviceConnectSensorsProps> = ({
  classes,
  location,
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const api = new DevicesApi(GetDefaultApiConfig());
  const sensors = useSensorByStatus();

  const name = extractParamFromQuery(location);
  if (name === null) {
    throw new Error("Device name is required.");
  }

  const state = useAsync(async () => {
    return await api.apiDevicesNameGet({
      name
    }).then(res => {
      sensors.setNotRegistered(res.sensors.filter(s => s.status === SensorDetailsApiModelStatusEnum.NotRegistered)); 
      sensors.setNotConnected(res.sensors.filter(s => s.status === SensorDetailsApiModelStatusEnum.NotConnected));
      sensors.setRunning(res.sensors.filter(s => s.status === SensorDetailsApiModelStatusEnum.Connected));
      return res;
    });
  }, [name]);
  
  return (
    <DashboardNewDeviceFrame activeStep={2}>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          {state.loading ? (
            <div><Skeleton variant="text"/><Skeleton variant="text"/><Skeleton variant="text"/></div>
            ) : (
            state.error ?
              <FormattedMessage
                id="dashboard.devices.new.errors.something_wrong"
                description="Error related when we wait for new connection"
                defaultMessage="Something is wrong. We can't continue."
              /> :
              <div>
                <FormattedMessage
                  id="dashboard.devices.new.step3.notice"
                  description="Notice related when we add new sensors"
                  defaultMessage="The device is now installed. You can start to connect sensors. You can also add sensors in the device details page later."
                />
                <DashboardDeviceOtherSensors
                  deviceName={state.value ? state.value.name : ""}
                  sensorsToRender={sensors.notRegistered}
                  sensors={sensors}
                />
                <div className={classes.actions}>
                  <Button variant="contained" color="primary" component={Link} to="..">
                    <FormattedMessage
                      id="dashboard.devices.new.control.finish"
                      description="The button text for going to finish adding"
                      defaultMessage="Finish"
                    />
                  </Button>
                  <Button component={Link} to="../new">
                    <FormattedMessage
                      id="dashboard.devices.new.control.create_new"
                      description="Control for adding new device (reset the wizard)"
                      defaultMessage="Connect new"
                    />
                  </Button>
                </div>
              </div>
              )
            }
        </Paper>
      </Grid>
    </DashboardNewDeviceFrame>
  );
};

const DashboardNewDeviceConnectSensors = withStyles(styles)(
  _DashboardNewDeviceConnectSensors
);

export default DashboardNewDeviceConnectSensors;
