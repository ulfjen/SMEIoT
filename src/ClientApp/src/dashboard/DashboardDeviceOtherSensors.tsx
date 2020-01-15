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
  BasicSensorApiModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import Tooltip from "@material-ui/core/Tooltip";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import ProgressIconButton from "../components/ProgressIconButton";
import AddIcon from "@material-ui/icons/Add";
import ListItem from "@material-ui/core/ListItem";
import List from "@material-ui/core/List";
const styles = ({ spacing }: Theme) => createStyles({
  list: {
    marginTop: 20
  },
});

export interface IDashboardDeviceOtherSensorsProps extends WithStyles<typeof styles> {
  deviceName: string;
  sensorsToRender: Array<BasicSensorApiModel>;
  sensors: Array<BasicSensorApiModel>;
  setSensors: React.Dispatch<React.SetStateAction<BasicSensorApiModel[]>>;
  notRegisteredSensors: Array<BasicSensorApiModel>;
  setNotRegisteredSensors: React.Dispatch<React.SetStateAction<BasicSensorApiModel[]>>;
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
});

const _DashboardDeviceOtherSensors: React.FunctionComponent<IDashboardDeviceOtherSensorsProps> = ({
  classes,
  deviceName,
  sensors,
  setSensors,
  sensorsToRender,
  disableAction,
  notRegisteredSensors,
  setNotRegisteredSensors
}) => {
  const intl = useIntl();

  const [loading, setLoading] = React.useState<boolean>(false);
  const [tooltipOpen, setTooltipOpen] = React.useState<boolean>(false);

  const sensorApi = new SensorsApi(GetDefaultApiConfig());
  
  const handleSensorRegisterationOnClick = React.useCallback(async (sensorName: string) => {
    setLoading(true);
    setTooltipOpen(false);
    const res = await sensorApi.apiSensorsPost({
      sensorLocatorBindingModel: {
        deviceName,
        name: sensorName
      }
    });
    setLoading(false);
    setSensors(sensors.concat(res));
    setNotRegisteredSensors(notRegisteredSensors.filter(s => s.sensorName !== sensorName));
  }, [deviceName, sensors, notRegisteredSensors]);

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
                onClick={() => handleSensorRegisterationOnClick(sensor.sensorName)}>
                <AddIcon />
              </ProgressIconButton>
            </Tooltip>} />
        </ListItem>
      )}
    </List>
  }
};

const DashboardDeviceOtherSensors = withStyles(styles)(_DashboardDeviceOtherSensors);

export default DashboardDeviceOtherSensors;
