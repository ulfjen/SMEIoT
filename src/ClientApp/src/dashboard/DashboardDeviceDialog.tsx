import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import {
  FormattedMessage
} from "react-intl";
import { NavigateFn } from "@reach/router";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogActions from "@material-ui/core/DialogActions";
import DialogTitle from "@material-ui/core/DialogTitle";
import Backdrop from "@material-ui/core/Backdrop";
import CircularProgress from "@material-ui/core/CircularProgress";
import Button from "@material-ui/core/Button";
import Snackbar from "@material-ui/core/Snackbar";
import Alert from "@material-ui/lab/Alert";
import { DevicesApi, ProblemDetails } from "smeiot-client";
import { GetDefaultApiConfig } from "..";
import {
  defineMessages,
  useIntl,
} from "react-intl";

const styles = ({ typography, palette, spacing, zIndex }: Theme) => createStyles({
  removeAction: {
    color: palette.error.main
  },
  backdrop: {
    zIndex: zIndex.drawer + 1,
    color: '#fff',
  },
});

export interface IDashboardDeviceDialogProps extends WithStyles<typeof styles> {
  open: boolean;
  closeDialog: () => void
  deviceName?: string;
  navigateUrl: string;
  navigate?: NavigateFn;
}

const messages = defineMessages({
  defaultError: {
    id: "dashboard.devices.show.dialog",
    description: "Message on snackbar",
    defaultMessage: "Something went wrong."
  }
});


const _DashboardDeviceDialog: React.FunctionComponent<IDashboardDeviceDialogProps> = ({
  classes, open, closeDialog, deviceName, navigate, navigateUrl
}) => {
  const intl = useIntl();
  const [removing, setRemoving] = React.useState<boolean>(false);

  const handleCancelClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.preventDefault();
    closeDialog();
  }

  const [snackbarOpen, setSnackbarOpen] = React.useState<boolean>(false);
  const [snackbarMessage, setSnackbarMessage] = React.useState<string>("");
  
  const handleRemoveClick = async (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.preventDefault();
    closeDialog();
    if (!deviceName) { return; }
    const api = new DevicesApi(GetDefaultApiConfig());
    setRemoving(true);
    await api.apiDevicesNameDelete({
      name: deviceName
    }).then((res) => {
      window.location.href = navigateUrl;
    }).catch(async response => {
      const pd: ProblemDetails = await response.json();
      setSnackbarOpen(true);
      let msg = "";
      if (pd.detail) {
        msg += pd.detail;
      }
      if (msg.length === 0) {
        msg = intl.formatMessage(messages.defaultError);
      }
      setSnackbarMessage(msg);
    }).finally(() => {
      setRemoving(false);
    });
  }

  const closeSnackbar = (e: React.SyntheticEvent<Element, Event> | null) => {
    if (e) { e.preventDefault() }
    setSnackbarOpen(false);
  };

  return <React.Fragment>
    <Dialog
      open={open}
      onClose={() => closeDialog()}
    >
      <DialogTitle>
        <FormattedMessage
          id="dashboard.devices.edit.dialog.title"
          description="The instruction for removing device's dialog"
          defaultMessage="Remove device {name}?"
          values={{
            name: deviceName
          }}
        />
      </DialogTitle>
      <DialogContent>
        <DialogContentText>
          <FormattedMessage
            id="dashboard.devices.edit.dialog.content"
            description="The instruction for removing device's dialog"
            defaultMessage="After removing device, we will no longer track devices and its sensor from MQTT."
          />
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleCancelClick} autoFocus>
          <FormattedMessage
            id="dashboard.devices.edit.dialog.remove.actions.cancel"
            description="The label at the removing device's dialog button"
            defaultMessage="Cancel"
          />
        </Button>
        <Button onClick={handleRemoveClick}>
          <FormattedMessage
            id="dashboard.devices.edit.dialog.remove.actions.remove"
            description="The label at the removing device's dialog button"
            defaultMessage="Remove"
          />
        </Button>
      </DialogActions>
    </Dialog>
    <Backdrop className={classes.backdrop} open={removing}>
      <CircularProgress color="inherit" />
    </Backdrop>
    <Snackbar anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }} open={snackbarOpen} autoHideDuration={5000} onClose={closeSnackbar}>
      <Alert onClose={closeSnackbar} severity="error">
        {snackbarMessage}
      </Alert>
    </Snackbar>
  </React.Fragment>
};

const DashboardDeviceDialog = withStyles(styles)(_DashboardDeviceDialog);

export default DashboardDeviceDialog;
