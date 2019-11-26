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
 * @interface SensorCandidatesApiModel
 */
export interface SensorCandidatesApiModel {
    /**
     * 
     * @type {Array<string>}
     * @memberof SensorCandidatesApiModel
     */
    names?: Array<string>;
}

export function SensorCandidatesApiModelFromJSON(json: any): SensorCandidatesApiModel {
    return SensorCandidatesApiModelFromJSONTyped(json, false);
}

export function SensorCandidatesApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): SensorCandidatesApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'names': !exists(json, 'names') ? undefined : json['names'],
    };
}

export function SensorCandidatesApiModelToJSON(value?: SensorCandidatesApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'names': value.names,
    };
}


