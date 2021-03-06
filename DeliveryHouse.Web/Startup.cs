using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Data.Entities;
using DeliveryHouse.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DeliveryHouse.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DataContext>();

            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication().AddCookie()
                                        .AddJwtBearer(cfg =>
                                        {
                                            cfg.TokenValidationParameters = new TokenValidationParameters
                                            {
                                                ValidIssuer = Configuration["Tokens:Issuer"],
                                                ValidAudience = Configuration["Tokens:Audience"],
                                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                                            };
                                        });

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddControllersWithViews();
            //services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });

            services.AddCors();
            //services.AddMvc().AddRazorOptions(opciones => opciones.AllowRecompilingViewsOnFileChange = true);

            services.AddTransient<SeedData>();
            services.AddControllersWithViews();
            services.AddScoped<ICombosHelper, ComboHelper>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IEmailHelper, EmailHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            if (env.IsProduction() || env.IsStaging() || env.IsEnvironment("Staging"))
            {
                app.UseExceptionHandler("/Error");
            }

            //app.UseCors(x =>
            //{
            //    x.AllowAnyOrigin()
            //     .AllowAnyMethod()
            //     .AllowAnyHeader()
            //     .AllowCredentials();
            //});

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action}/{id?}");
            //});

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
