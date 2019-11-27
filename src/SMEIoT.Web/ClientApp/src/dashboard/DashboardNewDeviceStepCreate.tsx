import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import Skeleton from "@material-ui/lab/Skeleton";
import { RouteComponentProps } from "@reach/router";
import BannerNotice from "../components/BannerNotice";
import SuggestTextField from "../components/SuggestTextField";
import ProgressButton from "../components/ProgressButton";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { GetDefaultApiConfig } from "../index";
import { DevicesApi, DeviceConfigBindingModel } from "smeiot-client";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
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

export interface IDashboardNewDeviceStepCreateProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {
  handleNext: React.MouseEventHandler<HTMLButtonElement>;
  handlingNext: boolean;
  device: DeviceConfigBindingModel;
  setDevice: React.Dispatch<React.SetStateAction<DeviceConfigBindingModel>>;
  onBannerClick: React.MouseEventHandler<HTMLButtonElement>;
  unconnectedDeviceName: string | null;
  setUnconnectedDeviceName: React.Dispatch<React.SetStateAction<string | null>>;
}

const messages = defineMessages({
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

const _DashboardNewDeviceStepCreate: React.FunctionComponent<IDashboardNewDeviceStepCreateProps> = ({
  classes,
  handleNext,
  handlingNext,
  device,
  setDevice,
  onBannerClick,
  unconnectedDeviceName,
  setUnconnectedDeviceName
}) => {
  const intl = useIntl();

  const [loading, setLoading] = React.useState<boolean>(true);
  const api = new DevicesApi(GetDefaultApiConfig());

  const [suggestingDeviceName, setSuggestDeviceName] = React.useState<boolean>(
    false
  );
  const onSuggestDeviceName: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestDeviceName(true);

    const res = await api.apiDevicesConfigSuggestDeviceNameGet();

    if (res.deviceName !== null) {
      device.name = res.deviceName;
      setDevice(device);
    }

    setSuggestDeviceName(false);
  };

  const onDeviceNameChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    device.name = event.target.value;
    setDevice(device);
  };

  const [suggestingKey, setSuggestKey] = React.useState<boolean>(false);
  const onSuggestKey: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestKey(true);

    const res = await api.apiDevicesConfigSuggestKeyGet();

    if (res.key !== null) {
      device.key = res.key;
    }

    setSuggestKey(false);
  };

  const onKeyChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    device.key = event.target.value;
    setDevice(device);
  };

  React.useEffect(() => {
    (async () => {
      var res = await api.apiDevicesConfigSuggestBootstrapGet();
      if (res.deviceName) {
        device.name = res.deviceName;
      }
      if (res.key) {
        device.key = res.key;
      }
      setDevice(device);
      if (res.continuedConfigurationDevice) {
        setUnconnectedDeviceName(res.continuedConfigurationDevice);
      }
      setLoading(false);
    })();
  }, []);

  return (
    <React.Fragment>
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
                // autoFocus
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
    </React.Fragment>
  );
};

const DashboardNewDeviceStepCreate = withStyles(styles)(
  _DashboardNewDeviceStepCreate
);

export default DashboardNewDeviceStepCreate;
