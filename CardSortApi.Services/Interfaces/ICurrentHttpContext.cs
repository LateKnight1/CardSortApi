using System.Security.Principal;

namespace CardSortApi.Services.Interfaces
{
	public interface ICurrentHttpContext
	{
		void SetPrincipal(IPrincipal principal);
	}
}
