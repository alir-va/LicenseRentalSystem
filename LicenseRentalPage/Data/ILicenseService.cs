using LicenseRentalPage.Data.Models;

namespace LicenseRentalPage.Data
{
	public interface ILicenseService
	{
		Task<List<License>> GetAll();
		Task<string> AddLicense(string newLicenseId);
		Task<string> Rent(string RequestId);
	}
}
