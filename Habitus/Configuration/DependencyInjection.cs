using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Habitus.Authorization;
using Habitus.Controllers.Config;
using Habitus.Domain.Models.Auth;
using Habitus.Domain.Repositories;
using Habitus.Domain.Services;
using Habitus.Helpers;
using Habitus.Persistence.Repositories;
using Habitus.Services;
using Hangfire;
using Hangfire.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Habitus.Configuration;

public static class DependencyInjection
{
    public static void AddKeyVault(this IConfigurationManager configuration)
    {
        configuration.AddAzureKeyVault(new KeyVaultManagement(configuration).SecretClient,
                                       new KeyVaultSecretManager());
    }
    public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection services,
                                                                 IConfigurationManager configuration)
    {
        services.AddSingleton<AppSettings>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new AppSettings
            {
                ConnectionString = config["connectionString"],
                Secret = config["jwt-secret"],
                TelegramToken = config["telegram-token"],
                TelegramBotUsername = config["telegram-username"]
            };
        });
        
        return services;
    }

    public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services)
    {
        services.AddDbContextFactory<HabitusContext>((provider, b) =>
        {
            var appSettings = provider.GetRequiredService<AppSettings>();
            b.UseSqlServer(appSettings.ConnectionString, x => x.UseDateOnlyTimeOnly());
        });

        return services;
    }

    public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services)
    {
        services.AddHangfire((serviceProvider, configuration) =>
            configuration.UseEFCoreStorage(
                () => serviceProvider.GetRequiredService<IDbContextFactory<HabitusContext>>().CreateDbContext(),
                new EFCoreStorageOptions
                {
                    CountersAggregationInterval = new TimeSpan(0, 5, 0),
                    DistributedLockTimeout = new TimeSpan(0, 10, 0),
                    JobExpirationCheckInterval = new TimeSpan(0, 30, 0),
                    QueuePollInterval = new TimeSpan(0, 0, 15),
                    Schema = string.Empty,
                    SlidingInvisibilityTimeout = new TimeSpan(0, 5, 0),
                }));

        return services;
    }

    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<HabitusUser, IdentityRole>(options => options.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<HabitusContext>()
                .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var appSettings = services.BuildServiceProvider().GetService<AppSettings>();
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Secret))
            };
        });

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            })
        );

        return services;
    }

    public static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceErrorResponse;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<IHabitService, HabitService>();
        services.AddScoped<IJwtUtils, JwtUtils>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<TelegramService, TelegramService>();
        services.AddScoped<IReminderService, ReminderService>();

        return services;
    }
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program));
        return services;
    }

    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<TelegramService>();
        services.AddHangfireServer();
        return services;
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                 Title = "MyAPI",
                 Version = "v1"
            });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });

            options.ExampleFilters();
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        return services;
    }

}
