import * as React from "react";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";

export interface IListItemLinkProps {
  icon?: React.ReactElement;
  primary: string;
  to: string;
}

export default class ListItemLink extends React.Component<IListItemLinkProps, {}> {
  render() {
    const { icon, primary, to } = this.props;
    
    return <ListItem button component="a" href={to}>
      {icon ? <ListItemIcon>{icon}</ListItemIcon> : null}
      <ListItemText primary={primary}/>
    </ListItem>
  }
}
