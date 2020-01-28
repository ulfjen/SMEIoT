import "./styles/site.scss";
import * as React from "react";
import { Router } from "@reach/router";
import { IntlProvider } from "react-intl";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import useMediaQuery from '@material-ui/core/useMediaQuery';
import createMuiTheme from "@material-ui/core/styles/createMuiTheme";
import { palette, typography } from "./theme";
import RootApp from "./RootApp";
import EnMessages from "./locales/en.json";
import DashboardApp from "./DashboardApp";
import { useAppCookie } from "./helpers/useCookie";

const SMEIoTApp: React.FunctionComponent = () => {
  let locale = "en";
  let messages: Record<string, string>;
  switch (locale) {
    case "en":
    default:
      messages = EnMessages;
  }
  
  const appCookie = useAppCookie();
  React.useEffect(() => {
    if (!appCookie.admin) {
      return;
    }

    const script = document.createElement('script');
    script.id = "mini-profiler";
    script.src = "/profiler/includes.min.js";
    script.async = true;
    // a hack so that we get 404 instead of 500
    // this does not get to a security problem anyhow but it's better with a 4xx.
    script.dataset.ids = "00000000-0000-0000-0000-000000000000";
    script.dataset.version = "4.1.0+202001271656";
    script.dataset.path = "/profiler/";
    script.dataset.position = "BottomLeft";
    script.dataset.authorized = "true"
    script.dataset.maxTraces = "10";
    script.dataset.toggleShortcut = "Alt+P";
    script.dataset.trivialMilliseconds = "2.0";
    script.dataset.ignoredDuplicateExecuteTypes = "Open,OpenAsync,Close,CloseAsync";

    document.body.appendChild(script);

    return () => {
      document.body.removeChild(script);
    }
  }, [appCookie]);


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
