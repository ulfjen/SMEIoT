import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import Typography from "@material-ui/core/Typography";
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { SensorDetailsApiModel, DeviceApiModel } from "smeiot-client/src";

const styles = ({typography, spacing, palette, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  sensorCard: {

  },
  heading: {
    fontSize: typography.pxToRem(15),
    flexBasis: '33.33%',
    flexShrink: 0,
  },
  secondaryHeading: {
    fontSize: typography.pxToRem(15),
    color: palette.text.secondary,
  },

});

export interface IDeviceListItem extends WithStyles<typeof styles> {
  device: DeviceApiModel;
  expanded: boolean;
}

const _DeviceListItem: React.FunctionComponent<IDeviceListItem & WithStyles<typeof styles>> = ({ classes, expanded, device }) => {
  expanded = true;
  
  return <ExpansionPanel expanded={expanded}>
    <ExpansionPanelSummary
      expandIcon={<ExpandMoreIcon />}
      aria-controls="panel1bh-content"
      id="panel1bh-header"
    >
      <Typography className={classes.heading}>{device.name}</Typography>
      <Typography className={classes.secondaryHeading}>device information</Typography>
    </ExpansionPanelSummary>
    <ExpansionPanelDetails>
      <Typography>
        more device information
      </Typography>
    </ExpansionPanelDetails>
  </ExpansionPanel>;
}

const DeviceListItem = withStyles(styles)(_DeviceListItem);
export default DeviceListItem;
