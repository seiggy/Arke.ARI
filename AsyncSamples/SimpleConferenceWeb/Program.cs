using Arke.ARI;
using SimpleConferenceWeb.Services;

namespace SimpleConferenceWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers();
            builder.Services.Configure<ARIConfig>(builder.Configuration.GetSection("ARIConfig"));
            builder.Services.AddHostedService((sp) => sp.GetRequiredService<AsteriskConferenceService>());
            builder.Services.AddSingleton<AsteriskConferenceService>();
            builder.Services.AddSingleton<StasisEndpoint>(new StasisEndpoint("192.168.1.132", 8088, "arke", "arke"));
            builder.Services.AddSingleton<AriClient>((c) =>
            {
                return new AriClient(c.GetRequiredService<StasisEndpoint>(), c, "arke");
            });

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/Index");
            });
            app.Run();
        }
    }
}