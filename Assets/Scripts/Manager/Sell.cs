using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sell : MonoBehaviour
{
    public GameObject Gold_text;
    private int Gold;
    public int index;
    void Start()
    {
        Gold_text = GameObject.Find("UI").transform.GetChild(0).gameObject;
        if (index == 0)
        {
           
            Gold = Mathf.RoundToInt(70 * 0.6f);
        }
        else if (index == 1)
        {
            
            Gold = Mathf.RoundToInt(110 * 0.6f);
        }
        else if (index == 2)
        {
            Gold = Mathf.RoundToInt(160 * 0.6f);
        }
        else if (index == 3)
        {
            Gold = Mathf.RoundToInt(230 * 0.6f);
        }
        else if (index == 4)
        {
            
            Gold = Mathf.RoundToInt(250 * 0.6f);
        }
        else
        {
           
            Gold = Mathf.RoundToInt(250 * 0.6f);
        }
    }

    void OnMouseDown()
    {

        Instantiate(Resources.Load("Node"), transform.parent.transform.position, Quaternion.identity);
        Gold_text.SetActive(true);
        Gold_text.GetComponent<Text>().text = "+ " + Gold.ToString();
        Gold_text.transform.position = new Vector2(transform.parent.transform.parent.transform.position.x, transform.parent.transform.parent.transform.position.y + 1f);
        Manager.money += Gold;
        Manager._instance.setcoin();
        SoundTower._instance.Sell();
        Destroy(transform.parent.transform.parent.gameObject);
    }

}
