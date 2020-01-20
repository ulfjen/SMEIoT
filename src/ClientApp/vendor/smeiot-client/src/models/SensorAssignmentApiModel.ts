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
/**
 * 
 * @export
 * @interface SensorAssignmentApiModel
 */
export interface SensorAssignmentApiModel {
    /**
     * 
     * @type {string}
     * @memberof SensorAssignmentApiModel
     */
    sensorName: string;
    /**
     * 
     * @type {string}
     * @memberof SensorAssignmentApiModel
     */
    userName: string;
}

export function SensorAssignmentApiModelFromJSON(json: any): SensorAssignmentApiModel {
    return SensorAssignmentApiModelFromJSONTyped(json, false);
}

export function SensorAssignmentApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): SensorAssignmentApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'sensorName': json['sensorName'],
        'userName': json['userName'],
    };
}

export function SensorAssignmentApiModelToJSON(value?: SensorAssignmentApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'sensorName': value.sensorName,
        'userName': value.userName,
    };
}


