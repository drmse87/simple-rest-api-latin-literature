using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json.Serialization;
using simple_web_api_latin_literature.Models;

namespace simple_web_api_latin_literature
{
    public class Startup
    {
        private string _baseUrl;
        public Startup(IConfiguration conf)
        {
            _conf = conf;
            StaticConfiguration = conf;
            _baseUrl = _conf["BaseUrl"];
        }

        public IConfiguration _conf { get; }
        internal static IConfiguration StaticConfiguration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddDbContextPool<Data.ApplicationDbContext>(options =>
                options.UseSqlite(_conf.GetConnectionString("LatinDB")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = _baseUrl,
                        ValidIssuer = _baseUrl,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["SecurityKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddHttpContextAccessor();

            services.AddSingleton<IUserContextService, JwtUserContext>();
            services.AddScoped<IWebhookService, WebhookService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
