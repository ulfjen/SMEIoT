import * as React from "react";
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import Link from '@material-ui/core/Link';
import Grid from '@material-ui/core/Grid';
import PersonOutlineIcon from '@material-ui/icons/PersonOutline';
import Typography from '@material-ui/core/Typography';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Container from '@material-ui/core/Container';
import { Link as ReachLink, RouteComponentProps } from "@reach/router";
import UserPasswordForm from "../components/UserPasswordForm";
import useUserCredentials from "../helpers/useUserCredentials";
import { SessionsApi, UsersApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { FormattedMessage, defineMessages, useIntl } from "react-intl";
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
    backgroundColor: palette.success.light,
  },
  submit: {
    margin: spacing(3, 0, 2),
    backgroundColor: palette.success.light,
    "&:hover": {
      backgroundColor: palette.success.main,
    }
  },
  backdrop: {
    zIndex: zIndex.drawer + 1,
    color: '#fff',
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

const _NewUser: React.FunctionComponent<INewUserProps & WithStyles<typeof styles>> = ({ classes, navigate }) => {
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
    
    await new UsersApi(GetDefaultApiConfig()).apiUsersPost({
      validatedUserCredentialsBindingModel: {
        userName: uc.userName,
        password: uc.password
      }
    }).then(async res => {
      await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
        loginBindingModel: {
          userName: uc.userName,
          password: uc.password
        }
      }).then(res => {
        navigate && navigate(res.returnUrl || "/");
        return res;
      });

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
  }, [uc, navigate]);

  return <Container component="main" maxWidth="xs">
    <CssBaseline />
    <ErrorBoundary>
      <Backdrop className={classes.backdrop} open={loading}>
        <CircularProgress color="inherit" />
      </Backdrop>
      <div className={classes.paper}>
        <Avatar className={classes.avatar}>
          <PersonOutlineIcon />
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
              <Link color="textPrimary" component={ReachLink} to="/login" variant="body2">
                <FormattedMessage
                  id="users.new.login_action"
                  description="Action label for redirecting to log in page"
                  defaultMessage="Already have an account? Log in"
                />
              </Link>
            </Grid>
          </Grid>
        </UserPasswordForm>
      </div>
    </ErrorBoundary>
  </Container>;
};

const NewUser = withStyles(styles)(_NewUser);

export default NewUser;
