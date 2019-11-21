import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Card from "@material-ui/core/Card";
import CardActionArea from "@material-ui/core/CardActionArea";
import CardMedia from "@material-ui/core/CardMedia";
import CardContent from "@material-ui/core/CardContent";
import CardActions from "@material-ui/core/CardActions";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import { SensorDetailsApiModel } from "smeiot-client/src";

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
    paddingTop: spacing(4),
    paddingBottom: spacing(4),
  },
  sensorCard: {

  }
});

export interface IDeviceListItem extends WithStyles<typeof styles> {
  // device: SensorDetailsApiModel;
}

const _DeviceListItem: React.FunctionComponent<IDeviceListItem & WithStyles<typeof styles>> = ({ classes }) => {
  const onClick = (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
  };

  return <div></div>;
  // return <Card className={classes.sensorCard}>
  //   <CardActionArea>
  //     <CardMedia
  //       component="img"
  //       alt="placeholder"
  //       height="140"
  //       image="/placeholder.jpg"
  //       title="placeholder"
  //     />
  //     <CardContent>
  //       <Typography gutterBottom variant="h5" component="h2">
  //         {sensor.sensorName}
  //         </Typography>
  //       <Typography variant="body2" color="textSecondary" component="p">
  //         secondary placeholder
  //         </Typography>
  //     </CardContent>
  //   </CardActionArea>
  //   <CardActions>
  //     <Button size="small" color="primary">
  //       Assign
  //       </Button>
  //     <Button size="small" color="primary">
  //       Details
  //       </Button>
  //   </CardActions>
  // </Card>;
}


const DeviceListItem = withStyles(styles)(_DeviceListItem);
export default DeviceListItem;