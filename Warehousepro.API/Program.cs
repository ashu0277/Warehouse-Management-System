using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using System.Text;

using WarehousePro.API.Data;

using WarehousePro.API.Models;

using WarehousePro.API.Models.Enums;

using WarehousePro.API.Services;

using WarehousePro.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════════════════════════

// 1. CONTROLLERS

// ══════════════════════════════════════════════════════════════════════════

builder.Services.AddControllers();

// ══════════════════════════════════════════════════════════════════════════

// 2. DATABASE — EF CORE + SQL SERVER

// ══════════════════════════════════════════════════════════════════════════

builder.Services.AddDbContext<AppDbContext>(options =>

	options.UseSqlServer(

		builder.Configuration.GetConnectionString("DefaultConnection")));

// ══════════════════════════════════════════════════════════════════════════

// 3. JWT AUTHENTICATION

// ══════════════════════════════════════════════════════════════════════════

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["SecretKey"]!;

var issuer = jwtSettings["Issuer"]!;

var audience = jwtSettings["Audience"]!;

builder.Services.AddAuthentication(options =>

{

	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})

.AddJwtBearer(options =>

{

	options.TokenValidationParameters = new TokenValidationParameters

	{

		ValidateIssuer = true,

		ValidateAudience = true,

		ValidateLifetime = true,

		ValidateIssuerSigningKey = true,

		ValidIssuer = issuer,

		ValidAudience = audience,

		IssuerSigningKey = new SymmetricSecurityKey(

									   Encoding.UTF8.GetBytes(secretKey)),

		ClockSkew = TimeSpan.Zero

	};

	options.Events = new JwtBearerEvents

	{

		OnChallenge = context =>

		{

			context.HandleResponse();

			context.Response.StatusCode = 401;

			context.Response.ContentType = "application/json";

			return context.Response.WriteAsync(

				"{\"message\": \"Unauthorized. Please login first.\"}");

		},

		OnForbidden = context =>

		{

			context.Response.StatusCode = 403;

			context.Response.ContentType = "application/json";

			return context.Response.WriteAsync(

				"{\"message\": \"Forbidden. You do not have permission.\"}");

		}

	};

});

builder.Services.AddAuthorization();

// ══════════════════════════════════════════════════════════════════════════

// 4. REGISTER ALL SERVICES

// ══════════════════════════════════════════════════════════════════════════

// ── Audit & Current User — register FIRST ─────────────────────────────────
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();



builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuditLogService, AuditLogService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ── Group 1 : Identity & Access ───────────────────────────────────────────

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserService, UserService>();

// ── Group 2 : Warehouse Layout ────────────────────────────────────────────

builder.Services.AddScoped<IWarehouseService, WarehouseService>();

builder.Services.AddScoped<IZoneService, ZoneService>();

builder.Services.AddScoped<IBinLocationService, BinLocationService>();

// ── Group 3 : Inventory ───────────────────────────────────────────────────

builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<IInventoryBalanceService, InventoryBalanceService>();

builder.Services.AddScoped<IStockReservationService, StockReservationService>();

// ── Group 4 : Inbound ─────────────────────────────────────────────────────

builder.Services.AddScoped<IInboundReceiptService, InboundReceiptService>();

builder.Services.AddScoped<IPutAwayTaskService, PutAwayTaskService>();

// ── Group 5 : Orders & Outbound ───────────────────────────────────────────

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IPickTaskService, PickTaskService>();

builder.Services.AddScoped<IPackingUnitService, PackingUnitService>();

builder.Services.AddScoped<IShipmentService, ShipmentService>();

// ── Group 6 : Replenishment & Slotting ────────────────────────────────────

builder.Services.AddScoped<IReplenishmentTaskService, ReplenishmentTaskService>();

builder.Services.AddScoped<ISlottingRuleService, SlottingRuleService>();

// ── Group 7 : Analytics & Notifications ───────────────────────────────────

builder.Services.AddScoped<IWarehouseReportService, WarehouseReportService>();

builder.Services.AddScoped<INotificationService, NotificationService>();

// ══════════════════════════════════════════════════════════════════════════

// BUILD

// ══════════════════════════════════════════════════════════════════════════
// ── CORS — Allow Angular ──────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular", policy =>
		policy.WithOrigins("http://localhost:4200")
			  .AllowAnyHeader()
			  .AllowAnyMethod());
});

var app = builder.Build();

// ══════════════════════════════════════════════════════════════════════════

// ══════════════════════════════════════════════════════════════════════════
// SEED DATA — Creates first Admin automatically if no users exist
// ══════════════════════════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	context.Database.EnsureCreated();
	if (!context.Users.Any())
	{
		context.Users.Add(new User
		{
			Name = "System Admin",
			Role = UserRole.Admin,
			Email = "admin@warehouse.com",
			Phone = "0000000000",
			PasswordHash = "Admin@123",    // Plain text — no BCrypt
			IsDeleted = false,
			CreatedAt = DateTime.UtcNow
		});
		context.SaveChanges();
	}
}

// ══════════════════════════════════════════════════════════════════════════

// MIDDLEWARE PIPELINE

// ══════════════════════════════════════════════════════════════════════════
app.UseCors("AllowAngular");
//app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
