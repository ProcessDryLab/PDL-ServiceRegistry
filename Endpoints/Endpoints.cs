using System.IO;
using Newtonsoft.Json;
using ServiceRegistry.ConnectedNodes;
using ServiceRegistry.Requests;

namespace ServiceRegistry.Endpoints
{
    public class Endpoints
    {
        public Endpoints(WebApplication app)
        {
            // MINERS
            app.MapGet("/miners", (HttpContext httpContext) =>
            {
                return JsonConvert.SerializeObject(ConnectedNodes.ConnectedNodes.Instance.GetList(NodeType.Miner));
            });

            app.MapPost("/miners", async (HttpRequest request) =>
            {
                return await Requests.Requests.GetConfig(request);
            });
            // REPOSITORIES
            app.MapGet("/repositories", (HttpContext httpContext) =>
            {
                return JsonConvert.SerializeObject(ConnectedNodes.ConnectedNodes.Instance.GetList(NodeType.Repository));
            });

            app.MapPost("/repositories", async (HttpRequest request) =>
            {
                return await Requests.Requests.GetConfig(request);
            });
            // PING
            app.MapGet("ping", (HttpContext httpContext) =>
            {
                return "pong";
            });

        }
    }
}
