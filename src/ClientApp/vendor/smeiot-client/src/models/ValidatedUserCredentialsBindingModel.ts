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
/**
 * 
 * @export
 * @interface ValidatedUserCredentialsBindingModel
 */
export interface ValidatedUserCredentialsBindingModel {
    /**
     * 
     * @type {string}
     * @memberof ValidatedUserCredentialsBindingModel
     */
    userName: string;
    /**
     * 
     * @type {string}
     * @memberof ValidatedUserCredentialsBindingModel
     */
    password: string;
}

export function ValidatedUserCredentialsBindingModelFromJSON(json: any): ValidatedUserCredentialsBindingModel {
    return ValidatedUserCredentialsBindingModelFromJSONTyped(json, false);
}

export function ValidatedUserCredentialsBindingModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): ValidatedUserCredentialsBindingModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'userName': json['userName'],
        'password': json['password'],
    };
}

export function ValidatedUserCredentialsBindingModelToJSON(value?: ValidatedUserCredentialsBindingModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'userName': value.userName,
        'password': value.password,
    };
}


