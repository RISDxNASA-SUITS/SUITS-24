using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPS : MonoBehaviour
{
    /************* Satellite **************/
    // hard coded center
    public const float SatCenterLatitude = 298353.077f;   // latitude at the center of the satellite image, in degree
    public const float SatCenterLongitude = 3272376.895f;  // longitude at the center of the satellite image, in degree
    // hard coded scale
    const float SatLatitudeRange = 93.79f;  // the satellite image covers this much latitudes in degree
    const float SatLongitudeRange = 96.154f;  // EDIT should be 116, but doesn't work if changed. Therefore tweaked. the satellite image covers this much longitudes in degree

    /************* GameObject References **************/
    private Camera mainCamera;
    private RectTransform mapRT, canvasRT;
    private Vector2 userGps = new Vector2(SatCenterLatitude, SatCenterLongitude);
    private TSScConnection tssConn;

    void Start()
    {
        mainCamera = Camera.main;
        mapRT = GameObject.Find("Map").GetComponent<RectTransform>();
        canvasRT = GameObject.Find("Nav Canvas").GetComponent<RectTransform>();
        tssConn = GameObject.Find("TSS Agent").GetComponent<TSScConnection>();
    }

    void Update()
    {
        if (tssConn.isIMUUpdated())
        {
            IMU imu = tssConn.GetIMU();
            userGps = new Vector2(imu.eva1.posx, imu.eva1.posy);
        }
    }

    // For simulation in Unity
    public Vector2 GetGpsCoords()
    {
#if true // Use simulated gps coords in Unity?
        Vector3 worldPos = mainCamera.transform.position;
        Vector2 gpsCoords = new Vector2(SatCenterLatitude, SatCenterLongitude);
        gpsCoords += 50 * new Vector2(worldPos.z, worldPos.x); // in mapnav, shift+w/a/s/d to move cur position
        return gpsCoords;
#else
        return userGps;
#endif
    }

    public Vector2 GpsToMapPos(float latitudeDeg, float longitudeDeg)
    {
        float du = (longitudeDeg - SatCenterLongitude) / SatLongitudeRange;  // -.5 ~ +.5 in horizontal map space
        float dv = (latitudeDeg - SatCenterLatitude) / SatLatitudeRange;     // -.5 ~ +.5 in vertical map sapce

        float mapRotZDeg = mapRT.localEulerAngles.z;
        Vector3 mapPos = mapRT.localScale.x * canvasRT.rect.height * new Vector3(du, 0, dv);
        mapPos = Quaternion.Euler(0.0f, -mapRotZDeg, 0.0f) * mapPos;

        return new Vector2(mapPos.x, mapPos.z);
    }

    // Actually: PanelPos to GPS
    public Vector2 MapPosToGps(Vector2 mapPos)
    {
        Vector2 mapOffset = mapRT.offsetMin;
        Vector3 worldPos = new Vector3(mapPos.x - mapOffset.x, 0, mapPos.y - mapOffset.y);

        // Un-rotate then scale to obtain the world space position
        float mapRotZDeg = mapRT.localEulerAngles.z;
        worldPos = Quaternion.Euler(0.0f, mapRotZDeg, 0.0f) * worldPos;

        worldPos /= (mapRT.localScale.x * canvasRT.rect.height);  // (du, 0, dv) in GPSToMapPos

        float longitudeDeg = worldPos.x * SatLongitudeRange + SatCenterLongitude;
        float latitudeDeg = worldPos.z * SatLatitudeRange + SatCenterLatitude;

        return new Vector2(latitudeDeg, longitudeDeg);
    }

    public Vector2 WorldToMapPos(Vector3 worldPos)
    {
        float scaleW2M = 1000.0f * mapRT.localScale.x;
        float mapRotZDeg = mapRT.localEulerAngles.z;

        // Rotate then scale to obtain the map space position
        Vector3 mapPos = Quaternion.Euler(0.0f, -mapRotZDeg, 0.0f) * worldPos;
        mapPos *= scaleW2M;

        return new Vector2(mapPos.x, mapPos.z);
    }

    private Vector3 MapToWorldPos(Vector2 mapPos)
    {
        Vector2 mapOffset = mapRT.offsetMin;
        Vector3 worldPos = new Vector3(mapPos.x - mapOffset.x, 0, mapPos.y - mapOffset.y);

        // Un-rotate then scale to obtain the world space position
        float mapRotZDeg = mapRT.localEulerAngles.z;
        worldPos = Quaternion.Euler(0.0f, mapRotZDeg, 0.0f) * worldPos;

        float scaleW2M = 1000.0f * mapRT.localScale.x;
        worldPos /= scaleW2M;

        return worldPos;
    }

    public void UpdateUserGps(Vector2 gps)
    {
        userGps = gps;
    }
}

