import * as React from "react";
import { Router, RouteComponentProps, Redirect } from "@reach/router";
import DashboardUsers from "./dashboard/DashboardUsers";
import DashboardSensors from "./dashboard/DashboardSensors";
import DashboardDevices from "./dashboard/DashboardDevices";
import DashboardEditUser from "./dashboard/DashboardEditUser";
import DashboardIndex from "./dashboard/DashboardIndex";
import DashboardMqttLogs from "./dashboard/DashboardMqttLogs";
import DashboardNewSensor from "./dashboard/DashboardNewSensor";
import DashboardNewDevice from "./dashboard/DashboardNewDevice";
import DashboardDevice from "./dashboard/DashboardDevice";
import DashboardSensorDetails from "./dashboard/DashboardSensorDetails";
import DashboardNewDeviceConnect from "./dashboard/DashboardNewDeviceConnect";
import DashboardNewDeviceConnectSensors from "./dashboard/DashboardNewDeviceConnectSensors";
import DashboardDeviceCredential from "./dashboard/DashboardDeviceCredential";
import DashboardBrokerStatistics from "./dashboard/DashboardBrokerStatistics";
import DashboardSensorAssignment from "./dashboard/DashboardSensorAssignment";
import DashboardSettings from "./dashboard/DashboardSettings";
import { useAppCookie } from "./helpers/useCookie";

export interface IDashboardApp extends RouteComponentProps {
}

const DashboardApp: React.FunctionComponent<IDashboardApp> = ({ location }) => {
  const cookie = useAppCookie();

  if (!cookie.userName) {
    if (location && location.pathname !== "/") {
      return <Redirect noThrow to="/" />;
    }
  } else {
    if (!cookie.admin && location && location.pathname && !location.pathname.startsWith("/dashboard/sensors")) {
      return <Redirect noThrow to="/dashboard/sensors" />;
    }
  }
  return <Router>
        <DashboardIndex path="/" />
        <DashboardDevices path="/devices" />
        <DashboardBrokerStatistics path="broker/statistics" />
        <DashboardNewDevice path="devices/new" />
        <DashboardNewDeviceConnect path="devices/wait_connection" />
        <DashboardNewDeviceConnectSensors path="devices/configure_sensors" />
        <DashboardDevice path="devices/:deviceName" />
        <DashboardDeviceCredential path="devices/:deviceName/credentials" />
        <DashboardSensorAssignment path="devices/:deviceName/:sensorName" />
        <DashboardSensors path="sensors" />
        <DashboardNewSensor path="sensors/new" />
        <DashboardSensorDetails path="sensors/:deviceName/:sensorName" />
        <DashboardMqttLogs path="broker/logs" />
        <DashboardUsers path="users" />
        <DashboardEditUser path="users/:userName" />
        <DashboardSettings path="settings" />
      </Router>;
};

export default DashboardApp;
