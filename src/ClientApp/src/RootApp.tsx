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

  return cookie.userName && location && location.pathname === "/" ? <Redirect noThrow to="/dashboard" /> :
    <Router>
      <Redirect noThrow from="/" to="/login" />
      <NewSession path="/login" />
      <NewUser path="/signup" />
      <EditUser path="/account" />
    </Router>;
};

export default RootApp;
