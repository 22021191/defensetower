using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHeal;
    public float currentHeal;
    public int Dame;
    public float movespeed;
    public int Armor;
    public int ResistanMagic;// kháng phép
    protected float searchvalue = 0.1f;
    protected Transform slider; 
    public Animator _anim; // Animation của Enemy
    private Transform target;// check Flip với Target nay
    public bool AllowMove = true;
    public bool FightToDeath { get; set; } 
    public bool isDead = false;
    //-----Path
    public GameObject Path; 
    private List<GameObject> Paths = new List<GameObject>();
    public List<Vector3> Paths_Pos = new List<Vector3>();
    private Vector3 offset; 
    public Vector3 PointToMove; 
    public GameObject _KnightToFight = null; 
    public bool StartAttack;
    public bool isRange;
    private bool isAttacking = false; 
    public float radius; 
    public GameObject bloodEffect; 
    public Transform blood_pos;
    public int coin;

    public GameObject enemySpawn;
    private bool canMove = true;
    private bool canSpawnEnemy = true;
    public bool isBoss;
    public TextMesh name;

    #region Unity Func
    void Start()
    {
        StartAttack = false;
        FightToDeath = false;
        currentHeal = maxHeal;
        slider = transform.Find("HealBar").transform.Find("slider").transform;
        _anim = gameObject.GetComponent<Animator>();
        _anim.SetInteger("Change", 0);
        if (Path != null)
        {
            FindPath();

        }
        if (isRange) 
        {
            InvokeRepeating("FindKnight", 0, 1f);
        }
    }
    void Update()
    {
        if (Manager.isFinishing == true) { _anim.enabled = false; return; }
        if (isDead) return;
        if (!StartAttack && canMove)
        {
            MoveToPath();
            if (canSpawnEnemy&&isBoss)
            {
                canSpawnEnemy = false;

                StartCoroutine(SpawnEnemy());
            }
        }
        else
        {
            if (_KnightToFight == null) 
            {
                StartAttack = false;
                FightToDeath = false;
            }
            else 
            {
                if (!isRange && !isAttacking)
                {
                    isAttacking = true;
                    AttackKnight();
                    Invoke("SetAttackingFalse", 1f);
                }
                else if (isRange && !isAttacking) 
                {
                    float distance = Vector2.Distance(transform.position, _KnightToFight.transform.position);
                    if (distance > 0.3f && distance <= radius)
                    {
                        isAttacking = true;
                        ShootKnight();
                        Invoke("SetAttackingFalse", 1f);
                    }
                    else if (distance <= 0.3f)
                    {
                        isAttacking = true;
                        AttackKnight();
                        Invoke("SetAttackingFalse", 1f);
                    }
                    else if (_KnightToFight != null)
                    {
                        _KnightToFight = null;
                    }
                }
            }
        }
    }
    #endregion
    #region Func
    public void FindPath()    // Tìm vị trí path trên bản đồ 
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
            offset = new Vector3(UnityEngine.Random.Range(0.05f, 0.1f), UnityEngine.Random.Range(0.05f, 0.3f), 0);
            Vector3 pathpos = path.transform.position + offset;
            Paths_Pos.Add(pathpos);
        }
    }
    private void MoveToPath()    
    {
        float neareset_path = 100f;
        for (int i = 0; i < Paths_Pos.Count; i++) 
        {
            float distance = Vector2.Distance(transform.position, Paths_Pos[i]);
            if (distance <= neareset_path)
            {
                neareset_path = distance;
                PointToMove = Paths_Pos[i]; 
            }
        }
        //-- di chuyen den path
        if (transform.position != PointToMove) 
        {
            _anim.SetInteger("Change", 0);
            CheckFlip(PointToMove); // check Flip voi diem Path
            transform.position = Vector2.MoveTowards(transform.position, PointToMove, Time.deltaTime * movespeed);// di chuyen den path
            if (transform.position == Paths_Pos[Paths_Pos.Count - 1])
            {
                Manager._instance.TakeHeart();
                Destroy(gameObject);
            }
            
        }
        else
        {
            Paths_Pos.Remove(PointToMove);
        }
    }
    private void SetAttackingFalse()
    {
        isAttacking = false;
    }
    private void CheckFlip(Vector3 pos) 
    {
        Quaternion ros = transform.rotation;
        Vector3 scale = slider.parent.transform.localScale;
        if (transform.position.x <= pos.x)
        {

            
            if (ros != Quaternion.Euler(0, 180, 0))
            {
                
                transform.rotation = Quaternion.Euler(0, 180, 0);
                /*scale.x *= -1;
                slider.parent.transform.localScale = scale;*/
            }
        }
        else
        {
            
            if (ros != Quaternion.Euler(0, 0, 0))
            {
                
                transform.rotation = Quaternion.Euler(0, 0, 0);
                /*scale.x *= -1;
                slider.parent.transform.localScale = scale;*/
            }

            
        }
        if (name != null)
        {
            name.transform.localRotation = transform.localRotation;
           
        }
    }
    public void TakeDamePhysic(int _dame)
    {
        int truedame = _dame - Armor; 
        if (currentHeal > 0)
        {
            Sound_Enemy._instance.Hurt();
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
    public void TakeDamageMagic(int _dame)
    {
        int truedame = _dame - ResistanMagic; 
        if (currentHeal > 0)
        {
            currentHeal = currentHeal - truedame;
            if (currentHeal > 0)
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z); // thay doi thanh mau
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
        Sound_Enemy._instance.Die();
        _anim.SetInteger("Change", 6); 
        isDead = true;
        Instantiate(bloodEffect, blood_pos.position, Quaternion.identity);
        Manager.money += coin;
        Manager._instance.setcoin();
        Destroy(gameObject, 1f);
    }
    private void AttackKnight()
    {
        _anim.SetInteger("Change", 4);
        Invoke("GiveDame", 0.5f);
        CheckFlip(_KnightToFight.gameObject.transform.position);
    }
    public void GiveDame() 
    {
        if (_KnightToFight != null) 
        {
            if (_KnightToFight.GetComponent<KnightController>() != null)
            {
                _KnightToFight.GetComponent<KnightController>().TakeDame(Dame);
                if (_KnightToFight.GetComponent<KnightController>().currentHeal <= 0)
                {
                    _KnightToFight = null;
                    StartAttack = false;
                }
            }
            else if (_KnightToFight.GetComponent<Golem>() != null)
            {
                _KnightToFight.GetComponent<Golem>().TakeDame(Dame);
                if (_KnightToFight.GetComponent<Golem>().currentHeal <= 0)
                {
                    _KnightToFight = null;
                    StartAttack = false;
                }
            }
            else if (_KnightToFight.GetComponent<Hero>() != null)
            {
                _KnightToFight.GetComponent<Hero>().TakeDame(Dame);
                if (_KnightToFight.GetComponent<Hero>().currentHeal <= 0)
                {
                    _KnightToFight = null;
                    StartAttack = false;
                }
            }
        }
    }
    #endregion


    #region Boss
    IEnumerator SpawnEnemy()
    {
        canMove = false;
        _anim.SetInteger("Change", 1);
        for (int i = 0; i < 5; i++)
        {
            float randomValue = UnityEngine.Random.Range(-1f, 1f);
            Vector3 offset = new Vector3(randomValue, randomValue, 0);
            GameObject enemy = Instantiate(enemySpawn, transform.position + offset, Quaternion.identity) as GameObject;
            if (enemy.GetComponent<EnemyController>() != null)
            {
                List<Vector3> path = new List<Vector3>();
                foreach (Vector3 pos in Paths_Pos)
                {
                    path.Add(pos);
                }
                enemy.GetComponent<EnemyController>().Paths_Pos = path;
            }
        }
        yield return new WaitForSeconds(2);
        canMove = true;
        yield return new WaitForSeconds(6);
        canSpawnEnemy = true;
    }
    #endregion
    #region range Enemy
    #region Ham truyen tham so cho mui ten bay
    private float getminSpeed()
    {
        float aux = 0.1f;
        while (moreSpeed(aux) == true)
        { aux = aux + searchvalue; }
        return aux;
    }
    private bool moreSpeed(float speed)
    {
        bool aux = false;
        float xTarget = _KnightToFight.transform.position.x;
        float yTarget = _KnightToFight.transform.position.y;
        float xCurrent = transform.position.x;
        float yCurrent = transform.position.y;
        float xDistance = Mathf.Abs(xTarget - xCurrent);
        float yDistance = yTarget - yCurrent;
        float fireAngle = 1.57075f - (float)(Mathf.Atan((Mathf.Pow(speed, 2f) + Mathf.Sqrt(Mathf.Pow(speed, 4f) - 9.8f * (9.8f * Mathf.Pow(xDistance, 2f) + 2f * yDistance * Mathf.Pow(speed, 2f)))) / (9.8f * xDistance)));
        float xSpeed = (float)Mathf.Sin(fireAngle) * speed;
        float ySpeed = (float)Mathf.Cos(fireAngle) * speed;
        if ((xTarget - xCurrent) < 0f) { xSpeed = -xSpeed; }
        if (float.IsNaN(xSpeed) || float.IsNaN(ySpeed)) { aux = true; }
        return aux;
    }
    #endregion
    private void FindKnight() // Tìm Knight để tấn công
    {
        GameObject[] Knights = GameObject.FindGameObjectsWithTag("Knight");
        float nearestDistance = 10f;
        foreach (GameObject knight in Knights)
        {
            float distan = Vector3.Distance(transform.position, knight.transform.position);
            if (distan < nearestDistance)
            {
                nearestDistance = distan;
                _KnightToFight = knight;
            }
        }
        if (nearestDistance < radius)
        {
            StartAttack = true;
        }
    }
    private void ShootKnight() // thực hiện việc bắn tên vào mục tiêu
    {
        CheckFlip(_KnightToFight.transform.position);
        _anim.SetInteger("Change", 5);
        GameObject arrow = Instantiate(Resources.Load("Archer/Arrow"), transform.position, Quaternion.identity) as GameObject;
        Parabol _arrow = arrow.GetComponent<Parabol>();
        _arrow.target = _KnightToFight;
        _arrow.Dame = Dame;
        if (_KnightToFight != null)
        {
            _arrow.maxLaunch = getminSpeed();
        }
        if (_KnightToFight == null)
            Destroy(arrow);
    }
    #endregion
}
