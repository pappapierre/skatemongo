using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Debugging ------------------------------")]
    public bool DebugKeyboard = false;

    [Header("UI BUTTONS ------------------------------")]
    public GameObject btn_Push;
    public GameObject btn_Brake;
    public GameObject btn_Pop;
    public GameObject btn_Stickit;
    public GameObject btn_KickFlip;
    public GameObject btn_HeelFlip;
    public GameObject btn_L_LeanLeft;
    public GameObject btn_L_LeanRight;
    public GameObject btn_R_LeanLeft;
    public GameObject btn_R_LeanRight;

    [Header("Camera Rig ------------------------------")]
    public GameObject CameraRig;
    public float predVelPow = 0.2f;
    public float predictionAchievement = 1f;

    [Header("physics objects -------------------------")]
    public Rigidbody rb;
    public float groundedDrag;
    public float airDrag;

    [Header("Board Rig -------------------------------")]
    public GameObject Board;
    public float centerOfMassOffset = 0.4f;
    public float comMagPower = 0.2f;
    public GameObject Deck;
    public GameObject TruckFront;
    public GameObject TruckBack;
    public GameObject[] wheels;

    [Header("Board effects ----------------------------")]
    public TrailRenderer[] wheelTrails;

    [Header("trickHotspots ----------------------------------")]
    public Collider boardCenter;
    public Collider boardNose;
    public Collider truckNose;
    public Collider boardTail;
    public Collider truckTail;

    [Header("physics forces --------------------------")]
    public float PushOffPower = 20f;
    public float BreakingPower = 0.994f;
    public float leanPower = 0.2f;
    public float leanVeloPower = 300;
    public float leaningDirectionAchieve = 2f;
    public float leaningDirectionDecay = 0.94f;

    public float leaningDir = 0;
    public float leaningDirection = 0;
    public float leaningDirectionLatent = 0;

    [Header("physics tweakers --------------------------")]
    public float forceLandingPower = 10f;

    [Header("tricks ----------------------------------")]
    public bool Grounded = false;
    public int wheelsOnGround = 0;
    public float popPower = 15f;
    public float spinMin = 0.3f;
    public float spinPowerAir = 0.9f;
    public Vector3 velocity;

    [Header("READONLY ----------------------------------")]
    //bools ------------------------------------------
    public bool pushed = false;
    public bool braking = false;
    public bool leaning = false;
    public bool forceLanding = false;
    public bool tricking = false;
    public bool fakey = false;

    Ray ray;
    RaycastHit hit;
    RaycastHit[] hits;

    [Header("READONLY TIME VALS----------------------------------")]
    public float timeSinceLastPush = 1;
    public float timeSinceTrick = 0;

    #region Unity Methods ------------------------------------------------------------------------------
    void Update() {
        //keep grounded state updated at all times - what if i'm popping ONTO something? 
        CheckKeyboardInputs();
        CheckGroundedState();

        KeepUpdated();
        KeepAnimatorUpdated();
        KeepButtonsUpdated();

        //if grounded, normal controls apply
        // push / brake / lean left / lean right
        if (Grounded) {
            //braking
            if (braking) {
                Action_Brake();
                BrakeEffects(true);
            } else {
                BrakeEffects(false);
            }

            //leaning
            if (leaning) {
                Action_Leaning(1);
            } else {
                leaningDirection *= leaningDirectionDecay;
            }
        } else {
            if (leaning) {
                Action_Leaning(spinPowerAir);
            } else {
                leaningDirection *= leaningDirectionDecay;
            }

            if (forceLanding)
                rb.AddForce(-Vector3.up * forceLandingPower);
        }

        //APPLY unapplied properties
        ApplyLeaning();
        ApplyBoardTransforms();

        //cache current velocity and move camera
        velocity = rb.velocity;
        CameraRig.transform.position = Vector3.Lerp(CameraRig.transform.position, transform.position + new Vector3(velocity.x, 0, velocity.z) * predVelPow, Time.deltaTime * predictionAchievement);
    }
    #endregion

    #region PC debugging -------------------------------------------------------------------------------
    //keyboard debugs for PC testing
    void CheckKeyboardInputs() {
        if (!DebugKeyboard || Application.isMobilePlatform)
            return;

        //RESET
        if (Input.GetKeyDown(KeyCode.R)) {
            Input_ResetBoard();
        }

        //FAKEY toggle
        if (Input.GetKeyDown(KeyCode.F)) {
            fakey = !fakey;
        }

        //PUSH
        if (Input.GetKeyDown(KeyCode.W))
            Input_Push();

        //BRAKE
        if (Input.GetKeyDown(KeyCode.S))
            Input_Brake(true);

        if (Input.GetKeyUp(KeyCode.S))
            Input_Brake(false);

        //LEAN
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            if (Input.GetKey(KeyCode.A))
                Input_LeanLeft(true);

            if (Input.GetKey(KeyCode.D))
                Input_LeanRight(true);
        } else {
            if (!Application.isMobilePlatform) {
                Input_LeanRight(false);
                Input_LeanLeft(false);
            }
        }

        //FORCELAND
        if (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Delete))
            Input_ForceLand(true);

        if (Input.GetKeyUp(KeyCode.KeypadPeriod) || Input.GetKeyUp(KeyCode.Delete))
            Input_ForceLand(false);

        // POP!
        if (Input.GetKeyDown(KeyCode.Keypad0))
            Input_Pop();

        // FLIPS!
        if (Input.GetKeyDown(KeyCode.Keypad1))
            Input_Kickflip();

        if (Input.GetKeyDown(KeyCode.Keypad2))
            Input_Heelflip();
    }
    #endregion

    #region UI dynamism --------------------------------------------------------------------------------
    void KeepButtonsUpdated() {
        if (Grounded && !tricking) {
            btn_Push.SetActive(true);
            btn_Brake.SetActive(true);
            btn_Pop.SetActive(true);

            btn_Stickit.SetActive(false);
            btn_KickFlip.SetActive(false);
            btn_HeelFlip.SetActive(false);

            btn_L_LeanLeft.SetActive(false);
            btn_L_LeanRight.SetActive(false);
            btn_R_LeanLeft.SetActive(true);
            btn_R_LeanRight.SetActive(true);
        } else {
            btn_Push.SetActive(false);
            btn_Brake.SetActive(false);
            btn_Pop.SetActive(false);

            btn_Stickit.SetActive(true);
            btn_KickFlip.SetActive(true);
            btn_HeelFlip.SetActive(true);

            btn_L_LeanLeft.SetActive(true);
            btn_L_LeanRight.SetActive(true);
            btn_R_LeanLeft.SetActive(false);
            btn_R_LeanRight.SetActive(false);
        }
    }
    #endregion

    #region input methods ------------------------------------------------------------------------------
    //reset board instantly on the spot
    public void Input_ResetBoard() {
        transform.position = transform.position + Vector3.up * 2f;
        transform.rotation = Quaternion.identity;
    }

    //IMPULSE push!
    public void Input_Push() {
        Action_Push();
    }

    //STATE - set BRAKING
    public void Input_Brake(bool val) {
        braking = val;
    }

    //STATE - set leaning LEFT
    public void Input_LeanLeft(bool val) {
        leaning = val;
        if (val)
            leaningDir = -1;
        else
            leaningDir = 0;
    }

    //STATE - set leaning RIGHT
    public void Input_LeanRight(bool val) {
        leaning = val;
        if (val)
            leaningDir = +1;
        else
            leaningDir = 0;
    }

    //IMPULSE pop!
    public void Input_Pop() {
        DoTrick("Pop");
    }

    //IMPULSE kickflip!
    public void Input_Kickflip() {
        DoTrick("Kickflip");
    }

    //IMPULSE heelflip!
    public void Input_Heelflip() {
        DoTrick("Heelflip");
    }

    //STATE forceLanding
    public void Input_ForceLand(bool val) {
        forceLanding = val;
    }
    #endregion

    #region actioning forces ---------------------------------------------------------------------------
    //push - impulse action
    void Action_Push() {
        if (!Grounded)
            return;

        GetComponent<Animator>().SetTrigger("Push");
        rb.AddForce(transform.forward * PushOffPower * timeSinceLastPush * (fakey ? -1 : 1) );
        //consecutive pushes deminish pushing power
        timeSinceLastPush *= 0.5f;
    }

    //brake - state action
    void Action_Brake() {
        rb.velocity *= BreakingPower;
    }

    //lean - state action
    void Action_Leaning(float leaningPower) {
        leaningDirection += leaningDir * Time.deltaTime * leaningDirectionAchieve * leaningPower ;
    }
    #endregion

    #region force and transform application ------------------------------------------------------------
    void KeepUpdated() {
        if (timeSinceLastPush < 1)
            timeSinceLastPush += Time.deltaTime;

        if (timeSinceTrick < 1)
            timeSinceTrick += Time.deltaTime;

        leaningDirectionLatent = Mathf.Lerp(leaningDirectionLatent, leaningDirection, Time.deltaTime * 4f);
    }

    void KeepAnimatorUpdated() {
        GetComponent<Animator>().SetBool("Grounded", Grounded);
        GetComponent<Animator>().SetBool("Braking", braking);

        if (GetComponent<Animator>().GetBool("Fakey") != fakey)
            GetComponent<Animator>().SetBool("Fakey", fakey);
    }

    //Check for 4 wheels on ground
    void CheckGroundedState() {
        //assume the worst:
        Grounded = false;
        wheelsOnGround = 0;

        //now validate
        foreach (GameObject each in wheels) {
            ray = new Ray(each.transform.position, -Vector3.up);
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag != "BoardComponent" && hit.collider.tag != "BoardHotspot") {
                    if ((ray.origin - hit.point).magnitude < 0.2f)
                        wheelsOnGround++;
                }
            }
        }

        //atleast 2 wheels on ground is grounded.
        if (wheelsOnGround >= 2 && timeSinceTrick > 0.2f) {
            Grounded = true;
            ApplyLandingQuality();
            tricking = false;
        }

        //set the rigidbody drag to relevant air/ground vals.
        if (Grounded) {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.white);
            if (rb.drag != groundedDrag)
                rb.drag = groundedDrag;
        } else {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red);
            if (rb.drag != airDrag)
                rb.drag = airDrag;
        }
    }
    #endregion

    #region Landing Quality
    void ApplyLandingQuality() {
        if (tricking) {
            if (CheckQualityOfLanding()) {
                RestartLandingQualityTimer(false);
            } else {
                RestartLandingQualityTimer(true);
            }
        }
    }

    bool CheckQualityOfLanding() {
        //float angleFromLaunch = Quaternion.Angle(orientationAtPop, transform.rotation);
        //Debug.DrawLine(transform.position, transform.position + orientationAtPop * Vector3.forward * 2, Color.blue, 2f);
        //Debug.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward * 2, Color.magenta, 2f);

        //print(angleFromLaunch);
        //if (angleFromLaunch > 150)
        //    fakey = !fakey;

        if ( (orientationAtPop.eulerAngles.y % 180) < (transform.rotation.eulerAngles.y % 180) - 15f ||
            (orientationAtPop.eulerAngles.y % 180) > (transform.rotation.eulerAngles.y % 180) + 15f) {
            return true;
        } else {
            return false;
        }
    }

    [Header("Landing stuff ------------------------------")]
    public Text LandingQualityText;
    public Color colorSolidLanding;
    public Color colorSketchyLanding;
    void RestartLandingQualityTimer(bool val) {
        if (val) {
            LandingQualityText.text = "<size=32> -</size> solid <size=32> -</size>";
            LandingQualityText.color = colorSolidLanding;
        } else {
            LandingQualityText.text = "<size=32> -</size> sketchy <size=32> -</size>";
            LandingQualityText.color = colorSketchyLanding;
        }

        if (LandingQualityTimerRoutine != null)
            StopCoroutine(LandingQualityTimerRoutine);

        LandingQualityTimerRoutine = StartCoroutine(LandingQualityTimer());
    }

    Coroutine LandingQualityTimerRoutine;
    IEnumerator LandingQualityTimer() {
        LandingQualityText.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.4f);

        var col = LandingQualityText.color;
        while (col.a > 0) {
            col.a -= Time.deltaTime * 3f;
            LandingQualityText.color = col;
            LandingQualityText.transform.localScale += Vector3.one * Time.deltaTime;
            yield return null;
        }

        LandingQualityText.color = Color.clear;
    }
    #endregion

    #region force application
    //leaning - visuals and forces
    void ApplyLeaning() {
        // * (fakey ? -1 : 1)
        leaningDirection = Mathf.Clamp(leaningDirection, -1, 1);
        rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + (leaningDirection * Vector3.up) * leanPower);

        if(Grounded)
        rb.AddForce((leaningDirection * transform.right) * leanVeloPower * velocity.magnitude);
    }

    //board transform runtime - visuals
    void ApplyBoardTransforms() {
        Deck.transform.localRotation = Quaternion.Euler(0, 0, -leaningDirection * 15f);
        TruckFront.transform.localRotation = Quaternion.Euler(0, leaningDirection * 15f, 0);
        TruckBack.transform.localRotation = Quaternion.Euler(0, -leaningDirection * 15f, 0);

        Board.transform.localPosition = Vector3.Lerp(Board.transform.localPosition,
            -Mathf.Clamp(rb.velocity.magnitude * comMagPower, -0.45f, 0.45f) * centerOfMassOffset * Vector3.forward,
            Time.deltaTime * 8f);
    }
    #endregion

    #region tricks and pops ----------------------------------------------------------------------------

    Quaternion orientationAtPop;

    //do trick by name
    void DoTrick(string name) {

        orientationAtPop = transform.rotation;
        timeSinceTrick = 0;

        switch (name) {
            case ("Pop"):
                tricking = true;

                //apply POP force if grounded
                if (Grounded)
                    rb.velocity += Vector3.up * popPower;

                //reset rotation to flat - HACK
                rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z);
                
                //trigger correct animation
                GetComponent<Animator>().SetTrigger("Pop");

                //prepare for landing
                StartCoroutine(Land(0.5f));

                //are we turning frontside/backside? 
                CheckAngular();
                break;
            case ("Kickflip"):
                tricking = true;
                if (!Grounded) {
                    //apply POP force if grounded
                    if (Grounded)
                    rb.velocity += Vector3.up * popPower;

                    //reset rotation to flat - HACK
                    rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z);

                    //trigger correct animation
                    GetComponent<Animator>().SetTrigger("Kickflip");

                    //prepare for landing
                    StartCoroutine(Land(0.5f));

                    //are we turning frontside/backside? 
                    CheckAngular();
                }
                break;
            case ("Heelflip"):
                tricking = true;
                if (!Grounded) {
                    //apply POP force if grounded
                    if (Grounded)
                        rb.velocity += Vector3.up * popPower;

                    //reset rotation to flat - HACK
                    rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z);

                    //trigger correct animation
                    GetComponent<Animator>().SetTrigger("Heelflip");

                    //prepare for landing
                    StartCoroutine(Land(0.5f));

                    //are we turning frontside/backside? 
                    CheckAngular();
                }
                break;
        }
    }

    //frontside / backside spin
    void CheckAngular() {
        if (leaningDirection > spinMin || leaningDirection < -spinMin) {
            rb.angularVelocity = leaningDirection * Vector3.up * 15f;
        }
    }

    //downward "feet on board" force
    IEnumerator Land(float t) {
        yield return new WaitForSeconds(t);
        rb.AddForce(-Vector3.up * 50);
        leaningDirectionLatent = leaningDirection;
    }
    #endregion

    #region FX methods ---------------------------------------------------------------------------------
    float trailTime = 0;
    void BrakeEffects(bool state) {
        //if (state) {
        //    trailTime = 1;
        //} else {
        //    trailTime = 0;
        //}
        //foreach (TrailRenderer each in wheelTrails) {
        //    each.time = Mathf.Lerp(each.time, trailTime, Time.deltaTime * 6f);
        //}
    }

    #endregion

}
