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
import DashboardNewDeviceStepper from "./DashboardNewDeviceStepper";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";

const styles = ({ spacing }: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    }
  });

export interface IDashboardNewDeviceFrameProps
  extends WithStyles<typeof styles> {
  activeStep: number;
  children: React.ReactNode;
}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.new.title",
    description: "Used as title in the new device page on the dashboard",
    defaultMessage: "Connect with a new device"
  },
  closeAriaLabel: {
    id: "dashboard.devices.new.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  }
});

const _DashboardNewDeviceFrame: React.FunctionComponent<IDashboardNewDeviceFrameProps> = ({
  classes,
  activeStep,
  children
}) => {
  const intl = useIntl();

  return (
    <DashboardFrame
      title={intl.formatMessage(messages.title)}
      drawer
      direction="ltr"
      toolbarRight={
        <IconButton
          edge="end"
          color="inherit"
          aria-label={intl.formatMessage(messages.closeAriaLabel)}
          to={"../../devices"}
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
                <Link component={ReachLink} color="inherit" to="../../devices">
                  <FormattedMessage
                    id="dashboard.devices.new.breadcrumb.devices"
                    description="The label at the breadcrumb for devices"
                    defaultMessage="Devices"
                  />
                </Link>
                <Typography color="textPrimary">
                  <FormattedMessage
                    id="dashboard.devices.new.breadcrumb.new"
                    description="The label at the breadcrumb for the new device wizard"
                    defaultMessage="New device"
                  />
                </Typography>
              </Breadcrumbs>
            </Grid>
            <Grid item xs={12}>
              <DashboardNewDeviceStepper activeStep={activeStep} />
            </Grid>
            {children}
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardNewDeviceFrame = withStyles(styles)(_DashboardNewDeviceFrame);

export default DashboardNewDeviceFrame;
