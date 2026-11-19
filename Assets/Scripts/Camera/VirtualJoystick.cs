using UnityEngine;
using UnityEngine.EventSystems;

namespace BellaVirtualLife.CameraSystem
{
    /// <summary>Simple left-side virtual joystick. Attach to the joystick background UI Image.</summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 65f;
        private RectTransform rectTransform;
        private Vector2 input;

        public Vector2 Direction => input;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            if (handle == null && transform.childCount > 0)
                handle = transform.GetChild(0) as RectTransform;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 local))
                return;

            Vector2 half = rectTransform.rect.size * 0.5f;
            input = new Vector2(local.x / half.x, local.y / half.y);
            input = Vector2.ClampMagnitude(input, 1f);
            if (handle != null) handle.anchoredPosition = input * handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            if (handle != null) handle.anchoredPosition = Vector2.zero;
        }
    }
}
