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
    float boost;

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

    enum State
    {
        Ground,
        Air
    }

    State state;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

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
                case "Boost":
                    boost = callbackContext.ReadValue<float>();
                    break;
            }
        };
    }

    void InitPhysics()
    {
        velocity = Vector3.zero;
        verticalVelocity = Vector3.Dot(RB.velocity, transform.up);
        moveVelocity = RB.velocity - (transform.up * verticalVelocity);
        speed = moveVelocity.magnitude;
        moving = move.magnitude > 0 && speed > moveDeadzone;
    }

    void Update()
    {
        ground = Physics.Raycast(transform.position, -transform.up, out groundHit, Mathf.Infinity, 3, QueryTriggerInteraction.Ignore) && groundHit.distance <= groundDistance;
        state = ground ? State.Ground : State.Air;

        // RB.AddForce(velocity, ForceMode.Impulse);

        velocity -= Vector3.up * gravity;
        RB.velocity += velocity;

    }
}
