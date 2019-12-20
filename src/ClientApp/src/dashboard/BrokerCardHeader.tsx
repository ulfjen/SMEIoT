import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Avatar from "@material-ui/core/Avatar";
import Typography from "@material-ui/core/Typography";
import { FormattedMessage } from "react-intl";
import StatusBadge from "../components/StatusBadge";
import { ReactComponent as Broker } from "../images/broker.svg";

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
  media: {
    height: "60%",
    filter: "brightness(0) invert(1)"
  },
  status: {
    marginLeft: "auto",
  },
  action: {
    marginLeft: "auto",
  }
});

export interface IBrokerCardHeader extends WithStyles<typeof styles> {
  className?: string;
  action?: React.ReactNode;
  status?: React.ReactNode;
}

const _BrokerCardHeader: React.FunctionComponent<IBrokerCardHeader> = ({ classes, className, action, status }) => {
  return <div className={clsx(classes.root, className)}>
    <div className={classes.head}>
      <Avatar>
        <Broker className={classes.media} />
      </Avatar>
      {action && <div className={classes.action}>{action}</div>}
    </div>
    <div className={classes.content}>
      <Typography component="span" variant="h5" display="block">
        <FormattedMessage
          id="dashboard.components.basic_broker_card.title"
          description="Title on the broker card"
          defaultMessage="Broker"
        />
      </Typography>
      {status && <div className={classes.status}>{status}</div>}
    </div>
  </div>;
};

const BrokerCardHeader = withStyles(styles)(_BrokerCardHeader);

export default BrokerCardHeader;
