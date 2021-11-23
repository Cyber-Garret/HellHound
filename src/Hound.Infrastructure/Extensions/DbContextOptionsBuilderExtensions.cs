using Hound.Infrastructure.Persistence.Common.DbContextOptionsExtensions;

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

internal static class DbContextOptionsBuilderExtensions
{
	private static readonly IdColumnNameAtBeginningExtension ExtensionInstance = new();

	public static DbContextOptionsBuilder UseIdColumnNameAtBeginning(
		this DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder == null)
			throw new ArgumentNullException(nameof(optionsBuilder));

		((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
			.AddOrUpdateExtension(ExtensionInstance);
		return optionsBuilder;
	}
}
