using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedRectile : MonoBehaviour
{
    public bool lockOn = false;
    public Transform enemyTransform;
    [SerializeField] GameObject targetLockedReticule;
    [SerializeField] ChargedProjectile chargedProjectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !lockOn)
        {
            Debug.Log("Charged Rectile Targeted");
            enemyTransform = other.gameObject.transform;
            targetLockedReticule.transform.position = enemyTransform.position;
            targetLockedReticule.SetActive(true);
            lockOn = true;
        }
    }

    public void Shoot()
    {
        lockOn = false;
        targetLockedReticule.SetActive(false);
        ChargedProjectile cp = Instantiate(
           chargedProjectile,
           new Vector3(transform.position.x, transform.position.y, transform.position.z + 10f),
           transform.rotation);
        if (enemyTransform != null)
            cp.Shoot(enemyTransform.position, transform.position, true);
        else
            cp.Shoot(Vector3.forward, transform.position, false);
    }



    
}
