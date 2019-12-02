import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import {Configuration} from "smeiot-client";
import * as React from "react";
import * as ReactDOM from "react-dom";
import DashboardApp from './DashboardApp';

export function GetDefaultApiConfig() {
  return new Configuration({basePath: "https://localhost:5001"});
}

ReactDOM.render(<DashboardApp/>,
  document.getElementById("react-main"));
