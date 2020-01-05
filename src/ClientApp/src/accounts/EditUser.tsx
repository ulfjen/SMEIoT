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
import useUserCredentials from "../helpers/useUserCredentials";
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
}

const _EditUser: React.FunctionComponent<IEditUserProps & WithStyles<typeof styles>> = ({ classes }) => {
  const uc = useUserCredentials();

  const [newPassword, setNewPassword] = React.useState<string>("");
  const [newPasswordError, setNewPasswordError] = React.useState<string>("");


  let currentUser: BasicUserApiModel = {
    createdAt: moment.utc().toISOString(),
    roles: [],
    userName: ""
  };

  // @ts-ignore
  if (window.SMEIoTPreRendered) {
    // @ts-ignore
    currentUser = window.SMEIoTPreRendered["currentUser"];
  }

  const handleEdit = async () => {
    try {
      const result = await new UsersApi(GetDefaultApiConfig()).apiUsersUserNamePasswordPut({
        userName: currentUser.userName || "",
        confirmedUserCredentialsUpdateBindingModel: {
          currentPassword: uc.password,
          newPassword: newPassword
        }
      });

      window.location.replace("/dashboard");
    } catch (response) {
      const { status, errors } = await response.json();
      if (errors.hasOwnProperty("UserName")) {
        uc.setUserNameError(errors["UserName"].join("\n"));
      }
      if (errors.hasOwnProperty("CurrentPassword")) {
        uc.setPasswordError(errors["CurrentPassword"].join("\n"));
      }
      if (errors.hasOwnProperty("NewPassword")) {
        setNewPasswordError(errors["NewPassword"].join("\n"));
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
          {currentUser.userName}
        </Typography>
        <Typography color="textSecondary">{roles}</Typography>
        <Typography>Created at: {moment(currentUser.createdAt).format("LLLL")}</Typography>

        <PasswordField
          label="Current Password"
          setPassword={uc.setPassword}
          errors={uc.passwordError}
          setErrors={uc.setPasswordError} />
        <PasswordField
          label="New Password"
          setPassword={setNewPassword}
          errors={newPasswordError}
          setErrors={setNewPasswordError} />
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
