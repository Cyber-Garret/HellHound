using System;
using System.Globalization;
using Xunit;

namespace Hound.InfrastructureTests;

public class RainbowRoleServiceTests
{
	[Theory]
	[InlineData("#008080", "008080", 32896)]
	public void Test1(string inputHex, string parsedHex, uint color)
	{
		var sut = new RainbowRoleService();

		var parseColor = sut.TryConvertHexStringToColor(inputHex, out var hexColor);

		Assert.Equal(parsedHex, hexColor);
		Assert.Equal(color, parseColor);
	}
}

public class RainbowRoleService
{
	public uint TryConvertHexStringToColor(string inputHex, out string parsedHex)
	{
		var hex = inputHex;

		if (hex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) ||
			hex.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
			hex = hex[2..];

		if (hex.StartsWith("#", StringComparison.CurrentCultureIgnoreCase))
			hex = hex[1..];

		parsedHex = hex;

		return uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var color)
			? color
			: 0;
	}
}
