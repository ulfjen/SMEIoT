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
import DashboardSensorBoard from "./DashboardSensorBoard";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { Link, RouteComponentProps } from "@reach/router";
import { Helmet } from "react-helmet";
import { SensorsApi } from "smeiot-client";
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
    }
  });

export interface IDashboardSensorDetailsRouteParams {
  deviceName: string;
  sensorName: string;
}
  
export interface IDashboardSensorDetails
  extends RouteComponentProps<IDashboardSensorDetailsRouteParams>,
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

const _DashboardSensorDetails: React.FunctionComponent<IDashboardSensorDetails> = ({
  classes,
  deviceName,
  sensorName
}) => {
  const intl = useIntl();

  const [value, setValues] = React.useState<number[] | undefined>();

  React.useEffect(() => {
    (async () => {
      const api = new SensorsApi(GetDefaultApiConfig());
      if (!deviceName || !sensorName) { return; }

      const res = await api.apiSensorsDeviceNameSensorNameGet({
        deviceName, sensorName
      });
      if (res !== null) {
        setValues(res.values);
      }
      // setLoading(false);
    })();
  }, []);

  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
  return (
    <Frame
      title="Sensors"
      direction="ltr"
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Helmet>
            <title>{intl.formatMessage(messages.title)}</title>
          </Helmet>
          <Grid container spacing={3}>
            {value !== undefined && value ? value.map(v => <p key={v}>{v}</p>) : null}
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardSensorDetails = withStyles(styles)(_DashboardSensorDetails);

export default DashboardSensorDetails;
