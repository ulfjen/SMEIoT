import * as React from "react";
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import Link from '@material-ui/core/Link';
import Grid from '@material-ui/core/Grid';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Container from '@material-ui/core/Container';
import { Router, RouteComponentProps } from "@reach/router";
import UserPasswordForm from "../components/UserPasswordForm";
import useUserCredentials from "../components/useUserCredentials";
import {
  Configuration, SessionsApi,
  UsersApi,
} from "smeiot-client";
import {GetDefaultApiConfig} from "../index";

const styles = ({palette, spacing}: Theme) => createStyles({
  '@global': {
    body: {
      backgroundColor: palette.common.white,
    },
  },
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
});

export interface INewUserProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const _NewUser: React.FunctionComponent<INewUserProps & WithStyles<typeof styles>> = ({classes}) => {
  const {
    userName, setUserName,
    password, setPassword,
    userNameErrors, setUserNameErrors,
    passwordErrors, setPasswordErrors
  } = useUserCredentials();

  const handleSubmit = async (event: React.MouseEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (event.target === undefined) {
      return;
    }

    try {
      const result = await new UsersApi(GetDefaultApiConfig()).apiUsersPost({
        validatedUserCredentialsBindingModel: {
          userName, password
        }
      });

      const login = await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
        loginBindingModel: {
          userName, password
        }
      });

      window.location.replace(login.returnUrl || "/");
    } catch (response) {
      const {status, errors} = await response.json();
      if (errors.hasOwnProperty("UserName")) {
        setUserNameErrors(errors["UserName"].join("\n"));
      }
      if (errors.hasOwnProperty("Password")) {
        setPasswordErrors(errors["Password"].join("\n"));
      }
    }
  };

    return <Container component="main" maxWidth="xs">
      <CssBaseline/>
      <div className={classes.paper}>
        <Avatar className={classes.avatar}>
          <LockOutlinedIcon/>
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign up
        </Typography>
        <UserPasswordForm url={undefined}
                          handleSubmit={handleSubmit}
                          userName={userName} setUserName={setUserName}
                          password={password} setPassword={setPassword}
                          userNameErrors={userNameErrors} setUserNameErrors={setUserNameErrors}
                          passwordErrors={passwordErrors} setPasswordErrors={setPasswordErrors}>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="primary"
            className={classes.submit}
          >
            Sign Up
          </Button>
          <Grid container justify="flex-end">
            <Grid item>
              <Link href="/login" variant="body2">
                Already have an account? Sign in
              </Link>
            </Grid>
          </Grid>
        </UserPasswordForm>
      </div>
    </Container>;
};

const NewUser = withStyles(styles)(_NewUser);

export default NewUser;
