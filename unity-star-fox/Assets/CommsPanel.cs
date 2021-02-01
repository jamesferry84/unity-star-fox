using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogTextObject;
    [SerializeField] float delay = 0.04f;
    [SerializeField] string fullText;
    [SerializeField] AudioClip radioOn;
    [SerializeField] AudioClip radioOff;
    [SerializeField] AudioClip mugshotVoice;
    string currentText = "";
    void Start()
    {
        fullText += " ";
        StartCoroutine("ShowText");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMugshotAudio()
    {
        AudioSource.PlayClipAtPoint(mugshotVoice, Camera.main.transform.position);
    }

    public void PlayRadioOnAudio()
    {
        AudioSource.PlayClipAtPoint(radioOn, Camera.main.transform.position);
    }

    public void PlayRadioOffAudio()
    {
        AudioSource.PlayClipAtPoint(radioOff, Camera.main.transform.position);
    }

    IEnumerator ShowText()
    {
        for(int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            dialogTextObject.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
