using System.Collections;
using System.Collections.Generic;
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
    public enum FarmTile
    {
        Normal,
        Hoe,
        Water,
        Seed,
        Lv1,
        Lv2,
        Lv3,
        Finish
    }

    //�ִϸ��̼�
    [SerializeField] Sprite[] sprites;
    [SerializeField] AnimationClip[] ani;
    SpriteRenderer sr;

    public int clothIndex = 0;

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
                    groundTilemap.BoxFill(currentPos, farmTiles[1], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[1]) //���� ���� �����϶� (���ֱ�)
                {
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
                        GameManager.Instance.WriteLog("������ �����մϴ�.");
                    }

                    
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[7]) //�� �ڶ� �����϶� (��Ȯ�ϱ�)
                {
                    groundTilemap.BoxFill(currentPos, farmTiles[0], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                    GameManager.Instance.WriteLog("�丶�並 ��Ȯ�ߴ�!");
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

        this.transform.position += dir * speed * Time.deltaTime;
    }

    void PlayerLook()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) //��
        {
            sr.sprite = sprites[clothIndex * 4 + 3];
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) //�Ʒ�
        {
            sr.sprite = sprites[clothIndex * 4 + 0];
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) //����
        {
            sr.sprite = sprites[clothIndex * 4 + 2];
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) //��
        {
            sr.sprite = sprites[clothIndex * 4 + 1];
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
                GameManager.Instance.OpenShop("Buy");
            }
            else if (collision.name == "Sell")
            {
                GameManager.Instance.OpenShop("Sell");
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
