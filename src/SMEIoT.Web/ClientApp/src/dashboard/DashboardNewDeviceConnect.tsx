import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import Skeleton from "@material-ui/lab/Skeleton";
import { RouteComponentProps } from "@reach/router";
import { Helmet } from "react-helmet";
import {
  DevicesApi,
  DeviceApiModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import DashboardNewDeviceFrame from "./DashboardNewDeviceFrame";
import extractParamFromQuery from "../helpers/extractParamFromQuery";
import BlockCode from "../components/BlockCode";
import LineCode from "../components/LineCode";
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
    },
    blockCode: {
      whiteSpace: "pre-wrap",
      wordWrap: "break-word"
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

  const [loading, setLoading] = React.useState<boolean>(true);
  const [loadingError, setLoadingError] = React.useState<boolean>(false);
  const [device, setDevice] = React.useState<DeviceApiModel>({});
  const [deviceConnected, setDeviceConnected] = React.useState<boolean>(false);

  const handleNext = async () => {
    if (navigate) {
      navigate(`connect_sensors?name=${device.name}`);
    }
  };

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
    <DashboardNewDeviceFrame activeStep={1}>
      <Helmet>
        <title>{intl.formatMessage(messages.title)}</title>
      </Helmet>

      <Grid item xs={12}>
        <Paper className={classes.paper}>
        {loading ? (
            <Skeleton variant="rect" className={classes.loadingPanel} />
          ) : (
            <div>

            {loadingError ?
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
             <div>
              <FormattedMessage
                id="dashboard.devices.new.step2.notice"
                description="Notice related when we wait for new connection"
                defaultMessage="Now you can copy the key to your device and start to connect with the broker.
            Once we receive a new message from the broker, we will prompt you to continue.
            Your device's name is {name}.
            Your device's key is shown below.
            {code}"
                values={{
                  name: <LineCode>{device.name}</LineCode>,
                  code: <BlockCode>{device.preSharedKey}</BlockCode>
                }}
              />
              </div>
            }
            <div>
                <Button
                  onClick={handleNext}
                  variant="contained"
                  color="primary"
                  disabled={!deviceConnected}
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
