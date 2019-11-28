import * as React from "react";
import * as ReactDOM from "react-dom";
import { Router } from "@reach/router";
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

interface IAction {
  [action: string]: () => JSX.Element;
}

interface IController {
  [controller: string]: IAction;
}

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

export const Routes: IController = {
  Home: {
    // Privacy() {
    // Privacy action code
    // }
  },
  
  Sessions: {
    New() {
      return <NewSession csrfToken={GetXsrfTokenFromDom()}/>;
    },
    
    Create() {
      return <NewSession csrfToken={GetXsrfTokenFromDom()}/>;
    }
  },

  Users: {
    New() {
      return <NewUser csrfToken={GetXsrfTokenFromDom()}/>;
    },

    Edit() {
      return <EditUser csrfToken={GetXsrfTokenFromDom()} />;
    },

  },

  Sensors: {
    Show() {
      return <SensorDetails/>;
    }
  }
};

// eslint-disable-next-line @typescript-eslint/no-namespace
// export namespace SMEIoT {

  export class BodyRouter {
    public static bind(controller: string, action: string) {
      action = action === undefined ? "init" : action;

      if (controller !== undefined && controller !== "" && Routes[controller] && Routes[controller][action] !== undefined) {
        return Routes[controller][action]();
      }
    }

    public static init() {
      let body = document.body;
      let controller = body.getAttribute("data-controller");
      let action = body.getAttribute("data-action");
      let language = "en";

      if (controller !== null && controller.startsWith("Dashboard")) {
        ReactDOM.render(
          <IntlProvider locale={language}>
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
          document.getElementById("react-main")
        );
        return;
      }

      if (controller !== null && action !== null) {
        ReactDOM.render(
          <IntlProvider locale={language}>
            <ThemeProvider theme={theme}>
              {BodyRouter.bind(controller, action)}
            </ThemeProvider>
          </IntlProvider>,
          document.getElementById("react-main")
        );
      }
    }
  }
// }


