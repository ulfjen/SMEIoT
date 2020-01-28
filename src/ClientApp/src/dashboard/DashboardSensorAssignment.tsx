import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { darken } from '@material-ui/core/styles';
import Grid from "@material-ui/core/Grid";
import Skeleton from "@material-ui/lab/Skeleton";
import Typography from "@material-ui/core/Typography";
import { RouteComponentProps } from "@reach/router";
import { useTitle, useAsync, useDebounce } from 'react-use';
import Snackbar from "@material-ui/core/Snackbar";
import Alert from "@material-ui/lab/Alert";
import {
  defineMessages,
  useIntl,
  FormattedMessage
} from "react-intl";
import {
  SensorDetailsApiModel,
  SensorsApi,
  SensorAssignmentsApi,
  AdminUserApiModelList,
  AdminUserApiModel,
  SensorAssignmentsApiApiSensorsDeviceNameSensorNameUsersGetRequest,
  AdminUsersApi
} from "smeiot-client";
import { GetDefaultApiConfig, UserAvatar } from "../index";
import DashboardFrame from "./DashboardFrame";
import Container from "@material-ui/core/Container";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import AddIcon from "@material-ui/icons/Add";
import { Link as ReachLink } from "@reach/router";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Link from '@material-ui/core/Link';
import NavigateNextIcon from "@material-ui/icons/NavigateNext";
import { AsyncState } from "react-use/lib/useAsync";
import ExpandedCardHeader from "../components/ExpandedCardHeader";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import useMenu from "../helpers/useMenu";
import useModal from "../helpers/useModal";
import DashboardSensorMenu from "./DashboardSensorMenu";
import DashboardSensorDialog from "./DashboardSensorDialog";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import TwoLayerLabelAction from "../components/TwoLayerLabelAction";
import Paper from "@material-ui/core/Paper";
import CardHeader from "@material-ui/core/CardHeader";
import ListItemAvatar from '@material-ui/core/ListItemAvatar';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';
import Avatar from "@material-ui/core/Avatar";
import InfiniteLoader from "react-window-infinite-loader";
import { FixedSizeList } from "react-window";
import List from "@material-ui/core/List";
import RemoveCircleIcon from '@material-ui/icons/RemoveCircle';
import TextField from "@material-ui/core/TextField";
import ProgressIconButton from "../components/ProgressIconButton";
import { ProblemDetails } from "smeiot-client/src";
import ValidationProblemDetails from "../models/ValidationProblemDetails";

const styles = ({ typography, palette, spacing, zIndex }: Theme) => createStyles({
  container: {
  },
  instructions: {
    marginTop: spacing(1),
    marginBottom: spacing(1)
  },
  loadingPanel: {
    height: 200
  },
  column: {
    flexBasis: '33.33%',
  },
  twoColumnSpan: {
    flexBasis: '66.66%',
  },
  helper: {
    borderLeft: `2px solid ${palette.divider}`,
    padding: spacing(1, 2),
  },
  link: {
    color: palette.primary.main,
    textDecoration: 'none',
    '&:hover': {
      textDecoration: 'underline',
    },
  },
  heading: {
    fontSize: typography.pxToRem(15),
  },
  paper: {
    backgroundColor: darken(palette.background.paper, 0.01),
    overflow: "hidden"
  },
  panel: {
    "&:first-child": {
      borderTop: '2px solid red',

    }
  },
  firstPanel: {
    borderTop: `2px solid ${palette.divider}`,
    '&:before': {
      top: 0,
      height: 0,
      display: 'none',
    },
  },
  summary: {
    padding: "0 16px 0 16px",
    backgroundColor: darken(palette.background.paper, 0.01),
  },
  details: {
    backgroundColor: palette.background.paper,
    alignItems: 'center',
  },
  warning: {
    color: palette.text.secondary
  },
  list: {
    marginTop: 20
  },
  addUser: {
    marginTop: 20
  },
  suggestItem: {
    backgroundColor: darken(palette.background.paper, 0.10)
  }
});


export interface IDashboardSensorAssignmentParams {
  deviceName: string;
  sensorName: string;
}

export interface IDashboardSensorAssignmentProps extends RouteComponentProps<IDashboardSensorAssignmentParams>, WithStyles<typeof styles> {

}

const ITEM_SIZE = 72;
const FRAME_PADDING = 24;
const USERS_PER_REQ = 10;

const messages = defineMessages({
  title: {
    id: "dashboard.sensors.assign.title",
    description: "Used as title in the edit sensor assignment page on the dashboard",
    defaultMessage: "Who can see {name}?"
  },
  keyLabel: {
    id: "dashboard.sensors.assign.key_label",
    description: "The label for editing psk key",
    defaultMessage: "Key"
  },
  closeAriaLabel: {
    id: "dashboard.sensors.edit.close",
    description: "The aria label for close action",
    defaultMessage: "Close this action"
  },
  moreAria: {
    id: "dashboard.sensors.edit.more",
    description: "The aria label for more action",
    defaultMessage: "More"
  },
  status: {
    connected: {
      id: "dashboard.sensors.edit.status.connected",
      description: "The status for sensor connected",
      defaultMessage: "Connected"
    },
    notConnected: {
      id: "dashboard.sensors.edit.status.not_connected",
      description: "The status for sensor not connected",
      defaultMessage: "Unknown"
    }
  },
  user: {
    id: "dashboard.sensors.edit.user.title",
    description: "The section title for sensor",
    defaultMessage: "Users"
  },
  role: {
    admin: {
      id: "dashboard.sensors.edit.assign.roles.admin",
      description: "The sub text for sensor",
      defaultMessage: "Admin"
    },
    normal: {
      id: "dashboard.sensors.edit.assign.roles.normal",
      description: "The sub text for sensor",
      defaultMessage: "Normal user"
    }
  },
  searchTitle: {
    id: "dashboard.sensors.edit.assign.role_title",
    description: "The title sensor",
    defaultMessage: "Search here to allow a user to see this sensor"
  },
  notFound: {
    id: "dashboard.sensors.edit.assign.search_not_found",
    description: "Error message",
    defaultMessage: "No matched users."
  }
});

interface SuggestUserState {
  user: AdminUserApiModel;
  adding: boolean;
}

function UnwrapValidationProblemDetails(pd: ValidationProblemDetails): string {
  let errs = [pd.title, pd.detail];
  if (pd.errors) {
    for (let key in pd.errors) {
      errs = errs.concat(pd.errors[key]);
    }
  }

  return errs.filter(e => e !== undefined).join("\n");
}

const _DashboardSensorAssignment: React.FunctionComponent<IDashboardSensorAssignmentProps> = ({
  classes,
  sensorName,
  deviceName,
  navigate
}) => {
  if (!sensorName || !deviceName) {
    throw new Error("No sensor is assigned to the route.");
  }
  const intl = useIntl();
  const title = intl.formatMessage(messages.title, { name: sensorName });
  useTitle(title);

  const aApi = new SensorAssignmentsApi(GetDefaultApiConfig());
  const api = new SensorsApi(GetDefaultApiConfig());
  
  const [q, setQ] = React.useState<string>("");
  // which one is the best approach?
  const [userList, setUserList] = React.useState<Array<SuggestUserState> | null>([]);
  const [deleting, setDeleting] = React.useState<Record<string, boolean>>({})
  const [suggestText, setSuggestText] = React.useState<string>("");
  const [suggesting, setSuggesting] = React.useState<boolean>(false);

  const [menuOpen, anchorEl, openMenu, closeMenu, menuSensorName] = useMenu();
  const [dialogOpen, openDialog, closeDialog, dialogSensorName] = useModal<string>();
  const [users, setUsers] = React.useState<Array<AdminUserApiModel>>([]);
  const [hasNextPage, setHasNextPage] = React.useState<boolean>(true);

  const closeSnackbar = (e: React.SyntheticEvent<Element, Event> | null) => {
    if (e) { e.preventDefault() }
    setSnackbarOpen(false);
  };
  const [snackbarMessage, setSnackbarMessage] = React.useState<string>("");
  const [snackbarOpen, setSnackbarOpen] = React.useState<boolean>(false);

  const handleAddSuggest = React.useCallback(async (e: React.MouseEvent<HTMLButtonElement, MouseEvent>, userName: string) => {
    e.preventDefault();
    if (!userList) { return; }
    setUserList(userList.map(u => {
      if (u.user.userName === userName) {
        return { user: u.user, adding: true };
      } else {
        return u;
      }
    }));
    await aApi.apiSensorsDeviceNameSensorNameUsersPost({
      deviceName, sensorName,
      assignUserSensorBindingModel: {
        userName
      }
    }).then(res => {
      const userIds = users.map(u => u.id);
      if (userIds.indexOf(res.user.id) === -1) {
        setUsers(users.concat(res.user));
      }
      setUserList(userList.filter(u => u.user.userName !== userName));
      return res;
    }).catch(async (res) => {
      const obj = await res.json();
      const err = UnwrapValidationProblemDetails(obj);

      setUserList(userList.map(u => {
        if (u.user.userName === userName) {
          return { user: u.user, adding: false };
        } else {
          return u;
        }
      }));
      setSnackbarMessage(err);
      setSnackbarOpen(true);
    });
  }, [userList, aApi, deviceName, sensorName, users]);

  const [debouncedQ, setDebouncedQ] = React.useState<string>('');
  useDebounce(
    () => {
      setDebouncedQ(q);
    },
    300,
    [q]
  );

  React.useEffect(() => {
    if (debouncedQ.length === 0) {
      setUserList([]);
      setSuggestText("");
      return;
    }
    if (suggesting) { return; }
    (async () => {
      const api = new AdminUsersApi(GetDefaultApiConfig());
      setSuggesting(true);
      setUserList([]);
      setSuggestText("");

      await api.apiAdminUsersSearchGet({
        query: debouncedQ,
        limit: 3
      }).then(res => {
        let hasSuggest = false;
        if (res.total !== 0) {
          const userIds = users.map(u => u.id);
          const suggestList = (userList || []).concat(res.users.map(u => { return {user: u, adding: false} })).filter(u => userIds.indexOf(u.user.id) === -1);
          console.log(users, userIds, suggestList);
          if (suggestList.length !== 0) {
            setUserList(suggestList);
            hasSuggest = true;
          }
        }
        if (!hasSuggest) {
          setSuggestText(intl.formatMessage(messages.notFound));
        }
        setSuggesting(false);
        return res;
      });
    })();
  }, [debouncedQ, users, intl, suggesting, userList]);

  const suggestUsers = (userList: Array<SuggestUserState>) => {
    return userList.map((u, idx) => <ListItem className={classes.suggestItem} key={idx}>
        <ListItemAvatar>
          <Avatar>
            {UserAvatar.getInstance().getSvg(u.user.userName)}
          </Avatar>
        </ListItemAvatar>

        <ListItemText primary={u.user.userName} secondary={u.user.roles.indexOf("Admin") !== -1 ? intl.formatMessage(messages.role.admin) : intl.formatMessage(messages.role.normal)} />
        <ProgressIconButton
          loading={u.adding}
          onClick={(e) => handleAddSuggest(e, u.user.userName) }>
          <AddIcon />
        </ProgressIconButton>
      </ListItem>);
  }

  const disableUserSensor = React.useCallback(async (e: React.MouseEvent<HTMLButtonElement, MouseEvent>, userName: string) => {
    e.preventDefault();
    const update: Record<string, boolean> = {};
    update[userName] = true;
    setDeleting({ ...deleting, ...update });
    await aApi.apiSensorsDeviceNameSensorNameUsersUserNameDelete({
      deviceName, sensorName, userName
    }).then(res => {
      setUsers(users.filter(u => u.userName !== userName));
    }).catch(async res => {
      const update: Record<string, boolean> = {};
      update[userName] = false;
      setDeleting({ ...deleting, ...update });
      const err = UnwrapValidationProblemDetails(await res.json());

      setSnackbarMessage(err);
    });

  }, [aApi, users, deleting, deviceName, sensorName]);

  const state: AsyncState<SensorDetailsApiModel> = useAsync(async () => await api.apiSensorsDeviceNameSensorNameGet({
    deviceName, sensorName
  }).then((res) => {
    return res;
  }));

  const userCount = hasNextPage ? users.length + 1 : users.length;
  const isUserLoaded = (index: number) => !hasNextPage || index < users.length;

  const initialHeight = userCount * ITEM_SIZE;
  const containerRef = React.createRef<HTMLElement>();
  const measureRef = React.createRef<HTMLDivElement>();
  const [width, setWidth] = React.useState(-1);
  const [height, setHeight] = React.useState(initialHeight);
  const measureAvailbleViewport = React.useCallback(() => {
    if (measureRef.current && containerRef.current) {
      const docHeight = containerRef.current.getBoundingClientRect().height;
      const measureRect = measureRef.current.getBoundingClientRect();
      setHeight(Math.min(userCount * ITEM_SIZE, docHeight - measureRect.top - FRAME_PADDING));
      setWidth(measureRect.width);
    }
  }, [measureRef, containerRef, setHeight, setWidth, userCount]);
  React.useEffect(() => measureAvailbleViewport(), [measureAvailbleViewport]);
  const [usersLoading, setUsersLoading] = React.useState<boolean>(false);

  const loadMoreUsers = React.useCallback((startIndex: number, stopIndex: number) => {
    if (usersLoading) {
      return null;
    }
    const req: SensorAssignmentsApiApiSensorsDeviceNameSensorNameUsersGetRequest = {
      offset: startIndex, // we only render things when scroll down so no need to be extra fancy
      limit: USERS_PER_REQ,
      sensorName,
      deviceName
    };
    setUsersLoading(true);
    return aApi.apiSensorsDeviceNameSensorNameUsersGet(req).then((result: AdminUserApiModelList) => {
      if (!result.users) { return; }

      const newUsers = users.concat(result.users);
      setUsers(newUsers);
      setHasNextPage(!(result.total && newUsers.length >= result.total));
      return result;
    }).catch(async (res) => {
      const pd: ProblemDetails = await res.json();
      console.log(pd);
    }).finally(() => {
      setUsersLoading(false);
    });
  }, [users, aApi, deviceName, sensorName, usersLoading]);

  const userItemRenderer = React.useCallback(({ index, style }: {
    index: number,
    style: React.CSSProperties
  }) => {
    return index === users.length ? <ListItem key={-1} style={style}>
      <ListItemAvatar>
        <Skeleton variant="circle" width={40} height={40} />
      </ListItemAvatar>

      <ListItemText disableTypography primary={<Skeleton variant="rect" width={200} height={17} />} secondary={<Skeleton variant="text" />} />
    </ListItem> :
      <ListItem key={index} style={style}>
        <ListItemAvatar>
          <Avatar>
            {UserAvatar.getInstance().getSvg(users[index].userName || "")}
          </Avatar>
        </ListItemAvatar>

        <ListItemText primary={users[index].userName} secondary={users[index].roles.indexOf("Admin") !== -1 ? intl.formatMessage(messages.role.admin) : intl.formatMessage(messages.role.normal)} />
        <ProgressIconButton
          disabled={users[index].roles.indexOf("Admin") !== -1}
          loading={deleting[users[index].userName]}
          onClick={(event) => { disableUserSensor(event, users[index].userName) }}
        >
          <RemoveCircleIcon />
        </ProgressIconButton>
      </ListItem>;
  }, [users, disableUserSensor, intl, deleting]);

  const innerListType = React.useMemo(() => React.forwardRef((props, ref: React.Ref<HTMLUListElement>) => (
    <List ref={ref} {...props} />
  )), []);

  return <DashboardFrame
    title={title}
    drawer
    direction="ltr"
    ref={containerRef}
    toolbarRight={
      <IconButton
        edge="end"
        color="inherit"
        aria-label={intl.formatMessage(messages.closeAriaLabel)}
        to={"../.."}
        component={ReachLink}
      >
        <CloseIcon />
      </IconButton>
    }
    content={
      <Container maxWidth="lg" className={classes.container}>
        <Snackbar anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }} open={snackbarOpen} autoHideDuration={5000} onClose={closeSnackbar}>
          <Alert onClose={closeSnackbar} severity="error">
            {snackbarMessage}
          </Alert>
        </Snackbar>
        <DashboardSensorMenu
          open={menuOpen}
          anchorEl={anchorEl}
          sensorName={menuSensorName}
          closeMenu={closeMenu}
          navigate={navigate}
          openDialog={openDialog}
        />
        <DashboardSensorDialog
          open={dialogOpen}
          deviceName={deviceName}
          sensorName={dialogSensorName}
          closeDialog={closeDialog}
          navigate={navigate}
        />
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />} aria-label="breadcrumb">
              <Link component={ReachLink} color="inherit" to="../..">
                <FormattedMessage
                  id="dashboard.devices.edit.breadcrumb.devices"
                  description="The label at the breadcrumb for devices"
                  defaultMessage="Devices"
                />
              </Link>
              <Link component={ReachLink} color="inherit" to="..">
                {deviceName}
              </Link>
              <Typography color="textPrimary">
                <FormattedMessage
                  id="dashboard.devices.edit.breadcrumb.sensor_assignment"
                  description="The label at the breadcrumb for editing sensors's assignment"
                  defaultMessage="Sensor Assignment"
                />
              </Typography>
            </Breadcrumbs>
          </Grid>
          <Grid item xs={12}>
            <Card>
              <ExpandedCardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={30} /> : state.value && <TwoLayerLabelAction greyoutFirst first={deviceName} second={sensorName} />}
                action={!state.loading &&
                  <IconButton
                    aria-label={intl.formatMessage(messages.moreAria)}
                    onClick={(e) => openMenu(e.currentTarget, state.value ? state.value.sensorName : "")}
                  >
                    <MoreVertIcon />
                  </IconButton>
                }
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : <React.Fragment>
                  <Typography variant="body2" color="textSecondary">
                    <FormattedMessage
                      id="dashboard.sensors.assign.instructions"
                      description="The instruction for sensor's assignment."
                      defaultMessage="You can configure who can see this sensor. All administators can see this sensor."
                    />
                  </Typography>
                  
                  <div className={classes.addUser}>
                    <TextField 
                      fullWidth
                      label={intl.formatMessage(messages.searchTitle)}
                      type="search"
                      value={q}
                      variant="outlined"
                      onChange={(e) => { setQ(e.currentTarget.value); }}
                    />
                    {suggesting ? <List>
                      <ListItem className={classes.suggestItem}>
                        <ListItemAvatar>
                          <Skeleton variant="circle" width={40} height={40} />
                        </ListItemAvatar>

                        <ListItemText disableTypography primary={<Skeleton variant="rect" width={200} height={24} />} secondary={<Skeleton variant="text" />} />
                      </ListItem> 
                    </List> : userList && userList.length !== 0 ? <List>
                      {suggestUsers(userList)}
                    </List> : suggestText}
                  </div>
                </React.Fragment>}
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>
              <CardHeader
                title={state.loading ? <Skeleton variant="rect" width={240} height={26} /> : intl.formatMessage(messages.user)}
                titleTypographyProps={{ color: "secondary", variant: "h6" }}
              />
              <CardContent>
                {state.loading ? <div><Skeleton variant="text" /><Skeleton variant="text" /><Skeleton variant="text" /></div> : 
                <div>
                  <div ref={measureRef}>
                    <Typography variant="body2" color="textSecondary" component="p">
                      <FormattedMessage
                        id="dashboard.sensors.edit.user.instruction"
                        description="The instruction for user cards."
                        defaultMessage="These users are able to see the sensor graph. Administrators are priviliedged to see all graphs."
                      />
                    </Typography>
                    <div>
                      <InfiniteLoader
                        isItemLoaded={isUserLoaded}
                        itemCount={userCount}
                        loadMoreItems={loadMoreUsers}
                      >
                        {({ onItemsRendered, ref }) => (
                          <FixedSizeList
                            className={classes.list}
                            height={Math.max(height, ITEM_SIZE * Math.min(userCount, 5))}
                            width={width}
                            itemCount={userCount}
                            itemSize={ITEM_SIZE}
                            onItemsRendered={onItemsRendered}
                            ref={ref}
                            innerElementType={innerListType}
                          >
                            {userItemRenderer}
                          </FixedSizeList>
                        )}
                      </InfiniteLoader>
                    </div>
                  </div>
                </div>}
              </CardContent>
            </Paper>
          </Grid>
        </Grid>
      </Container>} />;
};

const DashboardSensorAssignment = withStyles(styles)(_DashboardSensorAssignment);

export default DashboardSensorAssignment;
