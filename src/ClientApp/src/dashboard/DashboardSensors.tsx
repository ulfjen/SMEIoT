import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import { RouteComponentProps } from "@reach/router";
import { useTitle } from 'react-use';
import UserAvatarMenu from '../components/UserAvatarMenu';
import { useAppCookie } from "../helpers/useCookie";
import { SensorDetailsApiModel, SensorsApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import useInterval from "../helpers/useInterval";
import Card from "@material-ui/core/Card";
import CardActionArea from "@material-ui/core/CardActionArea";
import CardMedia from "@material-ui/core/CardMedia";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import NumberGraph from "../components/NumberGraph";
import Pagination from "material-ui-flat-pagination";
import moment from "moment";
import { FormattedMessage, defineMessages, useIntl } from "react-intl";
import {
  Link as ReachLink
} from "@reach/router";
import Skeleton from "@material-ui/lab/Skeleton";
import ErrorBoundary from "../components/ErrorBoundary";
import placeholder from "../images/placeholder400.jpg";

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
    sensorCard: {

    },
    media: {
      height: 200,
    }
  });

interface IDashboardSensorRoutes {
  page: number;
}

export interface IDashboardSensors extends RouteComponentProps<IDashboardSensorRoutes>, WithStyles<typeof styles> {
}

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
  },
  lastMessage: {
    id: "dashboard.sensors.index.last_messages",
    description: "Used as card secondary title in the sensor index page on the dashboard",
    defaultMessage: "Last value {last}"
  },
  noMessage: {
    id: "dashboard.sensors.index.no_messages",
    description: "Used as card secondary title in the sensor index page on the dashboard",
    defaultMessage: "Nothing stored."
  }
});

const _DashboardSensors: React.FunctionComponent<IDashboardSensors> = ({
  classes, navigate, location
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const appCookie = useAppCookie();

  const [loading, setLoading] = React.useState<boolean>(true);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [loadingError, setLoadingError] = React.useState<boolean>(false);
  let page = 1;
  if (location) {
    const pageQ = new URLSearchParams(location.search).get("page");
    if (pageQ && isNaN(parseInt(pageQ))) {
      page = parseInt(pageQ);
    }
  }

  const [sensors, setSensors] = React.useState<Array<SensorDetailsApiModel>>([]);
  const [total, setTotal] = React.useState<number>(0);

  useInterval(async () => {
    const api = new SensorsApi(GetDefaultApiConfig());
    setLoading(true);
    var res = await api.apiSensorsGet({
      offset: 0 * 10,
      limit: 10
    });
    if (res !== null && res.sensors) {
      setSensors(res.sensors);
      setTotal(res.total);
    } else {
      setLoadingError(true);
    }
    setLoading(false);
  }, 30000);

  const measureRef = React.createRef<HTMLDivElement>();
  const [width, setWidth] = React.useState(-1);
  const measureAvailbleViewport = React.useCallback(() => {
    if (measureRef.current) {
      const measureRect = measureRef.current.getBoundingClientRect();
      setWidth(measureRect.width);
    }
  }, [measureRef, setWidth]);
  React.useEffect(() => measureAvailbleViewport(), [measureAvailbleViewport]);

  const redirectToSensor = (deviceName: string, sensorName: string) => {
    navigate && navigate(`${deviceName}/${sensorName}`);
  }

  const renderSensor = (sensor: SensorDetailsApiModel, idx: number) => {
    const data = sensor.data.map(v => {
      return { x: new Date(Date.parse(v.createdAt)).getTime(), y: v.value }
    });
    return <Card ref={idx === 0 ? measureRef : undefined} key={idx} className={classes.sensorCard}>
      <CardActionArea onClick={() => redirectToSensor(sensor.deviceName, sensor.sensorName)}>
        {data.length === 0 ? <CardMedia
          component="img"
          title="placeholder"
          image={placeholder}
          height={200}
        /> : <CardMedia
          component="div"
        >
          <NumberGraph width={width} height={200} data={data} />
        </CardMedia>}
        <CardContent>
          <TwoLayerLabelAction first={sensor.deviceName} second={sensor.sensorName} />
          <Typography variant="body2" color="textSecondary" component="p">
            {sensor.data.length > 0 ? intl.formatMessage(messages.lastMessage, { last: moment(sensor.data[sensor.data.length - 1].createdAt).fromNow() }) : intl.formatMessage(messages.noMessage)}
          </Typography>
        </CardContent>
      </CardActionArea>
      <CardActions>
        {appCookie.admin && <Button size="small" color="default" component={ReachLink} to={`../devices/${sensor.deviceName}/${sensor.sensorName}`}>
          <FormattedMessage
            id="dashboard.sensors.index.sensor_card.actions.assign"
            description="Action for sensor card"
            defaultMessage="Assign"
          />
        </Button>}
        <Button size="small" color="primary" component={ReachLink} to={`${sensor.deviceName}/${sensor.sensorName}`}>
          <FormattedMessage
            id="dashboard.sensors.index.sensor_card.actions.details"
            description="Action for sensor card"
            defaultMessage="Details"
          />
        </Button>
      </CardActions>
    </Card>;
  }

  const renderSensors = () => {
    return sensors.map((s: SensorDetailsApiModel, idx: number) => (
      <Grid key={idx} item xs={12} md={6} lg={4}>
        {renderSensor(s, idx)}
      </Grid>
    ));
  };

  const handleOffest = (e: React.MouseEvent<HTMLElement, MouseEvent>, offset: number, page: number) => {
    if (!location) { return; }
    let path = location.pathname;
    if (page !== 1) {
      path = location.pathname + `?page=${page}`;
    }
    window.location.href = path;
  };

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
            <ErrorBoundary>

              {loading ? <Grid item xs={12} md={6} lg={4}>
                <Card className={classes.sensorCard}>
                  <Skeleton variant="rect" height={200} />
                  <CardContent>
                    <Skeleton variant="text" />
                    <Skeleton variant="text" />
                  </CardContent>
                </Card>
              </Grid> : renderSensors()}
              {!loading && <Grid item xs={12}>
                <Pagination
                  limit={10}
                  offset={(page - 1) * 10}
                  total={total}
                  onClick={(e, offset, page) => handleOffest(e, offset, page)}
                />
              </Grid>}
            </ErrorBoundary>
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardSensors = withStyles(styles)(_DashboardSensors);

export default DashboardSensors;
