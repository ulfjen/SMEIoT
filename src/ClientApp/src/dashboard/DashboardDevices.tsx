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
import DashboardFrame from "./DashboardFrame";
import { useTitle } from 'react-use';
import ErrorBoundary from "../components/ErrorBoundary";
import BrokerCard from "./BrokerCard";
import DashboardDeviceBoard from "./DashboardDeviceBoard";
import {
  BasicDeviceApiModel,
  DevicesApi
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { defineMessages, useIntl } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";
import { useAppCookie } from "../helpers/useCookie";
import UserAvatarMenu from '../components/UserAvatarMenu';

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
  classes, navigate
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));
  
  const [loading, setLoading] = React.useState<boolean>(true);
  const [loadingError, setLoadingError] = React.useState<boolean>(false);

  const appCookie = useAppCookie();

  return (
    <DashboardFrame
      title={intl.formatMessage(messages.title)}
      drawer
      direction="ltr"
      toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate}/>}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <ErrorBoundary>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <BrokerCard />
              </Grid>
              <DashboardDeviceBoard navigate={navigate} />
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
          </ErrorBoundary>
        </Container>
      }
    />
  );
};

const DashboardDevices = withStyles(styles)(_DashboardDevices);

export default DashboardDevices;
