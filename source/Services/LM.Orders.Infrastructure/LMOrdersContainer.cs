using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Dialect;
using AutoMapper;
using MongoDB.Driver;
using Confluent.Kafka;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LM.Orders.Application.QueryHandlers;
using LM.Orders.Application.Mappers;
using LM.Orders.Application.CommandHandlers;
using LM.Orders.Domain.Services;
using LM.Orders.Domain.Interfaces;
using LM.Orders.Infrastructure.Repositories;
using LM.Orders.Infrastructure.Database.Maps;
using LM.Orders.Infrastructure.Services;
using LM.Orders.Infrastructure.Database;

namespace LM.Orders.Infrastructure.Extensions
{
    public static class LMOrdersContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            Install(configuration, services);
            return services;
        }

        public static void Install(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                return CreateNHFactory(configuration, "DefaultConnection");
            });

            services.AddScoped(provider =>
            {
                var sessionFactory = provider.GetRequiredService<ISessionFactory>();
                return sessionFactory.OpenSession();
            });

            var mongoClient = new MongoClient(configuration.GetConnectionString("MongoConnection"));
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddScoped<IMongoDatabase>(sp => mongoClient.GetDatabase(configuration["MongoDb:DatabaseName"]));


            services.AddHttpClient();

            AddCacheServices(configuration, services);

            RegisterRepositories(services);

            AddServices(configuration, services);

            services.AddSingleton<IEventBusPublisher, KafkaEventBusPublisher>();
        }

        private static IServiceCollection AddCacheServices(IConfiguration configuration, IServiceCollection services)
        {
            var redisConnectionString = configuration.GetConnectionString("RedisConnection");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new InvalidOperationException("A ConnectionString 'RedisConnection' não foi encontrada.");
            }

            var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IOrderCacheService, RedisOrderCacheService>();

            return services;
        }

        private static IServiceCollection AddServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton<AutoMapper.IMapper>(sp =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(OrderMappingProfile).Assembly);
                });
                return config.CreateMapper();
            });

            var producerConfig = new ProducerConfig();
            configuration.GetSection("KafkaProducer").Bind(producerConfig);
            services.AddSingleton(producerConfig);

            services.AddScoped<OrderDomainService>();

            services.AddScoped<CreateOrderCommandHandler>();
            services.AddScoped<GetOrderQueryHandler>();


            return services;
        }

        private static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderDapperRepository, OrderDapperRepository>();
            services.AddScoped<IUnitOfWork, NhUnitOfWork>();

            return services;
        }

        private static ISessionFactory CreateNHFactory(IConfiguration configuration, string connectionStringName)
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"A ConnectionString '{connectionStringName}' não foi encontrada.");
            }

            var db = MsSqlConfiguration.MsSql2012
                .IsolationLevel(System.Data.IsolationLevel.ReadUncommitted)
                .ConnectionString(connectionString)
                .Dialect<MsSql2012Dialect>();

            #if DEBUG
                db = db.ShowSql();
            #endif

            return Fluently
                .Configure()
                .Database(db)
                .Mappings(m => m
                    .FluentMappings
                    .AddFromAssemblyOf<OrderMap>()
                    .Conventions.Add(DefaultLazy.Never()))
                .BuildSessionFactory();
        }
    }
}