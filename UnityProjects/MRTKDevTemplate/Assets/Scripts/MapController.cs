using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using MixedReality.Toolkit.Input;
using RectTransform = UnityEngine.RectTransform;

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

        private bool showDetailPage = false;
        private RectTransform mapPanelRT;
        private GameObject actionButtons;
        private GameObject mapButtons;
        private RectTransform currLocRT;
        private GameObject MapBackButton;
        private GameObject mapDetails;

        void Start()
        {
            mainCamera = Camera.main;
            mapRT = GameObject.Find("Map").GetComponent<RectTransform>();
            markerController = GameObject.Find("Markers").GetComponent<MarkerController>();
            mapRT.localScale = GetLocalScale(zoomSeries[zoomIndex]);
            gps = GameObject.Find("GPS").GetComponent<GPS>();
            StartTimestamp = DateTime.Now;

            mapPanelRT = GameObject.Find("Map Panel").GetComponent<RectTransform>();
            actionButtons = GameObject.Find("Marker Action Buttons");
            mapButtons = GameObject.Find("Map Buttons");
            mapDetails = GameObject.Find("Map Details");
            currLocRT = GameObject.Find("CurrLoc").GetComponent<RectTransform>();   
            MapBackButton = GameObject.Find("Map Back Button");
            MapBackButton.SetActive(false);
            mapDetails.SetActive(false);
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
                        markerController.OnMapEnter(localTouchPosition);
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
            mapPanelRT.sizeDelta = new Vector2(200, 350); // change the shape of the map we see
            mapButtons.SetActive(false); // hide the vertial map buttons
            Debug.Log("Open Detail Page");
            mapDetails.SetActive(true); // show the detail page
            currLocRT.localScale = new Vector3(0.14f, 0.08f, 1); // for the current location (green) icon
            showDetailPage = !showDetailPage;
        }
        public void closeDetailPage()
        {
            MapBackButton.SetActive(false);
            actionButtons.SetActive(false);
            mapDetails.SetActive(false);
            mapPanelRT.sizeDelta = new Vector2(350, 350);
            mapButtons.SetActive(true);
            Debug.Log("Close Detail Page");
            currLocRT.localScale = new Vector3(0.08f, 0.08f, 1);
            showDetailPage = !showDetailPage;
        }
    }


}