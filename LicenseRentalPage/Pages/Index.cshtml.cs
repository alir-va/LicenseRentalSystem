using LicenseRentalPage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LicenseRentalPage.Pages
{
	public class IndexModel : PageModel
	{
		[BindProperty]
		public string? RentInfo { get; set; }
		public string? RequestId { get; set; }

		private readonly ILicenseService _licenseService;

		public IndexModel(ILicenseService licenseService)
		{
			_licenseService = licenseService;
		}

		public IActionResult OnGet()
		{
			RequestId = GetClientName();
			RentInfo = "";
			return Page();
		}

		public IActionResult OnPost()
		{
			RequestId = GetClientName();
			RentInfo = _licenseService.Rent(RequestId).Result;
			//Expired();
			return Page();
		}

		public string GetClientName()
		{
			return HttpContext.Connection.Id ?? "Client unknow";
		}
	}
}