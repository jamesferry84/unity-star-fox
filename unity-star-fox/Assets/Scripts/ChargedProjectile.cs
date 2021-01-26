using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedProjectile : MonoBehaviour
{

    public void Shoot(Vector3 targetCoords, Vector3 originCoords)
    {
        transform.position = originCoords;
        Debug.Log("I'm Alive at " + transform.position);
        transform.DOLocalMove(targetCoords, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
    }


}
