
using BookReviewSystem_WebAPI_Evaluation.Helper;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Review.Models.Models.ReviewDB;
using Review.Models.SpDbContext;
using Review.Models.ValidationClass;
using Review.Services.Repository.Implementations;
using Review.Services.Repository.Interfaces;
using Serilog;

namespace BookReviewSystem_WebAPI_Evaluation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var ConnectionString = builder.Configuration["DbConnectionString"];

            builder.Services.AddDbContext<ReviewDBContext>(options =>
                options.UseSqlServer(
                    builder.Configuration["DbConnectionString"]
                    ));
            builder.Services.AddDbContext<ReviewManagementSpContext>(options =>
            {
                options.UseSqlServer(ConnectionString, sqlServerOptionsAction: sqlOptions =>
                {

                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Transient);

            builder.Services.AddDbContext<ReviewManagementSpContext>(options =>
            {
                options.UseSqlServer(ConnectionString, sqlServerOptionsAction: sqlOptions =>
                {

                    sqlOptions.EnableRetryOnFailure();

                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Transient);
            UnitOfWorkServiceCollectionExtentions.AddUnitOfWork<ReviewDBContext>(builder.Services);
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddControllers()
      .AddFluentValidation(fv =>
      {
          fv.RegisterValidatorsFromAssemblyContaining<ReviewRequestModelValidator>();
      });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<CustomMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
