using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    //이동
    Vector3 dir = Vector3.zero;
    [SerializeField] float speed;

    //낚시
    bool isNowFishing = false;
    bool isFishingZone = false;
    [SerializeField] GameObject fishingRod;
    float fishWaitTime;

    //상점
    bool isNowShop = false;
    bool isShopTile = false;
    //농사
    bool isFarmingZone = false;
    public TileBase tile;

    public TileBase[] farmTiles;

    bool wasMoving = false;

    //애니메이션
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
            //낚시 시작
            if (isFishingZone && !isNowFishing)
            {
                fishingRod.SetActive(true);

                isNowFishing = true;
                sr.sprite = sprites[16];

                coru = StartCoroutine(WaitFish());
            }
            //낚시 종료
            else if (isFishingZone && isNowFishing)
            {
                StopCoroutine(coru);
                fishingRod.SetActive(false);
                isNowFishing = false;
                sr.sprite = sprites[0];
            }
            //농사 시작
            else if (isFarmingZone)
            {
                Vector3Int currentPos = Vector3Int.CeilToInt(this.transform.position) - Vector3Int.right - Vector3Int.up;
                if (groundTilemap.GetTile(currentPos) == farmTiles[0]) //일반 땅 상태일때 (땅파기)
                {
                    GameManager.Instance.PlaySound("HOE");
                    groundTilemap.BoxFill(currentPos, farmTiles[1], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[1]) //땅이 파진 상태일때 (물주기)
                {
                    GameManager.Instance.PlaySound("FISH");
                    groundTilemap.BoxFill(currentPos, farmTiles[2], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[2]) //물이 있는 상태일때 (씨앗 뿌리기)
                {

                    //씨앗이 인벤토리에 있는지
                    InventoryItem seed = null;
                    foreach (InventoryItem item in GameManager.Instance.inventory)
                    {
                        if (item.cropData.name == "씨앗")
                        {
                            seed = item;
                            break;
                        }
                    }

                    //씨앗이 있으면
                    if (seed != null && seed.count > 0)
                    {
                        GameManager.Instance.PlaySound("HOE");
                        groundTilemap.BoxFill(currentPos, farmTiles[3], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                        StartCoroutine(GrowCrop(currentPos));

                        // 씨앗 1개 차감
                        seed.count--;
                        if (seed.count == 0)
                        {
                            GameManager.Instance.inventory.Remove(seed);
                        }

                        GameManager.Instance.UpdateInventoryUI();
                    }
                    //없으면
                    else
                    {
                        GameManager.Instance.PlaySound("NO");
                        GameManager.Instance.WriteLog("씨앗이 부족합니다.");
                    }

                    
                }
                else if (groundTilemap.GetTile(currentPos) == farmTiles[7]) //다 자란 상태일때 (수확하기)
                {
                    groundTilemap.BoxFill(currentPos, farmTiles[0], currentPos.x, currentPos.y, currentPos.x, currentPos.y);
                    GameManager.Instance.WriteLog("토마토를 수확했다!");
                    GameManager.Instance.PlaySound("FARM");
                    Crop tomato = null;
                    foreach (Crop crop in GameManager.Instance.crops)
                    {
                        if (crop.name == "토마토")
                        {
                            tomato = crop;
                            break;
                        }
                    }
                    GameManager.Instance.AddToInventory(tomato);

                }
            }
            //상점 이동
            else if (isShopTile)
            {
                if (isNowShop) //농장으로 이동
                {
                    isNowShop = false;
                    this.transform.position = new Vector3(-9.5f, 7.5f, 0);
                }
                else //상점으로 이동
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
        groundTilemap.BoxFill(crop, farmTiles[4], crop.x, crop.y, crop.x, crop.y); //1단계
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[5], crop.x, crop.y, crop.x, crop.y); //2단계
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[6], crop.x, crop.y, crop.x, crop.y); //3단계
        yield return new WaitForSeconds(Random.Range(2, 10));
        groundTilemap.BoxFill(crop, farmTiles[7], crop.x, crop.y, crop.x, crop.y); //성장완
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) //위
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 3];
            GameManager.Instance.PlaySound("WALK");
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) //아래
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 0];
            GameManager.Instance.PlaySound("WALK");
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) //오른
        {
            sr.sprite = sprites[GameManager.Instance.clothIndex * 4 + 2];
            GameManager.Instance.PlaySound("WALK");
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) //왼
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
