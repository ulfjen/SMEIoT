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
 * @interface UserRolesBindingModel
 */
export interface UserRolesBindingModel {
    /**
     * 
     * @type {Array<string>}
     * @memberof UserRolesBindingModel
     */
    roles?: Array<string>;
}

export function UserRolesBindingModelFromJSON(json: any): UserRolesBindingModel {
    return UserRolesBindingModelFromJSONTyped(json, false);
}

export function UserRolesBindingModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): UserRolesBindingModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'roles': !exists(json, 'roles') ? undefined : json['roles'],
    };
}

export function UserRolesBindingModelToJSON(value?: UserRolesBindingModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'roles': value.roles,
    };
}


