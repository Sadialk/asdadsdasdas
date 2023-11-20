using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Testing.Auth.model;

namespace Testing.Auth
{
    public static class AuthEndpoints
    {
        public static void AddAuthApi(this WebApplication app)
        {
            //register
            app.MapPost("api/register", async (UserManager<RentUser> userManager, RegisterUserDto registerUserDto) =>
            {
                var user = await userManager.FindByNameAsync(registerUserDto.Username);
                if (user != null)
                    return Results.UnprocessableEntity("user name already taken");

                var newUser = new RentUser
                {
                    Email = registerUserDto.Email,
                    UserName = registerUserDto.Username,
                };
                var createUserResult = await userManager.CreateAsync(newUser, registerUserDto.Password);

                if (!createUserResult.Succeeded)
                    return Results.UnprocessableEntity("fail");

                await userManager.AddToRoleAsync(newUser, RentRoles.RentUser);

                return Results.Created("api/login", new UserDto(newUser.Id, newUser.UserName, newUser.Email));
            });


            //login
            //check 
            app.MapPost("api/login", async (UserManager<RentUser> userManager, JwtTokenService jwtTokenService, LoginDto loginDto) =>
            {
                var user = await userManager.FindByNameAsync(loginDto.Username);
                if (user == null)
                    return Results.UnprocessableEntity("username or password is incorrect.");



                var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isPasswordValid)
                    return Results.UnprocessableEntity("username or password is incorrect.");
                user.ForceRelogin = false;
                await userManager.UpdateAsync(user);

                var roles = await userManager.GetRolesAsync(user);

                var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
                var refreshToken = jwtTokenService.CreateRefreshToken(user.Id);


                return Results.Ok(new SuccesfullLoginDto(accessToken, refreshToken));
            });

            // access token
            app.MapPost("api/accessToken", async (UserManager<RentUser> userManager, JwtTokenService jwtTokenService, RefreshAccessTokenDto refreshAccessTokenDto) =>
            {
                if (!jwtTokenService.TryParseRefreshToken(refreshAccessTokenDto.RefreshToken, out var claims))
                {
                    return Results.UnprocessableEntity();
                }
                var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Results.UnprocessableEntity("Invalid token");
                }
                if (user.ForceRelogin)
                {
                    return Results.UnprocessableEntity();
                }

                var roles = await userManager.GetRolesAsync(user);

                var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
                var refreshToken = jwtTokenService.CreateRefreshToken(user.Id);

                return Results.Ok(new SuccesfullLoginDto(accessToken, refreshToken));
            });
        }
    }
    public record SuccesfullLoginDto(string AccessToken, string RefreshToken);
    public record RegisterUserDto(string Username, string Email, string Password);
    public record LoginDto(string Username, string Password);
    public record RefreshAccessTokenDto(string RefreshToken);
    public record UserDto(string UserId, string UserName, string Email);
}
