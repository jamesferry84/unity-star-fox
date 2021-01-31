using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Player myPlayer;
    CinemachineVirtualCamera vCam;
    CinemachineBasicMultiChannelPerlin noise;
    float standardFOV = 75f;
    float brakeCamFovMin = 35f;
    float boostCamFovMax = 140f;
    public bool brakeCam = false;
    public bool boostCam = false;
    float cameraSpeed;
    

    [Header("Target")]
    public Transform target;

    [Space]

    [Header("Offset")]
    public Vector3 offset = Vector3.zero;
    public float distanceFromPlayer = 9f;

    [Space]

    [Header("Limits")]
    public Vector2 limits = new Vector2(5, 3);

    [Space]

    [Header("Smooth damp time")]
    [Range(0, 1)]
    public float smoothTime;

    private Vector3 velocity = Vector3.zero;

    [SerializeField] float defaultCameraSpeed = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        cameraSpeed = defaultCameraSpeed;
        myPlayer = FindObjectOfType<Player>();
        vCam = GameObject.Find("Player Camera OnRails").GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //standardFOV = vCam.m_Lens.FieldOfView;

    }

 
    void Update()
    {
        if (!Application.isPlaying)
        {
            transform.localPosition = offset;
        }

        FollowTarget(target);

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
       
    }

    public void FollowTarget(Transform t)
    {
        Vector3 localPos = transform.localPosition;
        Vector3 targetLocalPos = t.transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(localPos, new Vector3(targetLocalPos.x + offset.x, targetLocalPos.y + offset.y, targetLocalPos.z - distanceFromPlayer ), ref velocity, smoothTime);
    }

    public float GetCameraSpeed()
    {
        return cameraSpeed;
    }

    public void Noise(float amplitudeGain, float frequencyGain)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }

    public void BrakeCamera()
    {
        StartCoroutine("ApplyZoomForBrakeCam");
    }

    public void BoostCamera()
    {
        StartCoroutine("ApplyZoomForBoostCam");
    }



    IEnumerator ApplyZoomForBoostCam()
    {
        boostCam = true;
        cameraSpeed = 0.06f;
        yield return new WaitForSeconds(1.5f);
        cameraSpeed = defaultCameraSpeed;
        boostCam = false;
        // ReturnFieldOfView();
    }

    IEnumerator ApplyZoomForBrakeCam()
    {
        brakeCam = true;
        cameraSpeed = 0.01f;
        yield return new WaitForSeconds(1.5f);
        cameraSpeed = defaultCameraSpeed;
        brakeCam = false;
       // ReturnFieldOfView();
    }


    //if (brakeCam)
    //{
    //    if (vCam.m_Lens.FieldOfView >= brakeCamFovMin)
    //    {
    //        vCam.m_Lens.FieldOfView -= .2f;
    //    }
    //}
    //else
    //{
    //    if (vCam.m_Lens.FieldOfView <= standardFOV)
    //    {
    //        vCam.m_Lens.FieldOfView += .2f;
    //    }


    //    if (boostCam)
    //    {
    //        if (vCam.m_Lens.FieldOfView <= boostCamFovMax)
    //        {
    //            vCam.m_Lens.FieldOfView += .8f;
    //        }
    //    }
    //    else
    //    {
    //        if (vCam.m_Lens.FieldOfView >= standardFOV)
    //        {
    //            vCam.m_Lens.FieldOfView -= .6f;
    //        }
    //    }
    //}
}
