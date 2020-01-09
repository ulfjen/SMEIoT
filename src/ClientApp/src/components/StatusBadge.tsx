import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { lighten } from "@material-ui/core/styles";

const RADIUS_STANDARD = 7;

const styles = ({ palette, typography, transitions }: Theme) => createStyles({
  root: {
    display: 'flex',
    alignItems: "center",
    paddingTop: 5,
    paddingBottom: 5
  },
  content: {
    paddingLeft: RADIUS_STANDARD * 0.5,
    lineHeight: 1.5,
    fontSize: typography.body2.fontSize,
    letterSpacing: typography.body2.letterSpacing,
  },
  status: {
    boxSizing: "border-box",
    minWidth: RADIUS_STANDARD * 2,
    height: RADIUS_STANDARD * 2,
    borderRadius: RADIUS_STANDARD,
    transition: transitions.create('transform', {
      easing: transitions.easing.easeInOut,
      duration: transitions.duration.enteringScreen,
    }),
  },
  success: {
    color: lighten(palette.text.primary, 0.1),
  },
  successStatus: {
    backgroundColor: lighten(palette.success.main, 0.3)
  },
  info: {
    color: lighten(palette.text.primary, 0.1),
  },
  infoStatus: {
    backgroundColor: lighten(palette.info.main, 0.3),
  },
  warning: {
    color: lighten(palette.text.secondary, 0.2),
  },
  warningStatus: {
    backgroundColor: lighten(palette.warning.main, 0.3),
  },
  error: {
    color: lighten(palette.text.secondary, 0.2),
  },
  errorStatus: {
    backgroundColor: lighten(palette.error.main, 0.3),
  }
});

type SeverityType = 'error' | 'info' | 'success' | 'warning';
type SeverityStatusType = 'errorStatus' | 'infoStatus' | 'successStatus' | 'warningStatus';
const STATUS_MAP: Record<SeverityType, SeverityStatusType> = {
  'error': 'errorStatus',
  'info': 'infoStatus',
  'success': 'successStatus',
  'warning': 'warningStatus'
}

export interface IStatusBadgeProps extends WithStyles<typeof styles> {
  severity: SeverityType;
  className?: string;
  badge?: React.ReactNode;
}

const _StatusBadge: React.FunctionComponent<IStatusBadgeProps> = ({ classes, children, className, severity, badge }) => {
  const severityStatusMapped = STATUS_MAP[severity];

  return <div className={clsx(classes.root, className)}>
    <span
      className={badge ? classes.status : clsx(classes.status, classes[severityStatusMapped])}
    >
      {badge}
    </span>

    <span className={clsx(classes.content, classes[severity])}>{children}</span>
  </div>;
};

const StatusBadge = withStyles(styles)(_StatusBadge);
export default StatusBadge;
