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
            // Get list of registered Miner URLs
            app.MapGet("/miners", (HttpContext httpContext) =>
            {
                return JsonConvert.SerializeObject(ConnectedNodes.ConnectedNodes.Instance.GetRegisteredNodes(NodeType.Miner));
            });
            // Add a new Miner URL
            app.MapPost("/miners", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                return await ConnectedNodes.ConnectedNodes.Instance.AddNode(bodyString, NodeType.Miner);
            });
            // Remove a new Miner URL
            app.MapDelete("/miners", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                return ConnectedNodes.ConnectedNodes.Instance.RemoveNode(bodyString, NodeType.Miner);
            });

            // REPOSITORIES
            // Get list of registered Repository URLs
            app.MapGet("/repositories", (HttpContext httpContext) =>
            {
                return JsonConvert.SerializeObject(ConnectedNodes.ConnectedNodes.Instance.GetRegisteredNodes(NodeType.Repository));
            });
            // Add a new Repository URL
            app.MapPost("/repositories", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                return await ConnectedNodes.ConnectedNodes.Instance.AddNode(bodyString, NodeType.Repository);
            });
            // Remove a new Repository URL
            app.MapDelete("/repositories", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                return ConnectedNodes.ConnectedNodes.Instance.RemoveNode(bodyString, NodeType.Repository);
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

                bool validRequest = bodyString.TryParseJson(out List<string> filters);
                if (!validRequest || filters == null || filters.Count == 0) return Results.BadRequest($"Request body: {bodyString} is not a valid list");
                
                var configurations = ConnectedNodes.ConnectedNodes.Instance.GetConfigurations();
                var requestedNodeConfigs = filters.Where(filter => configurations.ContainsKey(filter)).Select(url => new { host = url, config = configurations[url] });
                
                string nodeConfigsString = JsonConvert.SerializeObject(requestedNodeConfigs, Formatting.Indented);
                return Results.Text(nodeConfigsString);
            });

        }
    }
}
