import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Stepper from "@material-ui/core/Stepper";
import Step from "@material-ui/core/Step";
import StepLabel from "@material-ui/core/StepLabel";
import { defineMessages, useIntl, IntlShape } from "react-intl";

const styles = ({ spacing }: Theme) =>
  createStyles({
    root: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
  });

export interface IDashboardNewDeviceStepperProps
  extends WithStyles<typeof styles> {
  activeStep: number;
}

const messages = defineMessages({
  step1: {
    id: "dashboard.devices.new.step1.label",
    description: "The label for connecting device - step 1",
    defaultMessage: "Create a key"
  },
  step2: {
    id: "dashboard.devices.new.step2.label",
    description: "The label for connecting device - step 2",
    defaultMessage: "Install and connect"
  },
  step3: {
    id: "dashboard.devices.new.step3.label",
    description: "The label for connecting device - step 3",
    defaultMessage: "Add sensors"
  }
});

const _DashboardNewDeviceStepper: React.FunctionComponent<IDashboardNewDeviceStepperProps &
  WithStyles<typeof styles>> = ({ classes, activeStep }) => {
  const intl = useIntl();
  const steps = [
    intl.formatMessage(messages.step1),
    intl.formatMessage(messages.step2),
    intl.formatMessage(messages.step3)
  ];

  return (
    <Paper className={classes.root}>
      <Stepper activeStep={activeStep} alternativeLabel>
        {steps.map(label => (
          <Step key={label}>
            <StepLabel>{label}</StepLabel>
          </Step>
        ))}
      </Stepper>
    </Paper>
  );
};

const DashboardNewDeviceStepper = withStyles(styles)(
  _DashboardNewDeviceStepper
);
export default DashboardNewDeviceStepper;
