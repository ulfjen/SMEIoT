import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Avatar from "@material-ui/core/Avatar";
import Typography from "@material-ui/core/Typography";

const styles = ({ transitions, spacing }: Theme) => createStyles({
  root: {
    display: 'flex',
    flexDirection: "column"
  },
  head: {
    display: 'flex',
  },
  content: {
    display: 'flex',
  },
  status: {
    marginLeft: "auto",
  },
  action: {
    marginLeft: "auto",
  },
  avatar: {
    marginBottom: 5,
  }
});

export interface IExpandedCardHeader extends WithStyles<typeof styles> {
  className?: string;
  action?: React.ReactNode;
  status?: React.ReactNode;
  avatar?: React.ReactNode;
  title: React.ReactNode | string;
}

const _ExpandedCardHeader: React.FunctionComponent<IExpandedCardHeader> = ({ classes, className, title, action, avatar, status }) => {
  return <div className={clsx(classes.root, className)}>
    <div className={classes.head}>
      {avatar && <Avatar className={classes.avatar}>
        {avatar}
      </Avatar>}
      {action && <div className={classes.action}>{action}</div>}
    </div>
    <div className={classes.content}>
      <Typography component="span" variant="h5" display="block">
        {title}
      </Typography>
      {status && <div className={classes.status}>{status}</div>}
    </div>
  </div>;
};

const ExpandedCardHeader = withStyles(styles)(_ExpandedCardHeader);

export default ExpandedCardHeader;
