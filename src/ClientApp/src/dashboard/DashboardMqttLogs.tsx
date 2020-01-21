import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import { RouteComponentProps } from '@reach/router';
import * as SignalR from "@microsoft/signalr";
import { FixedSizeList, ListChildComponentProps } from 'react-window';
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import DashboardBrokerFrame from "./DashboardBrokerFrame";
import { Typography } from "@material-ui/core";

const styles = ({ }: Theme) => createStyles({
  container: {
  },
  fixedHeight: {
    height: 240,
  },
  paper: {
    padding: 4
  }
});

const DashboardMqttLogRenderer: React.FunctionComponent<ListChildComponentProps> = ({ data, index, style }) => {
  return <Typography style={style}>{data[index]}</Typography>;
};

export interface IDashboardMqttLogsProps extends RouteComponentProps, WithStyles<typeof styles> {

}

const messages = defineMessages({
  title: {
    id: "dashboard.mqtt_logs.index.title",
    description: "Used as title in the mqtt logs page on the dashboard",
    defaultMessage: "MQTT Logs"
  }
});

const ITEM_SIZE = 25;
const FRAME_PADDING = 24;

const _DashboardMqttLogs: React.FunctionComponent<IDashboardMqttLogsProps> = ({ location, classes }) => {
  if (location === undefined) { throw new Error("No location is found."); }
  const intl = useIntl();

  const [hasData, setHasData] = React.useState<boolean>(false);
  const [logs, setLogs] = React.useState<string[]>([]);
  const [length, setLength] = React.useState<number>(0);
  const filter = new URLSearchParams(location.search).get("filter");

  React.useEffect(() => {
    const createHubConnection = async () => {
      const hubConnect = new SignalR.HubConnectionBuilder()
        .withUrl("/dashboard/mqtt_hub")
        .build();

      hubConnect.on("ReceiveMessage", (req: { receivedAt: string, topic: string, payload: string }) => {
        const newLogs = logs;
        if (filter && filter.length > 0) {
          if (!req.topic.startsWith(filter)) { return; }
        }
        console.log(hasData);
        setHasData(true);
        const log = `[${req.receivedAt}] ${req.topic}: ${req.payload}`;
        newLogs.push(log);
        setLogs(newLogs);
        setLength(newLogs.length);
      });

      try {
        await hubConnect.start().catch((err: string) => document.write(err));
      }
      catch (err) {
        alert(err);
      }
    }

    createHubConnection();
  }, [logs]);
  
  const initialHeight = length * ITEM_SIZE;
  const containerRef = React.createRef<HTMLElement>();
  const measureRef = React.createRef<HTMLDivElement>();
  const [width, setWidth] = React.useState(-1);
  const [height, setHeight] = React.useState(initialHeight);
  const measureAvailbleViewport = React.useCallback(() => {
    if (measureRef.current && containerRef.current) {
      const docHeight = containerRef.current.getBoundingClientRect().height;
      const measureRect = measureRef.current.getBoundingClientRect();
      setHeight(Math.min(length * ITEM_SIZE, docHeight - measureRect.top - FRAME_PADDING));
      setWidth(measureRect.width);
    }
  }, [measureRef, containerRef, setHeight, setWidth, length]);
  React.useEffect(() => measureAvailbleViewport(), [measureAvailbleViewport]);

  return <DashboardBrokerFrame
    ref={containerRef}
    title={intl.formatMessage(messages.title)}>
    <Grid item xs={12}>
      <Paper className={classes.paper} ref={measureRef}>
        {hasData ? 
        <FixedSizeList
          height={height}
          itemCount={length}
          itemData={logs}
          itemSize={ITEM_SIZE}
          width={width}
        >
          {DashboardMqttLogRenderer}
        </FixedSizeList> : 
        <Typography>
          <FormattedMessage
            id="dashboard.mqtt_logs.index.nothing"
            description="Nothing"
            defaultMessage="No logs is available."
          />
        </Typography>
        }
      </Paper>
    </Grid>
  </DashboardBrokerFrame>
};

const DashboardMqttLogs = withStyles(styles)(_DashboardMqttLogs);

export default DashboardMqttLogs;
