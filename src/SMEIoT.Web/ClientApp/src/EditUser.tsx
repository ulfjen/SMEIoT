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
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import TextField from '@material-ui/core/TextField';
import InputAdornment from "@material-ui/core/InputAdornment";
import IconButton from "@material-ui/core/IconButton";
import { Visibility, VisibilityOff } from "@material-ui/icons";

import {
  Configuration, SessionsApi,
  UsersApi,
  BasicUserApiModel,
} from "smeiot-client";
import {GetDefaultApiConfig} from "./index";
import moment from "moment";
import useUserCredentials from "./components/useUserCredentials";

const styles = ({palette, spacing}: Theme) => createStyles({
  '@global': {
    body: {
      backgroundColor: palette.common.white,
    },
  },
  container: {},
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

export interface IEditUserProps extends WithStyles<typeof styles> {
  csrfToken: string
}

const _EditUser: React.FunctionComponent<IEditUserProps & WithStyles<typeof styles>> = ({csrfToken, classes}) => {
  const {
    username, setUsername,
    password, setPassword,
    usernameErrors, setUsernameErrors,
    passwordErrors, setPasswordErrors
  } = useUserCredentials();

  let currentUser: BasicUserApiModel = {
    createdAt: moment.utc().toISOString(),
    roles: [],
    username: ""
  };

  // @ts-ignore
  if (window.SMEIoTPreRendered) {
    // @ts-ignore
    currentUser = window.SMEIoTPreRendered["currentUser"];
  }

  const handleSubmit = async (event: React.MouseEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (event.target === undefined) {
      return;
    }

    try {
      const result = await new UsersApi(GetDefaultApiConfig()).apiUsersPost({
        validatedUserCredentialsBindingModel: {
          username, password
        }
      });

      const login = await new SessionsApi(GetDefaultApiConfig()).apiSessionsPost({
        loginBindingModel: {
          username, password
        }
      });

      window.location.replace(login.returnUrl || "/");
    } catch (response) {
      const {status, errors} = await response.json();
      if (errors.hasOwnProperty("Username")) {
        setUsernameErrors(errors["Username"].join("\n"));
      }
      if (errors.hasOwnProperty("Password")) {
        setPasswordErrors(errors["Password"].join("\n"));
      }
    }
  };
  const [showPassword, setShowPassword] = React.useState<boolean>(false);

    return <Container component="main" maxWidth="lg">
      <CssBaseline/>
        <Card>
          <CardContent>
            <Typography variant="h5" component="h2">{username}
        </Typography>
            <Typography color="textSecondary">{currentUser.roles}</Typography>
          <Typography>Created at: {currentUser.createdAt}</Typography>

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
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />

          </CardContent>
          <CardActions>
            <Button onClick={() => { window.location.href = "/dashboard"; }}>Cancel</Button>
            <Button color="primary">Edit</Button>
          </CardActions>
        </Card>    </Container>;
};

const EditUser = withStyles(styles)(_EditUser);

export default EditUser;
