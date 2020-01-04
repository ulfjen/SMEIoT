import "./styles/site.scss";
import * as React from "react";
import { Router, RouteComponentProps } from "@reach/router";
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import useMediaQuery from '@material-ui/core/useMediaQuery';
import createMuiTheme from "@material-ui/core/styles/createMuiTheme";
import palette from "./theme";
import DashboardUsers from "./dashboard/DashboardUsers";
import DashboardSensors from "./dashboard/DashboardSensors";
import DashboardDevices from "./dashboard/DashboardDevices";
import DashboardEditUser from "./dashboard/DashboardEditUser";
import DashboardIndex from "./dashboard/DashboardIndex";
import DashboardMqttLogs from "./dashboard/DashboardMqttLogs";
import DashboardNewSensor from "./dashboard/DashboardNewSensor";
import DashboardNewDevice from "./dashboard/DashboardNewDevice";
import DashboardDeviceEdit from "./dashboard/DashboardDeviceEdit";
import DashboardSensorDetails from "./dashboard/DashboardSensorDetails";
import DashboardNewDeviceConnect from "./dashboard/DashboardNewDeviceConnect";
import DashboardNewDeviceConnectSensors from "./dashboard/DashboardNewDeviceConnectSensors";
import DashboardBrokerStatistics from "./dashboard/DashboardBrokerStatistics";
import EnMessages from "./locales/en.json";

export interface IDashboardApp extends RouteComponentProps {
  locale?: string
}

const DashboardApp: React.FunctionComponent<IDashboardApp> = ({
  locale
}) => {
  if (locale === undefined) {
    locale = "en";
  }
  let messages: Record<string, string>;
  switch (locale) {
    case "en":
    default:
      messages = EnMessages;
  }

  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');

  const theme = React.useMemo(
    () =>
      createMuiTheme({
        palette: Object.assign({
          type: prefersDarkMode ? 'dark' : 'light',
        }, palette)
      }),
    [prefersDarkMode],
  );

  return <Router>
        <DashboardIndex path="/" />
        <DashboardDevices path="/devices" />
        <DashboardBrokerStatistics path="broker/statistics" />
        <DashboardNewDevice path="devices/new" />
        <DashboardNewDeviceConnect path="devices/new/connect" />
        <DashboardNewDeviceConnectSensors path="devices/new/connect/sensors" />
        <DashboardDeviceEdit path="devices/:deviceName/edit" />
        <DashboardSensors path="sensors" />
        <DashboardNewSensor path="sensors/new" />
        <DashboardSensorDetails path="sensors/:deviceName/:sensorName"/>
        <DashboardMqttLogs path="broker/logs" />
        <DashboardUsers path="users" />
        <DashboardEditUser path="users/:userName" />
      </Router>;
};

export default DashboardApp;
