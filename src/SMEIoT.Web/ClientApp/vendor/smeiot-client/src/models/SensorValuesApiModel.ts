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
 * @interface SensorValuesApiModel
 */
export interface SensorValuesApiModel {
    /**
     * 
     * @type {Array<number>}
     * @memberof SensorValuesApiModel
     */
    values?: Array<number>;
    /**
     * 
     * @type {string}
     * @memberof SensorValuesApiModel
     */
    startedAt?: string;
    /**
     * 
     * @type {string}
     * @memberof SensorValuesApiModel
     */
    interval?: string;
}

export function SensorValuesApiModelFromJSON(json: any): SensorValuesApiModel {
    return SensorValuesApiModelFromJSONTyped(json, false);
}

export function SensorValuesApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): SensorValuesApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'values': !exists(json, 'values') ? undefined : json['values'],
        'startedAt': !exists(json, 'startedAt') ? undefined : json['startedAt'],
        'interval': !exists(json, 'interval') ? undefined : json['interval'],
    };
}

export function SensorValuesApiModelToJSON(value?: SensorValuesApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'values': value.values,
        'startedAt': value.startedAt,
        'interval': value.interval,
    };
}


