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
import ButtonGroup from "@material-ui/core/ButtonGroup";
import Button from "@material-ui/core/Button";

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
    defaultMessage: "Sensor {deviceName}/{sensorName}"
  },
  fabTooltip: {
    id: "dashboard.sensors.index.action.tooltip",
    description: "The tooltip title and aria label for the action button",
    defaultMessage: "Add"
  }
});


const FRAME_PADDING = 24;

const _DashboardSensorDetails: React.FunctionComponent<IDashboardSensorDetails> = ({
  classes,
  deviceName,
  sensorName,
  navigate
}) => {
  if (!deviceName || !sensorName) { throw new Error("Device name and sensor name must be provided"); }
  
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title, { deviceName, sensorName }));

  const [values, setValues] = React.useState<NumberTimeSeriesApiModel[]>([]);

  React.useEffect(() => {
    (async () => {
      const api = new SensorsApi(GetDefaultApiConfig());

      let now = new Date();
      let date = now;
      date.setDate(now.getDate() - 5);
      const res = await api.apiSensorsDeviceNameSensorNameGet({
        deviceName, sensorName,
        startedAt: date.toISOString(),
        duration: "5:00:00:05.123450000"
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

  const initialHeight = 600;
  const containerRef = React.createRef<HTMLElement>();
  const measureRef = React.createRef<HTMLDivElement>();
  const [width, setWidth] = React.useState(-1);
  const [height, setHeight] = React.useState(initialHeight);
  const measureAvailbleViewport = React.useCallback(() => {
    if (measureRef.current && containerRef.current) {
      const docHeight = containerRef.current.getBoundingClientRect().height;
      const measureRect = measureRef.current.getBoundingClientRect();
      setHeight(Math.min(initialHeight, docHeight - measureRect.top - FRAME_PADDING));
      setWidth(measureRect.width);
    }
  }, [measureRef, containerRef, setHeight, setWidth]);
  React.useEffect(() => measureAvailbleViewport(), [measureAvailbleViewport]);

  return (
    <DashboardFrame
      title={intl.formatMessage(messages.title, { deviceName, sensorName })}
      drawer
      direction="ltr"
      toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate}/>}
      ref={containerRef}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Grid container spacing={3}>
            {/* {value !== undefined && value ? value.map(v => <p>{v.value}</p>) : null} */}
            <Grid item xs={12}>
              <ButtonGroup variant="contained" color="default">
                <Button>30D</Button>
                <Button>1D</Button>
                <Button>1H</Button>
                <Button>15M</Button>
                <Button>1M</Button>
              </ButtonGroup>
            </Grid>
            <Grid item xs={12}>
              <Paper ref={measureRef}>
                <NumberGraph width={width} height={height} data={data}/>
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
