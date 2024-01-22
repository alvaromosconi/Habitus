using Habitus.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Habitus.Persistence.Repositories;
using Habitus.Domain.Repositories;
using Habitus.Domain.Services;
using Habitus.Domain.Models.Auth;
using Habitus.Controllers.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Habitus.Authorization;
using Habitus.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appSettings = new AppSettings
{
    Secret = builder.Configuration["JWT:Secret"],
    ValidAudience = builder.Configuration["JWT:ValidAudience"],
    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
    TelegramToken = builder.Configuration["Telegram:Token"],
    TelegramBotUsername = builder.Configuration["Telegram:Username"]
};

builder.Services.AddDbContextFactory<HabitusContext>(b => b.UseSqlServer(builder.Configuration["HabitusApp:ConnectionString"], x => x.UseDateOnlyTimeOnly()));

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
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = appSettings.ValidAudience,
            ValidIssuer = appSettings.ValidIssuer,
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
builder.Services.AddSwaggerGen(options => {
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

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHostedService<TelegramService>();
var serviceProvider = builder.Services.BuildServiceProvider();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();




