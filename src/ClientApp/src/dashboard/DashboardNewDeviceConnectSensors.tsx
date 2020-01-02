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
import { useTitle } from 'react-use';
import {
  DevicesApi,
  DeviceApiModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import DashboardNewDeviceFrame from "./DashboardNewDeviceFrame";
import extractParamFromQuery from "../helpers/extractParamFromQuery";
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";

const styles = ({ spacing }: Theme) =>
  createStyles({
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    loadingPanel: {
      height: 200
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

  const [loading, setLoading] = React.useState<boolean>(true);
  const [loadingError, setLoadingError] = React.useState<boolean>(false);
  const [device, setDevice] = React.useState<DeviceApiModel>({});

  React.useEffect(() => {
    (async () => {
      const name = extractParamFromQuery(location);
      if (name === null) {
        setLoadingError(true);
        return;
      }

      setLoading(true);
      const api = new DevicesApi(GetDefaultApiConfig());
      var res = await api.apiDevicesNameGet({
        name
      });
      if (res !== null) {
        setDevice(res);
      } else {
        setLoadingError(true);
      }
      setLoading(false);
    })();
  }, []);

  return (
    <DashboardNewDeviceFrame activeStep={2}>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          {loading ? (
            <Skeleton variant="rect" className={classes.loadingPanel} />
          ) : (
            loadingError ?
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
              />) :
              <div>
                <p>
                  <FormattedMessage
                    id="dashboard.devices.new.step3.notice"
                    description="Notice related when we add new sensors"
                    defaultMessage="The device is now installed. You can start to connect sensors. You can also add sensors in the device details page later."
                  />
                </p>
                <div>
                  <Button variant="contained" color="primary" component={Link} to="/dashboard/devices">
                    <FormattedMessage
                      id="dashboard.devices.new.control.finish"
                      description="The button text for going to finish adding"
                      defaultMessage="Finish"
                    />
                  </Button>
                  <Button component={Link} to="/dashboard/devices/new">
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
