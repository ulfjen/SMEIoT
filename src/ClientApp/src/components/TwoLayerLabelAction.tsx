import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { TypographyProps } from "@material-ui/core/Typography";

const styles = ({ palette, spacing, typography, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  line: {
    display: "flex",
    alignItems: "center",
  },
  root: {
    display: "inline-block"
  },
  labels: {
    display: "inline-block",
    marginRight: 10,
    fontSize: "1.15rem",
    fontWeight: typography.fontWeightLight,
    fontFamily: typography.fontFamily,
    lineHeight: 1.5,
    "& > span": {
      "&:first-child": {
        paddingLeft: 0
      },
      paddingLeft: 5
    },
  },
  divider: {
    color: palette.divider
  }
});

export interface ITwoLayerLabelActionProps extends WithStyles<typeof styles> {
  first: string;
  second: string;
  firstVariant?: TypographyProps["variant"];
  action?: React.ReactNode;
}

const _TwoLayerLabelAction: React.FunctionComponent<ITwoLayerLabelActionProps & WithStyles<typeof styles>> = ({
  classes, first, second, firstVariant, action
}) => {
  let variant = firstVariant || "inherit";

  return <div className={classes.root}>
    <div className={classes.line}>
      <div className={classes.labels}>
        <span>{first}</span>
        <span className={classes.divider}>/</span>
        <span>{second}</span>
      </div>
      {action}
    </div></div>;
};


const TwoLayerLabelAction = withStyles(styles)(_TwoLayerLabelAction);
export default TwoLayerLabelAction;
