using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    private Transform playerModel;
    PlayerCamera playerCamera;

    [Header("Player Attributes")]
    [SerializeField] bool toggleChargedShot = true;
    [SerializeField] float speed = 10f;
    [SerializeField] float forwardSpeed = .5f;
    [SerializeField] float maxSpeed = 30f;
    [SerializeField] float xRange = 6f;
    [SerializeField] float yRange = 5f;
    [SerializeField] float controlPitchFactor = -40f;
    [SerializeField] int steepTurnFactor = 2;

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem[] guns;
    [SerializeField] ParticleSystem barrelRoll;
    [SerializeField] ParticleSystem brake;
    [SerializeField] ParticleSystem boost;
    [SerializeField] ParticleSystem trail;
    [SerializeField] ParticleSystem chargeLaser;
    [SerializeField] ParticleSystem afterBurner;

    [Header("Sounds")]
    [SerializeField] AudioSource myAudioSource;
    [SerializeField] AudioClip laserSound;
    [SerializeField] AudioClip arwingAmbientSound;
    [SerializeField] AudioClip boostSound;
    [SerializeField] AudioClip brakeSound;
    [SerializeField] AudioClip itemCollect;
    [SerializeField] AudioClip chargeLaserSound;
    [SerializeField] AudioClip damageSound;

    [Header("Camera Porperties")]
    [SerializeField] float cameraSpeed = 1f;
    [SerializeField] float frequencyGain = 10f;
    [SerializeField] float amplitudeGain = 10f;

    [Header("GameObjects")]
    [SerializeField] GameObject normalReticule;
    [SerializeField] GameObject chargedReticule;
    [SerializeField] GameObject targetLockedReticule;
    [SerializeField] GameObject chargedProjectile;
    [SerializeField] ChargedRectile chargedRectileObject;

    SpriteRenderer SmallAimReticuleRenderer;

    Score score;
    Boost boostUi;

    bool targetLocked = false;
    bool isBoosting = false;
    bool isBraking = false;


    float horizontalInput, verticalInput;

    float maxRollLeft = 90f;
    float maxRollRight = -90f;

    bool sharpBank = false;
    bool isRollReturningToZero = false;
    float pitch;
    float yaw;
    float roll;
    bool firstButtonPressed = false;
    bool reset;
    float timeOfFirstButton;

    float chargeLaserTimer;
    float holdChargeTimer = 1f;

    bool laserCharged = false;

    ParticleSystem[] chargedParticleEffects;

    void Start()
    {
        SmallAimReticuleRenderer = normalReticule.GetComponent<SpriteRenderer>();
        playerModel = transform;
        playerCamera = FindObjectOfType<PlayerCamera>();
        chargedParticleEffects = chargedProjectile.GetComponents<ParticleSystem>();

        boostUi = FindObjectOfType<Boost>();
        score = FindObjectOfType<Score>();

    }

    void Update()
    {
        horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        verticalInput = CrossPlatformInputManager.GetAxis("Vertical");
        Brake();

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            boost.gameObject.SetActive(true);
            boost.Play();
        }

        if (CrossPlatformInputManager.GetButton("Jump"))
        {
            isBoosting = true;
        }

        if (CrossPlatformInputManager.GetButtonUp("Jump"))
        {
            isBoosting = false;
        }

        if (CrossPlatformInputManager.GetButtonDown("Brake"))
        {
            isBraking = true;
        }

        if (CrossPlatformInputManager.GetButtonUp("Brake"))
        {
            isBraking = false;
            afterBurner.gameObject.SetActive(true);
        }

        if (CrossPlatformInputManager.GetButton("Fire2") || CrossPlatformInputManager.GetButton("Fire3"))
        {
            int dir = Input.GetButton("Fire2") ? -1 : 1;
            isRollReturningToZero = false;
            Rotate90Degrees(dir);
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire2") || CrossPlatformInputManager.GetButtonUp("Fire3"))
        {
            isRollReturningToZero = true;
        }
        ReturnRollToZero();
        ApplyBoost();
        BarrelRollRight();
        BarrelRollLeft();
        Controls(horizontalInput, verticalInput);
        ProcessRotation();
        RotationLookForGuns(horizontalInput, verticalInput, speed);
        ProcessFiring();
    }

    void RotationLookForGuns(float h, float v, float speed)
    {
        var centerOfReticule = SmallAimReticuleRenderer.bounds.center;
        guns[0].transform.LookAt(new Vector3(
           centerOfReticule.x - 0.9f,
           centerOfReticule.y,
           centerOfReticule.z
           ));
        guns[1].transform.LookAt(new Vector3(
            centerOfReticule.x + 0.9f,
            centerOfReticule.y,
            centerOfReticule.z
            ));

    }

    void HorizontalLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }

    void ApplyBoost()
    {
        if (isBoosting)
        {
            boostUi.UpdateBoostAmount(-0.3f);
            forwardSpeed = forwardSpeed * 3;
        }
        else
        {
            forwardSpeed = .5f;
            boostUi.UpdateBoostAmount(0.1f);
        }
       
    }

    void Brake()
    {
        if (isBraking)
        {
            forwardSpeed = forwardSpeed / 3;
            boostUi.UpdateBoostAmount(-0.3f);
            afterBurner.gameObject.SetActive(false);
            //playerCamera.BrakeCamera();
        }
        else
        {
            forwardSpeed = .5f;
            boostUi.UpdateBoostAmount(0.1f);
        }
    }

    IEnumerator ProcessShake()
    {
        playerCamera.Noise(amplitudeGain, frequencyGain);
        transform.position = new Vector3(transform.position.x + .3f, transform.position.y + 1f, transform.position.z);
        yield return new WaitForSeconds(0.5f);
        playerCamera.Noise(0, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            StartCoroutine("ProcessShake");
            AudioSource.PlayClipAtPoint(damageSound,Camera.main.transform.position);
        }
        
    }

    

    void ProcessRotation()
    {

        pitch = verticalInput * controlPitchFactor; // down is positive, up negative
        yaw = horizontalInput * controlPitchFactor * -1;
        if (!sharpBank)
        {
            Mathf.Clamp(roll = horizontalInput * controlPitchFactor, maxRollRight, maxRollLeft); // left is positive, right negative
        }

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll); ; //pitch, yaw, roll
    }

    void Controls(float h, float v)
    {
        transform.localPosition += new Vector3(h, v, forwardSpeed) * speed * Time.deltaTime;
        ClampPosition();
    }

    void Rotate90Degrees(int dir)
    {
        sharpBank = true;
        if (dir < 0)
        {
            if (roll > maxRollRight)
            {
                roll -= 1.7f;
            }
        }
        else
        {
            if (roll < maxRollLeft)
            {
                roll += 1.7f;
            }
        }
       
        // horizontalInput += 0.5f;
    }

    void ReturnRollToZero()
    {
        if (isRollReturningToZero)
        {
            print(roll);
            if (Mathf.Abs(roll) <= Mathf.Epsilon)
            {
                isRollReturningToZero = false;
                sharpBank = false;
            }
               
            if ( (roll - 1f) > 0)
            {
                roll -= 1f;
            }
            else if ((roll + 1f) < 0)
            {
                roll += 1f;
            }
            else
            {
                isRollReturningToZero = false;
                sharpBank = false;
            }
        }
        
    }

    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }



    void ProcessFiring()
    {
       
        //var chargedParticle = chargeLaser.GetComponent<ParticleSystem>();
        //var emissionModule = chargeLaser.GetComponent<ParticleSystem>().emission;
        Vector3 targetCoords = new Vector3(0,0,0);
       

        if (CrossPlatformInputManager.GetButtonDown("Fire1") && !laserCharged)
        {
            chargeLaserTimer = Time.time;
            SetGunsActive(true);
            myAudioSource.PlayOneShot(laserSound);
        }
        else if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            if (toggleChargedShot)
            {
                SetGunsActive(true);
                if (Time.time - chargeLaserTimer > holdChargeTimer)
                {
                    if (!laserCharged)
                    {
                        myAudioSource.PlayOneShot(chargeLaserSound);
                    }
                    laserCharged = true;
                    SetGunsActive(false);
                    chargedProjectile.gameObject.SetActive(true);
                    foreach(var chargeEffect in chargedParticleEffects)
                    {
                        chargeEffect.Play();
                    }
                    //emissionModule.enabled = true;
                    if (!targetLocked)
                    {
                        chargedReticule.SetActive(true);
                        normalReticule.SetActive(false);

                    }
                }
            }
        }
        else
        {
            SetGunsActive(false);
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire1") && Time.time - chargeLaserTimer > holdChargeTimer)
        {
           
            chargedReticule.SetActive(false);
            normalReticule.SetActive(true);
            chargedProjectile.gameObject.SetActive(false);

            //emissionModule.enabled = false;
            //if (targetLocked)
            chargedRectileObject.Shoot();
            laserCharged = false;
            targetLocked = false;
            //chargeLaserTimer = 0;
        }
    }

    void SetGunsActive(bool activate)
    {
        foreach (var gun in guns)
        {
            var emissionModule = gun.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = activate;
        }
    }

    public float GetCameraSpeed()
    {
        return cameraSpeed;
    }

    


    void BarrelRollRight()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire2") && firstButtonPressed)
        {
            if (Time.time - timeOfFirstButton < 0.2f)
            {
  
                if (!DOTween.IsTweening(playerModel))
                {
                   
                    transform.DOLocalRotate(new Vector3(pitch, yaw, -360), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
                    barrelRoll.gameObject.SetActive(true);
                    barrelRoll.Play();
                }
                Debug.Log("DoubleClicked");
            }
            else
            {
                Debug.Log("Too late");
            }

            reset = true;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire2") && !firstButtonPressed)
        {
            firstButtonPressed = true;
            timeOfFirstButton = Time.time;
        }

        if (reset)
        {
           // barrelRoll.gameObject.SetActive(false);
            firstButtonPressed = false;
            reset = false;
        }
    }

    void BarrelRollLeft()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire3") && firstButtonPressed)
        {
            if (Time.time - timeOfFirstButton < 0.2f)
            {
                if (!DOTween.IsTweening(playerModel))
                {
                    transform.DOLocalRotate(new Vector3(pitch, yaw, 360), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
                    barrelRoll.gameObject.SetActive(true);
                    barrelRoll.Play();
                }
                Debug.Log("DoubleClicked");
            }
            else
            {
                Debug.Log("Too late");
            }

            reset = true;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire3") && !firstButtonPressed)
        {
            firstButtonPressed = true;
            timeOfFirstButton = Time.time;
        }

        if (reset)
        {
            firstButtonPressed = false;
            reset = false;
        }
    }

}
