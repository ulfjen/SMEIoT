import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardNewDeviceFrame from "./DashboardNewDeviceFrame";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import Skeleton from "@material-ui/lab/Skeleton";
import Typography from "@material-ui/core/Typography";
import AddIcon from "@material-ui/icons/Add";
import { RouteComponentProps } from "@reach/router";
import { useTitle, useAsync } from 'react-use';
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import BannerNotice from "../components/BannerNotice";
import ProgressButton from "../components/ProgressButton";
import SuggestTextField from "../components/SuggestTextField";
import {
  DeviceConfigBindingModel,
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
import { Link as ReachLink } from "@reach/router";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Link from '@material-ui/core/Link';
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import { AsyncState } from "react-use/lib/useAsync";
import StatusBadge from "../components/StatusBadge";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";

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
    },
    loadingPanel: {
      height: 200
    },
    card: {
      padding: 16,
    }
  });

export interface IDashboardEditDeviceRouteParams {
  deviceName: string;
}

export interface IDashboardDeviceEditProps
  extends RouteComponentProps<IDashboardEditDeviceRouteParams>,
  WithStyles<typeof styles> { }

const messages = defineMessages({
  title: {
    id: "dashboard.devices.edit.title",
    description: "Used as title in the edit device page on the dashboard",
    defaultMessage: "Edit device"
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
});

const _DashboardDeviceEdit: React.FunctionComponent<IDashboardDeviceEditProps> = ({
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

  const renderActionList = (deviceName: string, names: string[]) => {
    return names.map(name => <TwoLayerLabelAction
      firstLabel={deviceName}
      key={name}
      secondLabel={name}
      firstLabelVariant="inherit"
      actionIcon={<AddIcon />}
      actionIconOnClick={async (event) => {
        let parent = event.currentTarget.parentElement
        if (!parent) { return; }
        // .parentElement.children;
        let deviceName = parent.childNodes[0].textContent;
        let sensorName = parent.childNodes[2].textContent;
        if (deviceName === null || sensorName === null) { return; }
        let sensor = await api.apiSensorsPost({
          sensorLocatorBindingModel: {
            deviceName: deviceName,
            name: sensorName
          }
        });
        console.log(sensor);
      }}
    />);
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
            <Card className={classes.card}>
              <ExpandedCardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={32} /> : state.value && state.value.name}
                status={<StatusBadge
                  color={state.loading || state.value === undefined ? null : (state.value.connected ? "normal" : "error")}
                  badge={state.loading && <Skeleton variant="circle" height={14} width={14} />}
                >
                  {state.loading ? <Skeleton variant="rect" width={100} height={14} /> : state.value && intl.formatMessage(state.value.connected ? messages.connected : messages.notConnected)}
                </StatusBadge>}
              />
              <CardContent>
              </CardContent>
            </Card>

          </Grid>
          <Grid item xs={12}>
            <Paper>{renderActionList("pupate-potteen", sensorNames || [])}</Paper>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardDeviceEdit = withStyles(styles)(_DashboardDeviceEdit);

export default DashboardDeviceEdit;
