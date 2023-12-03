using Godot;
using Godot.NativeInterop;
using System;
using System.Diagnostics;

public partial class Plant : Node2D
{
	#region plant info variables
	[Export] PlantType _plantType;
	[Export] string _seedName;
	[Export] string _description;
	[Export] float _growthCycleLength = 5f;
	[Export] int _maxCycles;

	[Export] int _maxHarvestableAmount;
	[Export] Texture2D _deadTexture;
	[Export] Texture2D _sproutTexture;
	[Export] Texture2D _plantTexture;

	 public PlantType plantName
    {
        get { return _plantType; }
		set {_plantType = value;}	
    }
	
	public float growthCycleLength
    {
        get { return _growthCycleLength; }
		set { _growthCycleLength = value;}	
    }
    public int maxCycles
    {
        get { return _maxCycles; }
        set { _maxCycles = value; }
    }

    public string seedName{
		get  { return _seedName; }
		set {_seedName = value;}	
	}

	public string description{
		get  { return _description; }
		set {_description = value;}	
	}

	public Texture2D deadTexture{
		get {return _deadTexture;}
		set {_deadTexture = value;}	
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
	
	[Export]TextureRect trect;

	[Export] Item _harvestablePlant;
	#endregion
	
	#region variables for growing
	Timer _growthTimer = new Timer();

	int _currentCycle= 0;
	
	FieldHandler _myField  {get; set;}
	public FieldHandler myField{
		get { return _myField;}
		set { _myField = value;}
	}
	public bool growthStarted;

	public bool isTendedTo;
	
	GrowthState _state;

	Texture2D _bugSignTexture, _waterSignTexture;
	Sprite2D _warningSign = new Sprite2D();

	TextureProgressBar _progress = new TextureProgressBar();
	
	int _progressValue=0;
	bool _isPlayerNearby=false;
	bool _harvested=false;
	double _progressTimer;
	int index=0;

	public double currentGrowthTime{
		get {return _progress.Value;}
		set {_progress.Value = value;}	
	}
 	#endregion
	
	public override void _Ready()
	{
		InitializePlant();
		if(FarmManager.instance!=null) FarmManager.instance.AddPlantedPlant(this);
	}

	void InitializePlant(){

		_col = GetNode<Area2D>("%Area2D");
		trect = GetNode<TextureRect>("%TextureRect");
		if(myField==null){
			GetNode<TextureRect>("%TextureRect").Texture = _plantTexture;
			GD.Print(_plantType);
			_state = GrowthState.IsHarvestable;
			GetNode<TextureRect>("%TextureRect").Size= new Vector2(96, 96);
			if(_plantType == PlantType.Lupine) {
					GetNode<TextureRect>("%TextureRect").ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional;
			} 
		
			
			return;
		}
		Position = new Vector2(0, -5);
		_state = GrowthState.WaitWatering;
		
		
		AddChild(_growthTimer);
		
		_growthTimer.Timeout += GrowthCycle;	
		
		GetNode<TextureRect>("%TextureRect").Texture = _sproutTexture;
		_warningSign.Scale = new Vector2(0.06f,0.06f);
		_warningSign.Position = new Vector2(-10, 10);
		AddChild(_warningSign);

		_bugSignTexture = ResourceLoader.Load("res://assets/sprites/Farm sprites/sign_bugplant.png") as Texture2D;
		_waterSignTexture = ResourceLoader.Load("res://assets/sprites/Farm sprites/sign_waterplant.png") as Texture2D;

		var scene = ResourceLoader.Load<PackedScene>("res://scenes/farm/plant_progress_bar.tscn").Instantiate();
     	_progress = scene as TextureProgressBar;   
		AddChild(_progress);
		_progress.MaxValue = _growthCycleLength * _maxCycles;
		_progress.Value= 0;
		_progress.Hide();
		_col.InputEvent +=InteractWithPlant;
		PlantState();
		
	}

    public override void _PhysicsProcess(double delta)
    {
		if(_state == GrowthState.IsWilting || _state == GrowthState.IsInfested || _state == GrowthState.IsDead || _state == GrowthState.IsHarvestable) return;
		if(growthStarted)
		{
			_progressTimer += delta;
			if(_progressTimer>= 1)
			{
			_progress.Value += 1;
			_progressTimer=0;
			}
		}     
    }

	public void LoadPlant(double growthTime)
	{
		if(growthTime >= _progress.MaxValue)
		{
            _state = GrowthState.IsHarvestable;
			growthStarted = true;
            _warningSign.Texture = null;
        }
		

        PlantState();

		if (_plantType == PlantType.Lupine)
		{
			GetNode<TextureRect>("%TextureRect").ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional;
		}
	}

    void PlantState(){
		GD.Print(currentGrowthTime);
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
		if(@event is InputEventMouseButton button && _isPlayerNearby)
		{
			
			// Plant is planted, wait for water so it can start to grow
			if(button.IsPressed() && button.ButtonIndex == MouseButton.Left && _state == GrowthState.WaitWatering && FarmManager.instance.IsWaterBucketEquipped() ){
				WaterPlant();
			}

			// Plant is wilted, water it
			if(button.IsPressed()&& button.ButtonIndex == MouseButton.Left && _state == GrowthState.IsWilting && FarmManager.instance.IsWaterBucketEquipped()){
				WaterPlant();
			}

			// Plant is infested, bug spray it
			if(button.IsPressed()&& button.ButtonIndex == MouseButton.Left && _state == GrowthState.IsInfested && FarmManager.instance.IsBugSprayEquipped()){
				CurePlant();
			}

			// Plant is ready for harvest or it is dead
			if(button.IsPressed()&& button.ButtonIndex == MouseButton.Left && _state == GrowthState.IsHarvestable || button.IsPressed() && _state == GrowthState.IsDead){
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
	}
	void StartGrowth(){
        growthStarted=true;
      	_growthTimer.WaitTime = _growthCycleLength;
    	_growthTimer.Start();
	}
	void ContinueGrowth(){
		 _growthTimer.Stop();
		 _growthTimer.Start();
	}
	void StartDying(){
		if(_state == GrowthState.IsWilting){
			_warningSign.Texture = _waterSignTexture;
		}else if(_state == GrowthState.IsInfested){
			_warningSign.Texture = _bugSignTexture;
		}
	}
	public void WaterPlant(){
		FarmManager.instance.EmptyWaterBucket();
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

        if (_plantType == PlantType.Lupine)
        {
            GetNode<TextureRect>("%TextureRect").ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional;
			Position = new Vector2(0, -25);
        }
        _progress.Hide();
        _state = GrowthState.IsHarvestable;
		_growthTimer.Stop();
		trect.Texture = plantTexture;
	}
	async void Harvest(){
		if(_state == GrowthState.IsHarvestable && !_harvested){
			_harvested=true;
			// Add to inventory whatever collected
			 Random random = new Random();
        	 int randomAmount = random.Next(1, _maxHarvestableAmount);
			GD.Print("Harvested: " +plantName +" x"+randomAmount);
			RawInventoryItem _plant = new RawInventoryItem(_harvestablePlant.ID, _harvestablePlant.Name, randomAmount, _harvestablePlant.StackSize);
			await PlayerInventoryController.AddItemToInventory(_plant);
		}else if(_state == GrowthState.IsDead){
			GD.Print("Cleared plant: "+plantName);
		}
		
		if(myField!=null)myField.RemovePlant();
		QueueFree();
	}

	public void Die(){

		 if (_plantType == PlantType.Lupine)
        {
            GetNode<TextureRect>("%TextureRect").ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional;
			Position = new Vector2(0, -25);
        }
		_progress.Hide();
		_state = GrowthState.IsDead;
		_warningSign.Texture = null;
		trect.Texture = deadTexture;
		_growthTimer.Stop();
	}

	private void OnInteractable(Area2D body)
	{
		_isPlayerNearby=true;
	}

	private void OnNonInteractable(Area2D body)
	{
		_isPlayerNearby=false;
	}

	void ShowProgress(){
		if(_state == GrowthState.IsDead || _state == GrowthState.IsHarvestable) 
			return;

		if(!_progress.Visible) _progress.Show();
	}

	void HideProgress(){
		if(_progress.Visible) _progress.Hide();
	}
}
