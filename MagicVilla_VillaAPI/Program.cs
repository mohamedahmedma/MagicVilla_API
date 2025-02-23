using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            //Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
            //    .WriteTo.File("Log/villaLogs.txt" , rollingInterval: RollingInterval.Day)
            //    .CreateLogger();

            //builder.Host.UseSerilog();
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });
            builder.Services.AddIdentity<ApplicationUser , IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddResponseCaching();
            builder.Services.AddScoped<IVillaRepository, VillaRepository>();
            builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.UnsupportedApiVersionStatusCode = 404;

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.FormatGroupName = (group , version) => $"{group} - {version}";
                options.SubstituteApiVersionInUrl = true;
            });


            var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

            builder.Services.AddAuthentication( x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            builder.Services.AddControllers(option =>
            {
                option.CacheProfiles.Add("Default30", new CacheProfile()
                {
                    Duration = 30
                });
                //option.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddApiVersioning();

            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                            "Enter 'Bearer' [space] and then your token in the text input below. \r\t\r\t" +
                            "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2" , 
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "Mgaic Villa",
                    Description = "API to manage Villa",
                    TermsOfService = new Uri("https://example.com/termsformomo"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mohamed Code",
                        Url = new Uri("https://mohamedCode@gmail.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://exmaple.com/license")
                    }
                });
                option.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2.0",
                    Title = "Mgaic Villa V2",
                    Description = "API to manage Villa",
                    TermsOfService = new Uri("https://example.com/termsformomo"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mohamed Code",
                        Url = new Uri("https://mohamedCode@gmail.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://exmaple.com/license")
                    }
                });
            });


            builder.Services.AddSingleton<ILogging ,Logging.Logging>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
