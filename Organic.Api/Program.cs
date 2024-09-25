using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Organic.Application.Services.Auth;
using Organic.Application.Services.Campaigns;
using Organic.Application.Services.Factories;
using Organic.Application.Services.Interfaces;
using Organic.Application.Services.Posting;
using Organic.Application.Services.SocialMedia;
using Organic.Core.Entities;
using Organic.Core.Interfaces;
using Organic.Core.Services.Interfaces;
using Organic.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpClient<InstagramPostService>();
builder.Services.AddHttpClient<TwitterPostService>();

// Entity crud services
builder.Services.AddScoped<IScheduledPostService, ScheduledPostService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
builder.Services.AddScoped<ISocialMediaAccountService, SocialMediaAccountService>();

// Scheduling services
builder.Services.AddScoped<ISocialMediaPostService, InstagramPostService>(); // Register Instagram service
builder.Services.AddScoped<ISocialMediaPostService, TwitterPostService>();   // Register Twitter service
builder.Services.AddScoped<InstagramPostService>();  // Direct registration for Instagram
builder.Services.AddScoped<TwitterPostService>();    // Direct registration for Twitter
builder.Services.AddScoped<IPostalService, PostalService>();

builder.Services.AddScoped<IPostServiceFactory, PostServiceFactory>();

builder.Services.AddScoped<DatabaseSeeder>();

// Add CORS, Authentication, Authorization, Swagger, etc.
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
	{
		builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	});
	options.AddPolicy("AllowReactApp", policy =>
	{
		policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://192.168.8.180:3000")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});

// Add controllers
builder.Services.AddControllers();

// Add other necessary services
builder.Services.AddRazorPages(); // If you're using Razor Pages
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // For API documentation

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseCors("AllowReactApp");

	try
	{
		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			var dbContext = services.GetRequiredService<ApplicationDbContext>();

			dbContext.Database.Migrate();  // Ensure the database is up-to-date

			var seeder = services.GetRequiredService<DatabaseSeeder>();
			await seeder.SeedAsync();  // Seed the database
		}
	}
	catch (Exception ex)
	{
		// Log the error (or handle it in a way appropriate for your app)
		var logger = app.Services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Map controller routes

// Run the application
app.Run();
