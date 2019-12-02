import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import Container from "@material-ui/core/Container";
import Paper from "@material-ui/core/Paper";
import ListItemLine from "../components/ListItemLine";
import Grid from "@material-ui/core/Grid";
import { Helmet } from "react-helmet";
import { Link, RouteComponentProps } from '@reach/router';
import * as SignalR from "@microsoft/signalr";
import { FixedSizeList, areEqual, ListChildComponentProps } from 'react-window';
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import StatusBadge from "../components/StatusBadge";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  fixedHeight: {
    height: 240,
  },
  paper: {
    padding: spacing(4)
  }
});

const DashboardMqttLogRenderer: React.FunctionComponent<ListChildComponentProps> = ({ data, index, style }) => {
  return <p style={style}>{data[index]}</p>;
};

export interface IDashboardMqttLogsProps extends RouteComponentProps, WithStyles<typeof styles> {

}

const messages = defineMessages({
  title: {
    id: "dashboard.mqtt_logs.index.title",
    description: "Used as title in the mqtt logs page on the dashboard",
    defaultMessage: "MQTT Logs"
  },
});

const _DashboardMqttLogs: React.FunctionComponent<IDashboardMqttLogsProps> = ({ classes }) => {
  const intl = useIntl();

  const [hubConnection, setHubConnection] = React.useState<SignalR.HubConnection>();
  const [logs, setLogs] = React.useState<string[]>([]);
  const [length, setLength] = React.useState<number>(0);

  React.useEffect(() => {
    const createHubConnection = async () => {
      const hubConnect = new SignalR.HubConnectionBuilder()
        .withUrl("/dashboard/mqtt_hub")
        .build();

      hubConnect.on("ReceiveMessage", (req: { message: string }) => {
        const newLogs = logs;
        newLogs.push(req.message)
        setLogs(newLogs);
        setLength(newLogs.length);
      });

      try {
        await hubConnect.start().catch((err: string) => document.write(err));
      }
      catch (err) {
        alert(err);
      }
      setHubConnection(hubConnect);
    }

    createHubConnection();
  }, []);

  return <Frame title="Dashboard" direction="ltr"
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Helmet>
          <title>{intl.formatMessage(messages.title)}</title>
        </Helmet>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <h4>Broker</h4>
              <StatusBadge status="running"></StatusBadge>
            </Paper>
            </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>

            <FixedSizeList
              height={400}
              itemCount={length}
              itemData={logs}
              itemSize={35}
              width={"100%"}
            >
              {DashboardMqttLogRenderer}
            </FixedSizeList>
            </Paper>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardMqttLogs = withStyles(styles)(_DashboardMqttLogs);

export default DashboardMqttLogs;
