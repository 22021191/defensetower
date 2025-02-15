﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Arrow : MonoBehaviour
{

    public int damage = 1;
    public float lifeTime = 3f;
    public float speed = 3f;
    public float speedUpOverTime = 0.5f;
    public float hitDistance = 0.2f;
    public float ballisticOffset = 0.5f;
    public bool freezeRotation = false;
    private Vector2 originPoint;
    public GameObject target;
    private Vector2 aimPoint;
    private Vector2 myVirtualPosition;
    private Vector2 myPreviousPosition;
    private float counter;
    private SpriteRenderer sprite;

    private void Start()
    {
        target = GetComponent<Parabol>().target;
        Fire(target);
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void Fire(GameObject target)
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        originPoint = myVirtualPosition = myPreviousPosition = transform.position;
        this.target = target;
        aimPoint = target.transform.position;
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void FixedUpdate()
    {
        counter += Time.fixedDeltaTime;
        // Add acceleration
        speed += Time.fixedDeltaTime * speedUpOverTime;
        if (target != null)
        {
            aimPoint = target.transform.position;
        }
        Vector2 originDistance = aimPoint - originPoint; // vector khoảng cách giữa điểm bắn và mục tiêu
        Vector2 distanceToAim = aimPoint - (Vector2)myVirtualPosition; //vector khoảng cách giữa điểm bắn và mục tiêu
        myVirtualPosition = Vector2.Lerp(originPoint, aimPoint, counter * speed / originDistance.magnitude);// vector nội suy giữa vị trí ban đầu và mục tiêu
        transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);
        LookAtDirection2D((Vector2)transform.position - myPreviousPosition);
        myPreviousPosition = transform.position;
        sprite.enabled = true;
        
    }

    /// <summary>
    /// Looks at direction2d.
    /// </summary>
    /// <param name="direction">Direction.</param>
    private void LookAtDirection2D(Vector2 direction)
    {
        if (freezeRotation == false)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    /// <summary>
    /// Adds ballistic offset to trajectory.
    /// </summary>
    /// <returns>The ballistic offset.</returns>
    /// <param name="originDistance">Origin distance.</param>
    /// <param name="distanceToAim">Distance to aim.</param>
    private Vector2 AddBallisticOffset(float originDistance, float distanceToAim)
    {
        if (ballisticOffset > 0f)
        {
            float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
            offset *= originDistance;
            return (Vector2)myVirtualPosition + (ballisticOffset * offset * Vector2.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }
}
