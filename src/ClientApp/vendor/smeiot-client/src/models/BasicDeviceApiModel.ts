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
} from './';

/**
 * 
 * @export
 * @interface BasicDeviceApiModel
 */
export interface BasicDeviceApiModel {
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly name: string;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly createdAt: string;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly updatedAt: string;
    /**
     * 
     * @type {DeviceAuthenticationType}
     * @memberof BasicDeviceApiModel
     */
    authenticationType: DeviceAuthenticationType;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly preSharedKey: string;
    /**
     * 
     * @type {boolean}
     * @memberof BasicDeviceApiModel
     */
    readonly connected: boolean;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly connectedAt: string | null;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly lastMessageAt: string | null;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly mqttHost: string;
    /**
     * 
     * @type {number}
     * @memberof BasicDeviceApiModel
     */
    readonly mqttPort: number;
    /**
     * 
     * @type {string}
     * @memberof BasicDeviceApiModel
     */
    readonly mqttTopicPrefix: string;
}

export function BasicDeviceApiModelFromJSON(json: any): BasicDeviceApiModel {
    return BasicDeviceApiModelFromJSONTyped(json, false);
}

export function BasicDeviceApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): BasicDeviceApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
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

export function BasicDeviceApiModelToJSON(value?: BasicDeviceApiModel | null): any {
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


