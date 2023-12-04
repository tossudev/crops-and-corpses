using Godot;
using System;

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
        _forestButton.Pressed += () => {
    questManager.StartRescueQuest(Scene.Forest, _selectedDiff);
    CheckIfQuestStartedToday();
  };
  _ruinsButton.Pressed += () =>{ 
    questManager.StartRescueQuest(Scene.Ruins, _selectedDiff);
    CheckIfQuestStartedToday();
  };
  _caveButton.Pressed += () => { 
    questManager.StartRescueQuest(Scene.Cave, _selectedDiff);
    CheckIfQuestStartedToday();
  };

        Dif1Button.Pressed += () => SetQuestDifficulty(1);
        Dif2Button.Pressed += () => SetQuestDifficulty(2);
        Dif3Button.Pressed += () => SetQuestDifficulty(3);

        _closeButton.Pressed += CloseQuestBoard;
        
        CheckIfQuestStartedToday();

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
                CheckIfQuestStartedToday();
            }
        }
    }

    


   void CheckIfQuestStartedToday()
{
    var activeQuest = questManager.GetActiveQuest();
    if (activeQuest != null && _globalTime != null)
    {
        if(activeQuest.GetStartDay() < _globalTime.GetDay())
        {
            questStatusText.Text = "Quest already started";
        }
        else if (activeQuest.GetStartDay() == _globalTime.GetDay())
        {
            if (activeQuest == null && _globalTime.GetDay() == questManager.GetActiveQuest().GetStartDay())
            {
                questStatusText.Text = "Quest completed today, wait for next day to start new quest";
            }
            else
            {
                questStatusText.Text = "Quest started today";
            }
        }
    }
    else
    {
        questStatusText.Text = "Can start new quest";
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
        Visible = true;
        SetLevelsActive();
    }

    public int GetSelectedDifficulty()
    {
        return _selectedDiff;
    }

    
}