using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Config;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events;
using MCGalaxy.Tasks;
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.DB;
using MCGalaxy.Maths;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

static readonly random = new Random();
static BufferedBlockSender read;

// IP: Create Render System
// TODO: Create Interior Board
// TODO: Make Rotation ID do something lol
// TODO: SIDES ARE 14x14 LIDS ARE 16x16
// TODO: Create Command Structure for Creating Edges, Nodes, and Maps
// TDL: /Tinder Command
// TODO: Create Command Structure for Drawing Completed Maps

namespace MCGalaxy
{
    public class WaveFunctionCollapse : Plugin
    {
        public override string name { get { return "WaveFunctionCollapse"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
        public override string welcome { get { return "Loaded Message!"; } }
        public override string creator { get { return "capitjeff21"; } }
        public override void Load(bool startup)
        {
            // Replace with correct level For Input
            read = new BufferedBlockSender(LevelInfo.FindExact("WFCtest"));
            Filing.Unpack();
        }

        // Called when this plugin is being unloaded (e.g. on server shutdown)
        public override void Unload(bool shutdown)
        {
            Filing.Pack();
        }

        // Displays help for or information about this plugin
        public override void Help(Player p)
        {
            p.Message("No help is available for this plugin.");
        }
    }
}

public class Edge
{
    private static Regex pattern = new Regex(@"^[A-Z]{2}[srfo0123][\^\!\$]$");
    private static Dictionary<string, Edge> edgeCache = new Dictionary<string, Edge>();

    public static bool isSide(Edge e) { return e.ID.Contains('$') && !e.ID.Contains("0") && !e.ID.Contains("1") && !e.ID.Contains("2") && !e.ID.Contains("3") && !e.ID.Contains("r"); }
    public static bool isTop(Edge e) { return e.ID.Contains('^') && !e.ID.Contains("s") && !e.ID.Contains("f") && !e.ID.Contains("o"); }
    public static bool isBottom(Edge e) { return e.ID.Contains('!') && !e.ID.Contains("s") && !e.ID.Contains("f") && !e.ID.Contains("o"); }

    public string ID;
    private HashSet<string> nonStandardMatch = new HashSet<string>();
    private BlockID[][] blockArray = new BlockID[16][16] blockArray;

    public static Dictionary<string, Edge> EdgeCache { get => edgeCache; set => edgeCache = value; }
    public static Regex Pattern { get => pattern; private set => pattern = value; }

    public static Delete(string id) { EdgeCache.Remove(id); }

    public Edge(string ID, Edge[] nsMatch, BlockID[][] blocks)
    {
        if (!Pattern.Match(ID).Success)
        {
            Filing.Pack();
            throw new Exception("ID must follow given ID rules.");
        }
        if (blocks.Length != 16 || blocks[0].Length != 16)
        {
            Filing.Pack();
            throw new Exception("Block Array Wrong Size!");
        }
        this.ID = ID;
        foreach (Edge s in nsMatch)
        {
            this.AddMatch(s);
        }
        this.blockArray = blocks;
        EdgeCache[ID] = this;
    }

    public Edge(string ID, BlockID[][] blocks)
    {
        this(ID, new Edge[], blocks);
    }

    public bool Matches(Edge other)
    {
        return (this.StandardMatch(other.ID) || nonStandardMatch.Contains(other.ID));
    }

    public bool Matches(string otherID)
    {
        return this.StandardMatch(otherID) || nonStandardMatch.Contains(otherID);
    }

    public bool StandardMatch(string otherID)
    {
        if (ID.Contains('s') && ID == otherID)
        {
            return true;
        }
        if (ID.Substring == otherID[0] && ((ID[1] == 'f' && otherID[1] == 'o') || (ID[1] == 'o' && otherID[1] == 'f')))
        {
            return true;
        }
    }

    public void AddMatch(Edge other, bool isOther = false)
    {
        nonStandardMatch.Add(other.ID);
        if (!isOther)
        {
            other.AddMatch(this, true);
        }
    }

    public void AddMatch(string other, bool isOther = false)
    {
        AddMatch(EdgeCache[other]);
    }

    public void RemoveMatch(Edge other, bool isOther = false)
        {
            other.RemoveMatch(this, true);
    {
        if (nonStandardMatch.Remove(other.ID) && !isOther)
        }
    }

    private string PackArray()
    {
        List<string> temp = new List<string>();
        foreach (BlockID[] a in this.blockArray)
        {
            temp.Add(string.Join(',', a);
        }
        return string.Join("|", temp.ToArray());
    }

    public string Pack(HashSet<string> adjs)
    {
        if (this.nonStandardMatch.Count > 0) { adjs.Add($"{this.ID}:{string.Join(',', nonStandardMatch.ToArray())}"); }
        return $"{this.ID}:{this.PackArray()}";
    }
}

public class Node
{
    // TDL: Create equals method to make it impossible to add duplicate Nodes to cache
    // TDL: Allow for BFS using a bool[] hasConnection
    private static Dictionary<int, Node> nodeCache = new Dictionary<int, Node>();
    private static int count = 1;

    private Edge Top;
    private Edge Bottom;
    private Edge S0;
    private Edge S1;
    private Edge S2;
    private Edge S3;
    private int interiorID { get; private set; };
    private BlockID[,,] blockArray;
    private int Rotation = 0;
    private bool consider;

    public static Dictionary<int, Node> NodeCache { get => nodeCache; private set => nodeCache = value; }
    public static int Count { get => count; set => count = value; }
    public bool Consider { get => consider; set => consider = value; }

	public static Delete(int id) { EdgeCache.Remove(id); }

	public Node(Edge[] edges, BlockID[,,] interior, int rotation = 0)
    {
        if (edges.Length != 6)
        {
            Filing.Pack();
            throw new Exception("You must define all 6 edges.");
        }
        if (!(Edge.isTop(edges[0]) && Edge.isBottom(edges[1]) && Edge.isSide(edges[2]) && Edge.isSide(edges[3]) && Edge.isSide(edges[4]) && Edge.isSide(edges[5])))
        {
            Filing.Pack();
            throw new Exception("One of your edges is not the correct type. Check the 3rd and 4th characters of your IDs");
        }
        Top = edges[0]; Bottom = edges[1]; S0 = edges[2]; S1 = edges[3]; S2 = edges[4]; S3 = edges[5];
        interiorID = Count++;
        this.Rotation = rotation;
        Consider = true;
        nodeCache.Add(interiorID, this);
    }

    public Node(string[] edges, int rotation = 0, BlockID[,,] interior)
    {
        if (edges.Length != 6)
        {
            Filing.Pack();
            throw new Exception("You must define all 6 edges.");
        }
        Edge[] myEdges = new Edge[6];
        int index = 0;
        foreach (string e in edges)
        {
            if Edge.EdgeCache.ContainsKey(e)

            {
                myEdges[index++] = Edge.EdgeCache[e];
            }
            else
            {
                myEdges[index++] = new Edge(e);
            }
        }

        this(myEdges, rotation);
    }

    public Edge[] getEdgeList() { return new Edge[] { Top, S0, S1, Bottom, S2, S3}; }
    private string getEdgeString() { return $"{Top.ID},{S0.ID},{S1.ID},{Bottom.ID},{S2.ID},{S3.ID},{Consider ? 't' : 'f'}"; }

    public string PackArray()
    {
        string result = "";
        for (int y = 0; y < 14;  y++)
        {
            if (y != 0) { result += "&"; }
            for (int x = 0; x < 14; x++)
            {
                if (x != 0) { result += "|"; }
                for (int z = 0; z < 14; z++)
                {
                    result += z == 0 ? blockArray[y, x, z] : "," + blockArray[y, x, z];
                }
            }
        }
        return result;
    }

    public string Pack(List<string> interiors)
    {
        interiors.Add(this.PackArray());
        return $"{this.interiorID}:{this.getEdgeString()}*{this.Rotation}";
    }
}

public class Map
{
    private static HashSet<string> Accessors = new HashSet<string>();
    
    private Point[,,] map;
    private int width;
    private int depth;
    private int height;
    private string name;

	public static HashSet<global::System.String> Accessors { get => Accessors; set => Accessors = value; }

	public Map(int width, int depth, int height, string name)
    {
        this.name = name;
        this.width = width; this.depth = depth; this.height = height
        map = new Point[height, depth, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    map[i, j, k] = new Point();
                }
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    map[i, j, k].InitNeighbors(this.GenNeighbors(i, j, k));
                }
            }
        }

        this.Generate();
        this.Pack();
    }

    private Point[] GenNeighbors(int x, int z, int y)
    {
        Point Top;
        Point S0;
        Point S1;
        Point Bottom;
        Point S2;
        Point S3;

        Top = y == height - 1 ? null : map[x, y + 1, z];
        S0 = x == width - 1 ? null : map[x + 1, y, z];
        S1 = z == depth - 1 ? null : map[x, y, z + 1];
        Bottom = y == 0 ? null : map[x, y - 1, z];
        S2 = x == 0 ? null : map[x - 1, y, z];
        S3 = z == 0 ? null : map[x, y, z - 1];
        return [Top, S0, S1, Bottom, S2, S3];
    }

    public void Generate()
    {
        int remaining = height * width * depth;
        while remaining > 0 {
            Point force;
            remaining = CountRemaining(out force);
            if (force != null) { force.Declare(); }
        }

        int CountRemaining(out Point lowest)
        {
            int low = Integer.MaxValue;
            List<Point> lowPoint = new List<Point>;
            int count = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        if (!map[i, j, k].determined) 
                        {
                            count++;
                            if (map[i, j, k].options.Count < low)
                            {
                                lowPoint.Clear();
                                lowPoint.Add(map[i, j, k]);
                                low = map[i, j, k].options.Count;
                            }
                            else if (map[i, j, k].options.Count == low)
                            {
                                lowPoint.Add(map[i,j, k]);
                            }
                        }
                    }
                }
            }
            lowest = random.Next(lowPoint.Count);
            return count;
        }
    }

    public void Pack()
    {
        Accessors.Add(this.name);
        // REPLACE with Correct PR Directory
        using (StreamWriter writer = new StreamWriter($"\wmaps\{this.name}.qrsm", false))
        {
            for (int y = 0;  y < height; y++)
            {
				if (y != 0) { writer.Write('&'); }
				for (int x = 0; x < width; x++)
                {
                    if (x != 0) { writer.Write('|'); }
                    for (int z = 0; z < depth; z++)
                    {
                        writer.Write(z==0 ? ("" + map[y, x, z].value) : ("," + map[y, x, z].value));
                    }
                }
            }
        }
    }

    private class Point
    {
        private HashSet<int> options = new HashSet<int>;
        private Point[] neighbors = new Point[]
        private bool determined = false;
        private int value;

        public Point()
        {
            foreach (int n in Node.NodeCache.Keys)
            {
                if (Node.NodeCache[n].Consider)
                {
                    options.Add(n);
                }
            }
        }

        public void InitNeighbors(Point[] neighbors)
        {
            this.neighbors = neighbors;
        }

        public void Declare()
        {
            int chosen = options.ElementAt(random.Next(options.Count));
            determined = true;
            options.Clear();
            options.Add(chosen);
            value = determined;
            Notify(options);
        }

        public void Notify(HashSet<int> remaining)
        {
            foreach (Point n in neighbors)
            {
                if (n is not null)
                {
                    n.Recieve(remaining, neighbors.IndexOf(n));
                }
            }
        }

        public void Recieve(HashSet<int> remaining, int direction)
        {
            if (determined) { return; }
            if (Filter(remaining, direction)) {
                CheckForcedDeclare();
                Notify(options);
            }
        }

        public bool Filter(HashSet<int> remaining, int direction)
        {
            // This might be a shallow copy but also might not matter?
            HashSet<int> incompatible = new HashSet<int>(options);
            foreach (int n in remaining)
            {
                foreach (int m in incompatible)
                {
                    bool comp = Node.NodeCache[n].getEdgeList()[(direction + 3) % 6].Matches(Node.NodeCache[m].getEdgeList()[direction]);
                    if comp { incompatible.Remove(m); }
                }
            }
            
            foreach (int n in incompatible)
            {
                this.options.Remove(n);
            }
            return incompatible.Count > 0;
        }

        public void CheckForcedDeclare()
        {
            if (options.Count == 1)
            {
                determined = true;
                value = options.ToArray()[0];
            }
            else if (options.Count == 0)
            {
                determined = true;
                value = 0;
            }
        }

    }

}

public namespace Filing
{
    public void Pack()
    {
        HashSet<string> mapAccessors = new HashSet<string>(Map.Accessors);
        HashSet<string> nodes = new HashSet<string>;
        HashSet<string> edges = new HashSet<string>;
        HashSet<string> adjacencies = new HashSet<string>;
        List<string> interiors = new List<string>;
        foreach (int n in Node.NodeCache.Keys)
        {
            if (n == 0) { continue; }
            nodes.Add(Node.NodeCache[n].Pack(interiors));
        }
        foreach (string e in Edge.EdgeCache.Keys)
        {
            edges.Add(Edge.EdgeCache[e].Pack(adjacencies));
        }
		// REPLACE with Correct PR Directory
		using (StreamWriter writer = new StreamWriter("mapAccessors.qrs", false))
        {
            foreach (string m in mapAccessors)
            {
                writer.WriteLine(m);
            }
        }
        using (StreamWriter writer = new StreamWriter("nodes.qrs", false))
        {
            foreach (string n in nodes)
            {
                writer.WriteLine(n);
            }
        }
        using (StreamWriter writer = new StreamWriter("edges.qrs", false))
        {
            foreach (string e in edges)
            {
                writer.WriteLine(e);
            }
        }
        using (StreamWriter writer = new StreamWriter("adjacencies.qrs", false))
        {
            foreach (string a in adjacencies)
            {
                writer.WriteLine(a);
            }
        }
        using (StreamWriter writer = new StreamWriter("interiors.qrs", false))
        {
            foreach (string i in interiors)
            {
                writer.WriteLine(i);
            }
        }
    }

    public void Unpack()
    {
        string[] interiorF = File.ReadAllLines("interiors.qrs");
        string[] adjacencyF = File.ReadAllLines("adjacencies.qrs");
        string[] edgeF = File.ReadAllLines("edges.qrs");
        string[] nodeF = File.ReadAllLines("nodes.qrs");
        string[] accessorsF File.ReadAllLines("mapAccessors.qrs");

        // I'm so sorry for this design pattern
        Queue<List<List<string[]>>> IntArgs = new Queue<List<List<string[]>>>();
        foreach (string s in interiorF)
        {
            string[] a = s.Split('&');
            List<List<string[]>> complete = new List<List<string[]>>();
            foreach (string s2 in a)
            {
				string[] b = s2.Split('|');
				List<string[]> almost = new List<string[]>();
                foreach (string s3 in b)
                {
                    string[] c = s3.Split(',');
                    almost.Add(c);
                }
                complete.Add(almost);
            }
            IntArgs.Enqueue(complete);
        }

        Dictionary <int, BlockID[,,]> interiorCache = new Dictionary<int, BlockID[,,]>();
        int ind = 1;
        while (IntArgs.Count > 0)
        {
            List<List<string[]>> arg = IntArgs.Dequeue();
            BlockID[,,] blockArray = new BlockID[14, 14, 14];
            for (int y = 0; y < 14; y++)
            {
                for (int x = 0; x < 14; x++)
                {
                    for (int z = 0; z < 14; z++)
                    {
                        blockArray[y, x, z] = (BlockID)arg[y][x][z];
                    }
                }
            }
            interiorCache[ind] = blockArray;
        }

        Queue<List<string[]>> AdjArgs = new Queue<List<string[]>>();
        foreach(string s in adjacencyF)
        {
            string[] a = s.Split(':');
            List<string[]> complete = new List<string[]>();
            foreach (string s2 in a)
            {
                string[] b = s2.Split(',');
                complete.Add(b);
            }
            AdjArgs.Enqueue(complete);
        }

        Queue<List<List<string[]>>> EdgeArgs = new Queue<List<List<string[]>>>();
        foreach (string s in edgeF)
        {
            string[] a = s.Split(':');
            List<List<string[]>> complete = new List<List<string[]>>();
            foreach (string s2 in a)
            {
                string[] b = s2.Split('|');
                List<string[]> almost = new List<string[]>();
                foreach (string s3 in b)
                {
                    string[] c = s3.Split(",");
                    almost.Add(c);
                }
                complete.Add(almost);
            }
            EdgeArgs.Enqueue(complete);
        }

        while (EdgeArgs.Count > 0)
        {
            List<List<string[]>> EdgeArg = EdgeArgs.Dequeue();
            string[][] blockStrings = EdgeArg[1].ToArray();
            List<BlockID[]> blocks = new List<BlockID[]>(); 
            foreach (string[] a in blockStrings)
            {
                blocks.Add(a.Select(n => Convert.ToUInt16(n)).ToArray());
            }
            new Edge(EdgeArg[0][0][0], blocks.ToArray());
        }

        while (AdjArgs.Count > 0)
        {
            // Could convert to 2D array, but would be a waste of compute time
            List<string[]> arg = AdjArgs.Dequeue();
            Edge access = Edge.NodeCache[arg[0][0]];
            foreach (string s in arg[1])
            {
                // TEST/TODO: What happens when we delete an edge that others depend on? Gotta make sure we remove all matches on deletion
                access.AddMatch(s);
            }
        }

        // Add blank node, stopping case, matches to everything but nothing matches to it.
        BlockID[,,] tempInt = new BlockID[14,14,14];
        BlockID[][] tempWall = new BlockID[16][16];
        Edge[] edgeList = new Edge[] { new Edge("AAr^", tempWall), new Edge("AAr!", tempWall), new Edge("AAs$", tempWall), Edge.EdgeCache["AAs$"], Edge.EdgeCache["AAs$"], Edge.EdgeCache["AAs$"] };
        new Node(edgeList, tempInt);

        Queue<List<string[]>> NodeArgs = new Queue<List<string[]>>();
        char[] delims = { ':', '*' };
        foreach (string s in nodeF)
        {
            List<string[]> complete = new List<string[]>();
            string[] a = s.Split(delims);
            foreach (string s2 in a)
            {
                string[] b = s2.Split(',');
                complete.Add(b);
            }
            NodeArgs.Enqueue(complete);
        }
        int 
        while (NodeArgs.Count > 0)
        {
            List<string[]> NodeArg = NodeArgs.Dequeue();
            // TEST: I think this worked?
            new Node(NodeArg[1], NodeArg[2][0], interiorCache[(int)NodeArg[0][0]]);
        }

        foreach (string s in accessorsF)
        {
            Map.Accessors.Add(s);
        }
    }
}

public namespace ReadWrite
{
    private var SideBoard = null;
    private var LidBoard = null;

    // Coords are for the top left of the board
    public void RegisterSideBoard(Level l, ushort x, ushort y, ushort z, char _axis)
    {
        if (_axis != 'x' && _axis != 'z') { throw new Exception("The axis must be of type char and either 'x' or 'z'"); }
        SideBoard = new { Level lvl = l, ushort[] pos = new ushort[] {x, y, z}, axis = _axis };
        ClearSide();
    }

    public void AddSide(string id)
    {
        if (!Edge.Pattern.Match(ID).Success || !id.Contains('$')) { throw new Exception("ID must follow given ID rules, does your ID end with [$]?"); }
        new Edge(id, ReadSideBlocks());
    }

    public BlockID[][] ReadSideBlocks()
    {
		if (SideBoard is null) { throw new Exception("Board Cannot be Null"); }
        BlockID[][] contents = new BlockID[16][16];
        for (int y = 0; y < 16; y++)
        {
            for (int xz = 0; xz < 16; xz++)
            {
                contents[y][xz] = SideBoard.axis == 'x' ? FastGetBlock((ushort) (SideBoard.pos[0] + xz), (ushort) (SideBoard.pos[1] - y), (ushort) SideBoard.pos[2]) : FastGetBlock((ushort) SideBoard.pos[0], (ushort) (SideBoard.pos[1] - y), (ushort) (SideBoard.pos[2] + xz));
            }
        }
        ClearSide();
		return contents;
    }

	public void ClearSide()
	{
		if (SideBoard is null) { return; }
		for (int y = 0; y < 16; y++)
		{
			for (int xz = 0; xz < 16; xz++)
			{
                int index = SideBoard.axis == 'x' ? PosToInt(SideBoard.pos[0] + (ushort) xz, SideBoard.pos[1] - (ushort) y, SideBoard.pos[2]) : PosToInt(SideBoard.pos[0], SideBoard.pos[1] - (ushort) y, SideBoard.pos[2] + (ushort) xz);
                read.Add(index, Block.Air);
			}
		}
        read.Flush();
	}

	public void RegisterLidBoard(Level l, ushort x, ushort y, ushort z)
    {
        LidBoard = new { Level lvl = l, ushort[] pos = new ushort[] { x, y, z } };
		ClearLid();
	}

    public void AddLid(string id)
    {
		if (!Edge.Pattern.Match(ID).Success || !id.Contains('^') || !id.Contains('!')) { throw new Exception("ID must follow given ID rules, does your ID end with [^] or [!]?"); }
        Edge temp = new Edge(id, ReadLidBlocks());
	}

    public BlockID[][] ReadLidBlocks()
    {
		if (LidBoard is null) { throw new Exception("Board Cannot be Null"); }
        BlockID[][] contents = new BlockID[16][16];
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                contents[x][z] = FastGetBlock(LidBoard.pos[0] + (ushort)x, LidBoard.pos[1], LidBoard.pos[2] + (ushort)z);
            }
        }
		ClearLid()
		return contents;
	}

	public void ClearLid()
	{
		if (LidBoard is null) { return; }
		for (int x = 0; x < 16; x++)
		{
			for (int z = 0; z < 16; z++)
			{
                int index = PosToIndex(LidBoard.pos[0] + (ushort)x, LidBoard.pos[1], LidBoard.pos[2] + (ushort)z);
                read.Add(index, Block.Air);
			}
		}
		read.Flush();
	}

    public void RenderTempNode(var tempNode)
    {
        // TODO
    }

    public void RenderNode(int id)
    {
        // TODO
    }

    public void RenderEdge(string ID)
    {
        // TODO
    }

    public void TinderRender(string ID, string otherID)
    {
        // TODO
    }

    public void UnregisterSide() { SideBoard = null; }
    public void UnregisterLid() { LidBoard = null; }
}

static class Access
{
    private static enum await { registerSide, registerLid, registerInt, fresh, side, lid, interior, call }
    private static HashSet<await> awaiting = new List<await> { await.registerSide, await.registerLid, await.registerInt };
    private static var nodeConstructor = new { Edge[] edgeList = new Edge[6], BlockID[,,] interior };

    public static void RegSide(ushort x, ushort y, ushort z, char axis)
    {
        if (!awaiting.Contains(await.registerSide)) { return; }
        ReadWrite.RegisterSideBoard(x, y, z, axis);
        awaiting.Remove(await.registerSide);
        if (! (awaiting.Contains(await.registerSide) || awaiting.Contains(await.registerLid) || awaiting.Contains(await.registerInt) ) )
        {
            awaiting.Add(await.fresh);
        }
    }

	public static void RegLid(ushort x, ushort y, ushort z)
	{
		if (!awaiting.Contains(await.registerLid)) { return; }
		ReadWrite.RegisterLidBoard(x, y, z);
		awaiting.Remove(await.registerLid);
		if (!(awaiting.Contains(await.registerSide) || awaiting.Contains(await.registerLid) || awaiting.Contains(await.registerInt)))
		{
			awaiting.Add(await.fresh);
		}
	}

    public static void UnregSide() { ReadWrite.UnregisterSide(); }
    public static void UnregLid() { ReadWrite.UnregisterLid(); }

	// TODO: RegInt here

    // Yes I understand this is just access code for simplicity from the command interface.
	public static void SideAdd(string id) { ReadWrite.AddSide(id); }
    public static void LidAdd(string id) { ReadWrite.AddLid(id); }

	public static void Fresh()
    {
        if (!awaiting.Contains(await.fresh)) { return; }
        nodeConstructor = new { Edge[] edgeList = new Edge[6], BlockID[,,] interior };
        awaiting.Add(await.side); awaiting.Add(await.lid); awaiting.Add(await.interior);
    }

    public void ApplySide(int index, string edgeID)
    {
        if (!awaiting.Contains(await.side) || index < 0 || index > 3 || nodeConstructor.edgeList[index + 2] != null) { return; }
        nodeConstructor.edgeList[index + 2] = Edge.EdgeCache[edgeID];
        ReadWrite.RenderTempNode(nodeConstructor);
        if (nodeConstructor.edgeList[2] != null && nodeConstructor.edgeList[3] != null && nodeConstructor.edgeList[4] != null && nodeConstructor.edgeList[5] != null)
        {
            awaiting.Remove(await.side);
            if (awaiting.Count == 0)
            {
                awaiting.Add(await.call);
            }
        }
    }

	public void ApplyLid(string edgeID)
	{
        char type = edgeID[3];
        if (!awaiting.Contains(await.side) || !(type == '^' || type == '!') || (type == '^' && nodeConstructor.edgeList[0] != null) || (type == '!' && nodeConstructor.edgeList[1] != null)) { return; }
		if (type == '^') { nodeConstructor.edgeList[0] = Edge.EdgeCache[edgeID]; }
        else { nodeConstructor.edgeList[1] = Edge.EdgeCache[edgeID]; }
		ReadWrite.RenderTempNode(nodeConstructor);
        if (nodeConstructor.edgeList[0] != null && nodeConstructor.edgeList[1] != null)
		{
			awaiting.Remove(await.lid);
			if (awaiting.Count == 0)
			{
				awaiting.Add(await.call);
			}
		}
	}

    // TODO: Apply Interior here

    public void AddNode()
    {
        if (!awaiting.Contains(await.call)) { return; }
        new Node(nodeConstructor.edgeList, nodeConstructor.interior);
        awaiting.Remove(await.call);
        awaiting.Add(await.fresh);
    }

    public void AddMatch(string edgeID, string otherID)
    {
        Node.NodeCache[edgeID].AddMatch(otherID);
    }

    public void DeleteMatch(string edgeID, string otherID)
    {
        Node.NodeCache[edgeID].RemoveMatch(otherID);
    }
}
