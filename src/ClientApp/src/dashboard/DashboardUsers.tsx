import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import Container from '@material-ui/core/Container';
import List from "@material-ui/core/List";
import UserListItem from "./UserListItem";
import Typography from "@material-ui/core/Typography";
import Chip from "@material-ui/core/Chip";
import Menu from "@material-ui/core/Menu";
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
import Grid from '@material-ui/core/Grid';
import InfiniteLoader from 'react-window-infinite-loader';
import ListItemAvatar from '@material-ui/core/ListItemAvatar';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';
import Box from "@material-ui/core/Box";
import useMenu from '../helpers/useMenu';
import Avatar from '@material-ui/core/Avatar';
import { UserAvatar } from "..";
import IconButton from '@material-ui/core/IconButton';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import Measure from 'react-measure';

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  container: {
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
  }
});

export interface IDashboardUsersProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const messages = defineMessages({
  title: {
    id: "dashboard.users.index.title",
    description: "Used as title in the user index page on the dashboard",
    defaultMessage: "Users"
  },
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

const USERS_PER_REQ = 10;



const ITEM_SIZE = 72;
const FRAME_PADDING = 24;

const _DashboardUsers: React.FunctionComponent<IDashboardUsersProps> = ({ classes, navigate }) => {
  const intl = useIntl();
  const appCookie = useAppCookie();

  useTitle(intl.formatMessage(messages.title));

  const [users, setUsers] = React.useState<Array<AdminUserApiModel>>([]);
  const [focusedUserName, setFocusedUserName] = React.useState<null | string>(null);
  const [dialogOpen, setDiaglogOpen] = React.useState<boolean>(false);
  const [hasNextPage, setHasNextPage] = React.useState<boolean>(true);
  const [menuAnchor, menuItem, handleMenuClose, openMenu] = useMenu();
  const openMenucb = React.useCallback(openMenu, []);

  const handleEdit = React.useCallback(e => {
    e.stopPropagation();
    handleMenuClose();
    window.location.href = `/dashboard/users/${focusedUserName}`;
  }, [handleMenuClose]);

  const handleDelete = React.useCallback(e => {
    e.stopPropagation();
    handleMenuClose();
    setDiaglogOpen(true);
  }, [handleMenuClose, setDiaglogOpen]);

  const handleDialogClose = React.useCallback(e => {
    e.stopPropagation();
    setDiaglogOpen(false);
  }, [setDiaglogOpen]);

  const handleDeleteClose = React.useCallback(async (e) => {
    setDiaglogOpen(false);
  }, [setDiaglogOpen]);

  const innerListType = React.forwardRef((props, ref: React.Ref<HTMLUListElement>) => (
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
      console.log("setting newUser", newUsers, result);
      return result;
    });
  }
  // for loading indicator
  const userCount = hasNextPage ? users.length + 1 : users.length;

  const containerRef = React.createRef<HTMLElement>();
  const measureRef = React.createRef<HTMLDivElement>();
  const [listHeight, setListHeight] = React.useState(userCount * ITEM_SIZE);
  const [listWidtih, setListWidth] = React.useState(-1);

  const callback = () => {
    if (measureRef.current && containerRef.current) {
    const docHeight = containerRef.current.getBoundingClientRect().height;
    setListHeight(Math.min(userCount * ITEM_SIZE, docHeight - measureRef.current.getBoundingClientRect().top - FRAME_PADDING));
    setListWidth(measureRef.current.getBoundingClientRect().width);
    }
  }

  React.useEffect(() => {
    callback();

  }, [callback]);

  const userItemRenderer = React.useCallback(({ index, style }: {
    index: number,
    style: React.CSSProperties
  }) => {
    console.log("rendering", index);
    return index === users.length ? <ListItem key={-1} style={style}>
      <ListItemAvatar>
        <Skeleton variant="circle" width={40} height={40} />
      </ListItemAvatar>
  
      <ListItemText disableTypography primary={<Skeleton variant="rect" width={200} height={17} />} secondary={<Skeleton variant="text" />} />
    </ListItem> :
      <ListItem key={index} style={style} >
        <ListItemAvatar>
          <Avatar>
            {UserAvatar.getInstance().getSvg(users[index].userName || "")}
          </Avatar>
        </ListItemAvatar>
  
        <ListItemText primary={users[index].userName} secondary={null} />
        <div>
          <IconButton edge="end" onClick={(event) => openMenu(event.currentTarget, index)}>
            <MoreVertIcon />
          </IconButton>
        </div>
      </ListItem>;
  }, [users, openMenu]);

  return <DashboardFrame
    title={intl.formatMessage(messages.title)}
    direction="ltr"
    drawer
    ref={containerRef}
    toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate} />}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Paper className={classes.filterBar}>
          <Typography>Show:</Typography>
          <Chip label="Admin" />
        </Paper>
        <div ref={measureRef} className={classes.card}>
          <InfiniteLoader
            isItemLoaded={isUserLoaded}
            itemCount={userCount}
            loadMoreItems={loadMoreUsers}
          >
            {({ onItemsRendered, ref }) => (
              <FixedSizeList
                className={classes.list}
                height={listHeight}
                width={listWidtih}
                itemCount={userCount}
                itemSize={ITEM_SIZE}
                onItemsRendered={onItemsRendered}
                ref={ref}
                innerElementType={innerListType}
              >
                {userItemRenderer}
              </FixedSizeList>
            )}
          </InfiniteLoader>
          </div>

        <Menu
          anchorEl={menuAnchor}
          keepMounted
          open={Boolean(menuAnchor)}
          className={classes.usersMenu}
          onClose={handleMenuClose}
        >
          <MenuItem onClick={handleMenuClose} to={`/dashboard/users/${focusedUserName}`} component={Link}>Edit</MenuItem>
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
