using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EasyDdd.ShipmentManagement.Web.Pages.Errors;

public class _404Model : PageModel
{
	public string? Message { get; set; }

	public void OnGet(string? msg)
	{
		Message = msg;
	}
}