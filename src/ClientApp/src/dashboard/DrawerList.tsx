import * as React from "react";
import DashboardIcon from '@material-ui/icons/Dashboard';
import PeopleIcon from '@material-ui/icons/People';
import DynamicFeedIcon from '@material-ui/icons/DynamicFeed';
import DeviceHubIcon from '@material-ui/icons/DeviceHub';
import SettingsIcon from '@material-ui/icons/Settings';
import ListItemRoutedLink from "../components/ListItemRoutedLink";
import List from '@material-ui/core/List';
import {defineMessages, useIntl} from 'react-intl';

const messages = defineMessages({
  dashboard: {
    id: 'dashboard.drawer.dashboard',
    description: 'Used in the left drawer on the dashboard',
    defaultMessage: 'Dashboard',
  },
  devices: {
    id: 'dashboard.drawer.devices',
    description: 'Used in the left drawer on the dashboard',
    defaultMessage: 'Devices',
  },
  sensors: {
    id: 'dashboard.drawer.sensors',
    description: 'Used in the left drawer on the dashboard',
    defaultMessage: 'Sensors',
  },
  users: {
    id: 'dashboard.drawer.users',
    description: 'Used in the left drawer on the dashboard',
    defaultMessage: 'Users',
  },
  settings: {
    id: 'dashboard.drawer.settings',
    description: 'Used in the left drawer on the dashboard',
    defaultMessage: 'Settings',
  }
});

const DrawerList: React.FunctionComponent = () => {
  const intl = useIntl();

  return <List>
    <ListItemRoutedLink primary={intl.formatMessage(messages.dashboard)} to="/dashboard" icon={<DashboardIcon/>}/>
    <ListItemRoutedLink primary={intl.formatMessage(messages.devices)} to="/dashboard/devices" icon={<DeviceHubIcon/>} />
    <ListItemRoutedLink primary={intl.formatMessage(messages.sensors)} to="/dashboard/sensors" icon={<DynamicFeedIcon/>} />
    <ListItemRoutedLink primary={intl.formatMessage(messages.users)} to="/dashboard/users" icon={<PeopleIcon/>} />
    <ListItemRoutedLink primary={intl.formatMessage(messages.settings)} to="/dashboard/settings" icon={<SettingsIcon/>} />
  </List>
};

export default DrawerList;
