import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";



const styles = ({ palette, spacing }: Theme) => createStyles({
  triage: {
    margin: spacing(1),
    backgroundColor: palette.secondary.main,
  }
});

export interface ForgotPasswordProps extends WithStyles<typeof styles> { }

const ForgotPassword = withStyles(styles)(
  class extends React.Component<ForgotPasswordProps, {}> {
    render() {
      const { classes } = this.props;
      return <div>
        <p>Forgot password? Ask the administrator or reset the password.</p>
      </div>
    }
  }
);

export default ForgotPassword;
