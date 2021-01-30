using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedProjectile : MonoBehaviour
{

    GameObject targetToTrack;
    Transform targetPosUpdated;
    bool target = false;
    [SerializeField] float speed = 1f;

    private void Start()
    {
        transform.DOScale(new Vector3( transform.localScale.x * 6f, transform.localScale.x * 6f, transform.localScale.z), 2f);
    }

    public void Shoot(Vector3 targetCoords, Vector3 originCoords, bool hasTarget)
    {
       target = hasTarget;
       if (target)
        {
            transform.position += Time.deltaTime * speed * transform.forward;
        }
    }

    private void Update()
    {
        if (!target)
        {
            speed += 3f;
            transform.position += Time.deltaTime * speed * transform.forward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine("DestroyParticles");
    }

    private void OnParticleCollision(GameObject other)
    {
        StartCoroutine("DestroyParticles");
    }

    IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }


}
