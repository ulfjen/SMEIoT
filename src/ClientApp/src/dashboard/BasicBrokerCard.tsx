import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Card from "@material-ui/core/Card";
import CardActionArea from "@material-ui/core/CardActionArea";
import Avatar from "@material-ui/core/Avatar";
import IconButton from "@material-ui/core/IconButton";
import Typography from "@material-ui/core/Typography";
import Skeleton from "@material-ui/lab/Skeleton";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";
import useInterval from "../helpers/useInterval";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import { BrokerApi, BasicBrokerApiModel } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import LoadFactors from "../components/LoadFactors";
import StatusBadge from "../components/StatusBadge";
import { ReactComponent as Broker } from "../images/broker.svg";

const styles = ({ transitions, spacing }: Theme) => createStyles({
  root: {
    padding: 16,
  },
  media: {
    height: "60%",
    filter: "brightness(0) invert(1)"
  },
  secondary: {
    marginTop: 5
  }
});

export interface IBasicBrokerCard extends WithStyles<typeof styles> { 
  avatar?: boolean;
}

interface BasicBroker {
  running: boolean;
  min1?: number;
  min5?: number;
  min15?: number;
}

const messages = defineMessages({
  title: {
    id: "dashboard.components.basic_broker.title",
    description: "The broker block title on the basic broker card.",
    defaultMessage: "Broker"
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

const _BasicBrokerCard: React.FunctionComponent<IBasicBrokerCard> = ({ classes, avatar }) => {
  const intl = useIntl();

  const [broker, setBroker] = React.useState<BasicBrokerApiModel>({
    running: false,
    lastUpdatedAt: null,
    min1: null,
    min5: null,
    min15: null
  });
  const [loading, setLoading] = React.useState<boolean>(false);

  const api = new BrokerApi(GetDefaultApiConfig());
  const updateBroker = async () => {
    setLoading(true);
    let details = await api.apiBrokerBasicGet();
    if (details === null) { return; }
    setBroker(details);
    setLoading(false);
  };

  // const loading = useLoading(updateBroker, 400, 10000);
  useInterval(updateBroker, 10000);

  return <Card>
    <CardActionArea className={classes.root} component={ReachLink} to={"/dashboard/devices"}>
      <ExpandedCardHeader
        title={loading ? <Skeleton variant="rect" width={100} height={25} /> : <Typography variant="h5" color="primary" display="block">
          {intl.formatMessage(messages.title)}</Typography>}
        status={<StatusBadge
          color={loading ? null : (broker.running ? "normal" : "error")} 
          badge={loading && <Skeleton variant="circle" width={14} height={14}/>}
        >
          {loading ? <Skeleton variant="rect" width={60} height={14}/> : intl.formatMessage(broker.running ? messages.running : messages.stopped)}
        </StatusBadge>}
        avatar={avatar && <Broker className={classes.media} />}
      />
      <div className={classes.secondary}>
        {loading ? <Skeleton variant="rect" width={200} height={10} /> : null}
      </div>
      <Typography color="textSecondary">
        {
          loading ? <Skeleton variant="text" /> :
          (broker.running ? <FormattedMessage
            id="dashboard.components.basic_broker_card.instruct_running"
            description="Running instruction on the broker card when the broker runs normally."
            defaultMessage="The broker is operating."
          /> : <FormattedMessage
              id="dashboard.components.basic_broker_card.instruct_stopped"
              description="Running instruction on the broker card when the broker stopped."
              defaultMessage="The broker is stopped. Please wait a few seconds."
            />)
        }
      </Typography>
    </CardActionArea>
  </Card>;
};

const BasicBrokerCard = withStyles(styles)(_BasicBrokerCard);

export default BasicBrokerCard;
