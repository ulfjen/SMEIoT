import * as React from "react";
import DashboardIcon from '@material-ui/icons/Dashboard';
import PeopleIcon from '@material-ui/icons/People';
import BarChartIcon from '@material-ui/icons/BarChart';
import ListItemLink from "../components/ListItemLink";

const dashboardIcon = <DashboardIcon/>;
const sensorsIcon = <BarChartIcon/>;
const usersIcon = <PeopleIcon/>;

export const mainListItems = <div>
    <ListItemLink primary="Dashboard" to="/dashboard" icon={dashboardIcon}/>
    <ListItemLink primary="Sensors" to="/dashboard/sensors" icon={sensorsIcon} />
    <ListItemLink primary="Users" to="/dashboard/users" icon={usersIcon} />
</div>;

