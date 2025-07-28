using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance { get; private set; }

    //낚시 미니게임
    [SerializeField] GameObject p;
    int fishMoveSpeed = 500;
    Vector3 fishDir = Vector3.right;
    bool canCatch;
    [SerializeField] GameObject fishGameUI;
    [SerializeField] string[] fishs;

    //UI
    [SerializeField] GameObject inv;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //-500 ~ 500 을 왔다갔다 하도록
        p.transform.Translate(fishDir * fishMoveSpeed * Time.deltaTime);
        //p.transform.position += dir * fishMoveSpeed * Time.deltaTime;
        if (p.transform.localPosition.x >= 500f)
        {
            fishDir = Vector3.left;
        }
        else if (p.transform.localPosition.x <= -500f)
        {
            fishDir = Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canCatch)
            {
                Debug.Log("성공");
            }
            else
            {
                Debug.Log("실패");
            }
            StopFishGame();
        }

    }
    public void StartFishGame()
    {
        fishGameUI.SetActive(true);
    }
    void StopFishGame()
    {
        fishGameUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("테스트");
        if (collision.CompareTag("Target"))
        {
            canCatch = true;
            Debug.Log(canCatch);
        }
            
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            canCatch = false;
            Debug.Log(canCatch);
        }  
    }

    public void ClickBagButton()
    {
        inv.SetActive(true);
    }

    public void ClickBagExitButton()
    {
        inv.SetActive(false);
    }
}
