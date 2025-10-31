using MongoDB.Driver;

namespace real_estate_api.Infrastructure.Configuration
{
    public static class MongoDbServiceExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB settings
            services.Configure<MongoDbSettings>(
          configuration.GetSection("MongoDbSettings"));

            // Register MongoDB client as singleton
            services.AddSingleton<IMongoClient>(sp =>
                       {
                           var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                           return new MongoClient(settings!.ConnectionString);
                       });

            // Register MongoDB database as scoped
            services.AddScoped<IMongoDatabase>(sp =>
        {
            var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings!.DatabaseName);
        });

            return services;
        }
    }
}
