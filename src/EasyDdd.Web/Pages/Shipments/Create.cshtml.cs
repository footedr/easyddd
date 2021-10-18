using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyDdd.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyDdd.Web.Pages.Shipments
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;

		[BindProperty]
		public ShipmentRequest ShipmentRequest { get; set; } = new();

		public readonly SelectList StateList;

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
			StateList = new(Shared.States.All, "Name", "Abbreviation");
		}

        public void OnGet()
        {

        }

		public void OnPost()
		{
			
		}
    }
}
