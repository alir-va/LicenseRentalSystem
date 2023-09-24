using LicenseRentalPage.Data.Models;

namespace LicenseRentalPage.Data
{
	public static class SampleLicenses
	{
		// Store a number of licenses in a persistent storage
		public static List<License> GetLicenses() => new()
		{
			new("abc123", "not rented"),
			new("12345", "not rented"),
			new("start", "not rented")
		};
	}
}
