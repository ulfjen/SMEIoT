import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import {Router} from "./routes";
import {Configuration} from "smeiot-client/src";

export namespace SMEIoT {
  export const _Router = Router;
}

export function GetDefaultApiConfig() {
  return new Configuration({basePath: "https://localhost:5001"});
}

SMEIoT._Router.init();
