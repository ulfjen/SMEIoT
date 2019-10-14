import * as React from "react";
import Typography from "@material-ui/core/Typography";

export interface TitleProps {
  children: JSX.Element
}

export default class Title extends React.Component<TitleProps, {}> {
  render() {
    return <Typography component="h2" variant="h6" color="primary" gutterBottom>
      {this.props.children}
    </Typography>;
  }

}
