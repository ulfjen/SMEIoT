import clsx from 'clsx';
import CssBaseline from '@material-ui/core/CssBaseline';
import Drawer from '@material-ui/core/Drawer';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import Divider from '@material-ui/core/Divider';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import * as React from "react";
import DrawerList from "./DrawerList";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Hidden from "@material-ui/core/Hidden";
import CloseIcon from '@material-ui/icons/Close';
import UserAvatarMenu from "../components/UserAvatarMenu";
import { BasicUserApiModel } from 'smeiot-client';
import moment from 'moment';

const drawerWidth = 240;

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  root: {
    display: 'flex',
  },
  toolbar: mixins.toolbar,
  toolbarIcon: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    padding: '0 8px',
    ...mixins.toolbar,
  },
  appBar: {
    zIndex: zIndex.drawer + 1,
    transition: transitions.create(['width', 'margin'], {
      easing: transitions.easing.sharp,
      duration: transitions.duration.leavingScreen,
    }),
  },
  menuButton: {
    marginRight: 36,
  },
  menuButtonHidden: {
    display: 'none',
  },
  title: {
    flexGrow: 1,
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  appBarSpacer: mixins.toolbar,
  content: {
    flexGrow: 1,
    height: '100vh',
    overflow: 'auto',
    padding: spacing(3),
    backgroundColor: "#fafafa"
  },

});

export interface IFrameProps extends WithStyles<typeof styles> {
  title: string;
  direction: string;
  content: null | React.ReactNode;
  toolbarRight: null | React.ReactNode;
}

const _Frame: React.FunctionComponent<IFrameProps> = ({classes, title, direction, content, toolbarRight}) => {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  let currentUser: BasicUserApiModel = {
    createdAt: moment().utc().toISOString(),
    roles: [],
    username: ""
  };

  // @ts-ignore
  if (window.SMEIoTPreRendered) {
    // @ts-ignore
    currentUser = window.SMEIoTPreRendered["currentUser"];
  }
  if (toolbarRight === null) {
    toolbarRight = <UserAvatarMenu user={currentUser} />;
  }

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };


  const drawer = <div>
    <div className={classes.toolbarIcon}>
      <IconButton
        color="inherit"
        aria-label="close drawer"
        onClick={handleDrawerToggle}>
        <ChevronLeftIcon/>
      </IconButton>
    </div>
    <Hidden smUp implementation="css">
    <Divider/>
    </Hidden>
    <DrawerList/>
    </div>;


  return <div className={classes.root}>
    <CssBaseline/>
    <AppBar position="fixed" className={classes.appBar}>
      <Toolbar className={classes.toolbar}>
        <Hidden smUp implementation="css">
          <IconButton
            edge="start"
            color="inherit"
            aria-label="open drawer"
            className={classes.menuButton}
            onClick={handleDrawerToggle}
          >
            <MenuIcon/>
          </IconButton>
        </Hidden>

        <Typography component="h1" variant="h6" color="inherit" noWrap className={classes.title}>
          {title}
        </Typography>
        {toolbarRight}
      </Toolbar>
    </AppBar>
    <nav className={classes.drawer} aria-label="mailbox folders">
      <Hidden smUp implementation="js">
        <Drawer
          variant="temporary"
          anchor={direction === 'rtl' ? 'right' : 'left'}
          open={mobileOpen}
          onClose={handleDrawerToggle}
          classes={{
            paper: classes.drawerPaper,
          }}
          ModalProps={{
            keepMounted: true, // Better open performance on mobile.
          }}
        >
          {drawer}
        </Drawer>
      </Hidden>
      <Hidden xsDown implementation="js">
        <Drawer
          classes={{
            paper: classes.drawerPaper,
          }}
          variant="permanent"
          open
        >
          {drawer}
        </Drawer>
      </Hidden>
    </nav>
    <main className={classes.content}>
      <div className={classes.appBarSpacer}/>
      {content}
    </main>
  </div>;
};

const Frame = withStyles(styles)(_Frame);

export default Frame;
