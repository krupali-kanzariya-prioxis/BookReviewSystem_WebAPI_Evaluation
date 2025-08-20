using Microsoft.EntityFrameworkCore;
using Review.Services.RepositoryFactory;
using Review.Services.UnitOfWork;

namespace BookReviewSystem_WebAPI_Evaluation.Helper
{
    public static class UnitOfWorkServiceCollectionExtentions
    {
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
           where TContext : DbContext
        {
            services.AddTransient<IRepositoryFactory, UnitOfWork<TContext>>();
            services.AddTransient<IUnitOfWork, UnitOfWork<TContext>>();
            services.AddTransient<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            return services;
        }
    }
}
