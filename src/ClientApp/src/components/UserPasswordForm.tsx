import * as React from "react";
import TextField from '@material-ui/core/TextField';
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { useState } from "react";
import InputAdornment from "@material-ui/core/InputAdornment";
import IconButton from "@material-ui/core/IconButton";
import FormHelperText from '@material-ui/core/FormHelperText';
import { Visibility, VisibilityOff } from "@material-ui/icons";
import { UserCredentials } from "../helpers/useUserCredentials";
import { defineMessages, useIntl } from "react-intl";
import FormControl from "@material-ui/core/FormControl";

const styles = ({spacing}: Theme) => createStyles({
  form: {
    width: '100%', // Fix IE 11 issue.
    marginTop: spacing(3),
  }
});

interface IUserPasswordFormProps extends WithStyles<typeof styles> {
  handleSubmit: (event: React.MouseEvent<HTMLFormElement>) => Promise<void> | undefined;
  required?: boolean;
  userCredentials: UserCredentials;
}

const messages = defineMessages({
  username: {
    id: "components.userpasswordform.username_label",
    description: "UserName label in the form",
    defaultMessage: "Username"
  },
  password: {
    id: "components.userpasswordform.password_label",
    description: "Password label in the form",
    defaultMessage: "Password"
  }
});

const _UserPasswordForm: React.FunctionComponent<IUserPasswordFormProps & WithStyles<typeof styles>> = ({classes, children, handleSubmit, userCredentials, required}) => {
  const intl = useIntl();
  const [showPassword, setShowPassword] = useState<boolean>(false);

  const userNameChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    userCredentials.setUserName(event.target.value);
    userCredentials.setUserNameError("");
  };
  
  const passwordChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    userCredentials.setPassword(event.target.value);
    userCredentials.setPasswordError("");
  };
  
  return <form className={classes.form} noValidate method="POST" onSubmit={handleSubmit}>
    <FormControl error={true}>
      <FormHelperText>{userCredentials.entityError}</FormHelperText>
    </FormControl>
    <TextField
      variant="outlined"
      margin="normal"
      required={required}
      fullWidth
      label={intl.formatMessage(messages.username)}
      autoComplete="userName"
      autoFocus
      onChange={userNameChanged}
      error={userCredentials.userNameError.length > 0}
      helperText={userCredentials.userNameError}
    />
    <TextField
      variant="outlined"
      margin="normal"
      required={required}
      fullWidth
      label={intl.formatMessage(messages.password)}
      type={showPassword ? "text" : "password"}
      autoComplete="password"
      onChange={passwordChanged}
      error={userCredentials.passwordError.length > 0}
      helperText={userCredentials.passwordError}
      InputProps={{
        endAdornment: (
          <InputAdornment position="end">
            <IconButton
              edge="end"
              aria-label="toggle password visibility"
              onClick={() => setShowPassword(!showPassword)}
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
