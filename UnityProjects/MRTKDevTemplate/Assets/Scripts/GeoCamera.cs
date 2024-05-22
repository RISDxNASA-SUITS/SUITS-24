using UnityEngine;
using MixedReality.Toolkit.Suits.Map;

public class GeoCamera : SimpleCamera
{
    [SerializeField]
    private MapController _mapController;

    [SerializeField]
    private GeoSampleController _geoSampleController;

/*    void Start()
    {
        _mapController = GameObject.Find("Map Panel").GetComponent<MapController>();
        _geoSampleController = GameObject.Find("Geo Canvas").GetComponent<GeoSampleController>();
    }
*/

    // Get metadata ready for a new GeoNote
    public override Note newImageNote()
    {
        var note = new GeoNote();
        note.note_type = "image";

        var stationName = _mapController.ClosestGeoStationName();
        note.station_id = stationName[stationName.Length - 1].ToString();
        note.rock = new RockDataWrapper();
        note.rock.elements = _geoSampleController.GetRockData();

        return note;
    }
}
