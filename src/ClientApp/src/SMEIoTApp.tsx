import "./styles/site.scss";
import * as React from "react";
import { Router, RouteComponentProps } from "@reach/router";
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import useMediaQuery from '@material-ui/core/useMediaQuery';
import createMuiTheme from "@material-ui/core/styles/createMuiTheme";
import { palette, typography } from "./theme";
import RootApp from "./RootApp";
import EnMessages from "./locales/en.json";
import DashboardApp from "./DashboardApp";

export interface ISMEIoTApp {
}

const SMEIoTApp: React.FunctionComponent<ISMEIoTApp> = ({
}) => {
  let locale = "en";
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
        }, palette),
        typography
      }),
    [prefersDarkMode],
  );

  return <IntlProvider locale={locale} messages={messages}>
    <ThemeProvider theme={theme}>
      <Router>
        <DashboardApp path="/dashboard/*" />
        <RootApp path="/*" />
      </Router>
    </ThemeProvider>
  </IntlProvider>;
};

export default SMEIoTApp;
