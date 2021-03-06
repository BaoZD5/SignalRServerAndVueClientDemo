using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SignalRServerAndVueClientDemo.Filters;
using SignalRServerAndVueClientDemo.Hubs;
using SignalRServerAndVueClientDemo.LogHelper;

namespace SignalRServerAndVueClientDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(Logger, new FileInfo("log4net.config"));
           // _logger = LogManager.GetLogger(Logger.Name, typeof(Startup));
        }

        public static ILoggerRepository Logger { get; set; }

        static ILog _logger;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //���ÿ�����
            services.AddControllers();
            services.AddMvc(option =>
            {
                //���Ӵ��󲶻�
                option.Filters.Add(typeof(SysExceptionFilter));
                //option.EnableEndpointRouting = false;
            });
            services.AddRazorPages();
            services.AddSignalR().AddNewtonsoftJsonProtocol(cfg=>
            {
                var settings = cfg.PayloadSerializerSettings;
                //ʱ���ʽ
                settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //����ѭ������
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            //���ÿ���
            services.AddCors(c =>
                c.AddPolicy("AllowAll", p =>
                {
                    p.AllowAnyOrigin();
                    p.AllowAnyMethod();
                    p.AllowAnyHeader();
                })
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //log4 extension
           // loggerFactory.AddLog4Net();

            //_logger.Info("Startpup");
            //_logger.Info("Startpup2");

            //{
            //    var data = ReadHelper.Read();
            //}
            app.UseStaticFiles();
            
            //���ÿ���
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //�ս������·��Ĭ��
                endpoints.MapControllerRoute(
                               name: "default",
                               pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
    
}
