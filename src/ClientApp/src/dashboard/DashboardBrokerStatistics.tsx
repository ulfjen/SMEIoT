import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Grid from "@material-ui/core/Grid";
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import IconButton from '@material-ui/core/IconButton';
import Paper from '@material-ui/core/Paper';
import CloseIcon from "@material-ui/icons/Close";
import Frame from "./DashboardFrame";
import { useTitle } from 'react-use';
import BasicBrokerCard from "./BasicBrokerCard";
import useInterval from "../helpers/useInterval";
import { BrokerApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { defineMessages, useIntl } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";
import DashboardBrokerFrame from "./DashboardBrokerFrame";

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
      width: '100%',
      overflowX: 'auto',
    },
    table: {
      minWidth: 650,
    },
  });

export interface IDashboardBrokerStatistics
  extends RouteComponentProps,
  WithStyles<typeof styles> { }

const messages = defineMessages({
  title: {
    id: "dashboard.broker.statistics.title",
    description: "Used as title in the broker statistics page on the dashboard",
    defaultMessage: "Broker Statistics"
  },
});

const _DashboardBrokerStatistics: React.FunctionComponent<IDashboardBrokerStatistics> = ({
  classes
}) => {
  const intl = useIntl();

  const [statistics, setStatistics] = React.useState<{[key: string]: any}>({});

  const api = new BrokerApi(GetDefaultApiConfig());
  const updateBroker = async () => {
    let details = await api.apiBrokerStatisticsGet();
    if (details === null) { return; }
    setStatistics(details.statistics || {});
  }

  useInterval(updateBroker, 10000);
  React.useEffect(() => { updateBroker() }, []);

  return (
    <DashboardBrokerFrame
      title={intl.formatMessage(messages.title)}>
      <Grid item xs={12}>
        <Paper className={classes.paper}>
          <Table className={classes.table} size="small" aria-label="a dense table">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell align="right">Value</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {Object.keys(statistics).sort().map(k => <TableRow key={k}>
                <TableCell component="th" scope="row">
                  {k}
                </TableCell>
                <TableCell align="right">{statistics[k]}</TableCell>
              </TableRow>
              )}
            </TableBody>
          </Table>
        </Paper>
      </Grid>
    </DashboardBrokerFrame>
  );
};

const DashboardBrokerStatistics = withStyles(styles)(_DashboardBrokerStatistics);

export default DashboardBrokerStatistics;
