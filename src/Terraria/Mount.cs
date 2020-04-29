using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Terraria
{
	public class Mount
	{
		private class DrillBeam
		{
			public Point16 curTileTarget;

			public int cooldown;

			public DrillBeam()
			{
				curTileTarget = Point16.NegativeOne;
				cooldown = 0;
			}
		}

		private class DrillMountData
		{
			public float diodeRotationTarget;

			public float diodeRotation;

			public float outerRingRotation;

			public DrillBeam[] beams;

			public int beamCooldown;

			public Vector2 crosshairPosition;

			public DrillMountData()
			{
				beams = new DrillBeam[4];
				for (int i = 0; i < beams.Length; i++)
				{
					beams[i] = new DrillBeam();
				}
			}
		}

		private class MountData
		{
			public Texture2D backTexture;

			public Texture2D backTextureGlow;

			public Texture2D backTextureExtra;

			public Texture2D backTextureExtraGlow;

			public Texture2D frontTexture;

			public Texture2D frontTextureGlow;

			public Texture2D frontTextureExtra;

			public Texture2D frontTextureExtraGlow;

			public int textureWidth;

			public int textureHeight;

			public int xOffset;

			public int yOffset;

			public int[] playerYOffsets;

			public int bodyFrame;

			public int playerHeadOffset;

			public int heightBoost;

			public int buff;

			public int extraBuff;

			public int flightTimeMax;

			public bool usesHover;

			public float runSpeed;

			public float dashSpeed;

			public float swimSpeed;

			public float acceleration;

			public float jumpSpeed;

			public int jumpHeight;

			public float fallDamage;

			public int fatigueMax;

			public bool constantJump;

			public bool blockExtraJumps;

			public int abilityChargeMax;

			public int abilityDuration;

			public int abilityCooldown;

			public int spawnDust;

			public bool spawnDustNoGravity;

			public int totalFrames;

			public int standingFrameStart;

			public int standingFrameCount;

			public int standingFrameDelay;

			public int runningFrameStart;

			public int runningFrameCount;

			public int runningFrameDelay;

			public int flyingFrameStart;

			public int flyingFrameCount;

			public int flyingFrameDelay;

			public int inAirFrameStart;

			public int inAirFrameCount;

			public int inAirFrameDelay;

			public int idleFrameStart;

			public int idleFrameCount;

			public int idleFrameDelay;

			public bool idleFrameLoop;

			public int swimFrameStart;

			public int swimFrameCount;

			public int swimFrameDelay;

			public int dashingFrameStart;

			public int dashingFrameCount;

			public int dashingFrameDelay;

			public bool Minecart;

			public bool MinecartDirectional;

			public Action<Vector2> MinecartDust;

			public Vector3 lightColor = Vector3.One;

			public bool emitsLight;
		}

		public const int None = -1;

		public const int Rudolph = 0;

		public const int Bunny = 1;

		public const int Pigron = 2;

		public const int Slime = 3;

		public const int Turtle = 4;

		public const int Bee = 5;

		public const int Minecart = 6;

		public const int UFO = 7;

		public const int Drill = 8;

		public const int Scutlix = 9;

		public const int Unicorn = 10;

		public const int MinecartMech = 11;

		public const int CuteFishron = 12;

		public const int MinecartWood = 13;

		public const int maxMounts = 14;

		public const int FrameStanding = 0;

		public const int FrameRunning = 1;

		public const int FrameInAir = 2;

		public const int FrameFlying = 3;

		public const int FrameSwimming = 4;

		public const int FrameDashing = 5;

		public const int DrawBack = 0;

		public const int DrawBackExtra = 1;

		public const int DrawFront = 2;

		public const int DrawFrontExtra = 3;

		public const int scutlixBaseDamage = 50;

		public const int drillTextureWidth = 80;

		public const float drillRotationChange = (float)Math.PI / 60f;

		public const float maxDrillLength = 48f;

		public static int currentShader = 0;

		private static MountData[] mounts;

		private static Vector2[] scutlixEyePositions;

		private static Vector2 scutlixTextureSize;

		public static Vector2 drillDiodePoint1 = new Vector2(36f, -6f);

		public static Vector2 drillDiodePoint2 = new Vector2(36f, 8f);

		public static Vector2 drillTextureSize;

		public static int drillPickPower = 210;

		public static int drillPickTime = 6;

		public static int drillBeamCooldownMax = 1;

		private MountData _data;

		private int _type;

		private bool _flipDraw;

		private int _frame;

		private float _frameCounter;

		private int _frameExtra;

		private float _frameExtraCounter;

		private int _frameState;

		private int _flyTime;

		private int _idleTime;

		private int _idleTimeNext;

		private float _fatigue;

		private float _fatigueMax;

		private bool _abilityCharging;

		private int _abilityCharge;

		private int _abilityCooldown;

		private int _abilityDuration;

		private bool _abilityActive;

		private bool _aiming;

		public List<DrillDebugDraw> _debugDraw;

		private object _mountSpecificData;

		private bool _active;

		public bool Active => _active;

		public int Type => _type;

		public int FlyTime => _flyTime;

		public int BuffType => _data.buff;

		public int BodyFrame => _data.bodyFrame;

		public int XOffset => _data.xOffset;

		public int YOffset => _data.yOffset;

		public int PlayerOffset
		{
			get
			{
				if (!_active)
				{
					return 0;
				}
				return _data.playerYOffsets[_frame];
			}
		}

		public int PlayerOffsetHitbox
		{
			get
			{
				if (!_active)
				{
					return 0;
				}
				return _data.playerYOffsets[0] - _data.playerYOffsets[_frame] + _data.playerYOffsets[0] / 4;
			}
		}

		public int PlayerHeadOffset
		{
			get
			{
				if (!_active)
				{
					return 0;
				}
				return _data.playerHeadOffset;
			}
		}

		public int HeightBoost => _data.heightBoost;

		public float RunSpeed
		{
			get
			{
				if (_type == 4 && _frameState == 4)
				{
					return _data.swimSpeed;
				}
				if (_type == 12 && _frameState == 4)
				{
					return _data.swimSpeed;
				}
				if (_type == 12 && _frameState == 2)
				{
					return _data.runSpeed + 11f;
				}
				if (_type == 5 && _frameState == 2)
				{
					float num = _fatigue / _fatigueMax;
					return _data.runSpeed + 4f * (1f - num);
				}
				return _data.runSpeed;
			}
		}

		public float DashSpeed => _data.dashSpeed;

		public float Acceleration => _data.acceleration;

		public float FallDamage => _data.fallDamage;

		public bool AutoJump => _data.constantJump;

		public bool BlockExtraJumps => _data.blockExtraJumps;

		public bool Cart
		{
			get
			{
				if (_data == null || !_active)
				{
					return false;
				}
				return _data.Minecart;
			}
		}

		public bool Directional
		{
			get
			{
				if (_data == null)
				{
					return true;
				}
				return _data.MinecartDirectional;
			}
		}

		public Action<Vector2> MinecartDust
		{
			get
			{
				if (_data == null)
				{
					return DelegateMethods.Minecart.Sparks;
				}
				return _data.MinecartDust;
			}
		}

		public Vector2 Origin => new Vector2((float)_data.textureWidth / 2f, (float)_data.textureHeight / (2f * (float)_data.totalFrames));

		public bool CanFly
		{
			get
			{
				if (!_active || _data.flightTimeMax == 0)
				{
					return false;
				}
				return true;
			}
		}

		public bool CanHover
		{
			get
			{
				if (!_active || !_data.usesHover)
				{
					return false;
				}
				return true;
			}
		}

		public bool AbilityReady => _abilityCooldown == 0;

		public bool AbilityCharging => _abilityCharging;

		public bool AbilityActive => _abilityActive;

		public float AbilityCharge => (float)_abilityCharge / (float)_data.abilityChargeMax;

		public bool AllowDirectionChange
		{
			get
			{
				int type = _type;
				if (type == 9)
				{
					return _abilityCooldown < _data.abilityCooldown / 2;
				}
				return true;
			}
		}

		public Mount()
		{
			_debugDraw = new List<DrillDebugDraw>();
			Reset();
		}

		public void Reset()
		{
			_active = false;
			_type = -1;
			_flipDraw = false;
			_frame = 0;
			_frameCounter = 0f;
			_frameExtra = 0;
			_frameExtraCounter = 0f;
			_frameState = 0;
			_flyTime = 0;
			_idleTime = 0;
			_idleTimeNext = -1;
			_fatigueMax = 0f;
			_abilityCharging = false;
			_abilityCharge = 0;
			_aiming = false;
		}

		public static void Initialize()
		{
			mounts = new MountData[14];
			MountData mountData = new MountData();
			mounts[0] = mountData;
			mountData.spawnDust = 57;
			mountData.spawnDustNoGravity = false;
			mountData.buff = 90;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 160;
			mountData.runSpeed = 5.5f;
			mountData.dashSpeed = 12f;
			mountData.acceleration = 0.09f;
			mountData.jumpHeight = 17;
			mountData.jumpSpeed = 5.31f;
			mountData.totalFrames = 12;
			int[] array = new int[mountData.totalFrames];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 30;
			}
			array[1] += 2;
			array[11] += 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 13;
			mountData.bodyFrame = 3;
			mountData.yOffset = -7;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 6;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 6;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 4;
			mountData.idleFrameDelay = 30;
			mountData.idleFrameStart = 2;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.rudolphMountTexture[0];
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.rudolphMountTexture[1];
				mountData.frontTextureExtra = Main.rudolphMountTexture[2];
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[2] = mountData;
			mountData.spawnDust = 58;
			mountData.buff = 129;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 160;
			mountData.runSpeed = 5f;
			mountData.dashSpeed = 9f;
			mountData.acceleration = 0.08f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 6.01f;
			mountData.totalFrames = 16;
			array = new int[mountData.totalFrames];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = 22;
			}
			array[12] += 2;
			array[13] += 4;
			array[14] += 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 8;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 7;
			mountData.runningFrameCount = 5;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 11;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 1;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 3;
			mountData.idleFrameDelay = 30;
			mountData.idleFrameStart = 8;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.pigronMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[1] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 128;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.8f;
			mountData.runSpeed = 4f;
			mountData.dashSpeed = 7.5f;
			mountData.acceleration = 0.13f;
			mountData.jumpHeight = 15;
			mountData.jumpSpeed = 5.01f;
			mountData.totalFrames = 7;
			array = new int[mountData.totalFrames];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = 14;
			}
			array[2] += 2;
			array[3] += 4;
			array[4] += 8;
			array[5] += 8;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 7;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 1;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 5;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.bunnyMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[3] = mountData;
			mountData.spawnDust = 56;
			mountData.buff = 130;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.5f;
			mountData.runSpeed = 4f;
			mountData.dashSpeed = 4f;
			mountData.acceleration = 0.18f;
			mountData.jumpHeight = 12;
			mountData.jumpSpeed = 8.25f;
			mountData.constantJump = true;
			mountData.totalFrames = 4;
			array = new int[mountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 20;
			}
			array[1] += 2;
			array[3] -= 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 10;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 4;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.slimeMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[6] = mountData;
			mountData.Minecart = true;
			mountData.MinecartDirectional = true;
			mountData.MinecartDust = DelegateMethods.Minecart.Sparks;
			mountData.spawnDust = 213;
			mountData.buff = 118;
			mountData.extraBuff = 138;
			mountData.heightBoost = 10;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 13f;
			mountData.dashSpeed = 13f;
			mountData.acceleration = 0.04f;
			mountData.jumpHeight = 15;
			mountData.jumpSpeed = 5.15f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 3;
			array = new int[mountData.totalFrames];
			for (int m = 0; m < array.Length; m++)
			{
				array[m] = 8;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 13;
			mountData.playerHeadOffset = 14;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 3;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 0;
			mountData.inAirFrameDelay = 0;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.minecartMountTexture;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new MountData();
			mounts[4] = mountData;
			mountData.spawnDust = 56;
			mountData.buff = 131;
			mountData.heightBoost = 26;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 2f;
			mountData.dashSpeed = 2f;
			mountData.swimSpeed = 6f;
			mountData.acceleration = 0.08f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 3.15f;
			mountData.totalFrames = 12;
			array = new int[mountData.totalFrames];
			for (int n = 0; n < array.Length; n++)
			{
				array[n] = 26;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 13;
			mountData.playerHeadOffset = 30;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 3;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = 6;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 6;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.turtleMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[5] = mountData;
			mountData.spawnDust = 152;
			mountData.buff = 132;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 0f;
			mountData.usesHover = true;
			mountData.runSpeed = 2f;
			mountData.dashSpeed = 2f;
			mountData.acceleration = 0.16f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 4f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 12;
			array = new int[mountData.totalFrames];
			for (int num = 0; num < array.Length; num++)
			{
				array[num] = 16;
			}
			array[8] = 18;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 5;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 3;
			mountData.flyingFrameDelay = 12;
			mountData.flyingFrameStart = 5;
			mountData.inAirFrameCount = 3;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 5;
			mountData.idleFrameCount = 4;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 8;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.beeMountTexture[0];
				mountData.backTextureExtra = Main.beeMountTexture[1];
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[7] = mountData;
			mountData.spawnDust = 226;
			mountData.spawnDustNoGravity = true;
			mountData.buff = 141;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 0f;
			mountData.usesHover = true;
			mountData.runSpeed = 8f;
			mountData.dashSpeed = 8f;
			mountData.acceleration = 0.16f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 4f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 8;
			array = new int[mountData.totalFrames];
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				array[num2] = 16;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 8;
			mountData.standingFrameDelay = 4;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 8;
			mountData.runningFrameDelay = 4;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 8;
			mountData.flyingFrameDelay = 4;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 8;
			mountData.inAirFrameDelay = 4;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.UFOMountTexture[0];
				mountData.frontTextureExtra = Main.UFOMountTexture[1];
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new MountData();
			mounts[8] = mountData;
			mountData.spawnDust = 226;
			mountData.buff = 142;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 1f;
			mountData.usesHover = true;
			mountData.swimSpeed = 4f;
			mountData.runSpeed = 6f;
			mountData.dashSpeed = 4f;
			mountData.acceleration = 0.16f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 4f;
			mountData.blockExtraJumps = true;
			mountData.emitsLight = true;
			mountData.lightColor = new Vector3(0.3f, 0.3f, 0.4f);
			mountData.totalFrames = 1;
			array = new int[mountData.totalFrames];
			for (int num3 = 0; num3 < array.Length; num3++)
			{
				array[num3] = 4;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 1;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 1;
			mountData.flyingFrameDelay = 12;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 8;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.drillMountTexture[0];
				mountData.backTextureGlow = Main.drillMountTexture[3];
				mountData.backTextureExtra = null;
				mountData.backTextureExtraGlow = null;
				mountData.frontTexture = Main.drillMountTexture[1];
				mountData.frontTextureGlow = Main.drillMountTexture[4];
				mountData.frontTextureExtra = Main.drillMountTexture[2];
				mountData.frontTextureExtraGlow = Main.drillMountTexture[5];
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			drillTextureSize = new Vector2(80f, 80f);
			mountData = new MountData();
			mounts[9] = mountData;
			mountData.spawnDust = 152;
			mountData.buff = 143;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 0;
			mountData.fatigueMax = 0;
			mountData.fallDamage = 0f;
			mountData.abilityChargeMax = 40;
			mountData.abilityCooldown = 20;
			mountData.abilityDuration = 0;
			mountData.runSpeed = 8f;
			mountData.dashSpeed = 8f;
			mountData.acceleration = 0.4f;
			mountData.jumpHeight = 22;
			mountData.jumpSpeed = 10.01f;
			mountData.blockExtraJumps = false;
			mountData.totalFrames = 12;
			array = new int[mountData.totalFrames];
			for (int num4 = 0; num4 < array.Length; num4++)
			{
				array[num4] = 16;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 6;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 6;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 6;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 12;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 6;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.scutlixMountTexture[0];
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.scutlixMountTexture[1];
				mountData.frontTextureExtra = Main.scutlixMountTexture[2];
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			scutlixEyePositions = new Vector2[10];
			scutlixEyePositions[0] = new Vector2(60f, 2f);
			scutlixEyePositions[1] = new Vector2(70f, 6f);
			scutlixEyePositions[2] = new Vector2(68f, 6f);
			scutlixEyePositions[3] = new Vector2(76f, 12f);
			scutlixEyePositions[4] = new Vector2(80f, 10f);
			scutlixEyePositions[5] = new Vector2(84f, 18f);
			scutlixEyePositions[6] = new Vector2(74f, 20f);
			scutlixEyePositions[7] = new Vector2(76f, 24f);
			scutlixEyePositions[8] = new Vector2(70f, 34f);
			scutlixEyePositions[9] = new Vector2(76f, 34f);
			scutlixTextureSize = new Vector2(45f, 54f);
			for (int num5 = 0; num5 < scutlixEyePositions.Length; num5++)
			{
				scutlixEyePositions[num5] -= scutlixTextureSize;
			}
			mountData = new MountData();
			mounts[10] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 162;
			mountData.heightBoost = 34;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.2f;
			mountData.runSpeed = 4f;
			mountData.dashSpeed = 12f;
			mountData.acceleration = 0.3f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 8.01f;
			mountData.totalFrames = 16;
			array = new int[mountData.totalFrames];
			for (int num6 = 0; num6 < array.Length; num6++)
			{
				array[num6] = 28;
			}
			array[3] += 2;
			array[4] += 2;
			array[7] += 2;
			array[8] += 2;
			array[12] += 2;
			array[13] += 2;
			array[15] += 4;
			mountData.playerYOffsets = array;
			mountData.xOffset = 5;
			mountData.bodyFrame = 3;
			mountData.yOffset = 1;
			mountData.playerHeadOffset = 31;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 7;
			mountData.runningFrameDelay = 15;
			mountData.runningFrameStart = 1;
			mountData.dashingFrameCount = 6;
			mountData.dashingFrameDelay = 40;
			mountData.dashingFrameStart = 9;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 1;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 15;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.unicornMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[11] = mountData;
			mountData.Minecart = true;
			mountData.MinecartDust = DelegateMethods.Minecart.SparksMech;
			mountData.spawnDust = 213;
			mountData.buff = 167;
			mountData.extraBuff = 166;
			mountData.heightBoost = 12;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 20f;
			mountData.dashSpeed = 20f;
			mountData.acceleration = 0.1f;
			mountData.jumpHeight = 15;
			mountData.jumpSpeed = 5.15f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 3;
			array = new int[mountData.totalFrames];
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				array[num7] = 9;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = -1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 11;
			mountData.playerHeadOffset = 14;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 3;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 0;
			mountData.inAirFrameDelay = 0;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.minecartMechMountTexture[0];
				mountData.frontTextureGlow = Main.minecartMechMountTexture[1];
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new MountData();
			mounts[12] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 168;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 0f;
			mountData.usesHover = true;
			mountData.runSpeed = 2f;
			mountData.dashSpeed = 1f;
			mountData.acceleration = 0.2f;
			mountData.jumpHeight = 4;
			mountData.jumpSpeed = 3f;
			mountData.swimSpeed = 16f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 23;
			array = new int[mountData.totalFrames];
			for (int num8 = 0; num8 < array.Length; num8++)
			{
				array[num8] = 12;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 2;
			mountData.bodyFrame = 3;
			mountData.yOffset = 16;
			mountData.playerHeadOffset = 31;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 8;
			mountData.runningFrameCount = 7;
			mountData.runningFrameDelay = 14;
			mountData.runningFrameStart = 8;
			mountData.flyingFrameCount = 8;
			mountData.flyingFrameDelay = 16;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 8;
			mountData.inAirFrameDelay = 6;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = 8;
			mountData.swimFrameDelay = 4;
			mountData.swimFrameStart = 15;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.cuteFishronMountTexture[0];
				mountData.backTextureGlow = Main.cuteFishronMountTexture[1];
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new MountData();
			mounts[13] = mountData;
			mountData.Minecart = true;
			mountData.MinecartDirectional = true;
			mountData.MinecartDust = DelegateMethods.Minecart.Sparks;
			mountData.spawnDust = 213;
			mountData.buff = 184;
			mountData.extraBuff = 185;
			mountData.heightBoost = 10;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 10f;
			mountData.dashSpeed = 10f;
			mountData.acceleration = 0.03f;
			mountData.jumpHeight = 12;
			mountData.jumpSpeed = 5.15f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 3;
			array = new int[mountData.totalFrames];
			for (int num9 = 0; num9 < array.Length; num9++)
			{
				array[num9] = 8;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 13;
			mountData.playerHeadOffset = 14;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 3;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 0;
			mountData.inAirFrameDelay = 0;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.minecartWoodMountTexture;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
		}

		public static int GetHeightBoost(int MountType)
		{
			if (MountType <= -1 || MountType >= 14)
			{
				return 0;
			}
			return mounts[MountType].heightBoost;
		}

		public int JumpHeight(float xVelocity)
		{
			int num = _data.jumpHeight;
			switch (_type)
			{
			case 0:
				num += (int)(Math.Abs(xVelocity) / 4f);
				break;
			case 1:
				num += (int)(Math.Abs(xVelocity) / 2.5f);
				break;
			case 4:
				if (_frameState == 4)
				{
					num += 5;
				}
				break;
			}
			return num;
		}

		public float JumpSpeed(float xVelocity)
		{
			float num = _data.jumpSpeed;
			switch (_type)
			{
			case 0:
			case 1:
				num += Math.Abs(xVelocity) / 7f;
				break;
			case 4:
				if (_frameState == 4)
				{
					num += 2.5f;
				}
				break;
			}
			return num;
		}

		public void StartAbilityCharge(Player mountedPlayer)
		{
			if (Main.myPlayer == mountedPlayer.whoAmI)
			{
				int type = _type;
				if (type == 9)
				{
					float num = Main.screenPosition.X + (float)Main.mouseX;
					float num2 = Main.screenPosition.Y + (float)Main.mouseY;
					float ai = num - mountedPlayer.position.X;
					float ai2 = num2 - mountedPlayer.position.Y;
					Projectile.NewProjectile(num, num2, 0f, 0f, 441, 0, 0f, mountedPlayer.whoAmI, ai, ai2);
					_abilityCharging = true;
				}
			}
			else
			{
				int type2 = _type;
				if (type2 == 9)
				{
					_abilityCharging = true;
				}
			}
		}

		public void StopAbilityCharge()
		{
			int type = _type;
			if (type == 9)
			{
				_abilityCharging = false;
				_abilityCooldown = _data.abilityCooldown;
				_abilityDuration = _data.abilityDuration;
			}
		}

		public bool CheckBuff(int buffID)
		{
			if (_data.buff != buffID)
			{
				return _data.extraBuff == buffID;
			}
			return true;
		}

		public void AbilityRecovery()
		{
			if (_abilityCharging)
			{
				if (_abilityCharge < _data.abilityChargeMax)
				{
					_abilityCharge++;
				}
			}
			else if (_abilityCharge > 0)
			{
				_abilityCharge--;
			}
			if (_abilityCooldown > 0)
			{
				_abilityCooldown--;
			}
			if (_abilityDuration > 0)
			{
				_abilityDuration--;
			}
		}

		public void FatigueRecovery()
		{
			if (_fatigue > 2f)
			{
				_fatigue -= 2f;
			}
			else
			{
				_fatigue = 0f;
			}
		}

		public bool Flight()
		{
			if (_flyTime <= 0)
			{
				return false;
			}
			_flyTime--;
			return true;
		}

		public void UpdateDrill(Player mountedPlayer, bool controlUp, bool controlDown)
		{
			DrillMountData drillMountData = (DrillMountData)_mountSpecificData;
			for (int i = 0; i < drillMountData.beams.Length; i++)
			{
				DrillBeam drillBeam = drillMountData.beams[i];
				if (drillBeam.cooldown > 1)
				{
					drillBeam.cooldown--;
				}
				else if (drillBeam.cooldown == 1)
				{
					drillBeam.cooldown = 0;
					drillBeam.curTileTarget = Point16.NegativeOne;
				}
			}
			drillMountData.diodeRotation = drillMountData.diodeRotation * 0.85f + 0.15f * drillMountData.diodeRotationTarget;
			if (drillMountData.beamCooldown > 0)
			{
				drillMountData.beamCooldown--;
			}
		}

		public void UseDrill(Player mountedPlayer)
		{
			if (_type != 8 || !_abilityActive)
			{
				return;
			}
			DrillMountData drillMountData = (DrillMountData)_mountSpecificData;
			if (drillMountData.beamCooldown != 0)
			{
				return;
			}
			for (int i = 0; i < drillMountData.beams.Length; i++)
			{
				DrillBeam drillBeam = drillMountData.beams[i];
				if (drillBeam.cooldown != 0)
				{
					continue;
				}
				Point16 point = DrillSmartCursor(mountedPlayer, drillMountData);
				if (!(point != Point16.NegativeOne))
				{
					break;
				}
				drillBeam.curTileTarget = point;
				int pickPower = drillPickPower;
				bool flag = mountedPlayer.whoAmI == Main.myPlayer;
				if (flag)
				{
					bool flag2 = true;
					if (WorldGen.InWorld(point.X, point.Y) && Main.tile[point.X, point.Y] != null && Main.tile[point.X, point.Y].type == 26 && !Main.hardMode)
					{
						flag2 = false;
						mountedPlayer.Hurt(mountedPlayer.statLife / 2, -mountedPlayer.direction, false, false, Lang.deathMsg(mountedPlayer.name, -1, -1, -1, 4));
					}
					if (flag2)
					{
						mountedPlayer.PickTile(point.X, point.Y, pickPower);
					}
				}
				Vector2 vector = new Vector2((float)(point.X << 4) + 8f, (float)(point.Y << 4) + 8f);
				Vector2 v = vector - mountedPlayer.Center;
				float num = v.ToRotation();
				for (int j = 0; j < 2; j++)
				{
					float num2 = num + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
					float num3 = (float)Main.rand.NextDouble() * 2f + 2f;
					Vector2 vector2 = new Vector2((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
					int num4 = Dust.NewDust(vector, 0, 0, 230, vector2.X, vector2.Y);
					Main.dust[num4].noGravity = true;
					Main.dust[num4].customData = mountedPlayer;
				}
				if (flag)
				{
					Tile.SmoothSlope(point.X, point.Y);
				}
				drillBeam.cooldown = drillPickTime;
				break;
			}
			drillMountData.beamCooldown = drillBeamCooldownMax;
		}

		private Point16 DrillSmartCursor(Player mountedPlayer, DrillMountData data)
		{
			Vector2 value = (mountedPlayer.whoAmI != Main.myPlayer) ? data.crosshairPosition : (Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY));
			Vector2 center = mountedPlayer.Center;
			Vector2 value2 = value - center;
			float num = value2.Length();
			if (num > 224f)
			{
				num = 224f;
			}
			num += 32f;
			value2.Normalize();
			Vector2 start = center;
			Vector2 end = center + value2 * num;
			Point16 tilePoint = new Point16(-1, -1);
			if (!Utils.PlotTileLine(start, end, 65.6f, delegate(int x, int y)
			{
				tilePoint = new Point16(x, y);
				for (int i = 0; i < data.beams.Length; i++)
				{
					if (data.beams[i].curTileTarget == tilePoint)
					{
						return true;
					}
				}
				if (!WorldGen.CanKillTile(x, y))
				{
					return true;
				}
				return (Main.tile[x, y] == null || Main.tile[x, y].inActive() || !Main.tile[x, y].active()) ? true : false;
			}))
			{
				return tilePoint;
			}
			return new Point16(-1, -1);
		}

		public void UseAbility(Player mountedPlayer, Vector2 mousePosition, bool toggleOn)
		{
			switch (_type)
			{
			case 9:
			{
				if (Main.myPlayer != mountedPlayer.whoAmI)
				{
					break;
				}
				mousePosition = ClampToDeadZone(mountedPlayer, mousePosition);
				Vector2 vector = default(Vector2);
				vector.X = mountedPlayer.position.X + (float)(mountedPlayer.width / 2);
				vector.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
				int num3 = (_frameExtra - 6) * 2;
				Vector2 value = default(Vector2);
				for (int i = 0; i < 2; i++)
				{
					value.Y = vector.Y + scutlixEyePositions[num3 + i].Y + (float)_data.yOffset;
					if (mountedPlayer.direction == -1)
					{
						value.X = vector.X - scutlixEyePositions[num3 + i].X - (float)_data.xOffset;
					}
					else
					{
						value.X = vector.X + scutlixEyePositions[num3 + i].X + (float)_data.xOffset;
					}
					Vector2 vector2 = mousePosition - value;
					vector2.Normalize();
					vector2 *= 14f;
					int damage = 100;
					value += vector2;
					Projectile.NewProjectile(value.X, value.Y, vector2.X, vector2.Y, 606, damage, 0f, Main.myPlayer);
				}
				break;
			}
			case 8:
				if (Main.myPlayer == mountedPlayer.whoAmI)
				{
					if (!toggleOn)
					{
						_abilityActive = false;
					}
					else if (!_abilityActive)
					{
						if (mountedPlayer.whoAmI == Main.myPlayer)
						{
							float num = Main.screenPosition.X + (float)Main.mouseX;
							float num2 = Main.screenPosition.Y + (float)Main.mouseY;
							float ai = num - mountedPlayer.position.X;
							float ai2 = num2 - mountedPlayer.position.Y;
							Projectile.NewProjectile(num, num2, 0f, 0f, 453, 0, 0f, mountedPlayer.whoAmI, ai, ai2);
						}
						_abilityActive = true;
					}
				}
				else
				{
					_abilityActive = toggleOn;
				}
				break;
			}
		}

		public bool Hover(Player mountedPlayer)
		{
			if (_frameState == 2 || _frameState == 4)
			{
				bool flag = true;
				float num = 1f;
				float num2 = mountedPlayer.gravity / Player.defaultGravity;
				if (mountedPlayer.slowFall)
				{
					num2 /= 3f;
				}
				if (num2 < 0.25f)
				{
					num2 = 0.25f;
				}
				if (_type != 7 && _type != 8 && _type != 12)
				{
					if (_flyTime > 0)
					{
						_flyTime--;
					}
					else if (_fatigue < _fatigueMax)
					{
						_fatigue += num2;
					}
					else
					{
						flag = false;
					}
				}
				if (_type == 12 && !mountedPlayer.MountFishronSpecial)
				{
					num = 0.5f;
				}
				float num3 = _fatigue / _fatigueMax;
				if (_type == 7 || _type == 8 || _type == 12)
				{
					num3 = 0f;
				}
				float num4 = 4f * num3;
				float num5 = 4f * num3;
				if (num4 == 0f)
				{
					num4 = -0.001f;
				}
				if (num5 == 0f)
				{
					num5 = -0.001f;
				}
				float num6 = mountedPlayer.velocity.Y;
				if ((mountedPlayer.controlUp || mountedPlayer.controlJump) && flag)
				{
					num4 = -2f - 6f * (1f - num3);
					num6 -= _data.acceleration * num;
				}
				else if (mountedPlayer.controlDown)
				{
					num6 += _data.acceleration * num;
					num5 = 8f;
				}
				else
				{
					int jump = mountedPlayer.jump;
				}
				if (num6 < num4)
				{
					num6 = ((!(num4 - num6 < _data.acceleration)) ? (num6 + _data.acceleration * num) : num4);
				}
				else if (num6 > num5)
				{
					num6 = ((!(num6 - num5 < _data.acceleration)) ? (num6 - _data.acceleration * num) : num5);
				}
				mountedPlayer.velocity.Y = num6;
				mountedPlayer.fallStart = (int)(mountedPlayer.position.Y / 16f);
			}
			else if (_type != 7 && _type != 8 && _type != 12)
			{
				mountedPlayer.velocity.Y += mountedPlayer.gravity * mountedPlayer.gravDir;
			}
			else if (mountedPlayer.velocity.Y == 0f)
			{
				mountedPlayer.velocity.Y = 0.001f;
			}
			if (_type == 7)
			{
				float num7 = mountedPlayer.velocity.X / _data.dashSpeed;
				if ((double)num7 > 0.95)
				{
					num7 = 0.95f;
				}
				if ((double)num7 < -0.95)
				{
					num7 = -0.95f;
				}
				float fullRotation = (float)Math.PI / 4f * num7 / 2f;
				float num8 = Math.Abs(2f - (float)_frame / 2f) / 2f;
				Lighting.AddLight((int)(mountedPlayer.position.X + (float)(mountedPlayer.width / 2)) / 16, (int)(mountedPlayer.position.Y + (float)(mountedPlayer.height / 2)) / 16, 0.4f, 0.2f * num8, 0f);
				mountedPlayer.fullRotation = fullRotation;
			}
			else if (_type == 8)
			{
				float num9 = mountedPlayer.velocity.X / _data.dashSpeed;
				if ((double)num9 > 0.95)
				{
					num9 = 0.95f;
				}
				if ((double)num9 < -0.95)
				{
					num9 = -0.95f;
				}
				float num10 = mountedPlayer.fullRotation = (float)Math.PI / 4f * num9 / 2f;
				DrillMountData drillMountData = (DrillMountData)_mountSpecificData;
				float outerRingRotation = drillMountData.outerRingRotation;
				outerRingRotation += mountedPlayer.velocity.X / 80f;
				if (outerRingRotation > (float)Math.PI)
				{
					outerRingRotation -= (float)Math.PI * 2f;
				}
				else if (outerRingRotation < -(float)Math.PI)
				{
					outerRingRotation += (float)Math.PI * 2f;
				}
				drillMountData.outerRingRotation = outerRingRotation;
			}
			return true;
		}

		public void UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
		{
			if (_frameState != state)
			{
				_frameState = state;
				_frameCounter = 0f;
			}
			if (state != 0)
			{
				_idleTime = 0;
			}
			if (_data.emitsLight)
			{
				Point point = mountedPlayer.Center.ToTileCoordinates();
				Lighting.AddLight(point.X, point.Y, _data.lightColor.X, _data.lightColor.Y, _data.lightColor.Z);
			}
			switch (_type)
			{
			case 5:
				if (state != 2)
				{
					_frameExtra = 0;
					_frameExtraCounter = 0f;
				}
				break;
			case 7:
				state = 2;
				break;
			case 9:
				if (_aiming)
				{
					break;
				}
				_frameExtraCounter += 1f;
				if (_frameExtraCounter >= 12f)
				{
					_frameExtraCounter = 0f;
					_frameExtra++;
					if (_frameExtra >= 6)
					{
						_frameExtra = 0;
					}
				}
				break;
			case 8:
			{
				if (state != 0 && state != 1)
				{
					break;
				}
				Vector2 position = default(Vector2);
				position.X = mountedPlayer.position.X;
				position.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
				int num3 = (int)(position.X / 16f);
				float num15 = position.Y / 16f;
				float num4 = 0f;
				float num5 = mountedPlayer.width;
				while (num5 > 0f)
				{
					float num6 = (num3 + 1) * 16;
					float num7 = num6 - position.X;
					if (num7 > num5)
					{
						num7 = num5;
					}
					num4 += Collision.GetTileRotation(position) * num7;
					num5 -= num7;
					position.X += num7;
					num3++;
				}
				float num8 = num4 / (float)mountedPlayer.width - mountedPlayer.fullRotation;
				float num9 = 0f;
				float num10 = (float)Math.PI / 20f;
				if (num8 < 0f)
				{
					num9 = ((!(num8 > 0f - num10)) ? (0f - num10) : num8);
				}
				else if (num8 > 0f)
				{
					num9 = ((!(num8 < num10)) ? num10 : num8);
				}
				if (num9 != 0f)
				{
					mountedPlayer.fullRotation += num9;
					if (mountedPlayer.fullRotation > (float)Math.PI / 4f)
					{
						mountedPlayer.fullRotation = (float)Math.PI / 4f;
					}
					if (mountedPlayer.fullRotation < -(float)Math.PI / 4f)
					{
						mountedPlayer.fullRotation = -(float)Math.PI / 4f;
					}
				}
				break;
			}
			case 10:
			{
				bool flag = Math.Abs(velocity.X) > DashSpeed - RunSpeed / 2f;
				if (state == 1)
				{
					bool flag2 = false;
					if (flag)
					{
						state = 5;
						if (_frameExtra < 6)
						{
							flag2 = true;
						}
						_frameExtra++;
					}
					else
					{
						_frameExtra = 0;
					}
					if (flag2)
					{
						Vector2 vector = mountedPlayer.Center + new Vector2(mountedPlayer.width * mountedPlayer.direction, 0f);
						Vector2 value = new Vector2(40f, 30f);
						float num = (float)Math.PI * 2f * Main.rand.NextFloat();
						for (float num2 = 0f; num2 < 14f; num2 += 1f)
						{
							Dust dust = Main.dust[Dust.NewDust(vector, 0, 0, Utils.SelectRandom<int>(Main.rand, 176, 177, 179))];
							Vector2 value2 = Vector2.UnitY.RotatedBy(num2 * ((float)Math.PI * 2f) / 14f + num);
							value2 *= 0.2f * (float)_frameExtra;
							dust.position = vector + value2 * value;
							dust.velocity = value2 + new Vector2(RunSpeed - (float)(Math.Sign(velocity.X) * _frameExtra * 2), 0f);
							dust.noGravity = true;
							dust.scale = 1f + Main.rand.NextFloat() * 0.8f;
							dust.fadeIn = Main.rand.NextFloat() * 2f;
							dust.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
						}
					}
				}
				if (flag)
				{
					Dust dust2 = Main.dust[Dust.NewDust(mountedPlayer.position, mountedPlayer.width, mountedPlayer.height, Utils.SelectRandom<int>(Main.rand, 176, 177, 179))];
					dust2.velocity = Vector2.Zero;
					dust2.noGravity = true;
					dust2.scale = 0.5f + Main.rand.NextFloat() * 0.8f;
					dust2.fadeIn = 1f + Main.rand.NextFloat() * 2f;
					dust2.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
				}
				break;
			}
			}
			switch (state)
			{
			case 0:
				if (_data.idleFrameCount != 0)
				{
					if (_type == 5)
					{
						if (_fatigue != 0f)
						{
							if (_idleTime == 0)
							{
								_idleTimeNext = _idleTime + 1;
							}
						}
						else
						{
							_idleTime = 0;
							_idleTimeNext = 2;
						}
					}
					else if (_idleTime == 0)
					{
						_idleTimeNext = Main.rand.Next(900, 1500);
					}
					_idleTime++;
				}
				_frameCounter += 1f;
				if (_data.idleFrameCount != 0 && _idleTime >= _idleTimeNext)
				{
					float num13 = _data.idleFrameDelay;
					if (_type == 5)
					{
						num13 *= 2f - 1f * _fatigue / _fatigueMax;
					}
					int num14 = (int)((float)(_idleTime - _idleTimeNext) / num13);
					if (num14 >= _data.idleFrameCount)
					{
						if (_data.idleFrameLoop)
						{
							_idleTime = _idleTimeNext;
							_frame = _data.idleFrameStart;
						}
						else
						{
							_frameCounter = 0f;
							_frame = _data.standingFrameStart;
							_idleTime = 0;
						}
					}
					else
					{
						_frame = _data.idleFrameStart + num14;
					}
					if (_type == 5)
					{
						_frameExtra = _frame;
					}
				}
				else
				{
					if (_frameCounter > (float)_data.standingFrameDelay)
					{
						_frameCounter -= _data.standingFrameDelay;
						_frame++;
					}
					if (_frame < _data.standingFrameStart || _frame >= _data.standingFrameStart + _data.standingFrameCount)
					{
						_frame = _data.standingFrameStart;
					}
				}
				break;
			case 1:
			{
				float num11;
				switch (_type)
				{
				case 9:
					num11 = ((!_flipDraw) ? Math.Abs(velocity.X) : (0f - Math.Abs(velocity.X)));
					break;
				case 6:
					num11 = (_flipDraw ? velocity.X : (0f - velocity.X));
					break;
				case 13:
					num11 = (_flipDraw ? velocity.X : (0f - velocity.X));
					break;
				default:
					num11 = Math.Abs(velocity.X);
					break;
				}
				_frameCounter += num11;
				if (num11 >= 0f)
				{
					if (_frameCounter > (float)_data.runningFrameDelay)
					{
						_frameCounter -= _data.runningFrameDelay;
						_frame++;
					}
					if (_frame < _data.runningFrameStart || _frame >= _data.runningFrameStart + _data.runningFrameCount)
					{
						_frame = _data.runningFrameStart;
					}
				}
				else
				{
					if (_frameCounter < 0f)
					{
						_frameCounter += _data.runningFrameDelay;
						_frame--;
					}
					if (_frame < _data.runningFrameStart || _frame >= _data.runningFrameStart + _data.runningFrameCount)
					{
						_frame = _data.runningFrameStart + _data.runningFrameCount - 1;
					}
				}
				break;
			}
			case 3:
				_frameCounter += 1f;
				if (_frameCounter > (float)_data.flyingFrameDelay)
				{
					_frameCounter -= _data.flyingFrameDelay;
					_frame++;
				}
				if (_frame < _data.flyingFrameStart || _frame >= _data.flyingFrameStart + _data.flyingFrameCount)
				{
					_frame = _data.flyingFrameStart;
				}
				break;
			case 2:
				_frameCounter += 1f;
				if (_frameCounter > (float)_data.inAirFrameDelay)
				{
					_frameCounter -= _data.inAirFrameDelay;
					_frame++;
				}
				if (_frame < _data.inAirFrameStart || _frame >= _data.inAirFrameStart + _data.inAirFrameCount)
				{
					_frame = _data.inAirFrameStart;
				}
				if (_type == 4)
				{
					if (velocity.Y < 0f)
					{
						_frame = 3;
					}
					else
					{
						_frame = 6;
					}
				}
				else if (_type == 5)
				{
					float num12 = _fatigue / _fatigueMax;
					_frameExtraCounter += 6f - 4f * num12;
					if (_frameExtraCounter > (float)_data.flyingFrameDelay)
					{
						_frameExtra++;
						_frameExtraCounter -= _data.flyingFrameDelay;
					}
					if (_frameExtra < _data.flyingFrameStart || _frameExtra >= _data.flyingFrameStart + _data.flyingFrameCount)
					{
						_frameExtra = _data.flyingFrameStart;
					}
				}
				break;
			case 4:
				_frameCounter += (int)((Math.Abs(velocity.X) + Math.Abs(velocity.Y)) / 2f);
				if (_frameCounter > (float)_data.swimFrameDelay)
				{
					_frameCounter -= _data.swimFrameDelay;
					_frame++;
				}
				if (_frame < _data.swimFrameStart || _frame >= _data.swimFrameStart + _data.swimFrameCount)
				{
					_frame = _data.swimFrameStart;
				}
				break;
			case 5:
			{
				float num11;
				switch (_type)
				{
				case 9:
					num11 = ((!_flipDraw) ? Math.Abs(velocity.X) : (0f - Math.Abs(velocity.X)));
					break;
				case 6:
					num11 = (_flipDraw ? velocity.X : (0f - velocity.X));
					break;
				case 13:
					num11 = (_flipDraw ? velocity.X : (0f - velocity.X));
					break;
				default:
					num11 = Math.Abs(velocity.X);
					break;
				}
				_frameCounter += num11;
				if (num11 >= 0f)
				{
					if (_frameCounter > (float)_data.dashingFrameDelay)
					{
						_frameCounter -= _data.dashingFrameDelay;
						_frame++;
					}
					if (_frame < _data.dashingFrameStart || _frame >= _data.dashingFrameStart + _data.dashingFrameCount)
					{
						_frame = _data.dashingFrameStart;
					}
				}
				else
				{
					if (_frameCounter < 0f)
					{
						_frameCounter += _data.dashingFrameDelay;
						_frame--;
					}
					if (_frame < _data.dashingFrameStart || _frame >= _data.dashingFrameStart + _data.dashingFrameCount)
					{
						_frame = _data.dashingFrameStart + _data.dashingFrameCount - 1;
					}
				}
				break;
			}
			}
		}

		public void UpdateEffects(Player mountedPlayer)
		{
			mountedPlayer.autoJump = AutoJump;
			switch (_type)
			{
			case 9:
			{
				Vector2 center = mountedPlayer.Center;
				Vector2 vector5 = center;
				bool flag = false;
				float num9 = 1500f;
				for (int j = 0; j < 200; j++)
				{
					NPC nPC2 = Main.npc[j];
					if (nPC2.CanBeChasedBy(this))
					{
						float num10 = Vector2.Distance(nPC2.Center, vector5);
						if (((Vector2.Distance(vector5, vector5) > num10 && num10 < num9) || !flag) && Collision.CanHitLine(center, 0, 0, nPC2.position, nPC2.width, nPC2.height))
						{
							num9 = num10;
							vector5 = nPC2.Center;
							flag = true;
						}
					}
				}
				bool flag2 = flag;
				float num11 = Math.Abs((vector5 - center).ToRotation());
				if (mountedPlayer.direction == 1 && (double)num11 > 1.0471975949079879)
				{
					flag2 = false;
				}
				else if (mountedPlayer.direction == -1 && (double)num11 < 2.0943951461045853)
				{
					flag2 = false;
				}
				else if (!Collision.CanHitLine(center, 0, 0, vector5, 0, 0))
				{
					flag2 = false;
				}
				if (flag2)
				{
					if (_abilityCooldown == 0 && mountedPlayer.whoAmI == Main.myPlayer)
					{
						AimAbility(mountedPlayer, vector5);
						StopAbilityCharge();
						UseAbility(mountedPlayer, vector5, false);
					}
					else
					{
						AimAbility(mountedPlayer, vector5);
						_abilityCharging = true;
					}
				}
				else
				{
					_abilityCharging = false;
					ResetHeadPosition();
				}
				break;
			}
			case 10:
				mountedPlayer.doubleJumpUnicorn = true;
				if (Math.Abs(mountedPlayer.velocity.X) > mountedPlayer.mount.DashSpeed - mountedPlayer.mount.RunSpeed / 2f)
				{
					mountedPlayer.noKnockback = true;
				}
				if (mountedPlayer.itemAnimation > 0 && mountedPlayer.inventory[mountedPlayer.selectedItem].type == 1260)
				{
					AchievementsHelper.HandleSpecialEvent(mountedPlayer, 5);
				}
				break;
			case 12:
				if (mountedPlayer.MountFishronSpecial)
				{
					Vector3 vector4 = Colors.CurrentLiquidColor.ToVector3();
					vector4 *= 0.4f;
					Point point = (mountedPlayer.Center + Vector2.UnitX * mountedPlayer.direction * 20f + mountedPlayer.velocity * 10f).ToTileCoordinates();
					if (!WorldGen.SolidTile(point.X, point.Y))
					{
						Lighting.AddLight(point.X, point.Y, vector4.X, vector4.Y, vector4.Z);
					}
					else
					{
						Lighting.AddLight(mountedPlayer.Center + Vector2.UnitX * mountedPlayer.direction * 20f, vector4.X, vector4.Y, vector4.Z);
					}
					mountedPlayer.meleeDamage += 0.15f;
					mountedPlayer.rangedDamage += 0.15f;
					mountedPlayer.magicDamage += 0.15f;
					mountedPlayer.minionDamage += 0.15f;
					mountedPlayer.thrownDamage += 0.15f;
				}
				if (mountedPlayer.statLife <= mountedPlayer.statLifeMax2 / 2)
				{
					mountedPlayer.MountFishronSpecialCounter = 60f;
				}
				if (mountedPlayer.wet)
				{
					mountedPlayer.MountFishronSpecialCounter = 300f;
				}
				break;
			case 8:
				if (mountedPlayer.ownedProjectileCounts[453] < 1)
				{
					_abilityActive = false;
				}
				break;
			case 11:
			{
				Vector3 vector = new Vector3(0.4f, 0.12f, 0.15f);
				float num = 1f + Math.Abs(mountedPlayer.velocity.X) / RunSpeed * 2.5f;
				mountedPlayer.statDefense += (int)(2f * num);
				int num2 = Math.Sign(mountedPlayer.velocity.X);
				if (num2 == 0)
				{
					num2 = mountedPlayer.direction;
				}
				if (Main.netMode != 2)
				{
					vector *= num;
					Lighting.AddLight(mountedPlayer.Center, vector.X, vector.Y, vector.Z);
					Lighting.AddLight(mountedPlayer.Top, vector.X, vector.Y, vector.Z);
					Lighting.AddLight(mountedPlayer.Bottom, vector.X, vector.Y, vector.Z);
					Lighting.AddLight(mountedPlayer.Left, vector.X, vector.Y, vector.Z);
					Lighting.AddLight(mountedPlayer.Right, vector.X, vector.Y, vector.Z);
					float num3 = -24f;
					if (mountedPlayer.direction != num2)
					{
						num3 = -22f;
					}
					if (num2 == -1)
					{
						num3 += 1f;
					}
					Vector2 value = new Vector2(num3 * (float)num2, -19f).RotatedBy(mountedPlayer.fullRotation);
					Vector2 value2 = new Vector2(MathHelper.Lerp(0f, -8f, mountedPlayer.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(mountedPlayer.fullRotation / ((float)Math.PI / 4f)))).RotatedBy(mountedPlayer.fullRotation);
					if (num2 == Math.Sign(mountedPlayer.fullRotation))
					{
						value2 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(mountedPlayer.fullRotation / ((float)Math.PI / 4f)));
					}
					Vector2 vector2 = mountedPlayer.Bottom + value + value2;
					Vector2 vector3 = mountedPlayer.oldPosition + mountedPlayer.Size * new Vector2(0.5f, 1f) + value + value2;
					if (Vector2.Distance(vector2, vector3) > 3f)
					{
						int num4 = (int)Vector2.Distance(vector2, vector3) / 3;
						if (Vector2.Distance(vector2, vector3) % 3f != 0f)
						{
							num4++;
						}
						for (float num5 = 1f; num5 <= (float)num4; num5 += 1f)
						{
							Dust dust = Main.dust[Dust.NewDust(mountedPlayer.Center, 0, 0, 182)];
							dust.position = Vector2.Lerp(vector3, vector2, num5 / (float)num4);
							dust.noGravity = true;
							dust.velocity = Vector2.Zero;
							dust.customData = mountedPlayer;
							dust.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMinecart, mountedPlayer);
						}
					}
					else
					{
						Dust dust2 = Main.dust[Dust.NewDust(mountedPlayer.Center, 0, 0, 182)];
						dust2.position = vector2;
						dust2.noGravity = true;
						dust2.velocity = Vector2.Zero;
						dust2.customData = mountedPlayer;
						dust2.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMinecart, mountedPlayer);
					}
				}
				if (mountedPlayer.whoAmI != Main.myPlayer || mountedPlayer.velocity.X == 0f)
				{
					break;
				}
				Vector2 minecartMechPoint = GetMinecartMechPoint(mountedPlayer, 20, -19);
				int damage = 60;
				int num6 = 0;
				float num7 = 0f;
				for (int i = 0; i < 200; i++)
				{
					NPC nPC = Main.npc[i];
					if (nPC.active && nPC.immune[mountedPlayer.whoAmI] <= 0 && !nPC.dontTakeDamage && nPC.Distance(minecartMechPoint) < 300f && nPC.CanBeChasedBy(mountedPlayer) && Collision.CanHitLine(nPC.position, nPC.width, nPC.height, minecartMechPoint, 0, 0) && Math.Abs(MathHelper.WrapAngle(MathHelper.WrapAngle(nPC.AngleFrom(minecartMechPoint)) - MathHelper.WrapAngle((mountedPlayer.fullRotation + (float)num2 == -1f) ? ((float)Math.PI) : 0f))) < (float)Math.PI / 4f)
					{
						Vector2 v = nPC.position + nPC.Size * Utils.RandomVector2(Main.rand, 0f, 1f) - minecartMechPoint;
						num7 += v.ToRotation();
						num6++;
						int num8 = Projectile.NewProjectile(minecartMechPoint.X, minecartMechPoint.Y, v.X, v.Y, 591, 0, 0f, mountedPlayer.whoAmI, mountedPlayer.whoAmI);
						Main.projectile[num8].Center = nPC.Center;
						Main.projectile[num8].damage = damage;
						Main.projectile[num8].Damage();
						Main.projectile[num8].damage = 0;
						Main.projectile[num8].Center = minecartMechPoint;
					}
				}
				break;
			}
			}
		}

		public static Vector2 GetMinecartMechPoint(Player mountedPlayer, int offX, int offY)
		{
			int num = Math.Sign(mountedPlayer.velocity.X);
			if (num == 0)
			{
				num = mountedPlayer.direction;
			}
			float num2 = offX;
			int num3 = Math.Sign(offX);
			if (mountedPlayer.direction != num)
			{
				num2 -= (float)num3;
			}
			if (num == -1)
			{
				num2 -= (float)num3;
			}
			Vector2 value = new Vector2(num2 * (float)num, offY).RotatedBy(mountedPlayer.fullRotation);
			Vector2 value2 = new Vector2(MathHelper.Lerp(0f, -8f, mountedPlayer.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(mountedPlayer.fullRotation / ((float)Math.PI / 4f)))).RotatedBy(mountedPlayer.fullRotation);
			if (num == Math.Sign(mountedPlayer.fullRotation))
			{
				value2 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(mountedPlayer.fullRotation / ((float)Math.PI / 4f)));
			}
			return mountedPlayer.Bottom + value + value2;
		}

		public void ResetFlightTime(float xVelocity)
		{
			_flyTime = (_active ? _data.flightTimeMax : 0);
			if (_type == 0)
			{
				_flyTime += (int)(Math.Abs(xVelocity) * 20f);
			}
		}

		public void CheckMountBuff(Player mountedPlayer)
		{
			if (_type == -1)
			{
				return;
			}
			for (int i = 0; i < 22; i++)
			{
				if (mountedPlayer.buffType[i] == _data.buff || (Cart && mountedPlayer.buffType[i] == _data.extraBuff))
				{
					return;
				}
			}
			Dismount(mountedPlayer);
		}

		public void ResetHeadPosition()
		{
			if (_aiming)
			{
				_aiming = false;
				_frameExtra = 0;
				_flipDraw = false;
			}
		}

		private Vector2 ClampToDeadZone(Player mountedPlayer, Vector2 position)
		{
			int num;
			int num2;
			switch (_type)
			{
			case 9:
				num = (int)scutlixTextureSize.Y;
				num2 = (int)scutlixTextureSize.X;
				break;
			case 8:
				num = (int)drillTextureSize.Y;
				num2 = (int)drillTextureSize.X;
				break;
			default:
				return position;
			}
			Vector2 center = mountedPlayer.Center;
			position -= center;
			if (position.X > (float)(-num2) && position.X < (float)num2 && position.Y > (float)(-num) && position.Y < (float)num)
			{
				float num3 = (float)num2 / Math.Abs(position.X);
				float num4 = (float)num / Math.Abs(position.Y);
				if (num3 > num4)
				{
					position *= num4;
				}
				else
				{
					position *= num3;
				}
			}
			return position + center;
		}

		public bool AimAbility(Player mountedPlayer, Vector2 mousePosition)
		{
			_aiming = true;
			switch (_type)
			{
			case 9:
			{
				int frameExtra = _frameExtra;
				int direction = mountedPlayer.direction;
				Vector2 v = ClampToDeadZone(mountedPlayer, mousePosition);
				v -= mountedPlayer.Center;
				float num3 = MathHelper.ToDegrees(v.ToRotation());
				if (num3 > 90f)
				{
					mountedPlayer.direction = -1;
					num3 = 180f - num3;
				}
				else if (num3 < -90f)
				{
					mountedPlayer.direction = -1;
					num3 = -180f - num3;
				}
				else
				{
					mountedPlayer.direction = 1;
				}
				if ((mountedPlayer.direction > 0 && mountedPlayer.velocity.X < 0f) || (mountedPlayer.direction < 0 && mountedPlayer.velocity.X > 0f))
				{
					_flipDraw = true;
				}
				else
				{
					_flipDraw = false;
				}
				if (num3 >= 0f)
				{
					if ((double)num3 < 22.5)
					{
						_frameExtra = 8;
					}
					else if ((double)num3 < 67.5)
					{
						_frameExtra = 9;
					}
					else if ((double)num3 < 112.5)
					{
						_frameExtra = 10;
					}
				}
				else if ((double)num3 > -22.5)
				{
					_frameExtra = 8;
				}
				else if ((double)num3 > -67.5)
				{
					_frameExtra = 7;
				}
				else if ((double)num3 > -112.5)
				{
					_frameExtra = 6;
				}
				float abilityCharge = AbilityCharge;
				if (abilityCharge > 0f)
				{
					Vector2 vector = default(Vector2);
					vector.X = mountedPlayer.position.X + (float)(mountedPlayer.width / 2);
					vector.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
					int num4 = (_frameExtra - 6) * 2;
					Vector2 vector2 = default(Vector2);
					for (int i = 0; i < 2; i++)
					{
						vector2.Y = vector.Y + scutlixEyePositions[num4 + i].Y;
						if (mountedPlayer.direction == -1)
						{
							vector2.X = vector.X - scutlixEyePositions[num4 + i].X - (float)_data.xOffset;
						}
						else
						{
							vector2.X = vector.X + scutlixEyePositions[num4 + i].X + (float)_data.xOffset;
						}
						Lighting.AddLight((int)(vector2.X / 16f), (int)(vector2.Y / 16f), 1f * abilityCharge, 0f, 0f);
					}
				}
				if (_frameExtra == frameExtra)
				{
					return mountedPlayer.direction != direction;
				}
				return true;
			}
			case 8:
			{
				Vector2 v = ClampToDeadZone(mountedPlayer, mousePosition);
				v -= mountedPlayer.Center;
				DrillMountData drillMountData = (DrillMountData)_mountSpecificData;
				float num = v.ToRotation();
				if (num < 0f)
				{
					num += (float)Math.PI * 2f;
				}
				drillMountData.diodeRotationTarget = num;
				float num2 = drillMountData.diodeRotation % ((float)Math.PI * 2f);
				if (num2 < 0f)
				{
					num2 += (float)Math.PI * 2f;
				}
				if (num2 < num)
				{
					if (num - num2 > (float)Math.PI)
					{
						num2 += (float)Math.PI * 2f;
					}
				}
				else if (num2 - num > (float)Math.PI)
				{
					num2 -= (float)Math.PI * 2f;
				}
				drillMountData.diodeRotation = num2;
				drillMountData.crosshairPosition = mousePosition;
				return true;
			}
			default:
				return false;
			}
		}

		public void Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
		{
			if (playerDrawData == null)
			{
				return;
			}
			Texture2D texture2D;
			Texture2D texture2D2;
			switch (drawType)
			{
			case 0:
				texture2D = _data.backTexture;
				texture2D2 = _data.backTextureGlow;
				break;
			case 1:
				texture2D = _data.backTextureExtra;
				texture2D2 = _data.backTextureExtraGlow;
				break;
			case 2:
				if (_type == 0 && _idleTime >= _idleTimeNext)
				{
					return;
				}
				texture2D = _data.frontTexture;
				texture2D2 = _data.frontTextureGlow;
				break;
			case 3:
				texture2D = _data.frontTextureExtra;
				texture2D2 = _data.frontTextureExtraGlow;
				break;
			default:
				texture2D = null;
				texture2D2 = null;
				break;
			}
			if (texture2D == null)
			{
				return;
			}
			int type = _type;
			if ((type == 0 || type == 9) && drawType == 3 && shadow != 0f)
			{
				return;
			}
			int num = XOffset;
			int num2 = YOffset + PlayerOffset;
			if (drawPlayer.direction <= 0 && (!Cart || !Directional))
			{
				num *= -1;
			}
			Position.X = (int)(Position.X - Main.screenPosition.X + (float)(drawPlayer.width / 2) + (float)num);
			Position.Y = (int)(Position.Y - Main.screenPosition.Y + (float)(drawPlayer.height / 2) + (float)num2);
			int num3 = 0;
			switch (_type)
			{
			case 9:
				switch (drawType)
				{
				case 0:
					num3 = _frame;
					break;
				case 2:
					num3 = _frameExtra;
					break;
				case 3:
					num3 = _frameExtra;
					break;
				default:
					num3 = 0;
					break;
				}
				break;
			case 5:
				switch (drawType)
				{
				case 0:
					num3 = _frame;
					break;
				case 1:
					num3 = _frameExtra;
					break;
				default:
					num3 = 0;
					break;
				}
				break;
			default:
				num3 = _frame;
				break;
			}
			int num4 = _data.textureHeight / _data.totalFrames;
			Rectangle value = new Rectangle(0, num4 * num3, _data.textureWidth, num4);
			switch (_type)
			{
			case 0:
				if (drawType == 3)
				{
					drawColor = Color.White;
				}
				break;
			case 9:
				if (drawType == 3)
				{
					if (_abilityCharge == 0)
					{
						return;
					}
					drawColor = Color.Multiply(Color.White, (float)_abilityCharge / (float)_data.abilityChargeMax);
					drawColor.A = 0;
				}
				break;
			case 7:
				if (drawType == 3)
				{
					drawColor = new Color(250, 250, 250, 255) * drawPlayer.stealth * (1f - shadow);
				}
				break;
			}
			Color color = new Color(drawColor.ToVector4() * 0.25f + new Vector4(0.75f));
			switch (_type)
			{
			case 11:
				if (drawType == 2)
				{
					color = Color.White;
					color.A = 127;
				}
				break;
			case 12:
				if (drawType == 0)
				{
					float num5 = MathHelper.Clamp(drawPlayer.MountFishronSpecialCounter / 60f, 0f, 1f);
					color = Colors.CurrentLiquidColor;
					if (color == Color.Transparent)
					{
						color = Color.White;
					}
					color.A = 127;
					color *= num5;
				}
				break;
			}
			float num6 = 0f;
			switch (_type)
			{
			case 8:
			{
				DrillMountData drillMountData = (DrillMountData)_mountSpecificData;
				switch (drawType)
				{
				case 0:
					num6 = drillMountData.outerRingRotation - num6;
					break;
				case 3:
					num6 = drillMountData.diodeRotation - num6 - drawPlayer.fullRotation;
					break;
				}
				break;
			}
			case 7:
				num6 = drawPlayer.fullRotation;
				break;
			}
			Vector2 origin = Origin;
			int type2 = _type;
			int num8 = 8;
			float scale = 1f;
			SpriteEffects effect;
			switch (_type)
			{
			case 7:
				effect = SpriteEffects.None;
				break;
			case 8:
				effect = ((drawPlayer.direction == 1 && drawType == 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
				break;
			case 6:
			case 13:
				effect = (_flipDraw ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
				break;
			case 11:
				effect = ((Math.Sign(drawPlayer.velocity.X) == -drawPlayer.direction) ? (playerEffect ^ SpriteEffects.FlipHorizontally) : playerEffect);
				break;
			default:
				effect = playerEffect;
				break;
			}
			bool flag = false;
			int type3 = _type;
			int num9 = 8;
			DrawData item;
			if (!flag)
			{
				item = new DrawData(texture2D, Position, value, drawColor, num6, origin, scale, effect, 0);
				item.shader = currentShader;
				playerDrawData.Add(item);
				if (texture2D2 != null)
				{
					item = new DrawData(texture2D2, Position, value, color, num6, origin, scale, effect, 0);
					item.shader = currentShader;
				}
				playerDrawData.Add(item);
			}
			int type4 = _type;
			if (type4 != 8 || drawType != 3)
			{
				return;
			}
			DrillMountData drillMountData2 = (DrillMountData)_mountSpecificData;
			Rectangle value2 = new Rectangle(0, 0, 1, 1);
			Vector2 vector = drillDiodePoint1.RotatedBy(drillMountData2.diodeRotation);
			Vector2 vector2 = drillDiodePoint2.RotatedBy(drillMountData2.diodeRotation);
			for (int i = 0; i < drillMountData2.beams.Length; i++)
			{
				DrillBeam drillBeam = drillMountData2.beams[i];
				if (drillBeam.curTileTarget == Point16.NegativeOne)
				{
					continue;
				}
				for (int j = 0; j < 2; j++)
				{
					Vector2 value3 = new Vector2(drillBeam.curTileTarget.X * 16 + 8, drillBeam.curTileTarget.Y * 16 + 8) - Main.screenPosition - Position;
					Vector2 vector3;
					Color color2;
					if (j == 0)
					{
						vector3 = vector;
						color2 = Color.CornflowerBlue;
					}
					else
					{
						vector3 = vector2;
						color2 = Color.LightGreen;
					}
					color2.A = 128;
					color2 *= 0.5f;
					Vector2 v = value3 - vector3;
					float num7 = v.ToRotation();
					float y = v.Length();
					item = new DrawData(scale: new Vector2(2f, y), texture: Main.magicPixel, position: vector3 + Position, sourceRect: value2, color: color2, rotation: num7 - (float)Math.PI / 2f, origin: Vector2.Zero, effect: SpriteEffects.None, inactiveLayerDepth: 0);
					item.ignorePlayerRotation = true;
					item.shader = currentShader;
					playerDrawData.Add(item);
				}
			}
		}

		public void Dismount(Player mountedPlayer)
		{
			if (!_active)
			{
				return;
			}
			bool cart = Cart;
			_active = false;
			mountedPlayer.ClearBuff(_data.buff);
			_mountSpecificData = null;
			if (cart)
			{
				mountedPlayer.ClearBuff(_data.extraBuff);
				mountedPlayer.cartFlip = false;
				mountedPlayer.lastBoost = Vector2.Zero;
			}
			mountedPlayer.fullRotation = 0f;
			mountedPlayer.fullRotationOrigin = Vector2.Zero;
			if (Main.netMode != 2)
			{
				for (int i = 0; i < 100; i++)
				{
					if (_type == 6 || _type == 11 || _type == 13)
					{
						if (i % 10 == 0)
						{
							int type = Main.rand.Next(61, 64);
							int num = Gore.NewGore(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), Vector2.Zero, type);
							Main.gore[num].alpha = 100;
							Main.gore[num].velocity = Vector2.Transform(new Vector2(1f, 0f), Matrix.CreateRotationZ((float)(Main.rand.NextDouble() * 6.2831854820251465)));
						}
						continue;
					}
					int num2 = Dust.NewDust(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), mountedPlayer.width + 40, mountedPlayer.height, _data.spawnDust);
					Main.dust[num2].scale += (float)Main.rand.Next(-10, 21) * 0.01f;
					if (_data.spawnDustNoGravity)
					{
						Main.dust[num2].noGravity = true;
					}
					else if (Main.rand.Next(2) == 0)
					{
						Main.dust[num2].scale *= 1.3f;
						Main.dust[num2].noGravity = true;
					}
					else
					{
						Main.dust[num2].velocity *= 0.5f;
					}
					Main.dust[num2].velocity += mountedPlayer.velocity * 0.8f;
				}
			}
			Reset();
			mountedPlayer.position.Y += mountedPlayer.height;
			mountedPlayer.height = 42;
			mountedPlayer.position.Y -= mountedPlayer.height;
			if (mountedPlayer.whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(13, -1, -1, "", mountedPlayer.whoAmI);
			}
		}

		public void SetMount(int m, Player mountedPlayer, bool faceLeft = false)
		{
			if (_type == m)
			{
				return;
			}
			switch (m)
			{
			default:
				return;
			case 5:
				if (mountedPlayer.wet)
				{
					return;
				}
				break;
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				break;
			}
			if (_active)
			{
				mountedPlayer.ClearBuff(_data.buff);
				if (Cart)
				{
					mountedPlayer.ClearBuff(_data.extraBuff);
					mountedPlayer.cartFlip = false;
					mountedPlayer.lastBoost = Vector2.Zero;
				}
				mountedPlayer.fullRotation = 0f;
				mountedPlayer.fullRotationOrigin = Vector2.Zero;
				_mountSpecificData = null;
			}
			else
			{
				_active = true;
			}
			_flyTime = 0;
			_type = m;
			_data = mounts[m];
			_fatigueMax = _data.fatigueMax;
			if (Cart && !faceLeft && !Directional)
			{
				mountedPlayer.AddBuff(_data.extraBuff, 3600);
				_flipDraw = true;
			}
			else
			{
				mountedPlayer.AddBuff(_data.buff, 3600);
				_flipDraw = false;
			}
			if (_type == 9 && _abilityCooldown < 20)
			{
				_abilityCooldown = 20;
			}
			mountedPlayer.position.Y += mountedPlayer.height;
			for (int i = 0; i < mountedPlayer.shadowPos.Length; i++)
			{
				mountedPlayer.shadowPos[i].Y += mountedPlayer.height;
			}
			mountedPlayer.height = 42 + _data.heightBoost;
			mountedPlayer.position.Y -= mountedPlayer.height;
			for (int j = 0; j < mountedPlayer.shadowPos.Length; j++)
			{
				mountedPlayer.shadowPos[j].Y -= mountedPlayer.height;
			}
			if (_type == 7 || _type == 8)
			{
				mountedPlayer.fullRotationOrigin = new Vector2(mountedPlayer.width / 2, mountedPlayer.height / 2);
			}
			if (_type == 8)
			{
				_mountSpecificData = new DrillMountData();
			}
			if (Main.netMode != 2)
			{
				for (int k = 0; k < 100; k++)
				{
					if (_type == 6 || _type == 11 || _type == 13)
					{
						if (k % 10 == 0)
						{
							int type = Main.rand.Next(61, 64);
							int num = Gore.NewGore(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), Vector2.Zero, type);
							Main.gore[num].alpha = 100;
							Main.gore[num].velocity = Vector2.Transform(new Vector2(1f, 0f), Matrix.CreateRotationZ((float)(Main.rand.NextDouble() * 6.2831854820251465)));
						}
						continue;
					}
					int num2 = Dust.NewDust(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), mountedPlayer.width + 40, mountedPlayer.height, _data.spawnDust);
					Main.dust[num2].scale += (float)Main.rand.Next(-10, 21) * 0.01f;
					if (_data.spawnDustNoGravity)
					{
						Main.dust[num2].noGravity = true;
					}
					else if (Main.rand.Next(2) == 0)
					{
						Main.dust[num2].scale *= 1.3f;
						Main.dust[num2].noGravity = true;
					}
					else
					{
						Main.dust[num2].velocity *= 0.5f;
					}
					Main.dust[num2].velocity += mountedPlayer.velocity * 0.8f;
				}
			}
			if (mountedPlayer.whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(13, -1, -1, "", mountedPlayer.whoAmI);
			}
		}

		public bool CanMount(int m, Player mountingPlayer)
		{
			int num = 42 + mounts[m].heightBoost;
			Vector2 position = mountingPlayer.position + new Vector2(0f, mountingPlayer.height - num) + mountingPlayer.velocity;
			return Collision.IsClearSpotHack(position, 16f, mountingPlayer.width, num);
		}

		public bool FindTileHeight(Vector2 position, int maxTilesDown, out float tileHeight)
		{
			int num = (int)(position.X / 16f);
			int num2 = (int)(position.Y / 16f);
			for (int i = 0; i <= maxTilesDown; i++)
			{
				Tile tile = Main.tile[num, num2];
				bool flag = Main.tileSolid[tile.type];
				bool flag2 = Main.tileSolidTop[tile.type];
				if (!tile.active() || !flag || flag2)
				{
				}
				num2++;
			}
			tileHeight = 0f;
			return true;
		}
	}
}
