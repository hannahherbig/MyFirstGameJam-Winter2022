using Godot;
using System;

public class Enemy : KinematicBody2D
{

	[Export] public float HEALTH = 10;

	[Export] private int RANDOM_DIR_TICKS = 8;
	[Export] private int SPEED = 100;
	private int GRAVITY = 200; //Pretty useless, just makes sure they stay stuck to the ground for now. Will be used if they need to jump eventually
	private int CUR_DIR_TICK = 0;
	private Vector2 DIR_LEFT;
	private Vector2 DIR_RIGHT;
	private Vector2 CUR_DIR;
	private static Random RNG;

	private bool isFacingLeft = false;

	private bool canShoot;
	private RayCast2D lineOfSight;
	private Position2D bulletSpawn;

	private AnimatedSprite sprite;

	private float TIME_BETWEEN_SHOTS = 2;
	private float CURRENT_SHOT_COOLDOWN = 0;

	private PackedScene laserScene;

	public override void _Ready()
	{
		laserScene = GD.Load<PackedScene>("res://Scenes/EnemyLaser.tscn");
		lineOfSight = GetNode<RayCast2D>("Sight");
		bulletSpawn = GetNode<Position2D>("BulletSpawn");

		sprite = GetNode<AnimatedSprite>("AnimatedSprite");

		DIR_LEFT = new Vector2(-SPEED, GRAVITY);
		DIR_RIGHT = new Vector2(SPEED, GRAVITY);
		RNG = new Random();
	}

	public override void _PhysicsProcess(float delta)
	{
		if(CUR_DIR_TICK == 0)
		{
			CUR_DIR = RNG.Next(0, 2) == 1 ? DIR_LEFT : DIR_RIGHT;
			isFacingLeft = CUR_DIR == DIR_LEFT;
			sprite.FlipH = !isFacingLeft;
			float mult = isFacingLeft ? -1 : 1;
			lineOfSight.Rotation = (float)(mult * Math.PI * 90 / 180);
		}

		CUR_DIR_TICK++;

		if (CUR_DIR_TICK >= RANDOM_DIR_TICKS)
		{
			CUR_DIR_TICK = 0;
		}

		MoveAndSlideWithSnap(CUR_DIR, new Vector2(-1, 0));
	}

    public override void _Process(float delta)
    {
        if ((lineOfSight.GetCollider() is Player) && canShoot)
		{
			//Shoot();
		}

		CURRENT_SHOT_COOLDOWN += delta;
		if (CURRENT_SHOT_COOLDOWN > TIME_BETWEEN_SHOTS)
		{
			CURRENT_SHOT_COOLDOWN = 0;
			canShoot = true;
		}
    }

	private void Shoot()
	{
		canShoot = false;
		EnemyLaser projectile = (EnemyLaser)laserScene.Instance();
		projectile.Position = bulletSpawn.GlobalPosition;
		projectile.Rotation += isFacingLeft ? 0 : (float)Math.PI;
		GetNode<Level>("/root/LevelHolder/Level").AddChild(projectile);
	}
}
