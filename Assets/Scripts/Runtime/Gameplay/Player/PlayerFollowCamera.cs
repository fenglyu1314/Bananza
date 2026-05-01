using UnityEngine;

namespace Bananza.Gameplay.Player
{
    public class PlayerFollowCamera : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;

        [Header("Position Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -3f);
        [SerializeField] private float followSmoothTime = 0.1f;

        [Header("Look Settings")]
        [SerializeField] private float lookAtHeight = 1f;

        private Vector3 currentVelocity;

        private void LateUpdate()
        {
            if (target == null)
            {
                // 防御性编程：目标为空时不执行任何操作
                return;
            }

            FollowTarget();
            LookAtTarget();
        }

        private void FollowTarget()
        {
            // 计算目标位置（目标位置 + 偏移量）
            Vector3 targetPosition = target.position + offset;

            // 使用平滑阻尼移动到目标位置
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                followSmoothTime
            );
        }

        private void LookAtTarget()
        {
            // 计算注视点（目标位置 + 高度偏移）
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeight;

            // 让相机朝向注视点
            transform.LookAt(lookAtPoint);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // 确保数值合理
            followSmoothTime = Mathf.Max(0.01f, followSmoothTime);
            lookAtHeight = Mathf.Max(0f, lookAtHeight);
        }
#endif
    }
}
