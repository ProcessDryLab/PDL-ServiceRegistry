namespace ServiceRegistry.ConnectedNodes
{
    public class ConnectedNodes
    {
        private static ConnectedNodes? instance = null;
        private ConnectedNodes() { }

        public static ConnectedNodes Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConnectedNodes();
                }
                return instance;
            }
        }

        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public void AddNode(string path, Node node)
        {
            if(!nodes.ContainsKey(path) && node != null)
            {
                nodes.Add(path, node);
            }
            
        }

        public void RemoveNode(string path, Node node)
        {
            nodes.Remove(path);
        }

        public Node GetNode(string path)
        {
            return nodes[path];
        }
    }
}
