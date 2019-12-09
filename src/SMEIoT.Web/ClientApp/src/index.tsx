import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import {Configuration} from "smeiot-client";
import * as React from "react";
import * as ReactDOM from "react-dom";
import DashboardApp from './DashboardApp';
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import palette from "./theme";
import useMediaQuery from '@material-ui/core/useMediaQuery';
import createMuiTheme from "@material-ui/core/styles/createMuiTheme";
import NewSession from './NewSession';
import NewUser from './NewUser';
import SMEIoTApp from './SMEIoTApp';

export function GetDefaultApiConfig() {
  return new Configuration({basePath: "http://localhost:5000"});
}

ReactDOM.render(
  <SMEIoTApp/>,
  document.getElementById("react-main"));
