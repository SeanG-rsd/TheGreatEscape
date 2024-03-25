using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] private Image image;

    private float timeLeft;

    private bool activate;
    private void Update()
    {
        if (image.color.a > 0 && activate)
        {
            Color color = image.color;
            color.a = Mathf.Clamp01(color.a - (Time.deltaTime / timeLeft));
            image.color = color;
            if (color.a <= 0) { activate = false; }
        }
    }

    public void Activate(float time)
    {
        Color color = image.color;
        color.a = 1;
        image.color = color;
        timeLeft = time;
        activate = true;
    }
}
