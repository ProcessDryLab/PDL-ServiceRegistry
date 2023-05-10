using ServiceRegistry.ConnectedNodes;
using ServiceRegistry.Requests;

namespace ServiceRegistry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int pingTime = 5000; // 1 sec
            int configUpdateTime = 120000; // 10 sec TODO: Increase to once every 24 hours when we're done.

            var builder = WebApplication.CreateBuilder(args);
            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine("Environment.IsDevelopment");
                builder.WebHost.UseUrls("https://localhost:3000");
            }

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors();


            var app = builder.Build();

            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            new Endpoints.Endpoints(app);

            Thread updateStatusThread = new Thread(() =>
            {
                while (true)
                {
                    ConnectedNodes.ConnectedNodes.Instance.UpdateOnlineStatus();
                    Thread.Sleep(pingTime);
                }
            });

            Thread updateConfigThread = new Thread(() =>
            {
                while (true)
                {
                    ConnectedNodes.ConnectedNodes.Instance.GetAllConnectedHostConfig();
                    Thread.Sleep(configUpdateTime);
                }
            });

            //ConnectedNodes.ConnectedNodes.Instance.GetAllConnectedHostConfig();

            updateStatusThread.IsBackground = true;
            updateStatusThread.Start();

            updateConfigThread.IsBackground = true;
            updateConfigThread.Start();

            app.Run();
        }
    }
}