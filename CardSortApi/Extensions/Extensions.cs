using System.Web.Http.Dependencies;

namespace CardSortApi.Extensions
{
	public static class Extensions
	{
		public static T GetService<T>(this IDependencyScope config)
		{
			return (T) config.GetService(typeof(T));
		}
	}
}