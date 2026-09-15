using UnityEngine;

public class TouchCameraController : MonoBehaviour
{
    public float moveSpeed = 0.01f; // 摄像头移动速度
    public float minX = -10f; // 摄像头移动的最小X范围
    public float maxX = 10f; // 摄像头移动的最大X范围
    public float minY = -10f; // 摄像头移动的最小Y范围
    public float maxY = 10f; // 摄像头移动的最大Y范围

    private Vector2 lastTouchPosition;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 记录触摸开始的位置
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // 计算触摸移动的偏移量
                Vector2 delta = touch.position - lastTouchPosition;

                // 移动摄像头
                Vector3 newPosition = transform.position;
                newPosition.x -= delta.x * moveSpeed;
                newPosition.y -= delta.y * moveSpeed;

                // 限制摄像头的移动范围
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

                transform.position = newPosition;

                // 更新上一次触摸位置
                lastTouchPosition = touch.position;
            }
        }
    }
}