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


import * as runtime from '../runtime';
import {
    BrokerDetailsApiModel,
    BrokerDetailsApiModelFromJSON,
    BrokerDetailsApiModelToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
} from '../models';

/**
 * no description
 */
export class BrokerApi extends runtime.BaseAPI {

    /**
     */
    async apiBrokerGetRaw(): Promise<runtime.ApiResponse<BrokerDetailsApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/broker`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => BrokerDetailsApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiBrokerGet(): Promise<BrokerDetailsApiModel> {
        const response = await this.apiBrokerGetRaw();
        return await response.value();
    }

}