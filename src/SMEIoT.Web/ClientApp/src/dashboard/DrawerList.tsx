import * as React from "react";
import DashboardIcon from '@material-ui/icons/Dashboard';
import PeopleIcon from '@material-ui/icons/People';
import BarChartIcon from '@material-ui/icons/BarChart';
import DeviceHubIcon from '@material-ui/icons/DeviceHub';
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
  users: {
    id: 'dashboard.drawer.users',
    description: 'Used in the left drawer on the dashboard',
    defaultMessage: 'Users',
  }
});

export interface IDrawerListProps {
}

const DrawerList: React.FunctionComponent<IDrawerListProps> = () => {
  const intl = useIntl();

  return <List>
    <ListItemRoutedLink primary={intl.formatMessage(messages.dashboard)} to="/dashboard" icon={<DashboardIcon/>}/>
    <ListItemRoutedLink primary={intl.formatMessage(messages.devices)} to="/dashboard/devices" icon={<DeviceHubIcon/>} />
    <ListItemRoutedLink primary={intl.formatMessage(messages.users)} to="/dashboard/users" icon={<PeopleIcon/>} />
  </List>
};

export default DrawerList;
