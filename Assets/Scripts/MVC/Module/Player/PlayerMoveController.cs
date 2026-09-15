using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    private Transform cameraTransform;
    private Transform groundCheck;//需手动设置
    private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float sprintSpeed = 7.5f;
    public float turnSmoothTime = 0.08f;

    [Header("Jump")]
    public float jumpHeight = 1.2f;
    public float gravity = -25f;

    [Header("Ground")]
    public float groundRadius = 0.25f;
    public LayerMask groundLayer;

    private float verticalSpeed;
    private float turnVelocity;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        animator = GetComponent<Animator>();
        groundCheck = transform.Find("GroundCheck");
    }

    private void Update()
    {
        //检查角色是否在地上
        bool grounded = Physics.CheckSphere(
            groundCheck.position,//球体中心坐标
            groundRadius,//球体半径
            groundLayer,//只检测指定图层
            QueryTriggerInteraction.Ignore//忽略触发器碰撞场景里带 `Is Trigger` 的碰撞体（传送门、伤害区域、触发区域）不参与地面判定
        );

        if (grounded && verticalSpeed < 0f)
            verticalSpeed = -2f;//角色踩在地上时，强制把向下掉落的速度改成很小的 `-2`，避免落地弹跳、斜坡浮空、移动卡顿，让人物稳稳贴住地面。

        Vector2 moveInput = ReadMoveInput();
        Vector3 moveDirection = GetCameraRelativeDirection(moveInput);

        bool sprinting = GameApp.PlayerInputManager.IsSprint && moveInput.sqrMagnitude > 0.01f;//sqrMagnitude向量长度的平方，比Magnitude少开根号，节约性能
        float speed = sprinting ? sprintSpeed : moveSpeed;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            float targetAngle =
                Mathf.Atan2(moveDirection.x, moveDirection.z) *//arctan x/z
                Mathf.Rad2Deg;//根据移动向量的水平 X、Z 分量，算出**弧度值**：向量相对于世界 Z 轴（正北）的偏移角度；参数顺序固定：`Atan2(水平右X, 前后Z)`，匹配 3D 地面坐标系。Atan2 输出是**弧度**，Unity 物体旋转`eulerAngles.y`是**角度**，乘以该常量完成单位转换。

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnVelocity,//SmoothDampAngle需要记录上一帧的旋转角速度，才能做出 “加速→减速” 的惯性转身效果,必须引用，因为在时刻修改该值
                turnSmoothTime//代表从当前角度平滑到目标角度大概需要多少秒。
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        if (GameApp.PlayerInputManager.IsJump && grounded)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);//基于匀变速直线运动公式,得到跳跃初速度
        }
            
        verticalSpeed += gravity * Time.deltaTime;

        Vector3 velocity =
            moveDirection * speed +
            Vector3.up * verticalSpeed;

  

        transform.Translate(velocity * Time.deltaTime, Space.World);

        //UpdateAnimator(input, sprinting, grounded);
    }

    private Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;

        input = GameApp.PlayerInputManager.MoveInput;//input.x左右 input.y前后

        return Vector2.ClampMagnitude(input, 1f);
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (cameraTransform == null)
            return new Vector3(input.x, 0f, input.y);

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return (forward * input.y + right * input.x).normalized *
               input.magnitude;
    }

    private void UpdateAnimator(
        Vector2 input,
        bool sprinting,
        bool grounded
    )
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", input.magnitude);
        animator.SetBool("Sprint", sprinting);
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("VerticalSpeed", verticalSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundRadius
            );
    }
}