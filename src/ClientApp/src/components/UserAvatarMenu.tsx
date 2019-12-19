import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import IconButton from "@material-ui/core/IconButton";
import Avatar from "@material-ui/core/Avatar";
import {AdminUserApiModel} from "smeiot-client";
import Avatars from "@dicebear/avatars";
import sprites from "@dicebear/avatars-jdenticon-sprites";
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';


const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
});

export interface IUserAvatarMenuProps extends WithStyles<typeof styles> {
  user: AdminUserApiModel;
}

const _UserAvatarMenu: React.FunctionComponent<IUserAvatarMenuProps & WithStyles<typeof styles>> = ({ classes, user }) => {
  const username = user.username || "";
  let options = {};
  let avatars = new Avatars(sprites(options));
  const avatar = avatars.create(username);

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };  

  const handleLogout = async () => {
    const url = "/logout";
    await fetch(url, {
      method: 'delete',
      redirect: 'manual'
    })
    .then((resp) => {
      window.location.href = "/";
    });
  }

  return <div>
    <IconButton
      aria-label="account of current user"
      aria-controls="menu-appbar"
      aria-haspopup="true"
      onClick={handleClick}
      color="inherit"
    >
      <Avatar>
        <svg dangerouslySetInnerHTML={{ __html: avatar }} />
      </Avatar>
    </IconButton>
    <Menu
      id="menu-appbar"
      anchorEl={anchorEl}
      anchorOrigin={{
        vertical: 'top',
        horizontal: 'right',
      }}
      keepMounted
      transformOrigin={{
        vertical: 'top',
        horizontal: 'right',
      }}
      open={anchorEl != null}
      onClose={handleClose}
    >
      <MenuItem onClick={() => window.location.href = "/account"}>My account</MenuItem>
      <MenuItem onClick={handleLogout}>Log out</MenuItem>
    </Menu>
  </div>;
};


const UserAvatarMenu = withStyles(styles)(_UserAvatarMenu);
export default UserAvatarMenu;
