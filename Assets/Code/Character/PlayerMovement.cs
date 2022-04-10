using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    internal CharacterController controller;
    GameObject camTarget;
    [SerializeField] Transform groundCheck;

    public Vector3 movement, momentum;
    private float accel = 0;

    internal bool moving, jumping, run, jumpPressed, autoTurningPlayer;
    public float rollingProgress = 0;
    public float rollSpeed = 25;
    public float rollEndSpeed = 5;
    public float rollTimer;
    public float rollLength = 1;
    public float slideSpeed;

    public bool knockedBack = false;
    int jumps = 2;

    // GroundCheck:
    internal RaycastHit rayHit;
    internal Vector3 center, size;
    Vector3 groundNormal = Vector3.zero;
    RaycastHit groundCheckInfo;
    public Transform left;
    public Transform back;
    public Transform front;
    public Transform right;
    RaycastHit l;
    RaycastHit b;
    RaycastHit f;
    RaycastHit r;


    public int coyoteTime = 30;
    [SerializeField] int runningCoyoteTime = 0;
    bool coyoteTimeActive = false;

    public bool grounded => GroundCheck();
    int notGroundedCounter = 0;


    void Start() {
        controller = GetComponent<CharacterController>();
        camTarget = player.vcamera.camTarget;
        SetupJumpVariables();
        rollTimer = rollSpeed;
    }

    void SetupJumpVariables() {
        float timeToApex = player.maxJumpTime / 2;
        player.gravity = (-2 * player.maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        player.jumpSpeed = (2 * player.maxJumpHeight) / timeToApex;
    }

    public void HandleMovement() {
        if (SaveUtils.health <= 0) return;

        if (controller.height != 2 && rollTimer > rollLength) ResetHeight();

        if (!grounded && !coyoteTimeActive && jumps == 2) {
            StartCoroutine(CountCoyoteTime());
        } else if (grounded) {
            StopCoroutine(CountCoyoteTime());
            runningCoyoteTime = 0;
            coyoteTimeActive = false;
        }

        float m = 0.05f;
        if (new Vector3(movement.x, 0, movement.z).magnitude < m) {
            movement.x = Random.Range(-m, m);
            movement.z = Random.Range(-m, m);
        } 
        if (!knockedBack) controller.Move(movement * Time.deltaTime);

        ApplyGravity();

        if (!knockedBack) {
            if (OnSteepSlope()) {
                SteepSlopeMovement();
            } else {
                HandleJump();
                HandleWalk();
            }
            HandleMomentum();
            
        } else {
            player.knockbackHandler.HandleKnockback();
        }
        
        if (knockedBack == true && movement.y <= 0) knockedBack = !grounded;
        camTarget.transform.position = player.transform.position + (Vector3.up * player.cameraHeight); // move camera to player:
    }

    void ApplyGravity() {
        bool falling = movement.y <= 0;
        float gravity = !knockedBack ? player.gravity : player.gravity * 0.4f;
        if (grounded & falling) { //TODO Remove "falling" if it causes problems
            movement.y = player.groudedGravity;
        } else if (falling) {
            movement.y += gravity * player.fallMultiplier * player.vacuum.elements.jetGravity * Time.deltaTime;
        } else {
            movement.y += gravity * Time.deltaTime;
        }
        if (movement.y <= player.maxFallSpeed) movement.y = player.maxFallSpeed;
    }

    void HandleJump() {
        if (jumpPressed && (grounded || jumps > 0)) {
            if (jumps == 2) {
                movement.y = player.jumpSpeed;
                player.anim.animator.SetTrigger("jump");
            } else {
                movement.y = player.jumpSpeed * 0.75f;
                player.anim.animator.SetTrigger("doubleJump");
            }
            jumpPressed = false;
            jumps--;
        } else if (grounded && !jumpPressed) {
            player.anim.animator.SetTrigger("grounded");
            jumps = 2;
        }
    }

    IEnumerator CountCoyoteTime() {
        if (coyoteTimeActive) yield break;
        coyoteTimeActive = true;
        runningCoyoteTime = coyoteTime;
        while (runningCoyoteTime > 0 && jumps == 2) {
            runningCoyoteTime--;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        coyoteTimeActive = false;
        jumps = 1;
    }
    
    void HandleWalk() {
        if (rollingProgress > 0) return;

        float runMod = run ? 1.5f : 1;

        // dampen movement:
        if (moving) {
            if (grounded) {
                accel += player.accelSpeed;
            } else {
                accel += player.accelSpeed * .3f;
            }
            if (accel > 1) accel = 1;
            if (accel > player.inputReader.rawInput.magnitude) accel = player.inputReader.rawInput.magnitude;
            RotatePlayer();
        } else {
            accel -= player.decelSpeed;
            if (accel <= 0) accel = 0;
        }

        movement.x = player.inputReader.directionInput.x * accel * runMod;
        movement.z = player.inputReader.directionInput.y * accel * runMod;
        
        // Slow movement when vacuuming:
        if (player.vacuum.pull) {
            movement.x *= player.vacuumSpeedMod;
            movement.z *= player.vacuumSpeedMod;
        }

        // Movement in relation to the camera:
        movement = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * movement;
    }

    void HandleMomentum() {
        movement += momentum;
        if (grounded) {
            momentum = Vector3.ClampMagnitude(momentum, momentum.magnitude * 0.1f);
        } else {
            momentum = Vector3.ClampMagnitude(momentum, momentum.magnitude * 0.98f);
        }
        momentum.y *= 0.1f;
        if (momentum.magnitude <= 0.1) momentum = Vector3.zero;
    }

    public IEnumerator Roll() {
        rollTimer = 0;
        float startingSpeed = Mathf.Abs(movement.magnitude);
        controller.height = 1;
        controller.center = new Vector3(0, -0.5f, 0);

        while(rollTimer <= rollLength) {
            if (rollTimer == 0) player.anim.animator.SetTrigger("roll");
            if (rollTimer <= rollLength / 2) {
                // increase speed
                movement.z = rollSpeed * (rollTimer / (rollLength / 2));
                if (Mathf.Abs(movement.magnitude) < startingSpeed) movement.z = startingSpeed;
            } else {
                // decrease speed
                movement.z = rollSpeed * (1 - ((rollTimer - (rollLength / 2)) / (rollLength / 2)));
                if (movement.z <= rollEndSpeed) rollTimer = rollLength;
            }
            movement.x = player.inputReader.directionInput.x * 0.5f;
            movement = Quaternion.AngleAxis(player.model.transform.eulerAngles.y, Vector3.up) * movement;
            // RotatePlayer();

            yield return new WaitForSeconds(Time.deltaTime);
            rollTimer += Time.deltaTime;
            rollingProgress = rollTimer / rollLength;
        }
        rollingProgress = 0;
    }

    void ResetHeight() {
        if (!Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.up, 1.1f, ~(1 << 5))) {
            controller.height = 2;
            controller.center = Vector3.zero;
        }
    }
    
    public Vector3 GetTrueDirection() {
        return Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * new Vector3(player.inputReader.rawInput.x, 0, player.inputReader.rawInput.y);
    }

    void RotatePlayer() {
        if (player.vcamera.aiming) return;
        player.model.transform.forward = Vector3.Slerp(player.model.transform.forward, GetTrueDirection(), 20 * Time.deltaTime);
    }

    public IEnumerator PointPlayerAt(Vector3 direction) {
        if (autoTurningPlayer) yield break;
        autoTurningPlayer = true;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        while(Quaternion.Angle(targetRotation, player.model.transform.rotation) > 1f) {
            player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, targetRotation, 15 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        autoTurningPlayer = false;
    }

    bool GroundCheck() {
        // bool g = (controller.collisionFlags == CollisionFlags.Below
        //         || controller.collisionFlags == CollisionFlags.CollidedBelow
        //         || controller.isGrounded);

        bool g = Physics.CheckSphere(groundCheck.position, 0.3f, 1 << 0);

        // if (movement.y <= 0) {
            if (g && Physics.Raycast(groundCheck.position + player.up * 0.3f, -player.up, out groundCheckInfo, 15, 1 << 0, QueryTriggerInteraction.Ignore)) {
                groundNormal = groundCheckInfo.normal;
                // groundNormal = new Vector3(Mathf.Round(groundNormal.x * 10) * 0.1f, Mathf.Round(groundNormal.y * 10) * 0.1f, Mathf.Round(groundNormal.z * 10) * 0.1f);
            }

            // Physics.Raycast(left.position + Vector3.up * 0.3f, -player.up, out l, 0.6f, 1 << 0);
            // Physics.Raycast(back.position + Vector3.up * 0.3f, -player.up, out b, 0.6f, 1 << 0);
            // Physics.Raycast(front.position + Vector3.up * 0.3f, -player.up, out f, 0.6f, 1 << 0);
            // Physics.Raycast(right.position + Vector3.up * 0.3f, -player.up, out r, 0.6f, 1 << 0);
            // groundNormal = (Vector3.Cross(b.point - Vector3.up, l.point - Vector3.up) +
            //                 Vector3.Cross(l.point - Vector3.up, f.point - Vector3.up) +
            //                 Vector3.Cross(f.point - Vector3.up, r.point - Vector3.up) +
            //                 Vector3.Cross(r.point - Vector3.up, b.point - Vector3.up)
            //                 ).normalized;
        // } else {
        //     groundNormal = Vector3.zero;
        // }
        
        // player.transform.up = Vector3.Lerp(player.up, (Vector3.Cross(b.point - Vector3.up, l.point - Vector3.up) +
        //                         Vector3.Cross(l.point - Vector3.up, f.point - Vector3.up) +
        //                         Vector3.Cross(f.point - Vector3.up, r.point - Vector3.up) +
        //                         Vector3.Cross(r.point - Vector3.up, b.point - Vector3.up)
        //                         ).normalized, 0.9f * Time.deltaTime);

        if (g) {
            notGroundedCounter = 0;
        } else if (!g) {
            notGroundedCounter++;
        }

        return notGroundedCounter < 1; // increase if not enough
    }

    bool OnSteepSlope() {
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);
        return slopeAngle > controller.slopeLimit;
    }

    void SteepSlopeMovement() {
        Vector3 slopeDirection = Vector3.up - groundNormal * Vector3.Dot (Vector3.up, groundNormal);
        movement = slopeDirection * -slideSpeed;
    }

    // void OnDrawGizmos() {
    //     Gizmos.color = grounded ? Color.green : Color.red;
    //     if (knockedBack) Gizmos.DrawSphere(new Vector3(player.transform.position.x, controller.bounds.max.y, player.transform.position.z), 0.5f);
    //     Gizmos.DrawSphere(groundCheck.position, 0.3f);
    // }
}
