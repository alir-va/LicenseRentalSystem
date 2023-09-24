using LicenseRentalPage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LicenseRentalPage.Pages
{
	public class AddLicenseModel : PageModel
	{
		[BindProperty]
		public string? NewLicenseId { get; set; }
		public string? DisplayData { get; set; }

		private readonly ILicenseService _licenseService;

		public AddLicenseModel(ILicenseService licenseService)
		{
			_licenseService = licenseService;
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPost()
		{
			if (!string.IsNullOrWhiteSpace(NewLicenseId))
			{
				DisplayData = await _licenseService.AddLicense(NewLicenseId);
			}

			NewLicenseId = null;
			return Page();
		}
	}
}
