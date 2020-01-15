import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { useTitle, useAsync } from 'react-use';
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import {
  SensorsApi,
  DevicesApi,
  DeviceDetailsApiModel
} from "smeiot-client";
import { GetDefaultApiConfig } from "../index";
import { Link as ReachLink, NavigateFn } from "@reach/router";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogActions from "@material-ui/core/DialogActions";
import DialogTitle from "@material-ui/core/DialogTitle";
import Backdrop from "@material-ui/core/Backdrop";
import CircularProgress from "@material-ui/core/CircularProgress";
import Button from "@material-ui/core/Button";

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
  navigate?: NavigateFn;
}

const messages = defineMessages({
});

const _DashboardDeviceDialog: React.FunctionComponent<IDashboardDeviceDialogProps> = ({
  classes, open, closeDialog, deviceName, navigate
}) => {
  const intl = useIntl();

  const [removing, setRemoving] = React.useState<boolean>(false);

  const handleCancelClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.preventDefault();
    closeDialog();
  }

  const api = new DevicesApi(GetDefaultApiConfig());

  const handleRemoveClick = async (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.preventDefault();
    closeDialog();
    setRemoving(true);
  }

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
  </React.Fragment>
};

const DashboardDeviceDialog = withStyles(styles)(_DashboardDeviceDialog);

export default DashboardDeviceDialog;
