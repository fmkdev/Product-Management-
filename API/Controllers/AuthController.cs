using API.Domain.Entities;
using API.Dtos;
using API.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static API.Dtos.ResponseDto;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly IJwt _jwt;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IJwt jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Register(CreateUserDto createUserRequest)
        {
            var user = new User
            {
                UserName = createUserRequest.Username,
                Email = createUserRequest.Email                
            };
            var result = await _userManager.CreateAsync(user, createUserRequest.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ResponseDto()
                {
                    Status = ResponseStatus.Fail,
                    Message = "Registration failed"
                });
            }
            //assign role
            var roleResult = await _userManager.AddToRolesAsync(user, createUserRequest.Roles);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new ResponseDto()
                {
                    Status = ResponseStatus.Fail,
                    Message = "Registration failed"
                });
            }
            return Ok(new ResponseDto
            {
                Status = ResponseStatus.Success,
                Message = "User Successfully created"
            });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            var mappedUser = new
            {
                Username = user.UserName,
                Email = user.Email,
            };
            if (user != null)
            {
                if (!user.IsActive)
                {
                    return BadRequest(new ResponseDto()
                    {
                        Status = ResponseStatus.Fail,
                        Message = "Account not activated"
                    });
                }

                var res = await _signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                var canSigin = await _signInManager.CanSignInAsync(user);
                await _signInManager.SignInAsync(user, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var jwt = _jwt.Generate(user);
                    var response = new
                    {
                        id = user.Id,
                        auth_token = jwt.Token,
                        roles = roles,
                        user = mappedUser,
                        canLogin = canSigin
                    };
                    return Ok(new ResponseDto
                    {
                        Status = ResponseStatus.Success,
                        Data = response
                    });
                }
                else
                {
                    if (result.IsLockedOut || result.IsNotAllowed)
                    {
                        var response = new
                        {
                            canLogin = canSigin,
                            result.IsLockedOut,
                            result.IsNotAllowed,
                            result.RequiresTwoFactor,
                            user = mappedUser,
                        };

                        return Ok(new
                        {
                            status = ResponseStatus.Fail,
                            Data = response
                        });
                    }

                    return BadRequest(new ResponseDto()
                    {
                        Status = ResponseStatus.Fail,
                        Message = "incorrect email or password"
                    });
                }


            }

            return BadRequest(new
            {
                status = "unauthorized",
                message = "incorrect email or password"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new ResponseDto
            {
                Status = ResponseStatus.Success,
                Message = "Successfully logged out"
            });
        }
    }
    public class CreateUserDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string? Password { get; set; }
        [Required]
        public List<string>? Roles { get; set; }
    }
}
