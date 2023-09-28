using Godot;
using System;

public partial class Plant : Node2D
{
	#region plant info variables
	[Export] PlantType _plantType;
	[Export] string _seedName;
	[Export] string _description;
	[Export] float _growthTime{ get; set; }

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

	public float growthTime{
		get { return _growthTime;}
		set { _growthTime = value;}
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
	Timer _deathTimer = new Timer();

	[Export] float _deathTime = 5f;
	double _eventTimer = 0f;
	FieldHandler _myField  {get; set;}
	public FieldHandler myField{
		get { return _myField;}
		set { _myField = value;}
	}
	bool _growthStarted;
	
	GrowthState _state;

 	#endregion
	
	public override void _Ready()
	{
		InitializePlant();
	}

    public override void _PhysicsProcess(double delta)
    {
		// If plant already harvestable or in trouble no need for new event chance
		if(_state == GrowthState.IsHarvestable || _state == GrowthState.IsDead || _state == GrowthState.IsWilting || _state == GrowthState.IsInfested) return;
        if(_growthStarted && !_growthTimer.Paused)  
		{
			_eventTimer += delta;
			if (_eventTimer > 4){
			PlantEventChance();
			_eventTimer = 0;			
			}
		}
    }
    void PlantState(){

		switch(_state){
			case GrowthState.WaitWatering:
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
				break;
		}

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
		_growthTimer.Timeout +=EvolvePlant;	
		AddChild(_deathTimer);
		_deathTimer.Timeout +=Die;	
		trect.Texture = _sproutTexture;
		
	}

	void InteractWithPlant(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button)
		{
			// Plant is planted, wait for water so it can start to grow
			if(button.IsPressed() && _state == GrowthState.WaitWatering){
				WaterPlant();
			}

			// Plant is wilted, water it
			if(button.IsPressed() && _state == GrowthState.IsWilting){
				WaterPlant();
			}

			// Plant is infested, bug spray it
			if(button.IsPressed() && _state == GrowthState.IsInfested){
				CurePlant();
			}

			// Plant is ready for harvest
			if(button.IsPressed() && _state == GrowthState.IsHarvestable){
				HarvestPlant();
			}

			// Plant is dead
			if(button.IsPressed() && _state == GrowthState.IsDead){
				ClearPlant();
			}
		}
		

	}

	void StartGrowth(){
		 GD.Print("Growing stage started for "+plantName);
        _growthStarted=true;
        _growthTimer.WaitTime = growthTime;
        _growthTimer.Start();
	}

	void ContinueGrowth(){
		 GD.Print("Growing stage continued for "+plantName);
		_growthTimer.Paused=false;
		_deathTimer.Stop();
		
	}

	void EvolvePlant(){
 		
 		GD.Print(plantName+" is fully grown and ready for harvest!");
		_state = GrowthState.IsHarvestable;
		_growthTimer.Stop();
		trect.Texture = plantTexture;

	}
	void WaterPlant(){
		GD.Print("Watered: "+plantName);
		if(_state == GrowthState.WaitWatering)
			_state = GrowthState.StartGrowth;
		else if(_state == GrowthState.IsWilting)
			_state = GrowthState.ContinueGrowth;
		PlantState();	
	}

	void PlantEventChance(){
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
		else GD.Print("No event this cycle "+ _state);
	}
	void StartDying(){
		if(_state == GrowthState.IsWilting){
			GD.Print("Plant is wilting: "+plantName);
		}else if(_state == GrowthState.IsInfested){
			GD.Print("Plant is infested: "+plantName);
		}
		_deathTimer.WaitTime = _deathTime;
		_deathTimer.Start();
		_growthTimer.Paused=true;
	}
	void CurePlant(){
		GD.Print("Cured: "+plantName);
		_state = GrowthState.ContinueGrowth;
		PlantState();
	}

	void HarvestPlant(){
		GD.Print("Harvested: "+plantName);
		myField.RemovePlant();
		QueueFree();
	}

	void ClearPlant(){
		GD.Print("Cleared: "+plantName);
		myField.RemovePlant();
		QueueFree();
	}

	void Die(){
		GD.Print("Dead: "+plantName);
		_state = GrowthState.IsDead;
		_deathTimer.Stop();
	}
}
