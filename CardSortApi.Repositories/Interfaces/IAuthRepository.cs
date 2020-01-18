using System.Threading.Tasks;
using CardSortApi.Domain.Models;

namespace CardSortApi.Repositories.Interfaces
{
	public interface IAuthRepository
	{
		Task<User> GetUser(string userId);
		Task<User> GetUserByUsername(string username);
		Task<RequestResponse<bool>> IsEmailValid(string email);
		Task<RequestResponse<bool>> IsUsernameValid(string username);
		Task CreateAccount(User user);
		Task<User> SetVerificationStatus(int userId, bool verificationStatus);
	}
}
