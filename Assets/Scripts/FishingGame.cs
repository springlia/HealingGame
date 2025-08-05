using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FishingGame : MonoBehaviour
{
    //���� �̴ϰ���
    [SerializeField] string[] fishs;
    [SerializeField] GameObject target;

    int fishMoveSpeed = 500;
    Vector3 fishDir = Vector3.right;
    bool canCatch;
    float randomSize;
    Fish caughtFish;

    //����� ������ ��������
    //[SerializeField] private List<FishData> fishDatas;
    //[SerializeField] private GameObject fishPrefab;

    private void OnEnable()
    {
        caughtFish = GameManager.Instance.GetRandomFish();
        RandomSpawnTarget();


        randomSize = Random.Range(caughtFish.minSize, caughtFish.maxSize);
        Debug.Log($"����� �̸�: {caughtFish.name}, ������: {randomSize.ToString("N2")}, ���̵�: {caughtFish.difficulty}, ����: {caughtFish.price}");
        fishMoveSpeed = 500 * caughtFish.difficulty + Random.Range(1, 100);
        //����� ���ǵ� ���� = 500 * ����� ���̵� + ������ (1~100)
    }

    private void Update()
    {
        //-500 ~ 500 �� �Դٰ��� �ϵ���
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
                GameManager.Instance.WriteLog($"{caughtFish.name}({randomSize.ToString("N2")}cm)��(��) ��Ҵ�!");
                GameManager.Instance.AddToInventory(caughtFish);
            }
            else
            {
                GameManager.Instance.WriteLog("����Ⱑ ���������ȴ�...");
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
