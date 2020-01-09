import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { darken, lighten } from '@material-ui/core/styles';
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
  SensorsApi,
  DevicesApi,
  DeviceDetailsApiModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import Tooltip from "@material-ui/core/Tooltip";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import DashboardFrame from "./DashboardFrame";
import Container from "@material-ui/core/Container";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import AddIcon from "@material-ui/icons/Add";
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
import ListItem from "@material-ui/core/ListItem";
import List from "@material-ui/core/List";
import CardActions from "@material-ui/core/CardActions";
import MoreVertIcon from "@material-ui/icons/MoreVert";

const styles = ({ typography, palette, spacing }: Theme) => createStyles({
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
    color: palette.error.main
  },
  list: {
    marginTop: 20
  }
});

export interface IDashboardDeviceRouteParams {
  deviceName: string;
}

export interface IDashboardDeviceProps
  extends RouteComponentProps<IDashboardDeviceRouteParams>,
  WithStyles<typeof styles> { }

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

  const [sensorNames, setSensorNames] = React.useState<string[] | undefined>();
  // const [deviceName, setDeviceName] = React.useState<string>("");
  const [handlingNext, setHandlingNext] = React.useState<boolean>(false);
  const [loading, setLoading] = React.useState<boolean>(true);
  const [unconnectedDeviceName, setUnconnectedDeviceName] = React.useState<
    string | null
  >(null);
  const api = new SensorsApi(GetDefaultApiConfig());

  // const renderActionList = (deviceName: string, names: string[]) => {
  //   return names.map(name => <TwoLayerLabelAction
  //     first={deviceName}
  //     key={name}
  //     second={name}
  //     firstVariant="inherit"
  //     actionIconOnClick={async (event) => {
  //       let parent = event.currentTarget.parentElement
  //       if (!parent) { return; }
  //       // .parentElement.children;
  //       let deviceName = parent.childNodes[0].textContent;
  //       let sensorName = parent.childNodes[2].textContent;
  //       if (deviceName === null || sensorName === null) { return; }
  //       let sensor = await api.apiSensorsPost({
  //         sensorLocatorBindingModel: {
  //           deviceName: deviceName,
  //           name: sensorName
  //         }
  //       });
  //       console.log(sensor);
  //     }}
  //   />);
  // }

  const sensors = ["object", "primary"];
  const renderPanel = () => {
    return sensors.map((sensor, index) => <ExpansionPanel className={index === 0 ? clsx(classes.panel, classes.firstPanel) : classes.panel} key={index} square elevation={0} defaultExpanded={index === 0} TransitionProps={{ unmountOnExit: true }}>
      <ExpansionPanelSummary
        expandIcon={<ExpandMoreIcon />}
        className={classes.summary}
      >
        <div className={classes.column}>
          <Typography className={classes.heading}>{sensor}</Typography>
        </div>
        <div className={classes.column} />
      </ExpansionPanelSummary>
      <ExpansionPanelDetails className={classes.details}>
        <div className={classes.twoColumnSpan}>
          1
          </div>
        <div className={clsx(classes.column, classes.helper)}>
          <Typography variant="caption">
            Allow some users to watch the sensor graph by clicking "Assign" button or
            <br />
            <a href="#secondary-heading-and-columns" className={classes.link}>
              See graph
            </a>
          </Typography>
        </div>
      </ExpansionPanelDetails>
      <Divider />
      <ExpansionPanelActions>
        <Button size="small" className={classes.warning}>Disconnect</Button>
        <Button size="small" color="primary">
          Assign
      </Button>
      </ExpansionPanelActions>
    </ExpansionPanel>);
  }

  const renderOtherSensors = () => {
    return sensors.map(sensor =>
      <ListItem disableGutters key={sensor}>
        <TwoLayerLabelAction greyoutFirst first={"masses-pavy"} second={"b"} action={
          <Tooltip title={intl.formatMessage(messages.sensor.connect)} aria-label={intl.formatMessage(messages.sensor.connect)}>
            <IconButton aria-label={intl.formatMessage(messages.sensor.buttonLabel)} disabled={false} onClick={() => { }}>
              <AddIcon />
            </IconButton>
          </Tooltip>} />
      </ListItem>);
  }

  React.useEffect(() => {
    (async () => {
      const api = new DevicesApi(GetDefaultApiConfig());
      if (deviceName) {
        const res = await api.apiDevicesNameSensorCandidatesGet({
          name: deviceName
        });
        if (res !== null) {
          setSensorNames(res.names);
        }
      }
      setLoading(false);
    })();
  }, []);

  const deviceApi = new DevicesApi(GetDefaultApiConfig());
  const state: AsyncState<DeviceDetailsApiModel> = useAsync(async () => {
    return await deviceApi.apiDevicesNameGet({
      name: deviceName
    });
  })

  return <DashboardFrame
    title={intl.formatMessage(messages.title)}
    drawer
    direction="ltr"
    toolbarRight={
      <IconButton
        edge="end"
        color="inherit"
        aria-label={intl.formatMessage(messages.closeAriaLabel)}
        to={"../.."}
        component={ReachLink}
      >
        <CloseIcon />
      </IconButton>
    }
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />} aria-label="breadcrumb">
              <Link component={ReachLink} color="inherit" to="../..">
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
                action={
                  <IconButton
                    aria-label={intl.formatMessage(messages.moreAria)}
                    onClick={() => {}}
                  >
                    <MoreVertIcon />
                  </IconButton>
                }
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : <Typography variant="body2" color="textSecondary" component="p">
                some instructions
              </Typography>}
              </CardContent>
              <CardActions>
        <Button size="small" className={classes.warning}>
          Delete
        </Button>
        <Button size="small" color="primary">
          Manage authentication
        </Button>
        <Button size="small" color="primary">
          Logs
        </Button>
      </CardActions>
            </Card>
          </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <CardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={26} /> : intl.formatMessage(messages.connected.title)}
                titleTypographyProps={{ color: "secondary", variant: "h6" }}
              />
              {state.loading ? <CardContent><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></CardContent> : renderPanel()}
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
                  <Typography variant="body2" color="textSecondary" component="p">
                    We have found some sensors that can be connected to this device. After connecting, you can assign the sensor to users.
              </Typography>}
                {renderOtherSensors()}
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
                  These sensors are not connected to the broker. Please check connection and configuration on the device. They appear here because they were connected.
              </Typography>}
                <List className={classes.list}>
                  {renderOtherSensors()}

                </List>
              </CardContent>
            </Paper>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardDevice = withStyles(styles)(_DashboardDevice);

export default DashboardDevice;
