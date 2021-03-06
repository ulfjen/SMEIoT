import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Card from "@material-ui/core/Card";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import Button from "@material-ui/core/Button";
import Skeleton from "@material-ui/lab/Skeleton";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink
} from "@reach/router";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import StatusBadge from "../components/StatusBadge";
import useInterval from "../helpers/useInterval";
import { BrokerApi, BasicBrokerApiModel, ProblemDetails } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { ReactComponent as Broker } from "../images/broker.svg";
import { Typography } from "@material-ui/core";
import LoadFactors from "../components/LoadFactors";

const styles = ({ spacing, transitions }: Theme) => createStyles({
  expand: {
    transform: "rotate(0deg)",
    marginLeft: "auto",
    transition: transitions.create("transform", {
      duration: transitions.duration.shortest
    })
  },
  media: {
    height: "60%",
    filter: "brightness(0) invert(1)"
  },
  buttonLoading: {
    "& > span": {
      marginLeft: spacing(1),
      display: "inline-block"
    }
  }
});

export interface IBrokerCard extends WithStyles<typeof styles> { }

const messages = defineMessages({
  title: {
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
  },
  running: {
    id: "dashboard.components.basic_broker.status.running",
    description: "Used as in basic broker status",
    defaultMessage: "Running"
  },
  stopped: {
    id: "dashboard.components.basic_broker.status.stopped",
    description: "Used as in basic broker status",
    defaultMessage: "Stopped"
  },
});

interface BasicBrokerStatistics {
  [receivedMessages: string]: string;
}

const _BrokerCard: React.FunctionComponent<IBrokerCard> = ({ classes }) => {
  const intl = useIntl();

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [broker, setBroker] = React.useState<BasicBrokerApiModel>();

  const handleClose = () => {
    setAnchorEl(null);
  };
  const [loading, setLoading] = React.useState<boolean>(false);
  const [error, setError] = React.useState<string>("");

  const api = new BrokerApi(GetDefaultApiConfig());
  const updateBroker = async () => {
    setLoading(true);
    setError("");
    await api.apiBrokerBasicGet().then(res => {
      if (res !== null) {
        setBroker(res);
      }
      return res;
    })
    .catch(async res => {
      const pd: ProblemDetails = await res.json();
      setError(pd.title + "\n" + pd.detail);
    })
    .finally(() => {
      setLoading(false);
    });
  };

  useInterval(updateBroker, 10000);

  return (
    <Card>
      <ExpandedCardHeader
        // reload/restart is problematic with auth plugin. don't need to show the menu. stash the code here.
        // action={
        //   !loading &&
        //   <IconButton
        //     aria-label={intl.formatMessage(messages.more)}
        //     onClick={handleClick}
        //   >
        //     <MoreVertIcon />
        //   </IconButton>
        // }
        title={loading ? <Skeleton variant="rect" width={100} height={25} /> : intl.formatMessage(messages.title)}
        status={<StatusBadge
          severity={broker && broker.running ? "success" : "error"}
          badge={loading && <Skeleton variant="circle" height={14} width={14} />}
        >
          {loading ? <Skeleton variant="rect" width={60} height={14} /> : broker && intl.formatMessage(broker.running ? messages.running : messages.stopped)}
        </StatusBadge>}
        avatar={loading ? <Skeleton variant="circle" height={40} width={40} /> : <Broker className={classes.media} />}
      />
      <CardContent>
        {loading ?
          <div>
            <Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" />
          </div>
          : error.length > 0 ? <div>{error}</div> : <div>
            {broker && <LoadFactors min1={broker.min1} min5={broker.min5} min15={broker.min15} />}
            <Typography component="p" color="textSecondary">
              {broker && <FormattedMessage
                id="dashboard.broker.prompt"
                description="The prompt on the broker block."
                defaultMessage="Current monitoring {prefix}# at {host}:{port}"
                values={{
                  prefix: broker.mqttTopicPrefix,
                  host: broker.mqttHost,
                  port: broker.mqttPort
                }}
              />}
            </Typography>
          </div>
        }
      </CardContent>
      <CardActions disableSpacing>
        {loading ? <div className={classes.buttonLoading}><Skeleton variant="rect" width={50} height={22} /><Skeleton variant="rect" width={50} height={22} /></div> :
          <React.Fragment>
            <Button
              size="small"
              component={ReachLink}
              to="../broker/statistics"
            >
              {intl.formatMessage(messages.statistics)}
            </Button>
            <Button size="small" component={ReachLink} to="../broker/logs?filter=$SYS/broker">
              {intl.formatMessage(messages.logs)}
            </Button>
          </React.Fragment>
        }
      </CardActions>
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
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
