using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement runtime;
    public static UnityEvent onLanded = new UnityEvent();

    static Dictionary<GameObject, bool> lockMovementRegistry = new Dictionary<GameObject, bool>();
    public static bool IsMovementLocked()
    {
        foreach(GameObject gm in lockMovementRegistry.Keys)
        {
            lockMovementRegistry.TryGetValue(gm, out bool value);
            if (value)
                return true;
        }
        return false;
    }
    public static void LockMovement(GameObject instance, bool value)
    {
        if (lockMovementRegistry.ContainsKey(instance))
            lockMovementRegistry[instance] = value;
        else
            lockMovementRegistry.Add(instance, value);

        if (IsMovementLocked())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            runtime.rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            runtime.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    static Dictionary<GameObject, bool> stopMovementRegistry = new Dictionary<GameObject, bool>();
    public static bool IsMovementStopped()
    {
        foreach (GameObject gm in stopMovementRegistry.Keys)
        {
            stopMovementRegistry.TryGetValue(gm, out bool value);
            if (value)
                return true;
        }
        return false;
    }
    public static void StopMovement(GameObject instance, bool value)
    {
        if (stopMovementRegistry.ContainsKey(instance))
            stopMovementRegistry[instance] = value;
        else
            stopMovementRegistry.Add(instance, value);

        if (IsMovementStopped())
        {
            runtime.rb.isKinematic = true;
        }
        else
        {
            runtime.rb.isKinematic = false;
        }
    }

    [Header("Movement")]
    public float speed = 3f;
    public float diagonalMult = .75f;
    public float weightSlowMult = 0.01f;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity = 1f;
    public Vector2 xClamp;
    public Vector2 yClamp;

    float xRotation;
    float yRotation;

    [Header("Exhaustion Mults")]
    public float walkExhaustMult;
    public float runExhaustMult;
    public float jumpExhaustMult;
    public float crouchExhaustMult;

    [Header("Jump")]
    public Vector3 groundCheckPos;
    public Vector3 groundCheckPosCrouched;
    public float groundCheckRadius;
    public float jumpHeight;
    public List<Collider> groundeCheckExcludes = new List<Collider>();

    bool grounded;
    public Rigidbody rb { get; private set; }

    [Header("Running")]
    public float runSpeedMult;
    public float fovRunIncrease;

    bool running;
    float baseFov;

    [Header("Crouching")]
    public CapsuleCollider col;
    public float crouchSpeedMult;
    public float crouchHeightMult;
    public int crouchTransitionSpeed = 100;

    public bool crouching { get; private set; }
    float baseCamHeight;

    private void Awake()
    {
        runtime = this;

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (cam.transform.eulerAngles.x > xClamp.y)
            xRotation = cam.transform.eulerAngles.x - 360;
        else
            xRotation = cam.transform.eulerAngles.x;
        yRotation = transform.eulerAngles.y;

        baseFov = cam.fieldOfView;
        baseCamHeight = cam.transform.localPosition.y;

        LockMovement(gameObject, false);
        StopMovement(gameObject, false);

        SurvivalAttributes.runtime.multipliersThirst.Add("movement", 0);
        SurvivalAttributes.runtime.multipliersHunger.Add("movement", 0);
        SurvivalAttributes.runtime.multipliersFatigue.Add("movement", 0);
    }

    bool hasJumped = false;
    private void FixedUpdate()
    {
        if (IsMovementLocked() || IsMovementStopped())
            return;

        float speedAdjust = 1;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1 && Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        {
            speedAdjust = diagonalMult;
        }

        float horizontal = Input.GetAxis("Horizontal") * GetSpeed() * speedAdjust;
        float vertical = Input.GetAxis("Vertical") * GetSpeed() * speedAdjust;
        transform.Translate(horizontal, 0f, 0f);
        transform.Translate(0f, 0f, vertical);

        float mult =
            Convert.ToInt32(!running && !crouching && IsGrounded() && horizontal != 0 && vertical != 0) * walkExhaustMult +
            Convert.ToInt32(running) * runExhaustMult + 
            Convert.ToInt32(crouching) * crouchExhaustMult + 
            Convert.ToInt32(!IsGrounded()) * jumpExhaustMult;
        SurvivalAttributes.runtime.multipliersThirst["movement"] = mult;
        SurvivalAttributes.runtime.multipliersHunger["movement"] = mult;
        SurvivalAttributes.runtime.multipliersFatigue["movement"] = mult;
    }

    private void Update()
    {
        if (IsMovementLocked())
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        if (xClamp != Vector2.zero)
            xRotation = Mathf.Clamp(xRotation, xClamp.x, xClamp.y);
        if (yClamp != Vector2.zero)
            yRotation = Mathf.Clamp(yRotation, yClamp.x, yClamp.y);

        transform.eulerAngles = new Vector3(0f, yRotation, 0f);
        if (!animating)
            cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0f);

        if (hasJumped && IsGrounded())
        {
            hasJumped = false;
            onLanded.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetCrouching(!crouching);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (running == false && Input.GetAxis("Vertical") > 0)
            {
                SetRunning(!running);
            }
            else if (running == true)
            {
                SetRunning(!running);
            }
        }
        if (!Input.GetButton("Vertical"))
        {
            SetRunning(false);
        }

        if (running) FaunaBehavior.spookEvent.Invoke(false);
    }

    async void Jump()
    {
        Vector3 jumpforce = new Vector3(0, jumpHeight, 0);
        rb.linearVelocity = jumpforce;
        await Task.Delay(500);
        hasJumped = true;
    }

    public float GetSpeed()
    {
        float weightMult = Mathf.Clamp(1 - (weightSlowMult * PlayerInventory.runtime.GetTotalWeight()), 0.1f, 1);
        float vertSpeed = speed * (running ? runSpeedMult : 1);
        if (crouching)
            vertSpeed = speed * crouchSpeedMult;
        return vertSpeed * weightMult * Time.deltaTime;
    }

    public List<Collider> GetOverlaps()
    {
        return crouching ?
            Physics.OverlapSphere(transform.position + groundCheckPosCrouched, groundCheckRadius).Where(o => !groundeCheckExcludes.Contains(o)).ToList() :
            Physics.OverlapSphere(transform.position + groundCheckPos, groundCheckRadius).Where(o => !groundeCheckExcludes.Contains(o)).ToList();
    }
    public bool IsGrounded()
    {
        if (GetOverlaps().Count > 0)
            return true;
        return false;
    }

    int targetFov;
    void SetRunning(bool value)
    {
        running = value;
        if (running)
        {
            SetCrouching(false);
            ZoomFov((int)cam.fieldOfView, (int)baseFov + (int)fovRunIncrease);
        }
        else
        {
            ZoomFov((int)cam.fieldOfView, (int)baseFov);
        }
    }
    async void ZoomFov(int start, int target)
    {
        targetFov = target;

        int step = target - start >= 0 ? 1 : -1;
        for(int i = start; i != target; i += step)
        {
            if (targetFov != target)
                return;

            cam.fieldOfView = i;
            await Task.Delay(10);
        }
        cam.fieldOfView = target;
    }

    float targetHeight;
    void SetCrouching(bool value)
    {
        crouching = value;
        if (crouching)
        {
            SetRunning(false);
            PlayerHud.runtime.SetCrouch(crouching);
            CrouchTransition(cam.transform.localPosition.y, baseCamHeight * crouchHeightMult);
        }
        else
        {
            PlayerHud.runtime.SetCrouch(crouching);
            CrouchTransition(cam.transform.localPosition.y, baseCamHeight);
        }
    }
    async void CrouchTransition(float start, float target)
    {
        targetHeight = target;

        for (int i = 0; i <= crouchTransitionSpeed; i++)
        {
            if (targetHeight != target)
                return;

            Vector3 startPos = new Vector3(cam.transform.localPosition.x, start, cam.transform.localPosition.z);
            Vector3 targetPos = new Vector3(cam.transform.localPosition.x, target, cam.transform.localPosition.z);
            cam.transform.localPosition = Vector3.Lerp(startPos, targetPos, i / (float)crouchTransitionSpeed);
            float height = Mathf.Lerp(start, target, i / (float)crouchTransitionSpeed);
            col.height = height;

            await Task.Delay(10);
        }
        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, target, cam.transform.localPosition.z);
        col.height = target;
    }

    bool animating;
    public async void RecoilCamera(float rotate, float pushback)
    {
        animating = true;
        Vector3 startRotation = cam.transform.localEulerAngles;
        Vector3 startPos = cam.transform.localPosition;

        float recoilTime = 0.025f;
        float recoveryTime = 0.25f;

        for (float i = 0; i < recoilTime; i += 0.01f)
        {
            cam.transform.localEulerAngles = new Vector3(Mathf.Lerp(startRotation.x, startRotation.x - rotate, EaseOut(i / recoilTime)), startRotation.y, startRotation.z);
            cam.transform.localPosition = new Vector3(startPos.x, startPos.y, Mathf.Lerp(startPos.z, startPos.z - pushback, EaseOut(i / recoilTime)));
            await Task.Delay(10);
        }
        for (float i = 0; i < recoveryTime; i += 0.01f)
        {
            cam.transform.localEulerAngles = new Vector3(Mathf.SmoothStep(startRotation.x - rotate, startRotation.x, i / recoveryTime), startRotation.y, startRotation.z);
            cam.transform.localPosition = new Vector3(startPos.x, startPos.y, Mathf.SmoothStep(startPos.z - pushback, startPos.z, i / recoveryTime));
            await Task.Delay(10);
        }
        cam.transform.localEulerAngles = startRotation;
        cam.transform.localPosition = startPos;
        animating = false;
    }
    float EaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + groundCheckPos, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + groundCheckPosCrouched, groundCheckRadius);
    }
}