using ServiceRegistry.ConnectedNodes;

namespace ServiceRegistry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment()) builder.WebHost.UseUrls("https://localhost:3000");

            // Add services to the container.
            builder.Services.AddAuthorization();

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

            app.UseAuthorization();

            Node node = new Node();
            node.path = "pathRepository";
            node.type = "repository";

            ConnectedNodes.ConnectedNodes.Instance.AddNode(node.path, node);

            Node node2 = new Node();
            node2.path = "pathMiner";
            node2.type = "miner";

            ConnectedNodes.ConnectedNodes.Instance.AddNode(node2.path, node2);

            //new Requests.Requests(app);

            new Endpoints.Endpoints(app);

            app.Run();
        }
    }
}