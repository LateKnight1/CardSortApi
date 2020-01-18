using System.Threading.Tasks;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories;

namespace CardSortApi.Services.Interfaces
{
	public interface IEmailService
	{
		void SendWelcomeEmail(User user);
		RequestResponse<bool> ResendWelcomeEmail(User user);
	}
}
