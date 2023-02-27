using System.Collections.Generic;
using Reshape.ReGraph;
using Reshape.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Reshape.Reframework
{
    [DisallowMultipleComponent]
    public class RayCastingController : BaseBehaviour
    {
        public struct RayInfo
        {
            public GraphRunner activator;
            public CastType type;
            public string rayName;
            public Camera cameraView;
            public float rayDistance;

            public RayInfo (CastType cast, string actionName, GraphRunner runner, Camera cam, float distance)
            {
                type = cast;
                rayName = actionName;
                activator = runner;
                cameraView = cam;
                rayDistance = distance;
            }

            public RayInfo (CastType cast, string actionName, GraphRunner runner)
            {
                type = cast;
                rayName = actionName;
                activator = runner;
                cameraView = null;
                rayDistance = 0;
            }
        }

        public enum CastType
        {
            None,
            CastFromCameraToWorld = 10,
            CastFromMouseToWorld = 11,
            CastFromMouseToUi = 51,
        }

        private List<RayInfo> rays;
        private GraphRunner caller;

        public void AddRay (CastType cast, string actionName, GraphRunner runner, Camera cam, float distance)
        {
            if (cast is CastType.CastFromCameraToWorld or CastType.CastFromMouseToWorld)
                if (string.IsNullOrEmpty(actionName) || cam == null || distance <= 0)
                    return;
            if (rays == null)
                rays = new List<RayInfo>();
            rays.Add(new RayInfo(cast, actionName, runner, cam, distance));
        }

        public void AddRay (CastType cast, string actionName, GraphRunner runner)
        {
            if (cast == CastType.CastFromMouseToUi && string.IsNullOrEmpty(actionName))
                return;
            if (rays == null)
                rays = new List<RayInfo>();
            rays.Add(new RayInfo(cast, actionName, runner));
        }

        public void RemoveRay (string actionName)
        {
            if (rays != null)
            {
                for (int i = 0; i < rays.Count; i++)
                {
                    if (rays[i].rayName == actionName)
                    {
                        rays.RemoveAt(i);
                        i--;
                    }
                }

                if (rays.Count == 0)
                    Terminate();
            }
        }

        public void Terminate ()
        {
            Destroy(this);
        }

        protected override void Start ()
        {
            if (rays == null || rays.Count == 0)
            {
                Terminate();
                return;
            }

            caller = GetComponent<GraphRunner>();
        }

        protected void Update ()
        {
            for (int i = 0; i < rays.Count; i++)
            {
                RayInfo ray = rays[i];
                if (ray.type == CastType.CastFromCameraToWorld)
                {
                    RaycastHit? hit = RayCastFromScreenPointToWorld(ray.rayName, ray.cameraView, ReInput.mousePositionAtCenterScreen, ray.rayDistance);
                    if (hit != null)
                    {
                        RaycastHit theHit = (RaycastHit) hit;
                        TriggerRayReceived(ray, theHit);
                    }
                    else
                    {
                        TriggerRayMissed(ray);
                    }
                }
                else if (ray.type == CastType.CastFromMouseToWorld)
                {
                    RaycastHit? hit = RayCastFromScreenPointToWorld(ray.rayName, ray.cameraView, ReInput.mousePosition, ray.rayDistance);
                    if (hit != null)
                    {
                        RaycastHit theHit = (RaycastHit) hit;
                        TriggerRayReceived(ray, theHit);
                    }
                    else
                    {
                        TriggerRayMissed(ray);
                    }
                }
                else if (ray.type == CastType.CastFromMouseToUi)
                {
                    bool somethingHit = false;
                    bool somethingReceived = false;
                    GameObject hitGo = null;
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    {
                        PointerEventData pointerData = new PointerEventData(EventSystem.current) {pointerId = -1};
                        pointerData.position = ReInput.mousePosition;
                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pointerData, results);
                        for (int j = 0; j < results.Count; j++)
                        {
                            if (j == 0)
                            {
                                if (results[j].gameObject.GetComponentInParent<Canvas>().renderMode != RenderMode.WorldSpace)
                                {
                                    hitGo = results[j].gameObject;
                                    somethingHit = true;
                                }
                            }
                            if (RayCastFromScreenPointToUi(ray.rayName, results[j].gameObject))
                            {
                                hitGo = results[j].gameObject;
                                somethingReceived = true;
                                break;
                            }
                        }
                    }

                    if (somethingReceived)
                    {
                        TriggerRayReceived(ray, hitGo);
                    }
                    else if (somethingHit)
                    {
                        TriggerRayHit(ray, hitGo);
                    }
                    else
                    {
                        TriggerRayMissed(ray);
                    }
                }
            }
        }

        public void TriggerRayReceived (RayInfo ray, RaycastHit hit)
        {
            if (ray.activator != null)
            {
                //ray.rayName receive
            }

            if (caller != null)
            {
                //ray.rayName receive
            }
        }
        
        public void TriggerRayHit (RayInfo ray, GameObject hit)
        {
            if (ray.activator != null)
            {
                //ray.rayName receive
            }

            if (caller != null)
            {
                //ray.rayName receive
            }
        }
        
        public void TriggerRayReceived (RayInfo ray, GameObject hit)
        {
            if (ray.activator != null)
            {
                //ray.rayName receive
            }

            if (caller != null)
            {
                //ray.rayName receive
            }
        }


        public void TriggerRayMissed (RayInfo ray)
        {
            if (ray.activator != null)
            {
                //ray.rayName miss
            }

            if (caller != null)
            {
                //ray.rayName miss
            }
        }
    }
}