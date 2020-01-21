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
import DashboardFrame from "./DashboardFrame";
import clsx from 'clsx';
import { Link as ReachLink, RouteComponentProps } from '@reach/router';
import UserAvatarMenu from '../components/UserAvatarMenu';
import { useAppCookie } from '../helpers/useCookie';
import BasicBrokerCard from './BasicBrokerCard';
import { FormattedMessage, defineMessages, useIntl } from 'react-intl';
import useInterval from '../helpers/useInterval';
import { DashboardApi, SystemHighlightsApiModel } from 'smeiot-client';
import { GetDefaultApiConfig } from '..';
import Skeleton from '@material-ui/lab/Skeleton';

const styles = ({ palette, spacing }: Theme) => createStyles({
  container: {
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

const messages = defineMessages({
  title: {
    id: "dashboard.index.title",
    description: "Dashboard title",
    defaultMessage: "Dashboard"
  }
})

const _DashboardIndex: React.FunctionComponent<IDashboardIndexProps> = ({ classes, navigate }) => {
  const intl = useIntl();
  const fixedHeightPaper = clsx(classes.paper, classes.fixedHeight);

  const appCookie = useAppCookie();

  const [highlights, setHighlights] = React.useState<SystemHighlightsApiModel>();
  const [loading, setLoading] = React.useState<boolean>(false);
  useInterval(async () => {
    setLoading(true);
    const api = new DashboardApi(GetDefaultApiConfig());
    await api.apiDashboardGet().then((res) => {
      setHighlights(res);
      return res;
    }).finally(() => {
      setLoading(false);
    });
  }, 30000, true);

  return <DashboardFrame title={intl.formatMessage(messages.title)} direction="ltr"
    drawer
    toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate}/>}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          {/* Sensor stats */}
          <Grid item xs={12} md={8} lg={9}>
            <Paper className={fixedHeightPaper}>
              {loading ? <Skeleton variant="rect" width={120} height={30} /> : <Typography component="h2" variant="h6" color="primary" gutterBottom>
                <FormattedMessage
                  id="dashboard.index.sensors.title"
                  description="The title on dashboard sensor card"
                  defaultMessage="Sensors"
                /> 
                </Typography>
              }
              {loading ?
                <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> :
                <Typography component="p" variant="h4">
                  {!loading &&
                    <FormattedMessage
                      id="dashboard.index.sensors.first"
                      description="The fisrt text on dashboard sensor card"
                      defaultMessage="{connectedCount} connected of {count}"
                      values={{ count: highlights && highlights.sensorCount, connectedCount: highlights && highlights.connectedSensorCount}}
                    /> 
                  }
                </Typography>
              }
              <Typography color="textSecondary" className={classes.context}>
                {!loading &&
                  <FormattedMessage
                    id="dashboard.index.sensors.second"
                    description="The second text on dashboard sensor card"
                    defaultMessage="among {count} device ({connectedCount} connected)"
                    values={{ count: highlights && highlights.deviceCount, connectedCount: highlights && highlights.connectedDeviceCount}}
                  /> 
                }
              </Typography>
              {loading ? <Skeleton variant="rect" width={150} height={20} /> : <div>
                <Link color="primary" to="sensors" component={ReachLink}>
                  <FormattedMessage
                    id="dashboard.index.sensors.link"
                    description="The link on dashboard sensor card"
                    defaultMessage="View sensors"
                  /> 
                </Link>
              </div>}
            </Paper>
          </Grid>
          {/* User stats */}
          <Grid item xs={12} md={4} lg={3}>
            <Paper className={fixedHeightPaper}>
              {loading ? <Skeleton variant="rect" width={120} height={30} /> : <Typography component="h2" variant="h6" color="primary" gutterBottom>
                <FormattedMessage
                  id="dashboard.index.users.title"
                  description="The title on dashboard users card"
                  defaultMessage="Users"
                /> 
                </Typography>
              }
              {loading ?
                <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> :
                <Typography component="p" variant="h4">
                  {!loading && highlights && highlights.userCount}
                </Typography>
              }
              <Typography color="textSecondary" className={classes.context}>
                {!loading &&
                    <FormattedMessage
                      id="dashboard.index.users.second"
                      description="The second text on dashboard sensor card"
                      defaultMessage="{count} admin"
                      values={{ count: highlights && highlights.adminCount }}
                    /> 
                  }
              </Typography>
              {loading ? <Skeleton variant="rect" width={150} height={20} /> : <div>
                <Link color="primary" to="users" component={ReachLink}>
                  <FormattedMessage
                    id="dashboard.index.users.link"
                    description="The link on dashboard users card"
                    defaultMessage="View users"
                  /> 
                </Link>
              </div>}
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <BasicBrokerCard />
          </Grid>
        </Grid>
      </Container>
    } />;
};

const DashboardIndex = withStyles(styles)(_DashboardIndex);

export default DashboardIndex;
