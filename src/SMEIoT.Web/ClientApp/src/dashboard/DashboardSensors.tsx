import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import * as React from "react";
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
import { RouteComponentProps } from '@reach/router';

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
});

export interface IDashboardSensors extends RouteComponentProps, WithStyles<typeof styles> {
}


const _DashboardSensors: React.FunctionComponent<IDashboardSensors> = ({ classes }) => {
  const [sensors, setSensors] = React.useState<null | Array<SensorDetailsApiModel>>(null);

  let user: BasicUserApiModel = {
    createdAt: moment.utc().toISOString(),
    roles: [],
    username: ""
  };

  // @ts-ignore
  if (window.SMEIoTPreRendered) {
    // @ts-ignore
    user = window.SMEIoTPreRendered["currentUser"];
  }

  const renderSensors = () => {
    if (sensors == null) { return null; }

    return sensors.map(sensor => <SensorCard sensor={sensor} key={sensor.sensorName || ""} />);
  };

  const toolbarRight = <UserAvatarMenu user={user} />;

  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
  return <Frame title="Sensors" direction="ltr" toolbarRight={toolbarRight}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          {renderSensors()}
        </Grid>
      </Container>} />;
};

const DashboardSensors = withStyles(styles)(_DashboardSensors);

export default DashboardSensors;
