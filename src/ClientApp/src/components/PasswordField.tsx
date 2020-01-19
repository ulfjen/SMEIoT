import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import TextField from '@material-ui/core/TextField';
import InputAdornment from "@material-ui/core/InputAdornment";
import IconButton from "@material-ui/core/IconButton";
import Visibility from "@material-ui/icons/Visibility";
import VisibilityOff from "@material-ui/icons/VisibilityOff";

const styles = ({ palette, spacing }: Theme) => createStyles({
  '@global': {
  }
});

export interface IPasswordFieldProps extends WithStyles<typeof styles> {
  label: string;
  setPassword: React.Dispatch<React.SetStateAction<string>>;
  errors: string;
  setErrors: React.Dispatch<React.SetStateAction<string>>;
}

const _PasswordField: React.FunctionComponent<IPasswordFieldProps & WithStyles<typeof styles>> = ({ label, errors, setPassword, setErrors }) => {
  const [showPassword, setShowPassword] = React.useState<boolean>(false);

  return <TextField
    variant="outlined"
    margin="normal"
    required
    fullWidth
    name={label.toLowerCase().replace(" ", "-")}
    label={label}
    type={showPassword ? "text" : "password"}
    onChange={(event) => {
      setPassword(event.target.value);
      if (errors.length > 0) {
        setErrors("");
      }
    }}
    error={errors.length > 0}
    helperText={errors}
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
            {showPassword ? <VisibilityOff /> : <Visibility />}
          </IconButton>
        </InputAdornment>
      ),
    }}
  />
};

const PasswordField = withStyles(styles)(_PasswordField);
export default PasswordField;

