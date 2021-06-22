using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVFlow : MonoBehaviour
{
    [SerializeField] private float speed;
    private float timer;

    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(startPosition.position, endPosition.position, timer*0.25f);

        timer += Time.deltaTime;
        GetComponent<Renderer>().material.mainTextureOffset =
            new Vector2(-speed * timer, speed * timer);
    }
}
