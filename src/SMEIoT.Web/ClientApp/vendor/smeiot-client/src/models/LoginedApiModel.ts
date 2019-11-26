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
 * @interface LoginedApiModel
 */
export interface LoginedApiModel {
    /**
     * 
     * @type {string}
     * @memberof LoginedApiModel
     */
    returnUrl?: string;
}

export function LoginedApiModelFromJSON(json: any): LoginedApiModel {
    return LoginedApiModelFromJSONTyped(json, false);
}

export function LoginedApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): LoginedApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'returnUrl': !exists(json, 'returnUrl') ? undefined : json['returnUrl'],
    };
}

export function LoginedApiModelToJSON(value?: LoginedApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'returnUrl': value.returnUrl,
    };
}


