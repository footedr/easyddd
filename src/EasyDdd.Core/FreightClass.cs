using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EasyDdd.Kernel;

namespace EasyDdd.Core
{
	public class FreightClass : SimpleValueObject<string>
	{
		private FreightClass(string value) : base(value)
		{
		}

		public static FreightClass Class50 { get; } = new("50");
		public static FreightClass Class55 { get; } = new("55");
		public static FreightClass Class60 { get; } = new("60");
		public static FreightClass Class65 { get; } = new("65");
		public static FreightClass Class70 { get; } = new("70");
		public static FreightClass Class77_5 { get; } = new("77.5");
		public static FreightClass Class85 { get; } = new("85");
		public static FreightClass Class92_5 { get; } = new("92.5");
		public static FreightClass Class100 { get; } = new("100");
		public static FreightClass Class110 { get; } = new("110");
		public static FreightClass Class125 { get; } = new("125");
		public static FreightClass Class150 { get; } = new("150");
		public static FreightClass Class175 { get; } = new("175");
		public static FreightClass Class200 { get; } = new("200");
		public static FreightClass Class250 { get; } = new("250");
		public static FreightClass Class300 { get; } = new("300");
		public static FreightClass Class400 { get; } = new("400");
		public static FreightClass Class500 { get; } = new("500");

		public static IReadOnlyList<FreightClass> All { get; } = new[]
		{
			Class50,
			Class55,
			Class60,
			Class65,
			Class70,
			Class77_5,
			Class85,
			Class92_5,
			Class100,
			Class110,
			Class125,
			Class150,
			Class175,
			Class200,
			Class250,
			Class300,
			Class400,
			Class500
		};

		public static bool TryCreate(string? value, [NotNullWhen(true)] out FreightClass? freightClass, [NotNullWhen(false)] out string? errorMessage)
		{
			freightClass = All.SingleOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

			if (freightClass == null)
			{
				errorMessage = $"Unrecognized freight class: '{value ?? "(NULL)"}'";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public static FreightClass Create(string value)
		{
			if (!TryCreate(value, out var freightClass, out var errorMessage)) throw new FormatException(errorMessage);

			return freightClass;
		}

		public static implicit operator FreightClass(string freightClassValue)
		{
			return Create(freightClassValue);
		}

		public static implicit operator string(FreightClass freightClass)
		{
			return freightClass.Value;
		}
	}
}