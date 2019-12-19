import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
});


export interface IListItemLineProps extends WithStyles<typeof styles> {
  line: string;
}

const _ListItemLine: React.FunctionComponent<IListItemLineProps & WithStyles<typeof styles>> = ({ classes, line }) => {
  return <div>{line}</div>;
};


const ListItemLine = withStyles(styles)(_ListItemLine);
export default ListItemLine;
