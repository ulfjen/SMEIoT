import "./styles/site.scss";
import * as React from "react";
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import useMediaQuery from '@material-ui/core/useMediaQuery';
import createMuiTheme from "@material-ui/core/styles/createMuiTheme";
import palette from "./theme";
import { Router, RouteComponentProps } from "@reach/router";
import EnMessages from "./locales/en.json";
import NewSession from "./accounts/NewSession";
import NewUser from "./accounts/NewUser";
import EditUser from "./accounts/EditUser";

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

export interface IRootApp extends RouteComponentProps {
  locale?: string
}

const RootApp: React.FunctionComponent<IRootApp> = ({
  locale
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

  return <IntlProvider locale={locale} messages={messages}>
    <ThemeProvider theme={theme}>
      <Router>
        <NewSession path="/login" csrfToken={GetXsrfTokenFromDom()}/>
        <NewUser path="/signup" csrfToken={GetXsrfTokenFromDom()}/>
        <EditUser path="/account" csrfToken={GetXsrfTokenFromDom()}/>
      </Router>
    </ThemeProvider>
  </IntlProvider>;
};

export default RootApp;
