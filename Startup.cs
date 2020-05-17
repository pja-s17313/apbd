using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using pjatk_apbd.DAL;

namespace pjatk_apbd
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
      services.AddSingleton<IDbService, MockDbService>();
      services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      app.UseMiddleware<LoggingMiddleware>();

      app.Use(async (context, next) =>
      {
        var indexNumber = (string)context.Request.Headers["Index"];
        if (indexNumber == null)
        {
          context.Response.StatusCode = 400;
          return;
        }

        using (var client = new SqlConnection("Server=db-mssql.pjwstk.edu.pl;Database=s17313;User Id=apbds17313;Password=admin;"))
        using (var command = new SqlCommand())
        {
          command.Connection = client;
          command.CommandText = "SELECT 1 FROM Student WHERE IndexNumber = @indexNumber";
          command.Parameters.AddWithValue("indexNumber", indexNumber);

          var result = command.ExecuteScalar();
          if (result == null)
          {
            context.Response.StatusCode = 401;
            return;
          }
        }

        await next();
      });

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
