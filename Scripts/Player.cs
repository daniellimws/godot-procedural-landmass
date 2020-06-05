using Godot;
using System;

public class Player : KinematicBody
{
    [Export]
    public float MoveSpeed = 100;
    private Vector3 velocity;

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        velocity *= 0.8f;
        velocity.y = -10f;
        velocity = MoveAndSlide(velocity);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed)
            {
                if (eventKey.Scancode == (int)KeyList.W)
                    velocity.x = MoveSpeed;
                if (eventKey.Scancode == (int)KeyList.A)
                    velocity.z = -MoveSpeed;
                if (eventKey.Scancode == (int)KeyList.S)
                    velocity.x = -MoveSpeed;
                if (eventKey.Scancode == (int)KeyList.D)
                    velocity.z = MoveSpeed;
            }
            else
                velocity = Vector3.Zero;
        }
    }
}
