import Container from "@material-ui/core/Container";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Grid from '@material-ui/core/Grid';
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import Link from '@material-ui/core/Link';
import CloseIcon from "@material-ui/icons/Close";
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import DashboardFrame from "./DashboardFrame";
import { useTitle } from 'react-use';
import BasicBrokerCard from "./BasicBrokerCard";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import {
  Link as ReachLink,
} from "@reach/router";

const styles = ({ spacing }: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    }
  });

export interface IDashboardBrokerFrameProps
  extends WithStyles<typeof styles> {
  children: React.ReactNode;
  title: string;
}

const messages = defineMessages({
  closeAriaLabel: {
    id: "dashboard.devices.new.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  }
});

const _DashboardBrokerFrame: React.FunctionComponent<IDashboardBrokerFrameProps> = ({
  classes,
  title,
  children
}) => {
  const intl = useIntl();
  useTitle(title);

  return (
    <DashboardFrame
      title={title}
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
          <Grid container spacing={2}>
            <Grid item xs={12}>
            <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />} aria-label="breadcrumb">
              <Link component={ReachLink} color="inherit" to="../../devices">
                <FormattedMessage
                 id="dashboard.devices.breadcrumb.devices"
                 description="The label at the breadcrumb for devices"
                 defaultMessage="Devices"
                />
              </Link>
              <Typography color="textPrimary">
                <FormattedMessage
                 id="dashboard.devices.breadcrumb.broker"
                 description="The label at the breadcrumb for the broker"
                 defaultMessage="Broker"
                />
              </Typography>
            </Breadcrumbs>
            </Grid>
            <Grid item xs={12}>
              <BasicBrokerCard/>
            </Grid>
            {children}
          </Grid>
        </Container>
      }
    />
  );
};

const DashboardBrokerFrame = withStyles(styles)(_DashboardBrokerFrame);

export default DashboardBrokerFrame;
