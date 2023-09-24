using LicenseRentalPage.Data.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using License = LicenseRentalPage.Data.Models.License;

namespace LicenseRentalPage.Data
{
	public class LicenseService : ILicenseService
	{
		private readonly IConfiguration configuration;
		private readonly IMemoryCache memoryCache;
		private readonly string keys = "Licenses";

		public LicenseService(IConfiguration configuration, IMemoryCache memoryCache)
		{
			this.configuration = configuration;
			this.memoryCache = memoryCache;
		}

		/// <summary>
		/// Add licenses on the server
		/// </summary>
		/// <param name="newLicenseId">License id as unique alphanumeric strings.</param>
		/// <returns>Result of the request</returns>
		public async Task<string> AddLicense(string newLicenseId)
		{
			List<License> licenseList = await GetAll();
			License? license = licenseList.Where(l => l.Id == newLicenseId).FirstOrDefault();

			if (license == null)
			{
				licenseList.Add(new(newLicenseId, "not rented"));
				licenseList = memoryCache.Set(keys, licenseList);
			}

			return await Task.FromResult(license == null ? $"License '{newLicenseId}' added" : $"No license added, license with Id:'{newLicenseId}' already exists");
		}

		/// <summary>
		/// Retrieve all licenses
		/// If there is no licenses, retrieve licenses from persistent storage
		/// </summary>
		/// <returns>Licenses</returns>
		public async Task<List<License>> GetAll()
		{
			if (!memoryCache.TryGetValue(keys, out List<License>? Licenses))
			{
				if (Licenses == null)
					Licenses = SampleLicenses.GetLicenses();
				var cacheEntryOptions = new MemoryCacheEntryOptions();
				memoryCache.Set(keys, Licenses, cacheEntryOptions);
			}

			return await Task.FromResult(memoryCache.Get<List<License>>(keys) ?? new List<License>());
		}

		/// <summary>
		/// Each license can only be assigned to a single client at a time
		/// If there are any unassigned license(s), one should be given to the client
		/// </summary>
		/// <param name="requestId">The client that want to rent a license</param>
		/// <returns></returns>
		public async Task<string> Rent(string requestId)
		{
			List<License> licenseList = await GetAll();
			if (licenseList.Count == 0)
				return await Task.FromResult("No licens exist to rent");

			if (licenseList.Where(l => l.Status.Contains(requestId)).FirstOrDefault() != null)
				return await Task.FromResult("You have already rented a licens");

			License? license = licenseList.Where(l => l.Status == "not rented").FirstOrDefault();

			if (license != null)
			{
				_ = Task.Run(() => LicenseExpiresCountDown(license, requestId));
			}

			return await Task.FromResult(license != null ? $"You received license {license.Id}" : "No free license");
		}

		/// <summary>
		/// Licenses should expire after 15 seconds from rental.
		/// This license is now available for rental again.
		/// </summary>
		/// <param name="license">The rented license</param>
		/// <param name="requestId">The client that is renting the license</param>
		/// <returns></returns>
		private async Task LicenseExpiresCountDown(License license, string requestId)
		{
			List<License> licenseList = await GetAll();

			for (int i = 15; i > 0; i--)
			{
				licenseList.Where((l) => l.Id == license.Id).ToList().ForEach(l => l.Status = $"{requestId} ,{i} seconds left");
				licenseList = memoryCache.Set(keys, licenseList);
				Thread.Sleep(1000);
			}

			// Update license status when the license is available for rental again
			licenseList.Where((l) => l.Id == license.Id).ToList().ForEach(l => l.Status = "not rented");
			licenseList = memoryCache.Set(keys, licenseList);

			// Log when the license expires
			string logPath = configuration.GetValue<string>("LogPath") ?? "Storage\\Log.txt";
			string logMessages = $"License '{license.Id}' expired at {DateTime.Now}\n";
			await File.AppendAllTextAsync(logPath, logMessages);
		}
	}
}
