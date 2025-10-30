using System.Collections.Generic;
using UnityEngine;

public class QuadtreeSearchDemo : MonoBehaviour
{
    [Header("Settings")]
    public GameObject pointPrefab;
    public int numberOfPoints = 600;
    public int capacity = 40;
    public Rect area = new Rect(-50, -50, 100, 100);
    public Transform searchPlane;     // Assign your Plane object here
    public float searchInterval = 0.5f;

    private Quadtree quadtree;
    private List<GameObject> pointObjects = new List<GameObject>();
    private float nextSearchTime;

    void Start()
    {
        GameObject root = new GameObject("QuadtreeRoot");
        quadtree = new Quadtree(root, area, capacity);
        root.transform.parent = transform;

        GeneratePoints(numberOfPoints);
    }

    void Update()
    {
        quadtree.Draw();

        if (searchPlane == null) return;

        // Run search every few seconds
        if (Time.time >= nextSearchTime)
        {
            nextSearchTime = Time.time + searchInterval;
            PerformSearch();
        }
    }

    void GeneratePoints(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(area.xMin, area.xMax),
                Random.Range(area.yMin, area.yMax),
                0
            );

            GameObject p = Instantiate(pointPrefab, pos, Quaternion.identity, transform);
            p.GetComponent<Renderer>().material.color = Color.white;
            pointObjects.Add(p);
            quadtree.Insert(new Point(pos, p));
        }
    }

    void PerformSearch()
    {
        // Reset all point colors
        foreach (GameObject p in pointObjects)
            p.GetComponent<Renderer>().material.color = Color.white;

        // Define search rect based on plane position and scale
        Vector3 pos = searchPlane.position;
        Vector3 scale = searchPlane.localScale * 10f; // Unity plane = 10x10 units base size
        Rect searchRect = new Rect(
            pos.x - scale.x / 2f,
            pos.y - scale.z / 2f,
            scale.x,
            scale.z
        );

        // Search using quadtree
        List<Point> foundPoints = new List<Point>();
        quadtree.Query(searchRect, foundPoints);

        // Colorize found points
        foreach (Point p in foundPoints)
        {
            if (p.gameObject != null)
                p.gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            new Vector3(area.x + area.width / 2, area.y + area.height / 2, 0),
            new Vector3(area.width, area.height, 0)
        );
    }
}

// Represents a point in space, with reference to its GameObject
public class Point
{
    public Vector3 position;
    public GameObject gameObject;

    public Point(Vector3 position, GameObject gameObject)
    {
        this.position = position;
        this.gameObject = gameObject;
    }
}

public class Quadtree
{
    private Rect bounds;
    private int capacity;
    private List<Point> points;
    private bool divided;

    private Quadtree northeast;
    private Quadtree northwest;
    private Quadtree southeast;
    private Quadtree southwest;

    private GameObject gameObject;
    private LineRenderer lineRenderer;

    public Quadtree(GameObject go, Rect bounds, int capacity)
    {
        this.bounds = bounds;
        this.capacity = capacity;
        this.points = new List<Point>();
        this.gameObject = go;

        lineRenderer = go.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5;
        lineRenderer.loop = true;
        lineRenderer.widthMultiplier = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }

    public void Insert(Point p)
    {
        if (!bounds.Contains(new Vector2(p.position.x, p.position.y)))
            return;

        if (points.Count < capacity && !divided)
        {
            points.Add(p);
            return;
        }

        if (!divided)
            Subdivide();

        northeast.Insert(p);
        northwest.Insert(p);
        southeast.Insert(p);
        southwest.Insert(p);
    }

    private void Subdivide()
    {
        float x = bounds.x;
        float y = bounds.y;
        float w = bounds.width / 2f;
        float h = bounds.height / 2f;

        GameObject ne = new GameObject("NE");
        GameObject nw = new GameObject("NW");
        GameObject se = new GameObject("SE");
        GameObject sw = new GameObject("SW");

        ne.transform.parent = gameObject.transform;
        nw.transform.parent = gameObject.transform;
        se.transform.parent = gameObject.transform;
        sw.transform.parent = gameObject.transform;

        northeast = new Quadtree(ne, new Rect(x + w, y + h, w, h), capacity);
        northwest = new Quadtree(nw, new Rect(x, y + h, w, h), capacity);
        southeast = new Quadtree(se, new Rect(x + w, y, w, h), capacity);
        southwest = new Quadtree(sw, new Rect(x, y, w, h), capacity);

        divided = true;
    }

    // Quadtree search
    public void Query(Rect range, List<Point> found)
    {
        if (!bounds.Overlaps(range))
            return;

        foreach (Point p in points)
        {
            if (range.Contains(new Vector2(p.position.x, p.position.y)))
                found.Add(p);
        }

        if (divided)
        {
            northeast.Query(range, found);
            northwest.Query(range, found);
            southeast.Query(range, found);
            southwest.Query(range, found);
        }
    }

    public void Draw()
    {
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(bounds.x, bounds.y, 0);
        corners[1] = new Vector3(bounds.x + bounds.width, bounds.y, 0);
        corners[2] = new Vector3(bounds.x + bounds.width, bounds.y + bounds.height, 0);
        corners[3] = new Vector3(bounds.x, bounds.y + bounds.height, 0);
        corners[4] = corners[0];
        lineRenderer.SetPositions(corners);

        if (divided)
        {
            northeast.Draw();
            northwest.Draw();
            southeast.Draw();
            southwest.Draw();
        }
    }
}
