import * as React from 'react';
import {curveCatmullRom} from 'd3-shape';

import {
  XYPlot,
  XAxis,
  YAxis,
  HorizontalGridLines,
  VerticalGridLines,
  LineSeries
} from 'react-vis';

export default function NumberGraph(props: any) {
  return (
      <XYPlot width={600} height={300} xType="time">
        <HorizontalGridLines style={{stroke: '#B7E9ED'}} />
        <VerticalGridLines style={{stroke: '#B7E9ED'}} />
        <XAxis
            title="Time"
            style={{
              line: {stroke: '#ADDDE1'},
              ticks: {stroke: '#ADDDE1'},
              text: {stroke: 'none', fill: '#6b6b76', fontWeight: 600}
            }}
        />
        <YAxis title="Value" />
        <LineSeries
            className="first-series"
            data={props.data}
            style={{
              strokeLinejoin: 'round',
              strokeWidth: 4
            }}
        />
        {/* <LineSeries className="second-series" data={null} />
        <LineSeries
            className="third-series"
            curve={'curveMonotoneX'}
            data={[{x: 1, y: 10}, {x: 2, y: 4}, {x: 3, y: 2}, {x: 4, y: 15}]}
            strokeDasharray="7, 3"
        />
        <LineSeries
            className="fourth-series"
            curve={curveCatmullRom.alpha(0.5)}
            data={[{x: 1, y: 7}, {x: 2, y: 11}, {x: 3, y: 9}, {x: 4, y: 2}]}
        /> */}
      </XYPlot>
  );
}
