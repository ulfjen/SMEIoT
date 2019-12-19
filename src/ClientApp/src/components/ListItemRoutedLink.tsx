import * as React from "react";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import { Link } from "@reach/router";

export interface IListItemRoutedLink {
  icon?: React.ReactElement;
  primary: string;
  to: string;
}

const ListItemRoutedLink: React.FunctionComponent<IListItemRoutedLink> = ({ icon, primary, to }) => {
  return <ListItem button component={Link} to={to}>
    {icon ? <ListItemIcon>{icon}</ListItemIcon> : null}
    <ListItemText primary={primary} />
  </ListItem>
};

export default ListItemRoutedLink;
