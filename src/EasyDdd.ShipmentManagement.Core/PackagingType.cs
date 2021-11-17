using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core
{
	public class PackagingType : ValueObject, ISimpleValueObject<string>
	{
		private PackagingType(string code, string name)
		{
			Code = code;
			Name = name;
		}

		public string Code { get; }
		public string Name { get; }

		public static PackagingType Bag { get; } = new("BAG", "Bag");
		public static PackagingType Bale { get; } = new("BALE", "Bale");
		public static PackagingType Box { get; } = new("BOX", "Box");
		public static PackagingType Bucket { get; } = new("BUCKET", "Bucket");
		public static PackagingType Bundle { get; } = new("BUNDLE", "Bundle");
		public static PackagingType Can { get; } = new("CAN", "Can");
		public static PackagingType Carton { get; } = new("CARTON", "Carton");
		public static PackagingType Case { get; } = new("CASE", "Case");
		public static PackagingType Coil { get; } = new("COIL", "Coil");
		public static PackagingType Crate { get; } = new("CRATE", "Crate");
		public static PackagingType Cylinder { get; } = new("CYLINDER", "Cylinder");
		public static PackagingType Drum { get; } = new("DRUM", "Drums");
		public static PackagingType Pail { get; } = new("PAIL", "Pail");
		public static PackagingType Pieces { get; } = new("PIECES", "Pieces");
		public static PackagingType Pallet { get; } = new("PLT", "Pallet");
		public static PackagingType Reel { get; } = new("REEL", "Reel");
		public static PackagingType Roll { get; } = new("ROLL", "Roll");
		public static PackagingType Skid { get; } = new("SKID", "Skid");
		public static PackagingType Tube { get; } = new("TUBE", "Tube");

		public static IReadOnlyList<PackagingType> All { get; } = new[]
		{
			Bag,
			Bale,
			Box,
			Bucket,
			Bundle,
			Can,
			Carton,
			Case,
			Coil,
			Crate,
			Cylinder,
			Drum,
			Pail,
			Pieces,
			Pallet,
			Reel,
			Roll,
			Skid,
			Tube
		};

		string ISimpleValueObject<string>.Value => Code;

		protected override ITuple AsTuple()
		{
			return (Code, Name);
		}

		public static bool TryCreate(string? code, [NotNullWhen(true)] out PackagingType? packagingType, [NotNullWhen(false)] out string? errorMessage)
		{
			packagingType = All.SingleOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

			if (packagingType is null)
			{
				errorMessage = $"Unrecognized packaging type: '{code ?? "(NULL)"}'";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public static PackagingType Create(string code)
		{
			if (!TryCreate(code, out var packagingType, out var errorMessage)) throw new FormatException(errorMessage);

			return packagingType;
		}

		public static implicit operator PackagingType(string code)
		{
			return Create(code);
		}

		public static implicit operator string(PackagingType packagingType)
		{
			return packagingType.Code;
		}
	}
}