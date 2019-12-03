import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Card from "@material-ui/core/Card";
import CardActionArea from "@material-ui/core/CardActionArea";
import MenuItem from "@material-ui/core/MenuItem";
import CardHeader from "@material-ui/core/CardHeader";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import IconButton from "@material-ui/core/IconButton";
import Button from "@material-ui/core/Button";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import AssessmentIcon from "@material-ui/icons/Assessment";
import NotesIcon from "@material-ui/icons/Notes";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";
import StatusBadge from "../components/StatusBadge";
import useInterval from "../helpers/useInterval";
import { BrokerApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";

const styles = ({ transitions, spacing }: Theme) =>
  createStyles({
    expand: {
      transform: "rotate(0deg)",
      marginLeft: "auto",
      transition: transitions.create("transform", {
        duration: transitions.duration.shortest
      })
    },
    action: {
      padding: spacing(2),
    }
  });

export interface IBasicBrokerCard extends WithStyles<typeof styles> { }

const messages = defineMessages({
  broker: {
    id: "dashboard.broker.title",
    description: "The broker block title on the dashboard page.",
    defaultMessage: "Broker"
  },
  statistics: {
    id: "dashboard.broker.actions.statistics",
    description: "The action for viewing statistics on the broker block.",
    defaultMessage: "Statistics"
  },
  logs: {
    id: "dashboard.broker.actions.logs",
    description: "The action for viewing logs on the broker block.",
    defaultMessage: "Logs"
  },
  config: {
    id: "dashboard.broker.actions.config",
    description: "The action for editing config file on the broker block.",
    defaultMessage: "Config"
  },
  reload: {
    id: "dashboard.broker.actions.reload",
    description: "The action for reloading config file on the broker block.",
    defaultMessage: "Reload"
  },
  restart: {
    id: "dashboard.broker.actions.restart",
    description: "The action for restarting config file on the broker block.",
    defaultMessage: "Restart"
  },
  more: {
    id: "dashboard.broker.actions.more",
    description: "The action label for showing menu.",
    defaultMessage: "More"
  }
});

interface BasicBrokerStatistics {
  [receivedMessages: string]: string;
}

const _BasicBrokerCard: React.FunctionComponent<IBasicBrokerCard> = ({ classes }) => {
  const intl = useIntl();

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [running, setRunning] = React.useState<boolean>(false);
  const [statistics, setStatistics] = React.useState<BasicBrokerStatistics>({
  });
  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const api = new BrokerApi(GetDefaultApiConfig());
  const updateBroker = async () => {
    let details = await api.apiBrokerBasicGet();
    if (details === null) { return; }
    setRunning(details.running);
    setStatistics({
      receivedMessages: ""
    });
  }

  useInterval(updateBroker, 10000);
  React.useEffect(() => { updateBroker() }, []);

  return (
    <Card>
      <CardActionArea className={classes.action}>
        {intl.formatMessage(messages.broker)}
        <StatusBadge status={running ? "running" : "stopped"}/>
      </CardActionArea>
    </Card>
  );
};

const BasicBrokerCard = withStyles(styles)(_BasicBrokerCard);

export default BasicBrokerCard;
