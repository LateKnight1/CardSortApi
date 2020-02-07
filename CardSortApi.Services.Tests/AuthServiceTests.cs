using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using CardSortApi.Domain.Exceptions;
using CardSortApi.Domain.Identity;
using CardSortApi.Repositories;
using CardSortApi.Repositories.Interfaces;
using CardSortApi.Repositories.Models;
using CardSortApi.Services.Interfaces;
using Contacts.TestHelpers;
using FluentAssertions;
using Microsoft.Owin.Security.DataHandler.Encoder;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace CardSortApi.Services.Tests
{
	public class AuthServiceTests : BaseAutoMocker<AuthService>
	{
		private IAuthRepository _authRepository;
		private IApplicationConfiguration _applicationConfiguration;
		private const string SecretString = "testSecretForLotsOfImportantHappyTests";

		private const string TestJwt =
			"eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxMjM2NSIsInVzZXJuYW1lIjoidGVzdGVyTWNUZXN0ZXJzb24iLCJpYXQiOjE1MTYyMzkwMjJ9.NS7vAir6sXNgoouR9APNaCaUndVUPX2stQPgITXOAQD28qVlKKCvt688oRgaty-KSgwWtgr0tApOH8oRYc0TuA";

		public override void OnTestInitializing()
		{
			_authRepository = Get<IAuthRepository>();
			_applicationConfiguration = Get<IApplicationConfiguration>();
		}

		[TestFixture]
		public class GetAuthentication:AuthServiceTests
		{
			[Test]
			public void ShouldThrowNotFoundException_WhenUserNotFound()
			{
				var username = DynamicTestHelper<string>.Get();
				var password = DynamicTestHelper<string>.Get();

				_authRepository
					.Stub(x => x.GetUserByUsername(username))
					.Throw(new NotFoundException("No user found"));

				Assert.ThrowsAsync<NotFoundException>(async () => await _classUnderTest.GetAuthentication(username, password));
			}

			[Test]
			public void ThrowsAuthenticationException_WhenPasswordsDoNotMatch()
			{
				var username = DynamicTestHelper<string>.Get();
				const string password = "Incorrect";
				var user = DynamicTestHelper<User>.Get();

				_authRepository
					.Stub(x => x.GetUserByUsername(username))
					.Return(Task.FromResult(user));

				Assert.ThrowsAsync<AuthenticationException>(async () => await _classUnderTest.GetAuthentication(username, password));
			}

			[Test]
			public async Task ReturnsResponse_WhenAuthenticationSucceeds()
			{
				var username = DynamicTestHelper<string>.Get();
				const string password = "Incorrect";
				var user = DynamicTestHelper<User>.Get();
				user.Username = username;
				user.Password = password;
				var issuer = DynamicTestHelper<string>.Get();
				var secret = TextEncodings.Base64Url.Decode(SecretString);

				_authRepository
					.Stub(x => x.GetUserByUsername(username))
					.Return(Task.FromResult(user));
				_applicationConfiguration
					.Stub(x => x.Issuer())
					.Return(issuer);
				_applicationConfiguration
					.Stub(x => x.Secret())
					.Return(secret);

				var results = await _classUnderTest.GetAuthentication(username, password);

				results.Should().BeOfType<AuthResponse>();
				results.User.Should().Be(user);
				results.Jwt.Should().NotBeNull();
			}
		}

		[TestFixture]
		public class GetIdentity:AuthServiceTests
		{
			[Test]
			public async Task ShoudlReturnIdentity_WhenJwtValid()
			{
				var user = DynamicTestHelper<User>.Get();
				user.UserId = 12365;
				user.Username = "testerMcTesterson";

				_authRepository
					.Stub(x => x.GetUser(user.UserId.ToString()))
					.Return(Task.FromResult(user));
				
				var results = await _classUnderTest.GetIdentity(TestJwt);

				results.UserId.Should().Be(user.UserId.ToString());
				results.Username.Should().Be(user.Username);
			}

			[Test]
			public void ThrowsNotFoundException_WhenNoUserFoundWithId()
			{
				_authRepository
					.Stub(x => x.GetUser(Arg<string>.Is.Anything))
					.Throw(new NotFoundException("No User found with that id"));

				Assert.ThrowsAsync<NotFoundException>(async () => await _classUnderTest.GetIdentity(TestJwt));
			}

			[Test]
			public void ShouldThrowError_WhenJwtInvalid()
			{
				Assert.ThrowsAsync<ArgumentException>(async () => await _classUnderTest.GetIdentity("test"));
			}
		}
		[TestFixture]
		public class CreatePrincipal:AuthServiceTests
		{
			[Test]
			public void ShouldNotHaveAnyRolesIfUsernameNull()
			{
				var cardSortIdentity = new CardSortIdentity("tester", "JWT", true, "123", null);

				var results = _classUnderTest.CreatePrincipal(cardSortIdentity);
				results.Claims.Count(x => x.Type == ClaimTypes.Role).Should().Be(0);
			}

			[Test]
			public void ShouldOnlyHaveUserRole_WhenUsernameIsNotAdminUsername()
			{
				var identity = new CardSortIdentity(null, null, true, null, "puppyLover15");

				var results = _classUnderTest.CreatePrincipal(identity);

				results.Claims.Count(x => x.Type == ClaimTypes.Role).Should().Be(1);
				results.Claims.Where(x => x.Type == ClaimTypes.Role).Select(c => c.Value).Should().NotBeNull().And
					.Equal("user");
			}

			[Test]
			public void ShouldHaveUserRoleAndAdminRole_WhenUsernameIsAdminUsername()
			{
				var identity = new CardSortIdentity(null, null, true, null, "lateknight1");

				var results = _classUnderTest.CreatePrincipal(identity);

				results.Claims.Count(x => x.Type == ClaimTypes.Role).Should().Be(2);
				results.Claims.Where(x => x.Type == ClaimTypes.Role).Select(c => c.Value).First().Should().Be("user");
				results.Claims.Where(x => x.Type == ClaimTypes.Role).Select(c => c.Value).Last().Should().Be("admin");
			}
		}
	}
}
