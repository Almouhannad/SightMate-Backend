var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseNpgsql(CONFIG.ConnectionString);
//});

//builder.Services.AddIdentity<User, Role>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//    //options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = CONFIG.JWTIssuer,
//        ValidAudience = CONFIG.JWTAudience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CONFIG.JWTSecretKey))
//    };
//});

//builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    //app.ApplyMigrations();
}

app.MapGet("/health", () =>
{
    return new { Status = "OK" };
})
.WithName("Health");

// For testing only (Remove later)
//app.MapGet("/users/me", async(ClaimsPrincipal claims, ApplicationDbContext context) =>
//{
//    String userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
//    return await context.Users.FindAsync(Guid.Parse(userId));

//}).RequireAuthorization();

app.Run();