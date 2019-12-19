import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { emphasize } from "@material-ui/core/styles/colorManipulator";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  root: {
    wordWrap: "break-word",
    whiteSpace: "pre-wrap",
    backgroundColor: emphasize(palette.background.default)
  }
});


export interface IBlockCodeProps extends WithStyles<typeof styles> {
  children: string | React.ReactNode;
}

const _BlockCode: React.FunctionComponent<IBlockCodeProps & WithStyles<typeof styles>> = ({ classes, children }) => {
  return <pre className={classes.root}><code>{children}</code></pre>;
};


const BlockCode = withStyles(styles)(_BlockCode);
export default BlockCode;
