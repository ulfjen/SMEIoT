using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// NodaTime specific extension methods for <see cref="NpgsqlDbContextOptionsBuilder"/>.
    /// </summary>
    public static class NpgsqlNodaTimeDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Use NetTopologySuite to access SQL Server spatial data.
        /// </summary>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static NpgsqlDbContextOptionsBuilder UseCustomNodaTime(
            this NpgsqlDbContextOptionsBuilder optionsBuilder)
        {
            // we disabled this from 3.1.0. We use connection setting which allow dapper to work.
            // NpgsqlConnection.GlobalTypeMapper.UseNodaTime();

            var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)optionsBuilder).OptionsBuilder;

            var extension = coreOptionsBuilder.Options.FindExtension<NpgsqlNodaTimeOptionsExtension>()
                            ?? new NpgsqlNodaTimeOptionsExtension();

            ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}
