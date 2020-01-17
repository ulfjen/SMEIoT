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
  BasicSensorApiModel,
  DevicesApi,
  DeviceDetailsApiModel,
  SensorStatus,
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
import { IDashboardDeviceRouteParams } from "./DashboardDevice";
import SuggestTextField from "../components/SuggestTextField";
import ProgressButton from "../components/ProgressButton";

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
});

export interface IDashboardDeviceCredentialProps extends RouteComponentProps<IDashboardDeviceRouteParams>, WithStyles<typeof styles> {

}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.edit_credentials.title",
    description: "Used as title in the edit device credentials page on the dashboard",
    defaultMessage: "Credentials"
  },
  keyLabel: {
    id: "dashboard.devices.edit_credentials.key_label",
    description: "The label for editing psk key",
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
});

const _DashboardDeviceCredential: React.FunctionComponent<IDashboardDeviceCredentialProps> = ({
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

  const deviceApi = new DevicesApi(GetDefaultApiConfig());
  const sensors = useSensorByStatus();
  const [key, setKey] = React.useState<string>("");

  const state: AsyncState<DeviceDetailsApiModel> = useAsync(async () => {
    return await deviceApi.apiDevicesNameGet({
      name: deviceName
    }).then((res) => {
      setKey(res.preSharedKey);
      return res;
    });
  })

  const [suggestingKey, setSuggestKey] = React.useState<boolean>(false);
  const onSuggestKey: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestKey(true);

    const res = await deviceApi.apiDevicesConfigSuggestKeyGet();

    if (res.key !== null) {
      setKey(res.key);
    }

    setSuggestKey(false);
  };

  const onKeyChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setKey(event.target.value);
  };

  const [handlingEdit, setHandlingEdit] = React.useState<boolean>(false);

  const handleEdit = async () => {
    setHandlingEdit(true);
    const api = new DevicesApi(GetDefaultApiConfig());
    const res = await api.apiDevicesNamePut({
      name: deviceName,
      deviceConfigBindingModel: {
        key
      }
    });
    setHandlingEdit(false);
  };

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
        <DashboardDeviceMenu
          open={menuOpen}
          anchorEl={anchorEl}
          deviceName={menuDeviceName}
          closeMenu={closeMenu}
          hideConfigureItem
          navigate={navigate}
          openDialog={openDialog}
        />
        <DashboardDeviceDialog
          open={dialogOpen}
          deviceName={dialogDeviceName}
          closeDialog={closeDialog}
          navigate={navigate}
        />
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
              <Link component={ReachLink} color="inherit" to="..">
                <FormattedMessage
                  id="dashboard.devices.edit.breadcrumb.edit"
                  description="The label at the breadcrumb for devices"
                  defaultMessage="Configuration"
                />
              </Link>
              <Typography color="textPrimary">
                <FormattedMessage
                  id="dashboard.devices.edit.breadcrumb.credentials"
                  description="The label at the breadcrumb for editing device's credentials"
                  defaultMessage="Credentials"
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
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : <React.Fragment>
                  <Typography variant="body2" color="textSecondary">
                    <FormattedMessage
                      id="dashboard.device.edit_credentials.instructions"
                      description="The instruction for device's credentials."
                      defaultMessage="You can reconfigure device's credentials. Existing connections are not connected."
                    />
                  </Typography>
                  <SuggestTextField
                    label={intl.formatMessage(messages.keyLabel)}
                    value={key}
                    onChange={onKeyChange}
                    onSuggest={onSuggestKey}
                    suggesting={suggestingKey}
                  />
                  
                </React.Fragment>}
              </CardContent>
              <CardActions>
                <ProgressButton
                  onClick={handleEdit}
                  loading={handlingEdit}
                  disabled={suggestingKey}
                  variant="contained"
                  color="primary"
                >
                  <FormattedMessage
                    id="dashboard.devices.edit_credentials.control.edit"
                    description="The button text for editing devices's credentials"
                    defaultMessage="Save"
                  />
                </ProgressButton>
              </CardActions>
            </Card>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardDeviceCredential = withStyles(styles)(_DashboardDeviceCredential);

export default DashboardDeviceCredential;
