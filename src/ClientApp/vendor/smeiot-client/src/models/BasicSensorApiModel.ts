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
 * @interface BasicSensorApiModel
 */
export interface BasicSensorApiModel {
    /**
     * 
     * @type {string}
     * @memberof BasicSensorApiModel
     */
    sensorName: string;
    /**
     * 
     * @type {string}
     * @memberof BasicSensorApiModel
     */
    status: BasicSensorApiModelStatusEnum;
}

export function BasicSensorApiModelFromJSON(json: any): BasicSensorApiModel {
    return BasicSensorApiModelFromJSONTyped(json, false);
}

export function BasicSensorApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): BasicSensorApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'sensorName': json['sensorName'],
        'status': json['status'],
    };
}

export function BasicSensorApiModelToJSON(value?: BasicSensorApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'sensorName': value.sensorName,
        'status': value.status,
    };
}

/**
* @export
* @enum {string}
*/
export enum BasicSensorApiModelStatusEnum {
    NotRegistered = 'NotRegistered',
    NotConnected = 'NotConnected',
    Connected = 'Connected'
}


