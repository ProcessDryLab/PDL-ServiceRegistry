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

            app.MapPost("/connections/filters", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                bool validRequest = bodyString.TryParseJson(out List<string> requestedHostsList);
                if (!validRequest) return Results.BadRequest($"Request body: {bodyString} is not a valid list");

                var OnlineStatus = ConnectedNodes.ConnectedNodes.Instance.GetOnlineStatus();
                var requestedHostsOnlineStatus = requestedHostsList.Where(key => OnlineStatus.ContainsKey(key)).Select(k => new { host = k, status = OnlineStatus[k] });

                return Results.Ok(requestedHostsOnlineStatus);
            });

            app.MapGet("config/filters", async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string bodyString = await body.ReadToEndAsync();
                Console.WriteLine("Filters: " + bodyString);

                bool validRequest = bodyString.TryParseJson(out List<string> filters);
                if (!validRequest || filters == null || filters.Count == 0) return Results.BadRequest($"Request body: {bodyString} is not a valid list");

                return Results.Ok(filters); // TODO: get and return the configs

            });

        }
    }
}
