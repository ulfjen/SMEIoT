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

export interface DashboardUsersApiDashboardUsersUserNameGetRequest {
    userName: string;
}

/**
 * no description
 */
export class DashboardUsersApi extends runtime.BaseAPI {

    /**
     */
    async dashboardUsersUserNameGetRaw(requestParameters: DashboardUsersApiDashboardUsersUserNameGetRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.userName === null || requestParameters.userName === undefined) {
            throw new runtime.RequiredError('userName','Required parameter requestParameters.userName was null or undefined when calling dashboardUsersUserNameGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/dashboard/users/{userName}`.replace(`{${"userName"}}`, encodeURIComponent(String(requestParameters.userName))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async dashboardUsersUserNameGet(requestParameters: DashboardUsersApiDashboardUsersUserNameGetRequest): Promise<void> {
        await this.dashboardUsersUserNameGetRaw(requestParameters);
    }

}
