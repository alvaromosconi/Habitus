using Habitus.Configuration;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddKeyVault();

builder.Services
    .AddAppSettingsConfiguration(configuration)
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddDbContextConfiguration()
    .AddHangfireConfiguration()
    .AddIdentityConfiguration()
    .AddAuthenticationConfiguration()
    .AddCorsConfiguration()
    .AddCustomServices()
    .AddControllersConfiguration()
    .AddSwaggerConfiguration()
    .AddSwaggerExamplesFromAssemblies()
    .AddEndpointsApiExplorer()
    .AddAutoMapperConfiguration()
    .AddHostedServices();

var app = builder.Build();

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