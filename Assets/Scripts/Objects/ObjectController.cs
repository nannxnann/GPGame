﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public float smoothTime = 0.05F;
    public float distanceThreshold = 0.01F;

    private bool inTransition = false;
    private Vector2 velocity = Vector2.zero;
    private Vector2 transitionTarget = Vector2.zero;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!inTransition)
        {
            transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Vector2 current2DPosition = new Vector2(transform.position.x, transform.position.y);

        if (Vector2.Distance(current2DPosition, transitionTarget) <= distanceThreshold)
        {
            inTransition = false;
        }
        if (inTransition)
        {
            transform.position = Vector2.SmoothDamp(transform.position, transitionTarget, ref velocity, smoothTime);
            Debug.Log(transitionTarget);
        }
    }

    public void TransitionToPosition(Vector2 targetPosition)
    {
        if (!inTransition)
        {
            transitionTarget = targetPosition;
            Debug.Log(transitionTarget);
            inTransition = true;
        }
    }
}
