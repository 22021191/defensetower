using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebP.Net;
using System.Drawing;
using System.IO;
public class Manager : MonoBehaviour
{
  
    public static Manager _instance;
	public int initCoin;
    public GameObject _Interface;
    private GameObject preInterface;
    public GameObject build;
    public Sprite[] sprite;
    public GameObject[] Tower;
    public bool onclick { get; set; }

    //------------MONEY-------------
    public static int money;
    public Text _moneytext;
    public static int life;
    public Text lifeText;
    //--------GamOver----
    public GameObject panel_Over;
    public GameObject panel_pause;
    public GameObject panel_option;
    public GameObject panel_win;
    //----------Win Game---------
    public GameObject star1, star2, star3;
    public Animator _anim;
    public GameObject DeniedClick;
    public static bool isFinishing = false;
    private WebSocketHandler wsHandler;
    Dictionary<string, Dictionary<string, int>> giftCountByUser=new Dictionary<string, Dictionary<string, int>>();
    private readonly Queue<EventData> eventQueue = new Queue<EventData>();
    private bool isProcessing = false;
    private bool spawn=false;
    public List<GameObject> enemySpawn;
    
    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
	void Start ()
    {
        life = 20;
        _moneytext.text = money.ToString();
        lifeText.text = life.ToString();
        wsHandler = GetComponent<WebSocketHandler>();
        wsHandler.OnChatReceived += HandleChat;
        wsHandler.OnGiftReceived += HandleGift;
        wsHandler.OnLikeReceived += HandleLike;
        wsHandler.OnShareReceived += HandleShare;
    }
    private void Update()
    {
        if (spawn&&eventQueue.Count>0)
        {
            StartCoroutine(ProcessEventsWithDelay());
        }
    }
    public void setcoin()
    {
        _moneytext.text = money.ToString();
    }
    public void TakeHeart()
    {
        if (life > 0)
        {
            life -= 1;
        }
        else return;
        lifeText.text = life.ToString();
        if (life <= 0)
        {
            isFinishing = true;
            GameOver();
        }
    }
    private void GameOver()
    {
        DeniedClick.SetActive(true);
        panel_Over.SetActive(true);
        _anim.SetInteger("Change", 3);
        Invoke("settimeslacezero", 1f);  
    }
    public void PauseGame()
    {
        DeniedClick.SetActive(true);
        Time.timeScale = 0f;
        panel_pause.SetActive(true);
		//AdsControl.Instance.showAds ();
    }
    public void ConTinue()
    {
        panel_pause.SetActive(false);
        DeniedClick.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Resume()
    {
        StartCoroutine(HidePanelOption());
        Time.timeScale = 1f;
        DeniedClick.SetActive(false);
    }
    IEnumerator HidePanelOption()
    {
        _anim.SetInteger("Change", 2);
        yield return new WaitForSeconds(0.8f);
         panel_option.SetActive(false);
    }
    public void Option()
    {
        DeniedClick.SetActive(true);
        panel_option.SetActive(true);
        _anim.SetInteger("Change", 1);
        Invoke("settimeslacezero", 1f);
    }
    public void WinGame()// Hiện panel win game và đánh giá sao
    {
        DeniedClick.SetActive(true);
        PlayerPrefs.SetInt("Lock", SpawnLevel.level + 1);
        panel_win.SetActive(true);
        if (life < 5)
        {
            PlayerPrefs.SetInt("Star" + SpawnLevel.level, 1);
            star1.SetActive(true);
        }
        else if(life<10 && life >= 5)
        {
            PlayerPrefs.SetInt("Star" + SpawnLevel.level, 2);
            star1.SetActive(true);
            star2.SetActive(true);
        }
        else if (life >= 10)
        {
            PlayerPrefs.SetInt("Star" + SpawnLevel.level, 3);
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        _anim.SetInteger("Change", 4);
      //  Invoke("settimeslacezero", 1f);
    }
	public void ShowInterface(Node node)
    {
        foreach(Transform child in node.transform)
        {
            if (child.transform.name == "Interface")
            {
                Canvas _canvas = GameObject.Find("Canvas_Tower").GetComponent<Canvas>();
                if (_canvas.isActiveAndEnabled)
                {
                    _canvas.enabled = false;
                }
                Destroy(child.gameObject);
                return;
            }
        }
        GameObject Interface = Instantiate(_Interface, node.transform.position, Quaternion.identity);
        Interface.name = "Interface";
        Interface.transform.parent = node.transform;
    }
    public void HideInterface()
    {
        GameObject preInterface = GameObject.Find("Interface");
        if (preInterface!=null)
        {
            Destroy(preInterface.gameObject);
        }
        else
            return;
    }
    public void Build(string name, Vector3 node, GameObject obj)
    {
        SoundTower._instance.Build();
       GameObject _build= Instantiate(build,node, Quaternion.identity) as GameObject;
        _build.transform.parent = obj.transform.parent.transform.parent;
        foreach(Sprite _sprite in sprite)
        {
            if(_sprite.name==name)
            {
                _build.GetComponent<SpriteRenderer>().sprite = _sprite;
            }
        }
    }
    public void ShowTower(string name, Vector3 node,GameObject obj)
    {
        foreach (GameObject child in Tower)
        {
            if(child.gameObject.name==name)
            {
                Instantiate(child, node, Quaternion.identity);
            }
        }
       Destroy(obj.gameObject);


    }
    private void settimeslacezero()
    {
		//AdsControl.Instance.showAds ();
        Time.timeScale = 0;
    }
    private void settimescaleone()
    {
        Time.timeScale = 1;
    }
    private void HandleChat(ChatData chatData)
    {
        Debug.Log($"Chat from {chatData.nickname}: {chatData.comment}");
        // Xử lý chat message
    }

    private void HandleLike(LikeData likeData)
    {
        Debug.Log($"Chat from {likeData.nickname}: ");
        // Xử lý chat message
    }

    private void HandleShare(ShareData shareData)
    {
        Debug.Log($" from {shareData.nickname} ");
        // Xử lý gift
    }

  
    private void HandleGift(GiftData giftData)
    {
        Debug.Log("Ok");
        if (giftData.giftType >= 1 && giftData.giftType <= 4)
        {
            if (!giftCountByUser.ContainsKey(giftData.uniqueId))
            {
                giftCountByUser[giftData.uniqueId] = new Dictionary<string, int>();
            }

            // Khởi tạo giá trị cho giftId nếu chưa có
            if (!giftCountByUser[giftData.uniqueId].ContainsKey(giftData.giftId))
            {
                giftCountByUser[giftData.uniqueId][giftData.giftId] = 0;
            }
            // Log dữ liệu gift để debug
            Debug.Log("giftData1");
            if (!giftData.repeatEnd)
            {
                int leftCount = giftData.repeatCount - giftCountByUser[giftData.uniqueId][giftData.giftId];
                EnqueueEvents(giftData, leftCount);
                // Log dữ liệu gift để debug
                Debug.Log("giftData2");
                // Cập nhật lại số lượng gift đã xử lý
                giftCountByUser[giftData.uniqueId][giftData.giftId] += leftCount;
            }
            else
            {
                EnqueueEvents(giftData, giftData.repeatCount);
                giftCountByUser[giftData.uniqueId].Remove(giftData.giftId);

                // Xóa user nếu không còn gift nào liên quan
                if (giftCountByUser[giftData.uniqueId].Count == 0)
                {
                    giftCountByUser.Remove(giftData.uniqueId);
                }
            }
        }
        else
        {
            // Nếu giftType khác 1-4, chỉ đơn giản enqueue với số lần repeatCount
            giftData.giftType = 1;
            EnqueueEvents(giftData, giftData.repeatCount);
        }

        
    }

    // Hàm phụ để enqueue các sự kiện
    private void EnqueueEvents(GiftData giftData, int count)
    {
        Debug.Log("EnqueueEvents");
        int currentWaveIndex = WaveSpawnManager._instance.CurrentWave;
        if (currentWaveIndex == 0) 
        {
            currentWaveIndex = 1;
        }

        var enemyTypes = WaveSpawnManager._instance.waves[currentWaveIndex].enemyType;

        for (int i = 0; i < count; i++)
        {
            //int randomIndex = UnityEngine.Random.Range(0, enemyTypes.Length);
            GameObject path = enemyTypes[0].Path;

            eventQueue.Enqueue(new EventData(giftData.giftType, giftData.nickname, path, giftData.uniqueId, enemyTypes[0].positon_spawn, giftData.avatar));
        }

        ProcessEventQueue();
    }
    private void Instance_Enemy(GameObject enemyPref, GameObject path, GameObject position,string name,GameObject img)
    {
       
        GameObject enemy = Instantiate(enemyPref, position.transform.position, Quaternion.identity);
        if (enemy.GetComponent<EnemyController>() != null)
        {
            enemy.GetComponent<EnemyController>().Path = path;
            enemy.GetComponent<EnemyController>().name.text = name;
        }
        else if (enemy.GetComponent<FlyController>() != null)
        {
            enemy.GetComponent<FlyController>().name.text = name;
            enemy.GetComponent<FlyController>().Path = path;
        }
        img.gameObject.transform.parent=enemy.transform;
        img.transform.localPosition = new Vector3(0, 10, 0);

    }
    private void ProcessEventQueue()
    {
        Debug.Log(eventQueue.Count);
        spawn = true;
    }
    private IEnumerator ProcessEventsWithDelay()
    {
        spawn = false;
        // Kiểm tra nếu hàng đợi có sự kiện cần xử lý
        while (eventQueue.Count > 0)
        {
            // Lấy sự kiện ở đầu hàng đợi
            EventData eventData = eventQueue.Dequeue();
            UnityWebRequest request = UnityWebRequest.Get(eventData.url);  // Tải tệp RIFF (không phải ảnh Texture)
            yield return request.SendWebRequest();


            if (request.result == UnityWebRequest.Result.Success)
            {
                // Lấy dữ liệu WebP
                byte[] webpData = request.downloadHandler.data;

                // Sử dụng WebPSharp (hoặc unity.webp) để tải ảnh từ dữ liệu WebP
                Bitmap bitmap = WebPDecoder.Decode(webpData);
                byte[] imageBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = memoryStream.ToArray();
                }

                // Tạo Texture2D từ byte[]
                Texture2D texture = new Texture2D(2, 2); 
                if (texture.LoadImage(imageBytes))
                {
                    // Tạo sprite từ texture
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    // Tạo đối tượng GameObject mới với SpriteRenderer
                    GameObject newObject = new GameObject("WebP Image");
                    newObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.1f);
                    SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.sortingOrder = 300;

                    // Đặt vị trí nếu cần
                    newObject.transform.position = new Vector3(0, 0, 0);

                    // Thực thi hành động liên quan đến Enemy hoặc các logic khác
                    Instance_Enemy(enemySpawn[eventData.giftType - 1], eventData.path, eventData.positon_spawn, eventData.userName, newObject);

                    yield return new WaitForSeconds(0.6f); // Chờ một chút trước khi tiếp tục
                }
                else
                {
                    Debug.LogError("Failed to load WebP texture.");
                }
            }
            else
            {
                // In ra lỗi nếu không tải được
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Downloaded Data: {request.downloadHandler.text}");
            }
        }
        yield return new WaitForSeconds(1);
        spawn = true;
    }

}
