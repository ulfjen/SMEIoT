import Grid from "@material-ui/core/Grid";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import * as React from "react";
import Skeleton from "@material-ui/lab/Skeleton";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { FormattedMessage } from "react-intl";
import {
  Link as ReachLink
} from "@reach/router";
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
    },
    sensorCard: {
  
    }
  });

export interface IDashboardSensorBoard
  extends WithStyles<typeof styles> {
}

const _DashboardSensorBoard: React.FunctionComponent<IDashboardSensorBoard> = ({
  classes,
}) => {

  const [loading, setLoading] = React.useState<boolean>(true);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [loadingError, setLoadingError] = React.useState<boolean>(false);
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleClose = () => {
    setAnchorEl(null);
  };
  const [sensors, setSensors] = React.useState<Array<SensorDetailsApiModel>>([]);

  useInterval(async () => {
    const api = new SensorsApi(GetDefaultApiConfig());
    setLoading(true);
    var res = await api.apiSensorsGet({
      offset: 0,
      limit: 1000000000
    });
    if (res !== null && res.sensors) {
      setSensors(res.sensors);
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

  const renderSensor = (sensor: SensorDetailsApiModel, idx: number) => {
    const data = sensor.data.map(v => {
      return {x: new Date(Date.parse(v.createdAt)).getTime(), y: v.value}
    });
    return <Card ref={idx === 0 ? measureRef : undefined} key={idx} className={classes.sensorCard}>
    <CardActionArea>
      <CardMedia
        component="div"
        title="placeholder"
      >
        <NumberGraph width={width} height={200} data={data}/>
      </CardMedia>
      <CardContent>
        <TwoLayerLabelAction first={sensor.deviceName} second={sensor.sensorName} />
        <Typography variant="body2" color="textSecondary" component="p">
          secondary placeholder
        </Typography>
      </CardContent>
    </CardActionArea>
    <CardActions>
      <Button size="small" color="default" component={ReachLink} to={`../devices/${sensor.deviceName}/${sensor.sensorName}`}>
        <FormattedMessage
          id="dashboard.sensors.index.sensor_card.actions.assign"
          description="Action for sensor card"
          defaultMessage="Assign"
        /> 
      </Button>
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
      <Grid item xs={12} md={6} lg={4}>
        {renderSensor(s, idx)}
      </Grid>
    ));
  };

  return (
    <React.Fragment>
      {loading ? <Skeleton variant="rect" height={4}/> : renderSensors()}
      <Menu
        anchorEl={anchorEl}
        keepMounted
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
        <MenuItem
          button
          to="/dashboard/sensor/L401/edit"
          component={ReachLink}
          onClick={handleClose}
        >
          <FormattedMessage
            id="dashboard.sensor.actions.configure"
            description="The action for sensor card."
            defaultMessage="Configure"
          />
        </MenuItem>
        <MenuItem button onClick={handleClose}>
          <FormattedMessage
            id="dashboard.broker.actions.authenticate"
            description="The action for sensor card."
            defaultMessage="Manage authentication"
          />
        </MenuItem>
        <MenuItem button onClick={handleClose}>
          <FormattedMessage
            id="dashboard.broker.actions.delete"
            description="The action for sensor card."
            defaultMessage="Delete"
          />
        </MenuItem>
      </Menu>
    </React.Fragment>
  );
};

const DashboardSensorBoard = withStyles(styles)(_DashboardSensorBoard);

export default DashboardSensorBoard;
