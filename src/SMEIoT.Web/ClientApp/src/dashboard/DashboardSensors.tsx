import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import * as React from "react";
import {createStyles, Theme, WithStyles} from "@material-ui/core";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import clsx from 'clsx';

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

export interface IDashboardSensors extends WithStyles<typeof styles> {
}

const DashboardSensors = withStyles(styles)(({classes}: IDashboardSensors) => {
    const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
    return <Frame title="Sensors" direction="ltr" toolbarRight={null}
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
              <p>placeholder 123</p>
            </Paper>
          </Grid>
          {/* Recent Orders */}
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <p>placeholder</p>
            </Paper>
          </Grid>
        </Grid>
      </Container>}/>;
  }
);

export default DashboardSensors;
