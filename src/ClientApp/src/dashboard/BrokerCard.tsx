import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import Menu from "@material-ui/core/Menu";
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
import BrokerCardHeader from "./BrokerCardHeader";
import StatusBadge from "../components/StatusBadge";
import useInterval from "../helpers/useInterval";
import { BrokerApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";

const styles = ({ spacing, transitions }: Theme) => createStyles({
  expand: {
    transform: "rotate(0deg)",
    marginLeft: "auto",
    transition: transitions.create("transform", {
      duration: transitions.duration.shortest
    })
  },
  header: {
    padding: 16
  },
});

export interface IBrokerCard extends WithStyles<typeof styles> { }

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

const _BrokerCard: React.FunctionComponent<IBrokerCard> = ({ classes }) => {
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
    setRunning(details.running || false);
    setStatistics({
      receivedMessages: ""
    });
  }

  useInterval(updateBroker, 10000);

  return (
    <Card>
      <BrokerCardHeader 
        className={classes.header}
        running={running}
        action={
          <IconButton
            aria-label={intl.formatMessage(messages.more)}
            onClick={handleClick}
          >
            <MoreVertIcon />
          </IconButton>
        }
      />
      <CardContent>
        {/* <Typography variant="body2" color="textSecondary" component="p"> */}
        <p>Received Messages {statistics.receivedMessages}</p>
        {/* </Typography> */}
      </CardContent>
      <CardActions disableSpacing>
        <Button
          size="small"
          component={ReachLink}
          to="/dashboard/broker/statistics"
        >
          {intl.formatMessage(messages.statistics)}
        </Button>
        <Button size="small" component={ReachLink} to="/dashboard/broker/logs">
          {intl.formatMessage(messages.logs)}
        </Button>
      </CardActions>
      <Menu
        anchorEl={anchorEl}
        keepMounted
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
        <MenuItem
          button
          to="/dashboard/broker/config"
          component={ReachLink}
          onClick={handleClose}
        >
          <FormattedMessage
            id="dashboard.broker.actions.config"
            description="The action for editing config file on the broker block."
            defaultMessage="Config"
          />
        </MenuItem>
        <MenuItem button onClick={handleClose}>
          <FormattedMessage
            id="dashboard.broker.actions.reload"
            description="The action for reloading config file on the broker block."
            defaultMessage="Reload"
          />
        </MenuItem>
        <MenuItem button onClick={handleClose}>
          <FormattedMessage
            id="dashboard.broker.actions.restart"
            description="The action for restarting config file on the broker block."
            defaultMessage="Restart"
          />
        </MenuItem>
      </Menu>
    </Card>
  );
};

const BrokerCard = withStyles(styles)(_BrokerCard);

export default BrokerCard;
