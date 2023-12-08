using Godot;
using System;
using System.Linq;

public partial class QuestBoardUi : Control
{
    const string MenuButtonPath = "%DiffcultyButton";
    MenuButton QuestButton;

    const string BUTTON_FOREST_NODENAME = "%ForestButton";
    Button _forestButton;

    const string BUTTON_RUINS_NODENAME = "%RuinsButton";
    Button _ruinsButton;

    const string BUTTON_CAVE_NODENAME = "%CaveButton";
    Button _caveButton;


    const string button_Dif1 = "%dif1";
    Button Dif1Button;

    const string button_Dif2 = "%dif2";
    Button Dif2Button;

    const string button_Dif3 = "%dif3";
    Button Dif3Button;

    const string BUTTON_CLOSE_CONTAINER_NODENAME = "%CloseButtonContainer";
    Button _closeButton;

    const string LABEL_QUESTTATUSTEXT_NODENAME = "%QuestStatusText";
    Label questStatusText;

    const string label_CDiff = "%Cdif";
    Label CDiffLabel;

    QuestManager questManager;
    int _selectedDiff;

    GlobalTime _globalTime;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _globalTime = GetNodeOrNull<GlobalTime>("/root/GlobalTime");

        questManager = GetNode<QuestManager>("/root/QuestManager");
        _closeButton = GetNode(BUTTON_CLOSE_CONTAINER_NODENAME).GetNode<Button>("%CloseButton");

        _forestButton = GetNode<Button>(BUTTON_FOREST_NODENAME);
        _ruinsButton = GetNode<Button>(BUTTON_RUINS_NODENAME);
        _caveButton = GetNode<Button>(BUTTON_CAVE_NODENAME);
        Dif1Button = GetNode<Button>(button_Dif1);
        Dif2Button = GetNode<Button>(button_Dif2);
        Dif3Button = GetNode<Button>(button_Dif3);
        CDiffLabel = GetNode<Label>(label_CDiff);
        questStatusText = GetNode<Label>(LABEL_QUESTTATUSTEXT_NODENAME);


        // Button mapping
        _forestButton.Pressed += () =>
        {
            questManager.StartRescueQuest(Scene.Forest, _selectedDiff);
            UpdateQuestButtons();
        };
        _ruinsButton.Pressed += () =>
        {
            questManager.StartRescueQuest(Scene.Ruins, _selectedDiff);
            UpdateQuestButtons();
        };
        _caveButton.Pressed += () =>
        {
            questManager.StartRescueQuest(Scene.Cave, _selectedDiff);
            UpdateQuestButtons();
        };

        Dif1Button.Pressed += () => SetQuestDifficulty(1);
        Dif2Button.Pressed += () => SetQuestDifficulty(2);
        Dif3Button.Pressed += () => SetQuestDifficulty(3);

        _closeButton.Pressed += CloseQuestBoard;

        UpdateQuestButtons();

        CloseQuestBoard();
    }


    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("Toggel_QuestJournal"))
        {
            if (Visible)
            {
                CloseQuestBoard();
            }
            else
            {
                OpenQuestBoard();
            }
        }

        if (@event.IsActionPressed("close_QuestBoard"))
        {
            if (Visible)
            {
                CloseQuestBoard();
            }
          
        }
    }

    void SetQuestDifficulty(int diff)
    {
        _selectedDiff = diff;
        CDiffLabel.Text = diff.ToString();
    }


    void SetLevelsActive()
    {
        _ruinsButton.Visible = TownManager.currentTownStats.isRuinsUnlocked;

        _caveButton.Visible = TownManager.currentTownStats.isMineshaftUnlocked;
    }


    // close the quest board
    void CloseQuestBoard()
    {
        Visible = false;
    }

    // open the quest board
    public void OpenQuestBoard()
    {

        // if scene is not town, return
        if (!SceneManager.IsCurrentScene(this, Scene.Town))
        {
            return;
        }
        Visible = true;
        SetLevelsActive();
        UpdateQuestButtons();
    }
       

    public int GetSelectedDifficulty()
    {
        return _selectedDiff;
    }

    // hide the quest buttons if active quest is not null
    async void UpdateQuestButtons()
    {
        var quest = await PlayerInfo.GetActiveQuest();
        
        _forestButton.Visible = false;
        _ruinsButton.Visible = false;
        _caveButton.Visible = false;

        if (quest != null)
        {
            questStatusText.Text = "Quest Already started";
            return;
        }

        if (SaveData.allVillagerRawData.Count(data => data.isTownPopulation) >=
            TownManager.currentTownStats.populationCap)
        {
            questStatusText.Text = "Population cap reached! Level up to increase population cap";
            return;
        }
        
        _forestButton.Visible = true;
        _ruinsButton.Visible = true;
        _caveButton.Visible = true;
        questStatusText.Text = "Select a difficulty and then a location to start a new Quest";
    }
}