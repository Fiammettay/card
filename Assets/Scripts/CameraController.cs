using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Distance")]
    public float distance = 5f;
    public float minDistance = 0.6f;
    public float followSmoothTime = 0.04f;

    [Header("Rotation")]
    public float mouseSensitivity = 0.12f;
    public float minPitch = -35f;
    public float maxPitch = 70f;

    [Header("Wall Collision")]
    public float collisionRadius = 0.2f;
    public float collisionPadding = 0.1f;
    public float restoreSpeed = 5f;
    public LayerMask collisionLayer;

    [Header("Interaction Camera")]
    [SerializeField] private float interactionMoveSpeed = 10f;
    [SerializeField] private float interactionRotateSpeed = 120f;
    [SerializeField] private float returnMoveSpeed = 7f;
    [SerializeField] private float returnRotateSpeed = 300f;

    private enum CameraState
    {
        Follow,
        EnterInteraction,
        Interaction,
        Returning
    }

    private CameraState currentState = CameraState.Follow;

    // 当前交互物体提供的摄像机机位
    private Transform interactionCameraPoint;

    private float yaw;
    private float pitch = 15f;
    private float currentDistance;
    private Vector3 smoothTarget;
    private Vector3 smoothVelocity;

    private bool lastCursorUnlocked;
    private bool IsInInteraction
    {
        get
        {
            return currentState == CameraState.EnterInteraction ||
                   currentState == CameraState.Interaction;
        }
    }

    private void Start()
    {
        currentDistance = distance;

        if (target != null)
            smoothTarget = target.position + targetOffset;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameApp.MsgCenter.AddEvent(Defines.OnEnterInteractionCamera, EnterInteractionCamera);
    }

    private void LateUpdate()
    {
        UpdateCursorState();

        if (target == null)
            return;
      
        switch (currentState)
        {
            case CameraState.Follow:

                UpdateFollowCamera();
                break;

            case CameraState.EnterInteraction:

                UpdateInteractionCamera();
                break;

            case CameraState.Returning:
                UpdateReturningCamera();
                break;
        }
    }

    private void UpdateCursorState()
    {
        bool isTemporaryUnlocked =
            GameApp.PlayerInputManager != null &&
            GameApp.PlayerInputManager.IsUnLock;

        bool shouldUnlock =
            IsInInteraction || isTemporaryUnlocked;

        // 状态没有变化时不重复设置
        if (lastCursorUnlocked == shouldUnlock)
        {
            return;
        }

        lastCursorUnlocked = shouldUnlock;

        Cursor.lockState = shouldUnlock
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = shouldUnlock;
    }

    private void UpdateFollowCamera()
    {
        bool isTemporaryUnlocked =
            GameApp.PlayerInputManager != null &&
            GameApp.PlayerInputManager.IsUnLock;

        // 按住临时解锁键时不旋转镜头
        if (!isTemporaryUnlocked)
        {
            ReadLookInput();
        }

        CalculateFollowPos(
            out Vector3 desiredPosition,
            out Quaternion desiredRotation);

        transform.position = desiredPosition;
        transform.rotation = desiredRotation;
    }

    private void UpdateInteractionCamera()
    {
        if(interactionCameraPoint == null)
        {
            ExitInteractionCamera();
            return;
        }

        

        Vector3 position = Vector3.MoveTowards(
            transform.position,
            interactionCameraPoint.position,
            interactionMoveSpeed * Time.deltaTime
        );

        Quaternion rotation = Quaternion.RotateTowards(
            transform.rotation,
            interactionCameraPoint.rotation,
            interactionRotateSpeed * Time.deltaTime
        );

        transform.position = position;
        transform.rotation = rotation;

        bool positionReached = Vector3.SqrMagnitude(position - interactionCameraPoint.position) < 0.004f;
        bool rotationReached = Quaternion.Angle(rotation, interactionCameraPoint.rotation) < 0.5f;

        if(!positionReached || !rotationReached)
        {
            return;
        }

        transform.position = interactionCameraPoint.position;
        transform.rotation = interactionCameraPoint.rotation;

        currentState = CameraState.Interaction;

        GameApp.MsgCenter.PostEvent(Defines.OnInteractionCamera);
    }

    private void UpdateReturningCamera()
    {
        CalculateFollowPos(out Vector3 desiredPosition, out Quaternion desiredRotation);


        Vector3 position = Vector3.MoveTowards(
            transform.position,
            desiredPosition,
            returnMoveSpeed * Time.deltaTime
        );

        Quaternion rotation = Quaternion.RotateTowards(
            transform.rotation,
            desiredRotation,
            returnRotateSpeed * Time.deltaTime
        );

        transform.position = position;
        transform.rotation = rotation;

        bool positionReached = Vector3.SqrMagnitude(position - desiredPosition) < 0.0004f;

        bool rotationReached = Quaternion.Angle(rotation, desiredRotation) < 0.5f;

        if (positionReached && rotationReached)
        {
            currentState = CameraState.Follow;
        }
    }

    private void ReadLookInput()
    {
        yaw += GameApp.PlayerInputManager.MouseInput.x * mouseSensitivity;//水平旋转角度，即绕y轴旋转
        pitch -= GameApp.PlayerInputManager.MouseInput.y * mouseSensitivity;//垂直旋转角度,即绕x轴旋转
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private float GetAllowedDistance(Vector3 origin, Vector3 direction)
    {
        if (Physics.SphereCast(
            origin,
            collisionRadius,
            direction,
            out RaycastHit hit,
            distance,
            collisionLayer,
            QueryTriggerInteraction.Ignore
        ))//从跟随点发射一个球体检测墙壁
        {
            return Mathf.Clamp(
                hit.distance - collisionPadding,//碰到墙的距离，减去一定的容错
                minDistance,
                distance
            );
        }

        return distance;
    }

    //计算第三人称时摄像头位置
    private void CalculateFollowPos(out Vector3 desiredPosition, out Quaternion desiredRotation)
    {
        Vector3 desiredTarget = target.position + targetOffset;

        smoothTarget = Vector3.SmoothDamp(
            smoothTarget,
            desiredTarget,
            ref smoothVelocity,
            followSmoothTime
        );

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);//分别绕x,y,z旋转角
        Vector3 backward = rotation * Vector3.back;

        float allowedDistance = GetAllowedDistance(
            smoothTarget,
            backward
        );

        float speed = allowedDistance < currentDistance//离角色过远
            ? 25f
            : restoreSpeed;

        currentDistance = Mathf.MoveTowards(
            currentDistance,
            allowedDistance,
            speed * Time.deltaTime
        );

        desiredPosition = smoothTarget + backward * currentDistance;

        desiredRotation = rotation;
    }

    /// <summary>
    /// 进入指定的交互机位
    /// </summary>
    public void EnterInteractionCamera(object arg)
    {
        Transform cameraPoint = arg as Transform;
        if (cameraPoint == null)
        {
            Debug.LogWarning("交互物体没有设置 CameraPoint");
            return;
        }

        interactionCameraPoint = cameraPoint;
        currentState = CameraState.EnterInteraction;
    }

    /// <summary>
    /// 退出交互机位，返回正常第三人称视角
    /// </summary>
    public void ExitInteractionCamera()
    {
        interactionCameraPoint = null;

        if (currentState != CameraState.Follow)
        {
            currentState = CameraState.Returning;
        }
    }

    private void OnDestroy()
    {
        GameApp.MsgCenter.RemoveEvent(Defines.OnEnterInteractionCamera, EnterInteractionCamera);
    }
}