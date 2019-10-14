import Container from '@material-ui/core/Container';
import * as React from "react";
import {createStyles, Theme, WithStyles} from "@material-ui/core";
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
import {AdminUserApiModel} from "smeiot-client/src";
import moment from "moment";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/core/SvgIcon/SvgIcon";
import Toolbar from "@material-ui/core/Toolbar";

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

export interface IDashboardEditUserProps extends WithStyles<typeof styles> {
}


const _DashboardEditUser: React.FunctionComponent<IDashboardEditUserProps & WithStyles<typeof styles>> = ({classes}) => {
  let user: AdminUserApiModel = {
    createdAt: moment.utc().toISOString(),
    id: 0,
    lastSeenAt: moment.utc().toISOString(),
    roles: [],
    username: ""
  };
  // @ts-ignore
  if (window.SMEIoTPreRendered) {
    // @ts-ignore
    user = window.SMEIoTPreRendered["user"];
  }
  const username = user.username;
  let roles = user.roles != null ? user.roles.join(", ") : "";
  const onClosedUrl = "/dashboard/users";

  return <Frame title={`Edit ${username}`} direction="ltr" toolbarRight={
    <IconButton
      edge="end"
      color="inherit"
      aria-label="close this action"
      onClick={() => { window.location.href = onClosedUrl; }}
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
          <Typography color="textSecondary">{roles}</Typography>
          <Typography>Created at: {user.createdAt}</Typography>
          <Typography>Last seen at: {user.lastSeenAt}</Typography>
        </CardContent>
        <CardActions>
          <Button onClick={() => { window.location.href = onClosedUrl;}}>Cancel</Button>
          <Button color="primary">Edit</Button>
        </CardActions>
      </Card>
    </Container>
  } />;
};

const DashboardEditUser = withStyles(styles)(_DashboardEditUser);

export default DashboardEditUser;
