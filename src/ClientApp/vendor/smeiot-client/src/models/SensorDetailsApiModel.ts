/* tslint:disable */
/* eslint-disable */
/**
 * SMEIoT API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
import {
    NumberTimeSeriesApiModel,
    NumberTimeSeriesApiModelFromJSON,
    NumberTimeSeriesApiModelFromJSONTyped,
    NumberTimeSeriesApiModelToJSON,
} from './';

/**
 * 
 * @export
 * @interface SensorDetailsApiModel
 */
export interface SensorDetailsApiModel {
    /**
     * 
     * @type {Array<NumberTimeSeriesApiModel>}
     * @memberof SensorDetailsApiModel
     */
    data?: Array<NumberTimeSeriesApiModel>;
    /**
     * 
     * @type {string}
     * @memberof SensorDetailsApiModel
     */
    startedAt?: string;
    /**
     * 
     * @type {string}
     * @memberof SensorDetailsApiModel
     */
    sensorName?: string;
    /**
     * 
     * @type {string}
     * @memberof SensorDetailsApiModel
     */
    deviceName?: string;
}

export function SensorDetailsApiModelFromJSON(json: any): SensorDetailsApiModel {
    return SensorDetailsApiModelFromJSONTyped(json, false);
}

export function SensorDetailsApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): SensorDetailsApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'data': !exists(json, 'data') ? undefined : ((json['data'] as Array<any>).map(NumberTimeSeriesApiModelFromJSON)),
        'startedAt': !exists(json, 'startedAt') ? undefined : json['startedAt'],
        'sensorName': !exists(json, 'sensorName') ? undefined : json['sensorName'],
        'deviceName': !exists(json, 'deviceName') ? undefined : json['deviceName'],
    };
}

export function SensorDetailsApiModelToJSON(value?: SensorDetailsApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'data': value.data === undefined ? undefined : ((value.data as Array<any>).map(NumberTimeSeriesApiModelToJSON)),
        'startedAt': value.startedAt,
        'sensorName': value.sensorName,
        'deviceName': value.deviceName,
    };
}

