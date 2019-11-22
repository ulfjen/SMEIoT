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
import Typography from "@material-ui/core/Typography";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import { SensorDetailsApiModel, DeviceApiModel } from "smeiot-client";

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
}

const _DeviceCard: React.FunctionComponent<IDeviceCard &
  WithStyles<typeof styles>> = ({ classes, device }) => {

  var summary = "";
  if (device.name === "L401") {
    summary = "Connected with temp1";
  } else if (device.name === "L402") {
    summary = "No sensor connected.";
  } else {
    summary = "Device is not configured.";
  }

  return (
    <Card className={clsx(classes.card, !device.connected && classes.notConnected)}>
      <CardHeader
        action={
          <IconButton aria-label="settings">
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
        <IconButton aria-label="add to favorites"></IconButton>
        <IconButton aria-label="share"></IconButton>
      </CardActions>
    </Card>
  );
};

const DeviceCard = withStyles(styles)(_DeviceCard);
export default DeviceCard;
