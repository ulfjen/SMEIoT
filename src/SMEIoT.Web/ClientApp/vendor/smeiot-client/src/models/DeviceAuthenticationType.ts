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

/**
 * 
 * @export
 * @enum {string}
 */
export enum DeviceAuthenticationType {
    NUMBER_0 = 0
}

export function DeviceAuthenticationTypeFromJSON(json: any): DeviceAuthenticationType {
    return DeviceAuthenticationTypeFromJSONTyped(json, false);
}

export function DeviceAuthenticationTypeFromJSONTyped(json: any, ignoreDiscriminator: boolean): DeviceAuthenticationType {
    return json as DeviceAuthenticationType;
}

export function DeviceAuthenticationTypeToJSON(value?: DeviceAuthenticationType | null): any {
    return value as any;
}

