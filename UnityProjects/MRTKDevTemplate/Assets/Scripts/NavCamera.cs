using UnityEngine;
using MixedReality.Toolkit.Suits.Map;

public class NavCamera : SimpleCamera
{
    [SerializeField]
    private MarkerController _markerController;

    // Get metadata ready for a new NavNote
    public override Note newImageNote()
    {
        var note = new NavNote();
        note.note_type = "image";

        var currMarker = _markerController.currMarker;
        note.id = currMarker.ID;
        note.marker_type = currMarker.Type.ToString();

        return note;
    }
}
