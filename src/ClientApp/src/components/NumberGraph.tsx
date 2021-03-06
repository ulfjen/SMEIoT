import * as React from 'react';
import {
  XYPlot,
  XAxis,
  YAxis,
  HorizontalGridLines,
  VerticalGridLines,
  LineSeries
} from 'react-vis';
import { Theme } from '@material-ui/core/styles/createMuiTheme';
import useTheme from '@material-ui/styles/useTheme';
import { darken } from '@material-ui/core/styles/colorManipulator';
import { defineMessages, useIntl } from 'react-intl';

const messages = defineMessages({
  xais: {
    id: "component.number_graph.xais",
    description: "Used as axis title in the number graph component",
    defaultMessage: "Time"
  },
  yais: {
    id: "component.number_graph.yais",
    description: "Used as axis title in the number graph component",
    defaultMessage: "Value"
  }
})

const _NumberGraph = (props: any) => {
  const intl = useIntl();
  const {palette} = useTheme<Theme>();
  const gridLineColor = palette.text.hint;
  const axisStyle = {
    line: {stroke: darken(palette.text.hint, 0.2)},
    ticks: {stroke: palette.text.hint},
    text: {stroke: 'none', fill: palette.text.hint, fontWeight: 600}
  };
  return (
      <XYPlot width={props.width} height={props.height} xType="time">
        <HorizontalGridLines style={{stroke: gridLineColor}} />
        <VerticalGridLines style={{stroke: gridLineColor}} />
        <XAxis
          title={intl.formatMessage(messages.xais)}
          style={axisStyle}
        />
        <YAxis title={intl.formatMessage(messages.yais)} style={axisStyle}/>
        <LineSeries
          data={props.data}
          color={palette.info.light}
          style={{
            strokeLinejoin: 'round',
            strokeWidth: 4
          }}
        />
      </XYPlot>
  );
};

export default (_NumberGraph);
