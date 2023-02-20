using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    Vector2 move;
    Vector2 lookDelta;
    bool jump;
    float zoom;

    [SerializeField] float groundDistance;
    RaycastHit groundHit;
    bool ground;

    [SerializeField] Rigidbody RB;
    Vector3 velocity;

    [SerializeField] float gravity;
    [SerializeField] float moveDeadzone;
    Vector3 moveVelocity;
    bool moving;
    float speed;
    float speedPercent;
    float verticalVelocity;

    State state;

    [SerializeField] SphereCollider sphereCollider;

    [SerializeField] float height; // Distance between sphere center and ground
    [SerializeField] float stopDeadzone;

    [SerializeField] float baseSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float maxSpeed;

    [SerializeField] float boostAcceleration;
    [SerializeField] float boostMaxSpeed;

    [SerializeField] float friction;
    [SerializeField] float moveFriction;

    [SerializeField] int jumps;
    [SerializeField] float baseJumpHeight;
    [SerializeField] float fullJumpHeight;
    [SerializeField] float jumpTime;
    [SerializeField] float coyoteTime;
    Vector3 jumpNormal;
    bool canJump;
    bool jumping;
    int currentJumps;
    float currentJumpTime;
    float currentCoyoteTime;
    bool disableSnaping;

    [SerializeField, Range(0, 1)] float airControl; // Set between 0-1

    [SerializeField] Transform cameraTransform;
    [SerializeField] Vector2 lookSensitivity;
    [SerializeField] Vector2 cameraCorrectionForce;
    [SerializeField] float zoomSensitivity;
    [SerializeField] float cameraDistance;
    [SerializeField] float minCameraDistance;
    [SerializeField] float maxCameraDistance;
    [SerializeField] float cameraLoopMultiplier;
    [SerializeField] float cameraCorrectionOffset;
    [SerializeField] float cameraCorrectionIntensity;
    [SerializeField] float cameraHeight;
    [SerializeField, Range(0, 1)] float cameraSmoothness;
    Vector3 cameraNormal;
    Vector2 lookOffset;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        playerInput.onActionTriggered += callbackContext =>
        {
            switch (callbackContext.action.name)
            {
                case "Move":
                    move = callbackContext.ReadValue<Vector2>();
                    break;
                case "Look Delta":
                    lookDelta = callbackContext.ReadValue<Vector2>();
                    break;
                case "Jump":
                    jump = callbackContext.ReadValue<float>() == 1;
                    break;
                case "Zoom":
                    zoom = callbackContext.ReadValue<float>();
                    break;
            }
        };

        lookOffset = new Vector2(-90, 0);
    }
    private void FixedUpdate()
    {
        Count();
        ground = Physics.Raycast(transform.position, -transform.up, out groundHit, Mathf.Infinity, 3, QueryTriggerInteraction.Ignore) && groundHit.distance <= groundDistance;
        state = ground ? State.Ground : State.Air;
        InitPhysics();

        switch (state)
        {
            case State.Ground:
                SnapPositionToGround();
                SnapRotationToGround();
                Move();
                Friction();
                Jump();
                break;

            case State.Air:
                SnapPositionToGround();
                SnapRotationToGround();
                Move();
                velocity -= Vector3.up * gravity;
                AirControl();
                Jump();
                break;
        }

        RB.AddForce(velocity, ForceMode.Impulse);
        //RB.velocity += velocity;
    }

    private void Update()
    {
        Look();
    }

    void InitPhysics()
    {
        velocity = Vector3.zero;
        verticalVelocity = Vector3.Dot(RB.velocity, transform.up);
        moveVelocity = RB.velocity - (transform.up * verticalVelocity);
        speed = moveVelocity.magnitude;
        moving = move.magnitude > 0 && speed > moveDeadzone;
    }

    void SnapPositionToGround()
    {
        if (!ShouldSnapToGround())
        {
            return;
        }

        if (ground && !moving && speed <= stopDeadzone)
        {
            velocity -= RB.velocity;
        }

        Vector3 snapGoal = groundHit.point + (groundHit.normal * sphereCollider.radius);
  
        if (CollisionCheck(snapGoal).collided)
        {
            return;
        }

        transform.position = new Vector3(transform.position.x, snapGoal.y, transform.position.z);
    }

    void SnapRotationToGround()
    {
        Vector3 normal = ground ? groundHit.normal : Vector3.up;

        if (!ground)
        {
            Vector3 snapGoal = transform.position - (transform.up * height) + (normal * height);
            if (!CollisionCheck(snapGoal).collided)
            {
                transform.position = snapGoal;
            }
        }

        //transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, normal), normal);
    }

    CollisionInformation CollisionCheck(Vector3 goal)
    {
        CollisionInformation info = new CollisionInformation();

        if (Physics.SphereCast(transform.position, sphereCollider.radius - Physics.defaultContactOffset, Vector3.Normalize(goal - transform.position), out RaycastHit checkHit, Vector3.Distance(transform.position, goal), 3, QueryTriggerInteraction.Ignore))
        {
            info.hitInfo = checkHit;
            info.collided = true;
            info.cast = true;
        }

        if (Physics.CheckSphere(goal, sphereCollider.radius - Physics.defaultContactOffset, 3, QueryTriggerInteraction.Ignore))
        {
            info.collided = true;
            info.check = true;
        }

        return info;
    }

    void Move()
    {
        if (move.magnitude == 0)
        {
            return;
        }

        float currentAcceleration = acceleration;
        float currentMaxSpeed = maxSpeed;

        if ((groundHit.collider != null && groundHit.collider.gameObject.CompareTag("Boost")))
        {
           currentAcceleration = boostAcceleration;
           currentMaxSpeed = boostMaxSpeed;
        }

        float percent = Mathf.Clamp(speed / currentMaxSpeed, 0, 1);

        Vector3 moveForwardNormal = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
        Vector3 moveRightNormal = Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized;

        velocity += ((moveForwardNormal * move.y) + (moveRightNormal * move.x)) * (moving ? currentAcceleration * (1 - percent) : baseSpeed * (1 - Mathf.Max(Vector3.Dot(Vector3.forward, Vector3.up), 0)));
    }

    void Friction()
    {
        Vector3 frictionMask = velocity * (1 - Mathf.Max(Vector3.Dot(velocity, Vector3.up), 0));
        Vector3 frictionVelocity = moving ? (RB.velocity - (Vector3.Project(RB.velocity, frictionMask) * Mathf.Max(Vector3.Dot(RB.velocity.normalized, frictionMask.normalized), 0))) * moveFriction : RB.velocity * friction;

        velocity += moving ? -frictionVelocity + (velocity.normalized * frictionVelocity.magnitude * Mathf.Abs(Vector3.Dot(Vector3.Cross(moveVelocity, transform.up).normalized, velocity.normalized))) : -frictionVelocity;
    }

    bool ShouldSnapToGround()
    {
        if (ground && (groundHit.collider != null && groundHit.collider.gameObject.CompareTag("No_Snapping")))
        {
            disableSnaping = true;
            return false;
        }

        if (ground)
        {
            disableSnaping = false;
            return true;
        }

        if (jumping || disableSnaping)
        {
            return false;
        }

        return true;
    }

    void Jump()
    {
        if (ground)
        {
            canJump = true;
            jumping = false;
            currentJumps = jumps;
            currentCoyoteTime = coyoteTime;
        }
        else
        {
            if (currentCoyoteTime <= 0 && currentJumps == jumps)
            {
                currentJumps -= 1;
            }
            if (currentJumps > 0)
            {
                canJump = true;
            }
        }

        if (!jump)
        {
            //jumping = false;
            return;
        }

        if (canJump)
        {
            jumpNormal = transform.up;
            float jumpForce = Mathf.Sqrt(2 * (gravity / Time.fixedDeltaTime) * (ground ? (baseJumpHeight - groundDistance) : baseJumpHeight));

            velocity += jumpNormal * (ground ? jumpForce : Mathf.Max(jumpForce - Mathf.Max(verticalVelocity, 0), 0));
            canJump = false;
            jumping = true;
            currentJumps -= 1;
            currentJumpTime = jumpTime;

            if (ground)
            {
                Vector3 snapGoal = groundHit.point + (transform.up * (groundDistance + Physics.defaultContactOffset));
                if (CollisionCheck(snapGoal).collided)
                {
                    return;
                }
                transform.position = snapGoal;
            }

        }
        else if (jumping && currentJumpTime > 0)
        {
            velocity += jumpNormal * ((Time.fixedDeltaTime * (Mathf.Sqrt(2 * (gravity / Time.fixedDeltaTime) * fullJumpHeight) - Mathf.Sqrt(2 * (gravity / Time.fixedDeltaTime) * baseJumpHeight))) / jumpTime);
            if (currentJumpTime <= 0)
            {
                //jumping = false;
            }
        }
    }

    void Count()
    {
        // add boost

        if (currentJumpTime > 0)
        {
            currentJumpTime -= Time.fixedDeltaTime;
        }

        if (currentCoyoteTime > 0)
        {
            currentCoyoteTime -= Time.fixedDeltaTime;
        }
    }

    void AirControl()
    {
        if (!moving)
        {
            return;
        }

        velocity -= (moveVelocity - (Vector3.Project(moveVelocity, Vector3.ProjectOnPlane(velocity, transform.up)) * Mathf.Max(Vector3.Dot(moveVelocity.normalized, Vector3.ProjectOnPlane(velocity, transform.up).normalized), 0))) * airControl;
    }


    void Look()
    {
        Vector2 goalOffset = lookOffset + Vector2.Scale(lookDelta * Time.fixedDeltaTime, lookSensitivity);
        lookOffset = new Vector2(goalOffset.x, Mathf.Clamp(goalOffset.y, -89, 89));
        cameraDistance = Mathf.Clamp(cameraDistance + zoom * zoomSensitivity, minCameraDistance, maxCameraDistance);
        cameraNormal = Vector3.Lerp(cameraNormal, transform.up, 1 - cameraSmoothness);

        Quaternion cameraRotateGoal = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, cameraNormal), cameraNormal) * Quaternion.AngleAxis(lookOffset.x, Vector3.up) * Quaternion.AngleAxis(lookOffset.y, Vector3.right);
        cameraTransform.rotation = cameraRotateGoal;

        Vector3 cameraSnapGoal = (transform.position - (cameraTransform.forward * (cameraDistance - (cameraDistance * (1 - (Mathf.Max(Vector3.Dot(transform.up, Vector3.up), 0) + 1) / 2) * cameraLoopMultiplier)))) + (transform.up * cameraHeight);
        cameraTransform.position = cameraSnapGoal;

        if (lookDelta.magnitude > 0)
        {
            return;
        }

        CollisionInformation cameraCheckInformation = CollisionCheck(cameraSnapGoal);

        if (cameraCheckInformation.cast)
        {
            lookOffset.y += Vector3.Dot(cameraCheckInformation.hitInfo.normal, transform.up) * cameraCorrectionIntensity;
        }

        lookOffset.x += Vector3.Dot(Vector3.ProjectOnPlane(cameraTransform.right, transform.up), moveVelocity) * speedPercent * cameraCorrectionForce.x * Vector3.Dot(cameraTransform.up, Vector3.up);
        lookOffset.y += (Vector3.Dot(cameraTransform.up, transform.up) + cameraCorrectionOffset) * speedPercent * cameraCorrectionForce.y * cameraCorrectionIntensity;
    }
    struct CollisionInformation
    {
        public RaycastHit hitInfo;
        public bool collided;
        public bool cast;
        public bool check;
    }

    enum State
    {
        Ground,
        Air
    }
}
