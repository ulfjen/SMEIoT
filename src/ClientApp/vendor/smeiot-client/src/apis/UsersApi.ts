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
    BasicUserApiModel,
    BasicUserApiModelFromJSON,
    BasicUserApiModelToJSON,
    ConfirmedUserCredentialsUpdateBindingModel,
    ConfirmedUserCredentialsUpdateBindingModelFromJSON,
    ConfirmedUserCredentialsUpdateBindingModelToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
    UserCredentialsUpdateApiModel,
    UserCredentialsUpdateApiModelFromJSON,
    UserCredentialsUpdateApiModelToJSON,
    ValidatedUserCredentialsBindingModel,
    ValidatedUserCredentialsBindingModelFromJSON,
    ValidatedUserCredentialsBindingModelToJSON,
} from '../models';

export interface UsersApiApiUsersPostRequest {
    validatedUserCredentialsBindingModel?: ValidatedUserCredentialsBindingModel;
}

export interface UsersApiApiUsersUsernameGetRequest {
    username: string;
}

export interface UsersApiApiUsersUsernamePasswordPutRequest {
    username: string;
    confirmedUserCredentialsUpdateBindingModel?: ConfirmedUserCredentialsUpdateBindingModel;
}

/**
 * no description
 */
export class UsersApi extends runtime.BaseAPI {

    /**
     */
    async apiUsersPostRaw(requestParameters: UsersApiApiUsersPostRequest): Promise<runtime.ApiResponse<BasicUserApiModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/users`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: ValidatedUserCredentialsBindingModelToJSON(requestParameters.validatedUserCredentialsBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => BasicUserApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiUsersPost(requestParameters: UsersApiApiUsersPostRequest): Promise<BasicUserApiModel> {
        const response = await this.apiUsersPostRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiUsersUsernameGetRaw(requestParameters: UsersApiApiUsersUsernameGetRequest): Promise<runtime.ApiResponse<BasicUserApiModel>> {
        if (requestParameters.username === null || requestParameters.username === undefined) {
            throw new runtime.RequiredError('username','Required parameter requestParameters.username was null or undefined when calling apiUsersUsernameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/users/{username}`.replace(`{${"username"}}`, encodeURIComponent(String(requestParameters.username))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => BasicUserApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiUsersUsernameGet(requestParameters: UsersApiApiUsersUsernameGetRequest): Promise<BasicUserApiModel> {
        const response = await this.apiUsersUsernameGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiUsersUsernamePasswordPutRaw(requestParameters: UsersApiApiUsersUsernamePasswordPutRequest): Promise<runtime.ApiResponse<UserCredentialsUpdateApiModel>> {
        if (requestParameters.username === null || requestParameters.username === undefined) {
            throw new runtime.RequiredError('username','Required parameter requestParameters.username was null or undefined when calling apiUsersUsernamePasswordPut.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0; v=1.0';

        const response = await this.request({
            path: `/api/users/{username}/password`.replace(`{${"username"}}`, encodeURIComponent(String(requestParameters.username))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: ConfirmedUserCredentialsUpdateBindingModelToJSON(requestParameters.confirmedUserCredentialsUpdateBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => UserCredentialsUpdateApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiUsersUsernamePasswordPut(requestParameters: UsersApiApiUsersUsernamePasswordPutRequest): Promise<UserCredentialsUpdateApiModel> {
        const response = await this.apiUsersUsernamePasswordPutRaw(requestParameters);
        return await response.value();
    }

}