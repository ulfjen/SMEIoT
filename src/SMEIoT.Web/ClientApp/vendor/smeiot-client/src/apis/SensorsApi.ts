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


import * as runtime from '../runtime';
import {
    BasicSensorApiModel,
    BasicSensorApiModelFromJSON,
    BasicSensorApiModelToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
    SensorDetailsApiModel,
    SensorDetailsApiModelFromJSON,
    SensorDetailsApiModelToJSON,
    SensorLocatorBindingModel,
    SensorLocatorBindingModelFromJSON,
    SensorLocatorBindingModelToJSON,
} from '../models';

export interface SensorsApiApiSensorsNameGetRequest {
    name: string;
}

export interface SensorsApiApiSensorsPostRequest {
    sensorLocatorBindingModel?: SensorLocatorBindingModel;
}

export interface SensorsApiSensorsNameGetRequest {
    name: string;
}

/**
 * no description
 */
export class SensorsApi extends runtime.BaseAPI {

    /**
     */
    async apiSensorsNameGetRaw(requestParameters: SensorsApiApiSensorsNameGetRequest): Promise<runtime.ApiResponse<SensorDetailsApiModel>> {
        if (requestParameters.name === null || requestParameters.name === undefined) {
            throw new runtime.RequiredError('name','Required parameter requestParameters.name was null or undefined when calling apiSensorsNameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/sensors/{name}`.replace(`{${"name"}}`, encodeURIComponent(String(requestParameters.name))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => SensorDetailsApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiSensorsNameGet(requestParameters: SensorsApiApiSensorsNameGetRequest): Promise<SensorDetailsApiModel> {
        const response = await this.apiSensorsNameGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiSensorsPostRaw(requestParameters: SensorsApiApiSensorsPostRequest): Promise<runtime.ApiResponse<BasicSensorApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0';

        const response = await this.request({
            path: `/api/sensors`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: SensorLocatorBindingModelToJSON(requestParameters.sensorLocatorBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => BasicSensorApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiSensorsPost(requestParameters: SensorsApiApiSensorsPostRequest): Promise<BasicSensorApiModel> {
        const response = await this.apiSensorsPostRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async sensorsNameGetRaw(requestParameters: SensorsApiSensorsNameGetRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.name === null || requestParameters.name === undefined) {
            throw new runtime.RequiredError('name','Required parameter requestParameters.name was null or undefined when calling sensorsNameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/sensors/{name}`.replace(`{${"name"}}`, encodeURIComponent(String(requestParameters.name))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async sensorsNameGet(requestParameters: SensorsApiSensorsNameGetRequest): Promise<void> {
        await this.sensorsNameGetRaw(requestParameters);
    }

}
