import "./styles/site.scss";
import * as React from "react";
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import useMediaQuery from '@material-ui/core/useMediaQuery';
import createMuiTheme from "@material-ui/core/styles/createMuiTheme";
import palette from "./theme";
import { Router, RouteComponentProps, Redirect } from "@reach/router";
import EnMessages from "./locales/en.json";
import NewSession from "./accounts/NewSession";
import NewUser from "./accounts/NewUser";
import EditUser from "./accounts/EditUser";
import { useAppCookie } from "./helpers/useCookie";

export interface IRootApp extends RouteComponentProps {
  locale?: string
}

const RootApp: React.FunctionComponent<IRootApp> = ({
  locale, path, location
}) => {
  if (locale === undefined) {
    locale = "en";
  }
  let messages: Record<string, string>;
  switch (locale) {
    case "en":
    default:
      messages = EnMessages;
  }

  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');

  const theme = React.useMemo(
    () =>
      createMuiTheme({
        palette: Object.assign({
          type: prefersDarkMode ? 'dark' : 'light',
        }, palette)
      }),
    [prefersDarkMode],
  );
  const cookie = useAppCookie();

  return cookie.userName && location && location.pathname === "/" ? <Redirect noThrow to="/dashboard" /> :
    <IntlProvider locale={locale} messages={messages}>
      <ThemeProvider theme={theme}>
        <Router>
          <Redirect noThrow from="/" to="/login" />
          <NewSession path="/login" />
          <NewUser path="/signup" />
          <EditUser path="/account" />
        </Router>
      </ThemeProvider>
    </IntlProvider>;
};

export default RootApp;
