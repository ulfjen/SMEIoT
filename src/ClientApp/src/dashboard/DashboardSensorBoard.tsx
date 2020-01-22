import Grid from "@material-ui/core/Grid";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import * as React from "react";
import Skeleton from "@material-ui/lab/Skeleton";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import SensorCard from "../components/SensorCard";
import { FormattedMessage } from "react-intl";
import {
  Link as ReachLink
} from "@reach/router";
import { SensorDetailsApiModel, SensorsApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import useInterval from "../helpers/useInterval";

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
  const handleMoreClicked = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };
  const [sensors, setSensors] = React.useState<Array<SensorDetailsApiModel>>([]);

  useInterval(async () => {
    const api = new SensorsApi(GetDefaultApiConfig());
    var res = await api.apiSensorsGet({
      offset: 0,
      limit: 10
    });
    if (res !== null && res.sensors) {
      setSensors(res.sensors);
    } else {
      setLoadingError(true);
    }
    setLoading(false);
  }, 3000);

  const renderSensors = () => {
    return sensors.map((s: SensorDetailsApiModel) => (
      <Grid item xs={4}>
        <SensorCard sensor={s} key={s.sensorName} onMoreClick={handleMoreClicked} />
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
