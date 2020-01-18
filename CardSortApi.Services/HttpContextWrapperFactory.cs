using System.Web;
using CardSortApi.Services.Interfaces;

namespace CardSortApi.Services
{
	public class HttpContextWrapperFactory : IHttpContextWrapperFactory
	{
		public HttpContextBase CreateHttpContextWrapper() =>
			(HttpContext.Current == null) ? null : new HttpContextWrapper(HttpContext.Current);
	}
}