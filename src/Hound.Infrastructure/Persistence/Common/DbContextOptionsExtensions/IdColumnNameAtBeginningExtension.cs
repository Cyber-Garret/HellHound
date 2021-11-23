using System.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Hound.Infrastructure.Persistence.Common.DbContextOptionsExtensions;

internal class IdColumnNameAtBeginningExtension : IDbContextOptionsExtension
{
	private DbContextOptionsExtensionInfo? _info;

	public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

	public void ApplyServices(IServiceCollection serviceCollection)
	{
		new EntityFrameworkServicesBuilder(serviceCollection)
			.TryAdd<IConventionSetPlugin, Plugin>();
	}

	public void Validate(IDbContextOptions options)
	{
	}

	private sealed class Plugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			var convention = new Convention();
			conventionSet.PropertyAddedConventions.Add(convention);
			conventionSet.ForeignKeyAddedConventions.Add(convention);
			conventionSet.IndexAddedConventions.Add(convention);
			return conventionSet;
		}

		private sealed class Convention : IPropertyAddedConvention, IForeignKeyAddedConvention, IIndexAddedConvention
		{
			public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
			{
				//https://github.com/dotnet/efcore/issues/23301
				var storeObjectId =
					StoreObjectIdentifier.Create(propertyBuilder.Metadata.DeclaringEntityType, StoreObjectType.Table);

				var newName = GetNewColumnName(storeObjectId.GetValueOrDefault().Name);
				propertyBuilder.HasColumnName(newName, fromDataAnnotation: false);
			}

			public void ProcessIndexAdded(IConventionIndexBuilder indexBuilder, IConventionContext<IConventionIndexBuilder> context)
			{
				var newName = GetNewConstraintName(indexBuilder.Metadata.Properties,
					indexBuilder.Metadata.GetDatabaseName());
				indexBuilder.HasDatabaseName(newName, fromDataAnnotation: false);
			}

			private static string? GetNewConstraintName(IReadOnlyList<IConventionProperty> properties, string? currentName)
			{
				foreach (var property in properties)
				{
					var propertyName = property.Name;
					var newPropertyName = GetNewColumnName(propertyName);
					if (!ReferenceEquals(newPropertyName, propertyName))
						currentName = currentName?.Replace(propertyName, newPropertyName);
				}

				return currentName;
			}

			private static string GetNewColumnName(string columnName) =>
				columnName.Length > 2 && columnName.EndsWith("Id")
					? $"Id{columnName[..^2]}"
					: columnName;


			public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
			{
				var newName = GetNewConstraintName(foreignKeyBuilder.Metadata.Properties,
					foreignKeyBuilder.Metadata.GetConstraintName());
				foreignKeyBuilder.HasConstraintName(newName, fromDataAnnotation: false);
			}
		}
	}

	private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
	{
		public ExtensionInfo(IDbContextOptionsExtension extension)
			: base(extension)
		{
		}

		public override bool IsDatabaseProvider { get; } = false;

		public override string LogFragment { get; } = "\nusing id column name at beginning, ";

		public override int GetServiceProviderHashCode() => Extension.GetHashCode();
		public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) =>
			other is ExtensionInfo;

		public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
		{
			debugInfo["Naming:UseIdColumnNameAtBeginning"] =
				Extension.GetHashCode().ToString(CultureInfo.InvariantCulture);
		}
	}
}
