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
    LoginBindingModel,
    LoginBindingModelFromJSON,
    LoginBindingModelToJSON,
    LoginedApiModel,
    LoginedApiModelFromJSON,
    LoginedApiModelToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
} from '../models';

export interface SessionsApiApiSessionsPostRequest {
    loginBindingModel?: LoginBindingModel;
}

/**
 * no description
 */
export class SessionsApi extends runtime.BaseAPI {

    /**
     */
    async apiSessionsPostRaw(requestParameters: SessionsApiApiSessionsPostRequest): Promise<runtime.ApiResponse<LoginedApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0; v=1.0';

        const response = await this.request({
            path: `/api/sessions`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: LoginBindingModelToJSON(requestParameters.loginBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => LoginedApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiSessionsPost(requestParameters: SessionsApiApiSessionsPostRequest): Promise<LoginedApiModel> {
        const response = await this.apiSessionsPostRaw(requestParameters);
        return await response.value();
    }

}