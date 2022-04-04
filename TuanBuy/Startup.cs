using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TuanBuy.Models;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using Topic.Hubs;

namespace TuanBuy
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
            //新增cookie驗證
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
            {
                //未登入時會自動導向這個網址
                opt.LoginPath = new PathString("/Home/Login");
                //因權限被拒絕時進入的網址
                opt.AccessDeniedPath= new PathString("/Home/Index");
            });
            //注入HttpContext抓使用者資料
            services.AddHttpContextAccessor();

            //弄個Swagger測試API
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1",new OpenApiInfo{Title = "TuanBuy API中心",Version="v1"});
            });
            //EF CORE Context注入
            services.AddDbContext<TuanBuyContext>(option =>
                option.UseSqlServer(Configuration.GetConnectionString("TuanBuy")));
            //services.AddDbContext<Models.SqlDbServices>((builider) => 
            //    { builider.UseSqlServer(this.Configuration.GetConnectionString("TuanBuy")); });
            //倉儲模式注入
            services.AddTransient<GenericRepository<Product>>();
            services.AddTransient<GenericRepository<User>>();

            //services.AddSingleton<SqlDbServices>();
            services.AddSingleton<UserService>();

            //調用websingnalr服務
            services.AddSignalR();

            //加入Session狀態服務
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //開發模式才能進去
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TuanBuyService v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            //使用靜態檔案
            app.UseStaticFiles();

            //app.UseAuthorization();
            //使用Session Middleware
            app.UseSession();
            app.UseRouting();
            //使用權限註冊，這邊順序要Cookie -> Authentication -> Authorization
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
