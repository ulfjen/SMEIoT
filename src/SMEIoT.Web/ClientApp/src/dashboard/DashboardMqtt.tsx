import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import moment from "moment";
import { Link, RouteComponentProps } from '@reach/router';
import * as SignalR from "@microsoft/signalr";
import { FixedSizeList, areEqual } from 'react-window';
import DashboardMqttLineRenderer from './DashboardMqttLineRenderer';
import ListItemLine from "../components/ListItemLine";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  fixedHeight: {
    height: 240,
  },
});

export interface IDashboardMqttProps extends RouteComponentProps, WithStyles<typeof styles> {

}

const _DashboardMqtt: React.FunctionComponent<IDashboardMqttProps> = ({ classes }) => {
  const [hubConnection, setHubConnection] = React.useState<SignalR.HubConnection>();
  const [logs, setLogs] = React.useState<string[]>([]);
  const [length, setLength] = React.useState<number>(0);

  React.useEffect(() => {
    const createHubConnection = async () => {
      const hubConnect = new SignalR.HubConnectionBuilder()
        .withUrl("/dashboard/mqtt_hub")
        .build();

      hubConnect.on("ReceiveMessage", (req: {message: string}) => {
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

  return <Frame title="Dashboard" direction="ltr" toolbarRight={null}
    content={
      <div>
        <FixedSizeList 
          height={200}
          itemCount={length}
          itemData={logs}
          itemSize={35}
          width={800}
        >
          {DashboardMqttLineRenderer}
        </FixedSizeList>
      </div>} />;
};

const DashboardMqtt = withStyles(styles)(_DashboardMqtt);

export default DashboardMqtt;
