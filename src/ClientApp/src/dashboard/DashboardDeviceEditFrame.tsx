import Container from "@material-ui/core/Container";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import Grid from "@material-ui/core/Grid";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import { Link as ReachLink } from "@reach/router";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Typography from '@material-ui/core/Typography';
import Link from '@material-ui/core/Link';
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import DeviceStatusCard from "./DeviceStatusCard";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { DeviceApiModel } from "smeiot-client/src";

const styles = ({ spacing }: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    }
  });

export interface IDashboardDeviceDetailFrameProps
  extends WithStyles<typeof styles> {
  children: React.ReactNode;
  device?: DeviceApiModel;
}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.edit.title",
    description: "Used as title in the new device page on the dashboard",
    defaultMessage: "Connect with a new device"
  },
  closeAriaLabel: {
    id: "dashboard.devices.edit.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  }
});

const DashboardDeviceEditFrame: React.FunctionComponent<IDashboardDeviceDetailFrameProps> = ({
  classes,
  children,
  device
}) => {
  const intl = useIntl();

  return (
    <DashboardFrame
      title={intl.formatMessage(messages.title)}
      direction="ltr"
      toolbarRight={
        <IconButton
          edge="end"
          color="inherit"
          aria-label={intl.formatMessage(messages.closeAriaLabel)}
          to={"../.."}
          component={ReachLink}
        >
          <CloseIcon />
        </IconButton>
      }
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />} aria-label="breadcrumb">
                <Link component={ReachLink} color="inherit" to="../..">
                  <FormattedMessage
                    id="dashboard.devices.edit.breadcrumb.devices"
                    description="The label at the breadcrumb for devices"
                    defaultMessage="Devices"
                  />
                </Link>
                <Typography color="textPrimary">
                  <FormattedMessage
                    id="dashboard.devices.edit.breadcrumb.edit"
                    description="The label at the breadcrumb for editing device"
                    defaultMessage="Configuration"
                  />
                </Typography>
              </Breadcrumbs>
            </Grid>
            <Grid item xs={12}>
              <DeviceStatusCard device={device}/>
            </Grid>
            {children}
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardDeviceDetailFrame = withStyles(styles)(DashboardDeviceEditFrame);

export default DashboardDeviceDetailFrame;
