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
import { SessionsApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import useUserCredentials from "../helpers/useUserCredentials";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { useTitle } from 'react-use';
import ValidationProblemDetails from "../models/ValidationProblemDetails";
import ErrorBoundary from "../components/ErrorBoundary";
import Backdrop from "@material-ui/core/Backdrop";
import CircularProgress from "@material-ui/core/CircularProgress";

const styles = ({ palette, spacing, zIndex }: Theme) => createStyles({
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
  submit: {
    margin: spacing(3, 0, 2),
  },
  backdrop: {
    zIndex: zIndex.drawer + 1,
    color: '#fff',
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

const _NewSession: React.FunctionComponent<INewSessionProps & WithStyles<typeof styles>> = ({ classes, navigate }) => {
  const intl = useIntl();
  const uc = useUserCredentials();

  useTitle(intl.formatMessage(messages.title));

  const [loading, setLoading] = React.useState(false);

  const handleSubmit = React.useCallback(async (event: React.MouseEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (event.target === undefined) {
      return;
    }
    uc.setEntityError("");
    setLoading(true);

    await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
      loginBindingModel: {
        userName: uc.userName,
        password: uc.password
      }
    }).then(res => {
      navigate && navigate(res.returnUrl || "/");
      return res;
    }).catch(async response => {
      const details: ValidationProblemDetails = await response.json();
      if (details.detail) {
        uc.setEntityError(details.detail);
      }
      const err = details.errors;
      if (err) {
        if (err.hasOwnProperty("userName")) {
          uc.setUserNameError(err["userName"].join("\n"));
        }
        if (err.hasOwnProperty("password")) {
          uc.setPasswordError(err["password"].join("\n"));
        }
      }
    }).finally(() => {
      setLoading(false);
    });
  }, [uc]);

  return <Container component="main" maxWidth="xs">
    <CssBaseline />
    <ErrorBoundary>
      <Backdrop className={classes.backdrop} open={loading}>
        <CircularProgress color="inherit" />
      </Backdrop>
      <div className={classes.paper}>
        <Avatar className={classes.avatar}>
          <LockOutlinedIcon />
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
              <Link color="textPrimary" component={ReachLink} to="/signup" variant="body2">
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
    </ErrorBoundary>
  </Container>;
};

const NewSession = withStyles(styles)(_NewSession);
export default NewSession;
