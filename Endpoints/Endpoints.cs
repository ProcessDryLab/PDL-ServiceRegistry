using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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
                return JsonConvert.SerializeObject(ConnectedNodes.ConnectedNodes.Instance.GetRegisteredNodes(NodeType.Miner));
            });

            app.MapPost("/miners", async (HttpRequest request) =>
            {
                return await Requests.Requests.GetConfig(request);
            });
            // REPOSITORIES
            app.MapGet("/repositories", (HttpContext httpContext) =>
            {
                return JsonConvert.SerializeObject(ConnectedNodes.ConnectedNodes.Instance.GetRegisteredNodes(NodeType.Repository));
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

            app.MapPost("/connections/filters", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                bool validRequest = bodyString.TryParseJson(out List<string> filters);
                if (!validRequest || filters == null || filters.Count == 0) return Results.BadRequest($"Request body: {bodyString} is not a valid list");

                var onlineStatus = ConnectedNodes.ConnectedNodes.Instance.GetOnlineStatus();
                var requestedNodeUrls = filters.Where(key => onlineStatus.ContainsKey(key)).Select(k => new { host = k, status = onlineStatus[k] });

                return Results.Ok(requestedNodeUrls);
            });

            app.MapPost("config/filters", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                Console.WriteLine("Filters: " + bodyString);

                bool validRequest = bodyString.TryParseJson(out List<string> filters);
                if (!validRequest || filters == null || filters.Count == 0) return Results.BadRequest($"Request body: {bodyString} is not a valid list");

                var configurations = ConnectedNodes.ConnectedNodes.Instance.GetConfigurations();
                var requestedNodeConfigs = filters.Where(key => configurations.ContainsKey(key)).Select(k => new { host = k, config = configurations[k] });

                return Results.Ok(requestedNodeConfigs); // TODO: get and return the configs
            });

        }
    }
}
