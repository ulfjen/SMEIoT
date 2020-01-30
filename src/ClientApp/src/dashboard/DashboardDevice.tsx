import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { darken } from '@material-ui/core/styles';
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import Skeleton from "@material-ui/lab/Skeleton";
import Typography from "@material-ui/core/Typography";
import { RouteComponentProps } from "@reach/router";
import { useTitle, useAsync } from 'react-use';
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import {
  SensorDetailsApiModel,
  DevicesApi,
  DeviceDetailsApiModel,
  SensorDetailsApiModelStatusEnum
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import DashboardFrame from "./DashboardFrame";
import Container from "@material-ui/core/Container";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import { Link as ReachLink } from "@reach/router";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Link from '@material-ui/core/Link';
import Divider from '@material-ui/core/Divider';
import Button from '@material-ui/core/Button';
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import { AsyncState } from "react-use/lib/useAsync";
import StatusBadge from "../components/StatusBadge";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import CardHeader from "@material-ui/core/CardHeader";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import ExpansionPanelActions from "@material-ui/core/ExpansionPanelActions";
import useMenu from "../helpers/useMenu";
import useModal from "../helpers/useModal";
import DashboardDeviceMenu from "./DashboardDeviceMenu";
import DashboardDeviceDialog from "./DashboardDeviceDialog";
import DashboardDeviceOtherSensors from "./DashboardDeviceOtherSensors";
import useSensorByStatus from "../helpers/useSensorsByStatus";
import CardActions from "@material-ui/core/CardActions";
import DashboardSensorDialog from "./DashboardSensorDialog";
import NumberGraph from "../components/NumberGraph";
import LineCode from "../components/LineCode";
import placeholder from "../images/placeholder300.jpg";

const styles = ({ typography, palette, spacing, zIndex }: Theme) => createStyles({
  container: {
  },
  instructions: {
    marginTop: spacing(1),
    marginBottom: spacing(1)
  },
  loadingPanel: {
    height: 200
  },
  column: {
    flexBasis: '33.33%',
  },
  twoColumnSpan: {
    flexBasis: '66.66%',
  },
  helper: {
    borderLeft: `2px solid ${palette.divider}`,
    padding: spacing(1, 2),
  },
  link: {
    color: palette.primary.main,
    textDecoration: 'none',
    '&:hover': {
      textDecoration: 'underline',
    },
  },
  heading: {
    fontSize: typography.pxToRem(15),
  },
  paper: {
    backgroundColor: darken(palette.background.paper, 0.01),
    overflow: "hidden"
  },
  panel: {
    "&:first-child": {
      borderTop: '2px solid red',

    }
  },
  firstPanel: {
    borderTop: `2px solid ${palette.divider}`,
    '&:before': {
      top: 0,
      height: 0,
      display: 'none',
    },
  },
  summary: {
    padding: "0 16px 0 16px",
    backgroundColor: darken(palette.background.paper, 0.01),
  },
  details: {
    backgroundColor: palette.background.paper,
    alignItems: 'center',
  },
  warning: {
    color: palette.text.secondary
  },
  list: {
    marginTop: 20
  },
  removeAction: {
    color: palette.error.main
  },
  backdrop: {
    zIndex: zIndex.drawer + 1,
    color: '#fff',
  },
  media: {
    height: 150,
  }
});

export interface IDashboardDeviceRouteParams {
  deviceName: string;
}

export interface IDashboardDeviceProps extends RouteComponentProps<IDashboardDeviceRouteParams>, WithStyles<typeof styles> {

}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.edit.title",
    description: "Used as title in the edit device page on the dashboard",
    defaultMessage: "Configure device"
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
  closeAriaLabel: {
    id: "dashboard.devices.edit.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  },
  moreAria: {
    id: "dashboard.devices.edit.more",
    description: "The aria label for more action",
    defaultMessage: "More"
  },
  status: {
    connected: {
      id: "dashboard.devices.edit.status.connected",
      description: "The status for device connected",
      defaultMessage: "Connected"
    },
    notConnected: {
      id: "dashboard.devices.edit.status.not_connected",
      description: "The status for device not connected",
      defaultMessage: "Not connected"
    }
  },
  connected: {
    title: {
      id: "dashboard.devices.edit.connected_sensors.title",
      description: "The title for connected sensor card",
      defaultMessage: "Sensors"
    },
  },
  notRegistered: {
    title: {
      id: "dashboard.devices.edit.not_registered_sensors.title",
      description: "The title for not registered sensor card",
      defaultMessage: "Available Sensors"
    },
  },
  notConnected: {
    title: {
      id: "dashboard.devices.edit.not_connected_sensors.title",
      description: "The title for not connected sensor card",
      defaultMessage: "Inactive Sensors"
    },
  },
  instructions: {
    sensor: {
      id: "dashboard.devices.edit.sensor.instruction.primary",
      description: "Instruction text on sensor panel",
      defaultMessage: "Allow some users to watch the sensor graph by clicking \"Assign\" button or"
    },
    sensorLink: {
      id: "dashboard.devices.edit.sensor.instruction.link",
      description: "Instruction link on sensor panel",
      defaultMessage: "See graph"
    }
  },
  defaultError: {
    id: "dashboard.devices.edit.dialog",
    description: "Message on snackbar",
    defaultMessage: "Something went wrong."
  },
  nothing: {
    id: "dashboard.devices.edit.sensor.graph.nothing",
      description: "graph placeholder text on sensor panel",
      defaultMessage: "Nothing available yet"
  }
});

const _DashboardDevice: React.FunctionComponent<IDashboardDeviceProps> = ({
  classes,
  deviceName,
  navigate
}) => {
  if (!deviceName) {
    throw new Error("No device is assigned to the route.");
  }
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const [menuOpen, anchorEl, openMenu, closeMenu, menuDeviceName] = useMenu<string>();
  const [dialogOpen, openDialog, closeDialog, dialogDeviceName] = useModal<string>();
  const sensors = useSensorByStatus();

  const [sensorRemovalOpen, openSensorRemovalDialog, closeSensorRemovalDialog, sensorNameForRemoval] = useModal<string>();

  const onRemoveSensor = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>, sensorName: string) => {
    e.preventDefault();
    openSensorRemovalDialog(sensorName);
  };

  const renderPanels = (sensors: Array<SensorDetailsApiModel>) => {
    return sensors.length === 0 ? <CardContent>
      <Typography variant="body2" color="textSecondary" component="p">
        <FormattedMessage
          id="dashboard.devices.edit.sensors.empty"
          description="The text when no sensors"
          defaultMessage="You haven't added any sensors yet."
        />  
      </Typography>
    </CardContent> : sensors.map((sensor, index) => {
      const data = sensor.data.map(v => {
        return {x: new Date(Date.parse(v.createdAt)).getTime(), y: v.value}
      });
      return <ExpansionPanel className={index === 0 ? clsx(classes.panel, classes.firstPanel) : classes.panel} key={index} square elevation={0} defaultExpanded={index === 0} TransitionProps={{ unmountOnExit: true }}>
        <ExpansionPanelSummary
          expandIcon={<ExpandMoreIcon />}
          className={classes.summary}
        >
          <div className={classes.column}>
            <TwoLayerLabelAction className={classes.heading} greyoutFirst first={state.value ? state.value.name : ""} second={sensor.sensorName} />
          </div>
          <div className={classes.column} />
        </ExpansionPanelSummary>
        <ExpansionPanelDetails className={classes.details}>
          <div className={classes.twoColumnSpan}>
            {data.length === 0 ? <img alt={intl.formatMessage(messages.nothing)} src={placeholder} className={classes.media} /> : <NumberGraph width={300} height={150} data={data} />}
          </div>
          <div className={clsx(classes.column, classes.helper)}>
            <Typography variant="caption">
              {intl.formatMessage(messages.instructions.sensor)}
              <br />
              <ReachLink to={`../../sensors/${deviceName}/${sensor.sensorName}`} className={classes.link}>
                {intl.formatMessage(messages.instructions.sensorLink)}
              </ReachLink>
            </Typography>
          </div>
        </ExpansionPanelDetails>
        <Divider />
        <ExpansionPanelActions>
          <Button size="small" className={classes.warning} onClick={(e) => onRemoveSensor(e, sensor.sensorName)}>
            <FormattedMessage
              id="dashboard.devices.edit.sensor.actions.remove"
              description="Action for sensor"
              defaultMessage="Remove"
            /> 
          </Button>
          <Button size="small" variant="contained" color="primary" onClick={() => navigate && navigate(`${sensor.sensorName}`)}>
            <FormattedMessage
              id="dashboard.devices.edit.sensor.actions.assign"
              description="Action for sensor"
              defaultMessage="Assign"
            />
          </Button>
        </ExpansionPanelActions>
      </ExpansionPanel>
    });
  }

  const deviceApi = new DevicesApi(GetDefaultApiConfig());

  const state: AsyncState<DeviceDetailsApiModel> = useAsync(async () => {
    const res = await deviceApi.apiDevicesNameGet({
      name: deviceName
    }).then(res => {
      sensors.setNotRegistered(res.sensors.filter(s => s.status === SensorDetailsApiModelStatusEnum.NotRegistered)); 
      sensors.setNotConnected(res.sensors.filter(s => s.status === SensorDetailsApiModelStatusEnum.NotConnected));
      sensors.setRunning(res.sensors.filter(s => s.status === SensorDetailsApiModelStatusEnum.Connected));
      return res;
    });

    return res;
  });

  return <DashboardFrame
    title={intl.formatMessage(messages.title)}
    drawer
    direction="ltr"
    toolbarRight={
      <IconButton
        edge="end"
        color="inherit"
        aria-label={intl.formatMessage(messages.closeAriaLabel)}
        to={".."}
        component={ReachLink}
      >
        <CloseIcon />
      </IconButton>
    }
    content={
      <Container maxWidth="lg" className={classes.container}>
        <DashboardDeviceMenu
          open={menuOpen}
          anchorEl={anchorEl}
          deviceName={menuDeviceName}
          closeMenu={closeMenu}
          hideConfigureItem
          pathPrefix=".."
          navigate={navigate}
          openDialog={openDialog}
        />
        <DashboardDeviceDialog
          open={dialogOpen}
          deviceName={dialogDeviceName}
          closeDialog={closeDialog}
          navigate={navigate}
          navigateUrl={".."}
        />
        <DashboardSensorDialog
          open={sensorRemovalOpen}
          deviceName={deviceName}
          sensorName={sensorNameForRemoval}
          closeDialog={closeSensorRemovalDialog}
          refreshAfterDone
          navigate={navigate}
        />
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />} aria-label="breadcrumb">
              <Link component={ReachLink} color="inherit" to="..">
                <FormattedMessage
                  id="dashboard.devices.edit.breadcrumb.devices"
                  description="The label at the breadcrumb for devices"
                  defaultMessage="Devices"
                />
              </Link>
              <Typography color="textPrimary">
                <FormattedMessage
                  id="dashboard.devices.edit.breadcrumb.edit"
                  description="The label at the breadcrumb for editing device"
                  defaultMessage="Configuration"
                />
              </Typography>
            </Breadcrumbs>
          </Grid>
          <Grid item xs={12}>
            <Card>
              <ExpandedCardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={30} /> : state.value && state.value.name}
                status={<StatusBadge
                  severity={state.value !== undefined ? (state.value.connected ? "success" : "error") : "error"}
                  badge={state.loading && <Skeleton variant="circle" height={14} width={14} />}
                >
                  {state.loading ? <Skeleton variant="rect" width={100} height={14} /> : state.value && intl.formatMessage(state.value.connected ? messages.status.connected : messages.status.notConnected)}
                </StatusBadge>}
                action={!state.loading &&
                  <IconButton
                    aria-label={intl.formatMessage(messages.moreAria)}
                    onClick={(e) => openMenu(e.currentTarget, state.value ? state.value.name : "")}
                  >
                    <MoreVertIcon />
                  </IconButton>
                }
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : <Typography variant="body2" color="textSecondary">
                <FormattedMessage
                  id="dashboard.device.edit.instructions"
                  description="The instruction for device."
                  defaultMessage="Configured sensors will be shown here. But if a device is reconfigured or you believe it sends messages while this page doesn't show anything, refresh the page.
                  Any MQTT messages sent to {topic} at {host}:{port} will appear in different sections below."
                  values={{
                    topic: state.value && <LineCode>{`${state.value.mqttTopicPrefix}${state.value.name}/<any_sensor_name>`}</LineCode>,
                    host: state.value && <LineCode>{state.value.mqttHost}</LineCode>,
                    port: state.value && <LineCode>{state.value.mqttPort}</LineCode>,
                  }}
                />
              </Typography>}
              </CardContent>
              {!state.loading && state.value && <CardActions>
                <Button size="small" color="primary" component={ReachLink} to={`/dashboard/broker/logs?filter=iot/${state.value.name}`}>
                  <FormattedMessage
                    id="dashboard.device.edit.actions.logs"
                    description="The action for device cards."
                    defaultMessage="Logs"
                  />
                </Button>
              </CardActions>}
            </Card>
          </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <CardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={26} /> : intl.formatMessage(messages.connected.title)}
                titleTypographyProps={{ color: "secondary", variant: "h6" }}
              />
              {state.loading ?
                <CardContent><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></CardContent> :
                renderPanels(sensors.running)}
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <CardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={26} /> : intl.formatMessage(messages.notRegistered.title)}
                titleTypographyProps={{ color: "secondary", variant: "h6" }}
              />
              <CardContent>
                {state.loading ? <Skeleton variant="text" /> :
                  <Typography variant="body2" color="textSecondary">
                    <FormattedMessage
                      id="dashboard.device.edit.not_registered_sensors.instructions"
                      description="The instruction for device cards."
                      defaultMessage="We have found some sensors that can be connected to this device. After connecting, you can assign the sensor to users."
                    />
                  </Typography>}
                <DashboardDeviceOtherSensors
                  deviceName={state.value ? state.value.name : ""}
                  sensorsToRender={sensors.notRegistered}
                  sensors={sensors}
                />
              </CardContent>
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <CardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={26} /> : intl.formatMessage(messages.notConnected.title)}
                titleTypographyProps={{ color: "secondary", variant: "h6" }}
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : <Typography variant="body2" color="textSecondary" component="p">
                  <FormattedMessage
                    id="dashboard.device.edit.not_connected_sensors.instructions"
                    description="The instruction for device cards."
                    defaultMessage="These sensors are not connected to the broker. Please check connection and configuration on the device. They appear here because they were connected."
                  />
                </Typography>}
                <DashboardDeviceOtherSensors
                  deviceName={state.value ? state.value.name : ""}
                  sensorsToRender={sensors.notConnected}
                  sensors={sensors}
                  disableAction
                />
              </CardContent>
            </Paper>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardDevice = withStyles(styles)(_DashboardDevice);

export default DashboardDevice;
