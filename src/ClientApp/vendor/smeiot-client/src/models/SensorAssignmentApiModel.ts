// tslint:disable
// eslint-disable
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
    AdminUserApiModel,
    AdminUserApiModelFromJSON,
    AdminUserApiModelFromJSONTyped,
    AdminUserApiModelToJSON,
} from './';

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
     * @type {AdminUserApiModel}
     * @memberof SensorAssignmentApiModel
     */
    user: AdminUserApiModel;
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
        'user': AdminUserApiModelFromJSON(json['user']),
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
        'user': AdminUserApiModelToJSON(value.user),
    };
}


