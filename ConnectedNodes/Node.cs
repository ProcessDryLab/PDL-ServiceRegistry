namespace ServiceRegistry.ConnectedNodes
{
    public class Node
    {
        public string name; //url location
        public string type; //Miner, repository or service registry?
        public List<Param> parameters;
    }

    public class Param
    {
        public string name;
        public string type;
    }
}
