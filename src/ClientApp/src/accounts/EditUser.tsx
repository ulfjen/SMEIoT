import * as React from "react";
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Container from '@material-ui/core/Container';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import BuildIcon from '@material-ui/icons/Build';
import PasswordField from "../components/PasswordField";

import {
  Configuration, SessionsApi,
  UsersApi,
  BasicUserApiModel,
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import moment from "moment";
import useUserCredentials from "../components/useUserCredentials";
import { RouteComponentProps } from "@reach/router";

const styles = ({ palette, spacing }: Theme) => createStyles({
  '@global': {
    body: {
      backgroundColor: palette.common.white,
    },
  },
  container: {},
  paper: {
    marginTop: spacing(8),
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  },
  avatar: {
    margin: spacing(1),
    backgroundColor: palette.secondary.main,
  },
  form: {
    width: '100%', // Fix IE 11 issue.
    marginTop: spacing(3),
  },
  submit: {
    margin: spacing(3, 0, 2),
  },
  page: {
    marginTop: spacing(3)
  }
});

export interface IEditUserProps extends RouteComponentProps, WithStyles<typeof styles> {
  csrfToken: string
}

const _EditUser: React.FunctionComponent<IEditUserProps & WithStyles<typeof styles>> = ({ csrfToken, classes }) => {
  const {
    username, setUsername,
    password, setPassword,
    usernameErrors, setUsernameErrors,
    passwordErrors, setPasswordErrors
  } = useUserCredentials();

  const [newPassword, setNewPassword] = React.useState<string>("");
  const [newPasswordErrors, setNewPasswordErrors] = React.useState<string>("");


  let currentUser: BasicUserApiModel = {
    createdAt: moment.utc().toISOString(),
    roles: [],
    username: ""
  };

  // @ts-ignore
  if (window.SMEIoTPreRendered) {
    // @ts-ignore
    currentUser = window.SMEIoTPreRendered["currentUser"];
  }

  const handleEdit = async () => {
    try {
      const result = await new UsersApi(GetDefaultApiConfig()).apiUsersUsernamePasswordPut({
        username: currentUser.username || "",
        confirmedUserCredentialsUpdateBindingModel: {
          currentPassword: password,
          newPassword: newPassword
        }
      });

      window.location.replace("/dashboard");
    } catch (response) {
      const { status, errors } = await response.json();
      if (errors.hasOwnProperty("Username")) {
        setUsernameErrors(errors["Username"].join("\n"));
      }
      if (errors.hasOwnProperty("CurrentPassword")) {
        setPasswordErrors(errors["CurrentPassword"].join("\n"));
      }
      if (errors.hasOwnProperty("NewPassword")) {
        setNewPasswordErrors(errors["NewPassword"].join("\n"));
      }

    }
  };

  var roles = (currentUser.roles || []).join(", ");

  return <Container component="main" maxWidth="lg" className={classes.page}>
    <CssBaseline />
    <Card>
      <CardHeader
        avatar={
          <BuildIcon />
        }
        title={"Edit profile"}
      />
      <CardContent>
        <Typography variant="h5">
          {currentUser.username}
        </Typography>
        <Typography color="textSecondary">{roles}</Typography>
        <Typography>Created at: {moment(currentUser.createdAt).format("LLLL")}</Typography>

        <PasswordField
          label="Current Password"
          setPassword={setPassword}
          errors={passwordErrors}
          setErrors={setPasswordErrors} />
        <PasswordField
          label="New Password"
          setPassword={setNewPassword}
          errors={newPasswordErrors}
          setErrors={setNewPasswordErrors} />
      </CardContent>
      <CardActions>
        <Button onClick={() => { window.location.href = "/dashboard"; }}>Cancel</Button>
        <Button color="primary" onClick={handleEdit}>Edit</Button>
      </CardActions>
    </Card>
  </Container>;
};

const EditUser = withStyles(styles)(_EditUser);

export default EditUser;
