import * as React from "react";
import TextField from '@material-ui/core/TextField';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import {FunctionComponent, useContext, useState} from "react";
import InputAdornment from "@material-ui/core/InputAdornment";
import IconButton from "@material-ui/core/IconButton";
import {Visibility, VisibilityOff} from "@material-ui/icons";

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

export interface IUserPasswordFormProps extends WithStyles<typeof styles> {
  url: string | undefined;
  handleSubmit: (event: React.MouseEvent<HTMLFormElement>) => Promise<void> | undefined;
  userName: string;
  setUserName: React.Dispatch<React.SetStateAction<string>>;
  password: string;
  setPassword: React.Dispatch<React.SetStateAction<string>>;
  userNameErrors: string;
  setUserNameErrors: React.Dispatch<React.SetStateAction<string>>;
  passwordErrors: string;
  setPasswordErrors: React.Dispatch<React.SetStateAction<string>>;
}

const _UserPasswordForm: React.FunctionComponent<IUserPasswordFormProps & WithStyles<typeof styles>> = ({classes, url, children, handleSubmit, userName, userNameErrors, password, passwordErrors, setUserName, setPassword, setPasswordErrors, setUserNameErrors}) => {
  const [showPassword, setShowPassword] = useState<boolean>(false);
  
  // @ts-ignore
  const SMEIoTPreRendered = window["SMEIoTPreRendered"];
  if (SMEIoTPreRendered) {
    const model = SMEIoTPreRendered.model;
    const errors = SMEIoTPreRendered.validation_errors;
    if (model) {
      if (model.userName) { setUserName(model.userName); }
    }
  }

  return <form className={classes.form} noValidate method="POST" onSubmit={handleSubmit} action={url}>
    <TextField
      variant="outlined"
      margin="normal"
      required
      fullWidth
      id="userName"
      label="UserName"
      name="userName"
      autoComplete="userName"
      autoFocus
      onChange={(event) => {
        setUserName(event.target.value);
        if (userNameErrors.length > 0) {
          setUserNameErrors("");
        }
      }}
      error={userNameErrors.length > 0}
      helperText={userNameErrors}
    />
    <TextField
      variant="outlined"
      margin="normal"
      required
      fullWidth
      name="password"
      label="Password"
      type={showPassword ? "text" : "password"}
      id="password"
      autoComplete="current-password"
      onChange={(event) => {
        setPassword(event.target.value);
        if (passwordErrors.length > 0) {
          setPasswordErrors("");
        }
      }}
      error={passwordErrors.length > 0}
      helperText={passwordErrors}
      InputProps={{
        endAdornment: (
          <InputAdornment position="end">
            <IconButton
              edge="end"
              aria-label="toggle password visibility"
              onClick={() => {
                setShowPassword(!showPassword)
              }}
            >
              {showPassword ? <VisibilityOff/> : <Visibility/>}
            </IconButton>
          </InputAdornment>
        ),
      }}
    />
    {children}
  </form>

};

const UserPasswordForm = withStyles(styles)(_UserPasswordForm);
export default UserPasswordForm;
