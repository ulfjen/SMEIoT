import * as React from "react";
import ListItemLine from '../components/ListItemLine';

export interface IDashboardMqttLineRendererProps {
  data: string[];
  index: number;
}

const DashboardMqttLineRenderer: React.FunctionComponent<IDashboardMqttLineRendererProps> = ({ data, index }) => {
  return <ListItemLine line={data[index]} />;
};

export default DashboardMqttLineRenderer;
