import * as React from "react";
import clsx from "clsx";
import { WithStyles } from "@material-ui/styles/withStyles";
import createStyles from "@material-ui/styles/createStyles";
import { Theme } from "@material-ui/core/styles/createMuiTheme";
import withStyles from "@material-ui/core/styles/withStyles";
import Typography from "@material-ui/core/Typography";
import { FormattedMessage, defineMessages, useIntl } from "react-intl";

const styles = ({ palette, spacing }: Theme) =>
  createStyles({
    root: {
      display: "flex",
      width: 350,
      color: palette.text.secondary
    },
    text: {
      flex: 1
    }
  });

export interface ILoadFactors extends WithStyles<typeof styles> {
  min1?: number | null;
  min5?: number | null;
  min15?: number | null;
  className?: string;
}

const messages = defineMessages({
  kb: {
    id: "components.load_factors.kb",
    descrption: "kb/s",
    defaultMessage: "{num} KB/s"
  }
});

const _LoadFactors: React.FunctionComponent<ILoadFactors> = ({ classes, className, min1, min5, min15 }) => {
  const intl = useIntl();
  
  return (
    <div className={clsx(classes.root, className)}>
      <Typography component="span" className={classes.text}>
        <FormattedMessage
          id="components.load_factors.label"
          description="Label for the load factors component."
          defaultMessage="Load:"
        />
      </Typography>

      <Typography component="span" className={classes.text}>
        {min1 && intl.formatMessage(messages.kb, { num: (min1 / 1024.0).toFixed(2) })}
      </Typography>

      <Typography component="span" className={classes.text}>
        {min5 && intl.formatMessage(messages.kb, { num: (min5 / 1024.0).toFixed(2) })}
      </Typography>

      <Typography component="span" className={classes.text}>
        {min15 && intl.formatMessage(messages.kb, { num: (min15 / 1024.0).toFixed(2) })}
      </Typography>
    </div>
  );
};

const LoadFactors = withStyles(styles)(_LoadFactors);

export default LoadFactors;
