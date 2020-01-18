using System;
using System.Threading.Tasks;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories;
using CardSortApi.Repositories.Interfaces;
using CardSortApi.Repositories.Models;
using CardSortApi.Services.Interfaces;

namespace CardSortApi.Services
{
	public class AccountService : IAccountService
	{
		private readonly IAuthRepository _authRepository;
		private readonly IAuthService _authService;
		private readonly IEmailService _emailService;

		public AccountService(IAuthRepository authRepository, IAuthService authService, IEmailService emailService)
		{
			_authRepository = authRepository;
			_authService = authService;
			_emailService = emailService;
		}

		public async Task<RequestResponse<bool>> IsEmailValid(string email)
		{
			return await _authRepository.IsEmailValid(email);
		}

		public async Task<RequestResponse<bool>> IsUsernameValid(string username)
		{
			return await _authRepository.IsUsernameValid(username);
		}

		public async Task<RequestResponse<AuthResponse>> CreateAccount(string name, string email, string username, string password)
		{
			var user = GenerateUser(name, email, username, password);
			_emailService.SendWelcomeEmail(user);
			await _authRepository.CreateAccount(user);
			var response = await _authService.GetAuthentication(username, password);
			return new RequestResponse<AuthResponse>
			{
				Succeeded = true,
				ResponseBody = response
			};
		}

		public async Task<RequestResponse<bool>> ResendEmail(string username)
		{
			var user = await GetUserByUsername(username);

			return _emailService.ResendWelcomeEmail(user);
		}

		public async Task<RequestResponse<AuthResponse>> Login(string username, string password)
		{
			return new RequestResponse<AuthResponse>
			{
				ResponseBody = await _authService.GetAuthentication(username, password),
				Succeeded = true
			};
		}

		public async Task<RequestResponse<AuthResponse>> VerifyEmail(string username, string token)
		{
			var user = await _authRepository.GetUserByUsername(username);
			if (string.Compare(user.Verification_Token, token, StringComparison.CurrentCultureIgnoreCase) != 0)
				return new RequestResponse<AuthResponse>
				{
					Succeeded = false,
					ErrorMessage = "The Verification Token is invalid"
				};
			user = await _authRepository.SetVerificationStatus(user.UserId, true);
			return new RequestResponse<AuthResponse>
			{
				Succeeded = true,
				ResponseBody = await _authService.GetAuthentication(user.Username, user.Password)
			};
		}

		public async Task<User> GetUserByUsername(string username)
		{
			return await _authRepository.GetUserByUsername(username);
		}

		private static User GenerateUser(string name, string email, string username, string password)
		{
			var user = new User
			{
				Name = name,
				Email = email,
				Email_Verified = 0,
				Password = password,
				RefreshToken = Guid.NewGuid().ToString(),
				Username = username,
				Verification_Token = Guid.NewGuid().ToString()
			};

			return user;

		}
	}
}