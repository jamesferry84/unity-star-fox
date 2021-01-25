using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    private Transform playerModel;

    [SerializeField] float speed = 10f;
    [SerializeField] float maxSpeed = 30f;
    [SerializeField] float xRange = 6f;
    [SerializeField] float yRange = 5f;

    [SerializeField] float controlPitchFactor = -40f;
    [SerializeField] int steepTurnFactor = 2;

    [SerializeField] ParticleSystem[] guns;
    [SerializeField] float cameraSpeed = 1f;

    [SerializeField] AudioSource singleLaserSound;
    [SerializeField] AudioClip laserSound;

    [SerializeField] ParticleSystem barrelRoll;

    Animator myAnimator;
    

    float horizontalInput, verticalInput;
    float acceleration = 3f;
    float decelaration = 3f;

    float currentPitch = 0f;
    float currentRoll = 0f;

    float maxRollLeft = 90f;
    float maxRollRight = -90f;

    bool sharpBank = false;
    float pitch;
    float yaw;
    float roll;
    bool firstButtonPressed = false;
    bool reset;
    float timeOfFirstButton;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = transform;
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        BarrelRollRight();
        BarrelRollLeft();
        SharpTurn();
        Controls();
        ProcessRotation();
        ProcessFiring();
        Vector3 pos = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z + cameraSpeed));
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void ProcessRotation()
    {
        pitch = verticalInput * controlPitchFactor; // down is positive, up negative
        yaw = horizontalInput * controlPitchFactor * -1;
        if (!sharpBank && !barrelRoll)
        {
            Mathf.Clamp(roll = horizontalInput * controlPitchFactor, maxRollRight, maxRollLeft); // left is positive, right negative
        }
 
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll); ; //pitch, yaw, roll
    }

    void Controls()
    {

        horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        verticalInput = CrossPlatformInputManager.GetAxis("Vertical");

        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            if (roll > maxRollRight)
            {
                roll -= 1f;
            }
           // horizontalInput += 0.5f;
        }
        else if (CrossPlatformInputManager.GetButton("Fire3"))
        {
           //horizontalInput -= 0.5f;
            if (roll < maxRollLeft)
            {
                roll += 1f;
            }
        }
        

        float xOffset = horizontalInput * speed * Time.deltaTime;
        //float clampedSpeed = Mathf.Clamp(speed, 0, maxSpeed);
        float yOffset = verticalInput * speed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
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

    void ProcessFiring()
    {
       
        if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            
            SetGunsActive(true);
            singleLaserSound.PlayOneShot(laserSound);
        }
        else
        {
            SetGunsActive(false);
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
                    playerModel.DOLocalRotate(new Vector3(pitch, yaw, -360), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
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
                    playerModel.DOLocalRotate(new Vector3(pitch, yaw, 360), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
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
