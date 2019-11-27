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
import { Helmet } from "react-helmet";
import BrokerCard from "./BrokerCard";
import DeviceCard from "./DeviceCard";
import BannerNotice from "./BannerNotice";
import { DeviceApiModel, DeviceApiModelFromJSON } from "smeiot-client";
import moment from "moment";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";

const styles = ({
  palette,
  spacing,
  transitions,
  zIndex,
  mixins,
  breakpoints
}: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
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
    }
  });

export interface IDeviceBoard
  extends RouteComponentProps,
    WithStyles<typeof styles> {
  devices: Array<DeviceApiModel>;
  loaded: boolean;
  onBannerClick: React.MouseEventHandler<HTMLButtonElement>;
}

const messages = defineMessages({});

const _DeviceBoard: React.FunctionComponent<IDeviceBoard> = ({
  classes,
  devices,
  loaded,
  onBannerClick
}) => {
  const intl = useIntl();

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const handleMoreClicked = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const unconnectedDeviceNames = devices
    .filter((d: DeviceApiModel) => !d.connected)
    .map(d => d.name);

  const renderDevices = () => {
    return devices.map((d: DeviceApiModel) => (
      <Grid item key={d.name} xs={4}>
        <DeviceCard device={d} onMoreClick={handleMoreClicked} />
      </Grid>
    ));
  };

  return (
    <React.Fragment>
      {unconnectedDeviceNames.length > 0 && (
        <Grid item xs={12}>
          <BannerNotice onClick={onBannerClick}>
            <Typography component="p">
              <FormattedMessage
                id="dashboard.devices.index.unconnected_notice"
                description="Notice related with continuing connecting devices"
                defaultMessage="Notice: your devices {names} are not connected. Continue to connect instead of creating a new one?"
                values={{
                  name: unconnectedDeviceNames[0]
                }}
              />
            </Typography>
          </BannerNotice>
        </Grid>
      )}
      {loaded ? renderDevices() : <Skeleton variant="rect" height={4} />}
      <Menu
        anchorEl={anchorEl}
        keepMounted
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
        <MenuItem
          button
          to="/dashboard/device/L401/edit"
          component={ReachLink}
          onClick={handleClose}
        >
          <FormattedMessage
            id="dashboard.device.actions.configure"
            description="The action for device card."
            defaultMessage="Configure"
          />
        </MenuItem>
        <MenuItem button onClick={handleClose}>
          <FormattedMessage
            id="dashboard.broker.actions.authenticate"
            description="The action for device card."
            defaultMessage="Manage authentication"
          />
        </MenuItem>
        <MenuItem button onClick={handleClose}>
          <FormattedMessage
            id="dashboard.broker.actions.delete"
            description="The action for device card."
            defaultMessage="Delete"
          />
        </MenuItem>
      </Menu>
    </React.Fragment>
  );
};

const DeviceBoard = withStyles(styles)(_DeviceBoard);

export default DeviceBoard;
