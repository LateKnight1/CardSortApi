using System.Configuration;
using CardSortApi.Services.Interfaces;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace CardSortApi.Configuration
{
	public class ApplicationConfiguration : IApplicationConfiguration
	{
		public string Issuer() => ConfigurationManager.AppSettings["issuer"];

		public byte[] Secret() => TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);
	}
}