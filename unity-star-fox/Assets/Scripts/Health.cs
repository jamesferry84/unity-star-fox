using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public float healthAmount = 100; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Slider>().value = healthAmount;
    }

    public void UpdateHealthAmount(float amount)
    {
        healthAmount -= amount;
    }
}
