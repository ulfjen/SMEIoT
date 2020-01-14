using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  [Route("api/sensors")]
  public class SensorAssignmentsController : BaseController
  {
    private readonly ISensorAssignmentService _service;

    public SensorAssignmentsController(ISensorAssignmentService service)
    {
      _service = service;
    }
    
    [HttpPost("{name}/users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SensorAssignmentApiModel>> Create(string name, [BindRequired] AssignUserSensorBindingModel binding)
    {
      await _service.AssignSensorToUserAsync(name, binding.UserName);
      var result = new SensorAssignmentApiModel(name, binding.UserName);

      return CreatedAtAction(nameof(Create), result);
    }
    
    [HttpGet("{name}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async IAsyncEnumerable<User> Index(string name)
    {
      await foreach (var users in _service.ListAllowedUsersBySensorNameAsync(name))
      {
        yield return users;
      }
    }

    [HttpDelete("{name}/users/{userName}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(string name, string userName)
    {
      await _service.RevokeSensorFromUserAsync(name, userName);
      return Ok();
    }
  }
}
