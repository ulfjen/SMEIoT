import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Avatar from "@material-ui/core/Avatar";
import Typography from "@material-ui/core/Typography";

const styles = ({ palette }: Theme) => createStyles({
  root: {
    padding: 16,
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
  statusWithAction: {
    marginRight: 10
  },
  action: {
    marginLeft: "auto",
    marginTop: -6,
    marginRight: -12,
  },
  avatar: {
    marginBottom: 5,
  },
  headBottomBorder: {
    borderBottom: `2px solid ${palette.divider}`
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
  let head: React.ReactNode | undefined;
  let content: React.ReactNode | undefined;

  if (typeof title === "string") {
    title = <Typography color="primary" component="h2" variant="h5">
      {title}
    </Typography>
  }

  if (status) {
    const stautsClass = action ? clsx(classes.status, classes.statusWithAction) : classes.status;
    status = <div className={stautsClass}>{status}</div>;
  }

  let pushTitleToHead = false;
  if (avatar) {
    avatar = <Avatar className={classes.avatar}>
      {avatar}
    </Avatar>
  } else if (action) { // avoid blank under the title if no avatar with action
    pushTitleToHead = true;
  }

  if (pushTitleToHead) {
    head = title;
    className = className ? clsx(className, classes.headBottomBorder) : classes.headBottomBorder;
  } else {
    head = avatar;
    content = title;
  }
  head = <div className={classes.head}>
    {head}
    {action && <div className={classes.action}>{action}</div>}
  </div>;

  if (content || status) {
    content = <div className={classes.content}>
      {content}
      {status}
    </div>
  }

  return <div className={clsx(classes.root, className)}>
    {head}
    {content}
  </div>;
};

const ExpandedCardHeader = withStyles(styles)(_ExpandedCardHeader);

export default ExpandedCardHeader;
