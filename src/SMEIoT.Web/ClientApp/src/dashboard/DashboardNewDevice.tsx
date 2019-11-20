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
import {Helmet} from "react-helmet";
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
  nameLabel: {
    id: "dashboard.devices.new.step1.name",
    description: "The label for adding device name",
    defaultMessage: "Device name"
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
  nextControl: {
    id: "dashboard.devices.new.control.next",
    description: "The button text for controling next",
    defaultMessage: "Next"
  },
  finishControl: {
    id: "dashboard.devices.new.control.finish",
    description: "The button text for finishing",
    defaultMessage: "Finish"
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
        </React.Fragment>
      );
    case 1:
      return (
        <React.Fragment>
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
      );
    case 2:
      return (
        <React.Fragment>
          <p>
            <FormattedMessage
              id="dashboard.devices.new.step3.notice"
              description="Notice related when we add new sensors"
              defaultMessage="The device is now installed. You can start to connect sensors. You can also add sensors in the device details page later."
            />
          </p>
        </React.Fragment>
      );
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

            <Grid item xs={12}>
              <Paper className={classes.paper}>
                <div>
                  {getStepContent(activeStep, intl)}
                  <div>
                    <Button
                      variant="contained"
                      color="primary"
                      onClick={handleNext}
                    >
                      {activeStep === steps.length - 1
                        ? intl.formatMessage(messages.finishControl)
                        : intl.formatMessage(messages.nextControl)}
                    </Button>
                    {activeStep === steps.length - 1 ? (
                      <Button variant="contained" onClick={handleReset}>
                        <FormattedMessage
                          id="dashboard.devices.new.control.create_new"
                          description="Control for adding new device (reset the wizard)"
                          defaultMessage="Connect new"
                        />
                      </Button>
                    ) : null}
                  </div>
                </div>
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
