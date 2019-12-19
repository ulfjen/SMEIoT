import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { emphasize } from "@material-ui/core/styles/colorManipulator";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  root: {
    backgroundColor: emphasize(palette.background.default)
  }
});

export interface ILineCodeProps extends WithStyles<typeof styles> {
  children: string | React.ReactNode;
}

const _LineCode: React.FunctionComponent<ILineCodeProps & WithStyles<typeof styles>> = ({ classes, children }) => {
  return <code className={classes.root}>{children}</code>;
};


const LineCode = withStyles(styles)(_LineCode);
export default LineCode;
