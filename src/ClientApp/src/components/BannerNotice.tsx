import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Link from "@material-ui/core/Link";
import Card from "@material-ui/core/Card";
import CardActionArea from "@material-ui/core/CardActionArea";
import { Link as ReachLink } from "@reach/router";
import { darken, fade } from "@material-ui/core/styles/colorManipulator";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    root: {
      backgroundColor: palette.type === 'light' ? darken(palette.background.paper, 0.1) : fade(palette.background.paper, 0.1)
    },
    action: {
      padding: spacing(2)
    }
  });

export interface IBannerNoticeProps extends WithStyles<typeof styles> {
  children: JSX.Element;
  to?: string | null | undefined;
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
}

const _BannerNotice: React.FunctionComponent<IBannerNoticeProps &
  WithStyles<typeof styles>> = ({ classes, children, to, onClick }) => {
  if (to) {
    return (
      <Link underline="none" component={ReachLink} to={to}>
        <Card className={classes.root}>
          <CardActionArea className={classes.action}>
            {children}
          </CardActionArea>
        </Card>
      </Link>
    );
  } else {
    return (
      <Card className={classes.root}>
        <CardActionArea className={classes.action} onClick={onClick}>
          {children}
        </CardActionArea>
      </Card>
    );
  }
};

const BannerNotice = withStyles(styles)(_BannerNotice);
export default BannerNotice;
