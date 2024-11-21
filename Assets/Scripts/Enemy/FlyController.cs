using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{

    public float maxHeal;
    public float currentHeal; 
    public float movespeed;
    public int Armor;
    public int ResistanMagic;
    private Transform slider;
    private Animator _anim;
    private bool isDead = false;

    public GameObject Path;
    private List<GameObject> Paths = new List<GameObject>(); // Danh sách các path có trên bản đồ
    private List<Vector3> Paths_Pos = new List<Vector3>(); // vị trí của các path trong DS paths
    private Vector3 offset; 
    private Vector3 PointToMove;
    public TextMesh name;


    void Start()
    {
        currentHeal = maxHeal;
        _anim = GetComponent<Animator>();
        _anim.SetInteger("Change", 0);
        FindPath();
        slider = transform.Find("HealBar").transform.Find("slider").transform;
    }
    private void FindPath()    // Tìm vị trí path trên bản đồ 
    {
        for (int i = 0; i < Path.transform.childCount; i++)
        {
            if (!Paths.Contains(Path.transform.GetChild(i).gameObject))
            {
                Paths.Add(Path.transform.GetChild(i).gameObject);
            }
        }
        
        foreach (GameObject path in Paths)
        {
            offset = new Vector3(Random.Range(0.05f, 0.1f), Random.Range(0.05f, 0.3f), 0);
            Vector3 pathpos = path.transform.position + offset;
            Paths_Pos.Add(pathpos);
        }
    }
    private void MoveToPath()// thực hiện việc di chuyển đến path gần nhất
    {
        float neareset_path = 100f;
        for (int i = 0; i < Paths_Pos.Count; i++) // chay xong vong lap nay se tim duoc path ngan nhat
        {
            float distance = Vector3.Distance(transform.position, Paths_Pos[i]);
            if (distance <= neareset_path)
            {
                neareset_path = distance;
                PointToMove = Paths_Pos[i]; //toa do path ngan nhat
            }
        }
        //-- di chuyen den path
        if (transform.position != PointToMove) 
        {
            _anim.SetInteger("Change", 0); 
            CheckFlipWith(PointToMove); 
            transform.position = Vector3.MoveTowards(transform.position, PointToMove, Time.deltaTime * movespeed);// di chuyen den path
            if (transform.position == Paths_Pos[Paths_Pos.Count - 1]) // Nếu di chuyển đến path cuối cùng thì sẽ trừ máu Player
            {
                Manager._instance.TakeHeart();
                Destroy(gameObject);
            }
            if (transform.position.y >= PointToMove.y && Mathf.Abs(transform.position.x - PointToMove.x) <= 0.2f && Mathf.Abs(transform.position.y - PointToMove.y) > 0.2f) // Chạy xuống
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (transform.position.y < PointToMove.y && Mathf.Abs(transform.position.x - PointToMove.x) <= 0.2f && Mathf.Abs(transform.position.y - PointToMove.y) > 0.2f) // Chạy lên
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            Paths_Pos.Remove(PointToMove);
        }
    }
    private void CheckFlipWith(Vector3 pos) // kiểm tra việc quay mặt với tọa độ pos
    {
        Quaternion ros = transform.rotation;
        
        if (transform.position.x <= pos.x)
        {
            if (ros == Quaternion.Euler(0, 180, 0))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);

            }
        }
        else
        {
            if (ros == Quaternion.Euler(0, 0, 0))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);

            }
        }
    }
    public void TakeDamePhysic(int _dame) // nhận dame từ sát thương vật lý
    {
        int truedame = _dame - Armor; // Dame sau khi da tru giap va khang phep
        if (currentHeal > 0)
        {
            currentHeal = currentHeal - truedame;
            if (currentHeal > 0)
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, 1, 1); // thay doi thanh mau
            else
            {
                slider.transform.localScale = new Vector3(0, 1, 1);
                Die();
            }
        }
        else
            Die();
    }
    public void TakeDamageMagic(int _dame)// nhận dame từ sát thương phép
    {
        int truedame = _dame - ResistanMagic; // Dame sau khi da tru giap va khang phep
        if (currentHeal > 0)
        {
            currentHeal = currentHeal - truedame;
            if (currentHeal > 0)
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, 1, 1); // thay doi thanh mau
            else
            {
                slider.transform.localScale = new Vector3(0, 1, 1);
                Die();
            }
        }
        else
            Die();
    }
    private void Die()
    {
        //Sound_Enemy._instance.Die();
        _anim.SetInteger("Change", 3); // ANimation die   
        isDead = true;
        Destroy(gameObject, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        if (Manager.isFinishing == true) { _anim.enabled = false; return; }
        // đặt layer theo tọa độ Y
        int layer = Mathf.Clamp(Mathf.Abs(50 - Mathf.RoundToInt((transform.position.y - 1) * 15)), 1, 150);
        if (isDead) { return; }
            MoveToPath();
    }
}
