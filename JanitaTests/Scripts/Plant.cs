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
	Timer _plantTimer = new Timer();
	FieldHandler _myField  {get; set;}
	public FieldHandler myField{
		get { return _myField;}
		set { _myField = value;}
	}
	int _myFieldIndex {get; set;}
	public int myFieldIndex{
		get { return _myFieldIndex;}
		set { _myFieldIndex = value;}
	}

	bool isWilting, isInfested, isHarvestable;

 	#endregion
	
	public override void _Ready()
	{
		InitializePlant();
		WaitGrowth();
	}

	void InitializePlant(){
		_col.InputEvent +=InteractWithPlant;
		trect.ExpandMode=TextureRect.ExpandModeEnum.FitWidth;
		trect.StretchMode=TextureRect.StretchModeEnum.KeepCentered;
		trect.SetSize(new Vector2(147, 406));
		trect.SetPosition(new Vector2(-70, -204));
		AddChild(trect);
		AddChild(_plantTimer);
		GD.Print("Kasvi maassa");
		trect.Texture = _sproutTexture;
		_plantTimer.Timeout +=EvolvePlant;	
	}

	void InteractWithPlant(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button)
		{
			if(button.IsPressed() && isHarvestable){
				GD.Print("Harvested: "+plantName);
				HarvestPlant();
			}
		}
		

	}

	void WaitGrowth(){

		 GD.Print("Growing stage started for "+plantName);
        // Set the timer's wait time (in seconds)
        _plantTimer.WaitTime = growthTime;
        _plantTimer.Start();
	}

	void EvolvePlant(){
 		
 		GD.Print(plantName+" is fully grown and ready for harvest!");
		_plantTimer.Stop();
		isHarvestable=true;
		trect.Texture = plantTexture;
	
	}

	void HarvestPlant(){
		myField.RemovePlant();
		QueueFree();
	}
}
