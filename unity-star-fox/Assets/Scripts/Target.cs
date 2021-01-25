using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called before the first frame update
    Color originalColor;
    AudioSource myAudio;
    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        StartCoroutine("FlashHit");
    }

    IEnumerator FlashHit()
    {
        GetComponent<Renderer>().material.color = Color.white;
        myAudio.Play();
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material.color = originalColor;
        StopCoroutine("FlashHit");
    }
}
