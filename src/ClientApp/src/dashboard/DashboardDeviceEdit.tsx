import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardNewDeviceFrame from "./DashboardNewDeviceFrame";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import Skeleton from "@material-ui/lab/Skeleton";
import Typography from "@material-ui/core/Typography";
import AddIcon from "@material-ui/icons/Add";
import { RouteComponentProps } from "@reach/router";
import { useTitle } from 'react-use';
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import BannerNotice from "../components/BannerNotice";
import ProgressButton from "../components/ProgressButton";
import SuggestTextField from "../components/SuggestTextField";
import {
  DevicesApi,
  DeviceConfigBindingModel,
  SensorsApi
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import DashboardDeviceEditFrame from "./DashboardDeviceEditFrame";
import Container from "@material-ui/core/Container";
import Tooltip from "@material-ui/core/Tooltip";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";

const styles = ({ spacing }: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    },
    instructions: {
      marginTop: spacing(1),
      marginBottom: spacing(1)
    },
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    loadingPanel: {
      height: 200
    }
  });

export interface IDashboardEditDeviceRouteParams {
  deviceName: string;
}

export interface IDashboardDeviceEditProps
  extends RouteComponentProps<IDashboardEditDeviceRouteParams>,
  WithStyles<typeof styles> { }

const messages = defineMessages({
  title: {
    id: "dashboard.devices.edit.title",
    description: "Used as title in the edit device page on the dashboard",
    defaultMessage: "Edit device"
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
  }
});

const _DashboardDeviceEdit: React.FunctionComponent<IDashboardDeviceEditProps> = ({
  classes,
  deviceName,
  navigate
}) => {
  const intl = useIntl();
  useTitle(intl.formatMessage(messages.title));

  const [sensorNames, setSensorNames] = React.useState<string[] | undefined>();
  // const [deviceName, setDeviceName] = React.useState<string>("");
  const [handlingNext, setHandlingNext] = React.useState<boolean>(false);
  const [loading, setLoading] = React.useState<boolean>(true);
  const [unconnectedDeviceName, setUnconnectedDeviceName] = React.useState<
    string | null
  >(null);
  const api = new SensorsApi(GetDefaultApiConfig());

  const renderActionList = (deviceName: string, names: string[]) => {
    return names.map(name => <TwoLayerLabelAction
      firstLabel={deviceName}
      key={name}
      secondLabel={name}
      firstLabelVariant="inherit"
      actionIcon={<AddIcon/>}
      actionIconOnClick={async (event) => {
        let parent = event.currentTarget.parentElement
        if (!parent) { return; }
        // .parentElement.children;
        let deviceName = parent.childNodes[0].textContent;
        let sensorName = parent.childNodes[2].textContent;
        if (deviceName === null || sensorName === null) { return; }
        let sensor = await api.apiSensorsPost({
          sensorLocatorBindingModel: {
            deviceName: deviceName,
            name: sensorName
          }
        });
        console.log(sensor);
      }}
     />);
  }

  React.useEffect(() => {
    (async () => {
      const api = new DevicesApi(GetDefaultApiConfig());
      if (deviceName) {
        const res = await api.apiDevicesNameSensorCandidatesGet({
          name: deviceName
        });
        if (res !== null) {
          setSensorNames(res.names);
        }
      }
      setLoading(false);
    })();
  }, []);

  return <DashboardDeviceEditFrame device={undefined}>
    <Grid item xs={12}>
      <Paper>{renderActionList("pupate-potteen", sensorNames || [])}</Paper>
    </Grid>
  </DashboardDeviceEditFrame>;
};

const DashboardDeviceEdit = withStyles(styles)(_DashboardDeviceEdit);

export default DashboardDeviceEdit;
