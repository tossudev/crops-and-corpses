using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public  partial class DialogueControl : Control
{
	Button _buttonOpt1;
	Button _buttonOpt2;
	Button _buttonOpt3;
	
	 List<npcControl> NPCs = new List<npcControl>();

	RichTextLabel _nameText;
	RichTextLabel _text;
	ColorRect _backgroundColor;
	public bool farmingTaskStarted = false;
	public bool exitDialogue = false;
	public bool attackZombies = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		_backgroundColor = GetNode<ColorRect>("ColorRect");

		_buttonOpt1 = GetNode<Button>("ColorRect/Button1");
		_buttonOpt2 = GetNode<Button>("ColorRect/Button2");
		_buttonOpt3 = GetNode<Button>("ColorRect/Button3");

		_nameText = GetNode<RichTextLabel>("ColorRect/Name");
		_text = GetNode<RichTextLabel>("ColorRect/Text");

		_nameText.Text = "Name";
		_text.Text = "Hello, do you need help?";
		_buttonOpt1.Text = "Yes, I need help with farm";
		_buttonOpt2.Text = "Nevermind";
		_buttonOpt3.Text = "Attack zombies";

		Visible = false;

	}
	public void AddNPC(npcControl newNpc)
	{
		GD.Print("is this added?");
		NPCs.Add(newNpc);
		GD.Print(NPCs.Count);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(NPCs.Count);
		foreach(var NPC in NPCs)
		{
			if (NPC.dialogueWindow == true)
			{
				DialogueWindowVisible();
			}
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
		attackZombies = true;
		Visible = false;
	}

	private void DialogueWindowVisible()
	{
		GD.Print("Dialogue window opened");
		Visible = true;
		foreach(var NPC in NPCs)
		{
			NPC.dialogueWindow = false;

		if (NPC.CurrentState == npcControl.States.Patrol)
		{
			_text.Text = "Hello, do you need help?";
			_buttonOpt1.Text = "Yes, I need help with farm";
			_buttonOpt2.Text = "Nevermind";
			_buttonOpt3.Text = "Attack zombies";

			_buttonOpt1.Visible = true;
		}

		if (NPC.CurrentState == npcControl.States.TaskFarming)
		{
			_text.Text = "I'm busy";
			_buttonOpt2.Text = "Nevermind";

			_buttonOpt1.Visible = false;
		}

		if (NPC.CurrentState == npcControl.States.TaskCompleted)
		{
			_text.Text = "I did my job, do you need help with something else?";
			_buttonOpt1.Text = "Yes, I need help with farm";
			_buttonOpt2.Text = "Nevermind";
			_buttonOpt3.Text = "Attack zombies";

			_buttonOpt1.Visible = true;
		}
		}
		
	}
}
