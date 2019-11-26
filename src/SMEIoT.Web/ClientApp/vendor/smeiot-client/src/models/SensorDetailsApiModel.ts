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
    SensorValuesApiModel,
    SensorValuesApiModelFromJSON,
    SensorValuesApiModelFromJSONTyped,
    SensorValuesApiModelToJSON,
} from './';

/**
 * 
 * @export
 * @interface SensorDetailsApiModel
 */
export interface SensorDetailsApiModel {
    /**
     * 
     * @type {SensorValuesApiModel}
     * @memberof SensorDetailsApiModel
     */
    values?: SensorValuesApiModel;
    /**
     * 
     * @type {string}
     * @memberof SensorDetailsApiModel
     */
    sensorName?: string;
}

export function SensorDetailsApiModelFromJSON(json: any): SensorDetailsApiModel {
    return SensorDetailsApiModelFromJSONTyped(json, false);
}

export function SensorDetailsApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): SensorDetailsApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'values': !exists(json, 'values') ? undefined : SensorValuesApiModelFromJSON(json['values']),
        'sensorName': !exists(json, 'sensorName') ? undefined : json['sensorName'],
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
        
        'values': SensorValuesApiModelToJSON(value.values),
        'sensorName': value.sensorName,
    };
}


