using UnityEngine;
using UnityEngine.EventSystems;

namespace BellaVirtualLife.CameraSystem
{
    /// <summary>
    /// GTA-style third-person camera:
    /// - follows a target
    /// - touch-drag orbit (360 degrees)
    /// - two-finger pinch zoom
    /// - scroll-wheel zoom in the Editor
    /// - sphere-cast collision to avoid clipping through walls/furniture
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.55f, 0f);
        [SerializeField] private float followSmoothTime = 0.08f;

        [Header("Orbit")]
        [SerializeField] private float yaw = 0f;
        [SerializeField] private float pitch = 12f;
        [SerializeField] private float minPitch = -15f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float touchSensitivity = 0.18f;
        [SerializeField] private bool rotateOnlyOnRightSide = true;

        [Header("Zoom")]
        [SerializeField] private float distance = 4.2f;
        [SerializeField] private float minDistance = 1.6f;
        [SerializeField] private float maxDistance = 8f;
        [SerializeField] private float pinchSensitivity = 0.012f;
        [SerializeField] private float scrollSensitivity = 1.2f;

        [Header("Collision")]
        [SerializeField] private float collisionRadius = 0.22f;
        [SerializeField] private float collisionPadding = 0.08f;
        [SerializeField] private LayerMask collisionLayers = ~0;

        private Vector3 pivotVelocity;
        private Vector3 smoothedPivot;
        private bool initialized;
        private float pinchStartDistance;
        private float pinchStartCameraDistance;

        public float Distance => distance;

        private void LateUpdate()
        {
            if (target == null) return;

            ReadTouchInput();
#if UNITY_EDITOR || UNITY_STANDALONE
            ReadEditorInput();
#endif
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Vector3 wantedPivot = target.position + pivotOffset;
            if (!initialized)
            {
                smoothedPivot = wantedPivot;
                yaw = target.eulerAngles.y;
                initialized = true;
            }

            smoothedPivot = Vector3.SmoothDamp(
                smoothedPivot, wantedPivot, ref pivotVelocity, followSmoothTime);

            Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 backwards = orbitRotation * Vector3.back;
            Vector3 wantedPosition = smoothedPivot + backwards * distance;
            Vector3 finalPosition = ResolveCameraCollision(smoothedPivot, wantedPosition);

            transform.position = finalPosition;
            transform.rotation = Quaternion.LookRotation(smoothedPivot - finalPosition, Vector3.up);
        }

        private void ReadTouchInput()
        {
            if (Input.touchCount >= 2)
            {
                Touch a = Input.GetTouch(0);
                Touch b = Input.GetTouch(1);

                if (a.phase == TouchPhase.Began || b.phase == TouchPhase.Began)
                {
                    pinchStartDistance = Vector2.Distance(a.position, b.position);
                    pinchStartCameraDistance = distance;
                }
                else
                {
                    float current = Vector2.Distance(a.position, b.position);
                    float delta = current - pinchStartDistance;
                    distance = pinchStartCameraDistance - delta * pinchSensitivity;
                }
                return;
            }

            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase != TouchPhase.Moved) return;
                if (rotateOnlyOnRightSide && t.position.x < Screen.width * 0.42f) return;

                // Ignore UI controls (for example action buttons).
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(t.fingerId)) return;

                yaw += t.deltaPosition.x * touchSensitivity;
                pitch -= t.deltaPosition.y * touchSensitivity;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void ReadEditorInput()
        {
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * 4f;
                pitch -= Input.GetAxis("Mouse Y") * 4f;
            }

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.001f)
                distance -= scroll * scrollSensitivity;
        }
#endif

        private Vector3 ResolveCameraCollision(Vector3 origin, Vector3 desired)
        {
            Vector3 direction = desired - origin;
            float wantedDistance = direction.magnitude;
            if (wantedDistance <= 0.001f) return desired;
            direction /= wantedDistance;

            RaycastHit[] hits = Physics.SphereCastAll(
                origin, collisionRadius, direction, wantedDistance,
                collisionLayers, QueryTriggerInteraction.Ignore);

            float allowedDistance = wantedDistance;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == target || hit.transform.IsChildOf(target)) continue;
                allowedDistance = Mathf.Min(
                    allowedDistance, Mathf.Max(0.15f, hit.distance - collisionPadding));
            }

            return origin + direction * allowedDistance;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            initialized = false;
        }

        public void SnapBehindTarget()
        {
            if (target == null) return;
            yaw = target.eulerAngles.y;
            pitch = 12f;
        }
    }
}
