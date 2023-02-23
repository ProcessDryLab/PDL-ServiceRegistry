using System.IO;
using ServiceRegistry.ConnectedNodes;

namespace ServiceRegistry.Endpoints
{
    public class Endpoints
    {
        //ConnectedNodes.ConnectedNodes nodes = new ConnectedNodes.ConnectedNodes();
        public Endpoints(WebApplication app)
        {
            app.MapGet("resources/miners", (HttpContext httpContext) =>
            {
                Console.Write("Service registry has recieved a quest on /test");
                return ConnectedNodes.ConnectedNodes.Instance.nodes.Where(n => n.Value.type == "miner");
                //return "[miner1, miner2, miner3]";
            })
            .WithName("GetMiners");

            app.MapGet("resources/repositories", (HttpContext httpContext) =>
            {
                Console.Write("Service registry has recieved a quest on /test");
                return ConnectedNodes.ConnectedNodes.Instance.nodes.Where(n => n.Value.type == "repository");
                //return "[Repo1, repo2, repo3]";
            })
            .WithName("GetRepositories");

            app.MapPost("resources/Add/Repository", (HttpContext httpContext) =>
            {
                Console.Write("Service registry has recieved a quest on /Node");

                Node node = new Node();
                node.path = "path";
                node.type = "repository";

                ConnectedNodes.ConnectedNodes.Instance.AddNode(node.path, node);

                Console.Write(ConnectedNodes.ConnectedNodes.Instance.nodes);

                return ConnectedNodes.ConnectedNodes.Instance.nodes;
            })
            .WithName("PostRepositories");

        }
    }
}
