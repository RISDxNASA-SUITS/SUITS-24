// using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;



public enum MarkerType
{
    POI,
    Rover,
    Obstacle,
    RoverMarker,
};
public enum MarkerActionMode
{
    None,
    Add,
    Edit,
    Select,
}

public class MarkerController : MonoBehaviour
{
    private class Marker
    {
        public readonly MarkerType Type;
        public Vector2 GpsCoord;
        public readonly GameObject MapMarkerObj;
        public readonly RectTransform MapMarkerRT;
        // private readonly GameObject compassMarkerObj;
        // private readonly RectTransform compassMarkerRT;

        public Marker(MarkerType type, Vector2 gpsCoord)
        {
            Type = type;
            GpsCoord = gpsCoord;
            MapMarkerObj = Instantiate(prefabDict[type], markersTf);
            // compassMarkerObj = Instantiate(prefabDict[type], compassMarkersRT);
            MapMarkerRT = MapMarkerObj.GetComponent<RectTransform>();
            // compassMarkerRT = compassMarkerObj.GetComponent<RectTransform>();
        }

        public void CleanUp()
        {
            Destroy(MapMarkerObj);
            // Destroy(compassMarkerObj);
        }

        public void Update(Vector2 userGps, Vector3 userLook)
        {
            // Translate (offset) markers relative to map RT
            // Note: pos gives offsets in rotated MAP SPACE,
            //       but we must compute offsets in PANEL SPACE
            Vector2 mapMarkerOffset = gps.GpsToMapPos(GpsCoord.x, GpsCoord.y);
            MapMarkerRT.offsetMin = mapRT.offsetMin + mapMarkerOffset;
            MapMarkerRT.offsetMax = MapMarkerRT.offsetMin;

            // Adjust marker position on compass
            // Given userGPS and markerGPS, get markerDir that points from user to marker
            Vector2 markerRelGps = GpsCoord - userGps;  // delta (latitude, longitude)
            Vector3 markerDir = new Vector3(markerRelGps.y, 0.0f, markerRelGps.x);
            float angleToMarker = -Vector3.SignedAngle(markerDir, userLook, Vector3.up);
            // compassMarkerRT.offsetMin = new Vector2(angleToMarker * compassWidth, 0.0f);
            // compassMarkerRT.offsetMax = compassMarkerRT.offsetMin;
        }

        public void SetOpacity(float opacity)
        {
            MapMarkerObj.GetComponent<RawImage>().color = new Color(1, 1, 1, opacity);
            // compassMarkerObj.GetComponent<RawImage>().color = new Color(1, 1, 1, opacity);
        }

        public void Hide()
        {
            MapMarkerObj.SetActive(false);
            // compassMarkerObj.SetActive(false);
        }

        public void Show()
        {
            MapMarkerObj.SetActive(true);
            // compassMarkerObj.SetActive(true);
        }

        public bool IsHidden()
        {
            // return !MapMarkerObj.activeSelf || !compassMarkerObj.activeSelf;
            return !MapMarkerObj.activeSelf;
        }
    }

    [SerializeField] private float markerEditSensitivity = 0.000033f;

    private static RectTransform mapRT;
    // private static RectTransform mapRT, compassRT, compassMarkersRT;
    private static Transform markersTf;
    private static float compassWidth;

    private Camera mainCamera;
    private static GPS gps;
    // private static TSSAgent tssAgent;
    private static GameObject coord;
    private static TMPro.TMP_Text coordText;
    // private NotificationController notificationController;

    private static Dictionary<MarkerType, GameObject> prefabDict;
    private Dictionary<MarkerType, bool> showMarker;
    private Dictionary<GameObject, Marker> markers;
    [HideInInspector] public MarkerActionMode mode;
    private MarkerType selectedMarkerType;
    private Dictionary<MarkerType, GameObject> markerImages;
    private Dictionary<MarkerType, GameObject> glowingMarkerImages;
    private RectTransform currLocRT;
    private Marker currMarker;

    private GameObject actionButtons;
    private GameObject roverButtons;

    private Navigation navigation;
    private Marker navigatingTo;
    private bool isNavigating;

    // Rover
    private GameObject roverPrefab;
    private Marker rover;

    List<Vector2> InitialRoverMarkerLocs = new List<Vector2>
    {
        new Vector2(29.5648150f, -95.0817410f),
        new Vector2(29.5646824f, -95.0811564f),
        new Vector2(29.5650460f, -95.0810944f),
        new Vector2(29.5645430f, -95.0516440f),
        new Vector2(29.5648290f, -95.0813750f),
        new Vector2(29.5647012f, -95.0813750f),
        new Vector2(29.5651359f,-95.0807408f),
        new Vector2(29.5651465f, -95.0814092f),
        new Vector2(29.5648850f, -95.0808360f),
    };

    void Awake()
    {
        mainCamera = Camera.main;
        gps = GameObject.Find("GPS").GetComponent<GPS>();
        // tssAgent = GameObject.Find("TSSAgent").GetComponent<TSSAgent>();
        // notificationController = GameObject.Find("Notifications").GetComponent<NotificationController>();
        coord = GameObject.Find("Coordinate");
        coordText = coord.GetComponent<TMPro.TMP_Text>();
        mapRT = GameObject.Find("Map").GetComponent<RectTransform>();
        // compassRT = GameObject.Find("Compass Image").GetComponent<RectTransform>();
        // compassMarkersRT = GameObject.Find("Compass Markers").GetComponent<RectTransform>();
        markersTf = transform;
        currLocRT = GameObject.Find("CurrLoc").GetComponent<RectTransform>();
        // compassWidth = compassRT.rect.width / 360.0f;
        actionButtons = GameObject.Find("Marker Action Buttons");
        roverButtons = GameObject.Find("Rover Action Buttons");
        navigation = GameObject.Find("Navigation").GetComponent<Navigation>();
        markerImages = new Dictionary<MarkerType, GameObject>
        {
            { MarkerType.POI, GameObject.Find("POI Marker Image") },
            { MarkerType.Obstacle, GameObject.Find("Obstacle Marker Image") },
            { MarkerType.RoverMarker, GameObject.Find("Rover Marker Image") }
        };
        glowingMarkerImages = new Dictionary<MarkerType, GameObject>
        {
            { MarkerType.POI, GameObject.Find("Glowing POI Marker Image") },
            { MarkerType.Obstacle, GameObject.Find("Glowing Obstacle Marker Image") },
            { MarkerType.RoverMarker, GameObject.Find("Glowing Rover Marker Image") }
        };
        prefabDict = new Dictionary<MarkerType, GameObject>
        {
            { MarkerType.POI,  Resources.Load<GameObject>("CustomPrefabs/POI Marker") },
            { MarkerType.Rover, Resources.Load<GameObject>("CustomPrefabs/Rover") },
            { MarkerType.Obstacle, Resources.Load<GameObject>("CustomPrefabs/Obstacle Marker") },
            { MarkerType.RoverMarker, Resources.Load<GameObject>("CustomPrefabs/Rover Marker") },
        };
    }

    void Start()
    {
        // Initialize marker-related fields and states
        markers = new Dictionary<GameObject, Marker>();
        showMarker = new Dictionary<MarkerType, bool>
        {
            { MarkerType.POI, true },
            { MarkerType.Obstacle, true },
            { MarkerType.Rover, false },
            { MarkerType.RoverMarker, false },
        };

        actionButtons.SetActive(false);
        roverButtons.SetActive(false);
        foreach (var kvp in glowingMarkerImages)
        {
            kvp.Value.SetActive(false);
        };
        coord.SetActive(false);

        // Initialize rover
        Vector2 roverGpsCoord = new Vector2(GPS.SatCenterLatitude, GPS.SatCenterLongitude);
        rover = new Marker(MarkerType.Rover, roverGpsCoord);
        markers.Add(rover.MapMarkerObj, rover);

        // Populate the map with initial rover markers
        foreach (Vector2 gpsCoord in InitialRoverMarkerLocs)
        {
            Marker curr = new Marker(MarkerType.RoverMarker, gpsCoord);
            curr.SetOpacity(0.3f);
            markers.Add(curr.MapMarkerObj, curr);
        }

        // tssAgent = GameObject.Find("TSSAgent").GetComponent<TSSAgent>();
    }

    public void SetRoverLocation(Vector2 loc)
    {
        showMarker[MarkerType.Rover] = true;
        rover.GpsCoord = loc;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMarkers();
    }

    private void UpdateMarkers()
    {
        Vector2 userGps = gps.GetGpsCoords();
        Vector3 userLook = mainCamera.transform.forward;
        userLook.y = 0.0f;

        /************* Current Location ************/
        // Rotate
        float lookAngleZDeg = Vector3.Angle(Vector3.forward, userLook) * Mathf.Sign(userLook.x);
        float mapRotZDeg = mapRT.localEulerAngles.z;
        currLocRT.localRotation = Quaternion.Euler(0, 0, mapRotZDeg - lookAngleZDeg);

        // Translate
        currLocRT.offsetMin = mapRT.offsetMin + gps.GpsToMapPos(userGps.x, userGps.y);
        currLocRT.offsetMax = currLocRT.offsetMin;
        /*********************************/

        foreach (var kvp in markers)
        {
            MarkerType type = kvp.Value.Type;

            if (showMarker[type]) kvp.Value.Show();
            else
            {
                kvp.Value.Hide();
                continue;
            }

            kvp.Value.Update(userGps, userLook);
        }
    }

    public void HandleMarker(Vector2 touchCoord)
    {
        switch (mode)
        {
            case MarkerActionMode.Add:
                AddMarker(touchCoord);
                mode = MarkerActionMode.Edit;
                break;
            case MarkerActionMode.Edit:
                currMarker.GpsCoord = gps.MapPosToGps(touchCoord);
                break;
            case MarkerActionMode.Select:
                Vector3 pos = currMarker.MapMarkerObj.transform.position;
                pos.z -= 0.015f;
                pos.y -= 0.015f;
                if (currMarker.Type == MarkerType.RoverMarker) roverButtons.transform.position = pos;
                else actionButtons.transform.position = pos;
                var myCoord = currMarker.GpsCoord;
                coordText.text = string.Format("({0:0.0000000}, {1:0.0000000})", myCoord.x, myCoord.y);
                coord.SetActive(true);
                break;
        }
    }

    public void HideActionButtons()
    {
        if (currMarker != null && currMarker.Type == MarkerType.RoverMarker) currMarker.SetOpacity(0.3f);
        currMarker = null;
        actionButtons.SetActive(false);
        roverButtons.SetActive(false);
        coord.SetActive(false);
        mode = MarkerActionMode.None;
    }

    private void AddMarker(Vector2 touchCoord)
    {
        currMarker = new Marker(selectedMarkerType, gps.MapPosToGps(touchCoord));
        currMarker.SetOpacity(0.5f);
        markers.Add(currMarker.MapMarkerObj, currMarker);
        markerImages[selectedMarkerType].SetActive(true);
        glowingMarkerImages[selectedMarkerType].SetActive(false);
    }

    public void OnMapEnter(Vector2 touchCoord)
    {
        if (mode != MarkerActionMode.None) return;

        float minDist = markerEditSensitivity + 1;
        Vector2 touchGps = gps.MapPosToGps(touchCoord);

        foreach (var kvp in markers)
        {
            float dist = (kvp.Value.GpsCoord - touchGps).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                currMarker = kvp.Value;
            }
        }

        if (minDist < markerEditSensitivity)
        {
            if (currMarker.Type == MarkerType.RoverMarker)
            {
                currMarker.SetOpacity(1f);
                roverButtons.SetActive(true);
            }
            else actionButtons.SetActive(true);
            mode = MarkerActionMode.Select;
        }
        else
        {
            currMarker = null;
            mode = MarkerActionMode.None;
        }
    }

    public void OnMapExit()
    {
        if (mode == MarkerActionMode.Edit)
        {
            if (currMarker.MapMarkerObj != null) currMarker.SetOpacity(1f);
            mode = MarkerActionMode.None;
        }
    }

    // Button callbacks
    public void OnObstaclePressed()
    {
        SelectNewMarkerType(MarkerType.Obstacle);
    }
    public void OnPoiPressed()
    {
        SelectNewMarkerType(MarkerType.POI);
    }
    public void OnRoverMarkerPressed()
    {
        SelectNewMarkerType(MarkerType.RoverMarker);
    }

    public void OnMarkerMovePressed()
    {
        currMarker.SetOpacity(0.5f);
        actionButtons.SetActive(false);
        mode = MarkerActionMode.Edit;
    }

    public void OnMarkerDeletePressed()
    {
        if (navigation.destMarkerRT == currMarker.MapMarkerRT)
        {
            navigation.StopMarkerNavigate();
        }
        markers.Remove(currMarker.MapMarkerObj);
        currMarker.CleanUp();
        currMarker = null;

        actionButtons.SetActive(false);
        mode = MarkerActionMode.None;
    }

    public void OnMarkerNavigatePressed()
    {
        if (isNavigating && navigatingTo != currMarker)
        {
            navigation.StopMarkerNavigate();
            navigation.StartMarkerNavigate(currMarker.MapMarkerRT);
        }
        else
        {
            isNavigating = !isNavigating;
            if (isNavigating) navigation.StartMarkerNavigate(currMarker.MapMarkerRT);
            else navigation.StopMarkerNavigate();
        }

        actionButtons.SetActive(false);
        mode = MarkerActionMode.None;
    }

    public void OnRoverMarkerConfirmPressed()
    {
        // notificationController.PushSystemMessage("Moved rover to the selected POI", 3);
        // tssAgent.SendRoverMoveCommand(currMarker.GpsCoord);
        roverButtons.SetActive(false);
    }

    public void OnRoverMarkerCancelPressed()
    {
        roverButtons.SetActive(false);
        currMarker.SetOpacity(0.3f);
        coord.SetActive(false);
    }

    public void OnRoverRecallPressed()
    {
        // notificationController.PushSystemMessage("Recall rover to user's location", 3);
        // tssAgent.SendRoverRecallCommand();
    }

    private void SelectNewMarkerType(MarkerType type)
    {
        // Toggling the same marker type
        if (selectedMarkerType == type)
        {
            if (type == MarkerType.RoverMarker)
            {
                if (markerImages[type].activeSelf)
                {
                    // Start rover command
                    showMarker[type] = true;
                }
                else
                {
                    // Cancel rover command
                    showMarker[type] = false;
                }
            }
            else
            {
                mode = markerImages[type].activeSelf ? MarkerActionMode.Add : MarkerActionMode.None;
            }
            markerImages[type].SetActive(!markerImages[type].activeSelf);
            glowingMarkerImages[type].SetActive(!glowingMarkerImages[type].activeSelf);
        }
        else
        {
            // State reset if another marker is selected
            if (glowingMarkerImages[selectedMarkerType].activeSelf)
            {
                if (selectedMarkerType == MarkerType.RoverMarker)
                {
                    // Hide all rover markers again
                }
                markerImages[selectedMarkerType].SetActive(true);
                glowingMarkerImages[selectedMarkerType].SetActive(false);
            }

            // Set the current selection
            switch (type)
            {
                case MarkerType.POI:
                case MarkerType.Obstacle:
                    mode = MarkerActionMode.Add;
                    break;
                case MarkerType.RoverMarker:
                    showMarker[type] = true;
                    break;
            }
            markerImages[type].SetActive(false);
            glowingMarkerImages[type].SetActive(true);
            selectedMarkerType = type;
        }
    }
}
