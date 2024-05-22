// using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;

namespace MixedReality.Toolkit.Suits.Map
{
public enum MarkerType
{
    POI,
    Rover,
    Obstacle,
    Stations,
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
    public class Marker
    {
        public readonly MarkerType Type;
        public Vector2 GpsCoord;
        public readonly GameObject MapMarkerObj;
        public readonly RectTransform MapMarkerRT;
        public readonly int ID;
        public readonly string Name;
        public Marker(MarkerType type, GameObject prefab, Vector2 gpsCoord, string name)
        {
            Type = type;
            GpsCoord = gpsCoord;
            MapMarkerObj = Instantiate(prefab, markersTf);
            MapMarkerRT = MapMarkerObj.GetComponent<RectTransform>();
            ID = nextID++;
            Name = name;
        }

        public void CleanUp()
        {
            Destroy(MapMarkerObj);
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
        }

        public void Hide()
        {
            MapMarkerObj.SetActive(false);
        }

        public void Show()
        {
            MapMarkerObj.SetActive(true);
        }

        public bool IsHidden()
        {
            return !MapMarkerObj.activeSelf;
        }
    }

    private float markerEditSensitivity = 2f;

    private static RectTransform mapRT;
    private static Transform markersTf;
    private static float compassWidth;

    private Camera mainCamera;
    private static GPS gps;

    private static int nextID = 0;
    private static Dictionary<MarkerType, GameObject> prefabDict;
    private Dictionary<MarkerType, bool> showMarker;
    private Dictionary<GameObject, Marker> markers;
    [HideInInspector] public MarkerActionMode mode;
    private MarkerType selectedMarkerType;
    private Dictionary<MarkerType, GameObject> markerImages;
    private Dictionary<MarkerType, GameObject> glowingMarkerImages;
    private RectTransform currLocRT;
    [HideInInspector] public Marker currMarker;

    private GameObject actionButtons;

    private GameObject deleteConfirmation;

    private Navigation navigation;
    private Marker navigatingTo;
    private bool isNavigating;

    // Rover
    private GameObject roverPrefab;
    private Marker rover;

    private MapController mapController;

    private readonly Dictionary<string, Tuple<float, float>> stations = new Dictionary<string, Tuple<float, float>>
    {
        // For TEST on four corners + middle center
        // => width: 3272383 - 3272330 = 53	BUT, 13 blocks  ==> 4.07 UTM per block width  ==> 1andhalf left
        // => height: 298355 - 298305 = 50	BUT, 13 blocks  ==> 3.846 UTM per block height  ==> half down
        // (height, width)
        // { "B", new Tuple<float, float>(298353.077f + 93.79f / 2f, 3272376.895f - 96.154f / 2f) }, // top left
        // { "E", new Tuple<float, float>(298353.077f + 93.79f / 2f, 3272376.895f + 96.154f / 2f) }, // top right
        // // { "C", new Tuple<float, float>(298353.077f, 3272376.895f) }, // OUR calculated middle
        // { "D", new Tuple<float, float>(298353.077f - 93.79f / 2f, 3272376.895f - 96.154f / 2f) }, // Bottom Left
        // { "A", new Tuple<float, float>(298353.077f - 93.79f / 2f, 3272376.895f + 96.154f / 2f) }, //  Bottom Right

        { "A", new Tuple<float, float>(298353.077f-(3.846f * 6.5f), 3272376.895f-(4.07f * 7.5f)) }, // Point A
        { "B", new Tuple<float, float>(298353.077f-(3.846f * 4.5f), 3272376.895f-(4.07f * 3.5f)) }, // Point B
        { "C", new Tuple<float, float>(298353.077f-(3.846f * 2.5f), 3272376.895f-(4.07f * 5.5f)) }, // Point C
        { "D", new Tuple<float, float>(298353.077f-(3.846f * 0.5f), 3272376.895f-(4.07f * 3.5f)) }, // Point D
        { "E", new Tuple<float, float>(298353.077f-(3.846f * 1.5f), 3272376.895f+(4.07f * 1.5f)) }, // Point E
        { "F", new Tuple<float, float>(298353.077f+(3.846f * 2.5f), 3272376.895f+(4.07f * 6.5f)) }, // Point F
        { "G", new Tuple<float, float>(298353.077f+(3.846f * 2.5f), 3272376.895f+(4.07f * 1.5f)) }, // Point G
        { "UIA", new Tuple<float, float>(298353.077f-(3.846f * 7.5f), 3272376.895f+(4.07f * 3f)) }, // Point UIA
        { "COMM", new Tuple<float, float>(298353.077f+(3.846f * 0.5f), 3272376.895f+(4.07f * 7.5f)) }, // Point COMM

    };
        

    void Start()
    {
        mainCamera = Camera.main;
        gps = GameObject.Find("GPS").GetComponent<GPS>();
        mapRT = GameObject.Find("Map").GetComponent<RectTransform>();
        mapController = GameObject.Find("Map Panel").GetComponent<MapController>();
        markersTf = transform;
        currLocRT = GameObject.Find("CurrLoc").GetComponent<RectTransform>();
        actionButtons = GameObject.Find("Marker Action Buttons");
        deleteConfirmation = GameObject.Find("Delete Confirmation");
        navigation = GameObject.Find("Navigation").GetComponent<Navigation>();
        markerImages = new Dictionary<MarkerType, GameObject>
        {
            { MarkerType.POI, GameObject.Find("POI Marker Image") },
            { MarkerType.Obstacle, GameObject.Find("Obstacle Marker Image") },
        };
        glowingMarkerImages = new Dictionary<MarkerType, GameObject>
        {
            { MarkerType.POI, GameObject.Find("Glowing POI Marker Image") },
            { MarkerType.Obstacle, GameObject.Find("Glowing Obstacle Marker Image") },
        };
        prefabDict = new Dictionary<MarkerType, GameObject>
        {
            { MarkerType.POI,  Resources.Load<GameObject>("CustomPrefabs/POI Marker") },
            { MarkerType.Rover, Resources.Load<GameObject>("CustomPrefabs/Rover") },
            // { MarkerType.Rover, Resources.Load<Ga/meObject>("Prefabs/red_dot") },
            { MarkerType.Obstacle, Resources.Load<GameObject>("CustomPrefabs/Obstacle Marker") },
        };

        // Initialize marker-related fields and states
        markers = new Dictionary<GameObject, Marker>();
        glowingMarkerImages[selectedMarkerType].SetActive(false);
        showMarker = new Dictionary<MarkerType, bool>
        {
            { MarkerType.POI, true },
            { MarkerType.Obstacle, true },
            { MarkerType.Rover, false },
            { MarkerType.Stations, true }
        };

        actionButtons.SetActive(false);
        deleteConfirmation.SetActive(false);
        foreach (var kvp in glowingMarkerImages)
        {
            kvp.Value.SetActive(false);
        };

        // Initialize default stations
        foreach (var kvp in stations)
        {
            Vector2 utmCoord = new Vector2(kvp.Value.Item1, kvp.Value.Item2);
            string path = "Prefabs/Markers/" + kvp.Key;
            // var stationMarker = new Marker(MarkerType.Stations, Resources.Load<GameObject>(path), utmCoord, "Station " + kvp.Key);
            string name;
            switch (kvp.Key)
            {
                case "UIA":                   
                    name = "UIA Panel";
                    break;
                case "COMM":
                    name = "Comm Tower";
                    break;
                default:
                    name = "Geo Station " + kvp.Key;
                    break;
            }
            var stationMarker = new Marker(MarkerType.Stations, Resources.Load<GameObject>(path), utmCoord, name);
            markers.Add(stationMarker.MapMarkerObj, stationMarker);
        }

        // Initialize rover
        Vector2 roverGpsCoord = new Vector2(GPS.SatCenterLatitude, GPS.SatCenterLongitude);
        rover = new Marker(MarkerType.Rover, prefabDict[MarkerType.Rover], roverGpsCoord, "Rover");
        markers.Add(rover.MapMarkerObj, rover);
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

            // if (showMarker[type]) kvp.Value.Show();
            // else
            // {
            //     kvp.Value.Hide();
            //     continue;
            // }

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
                pos.y -= (-0.055f);
                actionButtons.transform.position = pos;
                var myCoord = currMarker.GpsCoord;
                break;
        }
    }

    public void HideActionButtons()
    {
        actionButtons.SetActive(false);
        mode = MarkerActionMode.None;
    }

    private void AddMarker(Vector2 touchCoord)
    {
        
        // currMarker = new Marker(selectedMarkerType, prefabDict[selectedMarkerType], gps.MapPosToGps(touchCoord), "POI");
        switch(selectedMarkerType)
        {
            case MarkerType.POI:
                currMarker = new Marker(selectedMarkerType, prefabDict[selectedMarkerType], gps.MapPosToGps(touchCoord), "POI");
                break;
            case MarkerType.Obstacle:
                currMarker = new Marker(selectedMarkerType, prefabDict[selectedMarkerType], gps.MapPosToGps(touchCoord), "Obstacle");
                break;
        }


        currMarker.SetOpacity(0.5f);
        markers.Add(currMarker.MapMarkerObj, currMarker);
        markerImages[selectedMarkerType].SetActive(true);
        glowingMarkerImages[selectedMarkerType].SetActive(false);
    }

    public void OnMapEnter(Vector2 touchCoord)
    {
        if (mode != MarkerActionMode.None || deleteConfirmation.activeSelf) return;

        float minDist = markerEditSensitivity;
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

        Debug.Log(minDist);
        if (minDist < markerEditSensitivity)
        {
            actionButtons.SetActive(true);
            mode = MarkerActionMode.Select;

            if (currMarker.Type == MarkerType.Stations)
            {
                actionButtons.transform.Find("Delete Button").gameObject.SetActive(false);
            }
            else
            {
                actionButtons.transform.Find("Delete Button").gameObject.SetActive(true);
            }
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

        mapController.closeDetailPage();
    
        // deleteConfirmation.SetActive(false);
        // actionButtons.SetActive(false);
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

    private void SelectNewMarkerType(MarkerType type)
    {
        // Toggling the same marker type
        if (selectedMarkerType == type)
        {
            mode = markerImages[type].activeSelf ? MarkerActionMode.Add : MarkerActionMode.None;
            markerImages[type].SetActive(!markerImages[type].activeSelf);
            glowingMarkerImages[type].SetActive(!glowingMarkerImages[type].activeSelf);
        }
        else
        {
            // State reset if another marker is selected
            if (glowingMarkerImages[selectedMarkerType].activeSelf)
            {
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
                default:
                    mode = MarkerActionMode.None;
                    break;
            }

            markerImages[type].SetActive(false);
            glowingMarkerImages[type].SetActive(true);
            selectedMarkerType = type;
        }
    }
}
}