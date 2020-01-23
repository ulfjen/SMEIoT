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
import { RouteComponentProps } from "@reach/router";
import { useTitle, useAsync } from 'react-use';
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
  DeviceBootstrapConfigBindingModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import FormControl from "@material-ui/core/FormControl";
import FormHelperText from '@material-ui/core/FormHelperText';

const styles = ({ spacing }: Theme) =>
  createStyles({
    container: {
    },
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    loadingPanel: {
      height: 200
    },
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
  useTitle(intl.formatMessage(messages.title));

  const [config, setConfig] = React.useState<DeviceBootstrapConfigBindingModel>({name: "", key: ""});
  const [handlingNext, setHandlingNext] = React.useState<boolean>(false);
  const [unconnectedDeviceName, setUnconnectedDeviceName] = React.useState<
    string | null
  >(null);
  const api = new DevicesApi(GetDefaultApiConfig());

  const [nameError, setNameError] = React.useState<string>("");
  const [keyError, setKeyError] = React.useState<string>("");
  const [entityError, setEntityError] = React.useState<string>("");

  const handleNext = async () => {
    setHandlingNext(true);
    setEntityError("");

    await api.apiDevicesBootstrapPost({
      deviceBootstrapConfigBindingModel: config
    }).then(res => {
      if (res !== null) {
        if (navigate) {
          navigate(`../wait_connection?name=${res.name}`);
        }
      }
      return res;
    }).catch(async res => {
      const pd = await res.json();
      if (pd.detail) { setEntityError(entityError); }
      const err = pd.errors;
      if (!err) { return; }
      if (err.hasOwnProperty("name")) { setNameError(err["name"].join("\n")); }
      if (err.hasOwnProperty("key")) { setKeyError(err["key"].join("\n")); }
    }).finally(() => {
      setHandlingNext(false);
    });
  };

  const [suggestingDeviceName, setSuggestDeviceName] = React.useState<boolean>(
    false
  );
  const onSuggestDeviceName: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestDeviceName(true);

    const res = await api.apiDevicesConfigSuggestDeviceNameGet();

    if (res.deviceName !== null) {
      setConfig({ ...config, name: res.deviceName });
      setNameError("");
    }

    setSuggestDeviceName(false);
    setNameError("");
  };

  const onDeviceNameChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setConfig({ ...config, name: event.target.value });
    setNameError("");
  };

  const [suggestingKey, setSuggestKey] = React.useState<boolean>(false);
  const onSuggestKey: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestKey(true);

    const res = await api.apiDevicesConfigSuggestKeyGet();

    if (res.key !== null) {
      setConfig({ ...config, key: res.key });
      setKeyError("");
    }

    setSuggestKey(false);
    setKeyError("");
  };

  const onKeyChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setConfig({ ...config, key: event.target.value });
    setKeyError("");
  };

  const onBannerClick = async (
    event: React.MouseEvent<HTMLButtonElement>
  ) => {
    if (navigate) {
      navigate(`../wait_connection?name=${unconnectedDeviceName}`);
    }
  }

  const state = useAsync(async () => {
    return await api.apiDevicesConfigSuggestBootstrapGet().then((res) => {
      setConfig({ ...config, name: res.deviceName, key: res.key });
      if (res.continuedConfigurationDevice) {
        setUnconnectedDeviceName(res.continuedConfigurationDevice);
      }
    });
  }, []);

  return (
    <DashboardNewDeviceFrame activeStep={0}>
      {!state.loading && unconnectedDeviceName && (
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
          {state.loading ?
            <div><Skeleton variant="text"/><Skeleton variant="text"/><Skeleton variant="text"/></div> :
            <div>
              <FormattedMessage
                id="dashboard.devices.new.step1.notice"
                description="Notice related with how we can add a new device"
                defaultMessage="We create a pre-shared key (PSK) for your new device and install it in our MQTT broker.
                  Be aware of your device's supported key length.
                  When your device connects with the broker with this key, you can add its sensor in the dashboard.
                  Registered and unused devices are shown on the devices page. "
              />
              <FormControl error={true}>
                <FormHelperText>{entityError}</FormHelperText>
              </FormControl>
              <SuggestTextField
                label={intl.formatMessage(messages.nameLabel)}
                autoFocus
                value={config.name}
                onChange={onDeviceNameChange}
                onSuggest={onSuggestDeviceName}
                suggesting={suggestingDeviceName}
                error={nameError}
              />
              <SuggestTextField
                label={intl.formatMessage(messages.keyLabel)}
                value={config.key}
                onChange={onKeyChange}
                onSuggest={onSuggestKey}
                suggesting={suggestingKey}
                error={keyError}
              />
              <div>
                <ProgressButton
                  onClick={handleNext}
                  loading={handlingNext}
                  disabled={suggestingKey || suggestingDeviceName}
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
          }
        </Paper>
      </Grid>
    </DashboardNewDeviceFrame>
  );
};

const DashboardNewDevice = withStyles(styles)(_DashboardNewDevice);

export default DashboardNewDevice;
