using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using MixedReality.Toolkit.Input;
using RectTransform = UnityEngine.RectTransform;
using UnityEngine.UI;
using UnityEditor.PackageManager;

namespace MixedReality.Toolkit.Suits.Map
{
    public class MapController : MRTKBaseInteractable
    {
        public DateTime? StartTimestamp { get; private set; }

        enum MapFocusMode
        {
            MapNoFocus,
            MapCenterUser,
            MapAlignUser,
            NumMapFocusModes,
        };

        private RectTransform mapRT;
        private Camera mainCamera;
        private static GPS gps;

        // Zoom
        private readonly List<float> zoomSeries = new List<float> { 1, 2, 5, 10 };
        private int zoomIndex = 1;

        // Pan
        private Dictionary<IXRInteractor, Vector2> lastPositions = new Dictionary<IXRInteractor, Vector2>();
        private Vector2 firstPosition;
        private Vector2 initialOffsetMin;

        // Focus
        private MapFocusMode focusMode = MapFocusMode.MapNoFocus;
        private Vector2 lastTouchPosition;

        // Each marker is a (type, gpsCoords, mapMarkerObj, compassMarkerObj, mapRT, compassRT) 5-tuple
        // private Dictionary<GameObject, (MarkerType, Vector2, GameObject, GameObject, RectTransform, RectTransform)> markers;
        private MarkerController markerController;
        private MarkerType selectedMarkerType;

        private GameObject markerImageNote;
        private GameObject markerVoiceNote;

        private bool showDetailPage = false;
        private RectTransform mapPanelRT;
        private GameObject actionButtons;
        private GameObject mapButtons;
        private RectTransform currLocRT;
        private GameObject MapBackButton;
        private GameObject mapDetails;
        private GameObject mapTitle;
        private GameObject ZoomInOut;

        /* Coordinates overlay */
        private RectTransform xCoordsTextsRT;
        private RectTransform yCoordsTextsRT;
        private HorizontalLayoutGroup xCoordsHGroup;
        private VerticalLayoutGroup yCoordsVGroup;

        private RectTransform xCoordsRT;

        private static float mapPanelWidth = 324.0f;
        private static float detailMapPanelWidth = 200.0f;

        void Start()
        {
            mainCamera = Camera.main;
            mapRT = GameObject.Find("Map").GetComponent<RectTransform>();
            markerController = GameObject.Find("Markers").GetComponent<MarkerController>();
            mapRT.localScale = GetLocalScale(zoomSeries[zoomIndex]);
            gps = GameObject.Find("GPS").GetComponent<GPS>();
            StartTimestamp = DateTime.Now;

            mapPanelRT = GameObject.Find("Map Panel").GetComponent<RectTransform>();
            mapPanelRT.sizeDelta = new Vector2(mapPanelWidth, mapPanelWidth);

            actionButtons = GameObject.Find("Marker Action Buttons");
            mapButtons = GameObject.Find("Map Buttons");
            mapDetails = GameObject.Find("Map Details");
            currLocRT = GameObject.Find("CurrLoc").GetComponent<RectTransform>();   
            MapBackButton = GameObject.Find("Map Back Button");
            MapBackButton.SetActive(false);
            mapDetails.SetActive(false);
            mapTitle = GameObject.Find("Map Title");
            ZoomInOut = GameObject.Find("ZoomInOut");

            markerImageNote = GameObject.Find("Marker Image Note");
            markerVoiceNote = GameObject.Find("Marker Voice Note");
            markerImageNote.SetActive(false);
            markerVoiceNote.SetActive(false);

            var xCoordsTexts = GameObject.Find("X Coordinates/Texts");
            var yCoordsTexts = GameObject.Find("Y Coordinates/Texts");
            xCoordsTextsRT = xCoordsTexts.GetComponent<RectTransform>();
            yCoordsTextsRT = yCoordsTexts.GetComponent<RectTransform>();
            xCoordsHGroup = xCoordsTexts.GetComponent<HorizontalLayoutGroup>();
            yCoordsVGroup = yCoordsTexts.GetComponent<VerticalLayoutGroup>();

            xCoordsRT = GameObject.Find("X Coordinates").GetComponent<RectTransform>();
        }

        void Update()
        {
            switch (focusMode)
            {
                case MapFocusMode.MapCenterUser:
                    CenterMapAtUser();
                    break;
                case MapFocusMode.MapAlignUser:
                    AlignMapWithUser();
                    break;
            }

            // Scale and translate the coordinates
            var spacing = ZoomLevel * BlockSize;
            xCoordsHGroup.spacing = spacing;
            yCoordsVGroup.spacing = spacing;

            var xCoordsLocalPos = xCoordsTextsRT.localPosition;
            xCoordsLocalPos.x = mapRT.offsetMin.x;
            xCoordsTextsRT.localPosition = xCoordsLocalPos;

            var yCoordsLocalPos = yCoordsTextsRT.localPosition;
            yCoordsLocalPos.y = mapRT.offsetMin.y;
            yCoordsTextsRT.localPosition = yCoordsLocalPos;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;

            foreach (var interactor in interactorsSelecting)
            {
                if (interactor is PokeInteractor)
                {
                    // attachTransform will be the actual point of the touch interaction (e.g. index tip)
                    Vector2 localTouchPosition = transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

                    // Have we seen this interactor before? If not, last position = current position
                    if (!lastPositions.TryGetValue(interactor, out Vector2 lastPosition))
                    {
                        // Pan
                        firstPosition = localTouchPosition;
                        initialOffsetMin = mapRT.offsetMin;

                        // Focus
                        focusMode = MapFocusMode.MapNoFocus;

                        // Marker
                        if (!mapDetails.activeSelf) markerController.OnMapEnter(localTouchPosition);
                    }

                    if (markerController.mode == MarkerActionMode.None)
                    {
                        // Update the offsets (top, right, bottom, left) based on the change in position
                        Vector2 delta = localTouchPosition - firstPosition;
                        mapRT.offsetMin = initialOffsetMin + delta;
                        mapRT.offsetMax = mapRT.offsetMin;
                    }
                    else
                    {
                        // Do something with the marker
                        markerController.HandleMarker(localTouchPosition);
                    }

                    // Write/update the last-position
                    if (lastPositions.ContainsKey(interactor))
                    {
                        lastPositions[interactor] = localTouchPosition;
                    }
                    else
                    {
                        lastPositions.Add(interactor, localTouchPosition);
                    }

                    break;
                }
            }

        }

        public float BlockSize
        {
            get { return mapPanelRT.sizeDelta.x / 25; }
        }

        /************* Scale ***************/

        private static Vector3 GetLocalScale(float scale)
        {
            return new Vector3(scale, scale, 1.0f);
        }

        public void MapZoomInCallback()
        {
            zoomIndex = zoomIndex >= zoomSeries.Count - 1 ? zoomIndex : zoomIndex + 1;
            mapRT.localScale = GetLocalScale(zoomSeries[zoomIndex]);
        }

        public void MapZoomOutCallback()
        {
            zoomIndex = zoomIndex <= 0 ? zoomIndex : zoomIndex - 1;
            mapRT.localScale = GetLocalScale(zoomSeries[zoomIndex]);
        }

        public float ZoomLevel { get { return mapRT.localScale.x; } }

        /************* Map Focus **************/
        public void MapFocusCallback()
        {
            MapToggleFocusMode();
            switch (focusMode)
            {
                case MapFocusMode.MapNoFocus:
                    MapRestoreLastRotation();
                    CenterMapAtUser();
                    break;
                case MapFocusMode.MapAlignUser:
                    MapRestoreLastRotation();
                    break;
            }
        }
        private void RotateMapWithUser()
        {
            Vector3 userLook = mainCamera.transform.forward;

            // Rotate map so that currLoc points up
            userLook.y = 0.0f;
            float lookAngleZDeg = Vector3.Angle(Vector3.forward, userLook) * Mathf.Sign(userLook.x);
            mapRT.localRotation = Quaternion.Euler(0.0f, 0.0f, lookAngleZDeg);
        }

        private void CenterMapAtUser()
        {
            Vector2 gpsCoords = gps.GetGpsCoords();
            Vector2 userPosMap = gps.GpsToMapPos(gpsCoords.x, gpsCoords.y);
            mapRT.offsetMin = -userPosMap;
            mapRT.offsetMax = mapRT.offsetMin;
        }

        private void AlignMapWithUser()
        {
            RotateMapWithUser();
            CenterMapAtUser();
        }

        private void MapToggleFocusMode()
        {
            int newMode = ((int)focusMode + 1) % (int)MapFocusMode.NumMapFocusModes;
            focusMode = (MapFocusMode)newMode;
        }

        private void MapRestoreLastRotation()
        {
            mapRT.localRotation = Quaternion.Euler(0, 0, 0);
        }

        /************* Interaction Event ***************/
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (markerController.mode == MarkerActionMode.Select)
            {
                markerController.HideActionButtons();
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            markerController.OnMapExit();

            // Remove the interactor from our last-position collection when it leaves.
            lastPositions.Remove(args.interactorObject);
        }

        /************* Time ***************/
        public void RecordStartTime()
        {
            StartTimestamp = DateTime.Now;
        }

        public void OpenDetailPage()
        {
            MapBackButton.SetActive(true);
            actionButtons.SetActive(false);
            mapPanelRT.sizeDelta = new Vector2(detailMapPanelWidth, mapPanelWidth); // change the shape of the map we see
            mapButtons.SetActive(false); // hide the vertial map buttons
            mapTitle.SetActive(false);
            ZoomInOut.SetActive(false);
            // Debug.Log("Open Detail Page");
            mapDetails.SetActive(true); // show the detail page
            currLocRT.localScale = new Vector3(0.3f, 0.185f, 1); // for the current location (green) icon
            showDetailPage = !showDetailPage;

            // Adjust the xCoords
            var newSize = xCoordsRT.sizeDelta;
            newSize.x = detailMapPanelWidth;
            xCoordsRT.sizeDelta = newSize;

            var newPos = xCoordsRT.anchoredPosition;
            newPos.x = (detailMapPanelWidth - mapPanelWidth) * 0.5f;
            xCoordsRT.anchoredPosition = newPos;
        }

        public void closeDetailPage()
        {
            MapBackButton.SetActive(false);
            actionButtons.SetActive(false);
            mapDetails.SetActive(false);
            mapPanelRT.sizeDelta = new Vector2(mapPanelWidth, mapPanelWidth);
            mapButtons.SetActive(true);
            mapTitle.SetActive(true);
            ZoomInOut.SetActive(true);
            // Debug.Log("Close Detail Page");
            currLocRT.localScale = new Vector3(0.2f, 0.2f, 1);
            showDetailPage = !showDetailPage;

            // Adjust the xCoords
            var newSize = xCoordsRT.sizeDelta;
            newSize.x = mapPanelWidth;
            xCoordsRT.sizeDelta = newSize;

            var newPos = xCoordsRT.anchoredPosition;
            newPos.x = 0.0f;
            xCoordsRT.anchoredPosition = newPos;
        }
    }


}
