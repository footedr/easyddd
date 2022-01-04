using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyDdd.Billing.Data;

public abstract class BillingConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
	where TEntity : class
{
	protected string Schema => "billing";
	protected abstract string TableName { get; }

	public void Configure(EntityTypeBuilder<TEntity> builder)
	{
		builder.ToTable(TableName, Schema);
		ConfigureEntity(builder);
	}

	protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}