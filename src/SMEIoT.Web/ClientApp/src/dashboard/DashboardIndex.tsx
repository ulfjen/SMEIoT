import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import Link from '@material-ui/core/Link';
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import clsx from 'clsx';
import { Link as ReachLink, LinkProps as ReachLinkProps, RouteComponentProps } from '@reach/router';

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
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
  context: {
    flex: 1,
  },
});

export interface IDashboardIndexProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const _DashboardIndex: React.FunctionComponent<IDashboardIndexProps> = ({ classes }) => {
  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);

    return <Frame title="Dashboard" direction="ltr" toolbarRight={null}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          {/* Sensor stats */}
          <Grid item xs={12} md={8} lg={9}>
            <Paper className={fixedHeightPaper}>
              <Typography component="h2" variant="h6" color="primary" gutterBottom>
                Sensors
            </Typography>
              <Typography component="p" variant="h4">
                1
            </Typography>
              <Typography color="textSecondary" className={classes.context}>
              </Typography>
              <div>
                <Link color="primary" to="/dashboard/sensors" component={ReachLink}>
                  View sensors
                </Link>
              </div>
            </Paper>
          </Grid>
          {/* User stats */}
          <Grid item xs={12} md={4} lg={3}>
            <Paper className={fixedHeightPaper}>
              <Typography component="h2" variant="h6" color="primary" gutterBottom>
                Users
              </Typography>
              <Typography component="p" variant="h4">
                1
              </Typography>
              <Typography color="textSecondary" className={classes.context}>
              </Typography>
              <div>
                <Link color="primary" to="/dashboard/sensors" component={ReachLink}>
                  View users
                </Link>
              </div>
            </Paper>
          </Grid>
          {/* System stats */}
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
