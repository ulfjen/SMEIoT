import { SensorDetailsApiModel } from "smeiot-client";
import React from "react";

export interface SensorListing {
  running: Array<SensorDetailsApiModel>;
  setRunning: React.Dispatch<React.SetStateAction<SensorDetailsApiModel[]>>;
  notRegistered: Array<SensorDetailsApiModel>;
  setNotRegistered: React.Dispatch<React.SetStateAction<SensorDetailsApiModel[]>>;
  notConnected: Array<SensorDetailsApiModel>;
  setNotConnected: React.Dispatch<React.SetStateAction<SensorDetailsApiModel[]>>;
}

function useSensorByStatus(): SensorListing {
  const [sensors, setSensors] = React.useState<Array<SensorDetailsApiModel>>([]);
  const [notRegisteredSensors, setNotRegisteredSensors] = React.useState<Array<SensorDetailsApiModel>>([]);
  const [notConnectedSensors, setNotConnectedSensors] = React.useState<Array<SensorDetailsApiModel>>([]);
  
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
