using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CodeChallenge.Data;
using CodeChallenge.Entities;
using CodeChallenge.Models.Request;
using CodeChallenge.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CodeChallenge.Services
{
    public class UserService
    {
        ApplicationDbContext _dbContext;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;
        IConfiguration _configuration;
        IMapper _mapper;

        public UserService
        (
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper
        )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<ServiceResponse> Register(ApplicationUser User, string Password, string Role)
        {
            User.UserName = User.Email;

            var result = _userManager.CreateAsync(User, Password).Result;

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Role));
                }
                await _userManager.AddToRoleAsync(User, Role);
                return new ServiceResponse { status = true, data = User };
            }
            else
            {
                return new ServiceResponse { status = false, response = "User account creation failed" };
            }
        }

        public ServiceResponse GetUserDetail(string Id)
        {
            var user = _dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Id);
            user.PasswordHash = null;
            user.SecurityStamp = null;
            user.ConcurrencyStamp = null;
            return new ServiceResponse { status = true, data = user };
        }

        public async Task<ServiceResponse> GenerateToken(AuthenticationRequestModel model)
        {
            try
            {
                ApplicationUser User = _dbContext.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == model.Email.ToLower());

                if (User == null)
                {
                    return new ServiceResponse { status = false, response = "User does not exist" };
                }


                var status = await _userManager.CheckPasswordAsync(User, model.Password);

                if (!status)
                {
                    return new ServiceResponse { status = false, response = "Incorrect password supplied" };
                }

                return await CreateAccessToken(User);

            }
            catch (Exception ex)
            {
                return new ServiceResponse { status = false, response = ex.Message };
            }
        }

        public async Task<ServiceResponse> CreateAccessToken(ApplicationUser User)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                List<string> role = (List<string>)await _userManager.GetRolesAsync(User);

                var claims = new[] {
                    new Claim (JwtRegisteredClaimNames.Sub, User.Id),
                    new Claim (ClaimTypes.NameIdentifier, User.Id),
                    new Claim (ClaimTypes.Role, role.FirstOrDefault ()),
                    new Claim (ClaimTypes.AuthorizationDecision, role.FirstOrDefault ()),
                    new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ()),
                    new Claim (JwtRegisteredClaimNames.Iat, new DateTimeOffset (now).ToUnixTimeSeconds ().ToString ())
                };

                int tokenExpiration = _configuration.GetValue<int>("Auth:Jwt:TokenExpiration");
                var IsserSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Auth:Jwt:Issuer"],
                    audience: _configuration["Auth:Jwt:Audience"],
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromHours(tokenExpiration)),
                    signingCredentials: new SigningCredentials(IsserSignKey, SecurityAlgorithms.HmacSha256)
                );

                var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

                var response = new AuthenticateUserResponseModel()
                {
                    Id = User.Id,
                    Token = encodeToken,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Role = role.FirstOrDefault(),
                    DateCreated = User.DateCreated,
                    Email = User.Email
                };

                return new ServiceResponse { data = response, status = true };
            }

            catch (Exception ex)
            {
                return new ServiceResponse { response = "Token generation failed", data = ex, status = false };
            }


        }

        public ServiceResponse IsImage(string image)
        {
            Bitmap bitmap;
            try
            {
                byte[] bytes = Convert.FromBase64String(image);
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    bitmap = new Bitmap(stream);
                    if (bitmap.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        return new ServiceResponse { status = true, response = ".jpg" };
                    }
                    else if (bitmap.RawFormat.Equals(ImageFormat.Png))
                    {
                        return new ServiceResponse { status = true, response = ".png" };
                    }
                    else
                    {
                        return new ServiceResponse { status = false, response = "Invalid Image format. Only Image of type JPEG or PNG is allowed" };
                    }
                }

            }
            catch (Exception ex)
            {
                return new ServiceResponse { status = false, response = "Invalid Image format. Only Image of type JPEG is allowed" + ex };
            }
        }
    }
}