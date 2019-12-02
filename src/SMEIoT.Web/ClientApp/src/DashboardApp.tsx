import * as React from "react";
import { Router } from "@reach/router"
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
import DashboardMqtt from "./dashboard/DashboardMqtt";
import DashboardNewSensor from "./dashboard/DashboardNewSensor";
import DashboardNewDevice from "./dashboard/DashboardNewDevice";
import DashboardNewDeviceConnect from "./dashboard/DashboardNewDeviceConnect";
import DashboardNewDeviceConnectSensors from "./dashboard/DashboardNewDeviceConnectSensors";
import EnMessages from "./locales/en.json";

export interface IDashboardApp {
  language?: string
}

const DashboardApp: React.FunctionComponent<IDashboardApp> = ({
  language
}) => {
  if (language === undefined) {
    language = "en";
  }
  let messages: Record<string, string>;
  switch (language) {
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

  return <IntlProvider locale={language} messages={messages}>
    <ThemeProvider theme={theme}>
      <Router>
        <DashboardIndex path="/dashboard" />
        <DashboardDevices path="/dashboard/devices" />
        <DashboardNewDevice path="/dashboard/devices/new" />
        <DashboardNewDeviceConnect path="/dashboard/devices/new/connect" />
        <DashboardNewDeviceConnectSensors path="/dashboard/devices/new/connect_sensors" />
        <DashboardSensors path="/dashboard/sensors" />
        <DashboardNewSensor path="/dashboard/sensors/new" />
        <DashboardMqtt path="/dashboard/mqtt" />
        <DashboardUsers path="/dashboard/users" />
        <DashboardEditUser path="/dashboard/users/:username" />
      </Router>
    </ThemeProvider>
  </IntlProvider>;
};

export default DashboardApp;
