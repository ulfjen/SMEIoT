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
    DeviceApiModel,
    DeviceApiModelFromJSON,
    DeviceApiModelToJSON,
    DeviceApiModelList,
    DeviceApiModelListFromJSON,
    DeviceApiModelListToJSON,
    DeviceConfigBindingModel,
    DeviceConfigBindingModelFromJSON,
    DeviceConfigBindingModelToJSON,
    DeviceConfigSuggestApiModel,
    DeviceConfigSuggestApiModelFromJSON,
    DeviceConfigSuggestApiModelToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
    SensorCandidatesApiModel,
    SensorCandidatesApiModelFromJSON,
    SensorCandidatesApiModelToJSON,
} from '../models';

export interface DevicesApiApiDevicesBootstrapPostRequest {
    deviceConfigBindingModel?: DeviceConfigBindingModel;
}

export interface DevicesApiApiDevicesGetRequest {
    start?: number;
    limit?: number;
}

export interface DevicesApiApiDevicesNameGetRequest {
    name: string;
}

export interface DevicesApiApiDevicesNamePskGetRequest {
    name: string;
}

export interface DevicesApiApiDevicesNamePutRequest {
    name: string;
    deviceConfigBindingModel?: DeviceConfigBindingModel;
}

export interface DevicesApiApiDevicesNameSensorCandidatesGetRequest {
    name: string;
}

/**
 * no description
 */
export class DevicesApi extends runtime.BaseAPI {

    /**
     */
    async apiDevicesBootstrapPostRaw(requestParameters: DevicesApiApiDevicesBootstrapPostRequest): Promise<runtime.ApiResponse<DeviceApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0; v=1.0';

        const response = await this.request({
            path: `/api/devices/bootstrap`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: DeviceConfigBindingModelToJSON(requestParameters.deviceConfigBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesBootstrapPost(requestParameters: DevicesApiApiDevicesBootstrapPostRequest): Promise<DeviceApiModel> {
        const response = await this.apiDevicesBootstrapPostRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiDevicesConfigSuggestBootstrapGetRaw(): Promise<runtime.ApiResponse<DeviceConfigSuggestApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices/config_suggest/bootstrap`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceConfigSuggestApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesConfigSuggestBootstrapGet(): Promise<DeviceConfigSuggestApiModel> {
        const response = await this.apiDevicesConfigSuggestBootstrapGetRaw();
        return await response.value();
    }

    /**
     */
    async apiDevicesConfigSuggestDeviceNameGetRaw(): Promise<runtime.ApiResponse<DeviceConfigSuggestApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices/config_suggest/device_name`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceConfigSuggestApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesConfigSuggestDeviceNameGet(): Promise<DeviceConfigSuggestApiModel> {
        const response = await this.apiDevicesConfigSuggestDeviceNameGetRaw();
        return await response.value();
    }

    /**
     */
    async apiDevicesConfigSuggestKeyGetRaw(): Promise<runtime.ApiResponse<DeviceConfigSuggestApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices/config_suggest/key`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceConfigSuggestApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesConfigSuggestKeyGet(): Promise<DeviceConfigSuggestApiModel> {
        const response = await this.apiDevicesConfigSuggestKeyGetRaw();
        return await response.value();
    }

    /**
     */
    async apiDevicesGetRaw(requestParameters: DevicesApiApiDevicesGetRequest): Promise<runtime.ApiResponse<DeviceApiModelList>> {
        const queryParameters: runtime.HTTPQuery = {};

        if (requestParameters.start !== undefined) {
            queryParameters['start'] = requestParameters.start;
        }

        if (requestParameters.limit !== undefined) {
            queryParameters['limit'] = requestParameters.limit;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceApiModelListFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesGet(requestParameters: DevicesApiApiDevicesGetRequest): Promise<DeviceApiModelList> {
        const response = await this.apiDevicesGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiDevicesNameGetRaw(requestParameters: DevicesApiApiDevicesNameGetRequest): Promise<runtime.ApiResponse<DeviceApiModel>> {
        if (requestParameters.name === null || requestParameters.name === undefined) {
            throw new runtime.RequiredError('name','Required parameter requestParameters.name was null or undefined when calling apiDevicesNameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices/{name}`.replace(`{${"name"}}`, encodeURIComponent(String(requestParameters.name))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesNameGet(requestParameters: DevicesApiApiDevicesNameGetRequest): Promise<DeviceApiModel> {
        const response = await this.apiDevicesNameGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiDevicesNamePskGetRaw(requestParameters: DevicesApiApiDevicesNamePskGetRequest): Promise<runtime.ApiResponse<string>> {
        if (requestParameters.name === null || requestParameters.name === undefined) {
            throw new runtime.RequiredError('name','Required parameter requestParameters.name was null or undefined when calling apiDevicesNamePskGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices/{name}/psk`.replace(`{${"name"}}`, encodeURIComponent(String(requestParameters.name))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async apiDevicesNamePskGet(requestParameters: DevicesApiApiDevicesNamePskGetRequest): Promise<string> {
        const response = await this.apiDevicesNamePskGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiDevicesNamePutRaw(requestParameters: DevicesApiApiDevicesNamePutRequest): Promise<runtime.ApiResponse<DeviceApiModel>> {
        if (requestParameters.name === null || requestParameters.name === undefined) {
            throw new runtime.RequiredError('name','Required parameter requestParameters.name was null or undefined when calling apiDevicesNamePut.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0; v=1.0';

        const response = await this.request({
            path: `/api/devices/{name}`.replace(`{${"name"}}`, encodeURIComponent(String(requestParameters.name))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: DeviceConfigBindingModelToJSON(requestParameters.deviceConfigBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => DeviceApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesNamePut(requestParameters: DevicesApiApiDevicesNamePutRequest): Promise<DeviceApiModel> {
        const response = await this.apiDevicesNamePutRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiDevicesNameSensorCandidatesGetRaw(requestParameters: DevicesApiApiDevicesNameSensorCandidatesGetRequest): Promise<runtime.ApiResponse<SensorCandidatesApiModel>> {
        if (requestParameters.name === null || requestParameters.name === undefined) {
            throw new runtime.RequiredError('name','Required parameter requestParameters.name was null or undefined when calling apiDevicesNameSensorCandidatesGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/devices/{name}/sensor_candidates`.replace(`{${"name"}}`, encodeURIComponent(String(requestParameters.name))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => SensorCandidatesApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiDevicesNameSensorCandidatesGet(requestParameters: DevicesApiApiDevicesNameSensorCandidatesGetRequest): Promise<SensorCandidatesApiModel> {
        const response = await this.apiDevicesNameSensorCandidatesGetRaw(requestParameters);
        return await response.value();
    }

}
