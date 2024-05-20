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

public record GeoSampleNote : Note
{
    public string site_name; // {"A", ..., "F"}
}
