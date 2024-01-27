using Habitus.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Habitus.Persistence.Repositories;
using Habitus.Domain.Repositories;
using Habitus.Domain.Services;
using Habitus.Domain.Models.Auth;
using Habitus.Authorization;
using Habitus.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Habitus.Controllers.Config;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri("https://habitusvault.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

var appSettings = new AppSettings
{
    ConnectionString = GetSecretFromKeyVault(keyVaultEndpoint, "connectionString"),
    Secret = GetSecretFromKeyVault(keyVaultEndpoint,"jwt-secret"),
    TelegramToken = GetSecretFromKeyVault(keyVaultEndpoint,"telegram-token"),
    TelegramBotUsername = GetSecretFromKeyVault(keyVaultEndpoint,"telegram-username")
};

// Function to retrieve secrets from Azure Key Vault
string GetSecretFromKeyVault(Uri secretUri, string secretName)
{
    var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

    var secret = secretClient.GetSecret(secretName);

    return secret.Value.Value;
}

builder.Services.AddDbContextFactory<HabitusContext>(b => b.UseSqlServer(appSettings.ConnectionString, x => x.UseDateOnlyTimeOnly()));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add custom services
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitService, HabitService>();

// Add Identity Service
builder.Services.AddIdentity<HabitusUser, IdentityRole>(opt => opt.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<HabitusContext>()
                .AddDefaultTokenProviders();

// Add Auth

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
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


builder.Services.AddSingleton(appSettings);
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TelegramService, TelegramService>();
builder.Services.AddScoped<IReminderService, ReminderService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceErrorResponse;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddEndpointsApiExplorer();
/*
builder.Services.AddSwaggerGen(options => {

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
*/

//builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHostedService<TelegramService>();

var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();