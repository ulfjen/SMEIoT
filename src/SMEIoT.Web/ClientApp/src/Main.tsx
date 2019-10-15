import * as React from "react";
import ThemeProvider from "@material-ui/styles/ThemeProvider";
import theme from "./theme";
import {Router} from "./routes";

export interface MainProps {
  controller: string
  action: string
}

class Main extends React.Component<MainProps, {}> {
  render() {
    const {controller, action} = this.props;

    return <ThemeProvider theme={theme}>
      {Router.bind(controller, action)}
    </ThemeProvider>;
  }
}

export default Main;
