import Container from '@material-ui/core/Container';
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import List from "@material-ui/core/List";
import UserListItem from "../components/UserListItem";
import Typography from "@material-ui/core/Typography";
import Chip from "@material-ui/core/Chip";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import Skeleton from "@material-ui/lab/Skeleton";
import {GetDefaultApiConfig} from "../index";
import {AdminUserApiModel, AdminUsersApi, BasicUserApiModel} from "smeiot-client";
import Card from "@material-ui/core/Card";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import DialogTitle from "@material-ui/core/DialogTitle";
import Paper from '@material-ui/core/Paper';
import moment from 'moment';
import { Link, RouteComponentProps } from '@reach/router';
import UserAvatarMenu from '../components/UserAvatarMenu';
import { useAppCookie } from '../helpers/useCookie';


const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  list: {
  },
  filterBar: {
    display: 'flex',
    flexWrap: 'wrap',
    '& > *': {
      margin: spacing(0.5),
    },
    marginBottom: spacing(2)
  },
  paper: {
    padding: spacing(2),
    display: 'flex',
    overflow: 'auto',
    flexDirection: 'column',
  },
  fixedHeight: {
    height: 240,
  },
  usersMenu: {
  },
  usersMenuDeleteItem: {
    color: palette.error.main
  }
});

export interface IDashboardUsersProps extends RouteComponentProps, WithStyles<typeof styles> {
}


const _DashboardUsers: React.FunctionComponent<IDashboardUsersProps> = ({classes, navigate}) => {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [loaded, setLoaded] = React.useState<boolean>(false);
  const [users, setUsers] = React.useState<null | Array<AdminUserApiModel>>(null);
  const [focusedUserName, setFocusedUserName] = React.useState<null | string>(null);
  const [dialogOpen, setDiaglogOpen] = React.useState<boolean>(false);

  const handleClose = () => {
    setAnchorEl(null);
  };
  
  const handleEdit = () => {
    handleClose();
    window.location.href = `/dashboard/users/${focusedUserName}`;
  };

  const handleDelete = () => {
    handleClose();
    setDiaglogOpen(true);
  };
  
  const handleDialogClose = () => {
    setDiaglogOpen(false);
  };
  
  const handleDeleteClose = async () => {
    setDiaglogOpen(false);
  };

  const requestUsers = async (start = 1, limit = 10) => {
    if (loaded) { return; }
    const result = await new AdminUsersApi(GetDefaultApiConfig()).apiAdminUsersGet({
        start, limit
    });
    setLoaded(true);
    setUsers((result.users as ((prevState: (Array<AdminUserApiModel> | null)) => (Array<AdminUserApiModel> | null)) | Array<AdminUserApiModel> | null));
  };

  const renderUserLists = () => {
    if (users == null) { return null; }

    return users.map(user => user ? <UserListItem user={user} setFocusedUserName={setFocusedUserName} setAnchorEl={setAnchorEl} key={user.id}/> : null);
  };

  React.useEffect(() => {
    requestUsers();
  }, []);

  const appCookie = useAppCookie();

  return <DashboardFrame title="Users" direction="ltr"
    toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate}/>}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Paper className={classes.filterBar}>
          <Typography>Show:</Typography>
          <Chip label="Admin"/>
        </Paper>
        <Card>
          <List className={classes.list}>
            {loaded ? (
              renderUserLists()
            ) : (
              <Skeleton variant="rect" height={4}/>
            )}
          </List>
        </Card>
        <Menu
          id="user-management-menu"
          anchorEl={anchorEl}
          keepMounted
          open={Boolean(anchorEl)}
          className={classes.usersMenu}
          onClose={handleClose}
        >
          <MenuItem onClick={handleClose} to={`/dashboard/users/${focusedUserName}`} component={Link}>Edit</MenuItem>
          <MenuItem className={classes.usersMenuDeleteItem} onClick={handleDelete}>Delete</MenuItem>
        </Menu>
        <Dialog
          open={dialogOpen}
          onClose={handleDialogClose}
          aria-labelledby="alert-dialog-title"
          aria-describedby="alert-dialog-description"
        >
          <DialogTitle>Delete account {focusedUserName}?</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Delete the user will disable its access to the system and asscociated users.
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleDialogClose} color="primary">
              Cancel
            </Button>
            <Button onClick={handleDeleteClose} color="primary" autoFocus>
              Delete
            </Button>
          </DialogActions>
        </Dialog>
      </Container>} />;
};

const DashboardUsers = withStyles(styles)(_DashboardUsers);

export default DashboardUsers;
