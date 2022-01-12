using EasyDdd.Billing.Core;
using EasyDdd.Billing.Core.ApproveStatement;
using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Web.Pages
{
	public class StatementSpotlightModel : PageModel
	{
		private readonly IReadModel<Statement> _readModel;
		private readonly IMediator _mediator;

		public StatementSpotlightModel(IReadModel<Statement> readModel,
			IMediator mediator)
		{
			_readModel = readModel;
			_mediator = mediator;
		}

		[FromQuery]
		public string? Id { get; set; }

		public Statement Statement { get; private set; } = default!;

		public async Task<IActionResult> OnGet()
		{
			if (Id == null)
			{
				return RedirectToPage("/errors/404", new { msg = "Statement was not found." });
			}

			var statement = await _readModel.Query(User)
				.Include(s => s.Lines)
				.SingleOrDefaultAsync(new StatementByIdSpecification(Id).ToExpression());

			if (statement == null)
			{
				return RedirectToPage("/errors/404", new { msg = $"Statement #{Id} was not found." });
			}

			Statement = statement;

			return Page();
		}

		public async Task<IActionResult> OnPost()
		{
			if (Id == null)
			{
				return RedirectToPage("/errors/404", new { msg = "Statement was not found." });
			}

			_ = await _mediator.Send(new ApproveStatementCommand(User, Id));

			return RedirectToPage("/Index");
		}
	}
}