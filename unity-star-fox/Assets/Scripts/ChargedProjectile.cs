using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedProjectile : MonoBehaviour
{

    GameObject targetToTrack;
    Transform targetPosUpdated;

    public void Shoot(Vector3 targetCoords, Vector3 originCoords)
    {
       // targetToTrack = target;
        transform.position = originCoords;
        Debug.Log("I'm Alive at " + transform.position);
        transform.DOLocalMove(targetCoords, 1f);
    }

    private void Update()
    {
        if (targetToTrack != null)
        {
            
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
