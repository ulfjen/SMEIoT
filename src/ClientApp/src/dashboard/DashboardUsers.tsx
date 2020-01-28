import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import Container from '@material-ui/core/Container';
import List from "@material-ui/core/List";
import Typography from "@material-ui/core/Typography";
import Chip from "@material-ui/core/Chip";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import Skeleton from "@material-ui/lab/Skeleton";
import { GetDefaultApiConfig } from "../index";
import { AdminUserApiModel, AdminUsersApi, AdminUserApiModelList, AdminUsersApiApiAdminUsersGetRequest } from "smeiot-client";
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
import { defineMessages, useIntl, FormattedMessage } from 'react-intl';
import { useTitle } from 'react-use';
import { FixedSizeList } from 'react-window';
import InfiniteLoader from 'react-window-infinite-loader';
import ListItemAvatar from '@material-ui/core/ListItemAvatar';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';
import Avatar from '@material-ui/core/Avatar';
import { UserAvatar } from "..";
import IconButton from '@material-ui/core/IconButton';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import useModal from "../helpers/useModal";
import useMenu from "../helpers/useMenu";

const FILTERS = ["Admin"]

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  container: {
  },
  list: {
  },
  filterBar: {
    display: 'flex',
    flexWrap: 'wrap',
    alignItems: 'center',
    '& > *': {
      margin: spacing(0.5),
    },
    marginBottom: spacing(2)
  },
  usersMenuDeleteItem: {
    color: palette.error.main
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

function eqSet<T>(as: Set<T>, bs: Set<T>) {
  if (as.size !== bs.size) return false;
  for (var a of as) if (!bs.has(a)) return false;
  return true;
}

const ITEM_SIZE = 72;
const FRAME_PADDING = 24;

const _DashboardUsers: React.FunctionComponent<IDashboardUsersProps> = ({ classes, navigate, location }) => {
  if (location === undefined) { throw new Error("No location is found."); }
  const intl = useIntl();
  const appCookie = useAppCookie();

  useTitle(intl.formatMessage(messages.title));

  const pathFilters = new URLSearchParams(location.search).getAll("roles").map(f => f.toLowerCase());
  const [filters, setFilters] = React.useState<Record<string, boolean>>(FILTERS.reduce((map: Record<string, boolean>, f: string) => {
    map[f] = pathFilters.indexOf(f.toLowerCase()) !== -1;
    return map;
  }, {}));

  const [users, setUsers] = React.useState<Array<AdminUserApiModel>>([]);
  const [hasNextPage, setHasNextPage] = React.useState<boolean>(true);
  const [menuOpen, menuAnchorEl, openMenu, closeMenu, menuUserName] = useMenu<string>();
  const [dialogOpen, openDialog, closeDialog, dialogUserName] = useModal<string>();

  const handleDelete = React.useCallback(e => {
    e.stopPropagation();
    openDialog(menuUserName || "");
    closeMenu();
  }, [openDialog, closeMenu, menuUserName]);

  const handleDialogClose = React.useCallback(e => {
    e.stopPropagation();
    closeDialog();
  }, [closeDialog]);

  const handleDeleteClose = React.useCallback(async (e) => {
    closeDialog()
  }, [closeDialog]);

  const innerListType = React.useMemo(() => React.forwardRef((props, ref: React.Ref<HTMLUListElement>) => (
    <List ref={ref} {...props} />
  )), []);
  const isUserLoaded = (index: number) => !hasNextPage || index < users.length;
  
  const active = Object.entries(filters).filter(([_, enabled]) => enabled).map(([k, v]) => k.toLowerCase());

  React.useEffect(() => {
    if (!eqSet<string>(new Set(active), new Set(pathFilters))) {
      let path = location.pathname;
      if (active.length !== 0) {
        path = location.pathname + "?"+active.map(f => `roles=${f}`).join("&");
      }
      window.location.href = path;
    }
  }, [filters, pathFilters, active, location.pathname]);

  const loadMoreUsers = React.useCallback((startIndex: number, stopIndex: number) => {
    const req: AdminUsersApiApiAdminUsersGetRequest = {
      offset: startIndex, // we only render things when scroll down so no need to be extra fancy
      limit: USERS_PER_REQ
    };
    if (active && active.length > 0) {
      req["roles"] = active;
    }
    return new AdminUsersApi(GetDefaultApiConfig()).apiAdminUsersGet(req).then((result: AdminUserApiModelList) => {
      if (!result.users) {
        return;
      }
      closeMenu();

      const newUsers = users.concat(result.users);
      setUsers(newUsers);
      if (result.total && newUsers.length >= result.total) {
        setHasNextPage(false);
      }
      return result;
    });
  }, [closeMenu, users, active]);

  // for loading indicator
  const userCount = hasNextPage ? users.length + 1 : users.length;

  const flipFilter = (filter: string, value: boolean) => {
    const inc: Record<string, boolean> = {};
    inc[filter] = value;
    
    setFilters({ ...filters, ...inc });
  };
  const selectFilter = (filter: string) => flipFilter(filter, true);
  const unselectFilter = (filter: string) => flipFilter(filter, false);
  const filterSelected = (filter: string) => filters[filter];
  const renderFilter = (label: string, index: number) => {
    return <Chip key={index} onClick={() => selectFilter(label)} onDelete={() => unselectFilter(label)} color={filterSelected(label) ? "primary" : "default"} label={label} />;
  };

  const renderFilters = () => {
    return FILTERS.map((name, index) => renderFilter(name, index));
  }


  const initialHeight = userCount * ITEM_SIZE;
  const containerRef = React.createRef<HTMLElement>();
  const measureRef = React.createRef<HTMLDivElement>();
  const [width, setWidth] = React.useState(-1);
  const [height, setHeight] = React.useState(initialHeight);
  const measureAvailbleViewport = React.useCallback(() => {
    if (measureRef.current && containerRef.current) {
      const docHeight = containerRef.current.getBoundingClientRect().height;
      const measureRect = measureRef.current.getBoundingClientRect();
      setHeight(Math.min(userCount * ITEM_SIZE, docHeight - measureRect.top - FRAME_PADDING));
      setWidth(measureRect.width);
    }
  }, [measureRef, containerRef, setHeight, setWidth, userCount]);
  React.useEffect(() => measureAvailbleViewport(), [measureAvailbleViewport]);

  const userItemRenderer = React.useCallback(({ index, style }: {
    index: number,
    style: React.CSSProperties
  }) => {
    return index === users.length ? <ListItem key={-1} style={style}>
      <ListItemAvatar>
        <Skeleton variant="circle" width={40} height={40} />
      </ListItemAvatar>

      <ListItemText disableTypography primary={<Skeleton variant="rect" width={200} height={17} />} secondary={<Skeleton variant="text" />} />
    </ListItem> :
      <ListItem key={index} style={style}>
        <ListItemAvatar>
          <Avatar>
            {UserAvatar.getInstance().getSvg(users[index].userName || "")}
          </Avatar>
        </ListItemAvatar>

        <ListItemText primary={users[index].userName} secondary={intl.formatMessage(messages.seen, { seen: moment(users[index].lastSeenAt).fromNow() })} />
        <IconButton edge="end" onClick={(event) => { openMenu(event.currentTarget, users[index].userName) }}>
          <MoreVertIcon />
        </IconButton>
      </ListItem>;
  }, [users, openMenu, intl]);

  return <DashboardFrame
    title={intl.formatMessage(messages.title)}
    direction="ltr"
    drawer
    ref={containerRef}
    toolbarRight={<UserAvatarMenu appCookie={appCookie} navigate={navigate} />}
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Paper className={classes.filterBar}>
          <Typography>
            <FormattedMessage
              id="dashboard.users.index.list.filter.prompt"
              description="The text on prompt users"
              defaultMessage="Show:"
            />
          </Typography>
          {renderFilters()}
        </Paper>
        <Paper ref={measureRef}>
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
                itemSize={ITEM_SIZE}
                onItemsRendered={onItemsRendered}
                ref={ref}
                innerElementType={innerListType}
              >
                {userItemRenderer}
              </FixedSizeList>
            )}
          </InfiniteLoader>
        </Paper>

        <Menu
          anchorEl={menuAnchorEl}
          open={menuOpen}
          onClose={closeMenu}
        >
          <MenuItem onClick={closeMenu} to={`/dashboard/users/${menuUserName}`} component={Link}>
            <FormattedMessage
              id="dashboard.users.index.list.actions.edit"
              description="The text on edit user menu"
              defaultMessage="Edit"
            />
          </MenuItem>
          <MenuItem className={classes.usersMenuDeleteItem} onClick={handleDelete}>
            <FormattedMessage
              id="dashboard.users.index.list.actions.delete"
              description="The text on delete user menu"
              defaultMessage="Delete"
            />
          </MenuItem>
        </Menu>
        <Dialog
          open={dialogOpen}
          onClose={handleDialogClose}
          aria-labelledby="alert-dialog-title"
          aria-describedby="alert-dialog-description"
        >
          <DialogTitle>
            <FormattedMessage
              id="dashboard.users.index.delete.title"
              description="The text on delete user dialog"
              defaultMessage="Delete account {name}?"
              values={{name: dialogUserName}}
            />
          </DialogTitle>
          <DialogContent>
            <DialogContentText>
              <FormattedMessage
                id="dashboard.users.index.delete.instruction"
                description="The text on delete user dialog"
                defaultMessage="Delete the user will disable its access to the system. And this may result in unexpected behaviour."
              />
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleDialogClose} color="primary">
              <FormattedMessage
                id="dashboard.users.index.delete.actions.cancel"
                description="The text on delete user dialog"
                defaultMessage="Cancel"
              />
            </Button>
            <Button onClick={handleDeleteClose} color="primary" autoFocus>
              <FormattedMessage
                id="dashboard.users.index.delete.actions.delete"
                description="The text on delete user dialog"
                defaultMessage="Delete"
              />
            </Button>
          </DialogActions>
        </Dialog>
      </Container>} />;
};

const DashboardUsers = withStyles(styles)(_DashboardUsers);

export default DashboardUsers;
