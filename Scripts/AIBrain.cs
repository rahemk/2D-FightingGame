using Godot;
using System;
using System.Collections.Generic;

public partial class AIBrain : Node
{
    private Player self;
    private Player opponent;
    private float actionCooldown = 0f;
    private Random rand = new Random();
    private MotionInputBuffer motionBuffer;
    private List<string> currentInputs = new List<string>(); 

    private struct SpecialAction
    {
        public string action;
        public float delay;
        public SpecialAction(string action, float delay)
        {
            this.action = action;
            this.delay = delay;
        }
    }

    private Queue<SpecialAction> specialActionQueue = new Queue<SpecialAction>();
    private float specialActionTimer = 0f;

    public override void _Ready()
    {
        CallDeferred(nameof(FindOpponent));
    }

    private void FindOpponent()
    {
        self = GetParent<Player>();

        foreach (Node node in GetTree().GetNodesInGroup("Player"))
        {
            if (node is Player p && p != self)
            {
                opponent = p;
                break;
            }
        }

        if (opponent == null)
        {
            GD.PrintErr("Could not find opponent!");
        }
    }

    public override void _Process(double delta)
    {
        if (opponent == null || !self.canMove || !self.IsOnFloor())
            return;

        if (specialActionQueue.Count > 0)
        {
            specialActionTimer -= (float)delta;
            if (specialActionTimer <= 0f)
            {
                var nextAction = specialActionQueue.Dequeue();
                SimulateInput(nextAction.action);
                specialActionTimer = nextAction.delay;
            }
            return;
        }

        actionCooldown -= (float)delta;
        if (actionCooldown <= 0)
        {
            float distance = opponent.Position.X - self.Position.X;
            float absDistance = Mathf.Abs(distance);

            if (absDistance > 200f)
            {
                int choice = rand.Next(0, 3);
                if (choice == 0)
                    Jump();
                else if (choice == 1)
                    MoveForward();
                else
                    ComboAttack();
            }
            else if (absDistance < 50f)
            {
                int choice = rand.Next(0, 4);
                if (choice == 0)
                    Jump();
                else if (choice < 3)
                    Block();
                else
                    MoveBackward();
            }
            else
            {
                int choice = rand.Next(0, 10); 
                switch (choice)
                {
                    case 0: Attack(); break;
                    case 1: ComboAttack(); break;
                    case 2: Block(); break;
                    case 3: MoveBackward(); break;
                    case 4: Jump(); break;
                    case 5:
                    case 6:
                    case 7: ThrowHadouken(); break;
                    case 8:
                    case 9: ThrowShoryuken(); break;
                }
            }

            actionCooldown = 0.3f + (float)GD.RandRange(0.0f, 0.2f);
        }
    }

    private bool IsFacingRight()
    {
        return self.FacingRight;
    }

    private void SimulateInput(string action)
    {
        string prefix = self.PlayerID == 1 ? "p1_" : "p2_";

        // Release any previously held inputs
        ReleaseCurrentInput();

        switch (action)
        {
            case "forward":
            {
                string input = prefix + (IsFacingRight() ? "right" : "left");
                Input.ActionPress(input);
                currentInputs.Add(input);
                break;
            }

            case "backward":
            {
                string input = prefix + (IsFacingRight() ? "left" : "right");
                Input.ActionPress(input);
                currentInputs.Add(input);
                break;
            }

            case "left":
            case "right":
            case "up":
            case "down":
            {
                string input = prefix + action;
                Input.ActionPress(input);
                currentInputs.Add(input);
                break;
            }

            case "down_forward":
            {
                string down = prefix + "down";
                string forward = prefix + (IsFacingRight() ? "right" : "left");
                Input.ActionPress(down);
                Input.ActionPress(forward);
                currentInputs.Add(down);
                currentInputs.Add(forward);
                break;
            }

            case "down_back":
            {
                string down = prefix + "down";
                string back = prefix + (IsFacingRight() ? "left" : "right");
                Input.ActionPress(down);
                Input.ActionPress(back);
                currentInputs.Add(down);
                currentInputs.Add(back);
                break;
            }

            case "LP":
            case "HP":
            {
                string input = prefix + action;
                Input.ActionPress(input);
                Input.ActionRelease(input);
                break;
            }
        }
    }

    private void ReleaseCurrentInput()
    {
        foreach (string input in currentInputs)
        {
            Input.ActionRelease(input);
        }
        currentInputs.Clear();
    }

    private void Attack()
    {
        int attackChoice = rand.Next(0, 2);
        SimulateInput(attackChoice == 0 ? "LP" : "HP");
    }

    private void ComboAttack()
    {
        Attack();
        specialActionQueue.Enqueue(new SpecialAction("LP", 0.05f));
        specialActionTimer = 0f;
    }

    private void Block() => SimulateInput("backward");
    private void MoveForward() => SimulateInput("forward");
    private void MoveBackward() => SimulateInput("backward");

    private void Jump()
    {
        ReleaseCurrentInput();
        SimulateInput("up");
    }

    private void ThrowHadouken()
    {
        specialActionQueue.Enqueue(new SpecialAction("down", 0.05f));
        specialActionQueue.Enqueue(new SpecialAction("down_forward", 0.05f));
        specialActionQueue.Enqueue(new SpecialAction("forward", 0.05f));
        specialActionQueue.Enqueue(new SpecialAction("LP", 0f));
        specialActionTimer = 0f;
    }

    private void ThrowShoryuken()
    {
        specialActionQueue.Enqueue(new SpecialAction("forward", 0.05f));
        specialActionQueue.Enqueue(new SpecialAction("down", 0.05f));
        specialActionQueue.Enqueue(new SpecialAction("down_forward", 0.05f));
        specialActionQueue.Enqueue(new SpecialAction("HP", 0f));
        specialActionTimer = 0f;
    }
}
