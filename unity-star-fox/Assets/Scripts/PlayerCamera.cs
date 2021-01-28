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
    [SerializeField] float defaultCameraSpeed = .1f;
    // Start is called before the first frame update
    void Start()
    {
        cameraSpeed = defaultCameraSpeed;
        myPlayer = FindObjectOfType<Player>();
        vCam = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //standardFOV = vCam.m_Lens.FieldOfView;
        Debug.Log("vcam = " + vCam);
        Debug.Log("noise = " + noise);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
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
}
