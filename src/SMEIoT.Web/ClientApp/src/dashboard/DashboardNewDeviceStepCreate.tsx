import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Paper from "@material-ui/core/Paper";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import Skeleton from "@material-ui/lab/Skeleton";
import Button from "@material-ui/core/Button";
import { RouteComponentProps } from "@reach/router";
import BannerNotice from "../components/BannerNotice";
import SuggestTextField from "../components/SuggestTextField";
import { defineMessages, useIntl, FormattedMessage } from "react-intl";
import { GetDefaultApiConfig } from "../index";
import { DevicesApi } from "smeiot-client";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    container: {
      paddingTop: spacing(4),
      paddingBottom: spacing(4)
    },
    backButton: {
      marginRight: spacing(1)
    },
    instructions: {
      marginTop: spacing(1),
      marginBottom: spacing(1)
    },
    list: {
      backgroundColor: "#ffffff"
    },
    filterBar: {
      backgroundColor: "#ffffff",
      display: "flex",
      flexWrap: "wrap",
      "& > *": {
        margin: spacing(0.5)
      },
      marginBottom: spacing(2)
    },
    paper: {
      padding: spacing(2),
      display: "flex",
      overflow: "auto",
      flexDirection: "column"
    },
    fixedHeight: {
      height: 240
    },
    usersMenu: {},
    usersMenuDeleteItem: {
      color: palette.error.main
    },
    avatar: {},
    cardContent: {},
    loadingPanel: {
      height: 200
    }
  });

export interface IDashboardNewDeviceStepCreateProps
  extends RouteComponentProps,
    WithStyles<typeof styles> {
  handleNext: (event: React.MouseEvent<HTMLButtonElement>) => void;
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
  handleNext
}) => {
  const intl = useIntl();

  const deviceNotConnected = true;
  const [loading, setLoading] = React.useState<boolean>(true);
  const api = new DevicesApi(GetDefaultApiConfig());

  const [suggestingDeviceName, setSuggestingDeviceName] = React.useState<
    boolean
  >(false);
  const [deviceName, setDeviceName] = React.useState<string | null | undefined>(
    null
  );
  const onSuggestDeviceName: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestingDeviceName(true);
    const res = await api.apiDevicesSuggestDeviceNameGet();
    setDeviceName(res.deviceName);
    setSuggestingDeviceName(false);
  };
  const onDeviceNameChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setDeviceName(event.target.value);
  };

  const [suggestingKey, setSuggestingKey] = React.useState<boolean>(false);
  const [key, setKey] = React.useState<string | null | undefined>();
  const onSuggestKey: React.MouseEventHandler<HTMLButtonElement> = async event => {
    setSuggestingKey(true);
    const res = await api.apiDevicesSuggestKeyGet();
    setKey(res.key);
    setSuggestingKey(false);
  };
  const onKeyChange: React.ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = event => {
    setKey(event.target.value);
  };

  React.useEffect(() => {
    (async () => {
      var res = await api.apiDevicesSuggestBootstrapConfigGet();
      setKey(res.key);
      setDeviceName(res.deviceName);
      setLoading(false);
    })();
  }, []);

  return (
    <React.Fragment>
      {!loading && deviceNotConnected && (
        <Grid item xs={12}>
          <BannerNotice to={null}>
            <Typography component="p">
              notice: your device L403 is not connected. Continue to connect
              instead of create a new one?
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
                value={deviceName}
                onChange={onDeviceNameChange}
                onSuggest={onSuggestDeviceName}
                suggesting={suggestingDeviceName}
              />
              <SuggestTextField
                label={intl.formatMessage(messages.keyLabel)}
                value={key}
                onChange={onKeyChange}
                onSuggest={onSuggestKey}
                suggesting={suggestingKey}
              />
              <div>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleNext}
                >
                  <FormattedMessage
                    id="dashboard.devices.new.control.create"
                    description="The button text for creating devices"
                    defaultMessage="Create"
                  />
                </Button>
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
