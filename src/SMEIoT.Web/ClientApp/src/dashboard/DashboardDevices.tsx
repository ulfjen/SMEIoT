import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Tooltip from '@material-ui/core/Tooltip';
import Fab from '@material-ui/core/Fab';
import * as React from "react";
import AddIcon from '@material-ui/icons/Add';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import clsx from 'clsx';
import UserAvatarMenu from '../components/UserAvatarMenu';
import { BasicUserApiModel, SensorDetailsApiModel } from 'smeiot-client/src';
import moment from 'moment';
import SensorCard from '../components/SensorCard';
import { Link, RouteComponentProps } from '@reach/router';

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  paper: {
    padding: spacing(2),
    display: 'flex',
    overflow: 'auto',
    flexDirection: 'column',
  },
  fixedHeight: {
    height: 240,
  },
  absolute: {
    position: 'absolute',
    bottom: spacing(2),
    right: spacing(3),
  },
});

export interface IDashboardDevices extends RouteComponentProps, WithStyles<typeof styles> {
}


const _DashboardDevices: React.FunctionComponent<IDashboardDevices> = ({ classes }) => {
  const [sensors, setSensors] = React.useState<null | Array<SensorDetailsApiModel>>(null);

  const renderSensors = () => {
    if (sensors == null) { return null; }

    return sensors.map(sensor => <SensorCard sensor={sensor} key={sensor.sensorName || ""} />);
  };


  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
  return <Frame title="Sensors" direction="ltr" toolbarRight={null}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          {renderSensors()}
        </Grid>
        <Tooltip title="Add" aria-label="add">
          <Fab
            color="secondary"
            className={classes.absolute}
            to={"/dashboard/sensors/new"}
            component={Link}
          >
            <AddIcon />
          </Fab>
        </Tooltip>
      </Container>} />;
};

const DashboardDevices = withStyles(styles)(_DashboardDevices);

export default DashboardDevices;
