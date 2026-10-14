using EndpointsPage.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	Args = args,
	WebRootPath = "Frontend"
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHealthChecks()
	.AddSqlServer(
		connectionString: connectionString,
		name: "sql-server-check",
		failureStatus: HealthStatus.Unhealthy,
		timeout: TimeSpan.FromSeconds(3),
		tags: new[] { "database", "critical" });

builder.Services.AddDbContext<InternsDbContext>(options =>
	options.UseSqlServer(connectionString));

// Configuración de CORS para permitir cualquier origen
builder.Services.AddCors(options =>
{
	options.AddPolicy("PermitirTodo", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File(
		path: "logs/log-.txt",
		rollingInterval: RollingInterval.Day,
		retainedFileCountLimit: 7,
		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
	)
	.CreateLogger();

builder.Host.UseSerilog();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
	};
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	app.MapScalarApiReference(options =>
	{
		options
			.WithTitle("Mi API de .NET")
			.WithTheme(ScalarTheme.DeepSpace)
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
	});
}

app.UseHttpsRedirection();

// Aplicación del middleware con la nueva política
app.UseCors("PermitirTodo");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();