using System.Drawing;

namespace Hound.Bot.Extensions;

public static class UintExtensions
{
	/// <summary>
	/// Help convert uint color value to <see cref="Color"/>
	/// </summary>
	public static Color UIntToColor(this uint color)
	{
		var a = (byte)(color >> 24);
		var r = (byte)(color >> 16);
		var g = (byte)(color >> 8);
		var b = (byte)(color >> 0);

		return Color.FromArgb(a, r, g, b);
	}
}