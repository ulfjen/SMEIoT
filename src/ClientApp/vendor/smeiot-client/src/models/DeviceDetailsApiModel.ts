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
    DeviceAuthenticationType,
    DeviceAuthenticationTypeFromJSON,
    DeviceAuthenticationTypeFromJSONTyped,
    DeviceAuthenticationTypeToJSON,
    SensorDetailsApiModel,
    SensorDetailsApiModelFromJSON,
    SensorDetailsApiModelFromJSONTyped,
    SensorDetailsApiModelToJSON,
} from './';

/**
 * 
 * @export
 * @interface DeviceDetailsApiModel
 */
export interface DeviceDetailsApiModel {
    /**
     * 
     * @type {Array<SensorDetailsApiModel>}
     * @memberof DeviceDetailsApiModel
     */
    readonly sensors: Array<SensorDetailsApiModel>;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly name: string;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly createdAt: string;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly updatedAt: string;
    /**
     * 
     * @type {DeviceAuthenticationType}
     * @memberof DeviceDetailsApiModel
     */
    authenticationType: DeviceAuthenticationType;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly preSharedKey: string;
    /**
     * 
     * @type {boolean}
     * @memberof DeviceDetailsApiModel
     */
    readonly connected: boolean;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly connectedAt: string | null;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly lastMessageAt: string | null;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly mqttHost: string;
    /**
     * 
     * @type {number}
     * @memberof DeviceDetailsApiModel
     */
    readonly mqttPort: number;
    /**
     * 
     * @type {string}
     * @memberof DeviceDetailsApiModel
     */
    readonly mqttTopicPrefix: string;
}

export function DeviceDetailsApiModelFromJSON(json: any): DeviceDetailsApiModel {
    return DeviceDetailsApiModelFromJSONTyped(json, false);
}

export function DeviceDetailsApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): DeviceDetailsApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'sensors': ((json['sensors'] as Array<any>).map(SensorDetailsApiModelFromJSON)),
        'name': json['name'],
        'createdAt': json['createdAt'],
        'updatedAt': json['updatedAt'],
        'authenticationType': DeviceAuthenticationTypeFromJSON(json['authenticationType']),
        'preSharedKey': json['preSharedKey'],
        'connected': json['connected'],
        'connectedAt': json['connectedAt'],
        'lastMessageAt': json['lastMessageAt'],
        'mqttHost': json['mqttHost'],
        'mqttPort': json['mqttPort'],
        'mqttTopicPrefix': json['mqttTopicPrefix'],
    };
}

export function DeviceDetailsApiModelToJSON(value?: DeviceDetailsApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'authenticationType': DeviceAuthenticationTypeToJSON(value.authenticationType),
    };
}


