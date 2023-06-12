using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScreenScript : MonoBehaviour
{
    const string CONST_unlocked = "unlocked_characters";

    const string CONST_SELECTED = "selected_character_slot_";
    const string CONST_CHARACTER_CARDS = "character_cards_";
    const string CONST_CHARACTER_LEVELS = "character_level_";

    [SerializeField]
    GameObject characterButtonPrefab;
    [SerializeField]
    GameObject rewardWindowPrefab;

    [System.NonSerialized]
    public List<GameObject> AllCharacterPrefabs = new List<GameObject>();

    static Dictionary<string, int> characterLevels = new Dictionary<string, int>();
    static List<CharacterStatBlock> StatsByLevel = new List<CharacterStatBlock>();
    public static int GetCharacterLevel(string characterName)
    {
        int level;
        if (characterLevels.TryGetValue(characterName, out level))
            return level;
        else
            throw new System.Exception($"{characterName} level unknown");
    }
    public static CharacterStatBlock GetCharacterStats(string characterName)
    {
        var level = GetCharacterLevel(characterName);
        var levelStats = CharacterScreenScript.StatsByLevel.FirstOrDefault(x => x.characterName == characterName && x.CharacterMetaLevel == level);
        return levelStats;
    }



    int currentSlot;
    Image currentImage;

    bool choosing = false;

    public void StartSelecting(GameObject button)//(int slot, Image portraitImage)
    {
        choosing = true;
        currentSlot = button.transform.GetSiblingIndex();
        currentImage = button.GetComponentInChildren<Image>();
    }
    public void SelectCharacter(GameObject character)
    {
        if (!choosing)
            return;
        choosing = false;
        MissionController.main.SetCharacterInSlot(character, currentSlot);

        SaveLoadSystem.SaveDataString($"{CONST_SELECTED}{currentSlot}", character.name);

        var cs = character.GetComponent<Character>();
        currentImage.sprite = cs.CharacterPortrait;

        FillCollection();
    }

    public void SaveInitialLevels()
    {
        List<string> lines = new List<string>();
        lines.Add("Character,Level,(Ц)Damage,(Ц)CriticalChance,(Ц)CriticalDamagePercent,(Д)Cooldown,(Д)Projectile speed,(Д)AOE Radius,(Ц)RicochetAmount,(Д)Ricochet range,(Д)Bullet Life(sec),(Ц)Bounces,(Ц)Burst,(Д)BurstCooldown,(Ц)Targeting (0 - ближний, 1 - случайный)");
        foreach (var character in AllCharacterPrefabs)
        {
            var cs = character.GetComponent<Character>();
            for (int i = 1; i < 11; i++)
            {
                cs.Stats.CharacterMetaLevel = i;
                var line = cs.Stats.SaveToString();
                lines.Add(line);
            }
        }
        var result = string.Join(System.Environment.NewLine, lines);
        System.IO.File.WriteAllText("TDLevelSheet.csv", result);
    }
    void LoadStatsByLevels()
    {
        var textFile = (TextAsset)Resources.Load("Sheets/TDLevelSheet");
        var lines = textFile.text.Split(System.Environment.NewLine);
        for (int i = 1; i < lines.Length; i++)
        {
            CharacterStatBlock csb = new CharacterStatBlock();
            csb.FillFromString(lines[i]);
            StatsByLevel.Add(csb);
        }
    }
    void LoadCharacters()
    {
        AllCharacterPrefabs = Resources.LoadAll<GameObject>("Prefabs/Characters").ToList();
        foreach( var character in AllCharacterPrefabs)
        {
            var cs = character.GetComponent<Character>();
            var level = SaveLoadSystem.LoadDataInt($"{CONST_CHARACTER_LEVELS}{cs.CharacterName}");
            if (level == 0)
                level = 1;
            characterLevels[cs.CharacterName] = level;
        }
    }

    void LoadChosenTeam()
    {
        var ccp = GameObject.Find("ChosenCharactersPanel");
        for (int i = 0; i < 3; i++)
        {
            var name = SaveLoadSystem.LoadDataString($"{CONST_SELECTED}{i}");
            if (!string.IsNullOrEmpty(name))
            {
                var character = AllCharacterPrefabs.FirstOrDefault(x => x.name.Contains(name));
                currentSlot = i;
                currentImage = ccp.transform.GetChild(i).GetComponentInChildren<Image>();
                choosing = true;
                SelectCharacter(character);
            }
        }
    }
    public void FillCollection()
    {
        var panelCollection = transform.Find("CharacterCollectionPanel");
        var panelLocked = transform.Find("CharactersLockedPanel");
        foreach (Transform btn in panelCollection.transform)
            GameObject.Destroy(btn.gameObject);
        foreach (Transform btn in panelLocked.transform)
            GameObject.Destroy(btn.gameObject);
        foreach (var character in AllCharacterPrefabs)
        {
            if (MissionController.main.Characters.Contains(character))
                continue;
            var cs = character.GetComponent<Character>();
            int level = GetCharacterLevel(cs.CharacterName);

            var panel = level == 0 ? panelLocked : panelCollection;
            var cbutton = GameObject.Instantiate(characterButtonPrefab, panel);
            var buttonScript = cbutton.GetComponent<CharacterSelectButton>();
            buttonScript.characterPrefab = character;
        }
    }

    public void LevelupCharacter(CharacterStatBlock csb)
    {
        int level = GetCharacterLevel(csb.characterName);

        level++;
        characterLevels[csb.characterName] = level;
        SaveLoadSystem.SaveDataInt($"{CONST_CHARACTER_LEVELS}{csb.characterName}", level);
    }

    public void GetRandomReward(int count)
    {
        List<Character> shardsList = new List<Character>();
        var tmp = AllCharacterPrefabs.ToList();
        for (int i = 0; i < count; i++)
        {
            if (!tmp.Any())
                break;
            var charIndex = Random.Range(0, tmp.Count());
            var character = tmp[charIndex].GetComponent<Character>();
            var cs = tmp[charIndex].GetComponent<Character>();
            tmp.RemoveAt(charIndex);
            shardsList.Add(character);

            int level = GetCharacterLevel(cs.CharacterName);
            if (level == 0)
                LevelupCharacter(cs.Stats);

            FillCollection();
            LoadChosenTeam();
        }




        var canvas = GameObject.Find("Canvas");


        var rewarder = GameObject.Instantiate(rewardWindowPrefab, canvas.transform).GetComponentInChildren<RewardPanelScript>();
        rewarder.characters = shardsList;

    }

    public void GetRandomLevel()
    {
        var charIndex = Random.Range(0, AllCharacterPrefabs.Count());
        var cs = AllCharacterPrefabs[charIndex].GetComponent<Character>();
        LevelupCharacter(cs.Stats);

        FillCollection();
        LoadChosenTeam();
    }
    public void ResetSave()
    {
        SaveLoadSystem.ResetSave();
        foreach (var ch in characterLevels.Keys.ToList())
        {
            characterLevels[ch] = 0;
        }
        FillCollection();
        LoadChosenTeam();
    }

    void Start()
    {
        LoadCharacters();
        //SaveInitialLevels();
        LoadStatsByLevels();
        LoadChosenTeam();
        FillCollection();

    }

    void Update()
    {
        
    }
}
