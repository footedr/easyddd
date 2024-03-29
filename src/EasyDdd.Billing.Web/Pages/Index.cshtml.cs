﻿using EasyDdd.Billing.Core;
using EasyDdd.Billing.Data.QueryHandlers;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EasyDdd.Billing.Web.Pages;

public class IndexModel : PageModel
{
	private readonly IOptions<BillingOptions> _billingOptions;
	private readonly IMediator _mediator;
	public IReadOnlyList<ShipmentListItem> IncompleteShipments = new List<ShipmentListItem>();
	public IReadOnlyList<PendingStatementListItem> PendingStatements = new List<PendingStatementListItem>();
	public IReadOnlyList<ProcessedStatementListItem> ProcessedStatements = new List<ProcessedStatementListItem>();

	public IndexModel(IMediator mediator, IOptions<BillingOptions> billingOptions)
	{
		_mediator = mediator;
		_billingOptions = billingOptions;
	}

	public async Task OnGet()
	{
		PendingStatements = (await _mediator.Send(new PendingStatementsQuery(User, _billingOptions.Value.CustomerCode, _billingOptions.Value.BillToAccount, _billingOptions.Value.BillToLocation))
				.ConfigureAwait(false))
			.Select(_ => new PendingStatementListItem(_.Identifier, _.BillToAccount, _.BillToLocation, _.BillingPeriod, _.CreatedAt.ToDateTimeUtc()))
			.OrderBy(_ => _.CreatedAt)
			.ToList();

		ProcessedStatements = (await _mediator.Send(new ProcessedStatementsQuery(User, _billingOptions.Value.CustomerCode, _billingOptions.Value.BillToAccount, _billingOptions.Value.BillToLocation))
				.ConfigureAwait(false))
			.Select(_ => new ProcessedStatementListItem(_.Identifier, _.BillToAccount, _.BillToLocation, _.BillingPeriod, _.ProcessedAt!.Value.ToDateTimeUtc()))
			.OrderBy(_ => _.ProcessedAt)
			.ToList();

		IncompleteShipments = (await _mediator.Send(new IncompleteShipmentsQuery(User))
				.ConfigureAwait(false))
			.Select(_ => new ShipmentListItem(_.Identifier, _.Status)
			{
				CarrierName = _.Carrier?.Name,
				DispatchNumber = _.DispatchInfo?.DispatchNumber,
				PickupNumber = _.DispatchInfo?.PickupNumber,
				TotalCost = _.TotalCost,
				DispatchDateTime = _.DispatchInfo?.DispatchDateTime
			})
			.OrderBy(_ => _.Identifier)
			.ToList();
	}
}