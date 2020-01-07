import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import clsx from "clsx";
import green from '@material-ui/core/colors/green';
import red from '@material-ui/core/colors/red';
import Typography from "@material-ui/core/Typography";

const RADIUS_STANDARD = 7;

const styles = ({ palette, typography, transitions }: Theme) => createStyles({
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
    color: "#333333"
  },
  normalBgColor: {
    backgroundColor: green[800]
  },
  errorColor: {
    color: "#666666"
  },
  errorBgColor: {
    backgroundColor: red[500]
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
  let colorBgClass = "";

  switch (color) {
    case "normal":
      colorClass = classes.normalColor;
      colorBgClass = classes.normalBgColor;
      break;
    case "error":
      colorClass = classes.errorColor;
      colorBgClass = classes.errorBgColor;
    default:
      break;
  }

  return (
    <div className={clsx(classes.root, className)}>
      <span
        className={clsx(classes.status, colorBgClass)}
      >
        {badge}
      </span>
      <span className={clsx(classes.content, colorClass)}>{children}</span>
    </div>
  );
};

const StatusBadge = withStyles(styles)(_StatusBadge);
export default StatusBadge;
