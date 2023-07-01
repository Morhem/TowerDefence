using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    #region Prefabs
    GameObject gameoverPanelPrefab;
    GameObject winPanelPrefab;
    GameObject pauseWindowPrefab;
    GameObject upgradeWindowPrefab;
    public GameObject upgradeButtonPrefab;
    GameObject missionInterfacePrefab;
    GameObject chestPrefab;
    public Dictionary<Perk, Sprite> icons = new Dictionary<Perk, Sprite>();
    #endregion
    #region UI Instances
    public static MissionController main;
    RectTransform canvas;
    GameObject missionInterface;
    Button pauseButton;
    GameObject pauseWindow;
    Slider XPSlider;
    Slider HPSlider;
    TMPro.TMP_Text WaveCounterText;
    TMPro.TMP_Text LevelText;
    #endregion
    #region Player Stats
    [SerializeField]
    int PlayerHP;
    int currentHP;
    float CurrentXP;
    [SerializeField]
    float CameraZoom;
    float cameraZoomIncrease = 1;

    public float[] UpgradeCost;

    float xpMultiplier = 1;
    [System.NonSerialized]
    public int Level = 0;
    public float MultiplierHP { get { return 1; } }
    public float MultiplierXP { get { return xpMultiplier; } }

    public List<UpgradeButton> PossibleUpgrades = new List<UpgradeButton>();
    #endregion
    
    #region Gameplay
    [System.NonSerialized]
    public GameObject[] Characters = new GameObject[3];
    List<Spawner> Spawners = new List<Spawner>();
    public int currentSpawner;

    [System.NonSerialized]
    public bool upgradeInProgress = false;
    Queue<bool> pendingUpgradeWindows = new Queue<bool>();



    [System.NonSerialized]
    public int AdditionalMinibosses = 0;
    public GameObject AdditionalMinibossPrefab;

    [System.NonSerialized]
    public List<Monster> Monsters = new List<Monster>();
    [System.NonSerialized]
    public List<Character> characters = new List<Character>();

    [System.NonSerialized]
    public bool Pause = false;

    [SerializeField]
    int Revives;
    [System.NonSerialized]
    public List<Upgrade> upgrades = new List<Upgrade>();
    #endregion


    private void Awake()
    {
        main = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        Pause = true;
    }
    void ReSaveUpgradeButtonsAsScriptables()
    {
        var allUBs = Resources.LoadAll<GameObject>("UIPrefabs/Abilities");

        foreach (var ub in allUBs.Skip(1))
        {
            var ubs = ub.GetComponent<UpgradeButton>();
            if (ubs != null)
            {
                ubs.SaveSO();
            }
        }
    }
    public void LoadMission(string sceneName)
    {
        var spawners = Resources.LoadAll<Spawner>($"Prefabs/Missions/{sceneName}/Spawners");

        MissionController.main.SetSpawners(spawners);

        missionInterfacePrefab = Resources.Load<GameObject>("UIPrefabs/MissionInterface");
        gameoverPanelPrefab = Resources.Load<GameObject>("UIPrefabs/GameOverPanel");
        winPanelPrefab = Resources.Load<GameObject>("UIPrefabs/WinPanel");
        pauseWindowPrefab = Resources.Load<GameObject>("UIPrefabs/PauseWindow");
        upgradeWindowPrefab = Resources.Load<GameObject>("UIPrefabs/PanelUpgrades");
        upgradeButtonPrefab = Resources.Load<GameObject>("UIPrefabs/Abilities/AbilityCard");

        chestPrefab = Resources.Load<GameObject>("Prefabs/Chest");

        var allSprites = Resources.LoadAll<Sprite>("Sprites/Ability Icons");


        foreach (Perk perk in (Perk[])System.Enum.GetValues(typeof(Perk)))
        {
            var sprite = allSprites.FirstOrDefault(x => x.name == perk.ToString());
            if (sprite != null)
                icons[perk] = sprite;
        }
        SceneManager.LoadScene($"{sceneName}", LoadSceneMode.Single);
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {


        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            return;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        missionInterface = GameObject.Instantiate(missionInterfacePrefab, canvas);


        pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(TogglePauseWindow);

        HPSlider = GameObject.Find("HPSlider").GetComponent<Slider>();
        XPSlider = GameObject.Find("XPSlider").GetComponent<Slider>();
        WaveCounterText = GameObject.Find("WaveCounterText").GetComponent<TMPro.TMP_Text>();
        LevelText = GameObject.Find("LevelText").GetComponent<TMPro.TMP_Text>();
        currentSpawner = 0;

        foreach (var spwn in Spawners)
        {
            spwn.Initialize();
            spwn.Activated = false;
        }
        AdjustCameraZoom();
        StartGame();
    }
    public void StartGame()
    {
        SpawnCharacters();
        foreach (var monster in Monsters.ToList())
        {
            Monsters.Remove(monster);
            GameObject.Destroy(monster.gameObject);
        }

        Level = 0;
        currentHP = PlayerHP;

        SpawnCharacters();
        SetXPSlider();
        SetHPSlider();
        SetLevelText();
        SetWaveCounter(0);
        Spawners[currentSpawner].Activated = true;
        Pause = false;

    }

    public void SetSpawners(IEnumerable<Spawner> spawners)
    {
        foreach (var spawner in Spawners.ToList())
            GameObject.Destroy(spawner);
        Spawners.Clear();
        foreach(var prefab in spawners)
        {
            var spawner = GameObject.Instantiate(prefab, transform);
            Spawners.Add(spawner);
        }
    }

    public void SetCharacterInSlot(GameObject characterPrefab, int slot)
    {
        Characters[slot] = characterPrefab;
        SpawnCharacters();
    }
    public void AddXP(float amount)
    {
        CurrentXP += amount * xpMultiplier;
        if (Levelup(GetUpgradeCost()))
            CreateUpgradeWindow(false);
        SetXPSlider();    
    }
    float GetUpgradeCost()
    {
        var cost = UpgradeCost.Length > Level ? UpgradeCost[Level] : UpgradeCost.Last();
        return cost;
    }
    public bool Levelup(float amount)
    {
        if (CurrentXP >= amount)
        {
            CurrentXP -= amount;
            MissionController.main.Level++;
            SetLevelText();
            SetXPSlider();
            return true;
        }
        else
            return false;

    }

    void AdjustCameraZoom()
    {
        var cam = Camera.main;
        cam.orthographicSize = CameraZoom * cameraZoomIncrease;
    }

    void SetHPSlider()
    {
        HPSlider.maxValue = PlayerHP;
        HPSlider.value = currentHP;
    }
    void SetXPSlider()
    {
        XPSlider.maxValue = GetUpgradeCost();
        XPSlider.value = CurrentXP;
    }
    void SetLevelText()
    {
        LevelText.text = $"LVL {Level + 1}";
    }
    public void SetWaveCounter(int wave)
    {
        WaveCounterText.text = $"Wave {wave + 1}";
    }

    bool CheckUpgrade(AbilityCardScriptableObject abilityCard, bool isChest, Character character)
    {
        if (abilityCard.singularUpgrade && upgrades.Any(x => x.id == abilityCard.ID))
            return false;
        if (!isChest && abilityCard.chestOnly)
            return false;
        if (isChest && abilityCard.chestOnly && upgrades.Count(x => x.character == character) < 3)
            return false;
        return true;
    }
    public void CheckPendingUpgrades()
    {
        if (!upgradeInProgress && pendingUpgradeWindows.Any())
            CreateUpgradeWindow(pendingUpgradeWindows.Dequeue());
    }
    public void CreateUpgradeWindow(bool isChest)
    {
        if (upgradeInProgress)
        {
            pendingUpgradeWindows.Enqueue(isChest);
            return;
        }
        Pause = true;
        upgradeInProgress = true;
        //int upgradeCount = isChest ? 1 : 3;
        int upgradeCount = 3;
        var canvas = GameObject.Find("Canvas");

        var allAvailableUpgrades = PossibleUpgrades.ToList();
        foreach (var character in characters)
            allAvailableUpgrades.AddRange(character.PossibleUpgrades);

        Dictionary<AbilityCardScriptableObject, Character> tmpUpgrades = new Dictionary<AbilityCardScriptableObject, Character>();
        var commonUpgrades = Resources.LoadAll<AbilityCardScriptableObject>($"UIPrefabs/Abilities/All");
        foreach (var upgrade in commonUpgrades)
        {
            if (CheckUpgrade(upgrade, isChest, null))
                tmpUpgrades[upgrade] = null;
        }

        foreach (var character in characters)
        {
            var upgrades = Resources.LoadAll<AbilityCardScriptableObject>($"UIPrefabs/Abilities/{character.CharacterName}");

            foreach (var upgrade in upgrades)
            {
                if (CheckUpgrade(upgrade, isChest, character))
                    tmpUpgrades[upgrade] = character;
            }


            //foreach (var upgrade in tower.PossibleUpgrades)
            //{
            //    if (CheckUpgrade(upgrade, isChest))
            //        tmpUpgrades[upgrade] = tower;
            //}
        }
        var upgrader = GameObject.Instantiate(MissionController.main.upgradeWindowPrefab, canvas.transform).GetComponentInChildren<UpgradePanelScript>();
        upgrader.upgradePool = tmpUpgrades;
        upgrader.isChest = isChest;
        upgrader.upgradeCount = upgradeCount;
    }

    public void Upgrade(Upgrade upgrade, Character tower, bool isPublic)
    {
        upgrades.Add(upgrade);
        if (isPublic)
        {
            if (upgrade.perk == Perk.Range)
            {
                cameraZoomIncrease += upgrade.magnitude1;
                AdjustCameraZoom();
            }
            else if (upgrade.perk == Perk.XPMultiplier)
            {
                xpMultiplier += upgrade.magnitude1;
            }
            else if (upgrade.perk == Perk.AdditionalMiniboss)
            {
                AdditionalMinibosses += (int)upgrade.magnitude1;
            }
            else
            {
                foreach (var tw in characters)
                {
                    tw.Upgrade(upgrade);
                }
            }
        }
        else
        {
            tower.Upgrade(upgrade);
        }
    }



    public void PhaseComplete()
    {
        currentSpawner++;
        if (currentSpawner < Spawners.Count())
        {
            Spawners[currentSpawner].Activated = true;
        }
        else
        {
            Pause = true;
            var winPanel = GameObject.Instantiate(winPanelPrefab, canvas);
            winPanel.transform.SetParent(canvas);
        }
    }

    public void SpawnCharacters()
    {
        var spawnPoints = GameObject.FindGameObjectsWithTag("CharacterSlot");
        foreach (var tower in characters)
            if (tower != null && tower && tower.gameObject.activeSelf)
                GameObject.Destroy(tower.gameObject);
        characters.Clear();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (Characters.Count() > i && Characters[i] != null)
            {
                var character = GameObject.Instantiate(Characters[i], spawnPoints[i].transform.position, Quaternion.identity);
                var ts = character.GetComponent<Character>();
                characters.Add(ts);

                var levelStats = CharacterScreenScript.GetCharacterStats(ts.CharacterName);
                ts.Stats.FillFromCSB(levelStats);
            }
        }
    }

    public void SpawnRewardChest()
    {
        CreateUpgradeWindow(true);
    }

    public void GetHit(int damage)
    {
        if (Pause)
            return;
        currentHP -= damage;
        SetHPSlider();
        if (currentHP <= 0)
            GameOver();
    }

    public void Revive()
    {
        var gameoverPanel = GameObject.Find("GameOverPanel");
        GameObject.Destroy(gameoverPanel);
        currentHP = PlayerHP / 2;
        Pause = false;
    }

    public void TogglePauseWindow()
    {
        
        if (pauseWindow != null && pauseWindow.activeSelf)
        {
            GameObject.Destroy(pauseWindow);
        }
        else
        {
            pauseWindow = GameObject.Instantiate(pauseWindowPrefab, missionInterface.transform);
        }
        
    }

    void GameOver()
    {
        Pause = true;
        var gameoverPanel = GameObject.Instantiate(gameoverPanelPrefab, canvas);
        gameoverPanel.name = "GameOverPanel";
        gameoverPanel.transform.SetParent(canvas);
        gameoverPanel.transform.GetChild(0).gameObject.SetActive(Revives-- > 0);
    }
}
