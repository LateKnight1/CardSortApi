using System.Threading.Tasks;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories.Models;

namespace CardSortApi.Services.Interfaces
{
	public interface IAccountService
	{
		Task<RequestResponse<bool>> IsEmailValid(string email);
		Task<RequestResponse<bool>> IsUsernameValid(string username);
		Task<RequestResponse<AuthResponse>> CreateAccount(string name, string email, string username, string password);
		Task<RequestResponse<bool>> ResendEmail(string username);
		Task<RequestResponse<AuthResponse>> Login(string username, string password);
		Task<RequestResponse<AuthResponse>> VerifyEmail(string username, string token);
	}
}
