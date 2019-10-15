import * as React from "react";
import DashboardIcon from '@material-ui/icons/Dashboard';
import PeopleIcon from '@material-ui/icons/People';
import BarChartIcon from '@material-ui/icons/BarChart';
import ListItemRoutedLink from "../components/ListItemRoutedLink";

const dashboardIcon = <DashboardIcon/>;
const sensorsIcon = <BarChartIcon/>;
const usersIcon = <PeopleIcon/>;

export const mainListItems = <div>
  <ListItemRoutedLink primary="Dashboard" to="/dashboard" icon={dashboardIcon}/>
  <ListItemRoutedLink primary="Sensors" to="/dashboard/sensors" icon={sensorsIcon} />
  <ListItemRoutedLink primary="Users" to="/dashboard/users" icon={usersIcon} />
</div>;

