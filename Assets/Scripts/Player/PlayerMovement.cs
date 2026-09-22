using UnityEngine;

namespace Butcher_TA
{
    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {
        [SerializeField] private Transform control;
        [SerializeField] private Transform modelPoint;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float borderLimit = 0.65f;
        [SerializeField] private float rotationAngle = 45f;
        [SerializeField] private float returnSpeed = 10f;

        private float targetX;
        private float previousPointerX;
        private bool wasDragging;
        private bool canMove;

        public bool CanMove
        {
            get => canMove;
            set
            {
                canMove = value;
                if (!value)
                {
                    targetX = control.localPosition.x;
                    wasDragging = false;
                }
            }
        }

        private void Update()
        {
            bool isDragging = CanMove && Input.GetMouseButton(0);
            if (isDragging)
            {
                if (!wasDragging)
                {
                    previousPointerX = Input.mousePosition.x;
                }
                else
                {
                    Move();
                }
            }

            wasDragging = isDragging;

            Vector3 position = control.localPosition;
            position.x = Mathf.MoveTowards(position.x, targetX, speed * Time.deltaTime);
            control.localPosition = position;

            if (isDragging && Mathf.Abs(targetX - position.x) > 0.001f)
            {
                RotateCharacter();
            }
            else
            {
                ReturnToOriginalRotation();
            }
        }

        private void Move()
        {
            float pointerX = Input.mousePosition.x;
            float delta = pointerX - previousPointerX;
            previousPointerX = pointerX;
            targetX = Mathf.Clamp(targetX + delta / Mathf.Max(1, Screen.width) * borderLimit * 2f, -borderLimit, borderLimit);
        }

        private void RotateCharacter()
        {
            float remainingMovement = targetX - control.localPosition.x;
            float targetAngle = Mathf.Clamp(remainingMovement / Mathf.Max(0.01f, borderLimit) * rotationAngle, -rotationAngle, rotationAngle);
            modelPoint.localRotation = Quaternion.Slerp(modelPoint.localRotation, Quaternion.Euler(0f, targetAngle, 0f), returnSpeed * Time.deltaTime);
        }

        private void ReturnToOriginalRotation()
        {
            modelPoint.localRotation = Quaternion.Slerp(modelPoint.localRotation, Quaternion.identity, returnSpeed * Time.deltaTime);
        }

        public void ResetPosition()
        {
            targetX = 0f;
            previousPointerX = 0f;
            wasDragging = false;
            control.localPosition = new Vector3(0f, control.localPosition.y, control.localPosition.z);
            modelPoint.localRotation = Quaternion.identity;
        }
    }

    public interface IPlayerMovement
    {
        bool CanMove { get; set; }
        void ResetPosition();
    }
}
