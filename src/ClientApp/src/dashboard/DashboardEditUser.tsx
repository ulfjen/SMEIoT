import Container from '@material-ui/core/Container';
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import Typography from "@material-ui/core/Typography";
import {GetDefaultApiConfig, UserAvatar} from "../index";
import Card from "@material-ui/core/Card";
import { AdminUserApiModel, AdminUsersApi } from "smeiot-client";
import moment from "moment";
import Avatar from "@material-ui/core/Avatar";
import CardHeader from "@material-ui/core/CardHeader";
import CardContent from "@material-ui/core/CardContent";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from '@material-ui/icons/Close';
import { Link, RouteComponentProps } from '@reach/router';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Switch from '@material-ui/core/Switch';

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
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
  },
  avatar: {},
  cardContent: {}
});

export interface IDashboardEditUserRouteParams {
  userName: string;
}

export interface IDashboardEditUserProps extends RouteComponentProps<IDashboardEditUserRouteParams>, WithStyles<typeof styles> {
  
}


const _DashboardEditUser: React.FunctionComponent<IDashboardEditUserProps> = ({ classes, userName }) => {
  const [user, setUser] = React.useState<AdminUserApiModel>({
    createdAt: moment.utc().toISOString(),
    id: 0,
    lastSeenAt: moment.utc().toISOString(),
    roles: [],
    userName: ""
  });
  const [admin, setAdmin] = React.useState<boolean>(false);  
  
  const saveUser = (user: AdminUserApiModel) => {
    setUser(user);

    if (user && user.roles && user.roles.indexOf("Admin") !== -1) {
      setAdmin(true);
    }
  }

  const requestUser = React.useCallback(async () => {
    if (userName === undefined || userName === null) { return; }
    saveUser(await new AdminUsersApi(GetDefaultApiConfig()).apiAdminUsersUserNameGet({
      userName
    }));
  }, [userName]);

  const handleRoleChange = async (event: React.ChangeEvent<HTMLInputElement>, checked: boolean) => {
    let userName = user.userName || "";
    let roles = user.roles || [];

    if (checked) {
      if (roles.indexOf("Admin")) {
        roles.push("Admin");
      }
    } else {
      roles = roles.filter(cur => cur !== "Admin");
    }

     var credentials = await new AdminUsersApi(GetDefaultApiConfig()).apiAdminUsersUserNameRolesPut({
       userName,
       userRolesBindingModel: {
         roles: roles || null
       }
     });
     user.roles = credentials.roles;
    setUser(user);

    setAdmin(checked);
  };

  React.useEffect(() => {
    // @ts-ignore
    if (window.SMEIoTPreRendered && window.SMEIoTPreRendered["user"]) {
      // @ts-ignore
      saveUser(window.SMEIoTPreRendered["user"]);
    } else {
      requestUser();
    }
  }, [requestUser]);
  return <DashboardFrame title={`Edit ${userName}`}
    drawer direction="ltr" toolbarRight={
    <IconButton
      edge="end"
      color="inherit"
      aria-label="close this action"
      to={"/dashboard/users"}
      component={Link}
    >
      <CloseIcon/>
    </IconButton>
  }
  content={
    <Container maxWidth="lg" className={classes.container}>
      <Card>
        <CardHeader
          avatar={
            <Avatar className={classes.avatar}>
              {UserAvatar.getInstance().create(userName || "")}
            </Avatar>
          }
          action={
            <IconButton aria-label="settings">
            </IconButton>
          }
          title={userName}
          subheader={user.id}
        />
        <CardContent className={classes.cardContent}>
          <FormControlLabel
            control={<Switch checked={admin} onChange={handleRoleChange} value="Admin" />}
            label="Admin"
          />
          <Typography>Created at: {moment(user.createdAt).format("LLLL")}</Typography>
          <Typography>Last seen at: {moment(user.lastSeenAt).format("LLLL")}</Typography>
        </CardContent>
      </Card>
    </Container>
  } />;
};

const DashboardEditUser = withStyles(styles)(_DashboardEditUser);

export default DashboardEditUser;
