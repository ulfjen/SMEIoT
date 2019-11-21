import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Tooltip from '@material-ui/core/Tooltip';
import Fab from '@material-ui/core/Fab';
import * as React from "react";
import AddIcon from '@material-ui/icons/Add';
import Link from '@material-ui/core/Link';
import ExpansionPanel from '@material-ui/core/ExpansionPanel';
import Skeleton from "@material-ui/lab/Skeleton";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Typography from "@material-ui/core/Typography";
import Frame from "./Frame";
import clsx from 'clsx';
import {Helmet} from "react-helmet";
import UserAvatarMenu from '../components/UserAvatarMenu';
import DeviceListItem from '../components/DeviceListItem';
import { BasicUserApiModel, DeviceApiModel, DeviceApiModelFromJSON } from 'smeiot-client';
import moment from 'moment';
import SensorCard from '../components/SensorCard';
import {defineMessages, useIntl, FormattedMessage} from 'react-intl';
import { Link as ReachLink, LinkProps as ReachLinkProps, RouteComponentProps } from '@reach/router';

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
  list: {

  }
});

export interface IDashboardDevices extends RouteComponentProps, WithStyles<typeof styles> {
}

const messages = defineMessages({
  title: {
    id: 'dashboard.devices.index.title',
    description: 'Used as title in the devices index page on the dashboard',
    defaultMessage: 'Devices',
  },
  fabTooltip: {
    id: 'dashboard.devices.index.action.tooltip',
    description: 'The tooltip title and aria label for the action button',
    defaultMessage: 'Add',
  }
});

const _DashboardDevices: React.FunctionComponent<IDashboardDevices> = ({ classes }) => {
  const intl = useIntl();
  const loaded = true;
  const [devices, setDevices] = React.useState<Array<DeviceApiModel>>([
    DeviceApiModelFromJSON({
      name: "a1",
      createdAt: "2019-01-01",
      updatedAt: "2020-01-01",
      authenticationType: 0,
      preSharedKey: "aaaaaaaaaaaaaaaaaa111",
      connected: true,
      connectedAt: "2020-01-01",
      lastMessageAt: "2020-01-01"
    }), DeviceApiModelFromJSON({
      name: "a2",
      createdAt: "2019-01-01",
      updatedAt: "2020-01-01",
      authenticationType: 0,
      preSharedKey: "aaaaaaaaaaaaaaaaaa112",
      connected: true,
      connectedAt: "2020-01-01",
      lastMessageAt: "2020-01-01"
    })
  ]);

  const renderDevices = () => {
    return devices.map((d: DeviceApiModel) => <DeviceListItem device={d} expanded={false} key={d.name}/>);
  };

  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);
  return <Frame title={intl.formatMessage(messages.title)} direction="ltr" toolbarRight={null}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Helmet>
          <title>{intl.formatMessage(messages.title)}</title>
        </Helmet>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <Typography component="h2" variant="h6" color="primary" gutterBottom>
                <FormattedMessage
                  id="dashboard.devices.index.broker.title"
                  description="The broker block title on the dashboard page."
                  defaultMessage="Broker"/>
              </Typography>
              <span>Running</span>
              <p>Connected: </p>
              <p>Received bytes</p>
              <p>Received placeholder</p>
              <div>
                <Link color="primary" to="/dashboard/broker/statistics" component={ReachLink}>
                  <FormattedMessage
                    id="dashboard.devices.index.broker.actions.statistics"
                    description="The action for viewing statistics on the broker block."
                    defaultMessage="Statistics"/>
                </Link>
                <Link color="primary" to="/dashboard/broker/logs" component={ReachLink}>
                  <FormattedMessage
                    id="dashboard.devices.index.broker.actions.logs"
                    description="The action for viewing logs on the broker block."
                    defaultMessage="Log"/>
                </Link>
                <Link color="primary" to="/dashboard/broker/config" component={ReachLink}>
                  <FormattedMessage
                    id="dashboard.devices.index.broker.actions.config"
                    description="The action for editing config file on the broker block."
                    defaultMessage="Config"/>
                </Link>
                <span>
                  <FormattedMessage
                    id="dashboard.devices.index.broker.actions.reload"
                    description="The action for reloading config file on the broker block."
                    defaultMessage="Reload"/>
                </span>
                <span>
                  <FormattedMessage
                    id="dashboard.devices.index.broker.actions.restart"
                    description="The action for restarting config file on the broker block."
                    defaultMessage="Restart"/>
                </span>
              </div>
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper>
              <div>
                {loaded ? (
                  renderDevices()
                ) : (
                  <Skeleton variant="rect" height={4}/>
                )}
              </div>
            </Paper>
          </Grid>
        </Grid>
        <Tooltip title={intl.formatMessage(messages.fabTooltip)} aria-label={intl.formatMessage(messages.fabTooltip)}>
          <Fab
            color="secondary"
            className={classes.absolute}
            to={"/dashboard/devices/new"}
            component={ReachLink}
          >
            <AddIcon />
          </Fab>
        </Tooltip>
      </Container>} />;
};

const DashboardDevices = withStyles(styles)(_DashboardDevices);

export default DashboardDevices;
