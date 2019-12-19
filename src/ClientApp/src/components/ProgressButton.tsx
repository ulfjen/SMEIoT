import * as React from "react";
import Button from "@material-ui/core/Button";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import CircularProgress from "@material-ui/core/CircularProgress";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    progress: {
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

export interface IProgressButtonProps extends WithStyles<typeof styles> {
  children: JSX.Element;
  onClick: React.MouseEventHandler<HTMLButtonElement>;
  loading: boolean;
  variant?: "text" | "outlined" | "contained";
  color?: "default" | "inherit" | "primary" | "secondary";
}

const _ProgressButton: React.FunctionComponent<IProgressButtonProps &
  WithStyles<typeof styles>> = ({
  classes,
  loading,
  children,
  onClick,
  variant,
  color
}) => {
  return (
    <div className={classes.wrapper}>
      <Button variant={variant} onClick={onClick} disabled={loading} color={color}>
        {children}
      </Button>
      {loading && <CircularProgress size={24} className={classes.progress} />}
    </div>
  );
};

const ProgressButton = withStyles(styles)(_ProgressButton);
export default ProgressButton;
