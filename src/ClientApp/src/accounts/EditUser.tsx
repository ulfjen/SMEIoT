import * as React from "react";
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { withStyles, lighten, darken } from '@material-ui/core/styles';
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import Container from '@material-ui/core/Container';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import BuildIcon from '@material-ui/icons/Build';
import DoneOutlineOutlinedIcon from '@material-ui/icons/DoneOutlineOutlined';
import CloseIcon from '@material-ui/icons/Close';
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
import Avatar from "@material-ui/core/Avatar";
import Snackbar from "@material-ui/core/Snackbar";
import Slide from "@material-ui/core/Slide";
import useInterval from "../helpers/useInterval";
import { TransitionProps } from "@material-ui/core/transitions";
import Paper from "@material-ui/core/Paper";
import IconButton from "@material-ui/core/IconButton";

const styles = ({ palette, spacing, typography, shape }: Theme) => createStyles({
  page: {
    marginTop: spacing(3)
  },
  label: {
    paddingTop: 3
  },
  content: {
    paddingTop: 0
  },
  snackbar: {
    color: darken(palette.success.main, 0.6),
    backgroundColor: lighten(palette.success.main, 0.9),
    '& $icon': {
      color: palette.success.main,
    }
  },
  snackbarPaper: {
    ...typography.body2,
    borderRadius: shape.borderRadius,
    backgroundColor: 'transparent',
    display: 'flex',
    padding: 2,
  },
  snackbarMessage: {
    padding: '2px 0',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
  },
  snackbarIcon: {
    marginRight: 12,
    padding: '7px 0',
    display: 'flex',
    fontSize: 22,
    opacity: 0.9,
  },
  snackbarAction: {
    display: 'flex',
    alignItems: 'center',
    marginLeft: 'auto',
    paddingLeft: 16,
    marginRight: -8,
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
  },
  closeSnackbar: {
    id: "users.edit.snackbar.close",
    description: "Close success message",
    defaultMessage: "Close"
  }
});

function TransitionUp(props: TransitionProps) {
  return <Slide {...props} direction="up" />;
}

const _EditUser: React.FunctionComponent<IEditUserProps> = ({ classes, navigate }) => {
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

  const [successBarOpen, setSuccessBarOpen] = React.useState<boolean>(false);

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

      setSuccessBarOpen(true);
      uc.setUserName("");
      uc.setPassword("");
      uc.setUserNameError("");
      uc.setPasswordError("");
      uc.setEntityError("");
      setNewPassword("");
      setNewPasswordError("");
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
  const handleClose = () => {
    setSuccessBarOpen(false);
  }

  const roles = ((state.value && state.value.roles) || []).join(", ");

  return <Container component="main" maxWidth="lg" className={classes.page}>
    <CssBaseline />
    <ErrorBoundary>
      <Card>
        <CardHeader
          avatar={
            <Avatar>
              <BuildIcon />
            </Avatar>}
          title={<Typography variant="h5" color="primary">{intl.formatMessage(messages.title)}</Typography>}
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
              id="users.edit.action.back"
              description="Cancel action in the user profile page"
              defaultMessage="Back"
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
      <Snackbar
        className={classes.snackbar}
        open={successBarOpen}
        anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
        autoHideDuration={3000}
        onClose={handleClose}
        TransitionComponent={TransitionUp}
        ContentProps={{ className: classes.snackbar }}
        message={
          <Paper
            square
            elevation={0}
            className={classes.snackbarPaper}
          >
            <div className={classes.snackbarIcon}>
              <DoneOutlineOutlinedIcon fontSize="inherit" />
            </div>
            <div className={classes.snackbarMessage}>
              <FormattedMessage
                id="users.edit.action.success_message"
                description="Success snackbar showed when editing is successful"
                defaultMessage="Successfully updated your password."
              />
            </div>
            <div className={classes.snackbarAction}>
              <IconButton
                size="small"
                aria-label={intl.formatMessage(messages.closeSnackbar)}
                title={intl.formatMessage(messages.closeSnackbar)}
                color="inherit"
                onClick={handleClose}
              >
                <CloseIcon fontSize="small" />
              </IconButton>
            </div>
          </Paper>}
      />
    </ErrorBoundary>
  </Container>;
};

const EditUser = withStyles(styles)(_EditUser);

export default EditUser;
