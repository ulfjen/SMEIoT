import * as React from "react";
import IconButton from "@material-ui/core/IconButton";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import CircularProgress from "@material-ui/core/CircularProgress";

const styles = ({ palette, spacing }: Theme) => createStyles({
  progress: {
    position: "absolute",
    top: 0,
    left: 0,
    zIndex: 1
  },
  wrapper: {
    display: "inline-block",
    position: "relative"
  }
});

export interface IProgressIconButtonProps extends WithStyles<typeof styles> {
  children: JSX.Element;
  onClick: React.MouseEventHandler<HTMLButtonElement>;
  loading: boolean;
  ariaLabel?: string;
  disabled?: boolean;
  variant?: "text" | "outlined" | "contained";
  color?: "default" | "inherit" | "primary" | "secondary";
}

const _ProgressIconButton = React.forwardRef(({
  classes,
  loading,
  children,
  onClick,
  variant,
  disabled,
  ariaLabel,
  color
}: IProgressIconButtonProps, ref: React.Ref<HTMLDivElement>) => {
  return (
    <div className={classes.wrapper} ref={ref}>
      <IconButton
        color={color}
        aria-label={ariaLabel}
        disabled={loading || disabled}
        onClick={onClick}
      >
        {children}
      </IconButton>
      {loading && <CircularProgress size={48} className={classes.progress} />}
    </div>
  );
});
 
const ProgressIconButton = withStyles(styles)(_ProgressIconButton);
export default ProgressIconButton;
