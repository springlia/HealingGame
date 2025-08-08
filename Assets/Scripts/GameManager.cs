using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string name;
    public int difficulty;
    public float price;
    public float minSize;
    public float maxSize;
    public string lore;
}


[System.Serializable]
public class InventoryItem
{
    public Item itemData;
    public int count;
}

[System.Serializable]
public class InventoryWrapper
{
    public List<InventoryItem> inventory;
}

[System.Serializable]
public class TileMapWrapper
{
    public List<string> tileNames;
}


public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance { get; private set; }

    //아이템
    public List<Item> items = new List<Item>();


    //낚시 상점
    public int fishRodLv = 1;
    const int Lv2UpPrice = 100;
    const int Lv3UpPrice = 300;
    [SerializeField] TextMeshProUGUI buyFishRodButtonText;

    //인벤토리
    public List<InventoryItem> inventory = new List<InventoryItem>();
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject inventorySlotPrefab;
    private List<InventorySlot> slotUIs = new List<InventorySlot>();
    const int InventorySize = 36;

    public Image itemInfoImage;
    public TextMeshProUGUI itemInfoNameText;
    public TextMeshProUGUI itemInfoLoreText;

    //의상
    public int clothIndex = 0;
    [SerializeField] bool[] clothUnlock;
    const int clothPrice = 500;
    [SerializeField] Image[] cloths;

    //UI
    [SerializeField] GameObject invUI;
    [SerializeField] GameObject fishGameUI;
    [SerializeField] TextMeshProUGUI logText;
    public GameObject buyShopUI;
    [SerializeField] GameObject closetUI;
    public GameObject clothShopUI;
    [SerializeField] GameObject itemInfoUI;

    [SerializeField] GameObject optionUI;

    //상점
    public GameObject sellButton;
    private InventoryItem selectedItem = null;

    public float money = 0;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GameObject player;
    Player p;

    //사운드
    public AudioClip walking;
    public AudioClip farming;
    public AudioClip fishing;
    public AudioClip shopping;
    public AudioClip hoeing;
    public AudioClip no;
    public AudioSource audioSource;
    AudioSource bgmAudio;

    private void Awake()
    {
        Instance = this;

        p = player.GetComponent<Player>();
        AddItem();
        audioSource = GetComponent<AudioSource>();
        LoadGameData();

        for (int i = 0; i < InventorySize; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            slotUIs.Add(slotObj.GetComponent<InventorySlot>());
        }

        // 저장된 게임이 없다면 최초 시작으로 판단
        if (!PlayerPrefs.HasKey("hasStarted"))
        {
            // 씨앗 1개 지급
            Item seed = null;
            foreach (Item item in GameManager.Instance.items)
            {
                if (item.name == "씨앗")
                {
                    seed = item;
                    break;
                }
            }
            AddToInventory(seed);

            // 최초 시작 마크 저장
            PlayerPrefs.SetInt("hasStarted", 1);
            PlayerPrefs.Save();
        }

        //시작 시 정보들 업데이트
        UpdateMoney(0);
        clothUnlock[0] = true;
        UpdateBuyFishRodUI();
        UpdateInventoryUI();
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "WALK":
                audioSource.clip = walking;
                break;
            case "FARM":
                audioSource.clip = farming;
                break;
            case "FISH":
                audioSource.clip = fishing;
                break;
            case "SHOP":
                audioSource.clip = shopping;
                break;
            case "HOE":
                audioSource.clip = hoeing;
                break;
            case "NO":
                audioSource.clip = no;
                break;
        }
        audioSource.Play();
    }

    //인벤토리 아이템 추가 (낚시류)
    public void AddToInventory(Item i)
    {
        // 이미 있는 항목이면 수량만 증가
        foreach (var item in inventory)
        {
            if (item.itemData != null && item.itemData.name == i.name)
            {
                item.count++;
                UpdateInventoryUI();
                return;
            }
        }
        // 새로 추가
        if (inventory.Count < InventorySize)
        {
            inventory.Add(new InventoryItem { itemData = i, count = 1 });
            UpdateInventoryUI();
        }
        else
        {
            WriteLog("인벤토리가 가득 찼습니다.");
        }
    }
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < inventory.Count)
            {
                Sprite itemIcon = null;
                string name = "";

                if (inventory[i].itemData != null)
                {
                    name = inventory[i].itemData.name;
                }
                itemIcon = GetItemIcon(name);
                slotUIs[i].AddItem(itemIcon, inventory[i].count, inventory[i]);
            }
            else
            {
                slotUIs[i].RemoveItem();
            }
        }
    }
    public Sprite GetItemIcon(string name)
    {
        return Resources.Load<Sprite>($"Icons/{name}"); // Resources/Icons 폴더에 저장
    }

    void AddItem()
    {
        items.Add(new Item { name = "고등어", difficulty = 2, price = 13.0f, minSize = 60, maxSize = 168, lore = "바다를 빠르게 헤엄치는 고단백 생선." });
        items.Add(new Item { name = "연어", difficulty = 2, price = 15.0f, minSize = 60, maxSize = 168, lore = "강에서 바다로, 다시 강으로 돌아오는 강인한 여정의 주인공." });
        items.Add(new Item { name = "복어", difficulty = 3, price = 30.0f, minSize = 3, maxSize = 94, lore = "맹독을 지닌 신비로운 생선, 신중히 다뤄야 한다." });
        items.Add(new Item { name = "참치", difficulty = 2, price = 15.0f, minSize = 30, maxSize = 155, lore = "넓은 바다를 누비는 강력한 사냥꾼." });
        items.Add(new Item { name = "도미", difficulty = 1, price = 9.0f, minSize = 30, maxSize = 79, lore = "고급 요리에 자주 쓰이는 맛좋은 생선." });
        items.Add(new Item { name = "해삼", difficulty = 3, price = 35.0f, minSize = 8, maxSize = 53, lore = "바다 밑바닥의 청소부, 영양가 높은 식재료." });
        items.Add(new Item { name = "청어", difficulty = 2, price = 20.0f, minSize = 20, maxSize = 53, lore = "무리지어 다니며 바다 생태계의 중요한 고리." });
        items.Add(new Item { name = "숭어", difficulty = 1, price = 10.0f, minSize = 20, maxSize = 58, lore = "담수와 해수를 오가는 적응력이 뛰어난 물고기." });
        items.Add(new Item { name = "장어", difficulty = 3, price = 35.0f, minSize = 30, maxSize = 206, lore = "긴 몸을 가진 야행성 사냥꾼." });
        items.Add(new Item { name = "문어", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124, lore = "똑똑한 두뇌와 팔로 적을 교묘히 제압한다." });
        items.Add(new Item { name = "붉은 퉁돔", difficulty = 1, price = 9.0f, minSize = 20, maxSize = 66, lore = "맑은 바다에서 자라는 소박한 맛의 생선." });
        items.Add(new Item { name = "오징어", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124, lore = "순발력과 잽싸기로 바다에서 살아남는 자." });
        items.Add(new Item { name = "멸치", difficulty = 1, price = 6.0f, minSize = 3, maxSize = 43, lore = "작지만 바다의 맛을 좌우하는 중요한 재료." });
        items.Add(new Item { name = "정어리", difficulty = 1, price = 8.0f, minSize = 3, maxSize = 33, lore = "영양가 풍부한 무리 생활의 대명사." });
        items.Add(new Item { name = "바닷가재", difficulty = 1, price = 5.0f, minSize = 3, maxSize = 20, lore = "귀족 요리의 주인공, 단단한 집게발이 특징." });
        items.Add(new Item { name = "날개다랑어", difficulty = 1, price = 10.0f, minSize = 51, maxSize = 104, lore = "빠른 속도로 바다를 가르는 다랑어의 변종." });
        items.Add(new Item { name = "조개", difficulty = 1, price = 3.0f, minSize = 2, maxSize = 5, lore = "조용히 바다 바닥에서 자라는 해산물." });
        items.Add(new Item { name = "쓰레기", difficulty = 1, price = 0.0f, minSize = 1, maxSize = 5, lore = "바다의 불청객, 조심히 처리해야 한다." });
        items.Add(new Item { name = "해초", difficulty = 1, price = 1.0f, minSize = 1, maxSize = 5, lore = "바다의 산소를 만드는 중요한 식물." });
        items.Add(new Item { name = "전설의 물고기", difficulty = 5, price = 50.0f, minSize = 10, maxSize = 50, lore = "오래된 전설 속에만 존재한다는 신비로운 생명체." });
        items.Add(new Item { name = "씨앗", difficulty = 0, price = 5.0f, minSize = 0, maxSize = 0, lore = "이렇게 작은 씨앗이 어떻게 자라날까요?" });
        items.Add(new Item { name = "토마토", difficulty = 0, price = 35.0f, minSize = 0, maxSize = 0, lore = "달콤하면서도 새콤한 맛이 조화를 이루는 인기 만점의 채소." });
    }
    public Item GetRandomFish()
    {
        return items[Random.Range(0, items.Count-2)];
    }

    public void StartFishGame()
    {
        fishGameUI.SetActive(true);
    }
    public void StopFishGame()
    {
        fishGameUI.SetActive(false);
    }

    public void ClickBagButton()
    {
        invUI.SetActive(true);
    }

    public void ClickBagExitButton()
    {
        itemInfoUI.SetActive(false);
        closetUI.SetActive(false);
        
        invUI.SetActive(false);
        sellButton.SetActive(false);
        selectedItem = null;
    }

    public void ClickShopExitButton()
    {
        buyShopUI.SetActive(false);
        clothShopUI.SetActive(false);
    }

    public void ItemInfo(InventoryItem item)
    {
        if (item == null)
        {
            return;
        }
        selectedItem = item;
        if (item.itemData != null)
        {
            itemInfoImage.sprite = GetItemIcon(item.itemData.name);
            itemInfoNameText.text = item.itemData.name;
            itemInfoLoreText.text = item.itemData.lore;
        }
        itemInfoUI.SetActive(true);
    }

    //로그 표기 함수
    public void WriteLog(string Log)
    {
        logText.text = Log;
        StartCoroutine(RemoveLog());
    }

    IEnumerator RemoveLog()
    {
        yield return new WaitForSeconds(3);
        logText.text = "";
    }

    public void UpdateMoney(float plusMoney) //돈 업데이트
    {
        money += plusMoney;
        moneyText.text = money + " 원";
    }

    public void BuySeed()
    {
        if (money >= 7)
        {
            UpdateMoney(-7);
            WriteLog("씨앗을 구매했습니다.");
            PlaySound("SHOP");
            //초기 씨앗 1개 지급
            Item seed = null;
            foreach (Item item in GameManager.Instance.items)
            {
                if (item.name == "씨앗")
                {
                    seed = item;
                    break;
                }
            }
            AddToInventory(seed);
        }
        else
        {
            PlaySound("NO");
            WriteLog("돈이 부족합니다.");
        }
    }

    public void UpdateBuyFishRodUI()
    {
        if (fishRodLv == 1)
        {
            buyFishRodButtonText.text = $"\n\n\n\n\n낚싯대 업그레이드\n현재: Lv{fishRodLv}\n{Lv2UpPrice}";
        }
        else if (fishRodLv == 2)
        {
            buyFishRodButtonText.text = $"\n\n\n\n\n낚싯대 업그레이드\n현재: Lv{fishRodLv}\n{Lv3UpPrice}";
        }
        else if (fishRodLv == 3)
        {
            buyFishRodButtonText.text = $"\n\n\n\n\n낚싯대 업그레이드\n현재: {fishRodLv}";
        }
    }

    public void BuyFishRod()
    {
        if (fishRodLv == 1 && money >= Lv2UpPrice)
        {
            fishRodLv++;
            UpdateMoney(-Lv2UpPrice);
            WriteLog("낚싯대 레벨이 올랐습니다!");
            UpdateBuyFishRodUI();
            PlaySound("SHOP");
        }
        else if (fishRodLv == 2 && money >= Lv3UpPrice)
        {
            fishRodLv++;
            UpdateMoney(-Lv3UpPrice);
            WriteLog("낚싯대 레벨이 올랐습니다!");
            UpdateBuyFishRodUI();
            PlaySound("SHOP");
        }
        else if (fishRodLv == 3)
        {
            WriteLog("이미 최대 레벨의 낚시대입니다.");
            PlaySound("NO");
        }
        else
        {
            WriteLog("돈이 부족합니다.");
            PlaySound("NO");
        }
    }

    public void SellItem()
    {
        if (selectedItem == null)
        {
            WriteLog("판매할 아이템을 선택해주세요.");
            PlaySound("NO");
            return;
        }

        string name = "";
        float price = 0;

        //아이템 정보 불러오기
        if (selectedItem.itemData != null)
        {
            name = selectedItem.itemData.name;
            price = selectedItem.itemData.price;
        }

        UpdateMoney(price);
        PlaySound("SHOP");
        // 아이템 하나 제거
        selectedItem.count--;
        if (selectedItem.count <= 0)
        {
            inventory.Remove(selectedItem);
            selectedItem = null;
            itemInfoUI.SetActive(false);
        }

        UpdateInventoryUI();
        WriteLog($"{name}을(를) {price}원에 판매했습니다.");
        PlaySound("SHOP");
    }

    public void OpenCloset()
    {
        for (int i = 1; i < 4; i++)
        {
            if (!clothUnlock[i])
            {
                cloths[i].color = Color.gray;
            }
        }
        closetUI.SetActive(true);
    }

    //옷 변경 함수
    public void ChangeCloth(int cloth)
    {
        if (clothUnlock[cloth])
        {
            clothIndex = cloth;
            WriteLog("의상 변경 완료!");
            PlaySound("SHOP");
            p.ChangeClothNow();
        }
        else
        {
            WriteLog("보유하고 있지 않은 의상입니다.");
            PlaySound("NO");
        }
        
    }

    public void BuyCloth(int cloth)
    {
        if (clothUnlock[cloth])
        {
            WriteLog("이미 보유하고 있는 의상입니다.");
            PlaySound("NO");
            return;
        }
        if (money >= clothPrice)
        {
            WriteLog("의상 구매 완료!");
            PlaySound("SHOP");
            cloths[cloth].color = Color.white; //옷장에서 잠금해제
            clothUnlock[cloth] = true;
            UpdateMoney(-clothPrice);
            
        }
        else
        {
            WriteLog("돈이 부족합니다.");
            PlaySound("NO");
        }
        
    }

    public void ClickOptionButton(string option)
    {
        if (option == "Exit") //옵션버튼 닫기
        {
            optionUI.SetActive(false);
        }
        if (option == "Open") //옵션버튼 열기
        {
            optionUI.SetActive(true);
        }
    }


    void SaveGameData()
    {
        // 기본 데이터 저장
        PlayerPrefs.SetInt("clothIndex", clothIndex);
        PlayerPrefs.SetFloat("money", money);
        PlayerPrefs.SetInt("fishRodLv", fishRodLv);

        // 의상 해금 정보 저장
        for (int i = 0; i < clothUnlock.Length; i++)
        {
            PlayerPrefs.SetInt($"clothUnlock_{i}", clothUnlock[i] ? 1 : 0);
        }

        // 인벤토리 저장 (JSON 직렬화)
        string invJson = JsonUtility.ToJson(new InventoryWrapper { inventory = inventory });
        PlayerPrefs.SetString("inventory", invJson);

        // 플레이어 위치 저장
        Vector3 pos = player.transform.position;
        PlayerPrefs.SetFloat("playerPosX", pos.x);
        PlayerPrefs.SetFloat("playerPosY", pos.y);
        PlayerPrefs.SetFloat("playerPosZ", pos.z);

        // 농장 타일맵 저장 (간단하게 타일 이름만 저장하는 예)
        List<string> tileNames = new List<string>();
        BoundsInt bounds = p.groundTilemap.cellBounds;
        foreach (Vector3Int posTile in bounds.allPositionsWithin)
        {
            TileBase tile = p.groundTilemap.GetTile(posTile);
            tileNames.Add(tile != null ? tile.name : "null");
        }
        PlayerPrefs.SetString("groundTilemap", JsonUtility.ToJson(new TileMapWrapper { tileNames = tileNames }));

        PlayerPrefs.Save();
    }
    void LoadGameData()
    {
        clothIndex = PlayerPrefs.GetInt("clothIndex", 0);
        money = PlayerPrefs.GetFloat("money", 0);
        fishRodLv = PlayerPrefs.GetInt("fishRodLv", 1);

        for (int i = 0; i < clothUnlock.Length; i++)
        {
            clothUnlock[i] = PlayerPrefs.GetInt($"clothUnlock_{i}", 0) == 1;
        }

        // 인벤토리 로드
        string invJson = PlayerPrefs.GetString("inventory", "");
        if (!string.IsNullOrEmpty(invJson))
        {
            InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(invJson);
            inventory = wrapper.inventory;
        }

        // 플레이어 위치
        Vector3 pos = new Vector3(
            PlayerPrefs.GetFloat("playerPosX", player.transform.position.x),
            PlayerPrefs.GetFloat("playerPosY", player.transform.position.y),
            PlayerPrefs.GetFloat("playerPosZ", player.transform.position.z));
        player.transform.position = pos;

        // 타일맵 로드
        string tileJson = PlayerPrefs.GetString("groundTilemap", "");
        if (!string.IsNullOrEmpty(tileJson))
        {
            TileMapWrapper mapWrapper = JsonUtility.FromJson<TileMapWrapper>(tileJson);
            BoundsInt bounds = p.groundTilemap.cellBounds;
            int i = 0;
            foreach (Vector3Int posTile in bounds.allPositionsWithin)
            {
                if (i >= mapWrapper.tileNames.Count) break;
                string tileName = mapWrapper.tileNames[i++];
                if (tileName == "null")
                {
                    p.groundTilemap.SetTile(posTile, null);
                }
                else
                {
                    TileBase found = System.Array.Find(p.farmTiles, t => t.name == tileName);
                    if (found != null)
                        p.groundTilemap.SetTile(posTile, found);
                }
            }
        }

        UpdateInventoryUI();

        p.ResumeCropsAfterLoad();
    }

    public void GameExit()
    {
        SaveGameData();
        //이것저것 저장하기 PlayerPrefs
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
