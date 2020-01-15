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
import { AsyncState } from "react-use/lib/useAsync";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";

const styles = ({ typography, palette, spacing }: Theme) => createStyles({
  removeAction: {
    color: palette.error.main
  },
});

export interface IDashboardDeviceMenuProps extends WithStyles<typeof styles> {
  open: boolean;
  anchorEl: HTMLElement | null;
  closeMenu: () => void;
  deviceName?: string;
  hideConfigureItem?: boolean;
  pathPrefix?: string;
  navigate?: NavigateFn;
  openDialog: (value: string) => void;
}

const messages = defineMessages({
});

const _DashboardDeviceMenu: React.FunctionComponent<IDashboardDeviceMenuProps> = ({
  classes, open, anchorEl, closeMenu, deviceName, navigate, hideConfigureItem, openDialog, pathPrefix
}) => {
  const intl = useIntl();
  if (!pathPrefix) {
    pathPrefix = ".";
  }

  const onCloseClick = (e: React.MouseEvent<HTMLUListElement, MouseEvent>) => {
    e.preventDefault();
    closeMenu();
  }

  const configureOnClick = (e: React.MouseEvent<HTMLLIElement, MouseEvent>) => {
    e.preventDefault();
    closeMenu();
    navigate && navigate(`${pathPrefix}/${deviceName}`);
  }

  const credentialsOnClick = (e: React.MouseEvent<HTMLLIElement, MouseEvent>) => {
    e.preventDefault();
    closeMenu();
    navigate && navigate(`${pathPrefix}/${deviceName}/credentials`);
  }

  const removeOnClick = (e: React.MouseEvent<HTMLLIElement, MouseEvent>) => {
    e.preventDefault();
    closeMenu();
    if (deviceName) { openDialog(deviceName) }
  }


  return <Menu
      anchorEl={anchorEl}
      keepMounted
      open={open}
      onClose={onCloseClick}
    >
      {!hideConfigureItem && <MenuItem
        button
        onClick={configureOnClick}
      >
        <FormattedMessage
          id="dashboard.components.device_menu.configure"
          description="The action in device menu."
          defaultMessage="Configure"
        />
      </MenuItem>
      }
      <MenuItem
        button
        onClick={credentialsOnClick}
      >
        <FormattedMessage
          id="dashboard.components.device_menu.configure_credentials"
          description="The action in device menu."
          defaultMessage="Credentials"
        />
      </MenuItem>
      <MenuItem
        button
        className={classes.removeAction}
        onClick={removeOnClick}
      >
        <FormattedMessage
          id="dashboard.components.device_menu.remove"
          description="The action in device menu."
          defaultMessage="Remove"
        />
      </MenuItem>
    </Menu>;
};

const DashboardDeviceMenu = withStyles(styles)(_DashboardDeviceMenu);

export default DashboardDeviceMenu;
