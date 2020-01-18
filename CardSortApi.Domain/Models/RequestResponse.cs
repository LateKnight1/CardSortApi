namespace CardSortApi.Domain.Models
{
	public class RequestResponse<T>
	{
		public bool Succeeded { get; set; }
		public T ResponseBody { get; set; }
		public string ErrorMessage { get; set; }
	}
}