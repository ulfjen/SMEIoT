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
 * @interface BrokerStatisticsApiModel
 */
export interface BrokerStatisticsApiModel {
    /**
     * 
     * @type {{ [key: string]: string; }}
     * @memberof BrokerStatisticsApiModel
     */
    statistics: { [key: string]: string; };
}

export function BrokerStatisticsApiModelFromJSON(json: any): BrokerStatisticsApiModel {
    return BrokerStatisticsApiModelFromJSONTyped(json, false);
}

export function BrokerStatisticsApiModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): BrokerStatisticsApiModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'statistics': json['statistics'],
    };
}

export function BrokerStatisticsApiModelToJSON(value?: BrokerStatisticsApiModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'statistics': value.statistics,
    };
}


