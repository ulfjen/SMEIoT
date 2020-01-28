import * as React from 'react';
import {
  XYPlot,
  XAxis,
  YAxis,
  HorizontalGridLines,
  VerticalGridLines,
  LineSeries,
  Highlight,
  Crosshair
} from 'react-vis';
import { Theme } from '@material-ui/core/styles/createMuiTheme';
import useTheme from '@material-ui/styles/useTheme';
import { darken } from '@material-ui/core/styles/colorManipulator';
import { defineMessages, useIntl } from 'react-intl';
import IDimension from '../models/IDimension';

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

const _InteractiveNumberGraph = (props: any) => {
  const intl = useIntl();
  const { dimension, setDimension, width, height, data } = props;
  const { palette } = useTheme<Theme>();
  const gridLineColor = palette.text.hint;
  const axisStyle = {
    line: {stroke: darken(palette.text.hint, 0.2)},
    ticks: {stroke: palette.text.hint},
    text: {stroke: 'none', fill: palette.text.hint, fontWeight: 600}
  };

  const [crosshairValues, setCrosshairValues] = React.useState<Array<any>>([]);
  const onNearestX = (value:any, {index}:any) => {
    setCrosshairValues([value]);
  }

  return (
      <XYPlot
        width={width}
        height={height}
        xType="time"
        xDomain={dimension && [dimension.left, dimension.right]}
        yDomain={dimension && [dimension.bottom, dimension.top]}
      >
        <HorizontalGridLines style={{stroke: gridLineColor}} />
        <VerticalGridLines style={{stroke: gridLineColor}} />
        <XAxis
          title={intl.formatMessage(messages.xais)}
          style={axisStyle}
        />
        <YAxis title={intl.formatMessage(messages.yais)} style={axisStyle}/>
        <LineSeries
          data={data}
          color={palette.info.light}
          style={{
            strokeLinejoin: 'round',
            strokeWidth: 4
          }}
          onNearestX={onNearestX}
        />
        <Highlight
          onBrushEnd={(area: IDimension) => setDimension(area)}
          onDrag={(area: IDimension) => {
            setDimension({
              bottom: dimension ? dimension.bottom : 0,
              top: dimension ? dimension.top : 0,
              left: area.left,
              right: area.right
            });
          }}
        />
        <Crosshair values={crosshairValues} />
      </XYPlot>
  );
};

export default (_InteractiveNumberGraph);
