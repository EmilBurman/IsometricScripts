using StateEnumerators;
using System;
using UnityEngine;



public class PlayerMovement : MonoBehaviour, IMovement
{
    //-------------------------------------------------------
    #region IMovement interface
    public void Grounded(float horizontal, float vertical)
    {
        throw new NotImplementedException();
    }

    public void ToggleTargeting(bool value, Transform target)
    {
        currentlyTargeting = value;
        this.target = target;
    }
    public void Airborne(float horizontal, float vertical)
    {
        throw new NotImplementedException();
    }

    public void CameraAngle(cameraState cameraAngle)
    {
        switch (cameraAngle)
        {
            case cameraState.position1:
                movementDisplacement = Quaternion.Euler(180, 45, 0);
                break;
            case cameraState.position2:
                movementDisplacement = Quaternion.Euler(-180, -45, 0);
                break;
            case cameraState.position3:
                movementDisplacement = Quaternion.Euler(180, -135, 0);
                break;
            case cameraState.position4:
                movementDisplacement = Quaternion.Euler(-180, 135, 0);
                break;
        }
    }
#endregion
    //-------------------------------------------------------


    //Public variables
    public float speed = 6f;            // The speed that the player will move at.
    public float sprintSpeedMultiplier = 2;
    public float jumpForce = 10f;

    //Internal variables
    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    int groundMask;                     // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.
    bool currentlyTargeting;            // Check if targeting
    Transform target;                   // If targeting, target this transform position
    Quaternion movementDisplacement;

    void Awake()
    {
        // Create a layer mask for the ground layer.
        groundMask = LayerMask.GetMask(Layers.GROUND);

        // Set up initial values
        currentlyTargeting = false;
        movementDisplacement = Quaternion.Euler(180, 45, 0);

        // Set up references.
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Store the input axes.
        float horizontalAxis = Input.GetAxisRaw(Inputs.HORIZONTAL);
        float verticalAxis = Input.GetAxisRaw(Inputs.VERTICAL);
        bool jump = Input.GetButtonDown(Inputs.JUMP);
        bool sprint = Input.GetButton(Inputs.SPRINT);
        bool aim = Input.GetButton(Inputs.AIM);

        // Move the player around the scene.
        Move(horizontalAxis, verticalAxis, sprint);
        Jump(jump);

        // Turn the player to face the mouse cursor if aiming or towards target if toggled.
        if (!currentlyTargeting)
            Turning(aim);
        else
            TargetTurning();

        // Animate the player.
        Animating(horizontalAxis, verticalAxis, sprint);
    }

    void Move(float h, float v, bool sprint)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0, v);

        float playerSpeed;

        if (sprint)
            playerSpeed = speed * sprintSpeedMultiplier;
        else
            playerSpeed = speed;

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * playerSpeed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + (movementDisplacement * movement));
    }

    void Jump(bool jump)
    {
        if (jump)
            playerRigidbody.velocity += jumpForce * Vector3.up;
    }

    void Turning(bool aiming)
    {

        if (movement != Vector3.zero && !aiming)
            playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Quaternion.Euler(180, 45, 0) * movement), 0.2f));
        else if (aiming)
        {
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if (Physics.Raycast(camRay, out floorHit, camRayLength, groundMask))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
        }
    }

    void TargetTurning()
    {
        Vector3 playerToTarget = target.position - transform.position;
        playerToTarget.y = 0f;
        playerRigidbody.MoveRotation(Quaternion.LookRotation(playerToTarget));
    }

    void Animating(float h, float v, bool sprint)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        bool moving = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking.
        if (moving && sprint)
            anim.SetBool("IsSprinting", sprint);
        else
            anim.SetBool("IsWalking", moving);
    }
}
