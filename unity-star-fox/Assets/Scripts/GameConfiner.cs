using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfiner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject camera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
    }
}
