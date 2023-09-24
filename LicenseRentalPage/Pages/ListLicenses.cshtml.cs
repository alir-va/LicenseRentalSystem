using LicenseRentalPage.Data;
using LicenseRentalPage.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LicenseRentalPage.Pages
{
    public class ListLicensesModel : PageModel
    {
		[BindProperty]
		public List<License>? DisplayData { get; set; }

		private readonly ILicenseService _licenseService;

		public ListLicensesModel(ILicenseService licenseService)
		{
			_licenseService = licenseService;
		}
		public async Task<IActionResult> OnGet()
        {
			DisplayData = await _licenseService.GetAll();
			return Page();
		}
    }
}
