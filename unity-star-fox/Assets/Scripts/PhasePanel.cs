using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhasePanel : MonoBehaviour
{
    [SerializeField] string phaseNumber;
    [SerializeField] string phaseDescription;

    [SerializeField] TextMeshProUGUI phaseNumberTextObject;
    [SerializeField] TextMeshProUGUI phaseDescriptionTextObject;
    // Start is called before the first frame update
    void Start()
    {
        phaseNumberTextObject.text = "- Phase " + phaseNumber + " -";
        phaseDescriptionTextObject.text = "- " + phaseDescription + " -";
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
