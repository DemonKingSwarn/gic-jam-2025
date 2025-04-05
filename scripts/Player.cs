using Godot;

public partial class Player : CharacterBody3D
{
    [Export] float speed = 5f;
    [Export] float jumpVel = 4.5f;
    [Export] float mouseSens = 0.03f;
    [Export] float bobFreq = 2f;
    [Export] float bobAmp = 0.08f;
    float tBob = 0f;

    float gravity = 9.8f;

    Vector3 velocity;

    [Export] Node3D head;
    [Export] Camera3D cam;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!IsOnFloor())
        {
            velocity.Y -= gravity * (float)delta;
        }

        if(Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = jumpVel;
        }

        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        Vector3 direction = (head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if(direction != null)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;
        }
        else 
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0f, speed);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0f, speed);
        }

        Velocity = velocity;
        MoveAndSlide();

        //Head Bob
        tBob += (float)delta * velocity.Length() * (IsOnFloor() ? 1f : 0f);
        var newCamTrans = cam.Transform;
        newCamTrans.Origin = cam.Transform.Origin;
        newCamTrans.Origin = HeadBob(tBob);
        cam.Transform = newCamTrans;
    }

    Vector3 HeadBob(float time)
    {
        Vector3 pos = Vector3.Zero;
        pos.Y = Mathf.Sin(time * bobFreq) * bobAmp;
        pos.X = Mathf.Cos(time * bobFreq / 2f) * bobAmp;
        return pos;
    }

    public override void _UnhandledInput(InputEvent ev)
    {
        if(ev is InputEventMouseMotion)
        {
            InputEventMouseMotion m = (InputEventMouseMotion)ev;
            head.RotateY(-m.Relative.X * mouseSens);
            cam.RotateX(-m.Relative.Y * mouseSens);
            float newCameraX = Mathf.Clamp(cam.Rotation.X, Mathf.DegToRad(-89f), Mathf.DegToRad(89f));
            cam.Rotation = new Vector3(newCameraX, cam.Rotation.Y, cam.Rotation.Z);
        }
    }
}
