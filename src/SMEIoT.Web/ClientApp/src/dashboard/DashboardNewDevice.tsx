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
import { Link as ReachLink, RouteComponentProps } from "@reach/router";
import { Helmet } from "react-helmet";
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
  DeviceConfigBindingModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";

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

export interface IDashboardNewDeviceProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {}

const messages = defineMessages({
  title: {
    id: "dashboard.devices.new.title",
    description: "Used as title in the new device page on the dashboard",
    defaultMessage: "Connect with a new device"
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

const _DashboardNewDevice: React.FunctionComponent<IDashboardNewDeviceProps> = ({
  classes,
  navigate
}) => {
  const intl = useIntl();

  const [device, setDevice] = React.useState<DeviceConfigBindingModel>({name: "", key: ""});
  const [handlingNext, setHandlingNext] = React.useState<boolean>(false);
  const [loading, setLoading] = React.useState<boolean>(true);
  const [unconnectedDeviceName, setUnconnectedDeviceName] = React.useState<
    string | null
  >(null);
  const api = new DevicesApi(GetDefaultApiConfig());

  const handleNext = async () => {
    setHandlingNext(true);
    const api = new DevicesApi(GetDefaultApiConfig());
    const res = await api.apiDevicesBootstrapPost({
      deviceConfigBindingModel: device
    });
    if (res !== null) {
      if (navigate) {
        navigate(`connect?name=${res.name}`);
      }
    }
  };

  const [suggestingDeviceName, setSuggestDeviceName] = React.useState<boolean>(
    false
  );
  const onSuggestDeviceName: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestDeviceName(true);

    const res = await api.apiDevicesConfigSuggestDeviceNameGet();

    if (res.deviceName !== null) {
      setDevice({ ...device, name: res.deviceName });
    }

    setSuggestDeviceName(false);
  };

  const onDeviceNameChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setDevice({ ...device, name: event.target.value });
  };

  const [suggestingKey, setSuggestKey] = React.useState<boolean>(false);
  const onSuggestKey: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestKey(true);

    const res = await api.apiDevicesConfigSuggestKeyGet();

    if (res.key !== null) {
      setDevice({ ...device, key: res.key });
    }

    setSuggestKey(false);
  };

  const onKeyChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setDevice({ ...device, key: event.target.value });
  };

  const onBannerClick = async (
    event: React.MouseEvent<HTMLButtonElement>
  ) => {
    if (navigate) {
      navigate(`connect?name=${unconnectedDeviceName}`);
    }
  }

  React.useEffect(() => {
    (async () => {
      let res = await api.apiDevicesConfigSuggestBootstrapGet();
      setDevice({ ...device, name: res.deviceName || undefined, key: res.key || undefined });
      if (res.continuedConfigurationDevice) {
        setUnconnectedDeviceName(res.continuedConfigurationDevice);
      }
      setLoading(false);
    })();
  }, []);

  return (
    <DashboardNewDeviceFrame activeStep={0}>
      <Helmet>
        <title>{intl.formatMessage(messages.title)}</title>
      </Helmet>
      {!loading && unconnectedDeviceName && (
        <Grid item xs={12}>
          <BannerNotice onClick={onBannerClick}>
            <Typography component="p">
              <FormattedMessage
                id="dashboard.devices.new.step1.unconnected_notice"
                description="Notice related with continuing connecting a device"
                defaultMessage="Notice: your device {name} is not connected. Continue to connect instead of create a new one?"
                values={{
                  name: unconnectedDeviceName
                }}
              />
            </Typography>
          </BannerNotice>
        </Grid>
      )}

      <Grid item xs={12}>
        <Paper className={classes.paper}>
          {loading ? (
            <Skeleton variant="rect" className={classes.loadingPanel} />
          ) : (
            <div>
              <p>
                <FormattedMessage
                  id="dashboard.devices.new.step1.notice"
                  description="Notice related with how we can add a new device"
                  defaultMessage="We create a pre-shared key (PSK) for your new device and install it in our MQTT broker. 
              When your device connects with the broker with this key, you can add its sensor values in the dashboard.
              Registed and unused keys are shown on the devices page. 
              Notice: the MQTT broker will be reloaded to install the key."
                />
              </p>
              <SuggestTextField
                label={intl.formatMessage(messages.nameLabel)}
                autoFocus
                value={device.name}
                onChange={onDeviceNameChange}
                onSuggest={onSuggestDeviceName}
                suggesting={suggestingDeviceName}
              />
              <SuggestTextField
                label={intl.formatMessage(messages.keyLabel)}
                value={device.key}
                onChange={onKeyChange}
                onSuggest={onSuggestKey}
                suggesting={suggestingKey}
              />
              <div>
                <ProgressButton
                  onClick={handleNext}
                  loading={handlingNext}
                  variant="contained"
                  color="primary"
                >
                  <FormattedMessage
                    id="dashboard.devices.new.control.create"
                    description="The button text for creating devices"
                    defaultMessage="Create"
                  />
                </ProgressButton>
              </div>
            </div>
          )}
        </Paper>
      </Grid>
    </DashboardNewDeviceFrame>
  );
};

const DashboardNewDevice = withStyles(styles)(_DashboardNewDevice);

export default DashboardNewDevice;
