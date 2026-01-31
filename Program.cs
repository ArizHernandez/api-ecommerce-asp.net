using System.Text;
using apiEcommerce.Constants;
using apiEcommerce.Data;
using apiEcommerce.Models;
using apiEcommerce.Reporsitory;
using apiEcommerce.Reporsitory.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mapster;
using apiEcommerce.Mapping;

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString = builder.Configuration.GetConnectionString("ConnectionUrl");
// var dbConnectionString = builder.Configuration.GetSection("ConnectionStrings")["ConnectionUrl"];
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretJWT");
if (string.IsNullOrEmpty(secretKey))
{
  throw new InvalidOperationException("Secret key isn't configurated");
}
// Add services to the container.
// Add services to the container.
//? -- Models mapper (Mapster) --
var typeAdapterConfig = new TypeAdapterConfig();
CategoryProfile.Register(typeAdapterConfig);
ProductProfile.Register(typeAdapterConfig);
UserProfile.Register(typeAdapterConfig);
builder.Services.AddSingleton(typeAdapterConfig);
//? -- Auth with Identity --
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

//? -- DB config --
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(dbConnectionString).UseSeeding(DataSeeder.SeedData));
//? -- Cache --
builder.Services.AddResponseCaching(options =>
{
  options.MaximumBodySize = 1024 * 1024;
  options.UseCaseSensitivePaths = true;
});
//? -- Authentication --
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  //? Info: Set true on production
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    ValidateIssuer = false,
    ValidateAudience = false
  };
});
//? -- env values --
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//? -- Api controlers --
builder.Services.AddControllers(options =>
{
  options.CacheProfiles.Add(CacheProfiles.Defaul10, CacheProfiles.Profile10);
  options.CacheProfiles.Add(CacheProfiles.Defaul20, CacheProfiles.Profile20);
});

//? -- Api documentation (Swagger) --
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
  options =>
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                    "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                    "Ejemplo: \"12345abcdef\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
      Version = "v1",
      Title = "Api Ecommerce",
      Description = "Api to manage products and users",
      TermsOfService = new Uri("http://example.com/terms"),
      Contact = new OpenApiContact
      {
        Name = "DevTalles",
        Url = new Uri("https://devtalles.com")
      },
      License = new OpenApiLicense
      {
        Name = "Usage license",
        Url = new Uri("http://example/license")
      },
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
      Version = "v2",
      Title = "Api Ecommerce",
      Description = "Api to manage products and users",
      TermsOfService = new Uri("http://example.com/terms"),
      Contact = new OpenApiContact
      {
        Name = "DevTalles",
        Url = new Uri("https://devtalles.com")
      },
      License = new OpenApiLicense
      {
        Name = "Usage license",
        Url = new Uri("http://example/license")
      },
    });
  }
);

//? -- Cors config --
builder.Services.AddCors(options =>
{
  options.AddPolicy(PolicyNames.AllowSpecificOrigin, builder =>
  {
    // builder.WithOrigins("http://localhost:3000");
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
  });
});

//? -- App versioning -- 
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.ReportApiVersions = true;
  // options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //?api-verison
});

apiVersioningBuilder.AddApiExplorer(options =>
{
  options.GroupNameFormat = "'v'VVV"; // v1, v2,v3, v...n
  options.SubstituteApiVersionInUrl = true; // on rout: /api/v{version}/products
});

//? -- build app --
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2");
  });
}

//? -- Enable filesa transaction --
app.UseStaticFiles();

app.UseHttpsRedirection();

//? -- Cors Middleware --
app.UseCors(PolicyNames.AllowSpecificOrigin);
//? -- Cache Middleware --
app.UseResponseCaching();
//? -- Auth Middleware --
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
