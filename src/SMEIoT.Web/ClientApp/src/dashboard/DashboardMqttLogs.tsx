import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import Container from "@material-ui/core/Container";
import ListItemLine from "../components/ListItemLine";
import Grid from "@material-ui/core/Grid";
import { Helmet } from "react-helmet";
import { Link, RouteComponentProps } from '@reach/router';
import * as SignalR from "@microsoft/signalr";
import { FixedSizeList, areEqual, ListChildComponentProps } from 'react-window';
import { defineMessages, useIntl, FormattedMessage } from "react-intl";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  fixedHeight: {
    height: 240,
  },
});

const DashboardMqttLogRenderer: React.FunctionComponent<ListChildComponentProps> = ({ data, index, style }) => {
  return <p style={style}>{data[index]}</p>;
};

export interface IDashboardMqttLogsProps extends RouteComponentProps, WithStyles<typeof styles> {

}

const _DashboardMqttLogs: React.FunctionComponent<IDashboardMqttLogsProps> = ({ classes }) => {
  const intl = useIntl();

  const [hubConnection, setHubConnection] = React.useState<SignalR.HubConnection>();
  const [logs, setLogs] = React.useState<string[]>([]);
  const [length, setLength] = React.useState<number>(0);

  const messages = defineMessages({
    title: {
      id: "dashboard.mqtt_logs.index.title",
      description: "Used as title in the mqtt logs page on the dashboard",
      defaultMessage: "MQTT Logs"
    },
  });

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
            <FixedSizeList
              height={400}
              itemCount={length}
              itemData={logs}
              itemSize={35}
              width={"100%"}
            >
              {DashboardMqttLogRenderer}
            </FixedSizeList>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardMqttLogs = withStyles(styles)(_DashboardMqttLogs);

export default DashboardMqttLogs;
