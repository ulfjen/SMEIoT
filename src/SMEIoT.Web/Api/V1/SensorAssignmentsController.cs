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
    private readonly UserManager<User> _userManager;

    public SensorAssignmentsController(
      ISensorAssignmentService service,
      IDeviceService deviceService,
      UserManager<User> userManager)
    {
      _service = service;
      _deviceService = deviceService;
      _userManager = userManager;
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
      var user = await _userManager.FindByNameAsync(binding.UserName);
      if (user == null) {
        throw new EntityNotFoundException($"cannot find the user {binding.UserName}.", "userName");
      }
      await _service.AssignSensorToUserAsync(sensor, user);
      var result = new SensorAssignmentApiModel(sensor.Name, user.UserName);

      return CreatedAtAction(nameof(Create), result);
    }
    
    [HttpGet("{deviceName}/{sensorName}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminUserApiModelList>> Index(string deviceName, string sensorName)
    {
      var device = await _deviceService.GetDeviceByNameAsync(deviceName);
      var sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, sensorName);
      
      var list = new List<AdminUserApiModel>();
      await foreach (var user in _service.ListAllowedUsersBySensorAsync(sensor))
      {
        list.Add(new AdminUserApiModel(user, Array.Empty<string>()));
      }
      var res = new AdminUserApiModelList(list, list.Count);
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
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null) {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }
      await _service.RevokeSensorFromUserAsync(sensor, user);
      return Ok();
    }
  }
}
