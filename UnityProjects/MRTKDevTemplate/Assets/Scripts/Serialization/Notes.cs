using MixedReality.Toolkit.Suits.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public record Note
{
    public string note_type;   // {"image", "voice"}
    public int id;
    public byte[] data;
    public string file_ext;
}

public record NavNote : Note
{
    public string marker_type; // nameof(Marker.MarkerType): {POI, Rover, Obstacle, Stations}
}

public record GeoNote : Note
{
    public string station_id; // {"A", ..., "F"}
    public RockDataWrapper rock;
}

/*public record GeoSendNote : Note
{
    public string station_id; // {"A", ..., "F"}
    public RockData rock;
}
*/
