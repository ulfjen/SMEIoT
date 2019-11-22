import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Link from "@material-ui/core/Link";
import Card from "@material-ui/core/Card";
import CardActionArea from "@material-ui/core/CardActionArea";
import { Link as ReachLink } from "@reach/router";

const styles = ({ palette, transitions }: Theme) =>
  createStyles({
    root: {
      backgroundColor: "#eeeeee",
    }
  });

export interface IBannerNoticeProps extends WithStyles<typeof styles> {
  children: JSX.Element;
  to: string | null | undefined;
}

const _BannerNotice: React.FunctionComponent<IBannerNoticeProps &
  WithStyles<typeof styles>> = ({ classes, children, to }) => {
  if (to) {
    return (
      <Link underline="none" component={ReachLink} to={to}>
        <Card className={classes.root}>
          <CardActionArea>
            {children}
          </CardActionArea>
        </Card>
      </Link>
    );
  } else {
    return (
      <Card className={classes.root}>
        <CardActionArea>
          {children}
        </CardActionArea>
      </Card>
    );
  }
};

const BannerNotice = withStyles(styles)(_BannerNotice);
export default BannerNotice;
