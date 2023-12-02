using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    private Camera mainCamera;
    private RectTransform mapRT;
    private Transform panelTf;
    private float canvasHalfWidth;
    private float canvasHalfHeight;

    private LineRenderer markerLr;
    private bool isNavigating;

    private RectTransform currLocRT;
    [HideInInspector] public RectTransform destMarkerRT;

    // Waypoint
    struct WayPoint
    {
        public WayPoint(Vector2 gpsCoord, GameObject obj, RectTransform rt)
        {
            GpsCoord = gpsCoord;
            Obj = obj;
            Rt = rt;
        }

        public Vector2 GpsCoord { get; }
        public GameObject Obj { get; }
        public RectTransform Rt { get; }
    }

    private List<WayPoint> waypoints;
    private bool isRecording = true;
    [SerializeField] private float waypointInterval = 0.00005f;
    private GameObject waypointPrefab;
    private Transform waypointsTf;
    private static GPS gps;

    // Navigation
    private GameObject segmentPrefab;
    private List<GameObject> segmentObjs;
    private List<LineRenderer> segmentLrs;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mapRT = GameObject.Find("Map").GetComponent<RectTransform>();
        panelTf = GameObject.Find("Map Panel").GetComponent<Transform>();
        markerLr = GameObject.Find("Map").GetComponent<LineRenderer>();
        currLocRT = GameObject.Find("CurrLoc").GetComponent<RectTransform>();
        Rect canvasR = GameObject.Find("Main Canvas").GetComponent<RectTransform>().rect;
        canvasHalfWidth = canvasR.width / 2;
        canvasHalfHeight = canvasR.height / 2;
        gps = GameObject.Find("GPS").GetComponent<GPS>();
        markerLr = GameObject.Find("Map").GetComponent<LineRenderer>();
        waypointsTf = GameObject.Find("Waypoints").GetComponent<Transform>();
        waypoints = new List<WayPoint>();
        segmentObjs = new List<GameObject>();
        segmentLrs = new List<LineRenderer>();
        waypointPrefab = Resources.Load<GameObject>("CustomPrefabs/Waypoint");
        segmentPrefab = Resources.Load<GameObject>("CustomPrefabs/Segment");
    }

    void Update()
    {
        if (isNavigating)
        {
            List<Vector2> offsets = new List<Vector2> { destMarkerRT.offsetMin, currLocRT.offsetMin };
            List<List<Vector3>> positions = GetRoute(offsets, false);
            if (positions.Count > 0) markerLr.SetPositions(positions[0].ToArray());
        }
        if (isRecording) RecordWaypoints();
        else ReplayWaypoints();
    }

    public void StartMarkerNavigate(RectTransform markerRT)
    {
        markerLr.positionCount = 2;
        destMarkerRT = markerRT;
        isNavigating = true;
    }

    public void StopMarkerNavigate()
    {
        markerLr.positionCount = 0;
        destMarkerRT = null;
        isNavigating = false;
    }

    private void RecordWaypoints()
    {
        Vector2 gpsCoords = gps.GetGpsCoords();
        int numWaypoints = waypoints.Count;

        if (numWaypoints == 0 || (gpsCoords - waypoints[numWaypoints-1].GpsCoord).magnitude > waypointInterval)
        {
            GameObject newWaypoint = Instantiate(waypointPrefab, waypointsTf);
            newWaypoint.SetActive(false);
            waypoints.Add(new WayPoint(gpsCoords, newWaypoint, newWaypoint.GetComponent<RectTransform>()));
        }
    }

    private void ReplayWaypoints()
    {
        Vector2 currGps = gps.GetGpsCoords();
        int numWaypoints = waypoints.Count;

        if (numWaypoints == 0) return;

        if ((currGps - waypoints[numWaypoints - 1].GpsCoord).magnitude < waypointInterval / 3)
        {
            numWaypoints--;
            Destroy(waypoints[numWaypoints].Obj);
            waypoints.RemoveAt(numWaypoints);
        }

        List<Vector2> offsets = new List<Vector2>();

        for (int i = 0; i < numWaypoints; i++)
        {
            Vector2 waypointGps = waypoints[i].GpsCoord;
            RectTransform rtMap = waypoints[i].Rt;

            // Translate (offset) markers relative to map RT
            // Note: pos gives offsets in rotated MAP SPACE,
            //       but we must compute offsets in PANEL SPACE

            rtMap.offsetMin = mapRT.offsetMin + gps.GpsToMapPos(waypointGps.x, waypointGps.y);
            rtMap.offsetMax = rtMap.offsetMin;
            offsets.Add(rtMap.offsetMin);
        }
        offsets.Add(currLocRT.offsetMin);

        // Obtain the navigation route and resize the number of segments
        List<List<Vector3>> routes = GetRoute(offsets, true);
        if (routes.Count > segmentObjs.Count)
        {
            for (int i = 0; i < routes.Count - segmentObjs.Count; i++)
            {
                GameObject segment = Instantiate(segmentPrefab, transform);
                segmentObjs.Add(segment);
                segmentLrs.Add(segment.GetComponent<LineRenderer>());
            }
        }
        else if (routes.Count < segmentObjs.Count)
        {
            for (int i = routes.Count; i < segmentObjs.Count; i++)
            {
                Destroy(segmentObjs[i]);
            }

            segmentObjs.RemoveRange(routes.Count, segmentObjs.Count - routes.Count);
            segmentLrs.RemoveRange(routes.Count, segmentLrs.Count - routes.Count);
        }

        // Draw all segments
        for (int i = 0; i < routes.Count; i++)
        {
            segmentLrs[i].positionCount = routes[i].Count;
            segmentLrs[i].SetPositions(routes[i].ToArray());
        }
    }

    private List<List<Vector3>> GetRoute(List<Vector2> offsets, bool isWayPoint)
    {
        List<List<Vector3>> segments = new List<List<Vector3>>();
        List<Vector3> positions = new List<Vector3>();

        bool prevInScope = true;
        float maxHeight = canvasHalfHeight * 0.98f;
        float maxWidth = canvasHalfWidth * 0.98f;

        bool IsWithinScope(Vector2 offset)
        {
            return (Math.Abs(offset.x) <= maxWidth && Math.Abs(offset.y) <= maxHeight);
        }

        Vector3 GetIntersectionWithBorder(Vector2 outside, Vector2 inside)
        {
            Vector2 outsideToInside = inside - outside;
            float deltaX = Math.Abs(outside.x) - maxWidth;
            float deltaY = Math.Abs(outside.y) - maxHeight;
            float scale = deltaX > deltaY ? Math.Abs(deltaX / outsideToInside.x) : Math.Abs(deltaY / outsideToInside.y);

            return OffsetToPos(outside + outsideToInside * scale, isWayPoint);
        }

        for (int i = 0; i < offsets.Count; i++)
        {
            bool currInScope = IsWithinScope(offsets[i]);
            bool nextInScope = i == offsets.Count - 1 || IsWithinScope(offsets[i + 1]);

            if (currInScope)
            {
                if (!prevInScope)
                {
                    Vector3 prevIntersection = GetIntersectionWithBorder(offsets[i - 1], offsets[i]);
                    positions.Add(prevIntersection);
                }
                positions.Add(OffsetToPos(offsets[i], isWayPoint));
                if (!nextInScope)
                {
                    Vector3 nextIntersection = GetIntersectionWithBorder(offsets[i + 1], offsets[i]);
                    positions.Add(nextIntersection);
                    segments.Add(positions);
                    positions = new List<Vector3>();
                }
            }

            prevInScope = currInScope;
        }

        if (positions.Count > 1) segments.Add(positions);

        return segments;
    }

    // UI callback
    public void ToggleWaypointAction()
    {
        isRecording = !isRecording;

        foreach (var waypoint in waypoints)
        {
            if (!isRecording) waypoint.Obj.SetActive(true);
            else Destroy(waypoint.Obj);
        }

        if (isRecording)
        {
            waypoints.Clear();
            foreach (var segmentLr in segmentLrs) segmentLr.positionCount = 0;
            segmentLrs.Clear();
        }
    }

    // Utility Function
    private Vector3 OffsetToPos(Vector2 offset, bool isWayPoint)
    {
        if (!isWayPoint) offset -= mapRT.offsetMin;

        float c = isWayPoint ? 200f : 100f;
        float xScale = c / canvasHalfWidth;
        float yScale = c / canvasHalfWidth;

        return new Vector3(offset.x * xScale, offset.y * yScale, -25f);
    }
}
