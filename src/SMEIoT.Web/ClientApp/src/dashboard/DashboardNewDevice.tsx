import Container from "@material-ui/core/Container";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Stepper from "@material-ui/core/Stepper";
import Step from "@material-ui/core/Step";
import StepLabel from "@material-ui/core/StepLabel";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import { Link, RouteComponentProps } from "@reach/router";
import { Helmet } from "react-helmet";
import {
  defineMessages,
  useIntl,
  IntlShape
} from "react-intl";
import DashboardNewDeviceStepCreate from "./DashboardNewDeviceStepCreate";
import DashboardNewDeviceStepConnect from "./DashboardNewDeviceStepConnect";
import DashboardNewDeviceStepConnectSensors from "./DashboardNewDeviceStepConnectSensors";

const styles = ({
  spacing,
}: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    },
    instructions: {
      marginTop: spacing(1),
      marginBottom: spacing(1)
    },
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    }
  });

export interface IDashboardNewDeviceProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.new.title",
    description: "Used as title in the new device page on the dashboard",
    defaultMessage: "Connect with a device"
  },
  closeAriaLabel: {
    id: "dashboard.devices.new.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  },
  keyLabel: {
    id: "dashboard.devices.new.step1.key",
    description: "The label for adding psk key",
    defaultMessage: "Key"
  },
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
  },
  connectTitle: {
    id: "dashboard.devices.new.title",
    description: "HTML title for the connecting new device page",
    defaultMessage: "Connect a new device"
  }
});

function getSteps(intl: IntlShape) {
  return [
    intl.formatMessage(messages.step1),
    intl.formatMessage(messages.step2),
    intl.formatMessage(messages.step3)
  ];
}

const _DashboardNewDevice: React.FunctionComponent<IDashboardNewDeviceProps> = ({
  classes
}) => {
  const intl = useIntl();

  const [activeStep, setActiveStep] = React.useState(0);
  const steps = getSteps(intl);

  const handleNext = () => {
    setActiveStep(prevActiveStep => prevActiveStep + 1);
  };

  const handleReset = () => {
    setActiveStep(0);
  };

  const getStepContent = () => {
    switch (activeStep) {
      case 0:
        return <DashboardNewDeviceStepCreate handleNext={handleNext} />;
      case 1:
        return <DashboardNewDeviceStepConnect handleNext={handleNext} />;
      case 2:
        return <DashboardNewDeviceStepConnectSensors handleNext={handleNext} handleReset={handleReset} />;
      default:
        return "Unknown stepIndex";
    }
  };

  React.useEffect(() => {
    // @ts-ignore
    if (window.SMEIoTPreRendered && window.SMEIoTPreRendered["user"]) {
      // @ts-ignore
      // saveUser(window.SMEIoTPreRendered["user"]);
    }
  }, []);

  return (
    <Frame
      title={intl.formatMessage(messages.title)}
      direction="ltr"
      toolbarRight={
        <IconButton
          edge="end"
          color="inherit"
          aria-label={intl.formatMessage(messages.closeAriaLabel)}
          to={"/dashboard/devices"}
          component={Link}
        >
          <Helmet>
            <title>{intl.formatMessage(messages.connectTitle)}</title>
          </Helmet>
          <CloseIcon />
        </IconButton>
      }
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Paper className={classes.paper}>
                <Stepper activeStep={activeStep} alternativeLabel>
                  {steps.map(label => (
                    <Step key={label}>
                      <StepLabel>{label}</StepLabel>
                    </Step>
                  ))}
                </Stepper>
              </Paper>
            </Grid>

            {getStepContent()}
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardNewDevice = withStyles(styles)(_DashboardNewDevice);

export default DashboardNewDevice;
