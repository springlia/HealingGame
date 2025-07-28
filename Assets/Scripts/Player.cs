using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    //�̵�
    Vector3 dir = Vector3.zero;
    [SerializeField] float speed;

    //����
    bool isNowFishing = false;
    bool isFishingZone = false;
    [SerializeField] GameObject fishingRod;
    float fishWaitTime;


    //�ִϸ��̼�
    [SerializeField] Sprite[] sprites;
    [SerializeField] AnimationClip[] ani;
    SpriteRenderer sr;

    Coroutine coru;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isNowFishing)
        {
            PlayerMove();

            PlayerLook();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isFishingZone && !isNowFishing) 
        {
            Debug.Log("���� ����");
            

            fishingRod.SetActive(true);

            isNowFishing = true;
            sr.sprite = sprites[4];
  
            coru = StartCoroutine(WaitFish());
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isFishingZone && isNowFishing)
        {
            Debug.Log("���� ����");
            StopCoroutine(coru);
            fishingRod.SetActive(false);
            isNowFishing = false;
            sr.sprite = sprites[0];
        }
    }

    void PlayerMove()
    {
        dir.x = Input.GetAxisRaw("Horizontal");
        dir.y = Input.GetAxisRaw("Vertical");

        this.transform.position += dir * speed * Time.deltaTime;
    }
    void PlayerLook()
    {
        if (Input.GetKeyDown(KeyCode.W)) //��
        {
            sr.sprite = sprites[3];
        }
        else if (Input.GetKeyDown(KeyCode.S)) //�Ʒ�
        {
            sr.sprite = sprites[0];
        }
        else if (Input.GetKeyDown(KeyCode.D)) //����
        {
            sr.sprite = sprites[2];
        }
        else if (Input.GetKeyDown(KeyCode.A)) //��
        {
            sr.sprite = sprites[1];
        }
    }

    IEnumerator WaitFish()
    {
        yield return new WaitForSeconds(Random.Range(2f, 10f));
        GameManager.Instance.StartFishGame();
        Debug.Log("������!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CanFishing"))
        {
            Debug.Log("���� ����");
            isFishingZone = true;
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CanFishing"))
        {
            Debug.Log("���� �Ұ���");
            isFishingZone = false;
        }
    }
}
