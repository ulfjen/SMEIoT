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
import { defineMessages, useIntl, IntlShape } from "react-intl";
import DashboardNewDeviceStepCreate from "./DashboardNewDeviceStepCreate";
import DashboardNewDeviceStepConnect from "./DashboardNewDeviceStepConnect";
import DashboardNewDeviceStepConnectSensors from "./DashboardNewDeviceStepConnectSensors";
import {
  DeviceConfigBindingModel,
  DevicesApi,
  DeviceApiModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";

const styles = ({ spacing }: Theme) =>
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
    defaultMessage: "Connect with a new device"
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
  const [handlingNext, setHandlingNext] = React.useState<boolean>(false);
  const [deviceConfig, setDeviceConfig] = React.useState<
    DeviceConfigBindingModel
  >({});
  const [unconnectedDeviceName, setUnconnectedDeviceName] = React.useState<
    string | null
  >(null);
  const [device, setDevice] = React.useState<DeviceApiModel>({});

  const handleNext = async () => {
    setHandlingNext(true);
    switch (activeStep) {
      case 0:
        const api = new DevicesApi(GetDefaultApiConfig());
        const res = await api.apiDevicesBootstrapPost({
          deviceConfigBindingModel: deviceConfig
        });
        if (res !== null) {
          setDevice(res);
        }
        break;
      case 1:
        break;
      case 2:
        break;
      default:
        break;
    }
    setActiveStep(prevActiveStep => prevActiveStep + 1);
    setHandlingNext(false);
  };

  const handleReset = () => {
    setActiveStep(0);
  };

  const [loadingConnect, setLoadingConnect] = React.useState<boolean>(false);

  const onConnectBannerClick = async (
    event: React.MouseEvent<HTMLButtonElement>
  ) => {
    setLoadingConnect(true);
    setActiveStep(1);
    setHandlingNext(false);
    if (unconnectedDeviceName !== null) {
      const api = new DevicesApi(GetDefaultApiConfig());
      const res = await api.apiDevicesNameGet({
        name: unconnectedDeviceName
      });
      if (res !== null) {
        setDevice(res);
      }
    }
    setLoadingConnect(false);
  };

  const getStepContent = () => {
    switch (activeStep) {
      case 0:
        return (
          <DashboardNewDeviceStepCreate
            unconnectedDeviceName={unconnectedDeviceName}
            setUnconnectedDeviceName={setUnconnectedDeviceName}
            onBannerClick={onConnectBannerClick}
            device={deviceConfig}
            setDevice={setDeviceConfig}
            handleNext={handleNext}
            handlingNext={handlingNext}
          />
        );
      case 1:
        return (
          <DashboardNewDeviceStepConnect
            loading={loadingConnect}
            device={device}
            handleNext={handleNext}
          />
        );
      case 2:
        return (
          <DashboardNewDeviceStepConnectSensors
            handleNext={handleNext}
            handleReset={handleReset}
          />
        );
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
            <title>{intl.formatMessage(messages.title)}</title>
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
