using Adda.API.Dtos;
using Adda.API.Services.PhotoService;
using Adda.API.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adda.API.Controllers;
[ApiController]
[Route("api/admin")]
public class AdminController(
    IUserService userService,
    IPhotoService photoService
) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IPhotoService _photoService = photoService;

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("usersWithRoles")]
    public async Task<IActionResult> GetUsersWithRolesAsync()
    {
        var userList = await _userService.GetUsersWithRolesAsync();
        return Ok(userList);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photosForModeration")]
    public async Task<IActionResult> GetPhotosForModerationAsync()
    {
        IEnumerable<object> photos = await _photoService.GetAllPhotosAsync();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("photo/{photoId}")]
    public async Task<IActionResult> ApprovePhotoAsync(int photoId)
    {
        var result = await _photoService.ApprovePhotoAsync(photoId);

        if (!result.IsError)
        {
            return NoContent();
        }
        return BadRequest("Failed to Approve user photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpDelete("photo/{photoId}")]
    public async Task<IActionResult> DeletePhotoAsync(int photoId)
    {
        var result = await _photoService.DeleteAsync(photoId);

        if (!result.IsError)
        {
            return Ok();
        }
        return BadRequest("Failed to delete the photo");
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("editRoles/{userName}")]
    public async Task<IActionResult> EditRolesAsync(string userName, EditRoleRequest request)
    {
        var result = await _userService.EditRolesAsync(userName, request);

        if (result.IsError)
        {
            return BadRequest("Failed to remove the roles");
        }

        return Ok(result.Value);
    }
}
