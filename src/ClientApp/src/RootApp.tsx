import "./styles/site.scss";
import * as React from "react";
import { Router, RouteComponentProps, Redirect } from "@reach/router";
import NewSession from "./accounts/NewSession";
import NewUser from "./accounts/NewUser";
import EditUser from "./accounts/EditUser";
import { useAppCookie } from "./helpers/useCookie";

export interface IRootApp extends RouteComponentProps {
}

const RootApp: React.FunctionComponent<IRootApp> = ({
  path, location
}) => {
  const cookie = useAppCookie();

  if (cookie.userName) {
    if (location && location.pathname !== "/account") {
      return <Redirect noThrow to="/dashboard" />;
    }
  } else {
    if (location && location.pathname === "/account") {
      return <Redirect noThrow to="/login" />;
    }
  }
  return <Router>
        <Redirect noThrow from="/" to="/login" />
        <NewSession path="/login" />
        <NewUser path="/signup" />
        <EditUser path="/account" />
      </Router>;
  
};

export default RootApp;
