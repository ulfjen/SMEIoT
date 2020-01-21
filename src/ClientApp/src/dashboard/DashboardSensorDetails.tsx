import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import NumberGraph from "../components/NumberGraph";
import { defineMessages, useIntl } from "react-intl";
import { RouteComponentProps } from "@reach/router";
import { useTitle } from 'react-use';
import { SensorsApi, NumberTimeSeriesApiModel } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { useAppCookie } from "../helpers/useCookie";
import UserAvatarMenu from "../components/UserAvatarMenu";

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
  sensorName,
  navigate
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const [values, setValues] = React.useState<NumberTimeSeriesApiModel[]>([]);

  React.useEffect(() => {
    (async () => {
      const api = new SensorsApi(GetDefaultApiConfig());
      if (!deviceName || !sensorName) { return; }

      const res = await api.apiSensorsDeviceNameSensorNameGet({
        deviceName, sensorName
      });
      if (res !== null) {
        setValues(res.data || []);
      }
      // setLoading(false);
    })();
  }, [deviceName, sensorName]);

  const data = values.map(v => {
    return {x: new Date(Date.parse(v.createdAt||"")).getTime(), y: v.value}
  });
  const appCookie = useAppCookie();

  return (
    <DashboardFrame
      title="Sensors"
      drawer
      direction="ltr"
      toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate}/>}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Grid container spacing={3}>
            {/* {value !== undefined && value ? value.map(v => <p>{v.value}</p>) : null} */}
            <Grid item xs={12}>
              <Paper>
                <NumberGraph data={data}/>
              </Paper>
            </Grid>
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardSensorDetails = withStyles(styles)(_DashboardSensorDetails);

export default DashboardSensorDetails;
