using CardSortApi.Domain.Dto;
using CardSortApi.Repositories.Models;
using CardSortApi.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CardSortApi.Domain.Models;

namespace CardSortApi.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly IAccountService _accountService;

		public AccountController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		[HttpGet]
		[AllowAnonymous]
		[ResponseType(typeof(bool))]
		[ActionName("validateEmail")]
		public async Task<IHttpActionResult> IsEmailValid(string email)
		{
			try
			{
				var isValidResponse = await _accountService.IsEmailValid(email);
				if (isValidResponse.Succeeded)
				{
					return Ok(isValidResponse.ResponseBody);
				}

				return BadRequest(isValidResponse.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpGet]
		[AllowAnonymous]
		[ResponseType(typeof(bool))]
		[ActionName("validateUsername")]
		public async Task<IHttpActionResult> IsUsernameValid(string username)
		{
			try
			{
				var response = await _accountService.IsUsernameValid(username);
				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ResponseType(typeof(AuthResponse))]
		[ActionName("create")]
		public async Task<IHttpActionResult> CreateAccount([FromBody] AccountRequest request)
		{
			try
			{
				var response =
					await _accountService.CreateAccount(request.Name, request.Email, request.Username,
						request.Password);
				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpPost]
		[Authorize]
		[ActionName("resendEmail")]
		public async Task<IHttpActionResult> ResendWelcomeEmail([FromBody] ResendRequest request)
		{
			try
			{
				var response = await _accountService.ResendEmail(request.Username);

				if (response.Succeeded)
				{
					return Ok();
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ActionName("login")]
		[ResponseType(typeof(AuthResponse))]
		public async Task<IHttpActionResult> Login([FromBody] AuthRequest request)
		{
			try
			{
				var response = await _accountService.Login(request.Username, request.Password);

				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ActionName("verify")]
		[ResponseType(typeof(AuthResponse))]
		public async Task<IHttpActionResult> VerifyEmail([FromBody] VerificationRequest request)
		{
			try
			{
				var response = await _accountService.VerifyEmail(request.Username, request.Token);
				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpPost]
		[Authorize]
		[ActionName("update")]
		[ResponseType(typeof(AuthResponse))]
		public async Task<IHttpActionResult> UpdateAccount([FromBody] UpdateRequest request)
		{
			try
			{
				var response = await _accountService.UpdateAccount(CurrentIdentity.UserId, request.Name, request.Email);
				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}
	}
}