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

export interface ILoginFormProps extends WithStyles<typeof styles> {
  csrfToken: string;
  url: string;
  username: string | undefined;
  errorPrompt: string | undefined;
}

const _LoginForm: React.FunctionComponent<ILoginFormProps & WithStyles<typeof styles>> = ({csrfToken, classes, url, username, errorPrompt, children}) => {
  const [trackedUsername, setUsername] = useState<string>(username || "");
  const [showPassword, setShowPassword] = useState<boolean>(false);
  
  return <form className={classes.form} noValidate method="POST" action={url}>
    <input type="hidden" name="__RequestVerificationToken" value={csrfToken}/>
    <TextField
      variant="outlined"
      margin="normal"
      required
      fullWidth
      id="username"
      label="Username"
      name="username"
      autoComplete="username"
      value={trackedUsername}
      onChange={(event: React.ChangeEvent<HTMLInputElement>) => setUsername(event.target.value)}
      autoFocus
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

const LoginForm = withStyles(styles)(_LoginForm);
export default LoginForm;
