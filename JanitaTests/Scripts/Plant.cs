using Godot;
using Godot.NativeInterop;
using System;

public partial class Plant : Node2D
{
	#region plant info variables
	[Export] PlantType _plantType;
	[Export] string _seedName;
	[Export] string _description;
	[Export] float _growthCycleLength = 5f;
	[Export] int _maxCycles;

	[Export] Texture2D _seedTexture;
	[Export] Texture2D _sproutTexture;
	[Export] Texture2D _plantTexture;

	 public PlantType plantName
    {
        get { return _plantType; }
		set {_plantType = value;}	
    }

	public string seedName{
		get  { return _seedName; }
		set {_seedName = value;}	
	}

	public string description{
		get  { return _description; }
		set {_description = value;}	
	}

	public Texture2D seedTexture{
		get {return _seedTexture;}
		set {_seedTexture = value;}	
	}

	public Texture2D sproutTexture{
		get {return _sproutTexture;}
		set {_sproutTexture = value;}	
	}

	public Texture2D plantTexture{
		get {return _plantTexture;}
		set {_plantTexture = value;}	
	}
	[Export] CollisionObject2D _col;
	
	TextureRect trect = new TextureRect();
	#endregion
	
	#region variables for growing
	Timer _growthTimer = new Timer();

	int _currentCycle= 0;
	double _growthCycleTimer = 0f;
	FieldHandler _myField  {get; set;}
	public FieldHandler myField{
		get { return _myField;}
		set { _myField = value;}
	}
	bool _growthStarted;
	
	GrowthState _state;

	Texture2D _bugSignTexture, _waterSignTexture;
	Sprite2D _warningSign = new Sprite2D();
 	#endregion
	
	public override void _Ready()
	{
		InitializePlant();
	}

	void InitializePlant(){

		_state = GrowthState.WaitWatering;
		_col.InputEvent +=InteractWithPlant;
		trect.ExpandMode=TextureRect.ExpandModeEnum.FitWidth;
		trect.StretchMode=TextureRect.StretchModeEnum.KeepCentered;
		trect.AnchorTop = 0.5f;
        trect.AnchorRight = 0.5f;
        trect.AnchorBottom = 0.5f;
        trect.AnchorLeft = 0.5f;

		AddChild(trect);
		AddChild(_growthTimer);
		_growthTimer.Timeout += GrowthCycle;	
		trect.Texture = _sproutTexture;

		_warningSign.Position = new Vector2(-2, -422);
		_warningSign.Scale = new Vector2(3,3);
		AddChild(_warningSign);

		_bugSignTexture = ResourceLoader.Load("res://JanitaTests/Images/bugsign.png") as Texture2D;
		_waterSignTexture = ResourceLoader.Load("res://JanitaTests/Images/watersign.png") as Texture2D;
		
		PlantState();
		
	}

    void PlantState(){
		switch(_state){
			case GrowthState.WaitWatering:
				_warningSign.Texture = _waterSignTexture;
				break;
			case GrowthState.StartGrowth:
				StartGrowth();
				break;
			case GrowthState.ContinueGrowth:
				ContinueGrowth();
				break;
			case GrowthState.IsWilting:
				StartDying();
				break;
			case GrowthState.IsInfested:
				StartDying();
				break;
			case GrowthState.IsHarvestable:
				EvolvePlant();
				break;
			case GrowthState.IsDead:
				Die();
				break;
		}
	}

	public GrowthState GetGrowthState(){
		return _state;
	}
	void InteractWithPlant(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button)
		{
			// Plant is planted, wait for water so it can start to grow
			if(button.IsPressed() && _state == GrowthState.WaitWatering && FarmManager.instance.IsWaterCanEquipped()){
				WaterPlant();
			}

			// Plant is wilted, water it
			if(button.IsPressed() && _state == GrowthState.IsWilting && FarmManager.instance.IsWaterCanEquipped()){
				WaterPlant();
			}

			// Plant is infested, bug spray it
			if(button.IsPressed() && _state == GrowthState.IsInfested && FarmManager.instance.IsBugSprayEquipped()){
				CurePlant();
			}

			// Plant is ready for harvest or it is dead
			if(button.IsPressed() && _state == GrowthState.IsHarvestable || button.IsPressed() && _state == GrowthState.IsDead){
				Harvest();
			}

		}
		

	}
	void GrowthCycle(){

		// Full growth cycle has passed and player hasnt helped the plant, it will die.
		if(_state == GrowthState.IsWilting || _state == GrowthState.IsInfested){
			_state = GrowthState.IsDead;
			PlantState();
			return;
		}

		_currentCycle++;
		// Last phase reached in growth cycle, plant is ready for harvest
		if(_currentCycle == _maxCycles)
		{
			_state = GrowthState.IsHarvestable;
			PlantState();
			return;
		}

		// Else continue growth cycle, do lottery for random event
		_growthCycleTimer = 0;

        Random random = new Random();
        int randomNumber = random.Next(1, 5);

		if(randomNumber==1){
			_state = GrowthState.IsWilting;	
			PlantState();
		}
		else if(randomNumber==2){
			_state = GrowthState.IsInfested;
			PlantState();
		}
		else GD.Print("No event this growth cycle.");
	}
	void StartGrowth(){
		 GD.Print("Growing stage started for "+plantName);
        _growthStarted=true;
      	_growthTimer.WaitTime = _growthCycleLength;
    	_growthTimer.Start();
	}
	void ContinueGrowth(){
		 GD.Print(plantName+" is healthy again.");
	}
	void StartDying(){
		if(_state == GrowthState.IsWilting){
			GD.Print("Plant is wilting: "+plantName);
			_warningSign.Texture = _waterSignTexture;
		}else if(_state == GrowthState.IsInfested){
			GD.Print("Plant is infested: "+plantName);
			_warningSign.Texture = _bugSignTexture;
		}
	}
	public void WaterPlant(){
		GD.Print("Watered: "+plantName);
		_warningSign.Texture = null;
		if(_state == GrowthState.WaitWatering)
			_state = GrowthState.StartGrowth;
		else if(_state == GrowthState.IsWilting)
			_state = GrowthState.ContinueGrowth;
		PlantState();	
	}
	public void CurePlant(){
		GD.Print("Cured: "+plantName);
		_warningSign.Texture =null;
		_state = GrowthState.ContinueGrowth;
		PlantState();
	}
	void EvolvePlant(){
 		
 		GD.Print(plantName+" is fully grown and ready for harvest!");
		_state = GrowthState.IsHarvestable;
		_growthTimer.Stop();
		trect.Texture = plantTexture;
	}
	void Harvest(){
		if(_state == GrowthState.IsHarvestable){
			// Add to inventory whatever collected
			GD.Print("Harvested: "+plantName);
		}else if(_state == GrowthState.IsDead){
			GD.Print("Cleared plant: "+plantName);
		}
		
		myField.RemovePlant();
		QueueFree();
	}

	void Die(){
		GD.Print("Dead: "+plantName);
		_state = GrowthState.IsDead;
		_warningSign.Texture = null;
		trect.Modulate = new Color(0,0,0);
		_growthTimer.Stop();
	}
}
