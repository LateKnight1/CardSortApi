using System;
using System.Threading.Tasks;
using CardSortApi.Domain.Exceptions;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories;
using CardSortApi.Repositories.Interfaces;
using CardSortApi.Repositories.Models;
using CardSortApi.Services.Interfaces;
using Contacts.TestHelpers;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace CardSortApi.Services.Tests
{
	public class AccountServiceTests : BaseAutoMocker<AccountService>
	{
		private IAuthRepository _authRepository;
		private IAuthService _authService;
		private IEmailService _emailService;

		public override void OnTestInitializing()
		{
			_authRepository = Get<IAuthRepository>();
			_authService = Get<IAuthService>();
			_emailService = Get<IEmailService>();
		}

		[TestFixture]
		public class IsEmailValid : AccountServiceTests
		{
			[Test]
			public async Task ReturnsTrue_WhenEmailValid()
			{
				var requestResponse = DynamicTestHelper<RequestResponse<bool>>.Get();
				requestResponse.ResponseBody = true;

				_authRepository
					.Stub(x => x.IsEmailValid(Arg<string>.Is.Anything))
					.Return(Task.FromResult(requestResponse));

				var results = await _classUnderTest.IsEmailValid("test");

				results.Should().NotBeNull();
				results.ResponseBody.Should().BeTrue();
			}

			[Test]
			public void ShouldThrowException_OnError()
			{
				_authRepository
					.Stub(x => x.IsEmailValid(Arg<string>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.IsEmailValid("test"));
			}
		}

		[TestFixture]
		public class IsUsernameValid : AccountServiceTests
		{
			[Test]
			public async Task ReturnsTrue_WhenUsernameValid()
			{
				var requestResponse = DynamicTestHelper<RequestResponse<bool>>.Get();
				requestResponse.ResponseBody = true;

				_authRepository
					.Stub(x => x.IsUsernameValid(Arg<string>.Is.Anything))
					.Return(Task.FromResult(requestResponse));

				var response = await _classUnderTest.IsUsernameValid("tester");

				response.Should().NotBeNull();
				response.ResponseBody.Should().BeTrue();
			}

			[Test]
			public void ShouldThrowException_OnError()
			{
				_authRepository
					.Stub(x => x.IsUsernameValid(Arg<string>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.IsUsernameValid("tester"));
			}
		}

		[TestFixture]
		public class CreateAccount:AccountServiceTests
		{
			[Test]
			public void ThrowsError_WithNullInputs()
			{
				Assert.ThrowsAsync<ValidationException>(async () =>
					await _classUnderTest.CreateAccount(null, "email", null, "password"));
			}

			[Test]
			public void ThrowErrorInSendWelcomeEmail()
			{
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();
				var username = DynamicTestHelper<string>.Get();
				var password = DynamicTestHelper<string>.Get();

				_emailService
					.Stub(x => x.SendWelcomeEmail(Arg<User>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () =>
					await _classUnderTest.CreateAccount(name, email, username, password));
			}

			[Test]
			public void ThrowError_WhenCreateAccountFails()
			{
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();
				var username = DynamicTestHelper<string>.Get();
				var password = DynamicTestHelper<string>.Get();

				_emailService
					.Stub(c => c.SendWelcomeEmail(Arg<User>.Is.Anything));
				_authRepository
					.Stub(x => x.CreateAccount(Arg<User>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () =>
					await _classUnderTest.CreateAccount(name, email, username, password));
			}

			[Test]
			public void ThrowError_WhenGetAuthenticationFails()
			{
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();
				var username = DynamicTestHelper<string>.Get();
				var password = DynamicTestHelper<string>.Get();

				_emailService
					.Stub(x => x.SendWelcomeEmail(Arg<User>.Is.Anything));
				_authRepository
					.Stub(x => x.CreateAccount(Arg<User>.Is.Anything))
					.Return(Task.FromResult<string>(null));
				_authService
					.Stub(x => x.GetAuthentication(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () =>
					await _classUnderTest.CreateAccount(name, email, username, password));
			}

			[Test]
			public async Task ShouldReturnAValidResponse_WhenNoErrors()
			{
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();
				var username = DynamicTestHelper<string>.Get();
				var password = DynamicTestHelper<string>.Get();
				var authResponse = DynamicTestHelper<AuthResponse>.Get();

				_emailService
					.Stub(x => x.SendWelcomeEmail(Arg<User>.Is.Anything));
				_authRepository
					.Stub(x => x.CreateAccount(Arg<User>.Is.Anything))
					.Return(Task.FromResult<string>(null));
				_authService
					.Stub(x => x.GetAuthentication(Arg<string>.Is.Anything,
						Arg<string>.Is.Anything))
					.Return(Task.FromResult(authResponse));

				var response = await _classUnderTest.CreateAccount(name, email, username, password);
				response.ResponseBody.Should().NotBeNull();
				response.Succeeded.Should().BeTrue();
			}
		}

		[TestFixture]
		public class ResendEmail:AccountServiceTests
		{
			[Test]
			public void ThrowsError_WhenGetUserFails()
			{
				_authRepository
					.Stub(x => x.GetUserByUsername(Arg<string>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.ResendEmail("userName"));
			}

			[Test]
			public async Task ReturnsValue_NoErrorsEncountered()
			{
				var user = DynamicTestHelper<User>.Get();
				var requestResponse = DynamicTestHelper<RequestResponse<bool>>.Get();
				requestResponse.ResponseBody = true;

				_authRepository
					.Stub(x => x.GetUserByUsername(Arg<string>.Is.Anything))
					.Return(Task.FromResult(user));

				_emailService
					.Stub(x => x.ResendWelcomeEmail(user))
					.Return(requestResponse);

				var response = await _classUnderTest.ResendEmail("doughnutChef");

				response.Should().NotBeNull();
				response.ResponseBody.Should().BeTrue();
			}
		}

		[TestFixture]
		public class Login:AccountServiceTests
		{
			[Test]
			public void ThrowsError_WhenGetAuthenticationFails()
			{
				_authService
					.Stub(x => x.GetAuthentication(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
					.Throw(new NotFoundException("No User"));

				Assert.ThrowsAsync<NotFoundException>(async () => await _classUnderTest.Login("username", "password"));
			}

			[Test]
			public async Task ReturnsAnAuthResponse_WhenAuthenticationSucceeds()
			{
				var authResponse = DynamicTestHelper<AuthResponse>.Get();

				_authService
					.Stub(x => x.GetAuthentication(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
					.Return(Task.FromResult(authResponse));

				var response = await _classUnderTest.Login("username", "password");
				response.Should().NotBeNull();
				response.ResponseBody.Should().BeOfType<AuthResponse>();
				response.ResponseBody.Jwt.Should().NotBeNull();
			}
		}

		[TestFixture]
		public class VerifyEmail:AccountServiceTests
		{
			[Test]
			public void ThrowsError_WhenNoUserFoundByUsername()
			{
				_authRepository
					.Stub(x => x.GetUserByUsername(Arg<string>.Is.Anything))
					.Throw(new NotFoundException("No User"));

				Assert.ThrowsAsync<NotFoundException>(
					async () => await _classUnderTest.VerifyEmail("username", "token"));
			}

			[Test]
			public async Task ShouldReturnFailureWithMessage_TokensAreNotSame()
			{
				var token1 = DynamicTestHelper<string>.Get();
				const string token2 = "completely random junk";

				var user = DynamicTestHelper<User>.Get();
				user.Verification_Token = token2;

				_authRepository
					.Stub(x => x.GetUserByUsername(Arg<string>.Is.Anything))
					.Return(Task.FromResult(user));

				var response = await _classUnderTest.VerifyEmail("username", token1);

				response.Succeeded.Should().BeFalse();
				response.ErrorMessage.Should().Be("The Verification Token is invalid");
			}

			[Test]
			public void ShouldThrowError_WhenSaveStatusFails()
			{
				var token = DynamicTestHelper<string>.Get();
				var user = DynamicTestHelper<User>.Get();
				user.Verification_Token = token;
				user.Email_Verified = 0;

				var user2 = DynamicTestHelper<User>.Get();
				user2.Username = user.Username;
				user2.UserId = user.UserId;
				user2.Email_Verified = 1;

				_authRepository
					.Stub(x => x.GetUserByUsername(user.Username))
					.Return(Task.FromResult(user));

				_authRepository
					.Stub(x => x.SetVerificationStatus(user.UserId, true))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.VerifyEmail(user.Username, token));
			}

			[Test]
			public void ThrowsError_WhenGetAuthenticationFails()
			{
				var token = DynamicTestHelper<string>.Get();
				var user = DynamicTestHelper<User>.Get();
				user.Verification_Token = token;
				user.Email_Verified = 0;

				var user2 = DynamicTestHelper<User>.Get();
				user2.Username = user.Username;
				user2.UserId = user.UserId;
				user2.Email_Verified = 1;

				_authRepository
					.Stub(x => x.GetUserByUsername(user.Username))
					.Return(Task.FromResult(user));
				_authRepository
					.Stub(x => x.SetVerificationStatus(user.UserId, true))
					.Return(Task.FromResult(user2));
				_authService
					.Stub(x => x.GetAuthentication(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.VerifyEmail(user.Username, token));
			}

			[Test]
			public async Task ShouldReturnResponse_WhenAllTasksSucceed()
			{
				var token = DynamicTestHelper<string>.Get();
				var user = DynamicTestHelper<User>.Get();
				user.Verification_Token = token;
				user.Email_Verified = 0;

				var user2 = DynamicTestHelper<User>.Get();
				user2.Username = user.Username;
				user2.UserId = user.UserId;
				user2.Email_Verified = 1;

				var authResponse = DynamicTestHelper<AuthResponse>.Get();
				authResponse.User = user2;

				_authRepository
					.Stub(x => x.GetUserByUsername(user.Username))
					.Return(Task.FromResult(user));
				_authRepository
					.Stub(x => x.SetVerificationStatus(user.UserId, true))
					.Return(Task.FromResult(user2));
				_authService
					.Stub(x => x.GetAuthentication(user2.Username, user2.Password))
					.Return(Task.FromResult(authResponse));

				var response = await _classUnderTest.VerifyEmail(user.Username, token);

				response.Should().NotBeNull();
				response.ResponseBody.Should().NotBeNull();
				response.ResponseBody.User.Should().Be(user2);
			}
		}

		[TestFixture]
		public class UpdateAccount:AccountServiceTests
		{
			[Test]
			public void ShouldThrowValidationException_InputIsNull()
			{
				Assert.ThrowsAsync<ValidationException>(async () => await _classUnderTest.UpdateAccount("123", null, "testEmail"));
			}

			[Test]
			public void ShouldThrowError_WhenUpdateFails()
			{
				var userId = DynamicTestHelper<string>.Get();
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();

				_authRepository
					.Stub(x => x.UpdateAccount(userId, name, email))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.UpdateAccount(userId, name, email));
			}

			[Test]
			public void ShouldThrowError_WhenGetAuthenticationFails()
			{
				var userId = DynamicTestHelper<string>.Get();
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();

				var user = DynamicTestHelper<User>.Get();

				_authRepository
					.Stub(x => x.UpdateAccount(userId, name, email))
					.Return(Task.FromResult(user));
				_authService
					.Stub(x => x.GetAuthentication(user.Username, user.Password))
					.Throw(new Exception());

				Assert.ThrowsAsync<Exception>(async () => await _classUnderTest.UpdateAccount(userId, name, email));
			}

			[Test]
			public async Task ShouldReturnAuthResponse_WhenNoErrors()
			{
				var userId = DynamicTestHelper<string>.Get();
				var name = DynamicTestHelper<string>.Get();
				var email = DynamicTestHelper<string>.Get();
				var user = DynamicTestHelper<User>.Get();
				var authResponse = DynamicTestHelper<AuthResponse>.Get();
				authResponse.User = user;

				_authRepository
					.Stub(x => x.UpdateAccount(userId, name, email))
					.Return(Task.FromResult(user));
				_authService
					.Stub(x => x.GetAuthentication(user.Username, user.Password))
					.Return(Task.FromResult(authResponse));

				var results = await _classUnderTest.UpdateAccount(userId, name, email);

				results.Should().NotBeNull();
				results.ResponseBody.User.Should().Be(user);
			}
		}

		[TestFixture]
		public class GetUserByUsername:AccountServiceTests
		{
			[Test]
			public void ShouldThrowException_WhenUserNotFound()
			{
				_authRepository
					.Stub(x => x.GetUserByUsername(Arg<string>.Is.Anything))
					.Throw(new NotFoundException("No user found"));

				Assert.ThrowsAsync<NotFoundException>(async () => await _classUnderTest.GetUserByUsername("tester"));
			}

			[Test]
			public async Task ShouldReturnUser_WhenNoErrors()
			{
				var user = DynamicTestHelper<User>.Get();
				user.Username = "tester";
				_authRepository
					.Stub(x => x.GetUserByUsername(user.Username))
					.Return(Task.FromResult(user));

				var results = await _classUnderTest.GetUserByUsername("tester");

				results.Should().NotBeNull();
				results.Username.Should().Be("tester");
			}
		}
	}
}
