import Grid from "@material-ui/core/Grid";
import * as React from "react";
import Typography from "@material-ui/core/Typography";
import Skeleton from "@material-ui/lab/Skeleton";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import clsx from "clsx";
import { useAsync } from 'react-use';
import Card from "@material-ui/core/Card";
import CardHeader from "@material-ui/core/CardHeader";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import BannerNotice from "../components/BannerNotice";
import IconButton from "@material-ui/core/IconButton";
import Button from "@material-ui/core/Button";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import { BasicDeviceApiModel, BasicDeviceApiModelList, DevicesApi } from "smeiot-client";
import moment from "moment";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  NavigateFn
} from "@reach/router";
import { GetDefaultApiConfig } from "../index";
import useMenu from "../helpers/useMenu";
import useModal from "../helpers/useModal";
import DashboardDeviceMenu from "./DashboardDeviceMenu";
import DashboardDeviceDialog from "./DashboardDeviceDialog";
import { darken, fade } from "@material-ui/core/styles/colorManipulator";

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
    maxWidth: 345,
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
    backgroundColor: palette.type === 'light' ? darken(palette.background.paper, 0.1) : fade(palette.background.paper, 0.1)
  }
});

export interface IDashboardDeviceBoard extends WithStyles<typeof styles> {
  navigate?: NavigateFn;
}

const messages = defineMessages({
  connected: {
    timed: {
      id: "dashboard.devices.index.card.connected.timed",
      description: "The text for device instaruction",
      defaultMessage: "Running normally. Last message {ago}."
    },
    notYet: {
      id: "dashboard.devices.index.card.connected.not_yet",
      description: "The text for device instaruction",
      defaultMessage: "Running normally. We haven't received its messages yet."
    }
  },
  notConnected: {
    id: "dashboard.devices.index.card.not_connected",
    description: "The text for device instaruction",
    defaultMessage: "The device is disconnected."
  }

});

const _DashboardDeviceBoard: React.FunctionComponent<IDashboardDeviceBoard> = ({
  classes, navigate
}) => {
  const intl = useIntl();

  const [menuOpen, anchorEl, openMenu, closeMenu, menuDeviceName] = useMenu<string>();
  const [dialogOpen, openDialog, closeDialog, dialogDeviceName] = useModal<string>();

  const handleMoreClicked = (e: React.MouseEvent<HTMLButtonElement>, deviceName?: string) => {
    e.preventDefault();
    openMenu(e.currentTarget, deviceName || "");
  };

  const renderDevice = (device: BasicDeviceApiModel | undefined, summary: string) => {
    const deviceClass = device && !device.connected ? clsx(classes.card, classes.notConnected) : classes.card;
    const key = device && device.name;

    return <Grid item key={key} xs={12} md={4}>
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
            />
        }

        <CardContent>
          {
            !device ?
              <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div>
              :
              <Typography variant="body2" color="textSecondary">
                {summary}
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
          <Button size="small" component={ReachLink} to={`/dashboard/broker/logs?filter=iot/${device.name}`}>
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
      let summary = "";
      if (d.connected) {
        if (d.lastMessageAt) {
          summary = intl.formatMessage(messages.connected.timed, { ago: moment(d.lastMessageAt).fromNow() });
        } else {
          summary = intl.formatMessage(messages.connected.notYet);
        }
      } else {
        summary = intl.formatMessage(messages.notConnected);
      }
      return renderDevice(d, summary);
    });
  };

  const [devices, setDevices] = React.useState<Array<BasicDeviceApiModel>>([]);

  const unconnectedDeviceNames = devices
    .filter((d: BasicDeviceApiModel) => !d.connected)
    .map(d => d.name)
    .slice(0, 3);

  const state = useAsync(async () => {
    return new DevicesApi(GetDefaultApiConfig()).apiDevicesGet({
      // start, limit
    }).then((res: BasicDeviceApiModelList) => {
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
          <BannerNotice to={`/dashboard/devices/wait_connection?name=${unconnectedDeviceNames[0]}`}>
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
        open={menuOpen}
        anchorEl={anchorEl}
        deviceName={menuDeviceName}
        closeMenu={closeMenu}
        navigate={navigate}
        openDialog={openDialog}
      />
      <DashboardDeviceDialog
        open={dialogOpen}
        deviceName={dialogDeviceName}
        closeDialog={closeDialog}
        navigate={undefined}
        navigateUrl={"."}
      />
    </React.Fragment>
  );
};

const DashboardDeviceBoard = withStyles(styles)(_DashboardDeviceBoard);

export default DashboardDeviceBoard;
