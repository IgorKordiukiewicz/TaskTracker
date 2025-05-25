using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;

internal class DomainEntityTypeConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Ignore(e => e.Events);

        base.Configure(builder);
    }
}
