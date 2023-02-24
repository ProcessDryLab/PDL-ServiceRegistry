﻿using System.IO;
using ServiceRegistry.ConnectedNodes;

namespace ServiceRegistry.Endpoints
{
    public class Endpoints
    {
        public Endpoints(WebApplication app)
        {
            app.MapGet("resources/miners", (HttpContext httpContext) =>
            {
                return ConnectedNodes.ConnectedNodes.Instance.Filter("Miner");
            })
            .WithName("GetMiners");

            app.MapGet("resources/repositories", (HttpContext httpContext) =>
            {
                return ConnectedNodes.ConnectedNodes.Instance.Filter("Repository");
            })
            .WithName("GetRepositories");

            app.MapGet("Ping", (HttpContext httpContext) =>
            {
                return "Pong";
            })
            .WithName("Ping");

            app.MapPost("resources/Repositories", async (HttpContext httpContext) =>
            {
                return await Requests.Requests.GetConfig("https://localhost:4000/configurations");
            })
            .WithName("PostRepositories");

            app.MapPost("resources/Miners", async (HttpContext httpContext) =>
            {
                return await Requests.Requests.GetConfig("http://localhost:5000/configurations");
            })
            .WithName("PostMiners");

        }
    }
}
