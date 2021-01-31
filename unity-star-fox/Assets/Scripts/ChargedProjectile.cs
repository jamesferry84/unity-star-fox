using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedProjectile : MonoBehaviour
{

    GameObject targetToTrack;
    Transform targetPosUpdated;
    bool target = false;
    [SerializeField] float speed = 0.3f;

    Vector3 targetVector;

    private void Start()
    {
        transform.DOScale(new Vector3( transform.localScale.x * 1.2f, transform.localScale.x * 1.2f, transform.localScale.z), 1f);
    }

    public void Shoot(Vector3 targetCoords, Vector3 originCoords, bool hasTarget, GameObject enemyObject)
    {
       target = hasTarget;
       if (target)
        {
            targetToTrack = enemyObject;
            targetVector = targetCoords;
           // transform.position += Time.deltaTime * speed * transform.forward;
        }
    }

    private void Update()
    {
        if (!target)
        {
            speed += 3f;
            transform.position += Time.deltaTime * speed * transform.forward;
        }
        else
        {
            if (targetToTrack != null)
            {
              //  StartCoroutine("DestroyParticles");
                transform.DOMove(targetToTrack.transform.position, 3f);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("im triggerd");
        StartCoroutine("DestroyParticles");
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("im colliding");
        StartCoroutine("DestroyParticles");
    }

    private void OnParticleCollision(GameObject other)
    {
        print("im on particle collision");
        StartCoroutine("DestroyParticles");
    }

    IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }


}
