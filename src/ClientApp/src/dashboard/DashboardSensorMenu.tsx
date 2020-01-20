import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import {
  FormattedMessage
} from "react-intl";
import { NavigateFn } from "@reach/router";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";

const styles = ({ typography, palette, spacing }: Theme) => createStyles({
  removeAction: {
    color: palette.error.main
  },
});

export interface IDashboardSensorMenuProps extends WithStyles<typeof styles> {
  open: boolean;
  anchorEl: HTMLElement | null;
  closeMenu: () => void;
  sensorName?: string;
  pathPrefix?: string;
  navigate?: NavigateFn;
  openDialog: (value: string) => void;
}

const _DashboardSensorMenu: React.FunctionComponent<IDashboardSensorMenuProps> = ({
  classes, open, anchorEl, closeMenu, sensorName, navigate, openDialog, pathPrefix
}) => {
  if (!pathPrefix) {
    pathPrefix = ".";
  }

  const onCloseClick = (e: React.MouseEvent<HTMLUListElement, MouseEvent>) => {
    e.preventDefault();
    closeMenu();
  }

  const removeOnClick = (e: React.MouseEvent<HTMLLIElement, MouseEvent>) => {
    e.preventDefault();
    closeMenu();
    if (sensorName) { openDialog(sensorName); }
  }

  return <Menu
      anchorEl={anchorEl}
      keepMounted
      open={open}
      onClose={onCloseClick}
    >
      <MenuItem
        button
        className={classes.removeAction}
        onClick={removeOnClick}
      >
        <FormattedMessage
          id="dashboard.components.sensor_menu.remove"
          description="The action in sensor menu."
          defaultMessage="Remove"
        />
      </MenuItem>
    </Menu>;
};

const DashboardSensorMenu = withStyles(styles)(_DashboardSensorMenu);

export default DashboardSensorMenu;
