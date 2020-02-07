using CardSortApi.Domain.Identity;
using CardSortApi.Repositories.Interfaces;
using CardSortApi.Repositories.Models;
using CardSortApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using User = CardSortApi.Repositories.User;

namespace CardSortApi.Services
{
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly IApplicationConfiguration _applicationConfiguration;

		public AuthService(IAuthRepository authRepository, IApplicationConfiguration applicationConfiguration)
		{
			_authRepository = authRepository;
			_applicationConfiguration = applicationConfiguration;
		}

		public async Task<AuthResponse> GetAuthentication(string username, string password)
		{
			var user = await _authRepository.GetUserByUsername(username);
			if (user.Password == password)
			{
				return await GenerateJwt(user);
			}
			throw new AuthenticationException("An invalid credential was passed");
		}

		public async Task<CardSortIdentity> GetIdentity(string authHeaderParameter)
		{
			var token = ParseJwt(authHeaderParameter);
			var claims = token.Claims;
			var claimsList = claims.ToList();
			var userId = claimsList.FirstOrDefault(x => x.Type == "nameid")?.Value;
			var username = claimsList.FirstOrDefault(x => x.Type == "username")?.Value;
			var user = await _authRepository.GetUser(userId);

			if (username != user.Username) return null;
			var identity = new CardSortIdentity(user.Name, "JWT", true, user.UserId.ToString(), user.Username);
			return identity;
		}

		public GenericPrincipal CreatePrincipal(CardSortIdentity cardSortIdentity)
		{
			var roles = new List<string>();

			if (string.IsNullOrEmpty(cardSortIdentity.Username))
				return new GenericPrincipal(cardSortIdentity, roles.ToArray());
			roles.Add("user");
			if (cardSortIdentity.Username.Equals("lateknight1"))
			{
				roles.Add("admin");
			}

			return new GenericPrincipal(cardSortIdentity, roles.ToArray());
		}

		private async Task<AuthResponse> GenerateJwt(User user)
		{
			var claims = await CreateClaims(user);
			var handler = new JwtSecurityTokenHandler();
			var issuer = _applicationConfiguration.Issuer();
			var secret = _applicationConfiguration.Secret();
			var credentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha512);
			var token = handler.CreateJwtSecurityToken(issuer, null, claims, DateTime.UtcNow,
				DateTime.UtcNow.AddDays(7), DateTime.UtcNow, credentials);
			user.Password = null;
			return new AuthResponse
			{
				User = user,
				Jwt = handler.WriteToken(token),
			};
		}

		private static Task<ClaimsIdentity> CreateClaims(User user)
		{
			var claims = new ClaimsIdentity();
			claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
			claims.AddClaim(new Claim(ClaimTypes.Name, user.Name));
			claims.AddClaim(new Claim("username", user.Username));

			return Task.FromResult(claims);
		}

		private JwtSecurityToken ParseJwt(string authHeaderParameter)
		{
			var handler = new JwtSecurityTokenHandler();
			return handler.ReadToken(authHeaderParameter) as JwtSecurityToken;
		}
	}
}