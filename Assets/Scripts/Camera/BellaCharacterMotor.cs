using UnityEngine;

namespace BellaVirtualLife.CameraSystem
{
    [RequireComponent(typeof(CharacterController))]
    public class BellaCharacterMotor : MonoBehaviour
    {
        [SerializeField] private VirtualJoystick joystick;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float moveSpeed = 3.2f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;

        private CharacterController controller;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (Camera.main != null) cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Vector2 input = joystick != null ? joystick.Direction : Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
            if (joystick == null || input.sqrMagnitude < 0.001f)
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
            Vector3 forward = cameraTransform != null
                ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized
                : Vector3.forward;
            Vector3 right = cameraTransform != null
                ? Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized
                : Vector3.right;

            Vector3 move = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion wanted = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, wanted, rotationSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }

        public void SetJoystick(VirtualJoystick value) => joystick = value;
    }
}
