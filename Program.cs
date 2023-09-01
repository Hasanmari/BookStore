using BookStore.Models;
using BookStore.Models.Reopsitories;
using Microsoft.EntityFrameworkCore;

namespace BookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var Configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddViewOptions(options =>
            {
                options.HtmlHelperOptions.ClientValidationEnabled = true;
            });

            // the  services
            builder.Services.AddScoped<IBookStoreRepository<Author>, AuthorDbRepository>();
            builder.Services.AddScoped<IBookStoreRepository<Book>, BookDbRepository>();
            builder.Services.AddScoped<AuthorDbRepository>();
            builder.Services.AddDbContext<BookStoreDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlCon"));
            });
            var app = builder.Build();

            //automatic migration
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
                db.Database.Migrate();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Book}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}