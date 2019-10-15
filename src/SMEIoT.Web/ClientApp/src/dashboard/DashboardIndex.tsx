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
import UserAvatarMenu from "../components/UserAvatarMenu";
import { BasicUserApiModel } from 'smeiot-client/src';
import moment from 'moment';


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

export interface IDashboardIndexProps extends WithStyles<typeof styles> {
}


const _DashboardIndex: React.FunctionComponent<IDashboardIndexProps & WithStyles<typeof styles>> = ({ classes }) => {
  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
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

  const toolbarRight = <UserAvatarMenu user={user}/>;
  return <Frame title="Dashboard" direction="ltr" toolbarRight={toolbarRight} 
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          {/* Chart */}
          <Grid item xs={12} md={8} lg={9}>
            <Paper className={fixedHeightPaper}>
              <p>placeholder</p>
            </Paper>
          </Grid>
          {/* Recent Deposits */}
          <Grid item xs={12} md={4} lg={3}>
            <Paper className={fixedHeightPaper}>
              <p>placeholder</p>
            </Paper>
          </Grid>
          {/* Recent Orders */}
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <p>placeholder</p>
            </Paper>
          </Grid>
        </Grid>
      </Container>
    } />;
};

const DashboardIndex = withStyles(styles)(_DashboardIndex);

export default DashboardIndex;
