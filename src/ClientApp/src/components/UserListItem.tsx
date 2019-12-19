import ListItemText from "@material-ui/core/ListItemText";
import ListItem from "@material-ui/core/ListItem";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import IconButton from "@material-ui/core/IconButton";
import MoreVertIcon from '@material-ui/icons/MoreVert';
import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import Avatar from "@material-ui/core/Avatar";
import {AdminUserApiModel} from "smeiot-client";
import moment from "moment";
import { Avatars } from "../avatars";

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
});

export interface IUserListItemProps extends WithStyles<typeof styles> {
  user: AdminUserApiModel;
  setAnchorEl: React.Dispatch<React.SetStateAction<null | HTMLElement>>;
  setFocusedUsername: React.Dispatch<React.SetStateAction<null | string>>;
}

const _UserListItem: React.FunctionComponent<IUserListItemProps & WithStyles<typeof styles>> = ({classes, user, setAnchorEl, setFocusedUsername}) => {
  const username = user.username || "";
  const avatar = Avatars.create(username);
  const lastSeenAt = moment(user.lastSeenAt).fromNow();
  
  const onClick = (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    setFocusedUsername(username);
    setAnchorEl(event.currentTarget);
  };
  
  return <ListItem>
    <ListItemAvatar>
      <Avatar>
        <svg dangerouslySetInnerHTML={{__html: avatar}} />
      </Avatar>
    </ListItemAvatar>

    <ListItemText primary={username} secondary={`Last seen at ${lastSeenAt}`}/>
    <ListItemSecondaryAction>
      <IconButton edge="end" aria-label="delete" onClick={onClick}>
        <MoreVertIcon/>
      </IconButton>
    </ListItemSecondaryAction>
  </ListItem>;
};


const UserListItem = withStyles(styles)(_UserListItem);
export default UserListItem;
