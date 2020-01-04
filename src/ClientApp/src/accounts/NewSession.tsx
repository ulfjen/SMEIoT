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
import { useAppCookie } from "../helpers/useCookie";
import {SessionsApi} from "smeiot-client";
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

export interface INewSessionProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const _NewSession: React.FunctionComponent<INewSessionProps & WithStyles<typeof styles>> = ({classes}) => {
  const {
    userName, setUserName,
    password, setPassword,
    userNameErrors, setUserNameErrors,
    passwordErrors, setPasswordErrors
  } = useUserCredentials();
  const cookie = useAppCookie();

  var errorPrompt: string | undefined = undefined;
  // @ts-ignore
  const SMEIoTPreRendered = window["SMEIoTPreRendered"];
  if (SMEIoTPreRendered) {
    const model = SMEIoTPreRendered.model;
    const errors = SMEIoTPreRendered.validation_errors;
    if (errors) {
      errorPrompt = errors[0].name;
    }

    if (model) {
      setUserName(model.userName);
    }
  }

  const handleSubmit = async (event: React.MouseEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (event.target === undefined) {
      return;
    }

      const login = await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
        loginBindingModel: {
          userName, password
        }
      });
      console.log(login);

      window.location.replace(login.returnUrl || "/");
    // catch (response) {
    //   const {status, errors} = await response.json();
    //   if (errors.hasOwnProperty("UserName")) {
    //     setUserNameErrors(errors["UserName"].join("\n"));
    //   }
    //   if (errors.hasOwnProperty("Password")) {
    //     setPasswordErrors(errors["Password"].join("\n"));
    //   }
    // }
  };

  return <Container component="main" maxWidth="xs">
    <CssBaseline/>
    <div className={classes.paper}>
      <Avatar className={classes.avatar}>
        <LockOutlinedIcon/>
      </Avatar>
      <Typography component="h1" variant="h5">
        Log in
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
          Log in
        </Button>
        <Grid container justify="flex-end">
          <Grid item>
            <Link href="/signup" variant="body2">
              {"Don't have an account? Sign Up"}
            </Link>
          </Grid>
        </Grid>
      </UserPasswordForm>
    </div>
  </Container>;
};

const NewSession = withStyles(styles)(_NewSession);
export default NewSession;
