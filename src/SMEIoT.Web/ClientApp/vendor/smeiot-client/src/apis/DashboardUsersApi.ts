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

export interface DashboardUsersApiDashboardUsersUsernameGetRequest {
    username: string;
}

/**
 * no description
 */
export class DashboardUsersApi extends runtime.BaseAPI {

    /**
     */
    async dashboardUsersUsernameGetRaw(requestParameters: DashboardUsersApiDashboardUsersUsernameGetRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.username === null || requestParameters.username === undefined) {
            throw new runtime.RequiredError('username','Required parameter requestParameters.username was null or undefined when calling dashboardUsersUsernameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/dashboard/users/{username}`.replace(`{${"username"}}`, encodeURIComponent(String(requestParameters.username))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async dashboardUsersUsernameGet(requestParameters: DashboardUsersApiDashboardUsersUsernameGetRequest): Promise<void> {
        await this.dashboardUsersUsernameGetRaw(requestParameters);
    }

}
