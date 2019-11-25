import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import { RouteComponentProps } from "@reach/router";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    }
  });

export interface IDashboardNewDeviceStepConnectSensorsProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {
  handleNext: (event: React.MouseEvent<HTMLButtonElement>) => void;
  handleReset: (event: React.MouseEvent<HTMLButtonElement>) => void;
}

const messages = defineMessages({
  connectTitle: {
    id: "dashboard.devices.new.title",
    description: "HTML title for the connecting new device page",
    defaultMessage: "Connect a new device"
  }
});

const _DashboardNewDeviceStepConnectSensors: React.FunctionComponent<IDashboardNewDeviceStepConnectSensorsProps> = ({
  classes,
  handleNext,
  handleReset
}) => {
  const intl = useIntl();

  return (
    <React.Fragment>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          <div>
            <p>
              <FormattedMessage
                id="dashboard.devices.new.step3.notice"
                description="Notice related when we add new sensors"
                defaultMessage="The device is now installed. You can start to connect sensors. You can also add sensors in the device details page later."
              />
            </p>
            <div>
              <Button variant="contained" color="primary" onClick={handleNext}>
                <FormattedMessage
                  id="dashboard.devices.new.control.finish"
                  description="The button text for going to finish adding"
                  defaultMessage="Finish"
                />
              </Button>
              <Button variant="contained" onClick={handleReset}>
                <FormattedMessage
                  id="dashboard.devices.new.control.create_new"
                  description="Control for adding new device (reset the wizard)"
                  defaultMessage="Connect new"
                />
              </Button>
            </div>
          </div>
        </Paper>
      </Grid>
    </React.Fragment>
  );
};

const DashboardNewDeviceStepConnectSensors = withStyles(styles)(
  _DashboardNewDeviceStepConnectSensors
);

export default DashboardNewDeviceStepConnectSensors;
