using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebChatPlay.BackgroundServices;
using WebChatPlay.Filters;
using WebChatPlay.MessageInbox;
using WebChatPlay.SignalRHubs;

namespace WebChatPlay
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
            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DomainRestricted", policy =>
                {
                    policy.Requirements.Add(new DomainRestrictedRequirement());
                });
            });

            services.Configure<CookiePolicyOptions>(options =>
                    {
                        // This lambda determines whether user consent for non-essential 
                        // cookies is needed for a given request.
                        options.CheckConsentNeeded = context => true;
                        // requires using Microsoft.AspNetCore.Http;
                        options.MinimumSameSitePolicy = SameSiteMode.Strict;
                    });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "__RequestVerificationToken";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });


            services.AddSingleton<IMessagesRepository, FakeMessagesRepository>();
            services.AddSingleton<InboxMaintService>();

            services.AddSingleton<MessageProducer>();
            // services.AddHostedService<RandomMessageProducer>();
            services.AddHostedService<MessageConsumer>();
            services.AddSingleton<MessageSender>();
            services.AddSignalR(options =>
            {
                // TODO: Will configure later
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseXHeaders();

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<MessagingHub>("/messagingHub", options =>
                {

                });
            });

        }


    }
}
