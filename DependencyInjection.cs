using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Survey_Basket.Authentication;
using Survey_Basket.Persistence;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Survey_Basket.Errors;
using Survey_Basket.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace Survey_Basket
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            services.AddCors(options =>
                    options.AddDefaultPolicy(builder =>
                            builder.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader()
                    )
            );

            services
                .AddOpenApiService()
                .AddMapsterConfiguration()
                .AddFluentValidationConfiguration()
                .AddAuthenticationConfiguration(configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
             throw new InvalidOperationException("connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IPollService, PollService>();

            services.AddScoped<IVoteService, VoteService>();

            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<IQuestionService, QuestionService>();

            services.AddScoped<IResultService, ResultService>();

            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IEmailSender, MailService>();

            services.AddExceptionHandler<GlobalExeptionHandler>();

            services.AddProblemDetails();
            services.AddBackGroundJobsConfiguration(configuration);
            services.AddHttpContextAccessor();

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

            services.AddHealthChecks()
                .AddSqlServer(name: "database",connectionString: connectionString)
                .AddHangfire(options => { options.MinimumAvailableServers = 1; });

    
            return services;
        }

        private static IServiceCollection AddOpenApiService(this IServiceCollection services)
        {
            services.AddOpenApi();
            return services;
        }
        private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            var mappingConfig = TypeAdapterConfig.GlobalSettings;

            mappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }

        private static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }

        private static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IAuthorizationHandler, PermissionAuthrizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthrizationPolicyProvider>();

            // services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;

            });
 
            return services;
        }

        private static IServiceCollection AddBackGroundJobsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
                 services.AddHangfire(config => config
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            services.AddHangfireServer();
            return services;
        }

 
    }
}
