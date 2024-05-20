using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public record NavNote
{
    public string marker_type; // {"poi", "hazard"}
    public string note_type;   // {"image", "voice"}
    public int id;
    public byte[] data;
    public string file_ext;
}

public record GeoSampleNote
{
    public string site_name; // {"A", ..., "F"}
    public string note_type; // {"image", "voice"}
    public int id;
    public byte[] data;
    public string file_ext;
}
