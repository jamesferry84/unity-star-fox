using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    private Transform playerModel;
    PlayerCamera playerCamera;

    [SerializeField] bool toggleChargedShot = true;

    [SerializeField] float speed = 10f;
    [SerializeField] float forwardSpeed = .5f;
    [SerializeField] float maxSpeed = 30f;
    [SerializeField] float xRange = 6f;
    [SerializeField] float yRange = 5f;

    [SerializeField] float controlPitchFactor = -40f;
    [SerializeField] int steepTurnFactor = 2;

    
    [SerializeField] float cameraSpeed = 1f;

    //Sounds
    [SerializeField] AudioSource myAudioSource;
    [SerializeField] AudioClip laserSound;
    [SerializeField] AudioClip arwingAmbientSound;
    [SerializeField] AudioClip boostSound;
    [SerializeField] AudioClip brakeSound;
    [SerializeField] AudioClip itemCollect;
    [SerializeField] AudioClip chargeLaserSound;
    [SerializeField] AudioClip damageSound;

    [SerializeField] ParticleSystem[] guns;
    [SerializeField] ParticleSystem barrelRoll;
    [SerializeField] ParticleSystem brake;
    [SerializeField] ParticleSystem boost;
    [SerializeField] ParticleSystem trail;
    [SerializeField] ParticleSystem chargeLaser;
    [SerializeField] ParticleSystem afterBurner;

    [SerializeField] float frequencyGain = 10f;
    [SerializeField] float amplitudeGain = 10f;

    [SerializeField] GameObject normalReticule;
    [SerializeField] GameObject chargedReticule;
    [SerializeField] GameObject targetLockedReticule;
    [SerializeField] ChargedProjectile chargedProjectile;

    [SerializeField] ChargedRectile chargedRectileObject;

    SpriteRenderer SmallAimReticuleRenderer;

    [SerializeField] Score score;
    [SerializeField] Boost boostUi;

    bool targetLocked = false;
    bool isBoosting = false;
    bool isBraking = false;


    float horizontalInput, verticalInput;

    float maxRollLeft = 90f;
    float maxRollRight = -90f;

    bool sharpBank = false;
    float pitch;
    float yaw;
    float roll;
    bool firstButtonPressed = false;
    bool reset;
    float timeOfFirstButton;

    float chargeLaserTimer;
    float holdChargeTimer = 1f;

    bool laserCharged = false;

    // Start is called before the first frame update
    void Start()
    {
        SmallAimReticuleRenderer = normalReticule.GetComponent<SpriteRenderer>();
        playerModel = transform;
        playerCamera = FindObjectOfType<PlayerCamera>();

        boostUi = FindObjectOfType<Boost>();
        
       // cameraSpeed = playerCamera.GetCameraSpeed();
        //chargedRectileObject = GetComponent<ChargedRectile>();
        //myAudioSource.loop = true;
        //myAudioSource.clip = arwingAmbientSound;
        //myAudioSource.Play();
    }

    // Update is called once per frame
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
        ApplyBoost();
        BarrelRollRight();
        BarrelRollLeft();
        SharpTurn();
        Controls(horizontalInput, verticalInput);
        ProcessRotation();
        RotationLookForGuns(horizontalInput,verticalInput, speed);
        ProcessFiring();
        //Vector3 pos = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z + cameraSpeed));
        //pos.x = Mathf.Clamp01(pos.x);
        //pos.y = Mathf.Clamp01(pos.y);
        //transform.position = Camera.main.ViewportToWorldPoint(pos);
       // cameraSpeed = playerCamera.GetCameraSpeed();
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
        //foreach (var gun in guns)
        //{
        //   // gun.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(normalReticule.transform.position), Mathf.Deg2Rad * speed * Time.deltaTime);
        //    gun.transform.LookAt(normalReticule.transform.position);
        //}
        //normalReticule.transform.position = Vector3.zero;
        //aimTarget.localPosition = new Vector3(h, v, 1);

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
        }
        else
        {
            boostUi.UpdateBoostAmount(0.1f);
        }
       
    }

    void Brake()
    {
        if (isBraking)
        {
            boostUi.UpdateBoostAmount(-0.3f);
            afterBurner.gameObject.SetActive(false);
            //playerCamera.BrakeCamera();
        }
        else
        {
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
        //Debug.Log(verticalInput); down is negative 1. up is 1
        //if (transform.localPosition.y <= -yRange && verticalInput <= 0)
        //{
        //    pitch = verticalInput * -20;
        //}
        //else
        //{
            pitch = verticalInput * controlPitchFactor; // down is positive, up negative
        //}
       
        yaw = horizontalInput * controlPitchFactor * -1;
        if (!sharpBank )
        {
            Mathf.Clamp(roll = horizontalInput * controlPitchFactor, maxRollRight, maxRollLeft); // left is positive, right negative
        }

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll); ; //pitch, yaw, roll
    }

    void Controls(float h, float v)
    {

        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            if (roll > maxRollRight)
            {
                roll -= 2.7f;
            }
           // horizontalInput += 0.5f;
        }
        else if (CrossPlatformInputManager.GetButton("Fire3"))
        {
           //horizontalInput -= 0.5f;
            if (roll < maxRollLeft)
            {
                roll += 2.7f;
            }
        }


        //float xOffset = horizontalInput * speed * Time.deltaTime;
        //float yOffset = verticalInput * speed * Time.deltaTime;

        //float rawXPos = transform.localPosition.x + xOffset;
        //float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        //float rawYPos = transform.localPosition.y + yOffset;
        //float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);

        //transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
        transform.localPosition += new Vector3(h, v, forwardSpeed) * speed * Time.deltaTime;
        ClampPosition();
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
       
        var chargedParticle = chargeLaser.GetComponent<ParticleSystem>();
        var emissionModule = chargeLaser.GetComponent<ParticleSystem>().emission;
        Vector3 targetCoords = new Vector3(0,0,0);
       

        if (CrossPlatformInputManager.GetButtonDown("Fire1") && !laserCharged)
        {
            score.UpdateScore(1);
            //Debug.DrawRay(normalReticule.transform.position, transform.forward*100f, Color.red,2f);
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
                    emissionModule.enabled = true;
                    if (!targetLocked)
                    {

                        //myAudioSource.PlayOneShot(chargeLaserSound);
                        chargedReticule.SetActive(true);
                        normalReticule.SetActive(false);

                    }

                    //RaycastHit hit;
                    //if (Physics.Raycast(normalReticule.transform.position, transform.forward, out hit, 100f, LayerMask.GetMask("Enemy")) && !targetLocked)
                    //{
                    //    Debug.DrawRay(transform.position, transform.forward, Color.green,10f); print("Hit");
                    //    targetLocked = true;
                    //    targetLockedReticule.transform.position = hit.transform.position;
                    //    targetLockedReticule.SetActive(true);
                    //    chargedReticule.SetActive(false);
                    //    normalReticule.SetActive(true);
                    //    Debug.Log("I hit: " + hit.transform.position);
                    //    targetCoords = targetLockedReticule.transform.position;

                    //}
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

            emissionModule.enabled = false;
            if (targetLocked)
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

    void SharpTurn()
    {
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            sharpBank = true;
        }

        if (CrossPlatformInputManager.GetButton("Fire3"))
        {
            sharpBank = true;
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire2"))
        {
            sharpBank = false;
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire3"))
        {
            sharpBank = false;
        }
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
