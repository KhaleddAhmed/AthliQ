using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.Entities;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AthliQ.Service.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AthliQUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AthliQUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            UserManager<AthliQUser> userManager,
            IMapper mapper,
            ITokenService tokenService,
            SignInManager<AthliQUser> signInManager,
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var genericResponse = new GenericResponse<UserDto>();

            if (loginDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Login";

                return genericResponse;
            }
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "User does not Exist";

                return genericResponse;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                loginDto.Password,
                false
            );
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "InCorrect Password";

                return genericResponse;
            }

            if(user.IsAccepted != true)
            {
                genericResponse.StatusCode= StatusCodes.Status400BadRequest;
                genericResponse.Message = "Admin didn't Accept Your Request Yet";
                return genericResponse;
            }

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager),
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Login Succeeded";

            genericResponse.Data = userDto;

            return genericResponse;
        }

        public async Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            GenericResponse<UserDto> response = new GenericResponse<UserDto>();
            if (registerDto is null)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Please Enter valid Data ";

                return response;
            }

            if (await CheckEmailExistAsync(registerDto.Email))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Email Already Exists";

                return response;
            }

            var mappedUser = _mapper.Map<AthliQUser>(registerDto);
            mappedUser.UserName = mappedUser.Email.Split("@")[0];

            var result = await _userManager.CreateAsync(mappedUser, registerDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(mappedUser, "User");
                if (registerDto.Clubs != null)
                {
                    foreach (var club in registerDto.Clubs)
                    {
                        var userclub = new UserClub
                        {
                            AthliQUserId = mappedUser.Id,
                            ClubName = club
                        };

                        await _unitOfWork.Repository<UserClub, int>().AddAsync(userclub);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }

            if (!result.Succeeded)
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Failed to Create User";

                return response;
            }

            var userDto = new UserDto()
            {
                Email = mappedUser.Email,
                UserName = mappedUser.UserName,
                Token = await _tokenService.CreateTokenAsync(mappedUser, _userManager),
            };

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "User Created Successfully";
            response.Data = userDto;

            return response;
        }
    }
}
