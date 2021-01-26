using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionFx;
    Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Hit");
        Instantiate(explosionFx, transform.position, transform.rotation);
        Destroy(gameObject);
        //StartCoroutine("FlashHit");
    }

    IEnumerator FlashHit()
    {
        GetComponent<Renderer>().material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material.color = originalColor;
        StopCoroutine("FlashHit");
    }
}
