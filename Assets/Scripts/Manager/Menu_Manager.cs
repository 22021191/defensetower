using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_Manager : MonoBehaviour
{
    public GameObject LevelManager;
    public Animator ControlGate;
    AsyncOperation ao;
    private bool click;
	public bool hasShop;
    void Start()
    {
		
        click = false;
        if (SceneManager.GetActiveScene().buildIndex != 0)
            ControlGate.SetInteger("Change", 1); // Open Gate
		
        Time.timeScale = 1f;
    }
    public void LoadGame()
    {
        StartCoroutine(LoadSceneGamePlay());
    }
    private IEnumerator LoadSceneGamePlay()
    {
        if (click == true) yield break;
        ao = SceneManager.LoadSceneAsync(2);
        ao.allowSceneActivation = false;
        click = true;
        ControlGate.SetInteger("Change", 2); // Close Gate
        yield return new WaitForSeconds(1f);
        click = false;
        ao.allowSceneActivation = true;
        Music._instance.PlayonGamePlay();
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        Music._instance.PlayonMenu();
    }
    public void SelectLV()
    {
        Time.timeScale = 1f;
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Manager.isFinishing = true;
           // GetComponent<Animator>().SetInteger("Change", 2);
        }
        StartCoroutine(LoadSceneSelect());
    }
    private IEnumerator LoadSceneSelect()
    {
        if (click == true) yield break;
        ao = SceneManager.LoadSceneAsync(1);
        ao.allowSceneActivation = false;
        click = true;
        ControlGate.SetInteger("Change", 2);
        yield return new WaitForSeconds(1f);
        click = false;
        ao.allowSceneActivation = true;
        Music._instance.PlayonMenu();
    }
    public void ShowUpgrade()
    {
        LevelManager.SetActive(false);
    }
    public void HideUpgrade()
    {
        LevelManager.SetActive(true);
    }
   
   
    public void Restart()
    {
        StartCoroutine(RestartGame());
    }


    IEnumerator RestartGame()
    {
        Time.timeScale = 1f;
        ControlGate.SetInteger("Change", 2); // Close Gate
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
    }
}
