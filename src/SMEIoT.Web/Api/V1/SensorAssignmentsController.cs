using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  [Route("api/sensors")]
  public class SensorAssignmentsController : BaseController
  {
    private readonly ISensorAssignmentService _service;
    private readonly IDeviceService _deviceService;
    private readonly IUserManagementService _userService;

    public SensorAssignmentsController(
      ISensorAssignmentService service,
      IDeviceService deviceService,
      IUserManagementService userService)
    {
      _service = service;
      _deviceService = deviceService;
      _userService = userService;
    }
    
    [HttpPost("{deviceName}/{sensorName}/users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SensorAssignmentApiModel>> Create(string deviceName, string sensorName, [BindRequired] AssignUserSensorBindingModel binding)
    {
      var device = await _deviceService.GetDeviceByNameAsync(deviceName);
      var sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, sensorName);
      var (user, roles) = await _userService.GetUserAndRoleByNameAsync(binding.UserName);

      await _service.AssignSensorToUserAsync(sensor, user);

      var userApiModel = new AdminUserApiModel(user, roles);
      var result = new SensorAssignmentApiModel(sensor.Name, userApiModel);

      return CreatedAtAction(nameof(Create), result);
    }
    
    [HttpGet("{deviceName}/{sensorName}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminUserApiModelList>> Index(string deviceName, string sensorName, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
      var device = await _deviceService.GetDeviceByNameAsync(deviceName);
      var sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, sensorName);
      
      var list = new List<AdminUserApiModel>();
      await foreach (var (user, roles) in _service.ListAllowedUsersBySensorAsync(sensor, offset, limit))
      {
        list.Add(new AdminUserApiModel(user, roles));
      }
      var cnt = await _service.NumberOfAllowedUsersBySensorAsync(sensor);
      var res = new AdminUserApiModelList(list, cnt);
      return Ok(res);
    }

    [HttpDelete("{deviceName}/{sensorName}/users/{userName}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(string deviceName, string sensorName, string userName)
    {
      var device = await _deviceService.GetDeviceByNameAsync(deviceName);
      var sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, sensorName);
      var (user, roles) = await _userService.GetUserAndRoleByNameAsync(userName);
      
      await _service.RevokeSensorFromUserAsync(sensor, user);

      return Ok();
    }
  }
}
