using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PathMarker
{
    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    // constructor for this class
    public PathMarker(MapLocation l, float g, float h, float f, GameObject marker, PathMarker p)
    {
        location = l;
        G = g;
        H = h;
        F = f;
        this.marker = marker; // set marker defined in above function with passed-in parameter marker
        parent = p;
    }

    // overried the base Equals method so that we can compare the relevant info that will differ for instances of this class
    // here, the relevant info is location - many other pieces of information could be the same!
    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.Equals(((PathMarker)obj).location);
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}

public class FindPathAStar : MonoBehaviour
{
    [Header("Path Boundaries")]
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;
    
    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    // visual representations only
    [Header("Path representation")]
    public GameObject start;
    public GameObject end;
    public GameObject pathP; // point point

    // navigation
    PathMarker goalNode;
    PathMarker startNode;
    PathMarker lastPos;
    bool done = false;

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject marker in markers)
        {
            Destroy(marker);
        }
    }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        // find free, e.g. open, locations on this randomly generated maze
        List<MapLocation> locations = new List<MapLocation>();
        for (int z = 1; z < maze.depth - 1; z++)
        {
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x, z] != 1)
                {
                    locations.Add(new MapLocation(x, z));

                }
            }
        }

        // create start and end points
        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale); // translate grid location to game actual location (Vector3)
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0,
                                   Instantiate(start, startLocation, Quaternion.identity),
                                   null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale); // translate grid location to game actual location (Vector3)
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0,
                                  Instantiate(end, goalLocation, Quaternion.identity),
                                  null);

        open.Clear();
        closed.Clear();
        open.Add(startNode);
        lastPos = startNode;
    }

    bool IsClosed(MapLocation marker)
    {
        foreach (PathMarker p in closed)
        {
            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    void Search(PathMarker thisNode)
    {
        if (thisNode == null) return;
        if (thisNode.Equals(goalNode)) { done = true; return; }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + thisNode.location; 

            // locations to exclude from search
            if (maze.map[neighbour.x, neighbour.z] == 1) { continue; }
            if (neighbour.x >= maze.width || neighbour.z >= maze.depth) { continue; }
            if (neighbour.x < 1 || neighbour.z < 1) { continue; }
            if (IsClosed(neighbour)) { continue; }

            // calculate fitness
            float G = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float F = G + H;

            // provide visual feedback - not required in production algorithm!
            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0, neighbour.z * maze.scale), Quaternion.identity);
            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();
            values[0].text = "G: " + G.ToString("0.00");
            values[1].text = "H: " + H.ToString("0.00");
            values[2].text = "F: " + F.ToString("0.00");

            // add a new PathMarker if not already in open list. If already in list, simply update with current fitness stats
            if (!UpdateMarker(neighbour, G, H, F, thisNode))
            {
                open.Add(new PathMarker(neighbour, G, H, F, pathBlock, thisNode));
            }

        }

        // find winning node. To do this, order by F and then by H as tiebreaker. We use the Linq library for this!
        open = open.OrderBy(p => p.F).ThenBy(n => n.H).ToList<PathMarker>();

        // store winning node in closed list then remove from open list
        //PathMarker pm = open[0]; // this should be sufficient, but below is a more robust version
        PathMarker pm = (PathMarker)open.ElementAt(0);
        closed.Add(pm);
        open.RemoveAt(0);

        // change color of pathmarker to signal it is winning node
        pm.marker.GetComponent<Renderer>().material = closedMaterial;
        lastPos = pm;
    }

    // update any open nodes with values quantifying fitness
    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginSearch();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Search(lastPos); // delete me later? for testing!
        }
    }
}
