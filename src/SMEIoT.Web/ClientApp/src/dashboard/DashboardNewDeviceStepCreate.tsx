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
import {
  defineMessages,
  useIntl,
  FormattedMessage,
} from "react-intl";
import { SensorsApi } from "smeiot-client";

const styles = ({
  palette,
  spacing,
}: Theme) =>
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

export interface IDashboardNewDeviceStepCreateProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {
  handleNext: (event: React.MouseEvent<HTMLButtonElement>) => void;
}

const messages = defineMessages({
  nameLabel: {
    id: "dashboard.devices.new.step1.name",
    description: "The label for adding device name",
    defaultMessage: "Device name"
  },
  keyLabel: {
    id: "dashboard.devices.new.step1.key",
    description: "The label for adding psk key",
    defaultMessage: "Key"
  }
});

const _DashboardNewDeviceStepCreate: React.FunctionComponent<IDashboardNewDeviceStepCreateProps> = ({
  classes,
  handleNext
}) => {
  const intl = useIntl();

  const deviceNotConnected = true;

  return (
    <React.Fragment>
      {deviceNotConnected && (
        <Grid item xs={12}>
          <BannerNotice to={null}>
            <Typography component="p">
              notice: your device L403 is not connected. Continue to connect
              instead of create a new one?
            </Typography>
          </BannerNotice>
        </Grid>
      )}

      <Grid item xs={12}>
        <Paper className={classes.paper}>
          <div>
            <p>
              <FormattedMessage
                id="dashboard.devices.new.step1.notice"
                description="Notice related with how we can add a new device"
                defaultMessage="We create a pre-shared key (PSK) for your new device and install it in our MQTT broker. 
              When your device connects with the broker with this key, you can add its sensor values in the dashboard.
              Registed and unused keys are shown on the devices page. 
              Notice: the MQTT broker will be reloaded to install the key."
              />
            </p>
            <TextField
              variant="outlined"
              margin="normal"
              required
              fullWidth
              id="device-name"
              label={intl.formatMessage(messages.nameLabel)}
              autoFocus
              onChange={event => {
                console.log(event.target.value);
              }}
              // error={usernameErrors.length > 0}
              // helperText={usernameErrors}
            />
            <TextField
              variant="outlined"
              margin="normal"
              required
              fullWidth
              id="psk-key"
              label={intl.formatMessage(messages.keyLabel)}
              onChange={event => {
                console.log(event.target.value);
              }}
              // error={passwordErrors.length > 0}
              // helperText={passwordErrors}
            />
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

const DashboardNewDeviceStepCreate = withStyles(styles)(
  _DashboardNewDeviceStepCreate
);

export default DashboardNewDeviceStepCreate;
