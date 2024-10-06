using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.UserRepository;
using Adda.API.Security.CurrentUserProvider;
using AutoMapper;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Adda.API.Services.UserService;

public class UserService(IMapper mapper, UserManager<User> userManager, ICurrentUserProvider currentUser, IUserRepository userRepository) : IUserService
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<User> _userManager = userManager;
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<Success>> BookmakAsync(int id, int recipientId)
    {
        Bookmark bookmark = await _userRepository.GetBookmarkAsync(id, recipientId);

        if (bookmark != null)
        {
            return Error.Conflict("You already bookmark this user.");
        }

        if (await _userRepository.GetAsync(recipientId, false) == null)
        {
            return Error.Validation("Please select a valid user.");
        }

        var newBookmark = new Bookmark { BookmarkerId = id, BookmarkedId = recipientId };

        await _userRepository.AddAsync(newBookmark);

        if (await _userRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Validation("Unable to perform the operation.");
    }
    public async Task<PageList<User>> GetAsync(UserParams filterOptions)
    {
        filterOptions.UserId = _currentUser.UserId;
        PageList<User> users = await _userRepository.GetAsync(filterOptions);
        return users;
    }
    public async Task<ErrorOr<User>> GetAsync(int id)
    {
        bool isCurrentUser = _currentUser.UserId == id;
        return await _userRepository.GetAsync(id, isCurrentUser);
    }

    public async Task<ErrorOr<User>> RegistrationAsync(UserForRegisterDto request)
    {
        try
        {

            User userToCreate = _mapper.Map<User>(request);

            IdentityResult result = await _userManager.CreateAsync(userToCreate, request.Password);

            if (result.Succeeded)
            {
                return userToCreate;

            }
            return Error.Failure(description: "Couldn't create user!");
        }
        catch (Exception e)
        {
            return Error.Failure(description: e.Message);
        }
    }

    public async Task<ErrorOr<Success>> UpdateAsync(int id, UserForUpdateDto userForUpdateDto)
    {
        User userFromRepo = await _userRepository.GetAsync(id, true);

        _mapper.Map(userForUpdateDto, userFromRepo); // (from, to)

        if (await _userRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Failure(description: "Couldn't update user!");
    }

    public async Task<IEnumerable<object>> GetUsersWithRolesAsync()
    {
        return await _userRepository.GetUsersWithRolesAsync();
    }

    public async Task<ErrorOr<IList<string>>> EditRolesAsync(string userName, RoleEditDto roleEditDto)
    {
        try
        {
            User user = await _userManager.FindByNameAsync(userName);

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            string[] selectedRoles = roleEditDto.RoleName;

            selectedRoles ??= [];
            IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return Error.Failure(description: "Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return Error.Failure(description: "Failed to remove the roles");
            }

            return (await _userManager.GetRolesAsync(user)).ToList();
        }
        catch (Exception e)
        {
            return Error.Failure(description: e.Message);
        }
    }
}