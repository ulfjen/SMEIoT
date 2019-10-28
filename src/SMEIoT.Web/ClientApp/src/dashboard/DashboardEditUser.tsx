import Container from '@material-ui/core/Container';
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Frame from "./Frame";
import List from "@material-ui/core/List";
import UserListItem from "../components/UserListItem";
import Typography from "@material-ui/core/Typography";
import Chip from "@material-ui/core/Chip";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import Skeleton from "@material-ui/lab/Skeleton";
import {GetDefaultApiConfig} from "../index";
import Card from "@material-ui/core/Card";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import DialogTitle from "@material-ui/core/DialogTitle";
import {AdminUserApiModel, UsersApi} from "smeiot-client/src";
import moment from "moment";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from '@material-ui/icons/Close';
import Toolbar from "@material-ui/core/Toolbar";
import { Link, RouteComponentProps } from '@reach/router';

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  list: {
    backgroundColor: "#ffffff"
  },
  filterBar: {
    backgroundColor: "#ffffff",
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
  usersMenu: {},
  usersMenuDeleteItem: {
    color: palette.error.main
  }
});

export interface IDashboardEditUserRouteParams {
  username: string;
}

export interface IDashboardEditUserProps extends RouteComponentProps<IDashboardEditUserRouteParams>, WithStyles<typeof styles> {
  
}


const _DashboardEditUser: React.FunctionComponent<IDashboardEditUserProps> = ({classes, username}) => {
  let user: AdminUserApiModel = {
    createdAt: moment.utc().toISOString(),
    id: 0,
    lastSeenAt: moment.utc().toISOString(),
    roles: [],
    username: ""
  };

  const requestUser = async () => {
    if (username === undefined || username === null) { return; }
    user = await new UsersApi(GetDefaultApiConfig()).apiUsersUsernameGet({
      username
    });
  };


  // @ts-ignore
  if (window.SMEIoTPreRendered && window.SMEIoTPreRendered["user"]) {
    // @ts-ignore
    user = window.SMEIoTPreRendered["user"];
  } else {
    requestUser();
  }
  const onClosedUrl = "/dashboard/users";

  return <Frame title={`Edit ${username}`} direction="ltr" toolbarRight={
    <IconButton
      edge="end"
      color="inherit"
      aria-label="close this action"
      to={onClosedUrl}
      component={Link}
    >
      <CloseIcon/>
    </IconButton>
  }
  content={
    <Container maxWidth="lg" className={classes.container}>
      <Card>
        <CardContent>
          <Typography variant="h5" component="h2">{username} <span color="textSecondary">
          ({user.id})
        </span></Typography>
          <Typography color="textSecondary">{user && user.roles ? user.roles.join(", ") : ""}</Typography>
          <Typography>Created at: {user.createdAt}</Typography>
          <Typography>Last seen at: {user.lastSeenAt}</Typography>
        </CardContent>
        <CardActions>
          <Button to={onClosedUrl} component={Link}>Cancel</Button>
          <Button color="primary">Edit</Button>
        </CardActions>
      </Card>
    </Container>
  } />;
};

const DashboardEditUser = withStyles(styles)(_DashboardEditUser);

export default DashboardEditUser;
