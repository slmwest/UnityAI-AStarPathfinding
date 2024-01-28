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
    bool done = false; //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
