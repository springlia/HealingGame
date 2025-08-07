using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    //����
    bool isNowShop = false;
    bool isShopTile = false;
    //���
    bool isFarmingZone = false;
    public TileBase tile;

    public TileBase[] farmTiles;

    bool wasMoving = false;

    //�ִϸ��̼�
    [SerializeField] Sprite[] sprites;
    [SerializeField] AnimationClip[] ani;
    SpriteRenderer sr;

    Coroutine coru;

    public Tilemap groundTilemap;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //���� ����
            if (isFishingZone && !isNowFishing)
            {
                fishingRod.SetActive(true);

                isNowFishing = true;
                sr.sprite = sprites[16];

                coru = StartCoroutine(WaitFish());
            }
            //���� ����
            else if (isFishingZone && isNowFishing)
            {
                StopCoroutine(coru);
                fishingRod.SetActive(false);
                isNowFishing = false;
                sr.sprite = sprites[0];
            }
            //��� ����
            else if (isFarmingZone)
            {
                Vector3Int currentPos = Vector3Int.CeilToInt(this.transform.position) - Vector3Int.right - Vector3Int.up;
                if (groundTilemap.GetTile(currentPos) == farmTiles[0]) //�Ϲ� �� �����϶� (���ı�)
                {
                    GameManager.Instance.PlaySound("HOE");
                    groundTilemap.BoxFill(currentPos, farmTiles[1], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[1]) //���� ���� �����϶� (���ֱ�)
                {
                    GameManager.Instance.PlaySound("FISH");
                    groundTilemap.BoxFill(currentPos, farmTiles[2], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[2]) //���� �ִ� �����϶� (���� �Ѹ���)
                {

                    //������ �κ��丮�� �ִ���
                    InventoryItem seed = null;
                    foreach (InventoryItem item in GameManager.Instance.inventory)
                    {
                        if (item.cropData.name == "����")
                        {
                            seed = item;
                            break;
                        }
                    }

                    //������ ������
                    if (seed != null && seed.count > 0)
                    {
                        GameManager.Instance.PlaySound("HOE");
                        groundTilemap.BoxFill(currentPos, farmTiles[3], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                        StartCoroutine(GrowCrop(currentPos));

                        // ���� 1�� ����
                        seed.count--;
                        if (seed.count == 0)
                        {
                            GameManager.Instance.inventory.Remove(seed);
                        }

                        GameManager.Instance.UpdateInventoryUI();
                    }
                    //������
                    else
                    {
                        GameManager.Instance.PlaySound("NO");
                        GameManager.Instance.WriteLog("������ �����մϴ�.");
                    }

                    
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[7]) //�� �ڶ� �����϶� (��Ȯ�ϱ�)
                {
                    groundTilemap.BoxFill(currentPos, farmTiles[0], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                    GameManager.Instance.WriteLog("�丶�並 ��Ȯ�ߴ�!");
                    GameManager.Instance.PlaySound("FARM");
                    Crop tomato = null;
                    foreach (Crop crop in GameManager.Instance.crops)
                    {
                        if (crop.name == "�丶��")
                        {
                            tomato = crop;
                            break;
                        }
                    }
                    GameManager.Instance.AddToInventory(tomato);

                }
            }
            //���� �̵�
            else if (isShopTile)
            {
                if (isNowShop) //�������� �̵�
                {
                    isNowShop = false;
                    this.transform.position = new Vector3(-9.5f, 7.5f, 0);
                }
                else //�������� �̵�
                {
                    isNowShop = true;
                    this.transform.position = new Vector3(64.5f, 7.5f, 0);
                }
            }
        }
    }

    IEnumerator GrowCrop(Vector3Int crop)
    {
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[4], crop.x, crop.y, crop.x, crop.y); //1�ܰ�
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[5], crop.x, crop.y, crop.x, crop.y); //2�ܰ�
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[6], crop.x, crop.y, crop.x, crop.y); //3�ܰ�
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[7], crop.x, crop.y, crop.x, crop.y); //�����
    }

    void PlayerMove()
    {
        dir.x = Input.GetAxisRaw("Horizontal");
        dir.y = Input.GetAxisRaw("Vertical");

        bool isMoving = dir != Vector3.zero;
        transform.position += dir * speed * Time.deltaTime;

        var gm = GameManager.Instance;

        if (isMoving && !wasMoving)
        {
            if (gm.audioSource.clip != gm.walking)
            {
                gm.audioSource.clip = gm.walking;
                gm.audioSource.loop = true;
                gm.audioSource.Play();
            }
            else if (!gm.audioSource.isPlaying)
            {
                gm.audioSource.Play();
            }
        }
        else if (!isMoving && wasMoving)
        {
            if (gm.audioSource.clip == gm.walking && gm.audioSource.isPlaying)
            {
                gm.audioSource.loop = false;
                gm.audioSource.Stop();
            }
        }

        wasMoving = isMoving;


    }

    public void ChangeClothNow()
    {
        sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 0];
    }

    void PlayerLook()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) //��
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 3];
            GameManager.Instance.PlaySound("WALK");
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) //�Ʒ�
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 0];
            GameManager.Instance.PlaySound("WALK");
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) //����
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 2];
            GameManager.Instance.PlaySound("WALK");
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) //��
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 1];
            GameManager.Instance.PlaySound("WALK");
        }

    }

   
    IEnumerator WaitFish()
    {
        yield return new WaitForSeconds(Random.Range(2f, 10f));
        GameManager.Instance.StartFishGame();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CanFishing"))
        {
            isFishingZone = true;
        } 
        else if (collision.CompareTag("Ground"))
        {
            isFarmingZone = true;
        }
        else if (collision.CompareTag("Shop"))
        {
            if (collision.name == "Teleport")
            {
                isShopTile = true;
            }
            else if (collision.name == "Buy")
            {
                GameManager.Instance.buyShopUI.SetActive(true);
            }
            else if (collision.name == "Sell")
            {
                GameManager.Instance.ClickBagButton();
                GameManager.Instance.sellButton.SetActive(true);
            }
            else if (collision.name == "Cloth")
            {
                GameManager.Instance.clothShopUI.SetActive(true);
            }
                
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CanFishing"))
        {
            isFishingZone = false;
        }
        else if (collision.CompareTag("Ground"))
        {
            isFarmingZone = false;
        }
        else if (collision.CompareTag("Shop"))
        {
            if (collision.name == "Teleport")
                isShopTile = false;
        }
    }
}
