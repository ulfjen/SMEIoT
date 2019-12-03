import Container from "@material-ui/core/Container";
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Grid from '@material-ui/core/Grid';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from "@material-ui/icons/Close";
import Frame from "./Frame";
import { Helmet } from "react-helmet";
import BasicBrokerCard from "./BasicBrokerCard";
import { defineMessages, useIntl } from "react-intl";
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

  return (
    <Frame
      title={title}
      direction="ltr"
      toolbarRight={
        <IconButton
          edge="end"
          color="inherit"
          aria-label="close this action"
          to={"/dashboard/devices"}
          component={ReachLink}
        >
          <CloseIcon />
        </IconButton>
      }
      content={
        <Container maxWidth="lg" className={classes.container}>
          <Helmet>
            <title>{title}</title>
          </Helmet>
          <Grid container spacing={2}>
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
