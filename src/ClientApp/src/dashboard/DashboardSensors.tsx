import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import Tooltip from "@material-ui/core/Tooltip";
import Fab from "@material-ui/core/Fab";
import * as React from "react";
import AddIcon from "@material-ui/icons/Add";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import DashboardSensorBoard from "./DashboardSensorBoard";
import { defineMessages, useIntl } from "react-intl";
import { Link, RouteComponentProps } from "@reach/router";
import { useTitle } from 'react-use';
import UserAvatarMenu from '../components/UserAvatarMenu';
import { useAppCookie } from "../helpers/useCookie";

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
    }
  });

export interface IDashboardSensors
  extends RouteComponentProps,
  WithStyles<typeof styles> { }

const messages = defineMessages({
  title: {
    id: "dashboard.sensors.index.title",
    description: "Used as title in the sensor index page on the dashboard",
    defaultMessage: "Sensors"
  },
  fabTooltip: {
    id: "dashboard.sensors.index.action.tooltip",
    description: "The tooltip title and aria label for the action button",
    defaultMessage: "Add"
  }
});

const _DashboardSensors: React.FunctionComponent<IDashboardSensors> = ({
  classes, navigate
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const appCookie = useAppCookie();

  return (
    <DashboardFrame
      title={intl.formatMessage(messages.title)}
      drawer={appCookie.admin}
      direction="ltr"
      toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate} />}
      hideHamburgerIcon={!appCookie.admin}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Grid container spacing={3}>
            <DashboardSensorBoard />
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardSensors = withStyles(styles)(_DashboardSensors);

export default DashboardSensors;
