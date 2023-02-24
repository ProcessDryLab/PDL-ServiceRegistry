using System.IO;
using ServiceRegistry.ConnectedNodes;

namespace ServiceRegistry.Endpoints
{
    public class Endpoints
    {
        public Endpoints(WebApplication app)
        {
            app.MapGet("resources/miners", (HttpContext httpContext) =>
            {
                Console.Write("Service registry has recieved a quest on /resources/miners");
                return ConnectedNodes.ConnectedNodes.Instance.Filter("miner");
            })
            .WithName("GetMiners");

            app.MapGet("resources/repositories", (HttpContext httpContext) =>
            {
                Console.Write("Service registry has recieved a quest on /resources/repositories");
                return ConnectedNodes.ConnectedNodes.Instance.Filter("repository");
            })
            .WithName("GetRepositories");

            app.MapGet("Ping", (HttpContext httpContext) =>
            {
                Console.Write("Service registry has recieved a quest on /ping");
                return "Pong";
            })
            .WithName("GetRepositories");

            app.MapPost("resources/Add/Repository", (HttpContext httpContext) =>
            {
                Requests.Requests.GetConfig("http://localhost:5000/configurations");
            })
            .WithName("PostRepositories");

        }
    }
}
