import Container from '@material-ui/core/Container';
import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import DashboardFrame from "./DashboardFrame";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from '@material-ui/icons/Close';
import { Link, RouteComponentProps } from '@reach/router';

const styles = ({palette, spacing, transitions, zIndex, mixins, breakpoints}: Theme) => createStyles({
  container: {
  },
  list: {
    backgroundColor: "#ffffff"
  },
  filterBar: {
    backgroundColor: "#ffffff",
    display: 'flex',
    flexWrap: 'wrap',
    '& > *': {
      margin: spacing(0.5),
    },
    marginBottom: spacing(2)
  },
  paper: {
    padding: spacing(2),
    display: 'flex',
    overflow: 'auto',
    flexDirection: 'column',
  },
  fixedHeight: {
    height: 240,
  },
  usersMenu: {},
  usersMenuDeleteItem: {
    color: palette.error.main
  },
  avatar: {},
  cardContent: {}
});


export interface IDashboardNewSensorProps extends RouteComponentProps, WithStyles<typeof styles> {
  
}


const _DashboardNewSensor: React.FunctionComponent<IDashboardNewSensorProps> = ({ classes }) => {
  const [candidates, setCandidates] = React.useState<string[]>([]);

  const requestCandidates = async () => {
    // let candidatesApiModel = await new SensorsApi(GetDefaultApiConfig()).apiSensorsCandidatesGet();
    setCandidates([]);
  };

  React.useEffect(() => {
    // @ts-ignore
    if (window.SMEIoTPreRendered && window.SMEIoTPreRendered["user"]) {
      // @ts-ignore
     // saveUser(window.SMEIoTPreRendered["user"]);
    } else {
      requestCandidates();
    }
  }, []);

  return <DashboardFrame 
    drawer
    title={`Create a new sensor`} direction="ltr" toolbarRight={
    <IconButton
      edge="end"
      color="inherit"
      aria-label="close this action"
      to={"/dashboard/sensors"}
      component={Link}
    >
      <CloseIcon/>
    </IconButton>
  }
  content={
    <Container maxWidth="lg" className={classes.container}>
      <p>{candidates}</p>
    </Container>
  } />;
};

const DashboardNewSensor = withStyles(styles)(_DashboardNewSensor);

export default DashboardNewSensor;
