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
 * @interface NumberTimeSeriesApiModel
 */
export interface NumberTimeSeriesApiModel {
    /**
     * 
     * @type {number}
     * @memberof NumberTimeSeriesApiModel
     */
    value: number;
    /**
     * 
     * @type {string}
     * @memberof NumberTimeSeriesApiModel
     */
    createdAt: string;
}

export function NumberTimeSeriesApiModelFromJSON(json: any): NumberTimeSeriesApiModel {
    return NumberTimeSeriesApiModelFromJSONTyped(json, false);
}

export function NumberTimeSeriesApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): NumberTimeSeriesApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'value': json['value'],
        'createdAt': json['createdAt'],
    };
}

export function NumberTimeSeriesApiModelToJSON(value?: NumberTimeSeriesApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'value': value.value,
        'createdAt': value.createdAt,
    };
}


