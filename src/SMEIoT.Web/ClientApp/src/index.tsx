import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import {SMEIoT} from "./routes";
import {Configuration} from "smeiot-client";

export function GetDefaultApiConfig() {
  return new Configuration({basePath: "https://localhost:5001"});
}

SMEIoT.BodyRouter.init();
