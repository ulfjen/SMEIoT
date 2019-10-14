import * as React from "react";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import {createStyles, Menu, Theme, WithStyles} from "@material-ui/core";
import withStyles from "@material-ui/core/styles/withStyles";

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
});

export interface IListItemMangageProps extends WithStyles<typeof styles> {
}

const ListItemManageMenu = withStyles(styles)(
  class extends React.Component<IListItemMangageProps, {}> {
    render() {
      const {} = this.props;

      return <div/>;//Menu keepMounted ></divMenu>;
    }
  }
);

export default ListItemManageMenu;
