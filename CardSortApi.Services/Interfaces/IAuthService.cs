using System.Security.Principal;
using System.Threading.Tasks;
using CardSortApi.Domain.Dto;
using CardSortApi.Domain.Identity;
using CardSortApi.Repositories.Models;

namespace CardSortApi.Services.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResponse> GetAuthentication(string username, string password);
		Task<CardSortIdentity> GetIdentity(string authHeaderParameter);
		GenericPrincipal CreatePrincipal(CardSortIdentity cardSortIdentity);
	}
}
