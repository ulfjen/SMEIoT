import Grid from "@material-ui/core/Grid";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import * as React from "react";
import AddIcon from "@material-ui/icons/Add";
import Typography from "@material-ui/core/Typography";
import Skeleton from "@material-ui/lab/Skeleton";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import clsx from "clsx";
import { useTitle, useAsync } from 'react-use';
import BrokerCard from "./BrokerCard";
import Card from "@material-ui/core/Card";
import CardHeader from "@material-ui/core/CardHeader";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import BannerNotice from "../components/BannerNotice";
import IconButton from "@material-ui/core/IconButton";
import Button from "@material-ui/core/Button";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import { BasicDeviceApiModel, BasicDeviceApiModelFromJSON, DevicesApi } from "smeiot-client";
import moment from "moment";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps,
  NavigateFn
} from "@reach/router";
import { GetDefaultApiConfig } from "../index";
import { DeviceApiModelList } from "smeiot-client/dist/models/DeviceApiModelList";
import DashboardDeviceMenu from "./DashboardDeviceMenu";

const styles = ({
  palette,
  spacing,
  transitions,
  zIndex,
  mixins,
  breakpoints
}: Theme) => createStyles({
  container: {
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
  absolute: {
    position: "absolute",
    bottom: spacing(2),
    right: spacing(3)
  },
  list: {},
  card: {
    maxWidth: 345
  },
  media: {
    height: 0,
    paddingTop: "56.25%" // 16:9
  },
  expand: {
    transform: "rotate(0deg)",
    marginLeft: "auto",
    transition: transitions.create("transform", {
      duration: transitions.duration.shortest
    })
  },
  expandOpen: {
    transform: "rotate(180deg)"
  },
  avatar: {
  },
  notConnected: {
    backgroundColor: "#eeeeee", // needs to dim other component
  }
});

export interface IDashboardDeviceBoard extends WithStyles<typeof styles> {
  navigate?: NavigateFn;
}

const messages = defineMessages({});

const _DashboardDeviceBoard: React.FunctionComponent<IDashboardDeviceBoard> = ({
  classes, navigate
}) => {
  const intl = useIntl();

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [anchoredDeviceName, setAnchoredDeviceName] = React.useState<string>("");
  const handleMoreClicked = (event: React.MouseEvent<HTMLButtonElement>, deviceName?: string) => {
    setAnchorEl(event.currentTarget);
    if (deviceName) {
      setAnchoredDeviceName(deviceName);
    }
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const renderDevice = (device: BasicDeviceApiModel | undefined, summary: string) => {
    const deviceClass = device && !device.connected ? clsx(classes.card, classes.notConnected) : classes.card;
    const key = device && device.name;

    return <Grid item key={key} xs={4} sm={6}>
      <Card className={deviceClass}>
        {
          !device ?
            <CardHeader
              title={<Skeleton variant="rect" width={120} height={32} />}
              subheader={<Skeleton variant="text" />}
            /> :
            <CardHeader
              action={
                <IconButton aria-label="settings" onClick={(event) => handleMoreClicked(event, device.name)}>
                  <MoreVertIcon />
                </IconButton>
              }
              title={device.name}
              subheader={summary}
            />
        }

        <CardContent>
          {
            !device ?
              <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div>
              :
              <Typography variant="body2" color="textSecondary" component="p">
                instruction space
        </Typography>
          }

        </CardContent>
        {device && <CardActions disableSpacing>
          <Button size="small" component={ReachLink} to={`/dashboard/sensors?device_name=${device.name}`}>
            <FormattedMessage
              id="dashboard.device.actions.see_sensor"
              description="The action to go to see sensors in the device card."
              defaultMessage="Sensors"
            />
          </Button>
          <Button size="small" component={ReachLink} to={`/dashboard/broker/logs?device_name=${device.name}`}>
            <FormattedMessage
              id="dashboard.device.actions.logs"
              description="The action for device cards."
              defaultMessage="Logs"
            />
          </Button>
        </CardActions>}
      </Card>
    </Grid >;
  }

  const renderDevices = () => {
    return devices.map((d: BasicDeviceApiModel) => {
      var summary = "";
      if (d.name === "L401") {
        summary = "Connected with temp1";
      } else if (d.name === "L402") {
        summary = "No sensor connected.";
      } else {
        summary = "Device is not configured.";
      }
      return renderDevice(d, summary);
    });
  };

  const [devices, setDevices] = React.useState<Array<BasicDeviceApiModel>>([]);

  const unconnectedDeviceNames = devices
    .filter((d: BasicDeviceApiModel) => !d.connected)
    .map(d => d.name);


  const state = useAsync(async () => {
    return new DevicesApi(GetDefaultApiConfig()).apiDevicesGet({
      // start, limit
    }).then((res: DeviceApiModelList) => {
      if (!res.devices) {
        return;
      }

      setDevices(res.devices);
      return res;
    });
  });

  return (
    <React.Fragment>
      {unconnectedDeviceNames.length > 0 && (
        <Grid item xs={12}>
          <BannerNotice to={`/dashboard/devices/new/connect?name=${unconnectedDeviceNames[0]}`}>
            <Typography component="p">
              <FormattedMessage
                id="dashboard.devices.index.unconnected_notice"
                description="Notice related with continuing connecting devices"
                defaultMessage="Notice: your devices {names} are not connected. Continue to connect instead of creating a new one?"
                values={{
                  names: unconnectedDeviceNames.join(", ")
                }}
              />
            </Typography>
          </BannerNotice>
        </Grid>
      )}
      {state.loading ? renderDevice(undefined, "") : renderDevices()}
      <DashboardDeviceMenu
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        deviceName={anchoredDeviceName}
        closeMenu={handleClose}
        navigate={navigate}
      />
    </React.Fragment>
  );
};

const DashboardDeviceBoard = withStyles(styles)(_DashboardDeviceBoard);

export default DashboardDeviceBoard;
