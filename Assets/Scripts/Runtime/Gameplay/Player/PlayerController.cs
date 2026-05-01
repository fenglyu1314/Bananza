using UnityEngine;
using UnityEngine.InputSystem;

namespace Bananza.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSmoothTime = 0.1f;

        [Header("Gravity Settings")]
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundedGravity = -0.5f;

        [Header("References")]
        [SerializeField] private Camera playerCamera;

        private CharacterController characterController;
        private PlayerInputActions playerInputActions;
        private Vector2 moveInput;
        private Vector3 velocity;
        private float turnSmoothVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            // 初始化输入系统
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();

            // 如果没有指定相机，尝试查找主相机
            if (playerCamera == null)
                playerCamera = Camera.main;
        }

        private void OnEnable()
        {
            playerInputActions.Player.Move.performed += OnMovePerformed;
            playerInputActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            playerInputActions.Player.Move.performed -= OnMovePerformed;
            playerInputActions.Player.Move.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector2.zero;
        }

        private void Update()
        {
            HandleMovement();
            HandleGravity();
        }

        private void HandleMovement()
        {
            if (moveInput.magnitude >= 0.1f && playerCamera != null)
            {
                // 计算相机相对方向
                Vector3 cameraForward = playerCamera.transform.forward;
                Vector3 cameraRight = playerCamera.transform.right;

                cameraForward.y = 0f;
                cameraRight.y = 0f;
                cameraForward.Normalize();
                cameraRight.Normalize();

                // 计算世界空间移动方向
                Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

                // 平滑旋转角色朝向
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // 移动角色
                Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
                characterController.Move(movement);
            }
        }

        private void HandleGravity()
        {
            if (characterController.isGrounded)
            {
                velocity.y = groundedGravity;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            characterController.Move(velocity * Time.deltaTime);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // 确保数值合理
            moveSpeed = Mathf.Max(0f, moveSpeed);
            turnSmoothTime = Mathf.Max(0.01f, turnSmoothTime);
            gravity = Mathf.Min(-0.1f, gravity);
        }
#endif
    }
}
