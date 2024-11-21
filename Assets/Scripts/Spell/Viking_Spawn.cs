using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking_Spawn : MonoBehaviour {


    public GameObject [] vikings;
    public Transform[] pos;
   private bool move = true;
    void Update()
    {
          if (move == false) return;
        //Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.position = Vector2.Lerp(transform.position, mouse, Time.deltaTime * 10f);
        if (Input.GetMouseButton(0))
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Check();
        }

    }
    private void Check()
    {
        FindObjectOfType<Set_Viking>().ChangeImage();
        move = false;
        SpawnViking();
    }
    private void SpawnViking()
    {
        SoundTower._instance.SoliderSpawn();
       // print("da goi ham Rain");
        for (int i = 0; i <= 1; i++)
        {
             Instantiate(vikings[i], pos[i].position, Quaternion.identity);
        }
        Set_Viking.countTime = true;
        Destroy(gameObject, 0.2f);
    }
   
   
}
