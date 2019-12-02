import Container from "@material-ui/core/Container";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import IconButton from '@material-ui/core/IconButton';
import Paper from '@material-ui/core/Paper';
import CloseIcon from "@material-ui/icons/Close";
import Frame from "./Frame";
import { Helmet } from "react-helmet";
import useInterval from "../helpers/useInterval";
import { BrokerApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { defineMessages, useIntl } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";

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
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
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
    WithStyles<typeof styles> {}

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

  const [running, setRunning] = React.useState<boolean>(false);
  const [statistics, setStatistics] = React.useState<{}>({});

  const api = new BrokerApi(GetDefaultApiConfig());
  const updateBroker = async () => {
    let details = await api.apiBrokerGet();
    if (details === null) { return; }
    setRunning(details.running);
    setStatistics(details.statistics);
  }

  useInterval(updateBroker, 10000);
  React.useEffect(() => { updateBroker() }, []);

  return (
    <Frame
      title={intl.formatMessage(messages.title)}
      direction="ltr"
      toolbarRight={
        <IconButton
          edge="end"
          color="inherit"
          aria-label="close this action"
          to={"/dashboard/devices"}
          component={ReachLink}
        >
          <CloseIcon/>
        </IconButton>
      }
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Helmet>
            <title>{intl.formatMessage(messages.title)}</title>
          </Helmet>
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
        </Container>
      }
    />
  );
};

const DashboardBrokerStatistics = withStyles(styles)(_DashboardBrokerStatistics);

export default DashboardBrokerStatistics;
