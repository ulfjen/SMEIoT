import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import {Configuration} from "smeiot-client";
import * as React from "react";
import * as ReactDOM from "react-dom";
import SMEIoTApp from './SMEIoTApp';

export function GetDefaultApiConfig() {
  return new Configuration({basePath: window.location.origin});
}

ReactDOM.render(
  <SMEIoTApp/>,
  document.getElementById("react-main"));
