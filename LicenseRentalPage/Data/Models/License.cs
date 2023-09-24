namespace LicenseRentalPage.Data.Models
{
	public class License
	{
		public License(string id, string status)
		{
			Id = id;
			Status = status;
		}

		public string Id { get; set; }
		public string Status { get; set; }
	}
}
