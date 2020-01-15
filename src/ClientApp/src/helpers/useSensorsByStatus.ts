import { BasicSensorApiModel } from "smeiot-client";
import React from "react";

export interface SensorListing {
  running: Array<BasicSensorApiModel>;
  setRunning: React.Dispatch<React.SetStateAction<BasicSensorApiModel[]>>;
  notRegistered: Array<BasicSensorApiModel>;
  setNotRegistered: React.Dispatch<React.SetStateAction<BasicSensorApiModel[]>>;
  notConnected: Array<BasicSensorApiModel>;
  setNotConnected: React.Dispatch<React.SetStateAction<BasicSensorApiModel[]>>;
}

function useSensorByStatus(): SensorListing {
  const [sensors, setSensors] = React.useState<Array<BasicSensorApiModel>>([]);
  const [notRegisteredSensors, setNotRegisteredSensors] = React.useState<Array<BasicSensorApiModel>>([]);
  const [notConnectedSensors, setNotConnectedSensors] = React.useState<Array<BasicSensorApiModel>>([]);
  
  return {
    running: sensors, 
    setRunning: setSensors,
    notRegistered: notRegisteredSensors,
    setNotRegistered: setNotRegisteredSensors,
    notConnected: notConnectedSensors,
    setNotConnected: setNotConnectedSensors
  }
};

export default useSensorByStatus;
