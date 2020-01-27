import 'react-app-polyfill/stable';
import '@fortawesome/fontawesome-free/css/all.css';
import './styles/site.scss';
import { Configuration } from "smeiot-client";
import * as React from "react";
import * as ReactDOM from "react-dom";
import SMEIoTApp from './SMEIoTApp';
import Avatars from "@dicebear/avatars";
import sprites from "@dicebear/avatars-jdenticon-sprites";

export function GetDefaultApiConfig() {
  return new Configuration({
    basePath: window.location.origin,
    credentials: "same-origin",
    middleware: [{
      post: (ctx) => new Promise(resolve => {
        const status = ctx.response.status;
        if (status >= 300 && status < 400) {
          const location = ctx.response.headers.get("Location");
          if (location) {
            window.location.replace(location);
          }
        }
        resolve(ctx.response);
      })
    }]
  });
}

export class UserAvatar {
  private static instance: UserAvatar;

  public static getInstance(): UserAvatar {
    if (!UserAvatar.instance) {
      UserAvatar.instance = new UserAvatar();
    }

    return UserAvatar.instance;
  }

  private avatars: Avatars;

  private constructor() { 
    const options = {};
    this.avatars = new Avatars(sprites(options));
  }
  public create(userName: string) {
    return this.avatars.create(userName);
  }
  public getSvg(userName: string) {
    return <svg dangerouslySetInnerHTML={{ __html: this.create(userName) }} />;
  }
}

ReactDOM.render(
  <SMEIoTApp/>,
  document.getElementById("react-main"));
