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

export interface UsersApiApiUsersUserNameGetRequest {
    userName: string;
}

export interface UsersApiApiUsersUserNamePasswordPutRequest {
    userName: string;
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
    async apiUsersUserNameGetRaw(requestParameters: UsersApiApiUsersUserNameGetRequest): Promise<runtime.ApiResponse<BasicUserApiModel>> {
        if (requestParameters.userName === null || requestParameters.userName === undefined) {
            throw new runtime.RequiredError('userName','Required parameter requestParameters.userName was null or undefined when calling apiUsersUserNameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/users/{userName}`.replace(`{${"userName"}}`, encodeURIComponent(String(requestParameters.userName))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => BasicUserApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiUsersUserNameGet(requestParameters: UsersApiApiUsersUserNameGetRequest): Promise<BasicUserApiModel> {
        const response = await this.apiUsersUserNameGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiUsersUserNamePasswordPutRaw(requestParameters: UsersApiApiUsersUserNamePasswordPutRequest): Promise<runtime.ApiResponse<UserCredentialsUpdateApiModel>> {
        if (requestParameters.userName === null || requestParameters.userName === undefined) {
            throw new runtime.RequiredError('userName','Required parameter requestParameters.userName was null or undefined when calling apiUsersUserNamePasswordPut.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0; v=1.0';

        const response = await this.request({
            path: `/api/users/{userName}/password`.replace(`{${"userName"}}`, encodeURIComponent(String(requestParameters.userName))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: ConfirmedUserCredentialsUpdateBindingModelToJSON(requestParameters.confirmedUserCredentialsUpdateBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => UserCredentialsUpdateApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiUsersUserNamePasswordPut(requestParameters: UsersApiApiUsersUserNamePasswordPutRequest): Promise<UserCredentialsUpdateApiModel> {
        const response = await this.apiUsersUserNamePasswordPutRaw(requestParameters);
        return await response.value();
    }

}
