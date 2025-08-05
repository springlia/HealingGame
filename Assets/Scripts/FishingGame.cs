using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FishingGame : MonoBehaviour
{
    //낚시 미니게임
    [SerializeField] string[] fishs;
    [SerializeField] GameObject target;

    int fishMoveSpeed = 500;
    Vector3 fishDir = Vector3.right;
    bool canCatch;
    float randomSize;
    Fish caughtFish;

    //물고기 데이터 가져오기
    //[SerializeField] private List<FishData> fishDatas;
    //[SerializeField] private GameObject fishPrefab;

    private void OnEnable()
    {
        caughtFish = GameManager.Instance.GetRandomFish();
        RandomSpawnTarget();


        randomSize = Random.Range(caughtFish.minSize, caughtFish.maxSize);
        Debug.Log($"물고기 이름: {caughtFish.name}, 사이즈: {randomSize.ToString("N2")}, 난이도: {caughtFish.difficulty}, 가격: {caughtFish.price}");
        fishMoveSpeed = 500 * caughtFish.difficulty + Random.Range(1, 100);
        //물고기 스피드 공식 = 500 * 물고기 난이도 + 랜덤값 (1~100)
    }

    private void Update()
    {
        //-500 ~ 500 을 왔다갔다 하도록
        this.transform.Translate(fishDir * fishMoveSpeed * Time.deltaTime);
        if (this.transform.localPosition.x >= 500f)
        {
            fishDir = Vector3.left;
        }
        else if (this.transform.localPosition.x <= -500f)
        {
            fishDir = Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canCatch)
            {
                GameManager.Instance.WriteLog($"{caughtFish.name}({randomSize.ToString("N2")}cm)을(를) 잡았다!");
                GameManager.Instance.AddToInventory(caughtFish);
            }
            else
            {
                GameManager.Instance.WriteLog("물고기가 도망가버렸다...");
            }
            GameManager.Instance.StopFishGame();
        }

    }


    void RandomSpawnTarget()
    {
        float xPos = Random.Range(-425.0f, 425.0f);
        target.transform.localPosition = new Vector3 (xPos, -150, 0);
        float width = Random.Range(0.5f, 1.1f);
        target.transform.localScale = new Vector3(width, 1);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            canCatch = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            canCatch = false;
        }
    }

}
