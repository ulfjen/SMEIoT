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
import { Link as ReachLink, RouteComponentProps } from "@reach/router";
import UserPasswordForm from "../components/UserPasswordForm";
import { useAppCookie } from "../helpers/useCookie";
import {SessionsApi} from "smeiot-client";
import {GetDefaultApiConfig} from "../index";
import useUserCredentials from "../helpers/useUserCredentials";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { useTitle } from 'react-use';


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

interface INewSessionProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const messages = defineMessages({
  title: {
    id: "sessions.new.title",
    description: "Title for new login",
    defaultMessage: "Log In"
  }
});

const _NewSession: React.FunctionComponent<INewSessionProps & WithStyles<typeof styles>> = ({classes}) => {
  const intl = useIntl();
  const uc = useUserCredentials();
  
  useTitle(intl.formatMessage(messages.title));
  var errorPrompt: string | undefined = undefined;

  const handleSubmit = async (event: React.MouseEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (event.target === undefined) {
      return;
    }

    const login = await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
      loginBindingModel: {
        userName: uc.userName,
        password: uc.password
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

  React.useEffect(() => {
    uc.setUserNameError("");
  }, [uc.userName]);
  React.useEffect(() => {
    uc.setPasswordError("");
  }, [uc.password]);

  return <Container component="main" maxWidth="xs">
    <CssBaseline/>
    <div className={classes.paper}>
      <Avatar className={classes.avatar}>
        <LockOutlinedIcon/>
      </Avatar>
      <Typography component="h1" variant="h5">
        {intl.formatMessage(messages.title)}
      </Typography>
      <UserPasswordForm handleSubmit={handleSubmit}
                        userCredentials={uc}>
        <Button
          type="submit"
          fullWidth
          variant="contained"
          color="primary"
          className={classes.submit}
        >
          <FormattedMessage
            id="sessions.new.action"
            description="Action label for the login"
            defaultMessage="Log in"
          />
        </Button>
        <Grid container justify="flex-end">
          <Grid item>
            <Link component={ReachLink} to="/signup" variant="body2">
              <FormattedMessage
              id="sessions.new.signup_action"
              description="Action label for redirecting to sign up page"
              defaultMessage="Don't have an account? Sign Up"
            />
            </Link>
          </Grid>
        </Grid>
      </UserPasswordForm>
    </div>
  </Container>;
};

const NewSession = withStyles(styles)(_NewSession);
export default NewSession;
