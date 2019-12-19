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
    AdminUserApiModel,
    AdminUserApiModelFromJSON,
    AdminUserApiModelToJSON,
    AdminUserApiModelList,
    AdminUserApiModelListFromJSON,
    AdminUserApiModelListToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
    UserCredentialsUpdateApiModel,
    UserCredentialsUpdateApiModelFromJSON,
    UserCredentialsUpdateApiModelToJSON,
    UserRolesBindingModel,
    UserRolesBindingModelFromJSON,
    UserRolesBindingModelToJSON,
} from '../models';

export interface AdminUsersApiApiAdminUsersGetRequest {
    start?: number;
    limit?: number;
}

export interface AdminUsersApiApiAdminUsersUsernameGetRequest {
    username: string;
}

export interface AdminUsersApiApiAdminUsersUsernameRolesPutRequest {
    username: string;
    userRolesBindingModel?: UserRolesBindingModel;
}

/**
 * no description
 */
export class AdminUsersApi extends runtime.BaseAPI {

    /**
     */
    async apiAdminUsersGetRaw(requestParameters: AdminUsersApiApiAdminUsersGetRequest): Promise<runtime.ApiResponse<AdminUserApiModelList>> {
        const queryParameters: runtime.HTTPQuery = {};

        if (requestParameters.start !== undefined) {
            queryParameters['start'] = requestParameters.start;
        }

        if (requestParameters.limit !== undefined) {
            queryParameters['limit'] = requestParameters.limit;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/admin/users`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => AdminUserApiModelListFromJSON(jsonValue));
    }

    /**
     */
    async apiAdminUsersGet(requestParameters: AdminUsersApiApiAdminUsersGetRequest): Promise<AdminUserApiModelList> {
        const response = await this.apiAdminUsersGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiAdminUsersUsernameGetRaw(requestParameters: AdminUsersApiApiAdminUsersUsernameGetRequest): Promise<runtime.ApiResponse<AdminUserApiModel>> {
        if (requestParameters.username === null || requestParameters.username === undefined) {
            throw new runtime.RequiredError('username','Required parameter requestParameters.username was null or undefined when calling apiAdminUsersUsernameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/admin/users/{username}`.replace(`{${"username"}}`, encodeURIComponent(String(requestParameters.username))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => AdminUserApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiAdminUsersUsernameGet(requestParameters: AdminUsersApiApiAdminUsersUsernameGetRequest): Promise<AdminUserApiModel> {
        const response = await this.apiAdminUsersUsernameGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiAdminUsersUsernameRolesPutRaw(requestParameters: AdminUsersApiApiAdminUsersUsernameRolesPutRequest): Promise<runtime.ApiResponse<UserCredentialsUpdateApiModel>> {
        if (requestParameters.username === null || requestParameters.username === undefined) {
            throw new runtime.RequiredError('username','Required parameter requestParameters.username was null or undefined when calling apiAdminUsersUsernameRolesPut.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0; v=1.0';

        const response = await this.request({
            path: `/api/admin/users/{username}/roles`.replace(`{${"username"}}`, encodeURIComponent(String(requestParameters.username))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: UserRolesBindingModelToJSON(requestParameters.userRolesBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => UserCredentialsUpdateApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiAdminUsersUsernameRolesPut(requestParameters: AdminUsersApiApiAdminUsersUsernameRolesPutRequest): Promise<UserCredentialsUpdateApiModel> {
        const response = await this.apiAdminUsersUsernameRolesPutRaw(requestParameters);
        return await response.value();
    }

}