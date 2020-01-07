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
import Box from "@material-ui/core/Box";
import MenuItem from "@material-ui/core/MenuItem";
import Skeleton from "@material-ui/lab/Skeleton";
import { GetDefaultApiConfig } from "../index";
import { AdminUserApiModel, AdminUsersApi, AdminUserApiModelList } from "smeiot-client";
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
import { defineMessages, useIntl } from 'react-intl';
import { useTitle, useAsync } from 'react-use';
import { FixedSizeList, areEqual, ListChildComponentProps } from 'react-window';
import AutoSizer from "react-virtualized-auto-sizer";
import Grid from '@material-ui/core/Grid';
import InfiniteLoader from 'react-window-infinite-loader';
import ListItemAvatar from '@material-ui/core/ListItemAvatar';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';


const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
    height: '90%'
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
  usersMenu: {
  },
  usersMenuDeleteItem: {
    color: palette.error.main
  },
  item: {
  },
  card: {
    height: "100%"
  }
});

export interface IDashboardUsersProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const messages = defineMessages({
  title: {
    id: "dashboard.users.index.title",
    description: "Used as title in the user index page on the dashboard",
    defaultMessage: "Users"
  }
});

const USERS_PER_REQ = 10;

const _DashboardUsers: React.FunctionComponent<IDashboardUsersProps> = ({ classes, navigate }) => {
  const intl = useIntl();
  const appCookie = useAppCookie();

  useTitle(intl.formatMessage(messages.title));

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [users, setUsers] = React.useState<Array<AdminUserApiModel>>([]);
  const [focusedUserName, setFocusedUserName] = React.useState<null | string>(null);
  const [dialogOpen, setDiaglogOpen] = React.useState<boolean>(false);
  const [hasNextPage, setHasNextPage] = React.useState<boolean>(true);

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

  const outerListType = React.forwardRef((props, ref: React.Ref<HTMLUListElement>) => (
    <List ref={ref} {...props} />
  ));
  const isUserLoaded = (index: number) => !hasNextPage || index < users.length;
  const loadMoreUsers = (startIndex: number, stopIndex: number) => {
    return new AdminUsersApi(GetDefaultApiConfig()).apiAdminUsersGet({
      offset: startIndex, // we only render things when scroll down so no need to be extra fancy
      limit: USERS_PER_REQ
    }).then((result: AdminUserApiModelList) => {
      if (!result.users) {
        return;
      }
      const newUsers = users.concat(result.users);
      setUsers(newUsers);
      if (result.total && newUsers.length >= result.total) {
        setHasNextPage(false);
      }
      console.log(newUsers, result);
      return result;
    });
  }
  // for loading indicator
  const userCount = hasNextPage ? users.length + 1 : users.length;

  const userItemRenderer: React.FunctionComponent<ListChildComponentProps> = ({ index, style }) => {
    return <Box css={style}>
      {index === users.length ? <ListItem>
      <ListItemAvatar>
        <Skeleton variant="circle" width={40} height={40} />
      </ListItemAvatar>

      <ListItemText primary={<Skeleton variant="rect" width={200} height={17} />} secondary={<Skeleton variant="text" />} />
    </ListItem> : <UserListItem className={classes.item} user={users[index]} setFocusedUserName={setFocusedUserName} setAnchorEl={setAnchorEl} key={users[index].id} />
    }</Box>
  };

  return <DashboardFrame
    title={intl.formatMessage(messages.title)}
    direction="ltr"
    drawer
    toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate} />}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Paper className={classes.filterBar}>
          <Typography>Show:</Typography>
          <Chip label="Admin" />
        </Paper>
        <Card className={classes.card}>
          <AutoSizer>
            {({ height, width }) => (
              <InfiniteLoader
                isItemLoaded={isUserLoaded}
                itemCount={userCount}
                loadMoreItems={loadMoreUsers}
              >
                {({ onItemsRendered, ref }) => (
                  <FixedSizeList
                    className={classes.list}
                    height={height}
                    width={width}
                    itemCount={userCount}
                    itemSize={72}
                    onItemsRendered={onItemsRendered}
                    ref={ref}
                    outerElementType={outerListType}
                  >
                    {userItemRenderer}
                  </FixedSizeList>
                )}
              </InfiniteLoader>
            )}
          </AutoSizer>
        </Card>
        <Menu
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
