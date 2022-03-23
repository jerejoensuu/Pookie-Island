using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    internal CharacterController controller;
    GameObject camTarget;

    public Vector3 movement;
    private float accel = 0;

    internal bool moving, jumping, jumpPressed, autoTurningPlayer;
    public float rollingProgress = 0;
    public float rollSpeed = 25;
    public float rollEndSpeed = 5;
    float runningRollSpeed = 0;
    public float rollTimer;
    public float rollLength = 1;

    public bool knockedBack = false;
    int jumps = 2;

    // GroundCheck:
    internal RaycastHit rayHit;
    internal Vector3 center, size;
    /* Coyote time WIP
    public int coyoteTime = 30;
    [SerializeField] int runningCoyoteTime = 0;
    bool coyoteTimeActive = false;
    */
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

        /* Coyote time WIP
        if (!grounded && !coyoteTimeActive && jumps == 2) {
            StartCoroutine(CountCoyoteTime());
        } else if (grounded) {
            StopCoroutine(CountCoyoteTime());
            runningCoyoteTime = 0;
            coyoteTimeActive = false;
        }
        if (coyoteTimeActive) Debug.Log(runningCoyoteTime);
        */

        if (!grounded && jumps > 1) jumps = 1;

        if (!knockedBack) controller.Move(movement * Time.deltaTime);

        ApplyGravity();

        if (!knockedBack) {
            HandleJump();
            HandleWalk();
        } else {
            player.knockbackHandler.HandleKnockback();
        }
        
        if (knockedBack == true) knockedBack = !grounded;
        camTarget.transform.position = player.transform.position + (Vector3.up * player.cameraHeight); // move camera to player:
    }

    void ApplyGravity() {
        bool falling = movement.y <= 0;
        float gravity = !knockedBack ? player.gravity : player.gravity * 0.25f;
        if (grounded & falling) { //TODO Remove "falling" if it causes problems
            movement.y = player.groudedGravity;
        } else if (falling) {
            movement.y += gravity * player.fallMultiplier * Time.deltaTime;
        } else {
            movement.y += gravity * Time.deltaTime;
        }
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

    /* Coyote time WIP
    IEnumerator CountCoyoteTime() {
        if (coyoteTimeActive) yield break;
        coyoteTimeActive = true;
        runningCoyoteTime = coyoteTime;
        while (runningCoyoteTime > 0) {
            runningCoyoteTime--;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        coyoteTimeActive = false;
    }
    */
    
    void HandleWalk() {
        if (rollingProgress > 0) return;

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

        movement.x = player.inputReader.directionInput.x * accel;
        movement.z = player.inputReader.directionInput.y * accel;
        
        // Slow movement when vacuuming:
        if (player.vacuum.pull) {
            movement.x *= player.vacuumSpeedMod;
            movement.z *= player.vacuumSpeedMod;
        }

        // Movement in relation to the camera:
        movement = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * movement;
    }

    public IEnumerator Roll() {
        rollTimer = 0;
        float startingSpeed = Mathf.Abs(movement.magnitude);

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
            movement.x = 0;
            movement = Quaternion.AngleAxis(player.model.transform.eulerAngles.y, Vector3.up) * movement;

            yield return new WaitForSeconds(Time.deltaTime);
            rollTimer += Time.deltaTime;
            rollingProgress = rollTimer / rollLength;
        }
        rollingProgress = 0;
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
        bool g = (controller.collisionFlags == CollisionFlags.Below
                || controller.collisionFlags == CollisionFlags.CollidedBelow
                || controller.isGrounded);

        if (g) {
            notGroundedCounter = 0;
        } else if (!g) {
            notGroundedCounter++;
        }

        return notGroundedCounter < 1; // increase if not enough
    }
}
