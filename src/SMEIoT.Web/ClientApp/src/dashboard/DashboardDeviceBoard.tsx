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
import BannerNotice from "../components/BannerNotice";
import { DeviceApiModel, DeviceApiModelFromJSON, DevicesApi } from "smeiot-client";
import moment from "moment";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";
import { GetDefaultApiConfig } from "../index";

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

export interface IDashboardDeviceBoard
  extends WithStyles<typeof styles> {
}

const messages = defineMessages({});

const _DashboardDeviceBoard: React.FunctionComponent<IDashboardDeviceBoard> = ({
  classes,
}) => {
  const intl = useIntl();

  const [loading, setLoading] = React.useState<boolean>(true);
  const [loadingError, setLoadingError] = React.useState<boolean>(false);
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

  const renderDevices = () => {
    return devices.map((d: DeviceApiModel) => (
      <Grid item key={d.name} xs={6} sm={4}>
        <DeviceCard device={d} onMoreClick={handleMoreClicked} />
      </Grid>
    ));
  };

  const [devices, setDevices] = React.useState<Array<DeviceApiModel>>([]);

  const unconnectedDeviceNames = devices
    .filter((d: DeviceApiModel) => !d.connected)
    .map(d => d.name);

  React.useEffect(() => {
    (async () => {
      const api = new DevicesApi(GetDefaultApiConfig());
      var res = await api.apiDevicesGet({
        // start, limit
      });
      if (res !== null && res.devices) {
        setDevices(res.devices);
      } else {
        setLoadingError(true);
      }
      setLoading(false);
    })();
  }, []);


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
      {loading ? <Skeleton variant="rect" height={4} /> : renderDevices()}
      <Menu
        anchorEl={anchorEl}
        keepMounted
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
        <MenuItem
          button
          to={`/dashboard/device/${anchoredDeviceName}/edit`}
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

const DashboardDeviceBoard = withStyles(styles)(_DashboardDeviceBoard);

export default DashboardDeviceBoard;
