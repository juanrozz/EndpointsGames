using EndpointsPage.DTOs.Request;
using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;
using EndpointsPage.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.DirectoryServices.AccountManagement;



namespace EndpointsPage.Controllers
{
	[Route("Api")]
	[ApiController]
	public class AuthenController : ControllerBase
	{
		private readonly IConfiguration _config;

		private readonly InternsDbContext _dbContext;
		private readonly ILogger<AuthenController> _logger;

		public AuthenController(ILogger<AuthenController> logger, IConfiguration config, InternsDbContext dbContext)
		{
			_logger = logger;
			_config = config;
			_dbContext = dbContext;
		}
		[HttpPost("SignUp")]
		public async Task<ActionResult> SignUp([FromBody] SiginUpRequestDto request)
		{
			bool newUserExist = await _dbContext.UsersJdrs.AnyAsync(n => n.Username == request.UserName);
			if (newUserExist == true)
			{
				//var responseError = GeneralResponse<object>.FailureResult(
				//    message: "The user is already registered",
				//    errors: new List<string> { "The user is already registered in the database" }
				//    );
				return StatusCode(StatusCodes.Status406NotAcceptable, GeneralResponseDto<object>.FailureResult(
					"The user is already registered",
					"The user is already registered in the database"
					));
			}
			bool isValidUserAD = false;
			//conectar al active directory
			try
			{
				using (PrincipalContext pC = new PrincipalContext(ContextType.Domain))
				{
					using (UserPrincipal user = UserPrincipal.FindByIdentity(pC, IdentityType.SamAccountName, request.UserName))
					{
						isValidUserAD = (user != null);
					}
				}
			}
			catch (Exception ex)
			{
				return Unauthorized(ex.ToString());
			}

			if (isValidUserAD == false)
			{
				//var responseError = GeneralResponse<object>.FailureResult(
				//    message: "Paisano your credentials its incorrects",
				//    errors: new List<string> { "its credentials don´t exist" }
				//    );
				return Unauthorized(GeneralResponseDto<object>.FailureResult(
					"Your credentials are incorrects",
					"Your credentials don´t exist"
					));
			}

			UsersJdr newUser = new UsersJdr
			{
				Username = request.UserName,
				Name = request.Name,
				DomainId = request.DomainId ?? 1,
				Adp = request.ADP
			};

			await _dbContext.UsersJdrs.AddAsync(newUser);
			await _dbContext.SaveChangesAsync();

			//var responseSuccessfully = GeneralResponse<object>.SuccesResult(
			//    message: "User Created Successfully",
			//    data: null
			//    );
			return Ok(GeneralResponseDto<object>.SuccesResult(null, "User Created Successfully"));

		}

		[HttpPost("Login")]

		public async Task<ActionResult<GeneralResponseDto<UserResponseDto>>> Login([FromBody] UserRequestDto request)
		{
			_logger.LogInformation("Intento de login para el usuario: {UserName}", request.UserName);

			bool isValidUserAD = false;
			//conectar al active directory
			try
			{
				using (PrincipalContext pC = new PrincipalContext(ContextType.Domain))
				{

					isValidUserAD = pC.ValidateCredentials(request.UserName, request.Password);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error en Active Directory para: {UserName}", request.UserName);
				return Unauthorized(ex.ToString());
			}

			if (isValidUserAD == false)
			{
				//var responseError = GeneralResponse<UserResponseDto>.FailureResult(
				//    message: "Paisano your credentials its incorrects",
				//    errors: new List<string> { "its credentials don´t exist"}
				//    );
				_logger.LogWarning("Contraseña incorrecta en AD para: {UserName}", request.UserName);
				return Unauthorized(GeneralResponseDto<UserResponseDto>.FailureResult(
					"Your credentials are incorrects",
					"Your credentials don´t exist"
					));
			}

			UsersJdr? currentUser = await _dbContext.UsersJdrs.FirstOrDefaultAsync(u => u.Username == request.UserName);
			if (currentUser == null)
			{
				//var responseError = GeneralResponse<UserResponseDto>.FailureResult(
				//    message: "the user not found, Do you want to register?",
				//    errors: new List<string> { "don´t exist, resgister" }
				//    );
				return Unauthorized(GeneralResponseDto<UserResponseDto>.FailureResult(
					"the user not found, Do you want to register?",
					"don´t exist, resgister"
					));
				//return Unauthorized("the user not found, Do you want to register?");
			}

			if (request.DomainId.HasValue && currentUser.DomainId != request.DomainId.Value)
			{
				return StatusCode(StatusCodes.Status403Forbidden, GeneralResponseDto<UserResponseDto>.FailureResult(
					"Access denied: this is not your registered domain",
					"Domain mismatch"
					));
			}


			if (currentUser.Status == false)
			{
				//var responseError = GeneralResponse<UserResponseDto>.FailureResult(
				//    message: "User is inactive",
				//    errors: new List<string> { "User is inactive" }
				//    );
				return Unauthorized(GeneralResponseDto<UserResponseDto>.FailureResult(
					"Invalid credentials",
					"User is inactive"
					));
			}
			UserResponseDto response = new UserResponseDto
			{
				Name = currentUser.Name,
				Token = GenerateTokenPrivate(currentUser),
				ADP = currentUser.Adp
			};
			//var responseSuccessfully = GeneralResponse<UserResponseDto>.SuccesResult(
			//    message: "User Created Successfully",
			//    data: response
			//    );
			_logger.LogInformation("¡Login Exitoso! Usuario: {UserName}", request.UserName);
			return Ok(GeneralResponseDto<UserResponseDto>.SuccesResult(
				response,
				"User LogIn Successfully"));
		}
		private string GenerateTokenPrivate(UsersJdr user)
		{
			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
			SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			List<Claim> claims = new List<Claim> {

				new Claim(JwtRegisteredClaimNames.NameId,user.UserId.ToString()),
				new Claim(JwtRegisteredClaimNames.FamilyName,user.Username),
				new Claim(JwtRegisteredClaimNames.GivenName,user.Name!),
				new Claim(JwtRegisteredClaimNames.UniqueName,user.Name),
				new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
			};
			JwtSecurityToken token = new JwtSecurityToken(
					issuer: _config["Jwt:Issuer"],
					audience: _config["Jwt:Audience"],
					claims: claims,
					expires: DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:Expires"])),
					notBefore: DateTime.UtcNow,
					signingCredentials: credentials
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}


}
