using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Npgsql;
using Simplify.ORM.Builders;
using Simplify.ORM.Interfaces;
using System.Data;

namespace Simplify.ORM.DependencyInjection
{
    public static class DatabaseConfigurator
    {
        public static void AddSQLServer(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnection>(provider => new SqlConnection(connectionString));

            services.AddTransient<ISimplifyQueryBuilder, SimplifySQLServerQueryBuilder>();
            services.AddTransient<ISimplifyCommandBuilder, SimplifySQLServerCommandBuilder>();
            services.AddTransient<ISimplifyExecutor, ISimplifyExecutor>();
            services.AddTransient(typeof(ISimplifyRepository<>), typeof(SimplifyRepository<>));
        }

        public static void AddPostgresSQL(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnection>(provider => new NpgsqlConnection(connectionString));

            services.AddTransient<ISimplifyQueryBuilder, SimplifyPostgresSQLQueryBuilder>();
            services.AddTransient<ISimplifyCommandBuilder, SimplifyPostgresSQLCommandBuilder>();
            services.AddTransient<ISimplifyExecutor, ISimplifyExecutor>();
            services.AddTransient(typeof(ISimplifyRepository<>), typeof(SimplifyRepository<>));
        }

        public static void AddMySQL(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnection>(provider => new MySqlConnection(connectionString));

            services.AddTransient<ISimplifyQueryBuilder, SimplifyMySQLQueryBuilder>();
            services.AddTransient<ISimplifyCommandBuilder, SimplifyMySQLCommandBuilder>();
            services.AddTransient<ISimplifyExecutor, ISimplifyExecutor>();
            services.AddTransient(typeof(ISimplifyRepository<>), typeof(SimplifyRepository<>));
        }

    }
}
