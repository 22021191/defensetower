﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : MonoBehaviour
{
    [Header("Property Knight")]
    public int dame;
    public int armor;
    public float moveSpeed;
    public float maxHeal;
    [SerializeField]
    public float currentHeal { get; set; }
    public int timerespawn; 
    private Transform slider;
    [HideInInspector]
    public GameObject target = null; 
    [HideInInspector]
    public Animator _anim; 
    public bool allowMove = true;
    public Vector3 positionFlag { get; set; }
    private Barrack _barrack; 
    public List<GameObject> ListEnemy = new List<GameObject>();
    [Header("Tầm phát hiện mục tiêu của Knight")]
    public float radius;
    private bool isDead = false;

    private bool isAttacking = false;
    private float timeregend = 0; 

    public EnemyController _target = null;
    public bool MustReturn = false;
    private EnemyController _enemycode;
    private SpriteRenderer _sprite;
    void Awake()
    {
        slider = transform.Find("HealBar").transform.Find("slider");
        _anim = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.2f);
        currentHeal = maxHeal;
        _barrack = transform.parent.gameObject.GetComponent<Barrack>();
       
    }
    
    void UpdateTarget() // tim muc tieu de chon va tan cong
    {
        if (isDead) return;
        if (target == null || allowMove == true) MustReturn = false;
        if (allowMove == false) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>() != null)
            {
                _enemycode = enemy.GetComponent<EnemyController>();
            }

            if (enemy == null) return;
            float range = Vector2.Distance(transform.position, enemy.transform.position); // khoang cach giua Knight va Enemy
            if (range <= radius && _enemycode.currentHeal > 0)// neu khoang cach < radius  thi them vao List
            {
                if (!ListEnemy.Contains(enemy) && enemy != null)// neu List chua co phan tu nay thi them vao
                {
                    ListEnemy.Add(enemy);
                }

            }
            else // neu o ngoai radius thi loai khoi List
                ListEnemy.Remove(enemy);
        }
        RemoveNull();
        // Chạy ListeEnemy xem phần tử nào đã lock target là chính Obj thì sẽ return
        foreach (GameObject enemy in ListEnemy)
        {
            if (enemy == null&&enemy.GetComponent<EnemyController>()==null) return;
            if (enemy.GetComponent<EnemyController>()._KnightToFight == gameObject)
            {
                SetTarget(enemy);
                MustReturn = true;
                return;
            }
        }
        if (MustReturn == true) return;
        //------KIỂM TRA LISTENEMY để chọn target-----------
        if (ListEnemy.Count == 1) // neu trong List chi co 1 thi tan cong Enemy do
        {
            isAttacking = false;
            if (ListEnemy[0] == null) return;
            SetTarget(ListEnemy[0]);
            if (CheckEnemyToFight4Ever(this, ListEnemy[0]))
            {
                MustReturn = true;
            }
            else if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && ListEnemy[0].GetComponent<EnemyController>()._KnightToFight == null)// nếu ko phải đánh tới chết và enemy chưa locktarget thì locktarget cho enemy là chính obj
            {
                isAttacking = false;
                ListEnemy[0].GetComponent<EnemyController>()._KnightToFight = this.gameObject;
            }
        }
        if (MustReturn == true) return;
        else if (ListEnemy.Count == 2)
        {
            if (ListEnemy[0] == null || ListEnemy[1] == null) return;
            isAttacking = false;
            RemoveNull();
            for (int i = 0; i <= 1; i++)
            {

                if (CheckEnemyToFight4Ever(this, ListEnemy[i]) == true) // neu check duoc phai danh toi chet enemy nay thi tan cong
                {
                    print("da chon duoc Enemy o vong for thu" + i);
                    SetTarget(ListEnemy[i]);
                    MustReturn = true;
                    break;
                }
                else if (!CheckEnemyToFight4Ever(this, ListEnemy[i]) && ListEnemy[i].GetComponent<EnemyController>()._KnightToFight == null)
                {
                    if (MustReturn == true) return;
                    ListEnemy[i].GetComponent<EnemyController>()._KnightToFight = this.gameObject;
                    SetTarget(ListEnemy[i]);
                    MustReturn = true;
                    break;
                }

            }
            // neu thay ca 2 deu da co Knight chon tan cong den chet thi chon con thu 2
            if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && !CheckEnemyToFight4Ever(this, ListEnemy[1]) && target == null)
            {
                SetTarget(ListEnemy[1]);
                MustReturn = true;
            }
        }
        else if (ListEnemy.Count >= 3)
        {
            RemoveNull();
            isAttacking = false;
            if (ListEnemy[0] == null || ListEnemy[1] == null || ListEnemy[2] == null) return;
            // Kiểm tra xem tất cả các Enemy nếu có 1 con locktarget là obj này thì break;
            foreach (GameObject enemy in ListEnemy)
            {
                // RemoveNull();
                if (CheckEnemyToFight4Ever(this, enemy)) // neu check fai danh toi chet thi tan cong enemy nay
                {
                    SetTarget(enemy);
                    MustReturn = true;
                    break;
                }
            }
            if (MustReturn == true) return;
            // Nếu ko bị lock thì chọn enemy để lock
            foreach (GameObject enemy in ListEnemy)
            {
                // RemoveNull();
                if (!CheckEnemyToFight4Ever(this, enemy) && enemy.GetComponent<EnemyController>()._KnightToFight == null) // neu check fai danh toi chet thi tan cong enemy nay
                {
                    if (MustReturn == true) return;
                    enemy.GetComponent<EnemyController>()._KnightToFight = this.gameObject;
                    SetTarget(enemy);
                    MustReturn = true;
                    break;
                }
            }
            if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && !CheckEnemyToFight4Ever(this, ListEnemy[1]) && !CheckEnemyToFight4Ever(this, ListEnemy[2]) && target == null)
            {
                SetTarget(ListEnemy[Random.Range(0, 3)]);
                MustReturn = true;
            }
        }
    }
    private void RegendHeal()// Tự động hồi máu
    {
        timeregend += Time.deltaTime;
        if (timeregend >= 3) // hồi máu sau 3s
        {
            if (currentHeal > 0 && currentHeal < maxHeal)
            {
                currentHeal += Time.deltaTime * 4;
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, 1, 1);
            }
            else if (currentHeal >= maxHeal)
            {
                currentHeal = maxHeal;
                slider.transform.localScale = new Vector3(1, 1, 1);
                timeregend = 0f;
            }
        }
    }
    void Update()
    {
        if (Manager.isFinishing == true) return;
        int layer = Mathf.Clamp(Mathf.Abs(50 - Mathf.RoundToInt((transform.position.y - 1) * 20)), 1, 150);
        _sprite.sortingOrder = layer;
        if (isDead) return;
        if (target == null) // Nếu không có mục tiêu để tấn công 
        {
            if (gameObject.transform.position == positionFlag) // khi đang ở vị trí đặt cờ Flag
            {
                allowMove = true;
                RegendHeal(); // tự động hồi máu
                _anim.SetInteger("Change", 0);
            }
        }
        else // Nếu target được phát hiện
        {
            if (gameObject.transform.position == positionFlag) // khi dang o vi tri dat Flag
            {
                allowMove = true;
            }
            if (Vector2.Distance(transform.position, target.transform.position) > 1f) 
            {
                target = null;
                return; 
            }
            CheckFlipWith(target);
            if (Vector2.Distance(transform.position, target.transform.position) > 1f) return;
            if (allowMove&&!isAttacking) // Nếu vị trí Knight != vị trí Enemy
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position + new Vector3(-0.2f, 0, 0), Time.deltaTime * moveSpeed);
                _anim.SetInteger("Change", 1);
                if (Vector2.Distance((Vector2)transform.position,target.transform.position) <= 0.4f)
                {
                    AttackEnemy();
                }
            }
            else
            {
                AttackEnemy();
            }
        }
    }
    public void CheckFlipWith(GameObject enemy)// check xem quay trái/ phải theo mục tiêu
    {
        if (transform.position.x <= enemy.transform.position.x)
        {
            if (transform.localScale.x == -1)
            {
                transform.localScale = new Vector3(1, 1, 1);
                slider.parent.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            }
        }
        else
        {
            if (transform.localScale.x == 1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                slider.parent.transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
            }
        }
    }
    private void SetTarget(GameObject enemy)// đặt giá trị cho biến target, là mục tiêu cần tấn công
    {
        target = enemy;
        _target = target.GetComponent<EnemyController>();
    }
    private void AttackEnemy() 
    {
        if (isAttacking == false)
        {
            _anim.SetInteger("Change", 2);
            isAttacking = true;
            SoundTower._instance.KnightFight();
            //--------Bat enemy đứng lại-------
            if (target.GetComponent<EnemyController>().StartAttack == false)
            {
                target.GetComponent<EnemyController>().StartAttack = true;
            }
        }
    }
    private bool CheckEnemyToFight4Ever(KnightController knight, GameObject enemy)// Kiểm tra xem có phải đánh nhau tới chết với mục tiêu này ko
    {
        GameObject KnightToFight = enemy.GetComponent<EnemyController>()._KnightToFight;
        if (KnightToFight == knight.gameObject) // neu Nếu KnightToFight ko phải là Obj này
        {
            return true;
        }
        else
            return false;
    }
    public void TakeDame(int _dame)//Hàm nhận Dame từ Enemy
    {
        if (currentHeal > 0)
        {
            int trueDame = _dame - armor;
            currentHeal -= trueDame;// tru mau
            if (currentHeal > 0)
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z);// thay doi thanh mau
            else
                Die();
        }
        else
            Die();
    }
    private void Die()
    {
        isDead = true;
        _barrack.StartRespawn(timerespawn);
        _anim.SetInteger("Change", 3); // chay Animation Die
        Destroy(gameObject, 1f);
    }
    private void RemoveNull()
    {

        for (int i = 0; i < ListEnemy.Count; i++)
        {
            if (ListEnemy[i] == null)
                ListEnemy.Remove(ListEnemy[i]);
        }
        //foreach(GameObject enemy in ListEnemy)
        //{
        //    if (enemy == null)
        //    {
        //        ListEnemy.Remove(enemy);
        //    }
        //}
    }
    public void GiveDame()// Hàm gây Dame cho Enemy
    {
        if (_target != null)
        {
            _target.TakeDamePhysic(dame);
            if (_target.currentHeal <= 0)
            {
                target = null;
                _target = null;
            }
        }
        else
        {
            _anim.SetInteger("Change", 0);
            isAttacking = false;
            return;
        }
    }
}
