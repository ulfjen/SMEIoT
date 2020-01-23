import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { darken } from '@material-ui/core/styles';
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import Skeleton from "@material-ui/lab/Skeleton";
import Typography from "@material-ui/core/Typography";
import { RouteComponentProps } from "@reach/router";
import { useTitle, useAsync } from 'react-use';
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import {
  BasicSensorApiModel,
  DevicesApi,
  DeviceDetailsApiModel,
  BasicSensorApiModelStatusEnum
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import DashboardFrame from "./DashboardFrame";
import Container from "@material-ui/core/Container";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import { Link as ReachLink } from "@reach/router";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Link from '@material-ui/core/Link';
import Divider from '@material-ui/core/Divider';
import Button from '@material-ui/core/Button';
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import { AsyncState } from "react-use/lib/useAsync";
import StatusBadge from "../components/StatusBadge";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import CardHeader from "@material-ui/core/CardHeader";
import ExpansionPanel from "@material-ui/core/ExpansionPanel";
import ExpansionPanelSummary from "@material-ui/core/ExpansionPanelSummary";
import ExpansionPanelDetails from "@material-ui/core/ExpansionPanelDetails";
import ExpansionPanelActions from "@material-ui/core/ExpansionPanelActions";
import useMenu from "../helpers/useMenu";
import useModal from "../helpers/useModal";
import CardActions from "@material-ui/core/CardActions";
import DashboardSensorDialog from "./DashboardSensorDialog";

const styles = ({ typography, palette, spacing, zIndex }: Theme) => createStyles({
  container: {
  },
  instructions: {
    marginTop: spacing(1),
    marginBottom: spacing(1)
  },
  loadingPanel: {
    height: 200
  },
  column: {
    flexBasis: '33.33%',
  },
  twoColumnSpan: {
    flexBasis: '66.66%',
  },
  helper: {
    borderLeft: `2px solid ${palette.divider}`,
    padding: spacing(1, 2),
  },
  link: {
    color: palette.primary.main,
    textDecoration: 'none',
    '&:hover': {
      textDecoration: 'underline',
    },
  },
  heading: {
    fontSize: typography.pxToRem(15),
  },
  paper: {
    backgroundColor: darken(palette.background.paper, 0.01),
    overflow: "hidden"
  },
  panel: {
    "&:first-child": {
      borderTop: '2px solid red',

    }
  },
  firstPanel: {
    borderTop: `2px solid ${palette.divider}`,
    '&:before': {
      top: 0,
      height: 0,
      display: 'none',
    },
  },
  summary: {
    padding: "0 16px 0 16px",
    backgroundColor: darken(palette.background.paper, 0.01),
  },
  details: {
    backgroundColor: palette.background.paper,
    alignItems: 'center',
  },
  warning: {
    color: palette.text.secondary
  },
  list: {
    marginTop: 20
  },
  removeAction: {
    color: palette.error.main
  },
  backdrop: {
    zIndex: zIndex.drawer + 1,
    color: '#fff',
  },
});

export interface IDashboardSettingsRouteParams {
  deviceName: string;
}

export interface IDashboardSettingsProps extends RouteComponentProps<IDashboardSettingsRouteParams>, WithStyles<typeof styles> {

}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.edit.title",
    description: "Used as title in the edit device page on the dashboard",
    defaultMessage: "Configure device"
  },
  nameLabel: {
    id: "dashboard.devices.new.step1.name",
    description: "The label for adding device name",
    defaultMessage: "Device name"
  },
  keyLabel: {
    id: "dashboard.devices.new.step1.key",
    description: "The label for adding psk key",
    defaultMessage: "Key"
  },
  closeAriaLabel: {
    id: "dashboard.devices.edit.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  },
  moreAria: {
    id: "dashboard.devices.edit.more",
    description: "The aria label for more action",
    defaultMessage: "More"
  },
});

const _DashboardSettings: React.FunctionComponent<IDashboardSettingsProps> = ({
  classes,
  navigate
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  return <DashboardFrame
    title={intl.formatMessage(messages.title)}
    drawer
    direction="ltr"
    toolbarRight={
      <IconButton
        edge="end"
        color="inherit"
        aria-label={intl.formatMessage(messages.closeAriaLabel)}
        to={".."}
        component={ReachLink}
      >
        <CloseIcon />
      </IconButton>
    }
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Card>
            </Card>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardSettings = withStyles(styles)(_DashboardSettings);

export default DashboardSettings;
