import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import {Configuration} from "smeiot-client";
import * as React from "react";
import * as ReactDOM from "react-dom";
import { Router } from "@reach/router"
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import theme from "./theme";
import NewUser from "./NewUser";
import EditUser from "./EditUser";
import SensorDetails from "./SensorDetails";
import NewSession from "./NewSession";
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

export function GetDefaultApiConfig() {
  return new Configuration({basePath: "https://localhost:5001"});
}
let language = "en";
let messages: Record<string, string>;
switch (language) {
  case "en":
  default:
    messages = EnMessages;
}

ReactDOM.render(
  <IntlProvider locale={language} messages={messages}>
    <ThemeProvider theme={theme}>
      <Router>
        <DashboardIndex path="/dashboard"/>
        <DashboardDevices path="/dashboard/devices" />
        <DashboardNewDevice path="/dashboard/devices/new" />
        <DashboardNewDeviceConnect path="/dashboard/devices/new/connect" />
        <DashboardNewDeviceConnectSensors path="/dashboard/devices/new/connect_sensors" />
        <DashboardSensors path="/dashboard/sensors" />
        <DashboardNewSensor path="/dashboard/sensors/new" />
        <DashboardMqtt path="/dashboard/mqtt" />
        <DashboardUsers path="/dashboard/users"/>
        <DashboardEditUser path="/dashboard/users/:username"/>
      </Router>
    </ThemeProvider>
  </IntlProvider>,
  document.getElementById("react-main"));
