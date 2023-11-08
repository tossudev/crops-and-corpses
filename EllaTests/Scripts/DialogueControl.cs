using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public  partial class DialogueControl : Control
{
	Button _buttonOpt1;
	Button _buttonOpt2;
	Button _buttonOpt3;
	Button _buttonOpt4;
	Button _buttonOpt5;
	[Export] Villager _villager;
	RichTextLabel _text;
	ColorRect _backgroundColor;
	public bool farmingTaskStarted = false;
	public bool resourcheTaskStarted = false;
	public bool findStone = false;
	public bool findWood = false;
	public bool exitDialogue = false;
	public bool attackZombies = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		_backgroundColor = GetNode<ColorRect>("ColorRect");

		_buttonOpt1 = GetNode<Button>("ColorRect/Button1");
		_buttonOpt2 = GetNode<Button>("ColorRect/Button2");
		_buttonOpt3 = GetNode<Button>("ColorRect/Button3");
		_buttonOpt4 = GetNode<Button>("ColorRect/Button4");
		_buttonOpt5 = GetNode<Button>("ColorRect/Button5");

		_text = GetNode<RichTextLabel>("ColorRect/Text");

		_text.Text = "Hello, do you need help?";
		_buttonOpt1.Text = "Yes, I need help with farm";
		_buttonOpt2.Text = "Nevermind";
		_buttonOpt3.Text = "Find some resourches";

		_buttonOpt4.Visible = false;
		_buttonOpt5.Visible = false;
		Visible = false;

	}
	/* public void AddNPC(npcControl newNpc)
	{	
		
		GD.Print("is this added?");
		NPCs.Add(newNpc);
		GD.Print(NPCs.Count);
	} */
	/* public int GetCountNPC(int count)
	{
		return count = NPCs.Count();
	} */

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(NPCs.Count);
		if (_villager.dialogueWindow == true)
			{
				DialogueWindowVisible();
			}
	}

	public void _on_button_1_button_up()
	{
		farmingTaskStarted = true;
		Visible = false;
	}

	public void _on_button_2_button_up()
	{
		exitDialogue = true;
		Visible = false;
	}

	public void _on_button_3_button_up()
	{
		resourcheTaskStarted = true;
		ResourcheGatheringD();
	}

	public void _on_button_4_button_up()
	{
		if(resourcheTaskStarted)
		{
			GD.Print("Finding stone");
			findStone = true;
		}
		Visible = false;
	}
	public void _on_button_5_button_up()
	{
		if(resourcheTaskStarted)
		{
			GD.Print("Finding Wood");
			findWood = true;
		}
		Visible = false;

	}

	private void DialogueWindowVisible()
	{
		GD.Print("Dialogue window opened");
		Visible = true;
		_villager.dialogueWindow = false;

		if (_villager.GetVillagerStates() == VillagerManager.VillagerStates.RoamAround)
		{
			GD.Print("OPEN DIALOGUE");
			_text.Text = "Hello, do you need help?";
			_buttonOpt1.Text = "Yes, I need help with farm";
			_buttonOpt2.Text = "Nevermind";
			_buttonOpt3.Text = "Find some resourches";
			_buttonOpt3.Visible = true;
			_buttonOpt4.Visible = false;
			_buttonOpt5.Visible = false;
			_buttonOpt1.Visible = true;
		}

 		if (_villager.GetVillagerStates() != VillagerManager.VillagerStates.RoamAround)
		{
			_text.Text = "I'M BUSY";
			_buttonOpt2.Text = "Nevermind";

			_buttonOpt1.Visible = false;
			_buttonOpt3.Visible = false;
			_buttonOpt4.Visible = false;
			_buttonOpt5.Visible = false;
		} 
	}

	void ResourcheGatheringD()
	{
		_text.Text = "What would you like me to find?";
		_buttonOpt5.Text = "Wood";
		_buttonOpt2.Text = "Nevermind";
		_buttonOpt4.Text = "Stone";
		_buttonOpt3.Visible = false;
		_buttonOpt1.Visible = false;
		_buttonOpt4.Visible = true;
		_buttonOpt5.Visible = true;
		Visible = true;
	}
}
