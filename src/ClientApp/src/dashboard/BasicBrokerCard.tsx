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
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";
import useInterval from "../helpers/useInterval";
import BrokerCardHeader from "./BrokerCardHeader";
import { BrokerApi } from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import LoadFactors from "../components/LoadFactors";

const styles = ({ transitions, spacing }: Theme) =>
  createStyles({
    root: {
      padding: 16,
    }
  });

export interface IBasicBrokerCard extends WithStyles<typeof styles> { }

interface BasicBroker {
  running: boolean;
  min1?: number;
  min5?: number;
  min15?: number;
}

const _BasicBrokerCard: React.FunctionComponent<IBasicBrokerCard> = ({ classes }) => {
  const intl = useIntl();

  const [broker, setBroker] = React.useState<BasicBroker>({
    running: false
  });

  const api = new BrokerApi(GetDefaultApiConfig());
  const updateBroker = async () => {
    let details = await api.apiBrokerBasicGet();
    if (details === null) { return; }
    setBroker({
      running: details.running || broker.running,
      min1: details.min1,
      min5: details.min5,
      min15: details.min15
    });
  };

  // const loading = useLoading(updateBroker, 400, 10000);
  useInterval(updateBroker, 10000);

  return <Card>
    <CardActionArea className={classes.root} component={ReachLink} to={"/dashboard/devices"}>
      <BrokerCardHeader running={broker.running} />
      <LoadFactors min1={broker.min1} min5={broker.min5} min15={broker.min15} />
      <Typography color="textSecondary">
        {
          broker.running ? <FormattedMessage
            id="dashboard.components.basic_broker_card.instruct_running"
            description="Running instruction on the broker card when the broker runs normally."
            defaultMessage="The broker is operating."
          /> : <FormattedMessage
              id="dashboard.components.basic_broker_card.instruct_stopped"
              description="Running instruction on the broker card when the broker stopped."
              defaultMessage="The broker is stopped. Please wait a few seconds."
            />
        }
      </Typography>
    </CardActionArea>
  </Card>;
};

const BasicBrokerCard = withStyles(styles)(_BasicBrokerCard);

export default BasicBrokerCard;
