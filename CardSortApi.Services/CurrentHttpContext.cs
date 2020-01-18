using System.Security.Principal;
using CardSortApi.Services.Interfaces;

namespace CardSortApi.Services
{
	public class CurrentHttpContext : ICurrentHttpContext
	{
		private readonly IHttpContextWrapperFactory _httpContextWrapperFactory;

		public CurrentHttpContext(IHttpContextWrapperFactory httpContextWrapperFactory)
		{
			_httpContextWrapperFactory = httpContextWrapperFactory;
		}

		public void SetPrincipal(IPrincipal principal)
		{
			_httpContextWrapperFactory.CreateHttpContextWrapper().User = principal;
		}
	}
}