using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFinal : MonoBehaviour
{
    public bool allowclick = false;
    private bool wascall = false;
    public bool waspoint = false; // co dang duoc chi chuot toi hay ko
    Collider2D coll;
    private UpgradeFinal[] _upgrades;
    void Start()
    {
        allowclick = true;
        coll = GetComponent<Collider2D>();
        _upgrades = FindObjectsOfType<UpgradeFinal>();
    }

    // Update is called once per frame
    void Update()
    {
        if (allowclick == false && !wascall)
        {
            wascall = true;
            Invoke("setonclicktrue", 0.2f);
        }
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Vector3.zero);
        if (hit.collider != null) 
        {
            if (hit.collider == coll) 
            {
                waspoint = true;
            }
            else
            {
                waspoint = false;
                if (_upgrades.Length == 2)
                {
                    if (!_upgrades[0].waspoint && !_upgrades[1].waspoint)
                    {
                        if (Input.GetMouseButtonDown(0) && allowclick == true)
                        {
                            Destroy(transform.parent.gameObject,0.1f);
                        }
                    }
                }

                else if (_upgrades.Length == 3)
                {
                    if (!_upgrades[0].waspoint && !_upgrades[1].waspoint && !_upgrades[2].waspoint)
                    {
                        if (Input.GetMouseButtonDown(0) && allowclick == true)
                        {
                            Destroy(transform.parent.gameObject,0.1f);
                        }
                    }

                }
            }
        }
    }
    void setonclicktrue()
    {
        allowclick = true;
        wascall = false;
    }
}