import ListItemText from "@material-ui/core/ListItemText";
import ListItem from "@material-ui/core/ListItem";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import IconButton from "@material-ui/core/IconButton";
import MoreVertIcon from '@material-ui/icons/MoreVert';
import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import Avatar from "@material-ui/core/Avatar";
import {AdminUserApiModel} from "smeiot-client";
import moment from "moment";
import { defineMessages, useIntl } from "react-intl";
import { UserAvatar } from "..";

const styles = ({palette}: Theme) => createStyles({
});

export interface IUserListItemProps extends WithStyles<typeof styles> {
  user: AdminUserApiModel;
  setAnchorEl: React.Dispatch<React.SetStateAction<null | HTMLElement>>;
  setFocusedUserName: React.Dispatch<React.SetStateAction<null | string>>;
  className?: string;
  style?: React.CSSProperties;
}

const messages = defineMessages({
  seen: {
    id: "dashboard.components.avatar_list_item.seen",
    description: "Used as the template message in the user list item",
    defaultMessage: "Last seen {seen}"
  },
  moreAria: {
    id: "dashboard.components.avatar_list_item.action.aria.more",
    description: "Used as an aria label",
    defaultMessage: "More"
  }
});

const _UserListItem: React.FunctionComponent<IUserListItemProps & WithStyles<typeof styles>> = ({classes, style, className, user, setAnchorEl, setFocusedUserName}) => {
  const intl = useIntl();
  const userName = user.userName || "";
  const lastSeenAt = moment(user.lastSeenAt).fromNow();
  
  const onClick = (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    setFocusedUserName(userName);
    console.log(event.currentTarget); 
    setAnchorEl(event.currentTarget);
  };
  
  return <ListItem style={style} className={className}>
    <ListItemAvatar>
      <Avatar>
      {UserAvatar.getInstance().getSvg(userName)}
      </Avatar>
    </ListItemAvatar>

    <ListItemText primary={userName} secondary={intl.formatMessage(messages.seen, { seen: lastSeenAt })}/>
    <div>
      <IconButton edge="end" aria-label={intl.formatMessage(messages.moreAria)} onClick={onClick}>
        <MoreVertIcon/>
      </IconButton>
    </div>
  </ListItem>;
};


const UserListItem = withStyles(styles)(_UserListItem);
export default UserListItem;
