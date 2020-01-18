using System.Web;

namespace CardSortApi.Services.Interfaces
{
	public interface IHttpContextWrapperFactory
	{
		HttpContextBase CreateHttpContextWrapper();
	}
}
