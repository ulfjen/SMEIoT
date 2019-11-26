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
 * @interface DeviceApiModel
 */
export interface DeviceApiModel {
    /**
     * 
     * @type {string}
     * @memberof DeviceApiModel
     */
    readonly name?: string;
    /**
     * 
     * @type {string}
     * @memberof DeviceApiModel
     */
    readonly createdAt?: string;
    /**
     * 
     * @type {string}
     * @memberof DeviceApiModel
     */
    readonly updatedAt?: string;
    /**
     * 
     * @type {DeviceAuthenticationType}
     * @memberof DeviceApiModel
     */
    authenticationType?: DeviceAuthenticationType;
    /**
     * 
     * @type {string}
     * @memberof DeviceApiModel
     */
    readonly preSharedKey?: string;
    /**
     * 
     * @type {boolean}
     * @memberof DeviceApiModel
     */
    readonly connected?: boolean;
    /**
     * 
     * @type {string}
     * @memberof DeviceApiModel
     */
    readonly connectedAt?: string;
    /**
     * 
     * @type {string}
     * @memberof DeviceApiModel
     */
    readonly lastMessageAt?: string;
}

export function DeviceApiModelFromJSON(json: any): DeviceApiModel {
    return DeviceApiModelFromJSONTyped(json, false);
}

export function DeviceApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): DeviceApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'name': !exists(json, 'name') ? undefined : json['name'],
        'createdAt': !exists(json, 'createdAt') ? undefined : json['createdAt'],
        'updatedAt': !exists(json, 'updatedAt') ? undefined : json['updatedAt'],
        'authenticationType': !exists(json, 'authenticationType') ? undefined : DeviceAuthenticationTypeFromJSON(json['authenticationType']),
        'preSharedKey': !exists(json, 'preSharedKey') ? undefined : json['preSharedKey'],
        'connected': !exists(json, 'connected') ? undefined : json['connected'],
        'connectedAt': !exists(json, 'connectedAt') ? undefined : json['connectedAt'],
        'lastMessageAt': !exists(json, 'lastMessageAt') ? undefined : json['lastMessageAt'],
    };
}

export function DeviceApiModelToJSON(value?: DeviceApiModel | null): any {
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


