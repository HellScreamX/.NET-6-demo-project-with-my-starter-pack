using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.BusinessLogicLayer.Services;
using Project.DAL;

var builder = WebApplication.CreateBuilder(args);

/*------------ Logger -------------*/
builder.Services.AddScoped<IExceptionLogger, ExceptionLogger>();

// Add services to the container.


builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<IEmailService, EmailService>
                (a => new EmailService(builder.Environment, builder.Configuration["SendGrid:Key"], builder.Configuration["CanSendEmails"], builder.Configuration["SendGrid:From"]));
builder.Services.AddScoped<IProjectUserManager, ProjectUserManager>();
builder.Services.AddScoped<IMetricManager, MetricManager>();
builder.Services.AddScoped<ICustomerManager, CustomerManager>();
builder.Services.AddScoped<ICommandeManager, CommandeManager>();
builder.Services.AddScoped<IContractorMetricsManager, ContractorMetricsManager>();
builder.Services.AddScoped<IContractorManager, ContractorManager>();



builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
    
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", policy => policy
                .RequireRole(Roles.SuperAdmin));
    options.AddPolicy("Contractor", policy => policy
                .RequireRole(Roles.Contractor));
    options.AddPolicy("AdminOrContractor", policy => policy
                .RequireRole(Roles.Contractor, Roles.SuperAdmin));                


});

builder.Services.AddIdentity<ProjectUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();
// Auto Mapper Configurations
builder.Services.AddAutoMapper(typeof(Program));


builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
});
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
                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Issuer"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                     ClockSkew = TimeSpan.Zero
                 };
                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     }
                 };
             });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
 app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My service");
                c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
            });

app.UseCustomExceptionHandler();
app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
