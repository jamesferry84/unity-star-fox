using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boost : MonoBehaviour
{
    float boostAmount = 100;
    float inactiveTime = 3f; //number of seconds for boost to be disabled
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Slider>().value = boostAmount;
    }

    public float GetBoostAmount()
    {
        return boostAmount;
    }

    public void UpdateBoostAmount(float amount)
    {
        if ( (boostAmount + amount) <= 100f && (boostAmount + amount) >= Mathf.Epsilon )
        {
            boostAmount += amount;
        }
            
    }
}
