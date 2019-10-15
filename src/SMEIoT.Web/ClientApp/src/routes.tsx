import NewUser from "./NewUser";
import EditUser from "./EditUser";
import SensorDetails from "./SensorDetails";
import Main from "./Main";
import * as React from "react";
import * as ReactDOM from "react-dom";
import NewSession from "./NewSession";
import DashboardUsers from "./dashboard/DashboardUsers";
import DashboardSensors from "./dashboard/DashboardSensors";
import DashboardEditUser from "./dashboard/DashboardEditUser";
import DashboardIndex from "./dashboard/DashboardIndex";

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

    EditUser() {
      return <EditUser csrfToken={GetXsrfTokenFromDom()} />;
    },

  },

  Sensors: {
    Show() {
      return <SensorDetails/>;
    }
  },

  Dashboard: {
    Index() {
      return <DashboardIndex/>;
    },
    
    Sensors() {
      return <DashboardSensors/>;
    },
    
    Users() {
      return <DashboardUsers/>;
    },
  },
  
  DashboardUsers: {
    EditUser() {
      return <DashboardEditUser/>;
    }
  }
};

class Router {
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

    if (controller != null && action != null) {
      ReactDOM.render(<Main controller={controller} action={action} />, document.getElementById("react-main"));
    }
  }
}

export { Router };

