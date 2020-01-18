using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CardSortApi.Domain.Identity;
using CardSortApi.Services.Interfaces;

namespace CardSortApi.Services.MessageHandlers
{
	public class JwtAuthHandler : DelegatingHandler
	{
		private readonly IAuthService _authService;
		private readonly ICurrentHttpContext _currentHttpContext;

		public JwtAuthHandler(ICurrentHttpContext currentHttpContext, IAuthService authService)
		{
			_currentHttpContext = currentHttpContext;
			_authService = authService;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var authHeader = request.Headers.Authorization;

			try
			{
				if (AuthHeaderIsValid(authHeader))
				{
					await SetIdentityAsync(authHeader.Parameter, request.RequestUri);
				}
			}
			catch(Exception e)
			{
				var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					RequestMessage = request,
					Content = new StringContent(e.Message)
				};

				return response;
			}

			return await base.SendAsync(request, cancellationToken);
		}

		private async Task SetIdentityAsync(string authHeaderParameter, Uri requestUri)
		{
			CardSortIdentity cardSortIdentity;

			try
			{
				cardSortIdentity = await _authService.GetIdentity(authHeaderParameter);
			}
			catch (Exception)
			{
				return;
			}

			if (cardSortIdentity == null)
			{
				// Add Logging here
				return;
			}

			CreateAndSetPrincipal(cardSortIdentity);
		}

		private void CreateAndSetPrincipal(CardSortIdentity cardSortIdentity)
		{
			var principal = _authService.CreatePrincipal(cardSortIdentity);
			Thread.CurrentPrincipal = principal;
			_currentHttpContext.SetPrincipal(principal);
		}

		private static bool AuthHeaderIsValid(AuthenticationHeaderValue authHeader)
		{
			return authHeader != null && authHeader.Scheme == "Bearer" && !string.IsNullOrEmpty(authHeader.Parameter);
		}
	}
}