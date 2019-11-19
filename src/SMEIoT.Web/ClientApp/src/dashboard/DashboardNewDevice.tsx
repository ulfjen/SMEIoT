import Container from "@material-ui/core/Container";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import TextField from "@material-ui/core/TextField";
import Skeleton from "@material-ui/lab/Skeleton";
import { GetDefaultApiConfig } from "../index";
import Paper from "@material-ui/core/Paper";
import { AdminUserApiModel, AdminUsersApi, UsersApi } from "smeiot-client";
import moment from "moment";
import Avatar from "@material-ui/core/Avatar";
import CardHeader from "@material-ui/core/CardHeader";
import Grid from "@material-ui/core/Grid";
import Stepper from "@material-ui/core/Stepper";
import Step from "@material-ui/core/Step";
import StepLabel from "@material-ui/core/StepLabel";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import { Link, RouteComponentProps } from "@reach/router";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Switch from "@material-ui/core/Switch";
import {
  defineMessages,
  useIntl,
  FormattedMessage,
  IntlShape
} from "react-intl";
import { SMEIoT } from "../avatars";
import { SensorsApi } from "smeiot-client";

const styles = ({
  palette,
  spacing,
  transitions,
  zIndex,
  mixins,
  breakpoints
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
  identityLabel: {
    id: "dashboard.devices.new.psk.identity",
    description: "The label for adding psk identity",
    defaultMessage: "Identity"
  },
  keyLabel: {
    id: "dashboard.devices.new.psk.key",
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
  }
});

function getSteps(intl: IntlShape) {
  return [
    intl.formatMessage(messages.step1),
    intl.formatMessage(messages.step2)
  ];
}

function getStepContent(stepIndex: number, intl: IntlShape) {
  switch (stepIndex) {
    case 0:
      return (
        <React.Fragment>
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
            id="psk-identity"
            label={intl.formatMessage(messages.identityLabel)}
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
        </React.Fragment>
      );
    case 1:
      return <React.Fragment>
      <p>
        <FormattedMessage
          id="dashboard.devices.new.step2.notice"
          description="Notice related when we wait for new connection"
          defaultMessage="Now you can copy the key to your device and start to connect with the broker.
            Once we receive a new message from the broker, we will prompt you."
        />
      </p>
      <p>(relevant information down here)</p>
    </React.Fragment>
    default:
      return "Unknown stepIndex";
  }
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

  const handleBack = () => {
    setActiveStep(prevActiveStep => prevActiveStep - 1);
  };

  const handleReset = () => {
    setActiveStep(0);
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

            <Grid item xs={12}>
              <Paper className={classes.paper}>
                {activeStep === steps.length ? (
                  <div>
                    <Typography className={classes.instructions}>
                      A new device is connected! Add sensors now (link).
                    </Typography>
                    <Button onClick={handleReset}>Connect with another device</Button>
                  </div>
                ) : (
                  <div>
                    <Typography className={classes.instructions}>
                      {getStepContent(activeStep, intl)}
                    </Typography>
                    <div>
                      <Button
                        disabled={activeStep === 0}
                        onClick={handleBack}
                        className={classes.backButton}
                      >
                        Back
                      </Button>
                      <Button
                        variant="contained"
                        color="primary"
                        onClick={handleNext}
                      >
                        {activeStep === steps.length - 1 ? "Finish" : "Create"}
                      </Button>
                    </div>
                  </div>
                )}
              </Paper>
            </Grid>
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardNewDevice = withStyles(styles)(_DashboardNewDevice);

export default DashboardNewDevice;
