import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import InteractiveNumberGraph from "../components/InteractiveNumberGraph";
import { defineMessages, useIntl } from "react-intl";
import { RouteComponentProps } from "@reach/router";
import { useTitle } from 'react-use';
import { SensorsApi, NumberTimeSeriesApiModel } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { useAppCookie } from "../helpers/useCookie";
import UserAvatarMenu from "../components/UserAvatarMenu";
import ButtonGroup from "@material-ui/core/ButtonGroup";
import Button from "@material-ui/core/Button";
import CircularProgress from "@material-ui/core/CircularProgress";
import IDimension from "../models/IDimension";
import Tooltip from "@material-ui/core/Tooltip";

const styles = ({
  spacing,
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
    loading: {
      textAlign: "center",
      padding: spacing(10, 10)
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
  },
  times: {
    all: {
      tooltip: {
        id: "dashboard.sensors.index.times.all.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "All values of sensors"
      },
      label: {
        id: "dashboard.sensors.index.times.all.label",
        description: "The label for the action button",
        defaultMessage: "All"
      }
    },
    days30: {
      tooltip: {
        id: "dashboard.sensors.index.times.days30.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "Last 30 days from the end of current graph"
      },
      label: {
        id: "dashboard.sensors.index.times.days30.label",
        description: "The label for the action button",
        defaultMessage: "30D"
      }
    },
    day: {
      tooltip: {
        id: "dashboard.sensors.index.times.day.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "Last day from the end of current graph"
      },
      label: {
        id: "dashboard.sensors.index.times.day.label",
        description: "The label for the action button",
        defaultMessage: "1D"
      }
    },
    hour: {
      tooltip: {
        id: "dashboard.sensors.index.times.hour.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "Last hour from the end of current graph"
      },
      label: {
        id: "dashboard.sensors.index.times.days30.label",
        description: "The label for the action button",
        defaultMessage: "1H"
      }
    },
    mins15: {
      tooltip: {
        id: "dashboard.sensors.index.times.mins15.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "Last 15 minutes from the end of current graph"
      },
      label: {
        id: "dashboard.sensors.index.times.mins15.label",
        description: "The label for the action button",
        defaultMessage: "15M"
      }
    },
    min: {
      tooltip: {
        id: "dashboard.sensors.index.times.min.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "Last minute from the end of current graph"
      },
      label: {
        id: "dashboard.sensors.index.times.min.label",
        description: "The label for the action button",
        defaultMessage: "1M"
      }
    },
    last: {
      tooltip: {
        id: "dashboard.sensors.index.times.last.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "Last 2 minutes with values"
      },
      label: {
        id: "dashboard.sensors.index.times.last.label",
        description: "The label for the action button",
        defaultMessage: "Last"
      }
    },
    now: {
      tooltip: {
        id: "dashboard.sensors.index.times.now.tooltip",
        description: "The tooltip title and aria label for the action button",
        defaultMessage: "A minute to current time"
      },
      label: {
        id: "dashboard.sensors.index.times.now.label",
        description: "The label for the action button",
        defaultMessage: "Now"
      }
    }
  }
});


const FRAME_PADDING = 24;

function getLastDateFromDimension(dimension: IDimension | null): Date | undefined {
  return dimension ? dimension.right : undefined;
}

interface FetchedRange {
  startedAt: number;
  endedAt: number;
}

const _DashboardSensorDetails: React.FunctionComponent<IDashboardSensorDetails> = ({
  classes,
  deviceName,
  sensorName,
  navigate
}) => {
  if (!deviceName || !sensorName) { throw new Error("Device name and sensor name must be provided"); }
  
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title, { deviceName, sensorName }));

  const now = React.useRef<Date>(new Date());

  const [values, setValues] = React.useState<NumberTimeSeriesApiModel[]>([]);
  const [dataRange, setDataRange] = React.useState<FetchedRange>({
    startedAt: now.current.getTime(),
    endedAt: now.current.getTime()
  });

  const [loading, setLoading] = React.useState<boolean>(false);

  React.useEffect(() => {
    (async () => {
      const api = new SensorsApi(GetDefaultApiConfig());
      setLoading(true);
      await api.apiSensorsDeviceNameSensorNameLastGet({
        deviceName, sensorName,
        seconds: 120
      }).then(res => {
        if (res !== null && res.data.length > 0) {
          setValues(res.data);
          const lastDate = Date.parse(res.data[res.data.length-1].createdAt);
          const firstDate = new Date(lastDate);
          firstDate.setSeconds(firstDate.getSeconds() - 120);
          setDataRange({
            startedAt: firstDate.getTime(),
            endedAt: lastDate
          });
        }
      }).finally(() => {
        setLoading(false);
      });
    })();
  }, [deviceName, sensorName]);

  let maxVal = Number.MIN_SAFE_INTEGER;
  let minVal = Number.MAX_SAFE_INTEGER;
  const data = values.map(v => {
    maxVal = Math.max(maxVal, v.value);
    minVal = Math.min(minVal, v.value);
    return {x: new Date(v.createdAt), y: v.value}
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

  const [dimension, setDimension] = React.useState<IDimension | null>(null);

  React.useEffect(() => {
    if (dimension === null || (dimension.left.getTime() >= dataRange.startedAt && dimension.right.getTime() <= dataRange.endedAt)) {
      return;
    }
    (async () => {
      const api = new SensorsApi(GetDefaultApiConfig());
      setLoading(true);
      const firstDate = dimension.left;
      const lastDate = dimension.right;
      const duration = new Date(lastDate.getTime() - firstDate.getTime());
      await api.apiSensorsDeviceNameSensorNameGet({
        deviceName, sensorName,
        startedAt: dimension.left.toISOString(),
        duration: `${duration.getDay()}:${duration.getHours().toString().padStart(2, '0')}:${duration.getMinutes().toString().padStart(2, '0')}:${duration.getSeconds().toString().padStart(2, '0')}.${duration.getMilliseconds().toString().padStart(9, '0')}`
      }).then(res => {
        if (res !== null && res.data.length > 0) {
          setValues(res.data);
          setDataRange({
            startedAt: firstDate.getTime(),
            endedAt: lastDate.getTime()
          });
        }
      }).finally(() => {
        setLoading(false);
      });
    })();
  }, [dimension, dataRange, deviceName, sensorName]);
  
  const handleDuration = (lastMoment: Date | undefined, seconds: number, now: Date) => {

    if (lastMoment === undefined) {
      lastMoment = data.length > 0 ? data[data.length-1].x : now;
    }

    console.log(lastMoment, dimension, seconds);

    let firstMoment = new Date(lastMoment.getTime());
    firstMoment.setSeconds(firstMoment.getSeconds() - seconds);
    setDimension({
      top: dimension ? dimension.top : maxVal,
      bottom: dimension ? dimension.bottom : minVal,
      left: firstMoment,
      right: lastMoment
    });
  };
  return (
    <DashboardFrame
      title={intl.formatMessage(messages.title, { deviceName, sensorName })}
      drawer={appCookie.admin}
      direction="ltr"
      toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate}/>}
      ref={containerRef}
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Grid container spacing={3}>
            {/* {value !== undefined && value ? value.map(v => <p>{v.value}</p>) : null} */}
            <Grid item xs={12}>
              <ButtonGroup variant="contained" color="default">
                <Tooltip title={intl.formatMessage(messages.times.all.tooltip)} aria-label={intl.formatMessage(messages.times.all.tooltip)}>
                  <Button onClick={() => handleDuration(getLastDateFromDimension(dimension), 60*60*24*365*1000, now.current)}>{intl.formatMessage(messages.times.all.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.days30.tooltip)} aria-label={intl.formatMessage(messages.times.days30.tooltip)}>
                  <Button onClick={() => handleDuration(getLastDateFromDimension(dimension), 60*60*24*30, now.current)}>{intl.formatMessage(messages.times.days30.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.day.tooltip)} aria-label={intl.formatMessage(messages.times.day.tooltip)}>
                  <Button onClick={() => handleDuration(getLastDateFromDimension(dimension), 60*60*24, now.current)}>{intl.formatMessage(messages.times.day.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.hour.tooltip)} aria-label={intl.formatMessage(messages.times.hour.tooltip)}>
                  <Button onClick={() => handleDuration(getLastDateFromDimension(dimension), 60*60, now.current)}>{intl.formatMessage(messages.times.hour.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.mins15.tooltip)} aria-label={intl.formatMessage(messages.times.mins15.tooltip)}>
                  <Button onClick={() => handleDuration(getLastDateFromDimension(dimension), 60*15, now.current)}>{intl.formatMessage(messages.times.mins15.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.min.tooltip)} aria-label={intl.formatMessage(messages.times.min.tooltip)}>
                  <Button onClick={() => handleDuration(getLastDateFromDimension(dimension), 60, now.current)}>{intl.formatMessage(messages.times.min.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.last.tooltip)} aria-label={intl.formatMessage(messages.times.last.tooltip)}>
                  <Button onClick={() => handleDuration(undefined, 60, now.current)}>{intl.formatMessage(messages.times.last.label)}</Button>
                </Tooltip>
                <Tooltip title={intl.formatMessage(messages.times.now.tooltip)} aria-label={intl.formatMessage(messages.times.now.tooltip)}>
                  <Button onClick={() => handleDuration(now.current, 60, now.current)}>{intl.formatMessage(messages.times.now.label)}</Button>
                </Tooltip>
              </ButtonGroup>
            </Grid>
            <Grid item xs={12}>
              <Paper ref={measureRef}>
                {loading ? <div className={classes.loading}><CircularProgress /></div> :
                  <InteractiveNumberGraph dimension={dimension} setDimension={setDimension} width={width} height={height} data={data}/>}
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
