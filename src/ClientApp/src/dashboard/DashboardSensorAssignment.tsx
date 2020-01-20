import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { darken } from '@material-ui/core/styles';
import Grid from "@material-ui/core/Grid";
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
  SensorsApi,
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import DashboardFrame from "./DashboardFrame";
import Container from "@material-ui/core/Container";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import { Link as ReachLink } from "@reach/router";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Link from '@material-ui/core/Link';
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import { AsyncState } from "react-use/lib/useAsync";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import useMenu from "../helpers/useMenu";
import useModal from "../helpers/useModal";
import DashboardSensorMenu from "./DashboardSensorMenu";
import DashboardSensorDialog from "./DashboardSensorDialog";
import CardActions from "@material-ui/core/CardActions";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";

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


export interface IDashboardSensorAssignmentParams {
  deviceName: string;
  sensorName: string;
}

export interface IDashboardSensorAssignmentProps extends RouteComponentProps<IDashboardSensorAssignmentParams>, WithStyles<typeof styles> {

}

const messages = defineMessages({
  title: {
    id: "dashboard.sensors.assign.title",
    description: "Used as title in the edit sensor assignment page on the dashboard",
    defaultMessage: "Who can see {name}?"
  },
  keyLabel: {
    id: "dashboard.sensors.assign.key_label",
    description: "The label for editing psk key",
    defaultMessage: "Key"
  },
  closeAriaLabel: {
    id: "dashboard.sensors.edit.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  },
  moreAria: {
    id: "dashboard.sensors.edit.more",
    description: "The aria label for more action",
    defaultMessage: "More"
  },
  status: {
    connected: {
      id: "dashboard.sensors.edit.status.connected",
      description: "The status for sensor connected",
      defaultMessage: "Connected"
    },
    notConnected: {
      id: "dashboard.sensors.edit.status.not_connected",
      description: "The status for sensor not connected",
      defaultMessage: "Unknown"
    }
  },
});

const _DashboardSensorAssignment: React.FunctionComponent<IDashboardSensorAssignmentProps> = ({
  classes,
  sensorName,
  deviceName,
  navigate
}) => {
  if (!sensorName || !deviceName) {
    throw new Error("No sensor is assigned to the route.");
  }
  const intl = useIntl();
  const title = intl.formatMessage(messages.title, { name: sensorName });
  useTitle(title);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [menuOpen, anchorEl, openMenu, closeMenu, menuSensorName] = useMenu<string>();
  const [dialogOpen, openDialog, closeDialog, dialogSensorName] = useModal<string>();

  const api = new SensorsApi(GetDefaultApiConfig());

  const state: AsyncState<SensorDetailsApiModel> = useAsync(async () => await api.apiSensorsDeviceNameSensorNameGet({
    deviceName, sensorName
  }).then((res) => {
    return res;
  }));

  // const [handlingEdit, setHandlingEdit] = React.useState<boolean>(false);

  // const handleEdit = async () => {
  //   setHandlingEdit(true);
  //   const api = new DevicesApi(GetDefaultApiConfig());
  //   await api.apiDevicesNamePut({
  //     name: deviceName,
  //     deviceConfigBindingModel: {
  //       key
  //     }
  //   }).then(res => {
  //     return res;
  //   }).catch(async res => {
  //     const pd = await res.json();
  //     const err = pd.errors;
  //     if (!err) { return; }
  //     if (err.hasOwnProperty("key")) { setKeyError(err["key"].join("\n")); }
  //   }).finally(() => {
  //     setHandlingEdit(false);
  //   });
  // };

  return <DashboardFrame
    title={title}
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
        <DashboardSensorMenu
          open={menuOpen}
          anchorEl={anchorEl}
          sensorName={menuSensorName}
          closeMenu={closeMenu}
          navigate={navigate}
          openDialog={openDialog}
        />
        <DashboardSensorDialog
          open={dialogOpen}
          deviceName={deviceName}
          sensorName={dialogSensorName}
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
                  id="dashboard.devices.edit.breadcrumb.sensor_assignment"
                  description="The label at the breadcrumb for editing sensors's assignment"
                  defaultMessage="Sensor Assignment"
                />
              </Typography>
            </Breadcrumbs>
          </Grid>
          <Grid item xs={12}>
            <Card>
              <ExpandedCardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={30} /> : state.value && <TwoLayerLabelAction greyoutFirst first={deviceName} second={sensorName} />}
                action={!state.loading &&
                  <IconButton
                    aria-label={intl.formatMessage(messages.moreAria)}
                    onClick={(e) => openMenu(e.currentTarget, state.value ? state.value.sensorName : "")}
                  >
                    <MoreVertIcon />
                  </IconButton>
                }
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : <React.Fragment>
                  <Typography variant="body2" color="textSecondary">
                    <FormattedMessage
                      id="dashboard.sensors.assign.instructions"
                      description="The instruction for sensor's assignment."
                      defaultMessage="You can configure who can see this sensor. All administators can see this sensor."
                    />
                  </Typography>
                </React.Fragment>}
              </CardContent>
              <CardActions>
                {/* <ProgressButton
                  onClick={handleEdit}
                  loading={handlingEdit}
                  disabled={suggestingKey}
                  variant="contained"
                  color="primary"
                >
                  <FormattedMessage
                    id="dashboard.sensors.assign.control.edit"
                    description="The button text for editing devices's credentials"
                    defaultMessage="Save"
                  />
                </ProgressButton> */}
              </CardActions>
            </Card>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardSensorAssignment = withStyles(styles)(_DashboardSensorAssignment);

export default DashboardSensorAssignment;
