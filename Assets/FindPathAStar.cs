using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



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
        for (int z = 1; z < maze.depth; z++)
        {
            for (int x = 1; x < maze.depth; x++)
            {
                if (maze.map[x, z] != 1)
                {
                    locations.Add(new MapLocation(x, z));

                }
            }
        }

        // create start and end points
        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x, 0, locations[0].z); // translate grid location to game actual location (Vector3)
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0,
                                    Instantiate(start, startLocation, Quaternion.identity),
                                   null);

        Vector3 goalLocation = new Vector3(locations[1].x, 0, locations[1].z); // translate grid location to game actual location (Vector3)
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0,
                                    Instantiate(end, goalLocation, Quaternion.identity),
                                   null);

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
        
    }
}
