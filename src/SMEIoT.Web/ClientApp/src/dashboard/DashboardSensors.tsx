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
import clsx from "clsx";
import UserAvatarMenu from "../components/UserAvatarMenu";
import { BasicUserApiModel, SensorDetailsApiModel, SensorDetailsApiModelFromJSON } from "smeiot-client";
import moment from "moment";
import SensorCard from "../components/SensorCard";
import SensorBoard from "../components/SensorBoard";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { Link, RouteComponentProps } from "@reach/router";
import { Helmet } from "react-helmet";

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
    }
  });

export interface IDashboardSensors
  extends RouteComponentProps,
    WithStyles<typeof styles> {}

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
  classes
}) => {
  const intl = useIntl();
  const [sensors, setSensors] = React.useState<Array<SensorDetailsApiModel>>([
    SensorDetailsApiModelFromJSON({
      values: [0.1, 0.2, 0.33],
      sensorName: "L401/temp1"
    }),
    SensorDetailsApiModelFromJSON({
      values: [0.14, 0.22, 0.33],
      sensorName: "L401/temp2"
    })
  ]);

  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
  return (
    <Frame
      title="Sensors"
      direction="ltr"
      toolbarRight={null}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Helmet>
            <title>{intl.formatMessage(messages.title)}</title>
          </Helmet>
          <Grid container spacing={3}>
            <SensorBoard sensors={sensors} loaded={true} />
          </Grid>
          <Tooltip
            title={intl.formatMessage(messages.fabTooltip)}
            aria-label={intl.formatMessage(messages.fabTooltip)}
          >
            <Fab
              color="secondary"
              className={classes.absolute}
              to={"/dashboard/sensors/new"}
              component={Link}
            >
              <AddIcon />
            </Fab>
          </Tooltip>
        </Container>
      }
    />
  );
};

const DashboardSensors = withStyles(styles)(_DashboardSensors);

export default DashboardSensors;
