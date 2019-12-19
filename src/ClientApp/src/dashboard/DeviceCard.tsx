import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import { red } from "@material-ui/core/colors";
import clsx from "clsx";
import withStyles from "@material-ui/core/styles/withStyles";
import Card from "@material-ui/core/Card";
import CardHeader from "@material-ui/core/CardHeader";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import IconButton from "@material-ui/core/IconButton";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import ListAltIcon from '@material-ui/icons/ListAlt';
import { SensorDetailsApiModel, DeviceApiModel } from "smeiot-client";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
  LinkProps as ReachLinkProps,
  RouteComponentProps
} from "@reach/router";

const styles = ({
  transitions,
  spacing,
  palette,
  zIndex,
  mixins,
  breakpoints
}: Theme) =>
  createStyles({
    card: {
      maxWidth: 345
    },
    media: {
      height: 0,
      paddingTop: "56.25%" // 16:9
    },
    expandOpen: {
      transform: "rotate(180deg)"
    },
    avatar: {
      backgroundColor: red[500]
    },
    notConnected: {
      backgroundColor: "#eeeeee", // needs to dim other component
    }
  });

export interface IDeviceCard extends WithStyles<typeof styles> {
  device: DeviceApiModel;
  onMoreClick: (event: React.MouseEvent<HTMLButtonElement>, deviceName?: string) => void;
}

const _DeviceCard: React.FunctionComponent<IDeviceCard &
  WithStyles<typeof styles>> = ({ classes, device, onMoreClick }) => {

  var summary = "";
  if (device.name === "L401") {
    summary = "Connected with temp1";
  } else if (device.name === "L402") {
    summary = "No sensor connected.";
  } else {
    summary = "Device is not configured.";
  }

  const onMoreClickHandler = (event: React.MouseEvent<HTMLButtonElement>) => onMoreClick(event, device.name);

  return (
    <Card className={clsx(classes.card, !device.connected && classes.notConnected)}>
      <CardHeader
        action={
          <IconButton aria-label="settings" onClick={onMoreClickHandler}>
            <MoreVertIcon />
          </IconButton>
        }
        title={device.name}
        subheader={summary}
      />
      <CardContent>
        <Typography variant="body2" color="textSecondary" component="p">
          instruction space
        </Typography>
      </CardContent>
      <CardActions disableSpacing>
        <Button size="small" component={ReachLink} to={`/dashboard/sensors?device_name=${device.name}`}>
          <FormattedMessage
            id="dashboard.device.actions.see_sensor"
            description="The action to go to see sensors in the device card."
            defaultMessage="Sensors"
          />
        </Button>
        <Button size="small" component={ReachLink} to={`/dashboard/broker/logs?device_name=${device.name}`}>
          <FormattedMessage
            id="dashboard.device.actions.logs"
            description="The action for device cards."
            defaultMessage="Logs"
          />
        </Button>
      </CardActions>
    </Card>
  );
};

const DeviceCard = withStyles(styles)(_DeviceCard);
export default DeviceCard;
