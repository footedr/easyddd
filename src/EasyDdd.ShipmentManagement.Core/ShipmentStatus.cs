﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core
{
	public class ShipmentStatus : SimpleValueObject<string>
	{
		public static readonly ShipmentStatus New = new("NEW", "New");
		public static readonly ShipmentStatus Rated = new("RATED", "Rated");
		public static readonly ShipmentStatus Dispatched = new("DSP", "Dispatched");
		public static readonly ShipmentStatus InTransit = new("INTRANSIT", "InTransit");
		public static readonly ShipmentStatus Delivered = new("DLV", "Delivered");
		public static readonly ShipmentStatus Void = new("VOID", "Void");

		private ShipmentStatus(string code, string description) : base(code)
		{
			Description = description;
		}

		public string Code => Value;
		public string Description { get; }

		public static IReadOnlyList<ShipmentStatus> All => new[]
		{
			New,
			Rated,
			Dispatched,
			InTransit,
			Delivered
		};

		public static bool TryCreate(string? code, [NotNullWhen(true)] out ShipmentStatus? status, [NotNullWhen(false)] out string? errorMessage)
		{
			status = All.SingleOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

			if (status is null)
			{
				errorMessage = $"Unrecognized shipment status: '{code ?? "(NULL)"}'";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public static ShipmentStatus Create(string code)
		{
			if (!TryCreate(code, out var status, out var errorMessage)) throw new FormatException(errorMessage);

			return status;
		}

		public static bool operator < (ShipmentStatus left, ShipmentStatus right)
		{
			return All.ToList().IndexOf(left) < All.ToList().IndexOf(right);
		}

		public static bool operator > (ShipmentStatus left, ShipmentStatus right)
		{
			return All.ToList().IndexOf(left) > All.ToList().IndexOf(right);
		}

		public static bool operator <= (ShipmentStatus left, ShipmentStatus right)
		{
			return All.ToList().IndexOf(left) <= All.ToList().IndexOf(right);
		}

		public static bool operator >= (ShipmentStatus left, ShipmentStatus right)
		{
			return All.ToList().IndexOf(left) >= All.ToList().IndexOf(right);
		}
	}
}