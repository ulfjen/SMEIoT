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
    offset?: number;
    limit?: number;
    roles?: Array<string>;
}

export interface AdminUsersApiApiAdminUsersSearchGetRequest {
    query: string;
    limit?: number;
}

export interface AdminUsersApiApiAdminUsersUserNameGetRequest {
    userName: string;
}

export interface AdminUsersApiApiAdminUsersUserNameRolesPutRequest {
    userName: string;
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

        if (requestParameters.offset !== undefined) {
            queryParameters['offset'] = requestParameters.offset;
        }

        if (requestParameters.limit !== undefined) {
            queryParameters['limit'] = requestParameters.limit;
        }

        if (requestParameters.roles) {
            queryParameters['roles'] = requestParameters.roles;
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
    async apiAdminUsersSearchGetRaw(requestParameters: AdminUsersApiApiAdminUsersSearchGetRequest): Promise<runtime.ApiResponse<AdminUserApiModelList>> {
        if (requestParameters.query === null || requestParameters.query === undefined) {
            throw new runtime.RequiredError('query','Required parameter requestParameters.query was null or undefined when calling apiAdminUsersSearchGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        if (requestParameters.query !== undefined) {
            queryParameters['query'] = requestParameters.query;
        }

        if (requestParameters.limit !== undefined) {
            queryParameters['limit'] = requestParameters.limit;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/admin/users/search`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => AdminUserApiModelListFromJSON(jsonValue));
    }

    /**
     */
    async apiAdminUsersSearchGet(requestParameters: AdminUsersApiApiAdminUsersSearchGetRequest): Promise<AdminUserApiModelList> {
        const response = await this.apiAdminUsersSearchGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiAdminUsersUserNameGetRaw(requestParameters: AdminUsersApiApiAdminUsersUserNameGetRequest): Promise<runtime.ApiResponse<AdminUserApiModel>> {
        if (requestParameters.userName === null || requestParameters.userName === undefined) {
            throw new runtime.RequiredError('userName','Required parameter requestParameters.userName was null or undefined when calling apiAdminUsersUserNameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/admin/users/{userName}`.replace(`{${"userName"}}`, encodeURIComponent(String(requestParameters.userName))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => AdminUserApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiAdminUsersUserNameGet(requestParameters: AdminUsersApiApiAdminUsersUserNameGetRequest): Promise<AdminUserApiModel> {
        const response = await this.apiAdminUsersUserNameGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiAdminUsersUserNameRolesPutRaw(requestParameters: AdminUsersApiApiAdminUsersUserNameRolesPutRequest): Promise<runtime.ApiResponse<UserCredentialsUpdateApiModel>> {
        if (requestParameters.userName === null || requestParameters.userName === undefined) {
            throw new runtime.RequiredError('userName','Required parameter requestParameters.userName was null or undefined when calling apiAdminUsersUserNameRolesPut.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; v=1.0';

        const response = await this.request({
            path: `/api/admin/users/{userName}/roles`.replace(`{${"userName"}}`, encodeURIComponent(String(requestParameters.userName))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: UserRolesBindingModelToJSON(requestParameters.userRolesBindingModel),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => UserCredentialsUpdateApiModelFromJSON(jsonValue));
    }

    /**
     */
    async apiAdminUsersUserNameRolesPut(requestParameters: AdminUsersApiApiAdminUsersUserNameRolesPutRequest): Promise<UserCredentialsUpdateApiModel> {
        const response = await this.apiAdminUsersUserNameRolesPutRaw(requestParameters);
        return await response.value();
    }

}
