import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import TextField from "@material-ui/core/TextField";
import Divider from "@material-ui/core/Divider";
import CircularProgress from "@material-ui/core/CircularProgress";
import InputAdornment from "@material-ui/core/InputAdornment";
import ProgressButton from "./ProgressButton";
import { FormattedMessage } from "react-intl";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    divider: {
      height: 36,
      margin: 4
    },
    buttonProgress: {
      position: "absolute",
      top: "50%",
      left: "50%",
      marginTop: -12,
      marginLeft: -12
    },
    wrapper: {
      position: "relative"
    }
  });

export interface ISuggestTextFieldProps extends WithStyles<typeof styles> {
  label: string;
  value?: string | null | undefined;
  onChange: React.ChangeEventHandler<HTMLInputElement | HTMLTextAreaElement>;
  onSuggest: React.MouseEventHandler<HTMLButtonElement>;
  suggesting: boolean;
  autoFocus?: boolean;
}

const _SuggestTextField: React.FunctionComponent<ISuggestTextFieldProps &
  WithStyles<typeof styles>> = ({
  classes,
  label,
  value,
  suggesting,
  onChange,
  onSuggest,
  autoFocus
}) => {
  return (
    <TextField
      variant="outlined"
      margin="normal"
      required
      fullWidth
      autoFocus={autoFocus}
      label={label}
      value={value}
      onChange={onChange}
      InputProps={{
        endAdornment: (
          <InputAdornment position="end">
            <Divider className={classes.divider} orientation="vertical" />
            <ProgressButton onClick={onSuggest} loading={suggesting}>
              <FormattedMessage
                id="dashboard.components.suggest"
                description="Suggest button on the right of the text field."
                defaultMessage="Suggest"
              />
            </ProgressButton>
          </InputAdornment>
        )
      }}
    />
  );
};

const SuggestTextField = withStyles(styles)(_SuggestTextField);
export default SuggestTextField;
