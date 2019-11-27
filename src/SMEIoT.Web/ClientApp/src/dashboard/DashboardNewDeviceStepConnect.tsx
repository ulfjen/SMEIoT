import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import TextField from "@material-ui/core/TextField";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import { RouteComponentProps } from "@reach/router";
import BannerNotice from "../components/BannerNotice";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { DeviceApiModel } from "smeiot-client";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    },
    backButton: {
      marginRight: spacing(1)
    },
    instructions: {
      marginTop: spacing(1),
      marginBottom: spacing(1)
    },
    list: {
      backgroundColor: "#ffffff"
    },
    filterBar: {
      backgroundColor: "#ffffff",
      display: "flex",
      flexWrap: "wrap",
      "& > *": {
        margin: spacing(0.5)
      },
      marginBottom: spacing(2)
    },
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    fixedHeight: {
      height: 240
    },
    usersMenu: {},
    usersMenuDeleteItem: {
      color: palette.error.main
    },
    avatar: {},
    cardContent: {}
  });

export interface IDashboardNewDeviceStepConnectProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {
  device: DeviceApiModel;
  handleNext: (event: React.MouseEvent<HTMLButtonElement>) => void;
  loading: boolean;
}

const messages = defineMessages({});

const _DashboardNewDeviceStepConnect: React.FunctionComponent<IDashboardNewDeviceStepConnectProps> = ({
  classes,
  device,
  handleNext,
  loading
}) => {
  const intl = useIntl();

  return (
    <React.Fragment>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          <div>
            <p>
              <FormattedMessage
                id="dashboard.devices.new.step2.notice"
                description="Notice related when we wait for new connection"
                defaultMessage="Now you can copy the key to your device and start to connect with the broker.
            Once we receive a new message from the broker, we will prompt you to continue.
            Your device's name is: {name}.
            Your device's key is: {key}."
                values={{
                  name: device.name,
                  key: device.preSharedKey
                }}
              />
            </p>
            <div>
              <Button variant="contained" color="primary" onClick={handleNext}>
                <FormattedMessage
                  id="dashboard.devices.new.control.next"
                  description="The button text for going next page"
                  defaultMessage="Next"
                />
              </Button>
            </div>
          </div>
        </Paper>
      </Grid>
    </React.Fragment>
  );
};

const DashboardNewDeviceStepConnect = withStyles(styles)(
  _DashboardNewDeviceStepConnect
);

export default DashboardNewDeviceStepConnect;
