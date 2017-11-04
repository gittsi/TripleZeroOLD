using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TripleZeroApi.Repository;
using Autofac;
using TripleZeroApi.Infrastructure.DI;

namespace TripleZeroApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        static IContainer autoFacContainer = null;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            MongoDBContext.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
            MongoDBContext.DatabaseName = Configuration.GetSection("MongoConnection:DatabaseName").Value;
            MongoDBContext.IsSSL = Convert.ToBoolean(Configuration.GetSection("MongoConnection:IsSSL").Value);

            autoFacContainer = AutofacConfig.ConfigureContainer();
            using (var scope = autoFacContainer.BeginLifetimeScope())
            {
            }


                services.AddMvc();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
