import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import clsx from "clsx";

const RADIUS_STANDARD = 7;

const styles = ({ palette, typography, transitions, spacing }: Theme) =>
  createStyles({
    root: {
      position: "relative"
    },
    content: {
      paddingLeft: RADIUS_STANDARD*0.5,
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
    statusRunning: {
      backgroundColor: "green"
    },
    statusStopped: {
      backgroundColor: "red"
    }
  });

export interface IStatusBadgeProps extends WithStyles<typeof styles> {
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
  status: "running" | "stopped";
  className?: string;
}

const messages = defineMessages({
  running: {
    id: "components.status.running",
    description: "Used as in status badge",
    defaultMessage: "Running"
  },
  stopped: {
    id: "components.status.stopped",
    description: "Used as in status badge",
    defaultMessage: "Stopped"
  },
});

const _StatusBadge: React.FunctionComponent<IStatusBadgeProps &
  WithStyles<typeof styles>> = ({
    classes,
    status,
    className
  }) => {
    const intl = useIntl();

    return (
      <div className={clsx(classes.root, className)}>
        <span
          className={clsx(classes.status, status === "running" ? classes.statusRunning : classes.statusStopped)}
        >
        </span>
        <span className={classes.content}>{status === "running" ? intl.formatMessage(messages.running) : intl.formatMessage(messages.stopped)}</span>
      </div>
    );
  };

const StatusBadge = withStyles(styles)(_StatusBadge);
export default StatusBadge;
