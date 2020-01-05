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
import useUserCredentials from "../helpers/useUserCredentials";
import {
  Configuration, SessionsApi,
  UsersApi,
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { FormattedMessage, defineMessages, useIntl } from "react-intl";
import { useTitle } from 'react-use';
import ProblemDetails from "../models/ProblemDetails";

const styles = ({ palette, spacing }: Theme) => createStyles({
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

const messages = defineMessages({
  title: {
    id: "users.new.title",
    description: "Title for new users",
    defaultMessage: "Sign Up"
  }
});

const _NewUser: React.FunctionComponent<INewUserProps & WithStyles<typeof styles>> = ({ classes }) => {
  const intl = useIntl();
  const uc = useUserCredentials();

  useTitle(intl.formatMessage(messages.title));
  const handleSubmit = async (event: React.MouseEvent<HTMLFormElement>) => {
    event.preventDefault();
    uc.setEntityError("");

    try {
      const result = await new UsersApi(GetDefaultApiConfig()).apiUsersPost({
        validatedUserCredentialsBindingModel: {
          userName: uc.userName,
          password: uc.password
        }
      });

      const login = await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
        loginBindingModel: {
          userName: uc.userName,
          password: uc.password
        }
      });

      window.location.replace(login.returnUrl || "/");
    } catch (response) {
      const details: ProblemDetails = await response.json();
      const err = details.errors;
      if (!err && details.detail) {
        uc.setEntityError(details.detail);
      } else {
        if (err.hasOwnProperty("userName")) {
          uc.setUserNameError(err["userName"].join("\n"));
        }
        if (err.hasOwnProperty("password")) {
          uc.setPasswordError(err["password"].join("\n"));
        }
      }
    }
  };

  return <Container component="main" maxWidth="xs">
    <CssBaseline />
    <div className={classes.paper}>
      <Avatar className={classes.avatar}>
        <LockOutlinedIcon />
      </Avatar>
      <Typography component="h1" variant="h5">
        {intl.formatMessage(messages.title)}
      </Typography>
      <UserPasswordForm required
        handleSubmit={handleSubmit}
        userCredentials={uc}>
        <Button
          type="submit"
          fullWidth
          variant="contained"
          color="primary"
          className={classes.submit}
        >
          <FormattedMessage
            id="users.new.action"
            description="Action label for the sign up"
            defaultMessage="Sign up"
          />
        </Button>
        <Grid container justify="flex-end">
          <Grid item>
            <Link component={ReachLink} to="/login" variant="body2">
              <FormattedMessage
                id="sessions.new.signup_action"
                description="Action label for redirecting to log in page"
                defaultMessage="Already have an account? Log in"
              />
            </Link>
          </Grid>
        </Grid>
      </UserPasswordForm>
    </div>
  </Container>;
};

const NewUser = withStyles(styles)(_NewUser);

export default NewUser;
