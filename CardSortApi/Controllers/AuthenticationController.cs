using System.Drawing;
using CardSortApi.Domain.Dto;
using CardSortApi.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;
using CardSortApi.Repositories.Models;

namespace CardSortApi.Controllers
{
	public class AuthenticationController : BaseApiController
	{
		private readonly IAuthService _authService;

		public AuthenticationController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<AuthResponse> RequestAuth(string username, string password)
		{
			var user = CurrentIdentity;
			var result = await _authService.GetAuthentication(username, password);
			return result;
		}
	}
}