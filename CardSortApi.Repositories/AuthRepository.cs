using System.Data.Entity;
using System.Globalization;
using System.Threading.Tasks;
using CardSortApi.Domain.Exceptions;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories.Interfaces;

namespace CardSortApi.Repositories
{
	public class AuthRepository : IAuthRepository
	{
		public async Task<User> GetUser(string userId)
		{
			User user;
			using (var context = new UserEntities())
			{
				int.TryParse(userId, out var id);
				user = await context.Users.FirstOrDefaultAsync(x => x.UserId == id);
			}

			return user;
		}

		public async Task<User> GetUserByUsername(string username)
		{
			User user;
			using (var context = new UserEntities())
			{
				user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
			}

			return user;
		}

		public async Task<RequestResponse<bool>> IsEmailValid(string email)
		{
			User user;
			using (var context = new UserEntities())
			{
				user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
			}
			return new RequestResponse<bool>
			{
				Succeeded = true,
				ResponseBody = user == null
			};
		}

		public async Task<RequestResponse<bool>> IsUsernameValid(string username)
		{
			User user;
			using (var context = new UserEntities())
			{
				user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
			}
			return new RequestResponse<bool>
			{
				Succeeded = true,
				ResponseBody = user == null
			};
		}

		public async Task CreateAccount(User user)
		{
			using (var context = new UserEntities())
			{
				context.Users.Add(user);
				await context.SaveChangesAsync();
			}
		}

		public async Task<User> SetVerificationStatus(int userId, bool verificationStatus)
		{
			using (var context = new UserEntities())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
				if (user == null)
				{
					throw new NotFoundException("User not found");
				}
				user.Email_Verified = verificationStatus ? 1 : 0;
				await context.SaveChangesAsync();
				return user;
			}
		}
	}
}