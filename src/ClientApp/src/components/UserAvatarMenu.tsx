import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import IconButton from "@material-ui/core/IconButton";
import Avatar from "@material-ui/core/Avatar";
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import { AppCookie } from "../helpers/useCookie";
import { NavigateFn } from "@reach/router";
import { UserAvatar } from "..";

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
  },
});

export interface IUserAvatarMenuProps extends WithStyles<typeof styles> {
  appCookie: AppCookie;
  navigate?: NavigateFn;
}

const _UserAvatarMenu: React.FunctionComponent<IUserAvatarMenuProps & WithStyles<typeof styles>> = ({ classes, appCookie, navigate }) => {
  const userName = appCookie.userName || "";
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
        {UserAvatar.getInstance().getSvg(userName)}
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
      <MenuItem onClick={() => navigate && navigate("/account")}>My account</MenuItem>
      <MenuItem onClick={handleLogout}>Log out</MenuItem>
    </Menu>
  </div>;
};


const UserAvatarMenu = withStyles(styles)(_UserAvatarMenu);
export default UserAvatarMenu;
