import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import clsx from "clsx";

const RADIUS_STANDARD = 7;

const styles = ({ typography, transitions }: Theme) => createStyles({
  root: {
    position: "relative"
  },
  content: {
    paddingLeft: RADIUS_STANDARD * 0.5,
    display: "inline-box",
    lineHeight: 1,
    fontSize: typography.pxToRem(14),
    verticalAlign: "middle"
  },
  status: {
    display: "inline-box",
    boxSizing: "border-box",
    minWidth: RADIUS_STANDARD * 2,
    height: RADIUS_STANDARD * 2,
    borderRadius: RADIUS_STANDARD,
    verticalAlign: "middle",
    transition: transitions.create('transform', {
      easing: transitions.easing.easeInOut,
      duration: transitions.duration.enteringScreen,
    }),
  },
  normalColor: {
    backgroundColor: "green"
  },
  errorColor: {
    backgroundColor: "red"
  }
});

export interface IStatusBadgeProps extends WithStyles<typeof styles> {
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
  color?: "error" | "normal" | null;
  className?: string;
  badge?: React.ReactNode;
}

const _StatusBadge: React.FunctionComponent<IStatusBadgeProps> = ({ classes, children, className, color, badge }) => {
  let colorClass = "";
  switch (color) {
    case "normal":
      colorClass = classes.normalColor;
      break;
    case "error":
      colorClass = classes.errorColor;
    default:
      break;
  }

  return (
    <div className={clsx(classes.root, className)}>
      <span
        className={clsx(classes.status, colorClass)}
      >
        {badge}
      </span>
      <span className={classes.content}>{children}</span>
    </div>
  );
};

const StatusBadge = withStyles(styles)(_StatusBadge);
export default StatusBadge;
