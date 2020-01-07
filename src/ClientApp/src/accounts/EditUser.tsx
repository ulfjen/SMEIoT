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
import { UsersApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import moment from "moment";
import useUserCredentials from "../helpers/useUserCredentials";
import { RouteComponentProps } from "@reach/router";
import { useIntl, defineMessages, FormattedMessage } from "react-intl";
import { useTitle, useAsync } from "react-use";
import { useAppCookie } from "../helpers/useCookie";
import ValidationProblemDetails from "../models/ValidationProblemDetails";
import Skeleton from "@material-ui/lab/Skeleton";
import ErrorBoundary from "../components/ErrorBoundary";

const styles = ({ palette, spacing }: Theme) => createStyles({
  page: {
    marginTop: spacing(3)
  },
  label: {
    paddingTop: 3
  },
  content: {
    paddingTop: 0
  }
});

export interface IEditUserProps extends RouteComponentProps, WithStyles<typeof styles> {
}

const messages = defineMessages({
  title: {
    id: "users.edit.title",
    description: "Title for editusers",
    defaultMessage: "Profile"
  },
  password: {
    id: "users.edit.password_label",
    description: "Password label",
    defaultMessage: "Password"
  },
  newPassword: {
    id: "users.edit.newpassword_label",
    description: "New password label",
    defaultMessage: "New password"
  }
});

const _EditUser: React.FunctionComponent<IEditUserProps & WithStyles<typeof styles>> = ({ classes, navigate }) => {
  const intl = useIntl();
  const uc = useUserCredentials();

  useTitle(intl.formatMessage(messages.title));
  const appCookie = useAppCookie();

  const [newPassword, setNewPassword] = React.useState<string>("");
  const [newPasswordError, setNewPasswordError] = React.useState<string>("");

  const state = useAsync(async () => {
    return await new UsersApi(GetDefaultApiConfig()).apiUsersUserNameGet({
      userName: appCookie.userName || ""
    });
  }, [appCookie.userName]);

  const handleEdit = async (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    event.preventDefault();
    if (event.target === undefined) {
      return;
    }
    uc.setEntityError("");

    if (!state.value || state.value.userName === undefined) {
      throw new Error("user can not be found. Try to clean the cookie and refresh.");
    }
    try {
      const result = await new UsersApi(GetDefaultApiConfig()).apiUsersUserNamePasswordPut({
        userName: state.value.userName,
        confirmedUserCredentialsUpdateBindingModel: {
          currentPassword: uc.password,
          newPassword: newPassword
        }
      });

      navigate && navigate("/dashboard");
    } catch (response) {
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
        if (err.hasOwnProperty("newPassword")) {
          setNewPasswordError(err["newPassword"].join("\n"));
        }
      }
    }
  };

  const roles = ((state.value && state.value.roles) || []).join(", ");

  return <Container component="main" maxWidth="lg" className={classes.page}>
    <CssBaseline />
    <ErrorBoundary>
      <Card>
        <CardHeader
          avatar={
            <BuildIcon />
          }
          title={intl.formatMessage(messages.title)}
        />
        <CardContent className={classes.content}>
          <div className={classes.label}>
            {state.loading ?
              <Skeleton variant="rect" width={160} height={30} /> :
              <Typography variant="h5">
                {state.value && state.value.userName}
              </Typography>}
          </div>
          <div className={classes.label}>
            {state.loading ? <Skeleton variant="rect" width={80} height={20} /> : <Typography color="textSecondary">{roles}</Typography>}
          </div>
          <div className={classes.label}>
            {state.loading ?
              <Skeleton variant="rect" width={480} height={20} /> :
              <Typography>
                <FormattedMessage
                  id="users.edit.created_at"
                  description="Account creation time message"
                  defaultMessage="Your account is created at {time}."
                  values={{
                    time: state.value && moment(state.value.createdAt).format("LLLL")
                  }}
                />
              </Typography>}
          </div>

          <PasswordField
            label={intl.formatMessage(messages.password)}
            setPassword={uc.setPassword}
            errors={uc.passwordError}
            setErrors={uc.setPasswordError} />
          <PasswordField
            label={intl.formatMessage(messages.newPassword)}
            setPassword={setNewPassword}
            errors={newPasswordError}
            setErrors={setNewPasswordError} />
        </CardContent>
        <CardActions>
          <Button onClick={() => { navigate && navigate("/dashboard") }}>
            <FormattedMessage
              id="users.edit.action.cancel"
              description="Cancel action in the user profile page"
              defaultMessage="Cancel"
            />
          </Button>
          <Button color="primary" onClick={handleEdit}>
            <FormattedMessage
              id="users.edit.action.edit"
              description="Edit action in the user profile page"
              defaultMessage="Edit"
            />
          </Button>
        </CardActions>
      </Card>
    </ErrorBoundary>
  </Container>;
};

const EditUser = withStyles(styles)(_EditUser);

export default EditUser;
