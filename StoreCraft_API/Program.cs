
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using StoreCraft_API.Data;
using StoreCraft_API.ErrorHandling;
using StoreCraft_API.Helpers;
using StoreCraft_API.Repository;
using StoreCraft_API.Services;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace StoreCraft_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Repository and Service
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCustomExceptionHandler();

            app.UseAuthorization();

            app.MapControllers();


            var logger = LogManager.GetLogger(typeof(Program));
            logger.Info("StoreCraft API application started successfully");

            app.Run();
        }
    }
}
