using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionFx;
    [SerializeField] AudioClip explosionSfx;
    [SerializeField] bool moveForwardAtAllTimes = false;
    [SerializeField] float speed = 1f;
    Player player;
    Score score;
    Rigidbody myRigidbody;
    Color originalColor;
    BoxCollider myBoxCollider;
    
    // Start is called before the first frame update
    void Start()
    {
       // originalColor = GetComponent<Renderer>().material.color;
        score = FindObjectOfType<Score>();
        myRigidbody = GetComponent<Rigidbody>();
        myBoxCollider = GetComponent<BoxCollider>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveForwardAtAllTimes)
        {
            transform.Translate(0, 0, speed * Time.deltaTime, Space.Self);
        }
        //transform.LookAt(
        //    new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z), Vector3.up
        //    ); ;
    }

    public void KillEnemy()
    {
        myBoxCollider.enabled = false;
        StartCoroutine("DelayBeforeExplosion");
        myRigidbody.useGravity = true;
        myRigidbody.mass = 3;
        transform.DORotate(new Vector3(180,180,10), 2f);
        Instantiate(explosionFx, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(explosionSfx, Camera.main.transform.position);
        score.UpdateScore(1);
        
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        KillEnemy();
        //StartCoroutine("FlashHit");
    }

    IEnumerator DelayBeforeExplosion()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(explosionFx, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    IEnumerator FlashHit()
    {
        GetComponent<Renderer>().material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material.color = originalColor;
        StopCoroutine("FlashHit");
    }
}
