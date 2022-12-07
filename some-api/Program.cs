using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace some_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(options=>
            {
                options.DefaultScheme = "UNKNOWN";
                options.DefaultChallengeScheme = "UNKNOWN";
            })
            .AddJwtBearer(Consts.MY_AAD_SCHEME, jwtOptions =>
            {
                jwtOptions.MetadataAddress = configuration["AzureAd:MetadataAddress"];
                jwtOptions.Authority = configuration["AzureAd:Authority"];
                jwtOptions.Audience = configuration["AzureAd:Audience"];
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = configuration.GetSection("ValidIssuers").Get<string[]>(),
                    ValidAudiences = configuration.GetSection("ValidAudiences").Get<string[]>()
                };
            })
            .AddPolicyScheme("UNKNOWN", "UNKNOWN", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    return Consts.MY_AAD_SCHEME;
                };
            });

            builder.Services.AddSingleton<IAuthorizationHandler, AllSchemesHandler>();

            builder.Services.AddAuthorization(options => {
                options.AddPolicy(Consts.MY_POLICY_ALL_IDP, policyRequirements =>
                {
                    policyRequirements.AddRequirements(new AllSchemesRequirement());
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(x=>x.AllowAnyOrigin());

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}