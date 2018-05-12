using StateEnumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour, IMovement
{
    #region IMovement interface

    public void Grounded(float horizontal, float vertical)
    {
        if (canMove && !isDead)
        {
            MovementInputRelativeToCamera(horizontal, horizontal);
            moveSpeed = UpdateMovement();
        }

        Vector3 motion = inputVec;

    }

    public void ToggleTargeting(bool value, Transform target)
    {
        throw new NotImplementedException();
    }

    public void Airborne(float horizontal, float vertical)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Variables
    //Components
    Rigidbody rb;
    protected Animator animator;
    public GameObject target;
    public Camera objectCamera;

    [HideInInspector]
    public Vector3 targetDashDirection;
    public bool useNavMesh = false;
    private UnityEngine.AI.NavMeshAgent agent;
    private float navMeshSpeed;
    public Transform goal;

    // Used for continuing momentum while in air
    public float inAirSpeed = 8f;
    float maxVelocity = 2f;
    float minVelocity = -2f;

    //rolling variables
    public float rollSpeed = 8;
    bool isRolling = false;
    public float rollduration;

    //movement variables
    [HideInInspector]
    public bool canMove = true;
    public float runSpeed = 6f;
    public float walkSpeed = 1.35f;
    float moveSpeed;
    float rotationSpeed = 40f;
    Quaternion movementDisplacement;        // Rotation of inputs in relation to camera angle.
    Vector3 movement;                       // The vector to store the direction of the player's movement.
    Vector3 inputVec;
    Vector3 newVelocity;

    //isStrafing/action variables
    [HideInInspector]
    public bool canAction = true;
    [HideInInspector]
    public bool isStrafing = false;
    [HideInInspector]
    public bool isDead = false;
    public float knockbackMultiplier = 1f;
    bool isKnockback;

    #endregion
    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = false;
    }

    #region Updates
    void FixedUpdate()
    {
        if (canMove && !isDead)
        {
            moveSpeed = UpdateMovement();
        }
    }

    void LateUpdate()
    {
        if (!useNavMesh)
        {
            //Get local velocity of charcter
            float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
            float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
            //Update animator with movement values
            animator.SetFloat("Velocity X", velocityXel / runSpeed);
            animator.SetFloat("Velocity Z", velocityZel / runSpeed);
            //if character is alive and can move, set our animator
            if (!isDead && canMove)
            {
                if (moveSpeed > 0)
                {
                    animator.SetBool("Moving", true);
                }
                else
                {
                    animator.SetBool("Moving", false);
                }
            }
        }
    }
    #endregion

    #region Move and rotate player
    void RotateTowardsMovementDirection()
    {
        if (movement != Vector3.zero)
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Quaternion.Euler(180, 45, 0) * movement), Time.deltaTime * rotationSpeed));
    }

    void MovementInputRelativeToCamera(float horizontal, float veritcal)
    {
        //converts control input vectors into camera facing vectors
        Transform cameraTransform = objectCamera.transform;
        //Forward vector relative to the camera along the x-z plane   
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        //Right vector relative to the camera always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        if (!isRolling)
        {
            targetDashDirection = horizontal * right + veritcal * -forward;
        }
        inputVec = horizontal * right + veritcal * forward;
    }

    float UpdateMovement()
    {
        if (!useNavMesh)
        {
            
        }
        Vector3 motion = inputVec;
        if (true)
        {
            //reduce input for diagonal movement
            if (motion.magnitude > 1)
            {
                motion.Normalize();
            }
            if (canMove)
            {
                //set speed by walking / running
                if (isStrafing)
                {
                    newVelocity = motion * walkSpeed;
                }
                else
                {
                    newVelocity = motion * runSpeed;
                }
                //if rolling use rolling speed and direction
                if (isRolling)
                {
                    //force the dash movement to 1
                    targetDashDirection.Normalize();
                    newVelocity = rollSpeed * targetDashDirection;
                }
            }
        }
        else
        {
            //if we are falling use momentum
            newVelocity = rb.velocity;
        }
        if (!isStrafing || !canMove)
        {
            RotateTowardsMovementDirection();
        }
        if (isStrafing)
        {
            //make character point at target
            Quaternion targetRotation;
            Vector3 targetPos = target.transform.position;
            targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
        }
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
        //return a movement value for the animator
        return inputVec.magnitude;
    }

    #endregion

    #region Roll
    void DodgeRoll(float horizontal, float vertical, bool dodge)
    {
        if (!isRolling && dodge)
        {
            if (vertical > .5 || vertical < -.5 || horizontal > .5 || horizontal < -.5)
            {
                StartCoroutine(_DirectionalRoll(vertical, horizontal));
            }
        }
    }
    public IEnumerator _DirectionalRoll(float x, float v)
    {
        //check which way the dash is pressed relative to the character facing
        float angle = Vector3.Angle(targetDashDirection, -transform.forward);
        float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(targetDashDirection, transform.forward)));
        // angle in [-179,180]
        float signed_angle = angle * sign;
        //angle in 0-360
        float angle360 = (signed_angle + 180) % 360;
        //deternime the animation to play based on the angle
        if (angle360 > 315 || angle360 < 45)
        {
            StartCoroutine(_Roll(1));
        }
        if (angle360 > 45 && angle360 < 135)
        {
            StartCoroutine(_Roll(2));
        }
        if (angle360 > 135 && angle360 < 225)
        {
            StartCoroutine(_Roll(3));
        }
        if (angle360 > 225 && angle360 < 315)
        {
            StartCoroutine(_Roll(4));
        }
        yield return null;
    }

    public IEnumerator _Roll(int rollNumber)
    {
        if (rollNumber == 1)
        {
            animator.SetTrigger("RollForwardTrigger");
        }
        if (rollNumber == 2)
        {
            animator.SetTrigger("RollRightTrigger");
        }
        if (rollNumber == 3)
        {
            animator.SetTrigger("RollBackwardTrigger");
        }
        if (rollNumber == 4)
        {
            animator.SetTrigger("RollLeftTrigger");
        }
        isRolling = true;
        yield return new WaitForSeconds(rollduration);
        isRolling = false;
    }
    #endregion

    #region Lock movement
    //method to keep character from moveing while attacking, etc
    public IEnumerator _LockMovementAndAttack(float delayTime, float lockTime)
    {
        yield return new WaitForSeconds(delayTime);
        canAction = false;
        canMove = false;
        animator.SetBool("Moving", false);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        inputVec = new Vector3(0, 0, 0);
        animator.applyRootMotion = true;
        yield return new WaitForSeconds(lockTime);
        canAction = true;
        canMove = true;
        animator.applyRootMotion = false;
    }
    #endregion
}
