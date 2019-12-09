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
import DashboardNewDeviceConnect from "./dashboard/DashboardNewDeviceConnect";
import DashboardNewDeviceConnectSensors from "./dashboard/DashboardNewDeviceConnectSensors";
import DashboardBrokerStatistics from "./dashboard/DashboardBrokerStatistics";
import EnMessages from "./locales/en.json";
import DashboardApp from "./DashboardApp";
import NewSession from "./NewSession";
import NewUser from "./NewUser";

function GetXsrfTokenFromDom()
{
  var token = "";
  try {
    const ele = document.getElementById("RequestVerificationToken");
    const raw = ele !== null ? ele.getAttribute("value") : "";
    token = raw !== null ? raw : "";
  } catch {
    token = "";
  }
  return token;
}


export interface ISMEIoTApp {
}

const SMEIoTApp: React.FunctionComponent<ISMEIoTApp> = ({
}) => {
  let locale = "en";
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

  
  return <IntlProvider locale={locale} messages={messages}>
  <ThemeProvider theme={theme}>
    <Router>
    <DashboardApp path="/dashboard/*" locale={locale}/>
    <NewSession path="/login" csrfToken={GetXsrfTokenFromDom()}/>
    <NewUser path="/signup" csrfToken={GetXsrfTokenFromDom()}/>
    </Router>
  </ThemeProvider>
</IntlProvider>;
};

export default SMEIoTApp;
