import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import {
  defineMessages,
  useIntl,
} from "react-intl";
import {
  SensorsApi,
  BasicSensorApiModel,
  ProblemDetails
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import Tooltip from "@material-ui/core/Tooltip";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import ProgressIconButton from "../components/ProgressIconButton";
import AddIcon from "@material-ui/icons/Add";
import ListItem from "@material-ui/core/ListItem";
import List from "@material-ui/core/List";
import { SensorListing } from "../helpers/useSensorsByStatus";
import Snackbar from "@material-ui/core/Snackbar";
import Alert from "@material-ui/lab/Alert";
import ValidationProblemDetails from "../models/ValidationProblemDetails";


const styles = ({ spacing }: Theme) => createStyles({
  list: {
    marginTop: 20
  },
});

export interface IDashboardDeviceOtherSensorsProps extends WithStyles<typeof styles> {
  deviceName: string;
  sensorsToRender: Array<BasicSensorApiModel>;
  sensors: SensorListing;
  disableAction?: boolean;
}

const messages = defineMessages({
  sensor: {
    connect: {
      id: "dashboard.devices.edit.sensor.connect",
      description: "Label on sensor connection component's action",
      defaultMessage: "Connect"
    },
    buttonLabel: {
      id: "dashboard.devices.edit.sensor.label",
      description: "Aria Label on sensor connection component's action",
      defaultMessage: "Connect sensor to the device"
    },
  },
  defaultError: {
    id: "dashboard.devices.edit.sensor.error_adding_sensor",
    description: "Message on snackbar",
    defaultMessage: "Something went wrong."
  }
});

const _DashboardDeviceOtherSensors: React.FunctionComponent<IDashboardDeviceOtherSensorsProps> = ({
  classes,
  deviceName,
  sensors,
  sensorsToRender,
  disableAction
}) => {
  const intl = useIntl();

  const [loading, setLoading] = React.useState<boolean>(false);
  const [tooltipOpen, setTooltipOpen] = React.useState<boolean>(false);

  const sensorApi = new SensorsApi(GetDefaultApiConfig());
  const [snackbarOpen, setSnackbarOpen] = React.useState<boolean>(false);
  const [snackbarMessage, setSnackbarMessage] = React.useState<string>("");
  
  const handleSensorRegistrationOnClick = React.useCallback(async (sensorName: string) => {
    setLoading(true);
    setTooltipOpen(false);
    await sensorApi.apiSensorsPost({
      sensorLocatorBindingModel: {
        deviceName,
        name: sensorName
      }
    }).then((res) => {
      sensors.setRunning(sensors.running.concat(res));
      sensors.setNotRegistered(sensors.notRegistered.filter(s => s.sensorName !== sensorName));
    }).catch((pd: ValidationProblemDetails) => {
      setSnackbarOpen(true);
      let msg = "";
      if (pd.errors) {
        if (pd.errors.hasOwnProperty("sensorName")) {
          msg += pd.errors["sensorName"].join(" ");
        }
        if (pd.errors.hasOwnProperty("deviceName")) {
          msg += pd.errors["deviceName"].join(" ");
        }
      }
      if (msg.length === 0) {
        msg = intl.formatMessage(messages.defaultError);
      }
      setSnackbarMessage(msg);
    }).finally(() => {
      setLoading(false);
    });
  }, [deviceName, sensors.running, sensors.notRegistered]);

  const closeSnackbar = (e: React.SyntheticEvent<Element, Event> | null) => {
    if (e) { e.preventDefault() }
    setSnackbarOpen(false);
  };

  if (sensorsToRender.length === 0) {
    return <div></div>;
  } else {
    return <List className={classes.list}>
      {sensorsToRender.map(sensor =>
        <ListItem disableGutters key={sensor.sensorName}>
          <TwoLayerLabelAction greyoutFirst first={deviceName} second={sensor.sensorName} action={
            !disableAction &&
            <Tooltip
              title={intl.formatMessage(messages.sensor.connect)}
              aria-label={intl.formatMessage(messages.sensor.connect)}
              open={tooltipOpen}
              onOpen={() => setTooltipOpen(true)}
              onClose={() => setTooltipOpen(false)}
            >
              <ProgressIconButton
                ariaLabel={intl.formatMessage(messages.sensor.buttonLabel)}
                loading={loading}
                onClick={() => handleSensorRegistrationOnClick(sensor.sensorName)}>
                <AddIcon />
              </ProgressIconButton>
            </Tooltip>} />
        </ListItem>
      )}
      <Snackbar anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }} open={snackbarOpen} autoHideDuration={5000} onClose={closeSnackbar}>
        <Alert onClose={closeSnackbar} severity="error">
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </List>
  }
};

const DashboardDeviceOtherSensors = withStyles(styles)(_DashboardDeviceOtherSensors);

export default DashboardDeviceOtherSensors;
