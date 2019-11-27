import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import Tooltip from "@material-ui/core/Tooltip";
import Fab from "@material-ui/core/Fab";
import * as React from "react";
import AddIcon from "@material-ui/icons/Add";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import { Helmet } from "react-helmet";
import BrokerCard from "../components/BrokerCard";
import DeviceBoard from "../components/DeviceBoard";
import {
  DeviceApiModel,
  DeviceApiModelFromJSON
} from "smeiot-client";
import { defineMessages, useIntl } from "react-intl";
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

export interface IDashboardDevices
  extends RouteComponentProps,
    WithStyles<typeof styles> {}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.index.title",
    description: "Used as title in the devices index page on the dashboard",
    defaultMessage: "Devices"
  },
  fabTooltip: {
    id: "dashboard.devices.index.action.tooltip",
    description: "The tooltip title and aria label for the action button",
    defaultMessage: "Add"
  }
});

const _DashboardDevices: React.FunctionComponent<IDashboardDevices> = ({
  classes
}) => {
  const intl = useIntl();
  const loaded = true;


  const [devices, setDevices] = React.useState<Array<DeviceApiModel>>([
    DeviceApiModelFromJSON({
      name: "L401",
      createdAt: "2019-01-01",
      updatedAt: "2020-01-01",
      authenticationType: 0,
      preSharedKey: "aaaaaaaaaaaaaaaaaa111",
      connected: true,
      connectedAt: "2020-01-01",
      lastMessageAt: "2020-01-01"
    }),
    DeviceApiModelFromJSON({
      name: "L402",
      createdAt: "2019-01-01",
      updatedAt: "2020-01-01",
      authenticationType: 0,
      preSharedKey: "aaaaaaaaaaaaaaaaaa112",
      connected: true,
      connectedAt: "2020-01-01",
      lastMessageAt: "2020-01-01"
    }),
    DeviceApiModelFromJSON({
      name: "L403",
      createdAt: "2019-01-01",
      updatedAt: "2020-01-01",
      authenticationType: 0,
      preSharedKey: "aaaaaaaaaaaaaaaaaa112",
      connected: false,
      connectedAt: "2020-01-01",
      lastMessageAt: "2020-01-01"
    })
  ]);

  const onContinueBannerClick = async (
    event: React.MouseEvent<HTMLButtonElement>
  ) => {
    // setLoadingConnect(true);
    // setActiveStep(1);
    // setHandlingNext(false);
    // if (unconnectedDeviceName !== null) {
    //   const api = new DevicesApi(GetDefaultApiConfig());
    //   const res = await api.apiDevicesNameGet({
    //     name: unconnectedDeviceName
    //   });
    //   if (res !== null) {
    //     setDevice(res);
    //   }
    // }
    // setLoadingConnect(false);
  };

  return (
    <Frame
      title={intl.formatMessage(messages.title)}
      direction="ltr"
      toolbarRight={null}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Helmet>
            <title>{intl.formatMessage(messages.title)}</title>
          </Helmet>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <BrokerCard />
            </Grid>
            <DeviceBoard onBannerClick={onContinueBannerClick} devices={devices} loaded={true} />
          </Grid>
          <Tooltip
            title={intl.formatMessage(messages.fabTooltip)}
            aria-label={intl.formatMessage(messages.fabTooltip)}
          >
            <Fab
              color="secondary"
              className={classes.absolute}
              to={"/dashboard/devices/new"}
              component={ReachLink}
            >
              <AddIcon />
            </Fab>
          </Tooltip>
        </Container>
      }
    />
  );
};

const DashboardDevices = withStyles(styles)(_DashboardDevices);

export default DashboardDevices;
