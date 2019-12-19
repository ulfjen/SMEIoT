import * as React from "react";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import { emphasize } from "@material-ui/core/styles/colorManipulator";
import { default as Typography, TypographyProps } from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";

const styles = ({ palette, spacing, transitions, zIndex, mixins, breakpoints }: Theme) => createStyles({
  root: {
  }
});

export interface ITwoLayerLabelActionProps extends WithStyles<typeof styles> {
  firstLabel: string;
  secondLabel: string;
  firstLabelVariant?: TypographyProps["variant"];
  actionIcon?: React.ReactNode;
  actionIconDisabled?: boolean;
  actionIconOnClick?: React.MouseEventHandler<HTMLButtonElement>;
}

const _TwoLayerLabelAction: React.FunctionComponent<ITwoLayerLabelActionProps & WithStyles<typeof styles>> = ({
  classes, firstLabel, secondLabel, firstLabelVariant, actionIcon, actionIconDisabled, actionIconOnClick
}) => {
  let variant = firstLabelVariant || "inherit";
  
  return <div className={classes.root}>
    <Typography component="span" variant={variant}>{firstLabel}</Typography> / <Typography component="span">{secondLabel}</Typography>
    {actionIconOnClick && actionIcon ?
      <IconButton aria-label="toggle password visibility" disabled={actionIconDisabled} onClick={actionIconOnClick}>
        {actionIcon}
      </IconButton>: null}
  </div>;
};


const TwoLayerLabelAction = withStyles(styles)(_TwoLayerLabelAction);
export default TwoLayerLabelAction;
