using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Tile_Entities;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace Terraria
{
	public class Player : Entity
	{
		public class SmartCursorSettings
		{
			public static bool SmartBlocksEnabled = false;

			public static bool SmartWallReplacement = true;

			public static bool SmartAxeAfterPickaxe = false;
		}

		public struct OverheadMessage
		{
			public string chatText;

			public TextSnippet[] snippets;

			public Vector2 messageSize;

			public int timeLeft;

			public void NewMessage(string message, int displayTime)
			{
				chatText = message;
				snippets = ChatManager.ParseMessage(chatText, Color.White);
				messageSize = ChatManager.GetStringSize(Main.fontMouseText, snippets, Vector2.One);
				timeLeft = displayTime;
			}
		}

		public const int maxSolarShields = 3;

		public const int nebulaMaxLevel = 3;

		public const int SupportedSlotsArmor = 3;

		public const int SupportedSlotsAccs = 7;

		public const int SupportedSlotSets = 10;

		public const int InitialAccSlotCount = 5;

		public const int miscSlotPet = 0;

		public const int miscSlotLight = 1;

		public const int miscSlotCart = 2;

		public const int miscSlotMount = 3;

		public const int miscSlotHook = 4;

		public const int maxBuffs = 22;

		public const int defaultWidth = 20;

		public const int defaultHeight = 42;

		private const int shadowMax = 3;

		private static byte[] ENCRYPTION_KEY = new UnicodeEncoding().GetBytes("h3y_gUyZ");

		public OverheadMessage chatOverhead = default(OverheadMessage);

		public bool alchemyTable;

		private bool GoingDownWithGrapple;

		private byte spelunkerTimer;

		public bool[] hideInfo = new bool[13];

		public int lostCoins;

		public string lostCoinString = "";

		public int soulDrain;

		public float drainBoost;

		public string name = "";

		public int taxMoney;

		public int taxTimer;

		public static int taxRate = 3600;

		public static int crystalLeafDamage = 100;

		public static int crystalLeafKB = 10;

		public bool[] NPCBannerBuff = new bool[251];

		public bool hasBanner;

		public Vector2 lastDeathPostion;

		public DateTime lastDeathTime;

		public bool showLastDeath;

		public int extraAccessorySlots = 2;

		public bool extraAccessory;

		public int tankPet = -1;

		public bool tankPetReset;

		public int stringColor;

		public int counterWeight;

		public bool yoyoString;

		public bool yoyoGlove;

		public int beetleOrbs;

		public float beetleCounter;

		public int beetleCountdown;

		public bool beetleDefense;

		public bool beetleOffense;

		public bool beetleBuff;

		public int solarShields;

		public int solarCounter;

		public Vector2[] solarShieldPos = new Vector2[3];

		public Vector2[] solarShieldVel = new Vector2[3];

		public bool solarDashing;

		public bool solarDashConsumedFlare;

		public int nebulaLevelLife;

		public int nebulaLevelMana;

		public int nebulaManaCounter;

		public int nebulaLevelDamage;

		public bool manaMagnet;

		public bool lifeMagnet;

		public bool lifeForce;

		public bool calmed;

		public bool inferno;

		public float flameRingRot;

		public float flameRingScale = 1f;

		public byte flameRingFrame;

		public byte flameRingAlpha;

		public int netManaTime;

		public int netLifeTime;

		public bool netMana;

		public bool netLife;

		public Vector2[] beetlePos = new Vector2[3];

		public Vector2[] beetleVel = new Vector2[3];

		public int beetleFrame;

		public int beetleFrameCounter;

		public static int manaSickTime = 300;

		public static int manaSickTimeMax = 600;

		public static float manaSickLessDmg = 0.25f;

		public float manaSickReduction;

		public bool manaSick;

		public bool stairFall;

		public int loadStatus;

		public Vector2[] itemFlamePos = new Vector2[7];

		public int itemFlameCount;

		public bool outOfRange;

		public float lifeSteal = 99999f;

		public float ghostDmg;

		public bool teleporting;

		public float teleportTime;

		public int teleportStyle;

		public bool sloping;

		public bool chilled;

		public bool dazed;

		public bool frozen;

		public bool stoned;

		public bool lastStoned;

		public bool ichor;

		public bool webbed;

		public int ropeCount;

		public int manaRegenBonus;

		public int manaRegenDelayBonus;

		public int dash;

		public int dashTime;

		public int dashDelay;

		public int eocDash;

		public int eocHit;

		public float accRunSpeed;

		public bool cordage;

		public int gem = -1;

		public int gemCount;

		public byte meleeEnchant;

		public byte pulleyDir;

		public bool pulley;

		public int pulleyFrame;

		public float pulleyFrameCounter;

		public bool blackBelt;

		public bool sliding;

		public int slideDir;

		public int launcherWait;

		public bool iceSkate;

		public bool carpet;

		public int spikedBoots;

		public int carpetFrame = -1;

		public float carpetFrameCounter;

		public bool canCarpet;

		public int carpetTime;

		public int miscCounter;

		public int infernoCounter;

		public bool sandStorm;

		public bool crimsonRegen;

		public bool ghostHeal;

		public bool ghostHurt;

		public bool sticky;

		public bool slippy;

		public bool slippy2;

		public bool powerrun;

		public bool flapSound;

		public bool iceBarrier;

		public bool dangerSense;

		public float endurance;

		public bool loveStruck;

		public bool stinky;

		public bool resistCold;

		public bool electrified;

		public bool dryadWard;

		public bool panic;

		public bool brainOfConfusion;

		public byte iceBarrierFrame;

		public byte iceBarrierFrameCounter;

		public bool shadowDodge;

		public float shadowDodgeCount;

		public bool palladiumRegen;

		public bool onHitDodge;

		public bool onHitRegen;

		public bool onHitPetal;

		public int petalTimer;

		public int shadowDodgeTimer;

		public int fishingSkill;

		public bool cratePotion;

		public bool sonarPotion;

		public bool accFishingLine;

		public bool accTackleBox;

		public int maxMinions = 1;

		public int numMinions;

		public float slotsMinions;

		public bool pygmy;

		public bool raven;

		public bool slime;

		public bool hornetMinion;

		public bool impMinion;

		public bool twinsMinion;

		public bool spiderMinion;

		public bool pirateMinion;

		public bool sharknadoMinion;

		public bool UFOMinion;

		public bool DeadlySphereMinion;

		public bool stardustMinion;

		public bool stardustGuardian;

		public bool stardustDragon;

		public float wingTime;

		public int wings;

		public int wingsLogic;

		public int wingTimeMax;

		public int wingFrame;

		public int wingFrameCounter;

		public int skinVariant;

		public bool ghost;

		public int ghostFrame;

		public int ghostFrameCounter;

		public int miscTimer;

		public bool pvpDeath;

		public BitsByte zone1 = (byte)0;

		public BitsByte zone2 = (byte)0;

		public bool boneArmor;

		public bool frostArmor;

		public bool honey;

		public bool crystalLeaf;

		public int[] doubleTapCardinalTimer = new int[4];

		public int[] holdDownCardinalTimer = new int[4];

		public bool paladinBuff;

		public bool paladinGive;

		public float[] speedSlice = new float[60];

		public float townNPCs;

		public double headFrameCounter;

		public double bodyFrameCounter;

		public double legFrameCounter;

		public int netSkip;

		public int oldSelectItem;

		public bool immune;

		public int immuneTime;

		public int immuneAlphaDirection;

		public int immuneAlpha;

		public int team;

		public bool hbLocked;

		public static int nameLen = 20;

		private float maxRegenDelay;

		public int sign = -1;

		public bool editedChestName;

		public int reuseDelay;

		public int aggro;

		public float activeNPCs;

		public bool mouseInterface;

		public bool lastMouseInterface;

		public int noThrow;

		public int changeItem = -1;

		public int selectedItem;

		public Item[] armor = new Item[20];

		public Item[] dye = new Item[10];

		public Item[] miscEquips = new Item[5];

		public Item[] miscDyes = new Item[5];

		public Item trashItem = new Item();

		public int itemAnimation;

		public int itemAnimationMax;

		public int itemTime;

		public int toolTime;

		public float itemRotation;

		public int itemWidth;

		public int itemHeight;

		public Vector2 itemLocation;

		public bool poundRelease;

		public float ghostFade;

		public float ghostDir = 1f;

		public int[] buffType = new int[22];

		public int[] buffTime = new int[22];

		public bool[] buffImmune = new bool[191];

		public int heldProj = -1;

		public int breathCD;

		public int breathMax = 200;

		public int breath = 200;

		public int lavaCD;

		public int lavaMax;

		public int lavaTime;

		public bool ignoreWater;

		public bool socialShadow;

		public bool socialGhost;

		public bool shroomiteStealth;

		public int stealthTimer;

		public float stealth = 1f;

		public string setBonus = "";

		public Item[] inventory = new Item[59];

		public bool[] inventoryChestStack = new bool[59];

		public Chest bank = new Chest(true);

		public Chest bank2 = new Chest(true);

		public float headRotation;

		public float bodyRotation;

		public float legRotation;

		public Vector2 headPosition;

		public Vector2 bodyPosition;

		public Vector2 legPosition;

		public Vector2 headVelocity;

		public Vector2 bodyVelocity;

		public Vector2 legVelocity;

		public float fullRotation;

		public Vector2 fullRotationOrigin = Vector2.Zero;

		public int nonTorch = -1;

		public float gfxOffY;

		public float stepSpeed = 1f;

		public static bool deadForGood = false;

		public bool dead;

		public int respawnTimer;

		public int attackCD;

		public int potionDelay;

		public byte difficulty;

		public byte wetSlime;

		public HitTile hitTile;

		public int jump;

		public int head = -1;

		public int body = -1;

		public int legs = -1;

		public sbyte handon = -1;

		public sbyte handoff = -1;

		public sbyte back = -1;

		public sbyte front = -1;

		public sbyte shoe = -1;

		public sbyte waist = -1;

		public sbyte shield = -1;

		public sbyte neck = -1;

		public sbyte face = -1;

		public sbyte balloon = -1;

		public bool[] hideVisual = new bool[10];

		public BitsByte hideMisc = (byte)0;

		public Rectangle headFrame;

		public Rectangle bodyFrame;

		public Rectangle legFrame;

		public Rectangle hairFrame;

		public bool controlLeft;

		public bool controlRight;

		public bool controlUp;

		public bool controlDown;

		public bool controlJump;

		public bool controlUseItem;

		public bool controlUseTile;

		public bool controlThrow;

		public bool controlInv;

		public bool controlHook;

		public bool controlTorch;

		public bool controlMap;

		public bool controlSmart;

		public bool controlMount;

		public bool releaseJump;

		public bool releaseUp;

		public bool releaseUseItem;

		public bool releaseUseTile;

		public bool releaseInventory;

		public bool releaseHook;

		public bool releaseThrow;

		public bool releaseQuickMana;

		public bool releaseQuickHeal;

		public bool releaseLeft;

		public bool releaseRight;

		public bool releaseSmart;

		public bool releaseMount;

		public bool releaseDown;

		public int altFunctionUse;

		public bool mapZoomIn;

		public bool mapZoomOut;

		public bool mapAlphaUp;

		public bool mapAlphaDown;

		public bool mapFullScreen;

		public bool mapStyle;

		public bool releaseMapFullscreen;

		public bool releaseMapStyle;

		public int leftTimer;

		public int rightTimer;

		public bool delayUseItem;

		public bool showItemIcon;

		public bool showItemIconR;

		public int showItemIcon2;

		public string showItemIconText = "";

		public int runSoundDelay;

		public float shadow;

		public Vector2[] shadowPos = new Vector2[3];

		public float[] shadowRotation = new float[3];

		public Vector2[] shadowOrigin = new Vector2[3];

		public int[] shadowDirection = new int[3];

		public int shadowCount;

		public float manaCost = 1f;

		public bool fireWalk;

		public bool channel;

		public int step = -1;

		public int anglerQuestsFinished;

		public int armorPenetration;

		public int statDefense;

		public int statLifeMax = 100;

		public int statLifeMax2 = 100;

		public int statLife = 100;

		public int statMana;

		public int statManaMax;

		public int statManaMax2;

		public int lifeRegen;

		public int lifeRegenCount;

		public int lifeRegenTime;

		public int manaRegen;

		public int manaRegenCount;

		public int manaRegenDelay;

		public bool manaRegenBuff;

		public bool noKnockback;

		public bool spaceGun;

		public float gravDir = 1f;

		public bool ammoCost80;

		public bool ammoCost75;

		public int stickyBreak;

		public bool magicQuiver;

		public bool magmaStone;

		public bool lavaRose;

		public int phantasmTime;

		public bool ammoBox;

		public bool ammoPotion;

		public bool chaosState;

		public bool strongBees;

		public bool sporeSac;

		public bool shinyStone;

		public int yoraiz0rEye;

		public bool yoraiz0rDarkness;

		public bool suspiciouslookingTentacle;

		public bool crimsonHeart;

		public bool lightOrb;

		public bool blueFairy;

		public bool redFairy;

		public bool greenFairy;

		public bool bunny;

		public bool turtle;

		public bool eater;

		public bool penguin;

		public bool magicLantern;

		public bool rabid;

		public bool sunflower;

		public bool wellFed;

		public bool puppy;

		public bool grinch;

		public bool miniMinotaur;

		public bool arcticDivingGear;

		public bool wearsRobe;

		public bool minecartLeft;

		public bool onWrongGround;

		public bool onTrack;

		public int cartRampTime;

		public bool cartFlip;

		public float trackBoost;

		public Vector2 lastBoost = Vector2.Zero;

		public Mount mount;

		public bool blackCat;

		public bool spider;

		public bool squashling;

		public bool babyFaceMonster;

		public bool magicCuffs;

		public bool coldDash;

		public bool sailDash;

		public bool eyeSpring;

		public bool snowman;

		public bool scope;

		public bool dino;

		public bool skeletron;

		public bool hornet;

		public bool zephyrfish;

		public bool tiki;

		public bool parrot;

		public bool truffle;

		public bool sapling;

		public bool cSapling;

		public bool wisp;

		public bool lizard;

		public bool archery;

		public bool poisoned;

		public bool venom;

		public bool blind;

		public bool blackout;

		public bool headcovered;

		public bool frostBurn;

		public bool onFrostBurn;

		public bool burned;

		public bool suffocating;

		public byte suffocateDelay;

		public bool dripping;

		public bool drippingSlime;

		public bool onFire;

		public bool onFire2;

		public bool noItems;

		public bool wereWolf;

		public bool wolfAcc;

		public bool hideMerman;

		public bool hideWolf;

		public bool forceMerman;

		public bool forceWerewolf;

		public bool rulerGrid;

		public bool rulerLine;

		public bool bleed;

		public bool confused;

		public bool accMerman;

		public bool merman;

		public bool brokenArmor;

		public bool silence;

		public bool slow;

		public bool gross;

		public bool tongued;

		public bool kbGlove;

		public bool kbBuff;

		public bool starCloak;

		public bool longInvince;

		public bool pStone;

		public bool manaFlower;

		public bool moonLeech;

		public bool vortexDebuff;

		public bool trapDebuffSource;

		public int meleeCrit = 4;

		public int rangedCrit = 4;

		public int magicCrit = 4;

		public int thrownCrit = 4;

		public float meleeDamage = 1f;

		public float rangedDamage = 1f;

		public float thrownDamage = 1f;

		public float bulletDamage = 1f;

		public float arrowDamage = 1f;

		public float rocketDamage = 1f;

		public float magicDamage = 1f;

		public float minionDamage = 1f;

		public float minionKB;

		public float meleeSpeed = 1f;

		public float thrownVelocity = 1f;

		public bool thrownCost50;

		public bool thrownCost33;

		public float moveSpeed = 1f;

		public float pickSpeed = 1f;

		public float wallSpeed = 1f;

		public float tileSpeed = 1f;

		public bool autoPaint;

		public int SpawnX = -1;

		public int SpawnY = -1;

		public int[] spX = new int[200];

		public int[] spY = new int[200];

		public string[] spN = new string[200];

		public int[] spI = new int[200];

		public static int tileRangeX = 5;

		public static int tileRangeY = 4;

		public int lastTileRangeX;

		public int lastTileRangeY;

		public static int tileTargetX;

		public static int tileTargetY;

		public static float defaultGravity = 0.4f;

		private static int jumpHeight = 15;

		private static float jumpSpeed = 5.01f;

		public float gravity = defaultGravity;

		public float maxFallSpeed = 10f;

		public float maxRunSpeed = 3f;

		public float runAcceleration = 0.08f;

		public float runSlowdown = 0.2f;

		public bool adjWater;

		public bool adjHoney;

		public bool adjLava;

		public bool oldAdjWater;

		public bool oldAdjHoney;

		public bool oldAdjLava;

		public bool[] adjTile = new bool[419];

		public bool[] oldAdjTile = new bool[419];

		private static int defaultItemGrabRange = 38;

		private static float itemGrabSpeed = 0.45f;

		private static float itemGrabSpeedMax = 4f;

		public byte hairDye;

		public Color hairDyeColor = Color.Transparent;

		public float hairDyeVar;

		public Color hairColor = new Color(215, 90, 55);

		public Color skinColor = new Color(255, 125, 90);

		public Color eyeColor = new Color(105, 90, 75);

		public Color shirtColor = new Color(175, 165, 140);

		public Color underShirtColor = new Color(160, 180, 215);

		public Color pantsColor = new Color(255, 230, 175);

		public Color shoeColor = new Color(160, 105, 60);

		public int hair;

		public bool hostile;

		public int accCompass;

		public int accWatch;

		public int accDepthMeter;

		public bool accFishFinder;

		public bool accWeatherRadio;

		public bool accJarOfSouls;

		public bool accCalendar;

		public int lastCreatureHit = -1;

		public bool accThirdEye;

		public byte accThirdEyeCounter;

		public byte accThirdEyeNumber;

		public bool accStopwatch;

		public bool accOreFinder;

		public int bestOre = -1;

		public bool accCritterGuide;

		public byte accCritterGuideCounter;

		public byte accCritterGuideNumber;

		public bool accDreamCatcher;

		public DateTime dpsStart;

		public DateTime dpsEnd;

		public DateTime dpsLastHit;

		public int dpsDamage;

		public bool dpsStarted;

		public string displayedFishingInfo = "";

		public bool discount;

		public bool coins;

		public bool goldRing;

		public bool accDivingHelm;

		public bool accFlipper;

		public bool doubleJumpCloud;

		public bool jumpAgainCloud;

		public bool dJumpEffectCloud;

		public bool doubleJumpSandstorm;

		public bool jumpAgainSandstorm;

		public bool dJumpEffectSandstorm;

		public bool doubleJumpBlizzard;

		public bool jumpAgainBlizzard;

		public bool dJumpEffectBlizzard;

		public bool doubleJumpFart;

		public bool jumpAgainFart;

		public bool dJumpEffectFart;

		public bool doubleJumpSail;

		public bool jumpAgainSail;

		public bool dJumpEffectSail;

		public bool doubleJumpUnicorn;

		public bool jumpAgainUnicorn;

		public bool dJumpEffectUnicorn;

		public bool autoJump;

		public bool justJumped;

		public float jumpSpeedBoost;

		public int extraFall;

		public bool spawnMax;

		public int blockRange;

		public int[] grappling = new int[20];

		public int grapCount;

		public int rocketTime;

		public int rocketTimeMax = 7;

		public int rocketDelay;

		public int rocketDelay2;

		public bool rocketRelease;

		public bool rocketFrame;

		public int rocketBoots;

		public bool canRocket;

		public bool jumpBoost;

		public bool noFallDmg;

		public int swimTime;

		public bool killGuide;

		public bool killClothier;

		public bool lavaImmune;

		public bool gills;

		public bool slowFall;

		public bool findTreasure;

		public bool invis;

		public bool detectCreature;

		public bool nightVision;

		public bool enemySpawns;

		public float thorns;

		public bool turtleArmor;

		public bool turtleThorns;

		public bool spiderArmor;

		public bool setSolar;

		public bool setVortex;

		public bool setNebula;

		public int nebulaCD;

		public bool setStardust;

		public bool vortexStealthActive;

		public bool waterWalk;

		public bool waterWalk2;

		public bool gravControl;

		public bool gravControl2;

		public bool bee;

		public int lastChest;

		public int flyingPigChest = -1;

		public int chest = -1;

		public int chestX;

		public int chestY;

		public int talkNPC = -1;

		public int fallStart;

		public int fallStart2;

		public int potionDelayTime = Item.potionDelay;

		public int restorationDelayTime = Item.restorationDelay;

		private int cHead;

		private int cBody;

		private int cLegs;

		private int cHandOn;

		private int cHandOff;

		private int cBack;

		private int cFront;

		private int cShoe;

		private int cWaist;

		private int cShield;

		private int cNeck;

		private int cFace;

		private int cBalloon;

		private int cWings;

		private int cCarpet;

		public int cGrapple;

		public int cMount;

		public int cMinecart;

		public int cPet;

		public int cLight;

		public int cYorai;

		public int[] ownedProjectileCounts = new int[651];

		public bool[] npcTypeNoAggro = new bool[540];

		public int lastPortalColorIndex;

		public int _portalPhysicsTime;

		public bool portalPhysicsFlag;

		public float MountFishronSpecialCounter;

		public Vector2 MinionTargetPoint = Vector2.Zero;

		public List<Point> TouchedTiles = new List<Point>();

		private bool makeStrongBee;

		public int _funkytownCheckCD;

		public int[] hurtCooldowns = new int[2];

		public static bool lastPound = true;

		public Vector2 MountedCenter
		{
			get
			{
				return new Vector2(position.X + (float)(width / 2), position.Y + 21f + (float)mount.PlayerOffsetHitbox);
			}
			set
			{
				position = new Vector2(value.X - (float)(width / 2), value.Y - 21f - (float)mount.PlayerOffsetHitbox);
			}
		}

		public bool CCed
		{
			get
			{
				if (!frozen && !webbed)
				{
					return stoned;
				}
				return true;
			}
		}

		public bool Male
		{
			get
			{
				return skinVariant < 4;
			}
			set
			{
				if (value)
				{
					if (skinVariant >= 4)
					{
						skinVariant -= 4;
					}
				}
				else if (skinVariant < 4)
				{
					skinVariant += 4;
				}
			}
		}

		public bool ZoneDungeon
		{
			get
			{
				return zone1[0];
			}
			set
			{
				zone1[0] = value;
			}
		}

		public bool ZoneCorrupt
		{
			get
			{
				return zone1[1];
			}
			set
			{
				zone1[1] = value;
			}
		}

		public bool ZoneHoly
		{
			get
			{
				return zone1[2];
			}
			set
			{
				zone1[2] = value;
			}
		}

		public bool ZoneMeteor
		{
			get
			{
				return zone1[3];
			}
			set
			{
				zone1[3] = value;
			}
		}

		public bool ZoneJungle
		{
			get
			{
				return zone1[4];
			}
			set
			{
				zone1[4] = value;
			}
		}

		public bool ZoneSnow
		{
			get
			{
				return zone1[5];
			}
			set
			{
				zone1[5] = value;
			}
		}

		public bool ZoneCrimson
		{
			get
			{
				return zone1[6];
			}
			set
			{
				zone1[6] = value;
			}
		}

		public bool ZoneWaterCandle
		{
			get
			{
				return zone1[7];
			}
			set
			{
				zone1[7] = value;
			}
		}

		public bool ZonePeaceCandle
		{
			get
			{
				return zone2[0];
			}
			set
			{
				zone2[0] = value;
			}
		}

		public bool ZoneTowerSolar
		{
			get
			{
				return zone2[1];
			}
			set
			{
				zone2[1] = value;
			}
		}

		public bool ZoneTowerVortex
		{
			get
			{
				return zone2[2];
			}
			set
			{
				zone2[2] = value;
			}
		}

		public bool ZoneTowerNebula
		{
			get
			{
				return zone2[3];
			}
			set
			{
				zone2[3] = value;
			}
		}

		public bool ZoneTowerStardust
		{
			get
			{
				return zone2[4];
			}
			set
			{
				zone2[4] = value;
			}
		}

		public bool ZoneDesert
		{
			get
			{
				return zone2[5];
			}
			set
			{
				zone2[5] = value;
			}
		}

		public bool ZoneGlowshroom
		{
			get
			{
				return zone2[6];
			}
			set
			{
				zone2[6] = value;
			}
		}

		public bool ZoneUndergroundDesert
		{
			get
			{
				return zone2[7];
			}
			set
			{
				zone2[7] = value;
			}
		}

		public Vector2 Directions => new Vector2(direction, gravDir);

		public Vector2 DefaultSize => new Vector2(20f, 42f);

		public bool PortalPhysicsEnabled
		{
			get
			{
				if (_portalPhysicsTime > 0)
				{
					return !mount.Active;
				}
				return false;
			}
		}

		public bool MountFishronSpecial
		{
			get
			{
				if (statLife >= statLifeMax2 / 2 && (!wet || lavaWet || honeyWet) && !dripping)
				{
					return MountFishronSpecialCounter > 0f;
				}
				return true;
			}
		}

		public bool HasMinionTarget => MinionTargetPoint != Vector2.Zero;

		public bool SlimeDontHyperJump
		{
			get
			{
				if (mount.Active && mount.Type == 3 && wetSlime > 0)
				{
					return !controlJump;
				}
				return false;
			}
		}

		public static event Action<Player> OnEnterWorld;

		public void RotateRelativePoint(ref float x, ref float y)
		{
			Vector2 vector = RotatedRelativePoint(new Vector2(x, y));
			x = vector.X;
			y = vector.Y;
		}

		public Vector2 RotatedRelativePoint(Vector2 pos, bool rotateForward = true)
		{
			Vector2 value = position + fullRotationOrigin;
			Matrix matrix = Matrix.CreateRotationZ(fullRotation * (float)rotateForward.ToInt());
			pos -= position + fullRotationOrigin;
			pos = Vector2.Transform(pos, matrix);
			return pos + value;
		}

		public void HealEffect(int healAmount, bool broadcast = true)
		{
			CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.HealLife, string.Concat(healAmount));
			if (broadcast && Main.netMode == 1 && whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(35, -1, -1, "", whoAmI, healAmount);
			}
		}

		public void ManaEffect(int manaAmount)
		{
			CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.HealMana, string.Concat(manaAmount));
			if (Main.netMode == 1 && whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(43, -1, -1, "", whoAmI, manaAmount);
			}
		}

		public static void EnterWorld(Player player)
		{
			if (Player.OnEnterWorld != null)
			{
				Player.OnEnterWorld(player);
			}
		}

		public static byte FindClosest(Vector2 Position, int Width, int Height)
		{
			byte result = 0;
			for (int i = 0; i < 16; i++)
			{
				if (Main.player[i].active)
				{
					result = (byte)i;
					break;
				}
			}
			float num = -1f;
			for (int j = 0; j < 16; j++)
			{
				if (Main.player[j].active && !Main.player[j].dead)
				{
					float num2 = Math.Abs(Main.player[j].position.X + (float)(Main.player[j].width / 2) - (Position.X + (float)(Width / 2))) + Math.Abs(Main.player[j].position.Y + (float)(Main.player[j].height / 2) - (Position.Y + (float)(Height / 2)));
					if (num == -1f || num2 < num)
					{
						num = num2;
						result = (byte)j;
					}
				}
			}
			return result;
		}

		public void checkArmor()
		{
		}

		public void ToggleInv()
		{
			if (Main.ingameOptionsWindow)
			{
				IngameOptions.Close();
				return;
			}
			if (Main.achievementsWindow)
			{
				AchievementsUI.Close();
				return;
			}
			if (CaptureManager.Instance.Active)
			{
				CaptureManager.Instance.Active = false;
				return;
			}
			if (talkNPC >= 0)
			{
				talkNPC = -1;
				Main.npcChatCornerItem = 0;
				Main.npcChatText = "";
				Main.PlaySound(11);
				return;
			}
			if (sign >= 0)
			{
				sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
				Main.PlaySound(11);
				return;
			}
			if (Main.clothesWindow)
			{
				Main.CancelClothesWindow();
				return;
			}
			if (!Main.playerInventory)
			{
				Recipe.FindRecipes();
				Main.playerInventory = true;
				Main.EquipPageSelected = 0;
				Main.PlaySound(10);
				return;
			}
			Main.playerInventory = false;
			Main.EquipPageSelected = 0;
			Main.PlaySound(11);
			if (ItemSlot.Options.HighlightNewItems)
			{
				Item[] array = inventory;
				foreach (Item ıtem in array)
				{
					ıtem.newAndShiny = false;
				}
			}
		}

		public void dropItemCheck()
		{
			if (!Main.playerInventory)
			{
				noThrow = 0;
			}
			if (noThrow > 0)
			{
				noThrow--;
			}
			if (!Main.craftGuide && Main.guideItem.type > 0)
			{
				Main.guideItem.position = base.Center;
				Item ıtem = GetItem(whoAmI, Main.guideItem, false, true);
				if (ıtem.stack > 0)
				{
					int num = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem.type, ıtem.stack, false, Main.guideItem.prefix, true);
					Main.item[num].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num, 1f);
					}
				}
				Main.guideItem = new Item();
			}
			if (!Main.reforge && Main.reforgeItem.type > 0)
			{
				Main.reforgeItem.position = base.Center;
				Item ıtem2 = GetItem(whoAmI, Main.reforgeItem, false, true);
				if (ıtem2.stack > 0)
				{
					int num2 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, Main.reforgeItem.prefix, true);
					Main.item[num2].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num2, 1f);
					}
				}
				Main.reforgeItem = new Item();
			}
			if (Main.myPlayer == whoAmI)
			{
				inventory[58] = Main.mouseItem.Clone();
			}
			bool flag = true;
			if (Main.mouseItem.type > 0 && Main.mouseItem.stack > 0 && !Main.gamePaused)
			{
				tileTargetX = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
				tileTargetY = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
				if (gravDir == -1f)
				{
					tileTargetY = (int)((Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY) / 16f);
				}
				if (selectedItem != 58)
				{
					oldSelectItem = selectedItem;
				}
				selectedItem = 58;
				flag = false;
			}
			if (flag && selectedItem == 58)
			{
				selectedItem = oldSelectItem;
			}
			if (WorldGen.InWorld(tileTargetX, tileTargetY) && Main.tile[tileTargetX, tileTargetY] != null && Main.tile[tileTargetX, tileTargetY].type == 334 && ItemFitsWeaponRack(inventory[selectedItem]))
			{
				noThrow = 2;
			}
			if (WorldGen.InWorld(tileTargetX, tileTargetY) && Main.tile[tileTargetX, tileTargetY] != null && Main.tile[tileTargetX, tileTargetY].type == 395 && ItemFitsItemFrame(inventory[selectedItem]))
			{
				noThrow = 2;
			}
			if (Main.mouseItem.type > 0 && !Main.playerInventory)
			{
				Main.mouseItem.position = base.Center;
				Item ıtem3 = GetItem(whoAmI, Main.mouseItem, false, true);
				if (ıtem3.stack > 0)
				{
					int num3 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem3.type, ıtem3.stack, false, 0, true);
					Main.item[num3].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num3, 1f);
					}
				}
				Main.mouseItem = new Item();
				Recipe.FindRecipes();
			}
			if (((!controlThrow || !releaseThrow || inventory[selectedItem].favorited || inventory[selectedItem].type <= 0 || Main.chatMode) && (((!Main.mouseRight || mouseInterface || !Main.mouseRightRelease) && Main.playerInventory) || Main.mouseItem.type <= 0 || Main.mouseItem.stack <= 0)) || noThrow > 0)
			{
				return;
			}
			if (inventory[selectedItem].favorited)
			{
				inventory[selectedItem] = GetItem(whoAmI, inventory[selectedItem], false, true);
				if (selectedItem == 58)
				{
					Main.mouseItem = inventory[selectedItem];
				}
				Recipe.FindRecipes();
				if (inventory[selectedItem].type == 0)
				{
					return;
				}
			}
			Item ıtem4 = new Item();
			bool flag2 = false;
			if (((Main.mouseRight && !mouseInterface && Main.mouseRightRelease) || !Main.playerInventory) && Main.mouseItem.type > 0 && Main.mouseItem.stack > 0)
			{
				ıtem4 = inventory[selectedItem];
				inventory[selectedItem] = Main.mouseItem;
				delayUseItem = true;
				controlUseItem = false;
				flag2 = true;
			}
			int num4 = Item.NewItem((int)position.X, (int)position.Y, width, height, inventory[selectedItem].type);
			if (!flag2 && inventory[selectedItem].type == 8 && inventory[selectedItem].stack > 1)
			{
				inventory[selectedItem].stack--;
			}
			else
			{
				inventory[selectedItem].position = Main.item[num4].position;
				Main.item[num4] = inventory[selectedItem];
				inventory[selectedItem] = new Item();
			}
			if (Main.netMode == 0)
			{
				Main.item[num4].noGrabDelay = 100;
			}
			Main.item[num4].velocity.Y = -2f;
			Main.item[num4].velocity.X = (float)(4 * direction) + velocity.X;
			Main.item[num4].favorited = false;
			Main.item[num4].newAndShiny = false;
			if (((Main.mouseRight && !mouseInterface) || !Main.playerInventory) && Main.mouseItem.type > 0)
			{
				inventory[selectedItem] = ıtem4;
				Main.mouseItem = new Item();
			}
			else
			{
				itemAnimation = 10;
				itemAnimationMax = 10;
			}
			Recipe.FindRecipes();
			if (Main.netMode == 1)
			{
				NetMessage.SendData(21, -1, -1, "", num4);
			}
		}

		public int HasBuff(int type)
		{
			if (buffImmune[type])
			{
				return -1;
			}
			for (int i = 0; i < 22; i++)
			{
				if (buffTime[i] >= 1 && buffType[i] == type)
				{
					return i;
				}
			}
			return -1;
		}

		public void AddBuff(int type, int time1, bool quiet = true)
		{
			if (buffImmune[type])
			{
				return;
			}
			int num = time1;
			if (Main.expertMode && whoAmI == Main.myPlayer && (type == 20 || type == 22 || type == 23 || type == 24 || type == 30 || type == 31 || type == 32 || type == 33 || type == 35 || type == 36 || type == 39 || type == 44 || type == 46 || type == 47 || type == 69 || type == 70 || type == 80))
			{
				num = (int)(Main.expertDebuffTime * (float)num);
			}
			if (!quiet && Main.netMode == 1)
			{
				bool flag = true;
				for (int i = 0; i < 22; i++)
				{
					if (buffType[i] == type)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					NetMessage.SendData(55, -1, -1, "", whoAmI, type, num);
				}
			}
			int num2 = -1;
			for (int j = 0; j < 22; j++)
			{
				if (buffType[j] != type)
				{
					continue;
				}
				if (type == 94)
				{
					buffTime[j] += num;
					if (buffTime[j] > manaSickTimeMax)
					{
						buffTime[j] = manaSickTimeMax;
					}
				}
				else if (buffTime[j] < num)
				{
					buffTime[j] = num;
				}
				return;
			}
			if (Main.vanityPet[type] || Main.lightPet[type])
			{
				for (int k = 0; k < 22; k++)
				{
					if (Main.vanityPet[type] && Main.vanityPet[buffType[k]])
					{
						DelBuff(k);
					}
					if (Main.lightPet[type] && Main.lightPet[buffType[k]])
					{
						DelBuff(k);
					}
				}
			}
			while (num2 == -1)
			{
				int num3 = -1;
				for (int l = 0; l < 22; l++)
				{
					if (!Main.debuff[buffType[l]])
					{
						num3 = l;
						break;
					}
				}
				if (num3 == -1)
				{
					return;
				}
				for (int m = num3; m < 22; m++)
				{
					if (buffType[m] == 0)
					{
						num2 = m;
						break;
					}
				}
				if (num2 == -1)
				{
					DelBuff(num3);
				}
			}
			buffType[num2] = type;
			buffTime[num2] = num;
			if (!Main.meleeBuff[type])
			{
				return;
			}
			for (int n = 0; n < 22; n++)
			{
				if (n != num2 && Main.meleeBuff[buffType[n]])
				{
					DelBuff(n);
				}
			}
		}

		public void DelBuff(int b)
		{
			buffTime[b] = 0;
			buffType[b] = 0;
			for (int i = 0; i < 21; i++)
			{
				if (buffTime[i] == 0 || buffType[i] == 0)
				{
					for (int j = i + 1; j < 22; j++)
					{
						buffTime[j - 1] = buffTime[j];
						buffType[j - 1] = buffType[j];
						buffTime[j] = 0;
						buffType[j] = 0;
					}
				}
			}
		}

		public void ClearBuff(int type)
		{
			for (int i = 0; i < 22; i++)
			{
				if (buffType[i] == type)
				{
					DelBuff(i);
				}
			}
		}

		public int CountBuffs()
		{
			int num = 0;
			for (int i = 0; i < 22; i++)
			{
				if (buffType[num] > 0)
				{
					num++;
				}
			}
			return num;
		}

		public void QuickHeal()
		{
			if (noItems || statLife == statLifeMax2 || potionDelay > 0)
			{
				return;
			}
			int num = statLifeMax2 - statLife;
			Item ıtem = null;
			int num2 = -statLifeMax2;
			for (int i = 0; i < 58; i++)
			{
				Item ıtem2 = inventory[i];
				if (ıtem2.stack <= 0 || ıtem2.type <= 0 || !ıtem2.potion || ıtem2.healLife <= 0)
				{
					continue;
				}
				int num3 = ıtem2.healLife - num;
				if (num2 < 0)
				{
					if (num3 > num2)
					{
						ıtem = ıtem2;
						num2 = num3;
					}
				}
				else if (num3 < num2 && num3 >= 0)
				{
					ıtem = ıtem2;
					num2 = num3;
				}
			}
			if (ıtem == null)
			{
				return;
			}
			Main.PlaySound(2, (int)position.X, (int)position.Y, ıtem.useSound);
			if (ıtem.potion)
			{
				if (ıtem.type == 227)
				{
					potionDelay = restorationDelayTime;
					AddBuff(21, potionDelay);
				}
				else
				{
					potionDelay = potionDelayTime;
					AddBuff(21, potionDelay);
				}
			}
			statLife += ıtem.healLife;
			statMana += ıtem.healMana;
			if (statLife > statLifeMax2)
			{
				statLife = statLifeMax2;
			}
			if (statMana > statManaMax2)
			{
				statMana = statManaMax2;
			}
			if (ıtem.healLife > 0 && Main.myPlayer == whoAmI)
			{
				HealEffect(ıtem.healLife);
			}
			if (ıtem.healMana > 0 && Main.myPlayer == whoAmI)
			{
				ManaEffect(ıtem.healMana);
			}
			ıtem.stack--;
			if (ıtem.stack <= 0)
			{
				ıtem.type = 0;
			}
			Recipe.FindRecipes();
		}

		public void QuickMana()
		{
			if (noItems || statMana == statManaMax2)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < 58)
				{
					if (inventory[num].stack > 0 && inventory[num].type > 0 && inventory[num].healMana > 0 && (potionDelay == 0 || !inventory[num].potion))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Main.PlaySound(2, (int)position.X, (int)position.Y, inventory[num].useSound);
			if (inventory[num].potion)
			{
				if (inventory[num].type == 227)
				{
					potionDelay = restorationDelayTime;
					AddBuff(21, potionDelay);
				}
				else
				{
					potionDelay = potionDelayTime;
					AddBuff(21, potionDelay);
				}
			}
			statLife += inventory[num].healLife;
			statMana += inventory[num].healMana;
			if (statLife > statLifeMax2)
			{
				statLife = statLifeMax2;
			}
			if (statMana > statManaMax2)
			{
				statMana = statManaMax2;
			}
			if (inventory[num].healLife > 0 && Main.myPlayer == whoAmI)
			{
				HealEffect(inventory[num].healLife);
			}
			if (inventory[num].healMana > 0)
			{
				AddBuff(94, manaSickTime);
				if (Main.myPlayer == whoAmI)
				{
					ManaEffect(inventory[num].healMana);
				}
			}
			inventory[num].stack--;
			if (inventory[num].stack <= 0)
			{
				inventory[num].type = 0;
			}
			Recipe.FindRecipes();
		}

		public void QuickBuff()
		{
			if (noItems)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < 58; i++)
			{
				if (CountBuffs() == 22)
				{
					return;
				}
				if (inventory[i].stack <= 0 || inventory[i].type <= 0 || inventory[i].buffType <= 0 || inventory[i].summon || inventory[i].buffType == 90)
				{
					continue;
				}
				int num2 = inventory[i].buffType;
				bool flag = true;
				for (int j = 0; j < 22; j++)
				{
					if (num2 == 27 && (buffType[j] == num2 || buffType[j] == 101 || buffType[j] == 102))
					{
						flag = false;
						break;
					}
					if (buffType[j] == num2)
					{
						flag = false;
						break;
					}
					if (Main.meleeBuff[num2] && Main.meleeBuff[buffType[j]])
					{
						flag = false;
						break;
					}
				}
				if (Main.lightPet[inventory[i].buffType] || Main.vanityPet[inventory[i].buffType])
				{
					for (int k = 0; k < 22; k++)
					{
						if (Main.lightPet[buffType[k]] && Main.lightPet[inventory[i].buffType])
						{
							flag = false;
						}
						if (Main.vanityPet[buffType[k]] && Main.vanityPet[inventory[i].buffType])
						{
							flag = false;
						}
					}
				}
				if (inventory[i].mana > 0 && flag)
				{
					if (statMana >= (int)((float)inventory[i].mana * manaCost))
					{
						manaRegenDelay = (int)maxRegenDelay;
						statMana -= (int)((float)inventory[i].mana * manaCost);
					}
					else
					{
						flag = false;
					}
				}
				if (whoAmI == Main.myPlayer && inventory[i].type == 603 && !Main.cEd)
				{
					flag = false;
				}
				if (num2 == 27)
				{
					num2 = Main.rand.Next(3);
					if (num2 == 0)
					{
						num2 = 27;
					}
					if (num2 == 1)
					{
						num2 = 101;
					}
					if (num2 == 2)
					{
						num2 = 102;
					}
				}
				if (!flag)
				{
					continue;
				}
				num = inventory[i].useSound;
				int num3 = inventory[i].buffTime;
				if (num3 == 0)
				{
					num3 = 3600;
				}
				AddBuff(num2, num3);
				if (inventory[i].consumable)
				{
					inventory[i].stack--;
					if (inventory[i].stack <= 0)
					{
						inventory[i].type = 0;
					}
				}
			}
			if (num > 0)
			{
				Main.PlaySound(2, (int)position.X, (int)position.Y, num);
				Recipe.FindRecipes();
			}
		}

		public void QuickMount()
		{
			if (mount.Active)
			{
				mount.Dismount(this);
			}
			else
			{
				if (frozen || tongued || webbed || stoned || gravDir == -1f || noItems)
				{
					return;
				}
				Item ıtem = null;
				if (ıtem == null && miscEquips[3].mountType != -1 && !MountID.Sets.Cart[miscEquips[3].mountType])
				{
					ıtem = miscEquips[3];
				}
				if (ıtem == null)
				{
					for (int i = 0; i < 58; i++)
					{
						if (inventory[i].mountType != -1 && !MountID.Sets.Cart[inventory[i].mountType])
						{
							ıtem = inventory[i];
							break;
						}
					}
				}
				if (ıtem != null && ıtem.mountType != -1 && mount.CanMount(ıtem.mountType, this))
				{
					mount.SetMount(ıtem.mountType, this);
					if (ıtem.useSound > 0)
					{
						Main.PlaySound(2, (int)base.Center.X, (int)base.Center.Y, ıtem.useSound);
					}
				}
			}
		}

		public void QuickGrapple()
		{
			if (frozen || tongued || webbed || stoned)
			{
				return;
			}
			if (mount.Active)
			{
				mount.Dismount(this);
			}
			if (noItems)
			{
				return;
			}
			Item ıtem = null;
			if (ıtem == null && Main.projHook[miscEquips[4].shoot])
			{
				ıtem = miscEquips[4];
			}
			if (ıtem == null)
			{
				for (int i = 0; i < 58; i++)
				{
					if (Main.projHook[inventory[i].shoot])
					{
						ıtem = inventory[i];
						break;
					}
				}
			}
			if (ıtem == null)
			{
				return;
			}
			if (ıtem.shoot == 73)
			{
				int num = 0;
				for (int j = 0; j < 1000; j++)
				{
					if (Main.projectile[j].active && Main.projectile[j].owner == Main.myPlayer && (Main.projectile[j].type == 73 || Main.projectile[j].type == 74))
					{
						num++;
					}
				}
				if (num > 1)
				{
					ıtem = null;
				}
			}
			else if (ıtem.shoot == 165)
			{
				int num2 = 0;
				for (int k = 0; k < 1000; k++)
				{
					if (Main.projectile[k].active && Main.projectile[k].owner == Main.myPlayer && Main.projectile[k].type == 165)
					{
						num2++;
					}
				}
				if (num2 > 8)
				{
					ıtem = null;
				}
			}
			else if (ıtem.shoot == 372)
			{
				int num3 = 0;
				for (int l = 0; l < 1000; l++)
				{
					if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == 372)
					{
						num3++;
					}
				}
				if (num3 > 2)
				{
					ıtem = null;
				}
			}
			else if (ıtem.type == 3572)
			{
				int num4 = 0;
				bool flag = false;
				for (int m = 0; m < 1000; m++)
				{
					if (Main.projectile[m].active && Main.projectile[m].owner == Main.myPlayer && Main.projectile[m].type >= 646 && Main.projectile[m].type <= 649)
					{
						num4++;
						if (Main.projectile[m].ai[0] == 2f)
						{
							flag = true;
						}
					}
				}
				if (num4 > 4 || (!flag && num4 > 3))
				{
					ıtem = null;
				}
			}
			else
			{
				for (int n = 0; n < 1000; n++)
				{
					if (Main.projectile[n].active && Main.projectile[n].owner == Main.myPlayer && Main.projectile[n].type == ıtem.shoot && Main.projectile[n].ai[0] != 2f)
					{
						ıtem = null;
						break;
					}
				}
			}
			if (ıtem == null)
			{
				return;
			}
			Main.PlaySound(2, (int)position.X, (int)position.Y, ıtem.useSound);
			if (Main.netMode == 1 && whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(51, -1, -1, "", whoAmI, 2f);
			}
			int num5 = ıtem.shoot;
			float shootSpeed = ıtem.shootSpeed;
			int damage = ıtem.damage;
			float knockBack = ıtem.knockBack;
			if (num5 == 13 || num5 == 32 || num5 == 315 || (num5 >= 230 && num5 <= 235) || num5 == 331)
			{
				grappling[0] = -1;
				grapCount = 0;
				for (int num6 = 0; num6 < 1000; num6++)
				{
					if (Main.projectile[num6].active && Main.projectile[num6].owner == whoAmI)
					{
						if (Main.projectile[num6].type == 13)
						{
							Main.projectile[num6].Kill();
						}
						if (Main.projectile[num6].type == 331)
						{
							Main.projectile[num6].Kill();
						}
						if (Main.projectile[num6].type == 315)
						{
							Main.projectile[num6].Kill();
						}
						if (Main.projectile[num6].type >= 230 && Main.projectile[num6].type <= 235)
						{
							Main.projectile[num6].Kill();
						}
					}
				}
			}
			if (num5 == 256)
			{
				int num7 = 0;
				int num8 = -1;
				int num9 = 100000;
				for (int num10 = 0; num10 < 1000; num10++)
				{
					if (Main.projectile[num10].active && Main.projectile[num10].owner == whoAmI && Main.projectile[num10].type == 256)
					{
						num7++;
						if (Main.projectile[num10].timeLeft < num9)
						{
							num8 = num10;
							num9 = Main.projectile[num10].timeLeft;
						}
					}
				}
				if (num7 > 1)
				{
					Main.projectile[num8].Kill();
				}
			}
			if (num5 == 73)
			{
				for (int num11 = 0; num11 < 1000; num11++)
				{
					if (Main.projectile[num11].active && Main.projectile[num11].owner == whoAmI && Main.projectile[num11].type == 73)
					{
						num5 = 74;
					}
				}
			}
			if (ıtem.type == 3572)
			{
				int num12 = -1;
				int num13 = -1;
				for (int num14 = 0; num14 < 1000; num14++)
				{
					Projectile projectile = Main.projectile[num14];
					if (projectile.active && projectile.owner == whoAmI && projectile.type >= 646 && projectile.type <= 649 && (num13 == -1 || num13 < projectile.timeLeft))
					{
						num12 = projectile.type;
						num13 = projectile.timeLeft;
					}
				}
				switch (num12)
				{
				case 646:
					num5 = 647;
					break;
				case 647:
					num5 = 648;
					break;
				case 648:
					num5 = 649;
					break;
				case -1:
				case 649:
					num5 = 646;
					break;
				}
			}
			Vector2 vector = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
			float num15 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
			float num16 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
			if (gravDir == -1f)
			{
				num16 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
			}
			float num17 = (float)Math.Sqrt(num15 * num15 + num16 * num16);
			if ((float.IsNaN(num15) && float.IsNaN(num16)) || (num15 == 0f && num16 == 0f))
			{
				num15 = direction;
				num16 = 0f;
				num17 = shootSpeed;
			}
			else
			{
				num17 = shootSpeed / num17;
			}
			num15 *= num17;
			num16 *= num17;
			Projectile.NewProjectile(vector.X, vector.Y, num15, num16, num5, damage, knockBack, whoAmI);
		}

		public void StatusNPC(int type, int i)
		{
			if (meleeEnchant > 0)
			{
				if (meleeEnchant == 1)
				{
					Main.npc[i].AddBuff(70, 60 * Main.rand.Next(5, 10));
				}
				if (meleeEnchant == 2)
				{
					Main.npc[i].AddBuff(39, 60 * Main.rand.Next(3, 7));
				}
				if (meleeEnchant == 3)
				{
					Main.npc[i].AddBuff(24, 60 * Main.rand.Next(3, 7));
				}
				if (meleeEnchant == 5)
				{
					Main.npc[i].AddBuff(69, 60 * Main.rand.Next(10, 20));
				}
				if (meleeEnchant == 6)
				{
					Main.npc[i].AddBuff(31, 60 * Main.rand.Next(1, 4));
				}
				if (meleeEnchant == 8)
				{
					Main.npc[i].AddBuff(20, 60 * Main.rand.Next(5, 10));
				}
				if (meleeEnchant == 4)
				{
					Main.npc[i].AddBuff(72, 120);
				}
			}
			if (frostBurn)
			{
				Main.npc[i].AddBuff(44, 60 * Main.rand.Next(5, 15));
			}
			if (magmaStone)
			{
				if (Main.rand.Next(4) == 0)
				{
					Main.npc[i].AddBuff(24, 360);
				}
				else if (Main.rand.Next(2) == 0)
				{
					Main.npc[i].AddBuff(24, 240);
				}
				else
				{
					Main.npc[i].AddBuff(24, 120);
				}
			}
			if (type == 3211)
			{
				Main.npc[i].AddBuff(69, 60 * Main.rand.Next(5, 10));
			}
			switch (type)
			{
			case 121:
				if (Main.rand.Next(2) == 0)
				{
					Main.npc[i].AddBuff(24, 180);
				}
				break;
			case 122:
				if (Main.rand.Next(10) == 0)
				{
					Main.npc[i].AddBuff(24, 180);
				}
				break;
			case 190:
				if (Main.rand.Next(4) == 0)
				{
					Main.npc[i].AddBuff(20, 420);
				}
				break;
			case 217:
				if (Main.rand.Next(5) == 0)
				{
					Main.npc[i].AddBuff(24, 180);
				}
				break;
			case 1123:
				if (Main.rand.Next(10) != 0)
				{
					Main.npc[i].AddBuff(31, 120);
				}
				break;
			}
		}

		public void StatusPvP(int type, int i)
		{
			if (meleeEnchant > 0)
			{
				if (meleeEnchant == 1)
				{
					Main.player[i].AddBuff(70, 60 * Main.rand.Next(5, 10));
				}
				if (meleeEnchant == 2)
				{
					Main.player[i].AddBuff(39, 60 * Main.rand.Next(3, 7));
				}
				if (meleeEnchant == 3)
				{
					Main.player[i].AddBuff(24, 60 * Main.rand.Next(3, 7));
				}
				if (meleeEnchant == 5)
				{
					Main.player[i].AddBuff(69, 60 * Main.rand.Next(10, 20));
				}
				if (meleeEnchant == 6)
				{
					Main.player[i].AddBuff(31, 60 * Main.rand.Next(1, 4));
				}
				if (meleeEnchant == 8)
				{
					Main.player[i].AddBuff(20, 60 * Main.rand.Next(5, 10));
				}
			}
			if (frostBurn)
			{
				Main.player[i].AddBuff(44, 60 * Main.rand.Next(1, 8));
			}
			if (magmaStone)
			{
				if (Main.rand.Next(7) == 0)
				{
					Main.player[i].AddBuff(24, 360);
				}
				else if (Main.rand.Next(3) == 0)
				{
					Main.player[i].AddBuff(24, 120);
				}
				else
				{
					Main.player[i].AddBuff(24, 60);
				}
			}
			switch (type)
			{
			case 121:
				if (Main.rand.Next(2) == 0)
				{
					Main.player[i].AddBuff(24, 180, false);
				}
				break;
			case 122:
				if (Main.rand.Next(10) == 0)
				{
					Main.player[i].AddBuff(24, 180, false);
				}
				break;
			case 190:
				if (Main.rand.Next(4) == 0)
				{
					Main.player[i].AddBuff(20, 420, false);
				}
				break;
			case 217:
				if (Main.rand.Next(5) == 0)
				{
					Main.player[i].AddBuff(24, 180, false);
				}
				break;
			case 1123:
				if (Main.rand.Next(9) != 0)
				{
					Main.player[i].AddBuff(31, 120, false);
				}
				break;
			}
		}

		public void Ghost()
		{
			immune = false;
			immuneAlpha = 0;
			controlUp = false;
			controlLeft = false;
			controlDown = false;
			controlRight = false;
			controlJump = false;
			if (Main.hasFocus && !Main.chatMode && !Main.editSign && !Main.editChest && !Main.blockInput)
			{
				Keys[] pressedKeys = Main.keyState.GetPressedKeys();
				if (Main.blockKey != 0)
				{
					bool flag = false;
					for (int i = 0; i < pressedKeys.Length; i++)
					{
						if (pressedKeys[i] == Main.blockKey)
						{
							pressedKeys[i] = Keys.None;
							flag = true;
						}
					}
					if (!flag)
					{
						Main.blockKey = Keys.None;
					}
				}
				for (int j = 0; j < pressedKeys.Length; j++)
				{
					string a = string.Concat(pressedKeys[j]);
					if (a == Main.cUp)
					{
						controlUp = true;
					}
					if (a == Main.cLeft)
					{
						controlLeft = true;
					}
					if (a == Main.cDown)
					{
						controlDown = true;
					}
					if (a == Main.cRight)
					{
						controlRight = true;
					}
					if (a == Main.cJump)
					{
						controlJump = true;
					}
				}
			}
			if (controlUp || controlJump)
			{
				if (velocity.Y > 0f)
				{
					velocity.Y *= 0.9f;
				}
				velocity.Y -= 0.1f;
				if (velocity.Y < -3f)
				{
					velocity.Y = -3f;
				}
			}
			else if (controlDown)
			{
				if (velocity.Y < 0f)
				{
					velocity.Y *= 0.9f;
				}
				velocity.Y += 0.1f;
				if (velocity.Y > 3f)
				{
					velocity.Y = 3f;
				}
			}
			else if ((double)velocity.Y < -0.1 || (double)velocity.Y > 0.1)
			{
				velocity.Y *= 0.9f;
			}
			else
			{
				velocity.Y = 0f;
			}
			if (controlLeft && !controlRight)
			{
				if (velocity.X > 0f)
				{
					velocity.X *= 0.9f;
				}
				velocity.X -= 0.1f;
				if (velocity.X < -3f)
				{
					velocity.X = -3f;
				}
			}
			else if (controlRight && !controlLeft)
			{
				if (velocity.X < 0f)
				{
					velocity.X *= 0.9f;
				}
				velocity.X += 0.1f;
				if (velocity.X > 3f)
				{
					velocity.X = 3f;
				}
			}
			else if ((double)velocity.X < -0.1 || (double)velocity.X > 0.1)
			{
				velocity.X *= 0.9f;
			}
			else
			{
				velocity.X = 0f;
			}
			position += velocity;
			ghostFrameCounter++;
			if (velocity.X < 0f)
			{
				direction = -1;
			}
			else if (velocity.X > 0f)
			{
				direction = 1;
			}
			if (ghostFrameCounter >= 8)
			{
				ghostFrameCounter = 0;
				ghostFrame++;
				if (ghostFrame >= 4)
				{
					ghostFrame = 0;
				}
			}
			if (position.X < Main.leftWorld + (float)(Lighting.offScreenTiles * 16) + 16f)
			{
				position.X = Main.leftWorld + (float)(Lighting.offScreenTiles * 16) + 16f;
				velocity.X = 0f;
			}
			if (position.X + (float)width > Main.rightWorld - (float)(Lighting.offScreenTiles * 16) - 32f)
			{
				position.X = Main.rightWorld - (float)(Lighting.offScreenTiles * 16) - 32f - (float)width;
				velocity.X = 0f;
			}
			if (position.Y < Main.topWorld + (float)(Lighting.offScreenTiles * 16) + 16f)
			{
				position.Y = Main.topWorld + (float)(Lighting.offScreenTiles * 16) + 16f;
				if ((double)velocity.Y < -0.1)
				{
					velocity.Y = -0.1f;
				}
			}
			if (position.Y > Main.bottomWorld - (float)(Lighting.offScreenTiles * 16) - 32f - (float)height)
			{
				position.Y = Main.bottomWorld - (float)(Lighting.offScreenTiles * 16) - 32f - (float)height;
				velocity.Y = 0f;
			}
		}

		public void OnHit(float x, float y, Entity victim)
		{
			if (Main.myPlayer != whoAmI)
			{
				return;
			}
			if (onHitDodge && shadowDodgeTimer == 0 && Main.rand.Next(4) == 0)
			{
				if (!shadowDodge)
				{
					shadowDodgeTimer = 1800;
				}
				AddBuff(59, 1800);
			}
			if (onHitRegen)
			{
				AddBuff(58, 300);
			}
			if (stardustMinion && victim is NPC)
			{
				for (int i = 0; i < 1000; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (projectile.active && projectile.owner == whoAmI && projectile.type == 613 && !(projectile.localAI[1] > 0f) && Main.rand.Next(2) == 0)
					{
						Vector2 vector = new Vector2(x, y) - projectile.Center;
						if (vector.Length() > 0f)
						{
							vector.Normalize();
						}
						vector *= 20f;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector.X, vector.Y, 614, projectile.damage / 3, 0f, projectile.owner, 0f, victim.whoAmI);
						projectile.localAI[1] = 30 + Main.rand.Next(4) * 10;
					}
				}
			}
			if (onHitPetal && petalTimer == 0)
			{
				petalTimer = 20;
				int num = 1;
				if (x < position.X + (float)(width / 2))
				{
					num = -1;
				}
				num = direction;
				float num2 = Main.screenPosition.X;
				if (num < 0)
				{
					num2 += (float)Main.screenWidth;
				}
				float y2 = Main.screenPosition.Y;
				y2 += (float)Main.rand.Next(Main.screenHeight);
				Vector2 vector2 = new Vector2(num2, y2);
				float num3 = x - vector2.X;
				float num4 = y - vector2.Y;
				num3 += (float)Main.rand.Next(-50, 51) * 0.1f;
				num4 += (float)Main.rand.Next(-50, 51) * 0.1f;
				int num5 = 24;
				float num6 = (float)Math.Sqrt(num3 * num3 + num4 * num4);
				num6 = (float)num5 / num6;
				num3 *= num6;
				num4 *= num6;
				Projectile.NewProjectile(num2, y2, num3, num4, 221, 36, 0f, whoAmI);
			}
			if (!crystalLeaf || petalTimer != 0)
			{
				return;
			}
			int type = inventory[selectedItem].type;
			int num7 = 0;
			while (true)
			{
				if (num7 < 1000)
				{
					if (Main.projectile[num7].owner == whoAmI && Main.projectile[num7].type == 226)
					{
						break;
					}
					num7++;
					continue;
				}
				return;
			}
			petalTimer = 50;
			float num8 = 12f;
			Vector2 vector3 = new Vector2(Main.projectile[num7].position.X + (float)width * 0.5f, Main.projectile[num7].position.Y + (float)height * 0.5f);
			float num9 = x - vector3.X;
			float num10 = y - vector3.Y;
			float num11 = (float)Math.Sqrt(num9 * num9 + num10 * num10);
			num11 = num8 / num11;
			num9 *= num11;
			num10 *= num11;
			Projectile.NewProjectile(Main.projectile[num7].Center.X - 4f, Main.projectile[num7].Center.Y, num9, num10, 227, crystalLeafDamage, crystalLeafKB, whoAmI);
		}

		public void openPresent()
		{
			if (Main.rand.Next(15) == 0 && Main.hardMode)
			{
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, 602);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
				return;
			}
			if (Main.rand.Next(30) == 0)
			{
				int number2 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1922);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number2, 1f);
				}
				return;
			}
			if (Main.rand.Next(400) == 0)
			{
				int number3 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1927);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number3, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number4 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1870);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number4, 1f);
				}
				number4 = Item.NewItem((int)position.X, (int)position.Y, width, height, 97, Main.rand.Next(30, 61));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number4, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number5 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1909);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number5, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number6 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1917);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number6, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number7 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1915);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number7, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number8 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1918);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number8, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number9 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1921);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number9, 1f);
				}
				return;
			}
			if (Main.rand.Next(300) == 0)
			{
				int number10 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1923);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number10, 1f);
				}
				return;
			}
			if (Main.rand.Next(40) == 0)
			{
				int number11 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1907);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number11, 1f);
				}
				return;
			}
			if (Main.rand.Next(10) == 0)
			{
				int number12 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1908);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number12, 1f);
				}
				return;
			}
			if (Main.rand.Next(15) == 0)
			{
				switch (Main.rand.Next(5))
				{
				case 0:
				{
					int number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1932);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number14, 1f);
					}
					number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1933);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number14, 1f);
					}
					number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1934);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number14, 1f);
					}
					break;
				}
				case 1:
				{
					int number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1935);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number16, 1f);
					}
					number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1936);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number16, 1f);
					}
					number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1937);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number16, 1f);
					}
					break;
				}
				case 2:
				{
					int number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1940);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number17, 1f);
					}
					number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1941);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number17, 1f);
					}
					number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1942);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number17, 1f);
					}
					break;
				}
				case 3:
				{
					int number15 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1938);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number15, 1f);
					}
					break;
				}
				case 4:
				{
					int number13 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1939);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number13, 1f);
					}
					break;
				}
				}
				return;
			}
			if (Main.rand.Next(7) == 0)
			{
				int num = Main.rand.Next(3);
				if (num == 0)
				{
					num = 1911;
				}
				if (num == 1)
				{
					num = 1919;
				}
				if (num == 2)
				{
					num = 1920;
				}
				int number18 = Item.NewItem((int)position.X, (int)position.Y, width, height, num);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number18, 1f);
				}
				return;
			}
			if (Main.rand.Next(8) == 0)
			{
				int number19 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1912, Main.rand.Next(1, 4));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number19, 1f);
				}
				return;
			}
			if (Main.rand.Next(9) == 0)
			{
				int number20 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1913, Main.rand.Next(20, 41));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number20, 1f);
				}
				return;
			}
			switch (Main.rand.Next(3))
			{
			case 0:
			{
				int number22 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1872, Main.rand.Next(20, 50));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number22, 1f);
				}
				break;
			}
			case 1:
			{
				int number23 = Item.NewItem((int)position.X, (int)position.Y, width, height, 586, Main.rand.Next(20, 50));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number23, 1f);
				}
				break;
			}
			default:
			{
				int number21 = Item.NewItem((int)position.X, (int)position.Y, width, height, 591, Main.rand.Next(20, 50));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number21, 1f);
				}
				break;
			}
			}
		}

		public void QuickSpawnItem(int item, int stack = 1)
		{
			int number = Item.NewItem((int)position.X, (int)position.Y, width, height, item, stack, false, -1);
			if (Main.netMode == 1)
			{
				NetMessage.SendData(21, -1, -1, "", number, 1f);
			}
		}

		public void OpenBossBag(int type)
		{
			switch (type)
			{
			case 3318:
			{
				if (Main.rand.Next(2) == 0)
				{
					QuickSpawnItem(2430);
				}
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2493);
				}
				int num2 = Main.rand.Next(256, 259);
				int num3;
				for (num3 = Main.rand.Next(256, 259); num3 == num2; num3 = Main.rand.Next(256, 259))
				{
				}
				QuickSpawnItem(num2);
				QuickSpawnItem(num3);
				if (Main.rand.Next(2) == 0)
				{
					QuickSpawnItem(2610);
				}
				else
				{
					QuickSpawnItem(2585);
				}
				QuickSpawnItem(998);
				QuickSpawnItem(3090);
				break;
			}
			case 3319:
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2112);
				}
				if (Main.rand.Next(30) == 0)
				{
					QuickSpawnItem(1299);
				}
				if (WorldGen.crimson)
				{
					int num5 = Main.rand.Next(20) + 10;
					num5 += Main.rand.Next(20) + 10;
					num5 += Main.rand.Next(20) + 10;
					QuickSpawnItem(880, num5);
					num5 = Main.rand.Next(3) + 1;
					QuickSpawnItem(2171, num5);
				}
				else
				{
					int num6 = Main.rand.Next(20) + 10;
					num6 += Main.rand.Next(20) + 10;
					num6 += Main.rand.Next(20) + 10;
					QuickSpawnItem(56, num6);
					num6 = Main.rand.Next(3) + 1;
					QuickSpawnItem(59, num6);
					num6 = Main.rand.Next(30) + 20;
					QuickSpawnItem(47, num6);
				}
				QuickSpawnItem(3097);
				break;
			case 3320:
			{
				int num4 = Main.rand.Next(15, 30);
				num4 += Main.rand.Next(15, 31);
				QuickSpawnItem(56, num4);
				num4 = Main.rand.Next(10, 20);
				QuickSpawnItem(86, num4);
				if (Main.rand.Next(20) == 0)
				{
					QuickSpawnItem(994);
				}
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2111);
				}
				QuickSpawnItem(3224);
				break;
			}
			case 3321:
			{
				int num = Main.rand.Next(20, 46);
				num += Main.rand.Next(20, 46);
				QuickSpawnItem(880, num);
				num = Main.rand.Next(10, 20);
				QuickSpawnItem(1329, num);
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2104);
				}
				if (Main.rand.Next(20) == 0)
				{
					QuickSpawnItem(3060);
				}
				QuickSpawnItem(3223);
				break;
			}
			case 3322:
			{
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2108);
				}
				int num7 = Main.rand.Next(3);
				switch (num7)
				{
				case 0:
					num7 = 1121;
					break;
				case 1:
					num7 = 1123;
					break;
				case 2:
					num7 = 2888;
					break;
				}
				QuickSpawnItem(num7);
				QuickSpawnItem(3333);
				if (Main.rand.Next(3) == 0)
				{
					QuickSpawnItem(1132);
				}
				if (Main.rand.Next(9) == 0)
				{
					QuickSpawnItem(1170);
				}
				if (Main.rand.Next(9) == 0)
				{
					QuickSpawnItem(2502);
				}
				QuickSpawnItem(1129);
				QuickSpawnItem(Main.rand.Next(842, 845));
				QuickSpawnItem(1130, Main.rand.Next(10, 30));
				QuickSpawnItem(2431, Main.rand.Next(17, 30));
				break;
			}
			case 3323:
				QuickSpawnItem(3245);
				switch (Main.rand.Next(3))
				{
				case 0:
					QuickSpawnItem(1281);
					break;
				case 1:
					QuickSpawnItem(1273);
					break;
				default:
					QuickSpawnItem(1313);
					break;
				}
				break;
			case 3324:
			{
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2105);
				}
				QuickSpawnItem(367);
				if (!extraAccessory)
				{
					QuickSpawnItem(3335);
				}
				int num8 = Main.rand.Next(4);
				num8 = ((num8 != 3) ? (489 + num8) : 2998);
				QuickSpawnItem(num8);
				switch (Main.rand.Next(3))
				{
				case 0:
					QuickSpawnItem(514);
					break;
				case 1:
					QuickSpawnItem(426);
					break;
				case 2:
					QuickSpawnItem(434);
					break;
				}
				break;
			}
			case 3325:
				TryGettingDevArmor();
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2113);
				}
				QuickSpawnItem(548, Main.rand.Next(25, 41));
				QuickSpawnItem(1225, Main.rand.Next(20, 36));
				QuickSpawnItem(3355);
				break;
			case 3326:
				TryGettingDevArmor();
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2106);
				}
				QuickSpawnItem(549, Main.rand.Next(25, 41));
				QuickSpawnItem(1225, Main.rand.Next(20, 36));
				QuickSpawnItem(3354);
				break;
			case 3327:
				TryGettingDevArmor();
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2107);
				}
				QuickSpawnItem(547, Main.rand.Next(25, 41));
				QuickSpawnItem(1225, Main.rand.Next(20, 36));
				QuickSpawnItem(3356);
				break;
			case 3328:
				TryGettingDevArmor();
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2109);
				}
				QuickSpawnItem(1141);
				QuickSpawnItem(3336);
				if (Main.rand.Next(15) == 0)
				{
					QuickSpawnItem(1182);
				}
				if (Main.rand.Next(20) == 0)
				{
					QuickSpawnItem(1305);
				}
				if (Main.rand.Next(2) == 0)
				{
					QuickSpawnItem(1157);
				}
				if (Main.rand.Next(10) == 0)
				{
					QuickSpawnItem(3021);
				}
				switch (Main.rand.Next(6))
				{
				case 0:
					QuickSpawnItem(758);
					QuickSpawnItem(771, Main.rand.Next(50, 150));
					break;
				case 1:
					QuickSpawnItem(1255);
					break;
				case 2:
					QuickSpawnItem(788);
					break;
				case 3:
					QuickSpawnItem(1178);
					break;
				case 4:
					QuickSpawnItem(1259);
					break;
				case 5:
					QuickSpawnItem(1155);
					break;
				}
				break;
			case 3329:
				TryGettingDevArmor();
				QuickSpawnItem(3337);
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2110);
				}
				switch (Main.rand.Next(8))
				{
				case 0:
					QuickSpawnItem(1258);
					QuickSpawnItem(1261, Main.rand.Next(60, 100));
					break;
				case 1:
					QuickSpawnItem(1122);
					break;
				case 2:
					QuickSpawnItem(899);
					break;
				case 3:
					QuickSpawnItem(1248);
					break;
				case 4:
					QuickSpawnItem(1294);
					break;
				case 5:
					QuickSpawnItem(1295);
					break;
				case 6:
					QuickSpawnItem(1296);
					break;
				case 7:
					QuickSpawnItem(1297);
					break;
				}
				QuickSpawnItem(2218, Main.rand.Next(18, 24));
				break;
			case 3330:
				TryGettingDevArmor();
				QuickSpawnItem(3367);
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(2588);
				}
				if (Main.rand.Next(10) == 0)
				{
					QuickSpawnItem(2609);
				}
				switch (Main.rand.Next(5))
				{
				case 0:
					QuickSpawnItem(2611);
					break;
				case 1:
					QuickSpawnItem(2624);
					break;
				case 2:
					QuickSpawnItem(2622);
					break;
				case 3:
					QuickSpawnItem(2621);
					break;
				case 4:
					QuickSpawnItem(2623);
					break;
				}
				break;
			case 3331:
				TryGettingDevArmor();
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(3372);
				}
				break;
			case 3332:
			{
				TryGettingDevArmor();
				if (Main.rand.Next(7) == 0)
				{
					QuickSpawnItem(3373);
				}
				if (!HasItem(3384))
				{
					QuickSpawnItem(3384);
				}
				QuickSpawnItem(3460, Main.rand.Next(90, 111));
				QuickSpawnItem(1131);
				QuickSpawnItem(3577);
				int item = Utils.SelectRandom<int>(Main.rand, 3063, 3389, 3065, 1553, 3546, 3541, 3570, 3571, 3569);
				QuickSpawnItem(item);
				break;
			}
			}
			int num9 = -1;
			if (type == 3318)
			{
				num9 = 50;
			}
			if (type == 3319)
			{
				num9 = 4;
			}
			if (type == 3320)
			{
				num9 = 13;
			}
			if (type == 3321)
			{
				num9 = 266;
			}
			if (type == 3322)
			{
				num9 = 222;
			}
			if (type == 3323)
			{
				num9 = 35;
			}
			if (type == 3324)
			{
				num9 = 113;
			}
			if (type == 3325)
			{
				num9 = 134;
			}
			if (type == 3326)
			{
				num9 = 125;
			}
			if (type == 3327)
			{
				num9 = 127;
			}
			if (type == 3328)
			{
				num9 = 262;
			}
			if (type == 3329)
			{
				num9 = 245;
			}
			if (type == 3330)
			{
				num9 = 370;
			}
			if (type == 3331)
			{
				num9 = 439;
			}
			if (type == 3332)
			{
				num9 = 398;
			}
			if (num9 <= 0)
			{
				return;
			}
			NPC nPC = new NPC();
			nPC.SetDefaults(num9);
			float value = nPC.value;
			value *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
			if (Main.rand.Next(5) == 0)
			{
				value *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
			}
			if (Main.rand.Next(10) == 0)
			{
				value *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
			}
			if (Main.rand.Next(15) == 0)
			{
				value *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;
			}
			if (Main.rand.Next(20) == 0)
			{
				value *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
			}
			while ((int)value > 0)
			{
				if (value > 1000000f)
				{
					int num10 = (int)(value / 1000000f);
					value -= (float)(1000000 * num10);
					QuickSpawnItem(74, num10);
					continue;
				}
				if (value > 10000f)
				{
					int num11 = (int)(value / 10000f);
					value -= (float)(10000 * num11);
					QuickSpawnItem(73, num11);
					continue;
				}
				if (value > 100f)
				{
					int num12 = (int)(value / 100f);
					value -= (float)(100 * num12);
					QuickSpawnItem(72, num12);
					continue;
				}
				int num13 = (int)value;
				if (num13 < 1)
				{
					num13 = 1;
				}
				value -= (float)num13;
				QuickSpawnItem(71, num13);
			}
		}

		private void TryGettingDevArmor()
		{
			if (Main.rand.Next(20) == 0)
			{
				switch (Main.rand.Next(12))
				{
				case 0:
					QuickSpawnItem(666);
					QuickSpawnItem(667);
					QuickSpawnItem(668);
					QuickSpawnItem(665);
					QuickSpawnItem(3287);
					break;
				case 1:
					QuickSpawnItem(1554);
					QuickSpawnItem(1555);
					QuickSpawnItem(1556);
					QuickSpawnItem(1586);
					break;
				case 2:
					QuickSpawnItem(1587);
					QuickSpawnItem(1588);
					QuickSpawnItem(1586);
					break;
				case 3:
					QuickSpawnItem(1557);
					QuickSpawnItem(1558);
					QuickSpawnItem(1559);
					QuickSpawnItem(1585);
					break;
				case 4:
					QuickSpawnItem(1560);
					QuickSpawnItem(1561);
					QuickSpawnItem(1562);
					QuickSpawnItem(1584);
					break;
				case 5:
					QuickSpawnItem(1563);
					QuickSpawnItem(1564);
					QuickSpawnItem(1565);
					QuickSpawnItem(3582);
					break;
				case 6:
					QuickSpawnItem(1566);
					QuickSpawnItem(1567);
					QuickSpawnItem(1568);
					break;
				case 7:
					QuickSpawnItem(1580);
					QuickSpawnItem(1581);
					QuickSpawnItem(1582);
					QuickSpawnItem(1583);
					break;
				case 8:
					QuickSpawnItem(3226);
					QuickSpawnItem(3227);
					QuickSpawnItem(3228);
					QuickSpawnItem(3288);
					break;
				case 9:
					QuickSpawnItem(3583);
					QuickSpawnItem(3581);
					QuickSpawnItem(3578);
					QuickSpawnItem(3579);
					QuickSpawnItem(3580);
					break;
				case 10:
					QuickSpawnItem(3585);
					QuickSpawnItem(3586);
					QuickSpawnItem(3587);
					QuickSpawnItem(3588);
					QuickSpawnItem(3024, 4);
					break;
				case 11:
					QuickSpawnItem(3589);
					QuickSpawnItem(3590);
					QuickSpawnItem(3591);
					QuickSpawnItem(3592);
					QuickSpawnItem(3599, 4);
					break;
				}
			}
		}

		public void openCrate(int type)
		{
			int num = type - 2334;
			if (type >= 3203)
			{
				num = type - 3203 + 3;
			}
			switch (num)
			{
			case 0:
			{
				bool flag2 = true;
				while (flag2)
				{
					if (Main.hardMode && flag2 && Main.rand.Next(200) == 0)
					{
						int number12 = Item.NewItem((int)position.X, (int)position.Y, width, height, 3064);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number12, 1f);
						}
						flag2 = false;
					}
					if (flag2 && Main.rand.Next(40) == 0)
					{
						int type10 = 3200;
						int stack10 = 1;
						int number13 = Item.NewItem((int)position.X, (int)position.Y, width, height, type10, stack10, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number13, 1f);
						}
						flag2 = false;
					}
					if (flag2 && Main.rand.Next(40) == 0)
					{
						int type11 = 3201;
						int stack11 = 1;
						int number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, type11, stack11, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number14, 1f);
						}
						flag2 = false;
					}
					if (Main.hardMode && flag2 && Main.rand.Next(25) == 0)
					{
						int type12 = 2424;
						int stack12 = 1;
						int number15 = Item.NewItem((int)position.X, (int)position.Y, width, height, type12, stack12, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number15, 1f);
						}
						flag2 = false;
					}
					if (Main.rand.Next(45) == 0)
					{
						int num5 = Main.rand.Next(5);
						switch (num5)
						{
						case 0:
							num5 = 285;
							break;
						case 1:
							num5 = 953;
							break;
						case 2:
							num5 = 946;
							break;
						case 3:
							num5 = 3068;
							break;
						case 4:
							num5 = 3084;
							break;
						}
						int number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, num5, 1, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number16, 1f);
						}
						flag2 = false;
					}
					if (!Main.hardMode && flag2 && Main.rand.Next(50) == 0)
					{
						int type13 = 997;
						int stack13 = 1;
						int number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, type13, stack13);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number17, 1f);
						}
						flag2 = false;
					}
					if (Main.rand.Next(7) == 0)
					{
						int type14;
						int stack14;
						if (Main.rand.Next(3) == 0)
						{
							type14 = 73;
							stack14 = Main.rand.Next(1, 6);
						}
						else
						{
							type14 = 72;
							stack14 = Main.rand.Next(20, 91);
						}
						int number18 = Item.NewItem((int)position.X, (int)position.Y, width, height, type14, stack14);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number18, 1f);
						}
						flag2 = false;
					}
					if (Main.rand.Next(7) == 0)
					{
						int num6 = Main.rand.Next(8);
						switch (num6)
						{
						case 0:
							num6 = 12;
							break;
						case 1:
							num6 = 11;
							break;
						case 2:
							num6 = 14;
							break;
						case 3:
							num6 = 13;
							break;
						case 4:
							num6 = 699;
							break;
						case 5:
							num6 = 700;
							break;
						case 6:
							num6 = 701;
							break;
						case 7:
							num6 = 702;
							break;
						}
						if (Main.hardMode && Main.rand.Next(2) == 0)
						{
							num6 = Main.rand.Next(6);
							switch (num6)
							{
							case 0:
								num6 = 364;
								break;
							case 1:
								num6 = 365;
								break;
							case 2:
								num6 = 366;
								break;
							case 3:
								num6 = 1104;
								break;
							case 4:
								num6 = 1105;
								break;
							case 5:
								num6 = 1106;
								break;
							}
						}
						int stack15 = Main.rand.Next(8, 21);
						int number19 = Item.NewItem((int)position.X, (int)position.Y, width, height, num6, stack15);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number19, 1f);
						}
						flag2 = false;
					}
					if (Main.rand.Next(8) == 0)
					{
						int num7 = Main.rand.Next(8);
						switch (num7)
						{
						case 0:
							num7 = 20;
							break;
						case 1:
							num7 = 22;
							break;
						case 2:
							num7 = 21;
							break;
						case 3:
							num7 = 19;
							break;
						case 4:
							num7 = 703;
							break;
						case 5:
							num7 = 704;
							break;
						case 6:
							num7 = 705;
							break;
						case 7:
							num7 = 706;
							break;
						}
						int num8 = Main.rand.Next(2, 8);
						if (Main.hardMode && Main.rand.Next(2) == 0)
						{
							num7 = Main.rand.Next(6);
							switch (num7)
							{
							case 0:
								num7 = 381;
								break;
							case 1:
								num7 = 382;
								break;
							case 2:
								num7 = 391;
								break;
							case 3:
								num7 = 1184;
								break;
							case 4:
								num7 = 1191;
								break;
							case 5:
								num7 = 1198;
								break;
							}
							num8 -= Main.rand.Next(2);
						}
						int number20 = Item.NewItem((int)position.X, (int)position.Y, width, height, num7, num8);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number20, 1f);
						}
						flag2 = false;
					}
					if (Main.rand.Next(7) == 0)
					{
						int num9 = Main.rand.Next(10);
						switch (num9)
						{
						case 0:
							num9 = 288;
							break;
						case 1:
							num9 = 290;
							break;
						case 2:
							num9 = 292;
							break;
						case 3:
							num9 = 299;
							break;
						case 4:
							num9 = 298;
							break;
						case 5:
							num9 = 304;
							break;
						case 6:
							num9 = 291;
							break;
						case 7:
							num9 = 2322;
							break;
						case 8:
							num9 = 2323;
							break;
						case 9:
							num9 = 2329;
							break;
						}
						int stack16 = Main.rand.Next(1, 4);
						int number21 = Item.NewItem((int)position.X, (int)position.Y, width, height, num9, stack16);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number21, 1f);
						}
						flag2 = false;
					}
				}
				if (Main.rand.Next(3) == 0)
				{
					int num10 = Main.rand.Next(2);
					switch (num10)
					{
					case 0:
						num10 = 28;
						break;
					case 1:
						num10 = 110;
						break;
					}
					int stack17 = Main.rand.Next(5, 16);
					int number22 = Item.NewItem((int)position.X, (int)position.Y, width, height, num10, stack17);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number22, 1f);
					}
				}
				if (Main.rand.Next(3) == 0)
				{
					int type15 = (Main.rand.Next(3) != 0) ? 2674 : 2675;
					int stack18 = Main.rand.Next(1, 5);
					int number23 = Item.NewItem((int)position.X, (int)position.Y, width, height, type15, stack18);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number23, 1f);
					}
				}
				return;
			}
			case 1:
			{
				bool flag = true;
				while (flag)
				{
					if (Main.hardMode && flag && Main.rand.Next(60) == 0)
					{
						int number = Item.NewItem((int)position.X, (int)position.Y, width, height, 3064);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number, 1f);
						}
						flag = false;
					}
					if (flag && Main.rand.Next(25) == 0)
					{
						int type2 = 2501;
						int stack = 1;
						int number2 = Item.NewItem((int)position.X, (int)position.Y, width, height, type2, stack);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number2, 1f);
						}
						flag = false;
					}
					if (flag && Main.rand.Next(20) == 0)
					{
						int type3 = 2587;
						int stack2 = 1;
						int number3 = Item.NewItem((int)position.X, (int)position.Y, width, height, type3, stack2);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number3, 1f);
						}
						flag = false;
					}
					if (flag && Main.rand.Next(15) == 0)
					{
						int type4 = 2608;
						int stack3 = 1;
						int number4 = Item.NewItem((int)position.X, (int)position.Y, width, height, type4, stack3, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number4, 1f);
						}
						flag = false;
					}
					if (flag && Main.rand.Next(20) == 0)
					{
						int type5 = 3200;
						int stack4 = 1;
						int number5 = Item.NewItem((int)position.X, (int)position.Y, width, height, type5, stack4, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number5, 1f);
						}
						flag = false;
					}
					if (flag && Main.rand.Next(20) == 0)
					{
						int type6 = 3201;
						int stack5 = 1;
						int number6 = Item.NewItem((int)position.X, (int)position.Y, width, height, type6, stack5, false, -1);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number6, 1f);
						}
						flag = false;
					}
					if (Main.rand.Next(4) == 0)
					{
						int type7 = 73;
						int stack6 = Main.rand.Next(5, 11);
						int number7 = Item.NewItem((int)position.X, (int)position.Y, width, height, type7, stack6);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number7, 1f);
						}
						flag = false;
					}
					if (Main.rand.Next(4) == 0)
					{
						int num2 = Main.rand.Next(8);
						switch (num2)
						{
						case 0:
							num2 = 20;
							break;
						case 1:
							num2 = 22;
							break;
						case 2:
							num2 = 21;
							break;
						case 3:
							num2 = 19;
							break;
						case 4:
							num2 = 703;
							break;
						case 5:
							num2 = 704;
							break;
						case 6:
							num2 = 705;
							break;
						case 7:
							num2 = 706;
							break;
						}
						int num3 = Main.rand.Next(6, 15);
						if (Main.hardMode && Main.rand.Next(3) != 0)
						{
							num2 = Main.rand.Next(6);
							switch (num2)
							{
							case 0:
								num2 = 381;
								break;
							case 1:
								num2 = 382;
								break;
							case 2:
								num2 = 391;
								break;
							case 3:
								num2 = 1184;
								break;
							case 4:
								num2 = 1191;
								break;
							case 5:
								num2 = 1198;
								break;
							}
							num3 -= Main.rand.Next(2);
						}
						int number8 = Item.NewItem((int)position.X, (int)position.Y, width, height, num2, num3);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number8, 1f);
						}
						flag = false;
					}
					if (Main.rand.Next(4) == 0)
					{
						int num4 = Main.rand.Next(8);
						switch (num4)
						{
						case 0:
							num4 = 288;
							break;
						case 1:
							num4 = 296;
							break;
						case 2:
							num4 = 304;
							break;
						case 3:
							num4 = 305;
							break;
						case 4:
							num4 = 2322;
							break;
						case 5:
							num4 = 2323;
							break;
						case 6:
							num4 = 2324;
							break;
						case 7:
							num4 = 2327;
							break;
						}
						int stack7 = Main.rand.Next(2, 5);
						int number9 = Item.NewItem((int)position.X, (int)position.Y, width, height, num4, stack7);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number9, 1f);
						}
						flag = false;
					}
				}
				if (Main.rand.Next(2) == 0)
				{
					int type8 = Main.rand.Next(188, 190);
					int stack8 = Main.rand.Next(5, 16);
					int number10 = Item.NewItem((int)position.X, (int)position.Y, width, height, type8, stack8);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number10, 1f);
					}
				}
				if (Main.rand.Next(2) == 0)
				{
					int type9 = (Main.rand.Next(3) != 0) ? 2675 : 2676;
					int stack9 = Main.rand.Next(2, 5);
					int number11 = Item.NewItem((int)position.X, (int)position.Y, width, height, type9, stack9);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number11, 1f);
					}
				}
				return;
			}
			case 2:
			{
				bool flag3 = true;
				while (flag3)
				{
					if (Main.hardMode && flag3 && Main.rand.Next(20) == 0)
					{
						int number24 = Item.NewItem((int)position.X, (int)position.Y, width, height, 3064);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number24, 1f);
						}
						flag3 = false;
					}
					if (flag3 && Main.rand.Next(10) == 0)
					{
						int type16 = 2491;
						int stack19 = 1;
						int number25 = Item.NewItem((int)position.X, (int)position.Y, width, height, type16, stack19);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number25, 1f);
						}
						flag3 = false;
					}
					if (Main.rand.Next(3) == 0)
					{
						int type17 = 73;
						int stack20 = Main.rand.Next(8, 21);
						int number26 = Item.NewItem((int)position.X, (int)position.Y, width, height, type17, stack20);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", number26, 1f);
						}
						flag3 = false;
					}
					if (Main.rand.Next(3) != 0)
					{
						continue;
					}
					int num11 = Main.rand.Next(4);
					switch (num11)
					{
					case 0:
						num11 = 21;
						break;
					case 1:
						num11 = 19;
						break;
					case 2:
						num11 = 705;
						break;
					case 3:
						num11 = 706;
						break;
					}
					if (Main.hardMode && Main.rand.Next(3) != 0)
					{
						num11 = Main.rand.Next(4);
						switch (num11)
						{
						case 0:
							num11 = 382;
							break;
						case 1:
							num11 = 391;
							break;
						case 2:
							num11 = 1191;
							break;
						case 3:
							num11 = 1198;
							break;
						}
					}
					int stack21 = Main.rand.Next(15, 31);
					int number27 = Item.NewItem((int)position.X, (int)position.Y, width, height, num11, stack21);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number27, 1f);
					}
					flag3 = false;
				}
				if (Main.rand.Next(3) == 0)
				{
					int num12 = Main.rand.Next(5);
					switch (num12)
					{
					case 0:
						num12 = 288;
						break;
					case 1:
						num12 = 296;
						break;
					case 2:
						num12 = 305;
						break;
					case 3:
						num12 = 2322;
						break;
					case 4:
						num12 = 2323;
						break;
					}
					int stack22 = Main.rand.Next(2, 6);
					int number28 = Item.NewItem((int)position.X, (int)position.Y, width, height, num12, stack22);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number28, 1f);
					}
				}
				if (Main.rand.Next(2) == 0)
				{
					int type18 = Main.rand.Next(499, 501);
					int stack23 = Main.rand.Next(5, 21);
					int number29 = Item.NewItem((int)position.X, (int)position.Y, width, height, type18, stack23);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number29, 1f);
					}
				}
				if (Main.rand.Next(3) != 0)
				{
					int type19 = 2676;
					int stack24 = Main.rand.Next(3, 8);
					int number30 = Item.NewItem((int)position.X, (int)position.Y, width, height, type19, stack24);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number30, 1f);
					}
				}
				return;
			}
			}
			int maxValue = 6;
			bool flag4 = true;
			while (flag4)
			{
				if (num == 3 && flag4 && Main.rand.Next(maxValue) == 0)
				{
					int type20;
					switch (Main.rand.Next(5))
					{
					case 0:
						type20 = 162;
						break;
					case 1:
						type20 = 111;
						break;
					case 2:
						type20 = 96;
						break;
					case 3:
						type20 = 115;
						break;
					default:
						type20 = 64;
						break;
					}
					int number31 = Item.NewItem((int)position.X, (int)position.Y, width, height, type20, 1, false, -1);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number31, 1f);
					}
					flag4 = false;
				}
				if (num == 4 && flag4 && Main.rand.Next(maxValue) == 0)
				{
					int type21;
					switch (Main.rand.Next(5))
					{
					case 0:
						type21 = 800;
						break;
					case 1:
						type21 = 802;
						break;
					case 2:
						type21 = 1256;
						break;
					case 3:
						type21 = 1290;
						break;
					default:
						type21 = 3062;
						break;
					}
					int number32 = Item.NewItem((int)position.X, (int)position.Y, width, height, type21, 1, false, -1);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number32, 1f);
					}
					flag4 = false;
				}
				if (num == 5 && flag4 && Main.rand.Next(maxValue) == 0)
				{
					int type22 = 3085;
					int number33 = Item.NewItem((int)position.X, (int)position.Y, width, height, type22, 1, false, -1);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number33, 1f);
					}
					flag4 = false;
				}
				if (num == 6 && flag4 && Main.rand.Next(maxValue) == 0)
				{
					int type23;
					switch (Main.rand.Next(3))
					{
					case 0:
						type23 = 158;
						break;
					case 1:
						type23 = 65;
						break;
					default:
						type23 = 159;
						break;
					}
					int number34 = Item.NewItem((int)position.X, (int)position.Y, width, height, type23, 1, false, -1);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number34, 1f);
					}
					flag4 = false;
				}
				if (num == 8 && flag4 && Main.rand.Next(maxValue) == 0)
				{
					int type24;
					switch (Main.rand.Next(5))
					{
					case 0:
						type24 = 212;
						break;
					case 1:
						type24 = 964;
						break;
					case 2:
						type24 = 211;
						break;
					case 3:
						type24 = 213;
						break;
					default:
						type24 = 2292;
						break;
					}
					int number35 = Item.NewItem((int)position.X, (int)position.Y, width, height, type24, 1, false, -1);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number35, 1f);
					}
					flag4 = false;
				}
				if (Main.rand.Next(4) == 0)
				{
					int type25 = 73;
					int stack25 = Main.rand.Next(5, 13);
					int number36 = Item.NewItem((int)position.X, (int)position.Y, width, height, type25, stack25);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number36, 1f);
					}
					flag4 = false;
				}
				if (Main.rand.Next(4) != 0)
				{
					continue;
				}
				int num13 = Main.rand.Next(6);
				switch (num13)
				{
				case 0:
					num13 = 22;
					break;
				case 1:
					num13 = 21;
					break;
				case 2:
					num13 = 19;
					break;
				case 3:
					num13 = 704;
					break;
				case 4:
					num13 = 705;
					break;
				case 5:
					num13 = 706;
					break;
				}
				int num14 = Main.rand.Next(10, 21);
				if (Main.hardMode && Main.rand.Next(3) != 0)
				{
					num13 = Main.rand.Next(6);
					switch (num13)
					{
					case 0:
						num13 = 381;
						break;
					case 1:
						num13 = 382;
						break;
					case 2:
						num13 = 391;
						break;
					case 3:
						num13 = 1184;
						break;
					case 4:
						num13 = 1191;
						break;
					case 5:
						num13 = 1198;
						break;
					}
					num14 -= Main.rand.Next(3);
				}
				int number37 = Item.NewItem((int)position.X, (int)position.Y, width, height, num13, num14);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number37, 1f);
				}
				flag4 = false;
			}
			if (Main.rand.Next(4) == 0)
			{
				int num15 = Main.rand.Next(6);
				switch (num15)
				{
				case 0:
					num15 = 288;
					break;
				case 1:
					num15 = 296;
					break;
				case 2:
					num15 = 304;
					break;
				case 3:
					num15 = 305;
					break;
				case 4:
					num15 = 2322;
					break;
				case 5:
					num15 = 2323;
					break;
				}
				int stack26 = Main.rand.Next(2, 5);
				int number38 = Item.NewItem((int)position.X, (int)position.Y, width, height, num15, stack26);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number38, 1f);
				}
				flag4 = false;
			}
			if (Main.rand.Next(2) == 0)
			{
				int type26 = Main.rand.Next(188, 190);
				int stack27 = Main.rand.Next(5, 18);
				int number39 = Item.NewItem((int)position.X, (int)position.Y, width, height, type26, stack27);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number39, 1f);
				}
			}
			if (Main.rand.Next(2) == 0)
			{
				int type27 = (Main.rand.Next(2) != 0) ? 2675 : 2676;
				int stack28 = Main.rand.Next(2, 7);
				int number40 = Item.NewItem((int)position.X, (int)position.Y, width, height, type27, stack28);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number40, 1f);
				}
			}
			if (num != 3 && num != 4 && num != 7)
			{
				return;
			}
			if (Main.hardMode && Main.rand.Next(2) == 0)
			{
				int type28 = 521;
				if (num == 7)
				{
					type28 = 520;
				}
				int stack29 = Main.rand.Next(2, 6);
				int number41 = Item.NewItem((int)position.X, (int)position.Y, width, height, type28, stack29);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number41, 1f);
				}
			}
			if (Main.hardMode && Main.rand.Next(2) == 0)
			{
				int type29 = 522;
				int stack30 = Main.rand.Next(2, 6);
				switch (num)
				{
				case 4:
					type29 = 1332;
					break;
				case 7:
					type29 = 502;
					stack30 = Main.rand.Next(4, 11);
					break;
				}
				int number42 = Item.NewItem((int)position.X, (int)position.Y, width, height, type29, stack30);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number42, 1f);
				}
			}
		}

		public bool consumeItem(int type)
		{
			for (int i = 0; i < 58; i++)
			{
				if (inventory[i].stack > 0 && inventory[i].type == type)
				{
					inventory[i].stack--;
					if (inventory[i].stack <= 0)
					{
						inventory[i].SetDefaults();
					}
					return true;
				}
			}
			return false;
		}

		public void openLockBox()
		{
			bool flag = true;
			while (flag)
			{
				flag = false;
				int num = 0;
				switch (Main.rand.Next(7))
				{
				case 1:
					num = 329;
					break;
				case 2:
					num = 155;
					break;
				case 3:
					num = 156;
					break;
				case 4:
					num = 157;
					break;
				case 5:
					num = 163;
					break;
				case 6:
					num = 113;
					break;
				default:
					num = 164;
					break;
				}
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, num, 1, false, -1);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
				if (Main.rand.Next(3) == 0)
				{
					flag = false;
					int num2 = Main.rand.Next(1, 4);
					if (Main.rand.Next(2) == 0)
					{
						num2 += Main.rand.Next(2);
					}
					if (Main.rand.Next(3) == 0)
					{
						num2 += Main.rand.Next(3);
					}
					if (Main.rand.Next(4) == 0)
					{
						num2 += Main.rand.Next(3);
					}
					if (Main.rand.Next(5) == 0)
					{
						num2 += Main.rand.Next(1, 3);
					}
					int number2 = Item.NewItem((int)position.X, (int)position.Y, width, height, 73, num2);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number2, 1f);
					}
				}
				if (Main.rand.Next(2) == 0)
				{
					flag = false;
					int number3 = Item.NewItem((int)position.X, (int)position.Y, width, height, 72, Main.rand.Next(10, 100));
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number3, 1f);
					}
				}
				if (Main.rand.Next(3) == 0)
				{
					flag = false;
					int number4 = Item.NewItem((int)position.X, (int)position.Y, width, height, 188, Main.rand.Next(2, 6));
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number4, 1f);
					}
				}
				if (Main.rand.Next(3) == 0)
				{
					flag = false;
					int type;
					switch (Main.rand.Next(9))
					{
					case 0:
						type = 296;
						break;
					case 1:
						type = 2346;
						break;
					case 2:
						type = 305;
						break;
					case 3:
						type = 2323;
						break;
					case 4:
						type = 292;
						break;
					case 5:
						type = 294;
						break;
					case 6:
						type = 288;
						break;
					default:
						type = ((Main.netMode != 1) ? 2350 : 2997);
						break;
					}
					int number5 = Item.NewItem((int)position.X, (int)position.Y, width, height, type, Main.rand.Next(1, 4));
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number5, 1f);
					}
				}
			}
		}

		public void openHerbBag()
		{
			int num = Main.rand.Next(2, 5);
			if (Main.rand.Next(3) == 0)
			{
				num++;
			}
			for (int i = 0; i < num; i++)
			{
				int num2 = Main.rand.Next(14);
				if (num2 == 0)
				{
					num2 = 313;
				}
				if (num2 == 1)
				{
					num2 = 314;
				}
				if (num2 == 2)
				{
					num2 = 315;
				}
				if (num2 == 3)
				{
					num2 = 317;
				}
				if (num2 == 4)
				{
					num2 = 316;
				}
				if (num2 == 5)
				{
					num2 = 318;
				}
				if (num2 == 6)
				{
					num2 = 2358;
				}
				if (num2 == 7)
				{
					num2 = 307;
				}
				if (num2 == 8)
				{
					num2 = 308;
				}
				if (num2 == 9)
				{
					num2 = 309;
				}
				if (num2 == 10)
				{
					num2 = 311;
				}
				if (num2 == 11)
				{
					num2 = 310;
				}
				if (num2 == 12)
				{
					num2 = 312;
				}
				if (num2 == 13)
				{
					num2 = 2357;
				}
				int num3 = Main.rand.Next(2, 5);
				if (Main.rand.Next(3) == 0)
				{
					num3 += Main.rand.Next(1, 5);
				}
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, num2, num3);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
			}
		}

		public void openGoodieBag()
		{
			if (Main.rand.Next(150) == 0)
			{
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, 1810);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
				return;
			}
			if (Main.rand.Next(150) == 0)
			{
				int number2 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1800);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number2, 1f);
				}
				return;
			}
			if (Main.rand.Next(4) == 0)
			{
				int number3 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1809, Main.rand.Next(10, 41));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number3, 1f);
				}
				return;
			}
			if (Main.rand.Next(10) == 0)
			{
				int number4 = Item.NewItem((int)position.X, (int)position.Y, width, height, Main.rand.Next(1846, 1851));
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number4, 1f);
				}
				return;
			}
			switch (Main.rand.Next(19))
			{
			case 0:
			{
				int number6 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1749);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number6, 1f);
				}
				number6 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1750);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number6, 1f);
				}
				number6 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1751);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number6, 1f);
				}
				break;
			}
			case 1:
			{
				int number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1746);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number17, 1f);
				}
				number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1747);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number17, 1f);
				}
				number17 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1748);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number17, 1f);
				}
				break;
			}
			case 2:
			{
				int number18 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1752);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number18, 1f);
				}
				number18 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1753);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number18, 1f);
				}
				break;
			}
			case 3:
			{
				int number19 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1767);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number19, 1f);
				}
				number19 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1768);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number19, 1f);
				}
				number19 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1769);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number19, 1f);
				}
				break;
			}
			case 4:
			{
				int number11 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1770);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number11, 1f);
				}
				number11 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1771);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number11, 1f);
				}
				break;
			}
			case 5:
			{
				int number8 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1772);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number8, 1f);
				}
				number8 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1773);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number8, 1f);
				}
				break;
			}
			case 6:
			{
				int number22 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1754);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number22, 1f);
				}
				number22 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1755);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number22, 1f);
				}
				number22 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1756);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number22, 1f);
				}
				break;
			}
			case 7:
			{
				int number10 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1757);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number10, 1f);
				}
				number10 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1758);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number10, 1f);
				}
				number10 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1759);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number10, 1f);
				}
				break;
			}
			case 8:
			{
				int number12 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1760);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number12, 1f);
				}
				number12 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1761);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number12, 1f);
				}
				number12 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1762);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number12, 1f);
				}
				break;
			}
			case 9:
			{
				int number20 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1763);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number20, 1f);
				}
				number20 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1764);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number20, 1f);
				}
				number20 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1765);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number20, 1f);
				}
				break;
			}
			case 10:
			{
				int number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1766);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number14, 1f);
				}
				number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1775);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number14, 1f);
				}
				number14 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1776);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number14, 1f);
				}
				break;
			}
			case 11:
			{
				int number7 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1777);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number7, 1f);
				}
				number7 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1778);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number7, 1f);
				}
				break;
			}
			case 12:
			{
				int number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1779);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number16, 1f);
				}
				number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1780);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number16, 1f);
				}
				number16 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1781);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number16, 1f);
				}
				break;
			}
			case 13:
			{
				int number13 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1819);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number13, 1f);
				}
				number13 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1820);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number13, 1f);
				}
				break;
			}
			case 14:
			{
				int number23 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1821);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number23, 1f);
				}
				number23 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1822);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number23, 1f);
				}
				number23 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1823);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number23, 1f);
				}
				break;
			}
			case 15:
			{
				int number21 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1824);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number21, 1f);
				}
				break;
			}
			case 16:
			{
				int number15 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1838);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number15, 1f);
				}
				number15 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1839);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number15, 1f);
				}
				number15 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1840);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number15, 1f);
				}
				break;
			}
			case 17:
			{
				int number9 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1841);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number9, 1f);
				}
				number9 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1842);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number9, 1f);
				}
				number9 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1843);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number9, 1f);
				}
				break;
			}
			case 18:
			{
				int number5 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1851);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number5, 1f);
				}
				number5 = Item.NewItem((int)position.X, (int)position.Y, width, height, 1852);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number5, 1f);
				}
				break;
			}
			}
		}

		public void UpdateDyes(int plr)
		{
			cHead = 0;
			cBody = 0;
			cLegs = 0;
			cHandOn = 0;
			cHandOff = 0;
			cBack = 0;
			cFront = 0;
			cShoe = 0;
			cWaist = 0;
			cShield = 0;
			cNeck = 0;
			cFace = 0;
			cBalloon = 0;
			cWings = 0;
			cCarpet = 0;
			cGrapple = (cMount = (cMinecart = (cPet = (cLight = (cYorai = 0)))));
			if (dye[0] != null)
			{
				cHead = dye[0].dye;
			}
			if (dye[1] != null)
			{
				cBody = dye[1].dye;
			}
			if (dye[2] != null)
			{
				cLegs = dye[2].dye;
			}
			if (wearsRobe)
			{
				cLegs = cBody;
			}
			if (miscDyes[0] != null)
			{
				cPet = miscDyes[0].dye;
			}
			if (miscDyes[1] != null)
			{
				cLight = miscDyes[1].dye;
			}
			if (miscDyes[2] != null)
			{
				cMinecart = miscDyes[2].dye;
			}
			if (miscDyes[3] != null)
			{
				cMount = miscDyes[3].dye;
			}
			if (miscDyes[4] != null)
			{
				cGrapple = miscDyes[4].dye;
			}
			for (int i = 0; i < 20; i++)
			{
				int num = i % 10;
				if (dye[num] != null && armor[i].type > 0 && armor[i].stack > 0 && (i / 10 >= 1 || !hideVisual[num] || armor[i].wingSlot > 0 || armor[i].type == 934))
				{
					if (armor[i].handOnSlot > 0 && armor[i].handOnSlot < 19)
					{
						cHandOn = dye[num].dye;
					}
					if (armor[i].handOffSlot > 0 && armor[i].handOffSlot < 12)
					{
						cHandOff = dye[num].dye;
					}
					if (armor[i].backSlot > 0 && armor[i].backSlot < 10)
					{
						cBack = dye[num].dye;
					}
					if (armor[i].frontSlot > 0 && armor[i].frontSlot < 5)
					{
						cFront = dye[num].dye;
					}
					if (armor[i].shoeSlot > 0 && armor[i].shoeSlot < 18)
					{
						cShoe = dye[num].dye;
					}
					if (armor[i].waistSlot > 0 && armor[i].waistSlot < 12)
					{
						cWaist = dye[num].dye;
					}
					if (armor[i].shieldSlot > 0 && armor[i].shieldSlot < 6)
					{
						cShield = dye[num].dye;
					}
					if (armor[i].neckSlot > 0 && armor[i].neckSlot < 9)
					{
						cNeck = dye[num].dye;
					}
					if (armor[i].faceSlot > 0 && armor[i].faceSlot < 9)
					{
						cFace = dye[num].dye;
					}
					if (armor[i].balloonSlot > 0 && armor[i].balloonSlot < 16)
					{
						cBalloon = dye[num].dye;
					}
					if (armor[i].wingSlot > 0 && armor[i].wingSlot < 37)
					{
						cWings = dye[num].dye;
					}
					if (armor[i].type == 934)
					{
						cCarpet = dye[num].dye;
					}
				}
			}
			cYorai = cPet;
		}

		public int ArmorSetDye()
		{
			switch (Main.rand.Next(3))
			{
			case 0:
				return cHead;
			case 1:
				return cBody;
			case 2:
				return cLegs;
			default:
				return cBody;
			}
		}

		public void UpdateBuffs(int i)
		{
			if (soulDrain > 0 && whoAmI == Main.myPlayer)
			{
				AddBuff(151, 2);
			}
			for (int j = 0; j < 1000; j++)
			{
				if (Main.projectile[j].active && Main.projectile[j].owner == i)
				{
					ownedProjectileCounts[Main.projectile[j].type]++;
				}
			}
			for (int k = 0; k < 22; k++)
			{
				if (buffType[k] <= 0 || buffTime[k] <= 0)
				{
					continue;
				}
				if (whoAmI == Main.myPlayer && buffType[k] != 28)
				{
					buffTime[k]--;
				}
				if (buffType[k] == 1)
				{
					lavaImmune = true;
					fireWalk = true;
					buffImmune[24] = true;
				}
				else if (buffType[k] == 158)
				{
					manaRegen += 2;
				}
				else if (buffType[k] == 159 && inventory[selectedItem].melee)
				{
					armorPenetration = 4;
				}
				else if (buffType[k] == 2)
				{
					lifeRegen += 4;
				}
				else if (buffType[k] == 3)
				{
					moveSpeed += 0.25f;
				}
				else if (buffType[k] == 4)
				{
					gills = true;
				}
				else if (buffType[k] == 5)
				{
					statDefense += 8;
				}
				else if (buffType[k] == 6)
				{
					manaRegenBuff = true;
				}
				else if (buffType[k] == 7)
				{
					magicDamage += 0.2f;
				}
				else if (buffType[k] == 8)
				{
					slowFall = true;
				}
				else if (buffType[k] == 9)
				{
					findTreasure = true;
				}
				else if (buffType[k] == 10)
				{
					invis = true;
				}
				else if (buffType[k] == 11)
				{
					Lighting.AddLight((int)(position.X + (float)(width / 2)) / 16, (int)(position.Y + (float)(height / 2)) / 16, 0.8f, 0.95f, 1f);
				}
				else if (buffType[k] == 12)
				{
					nightVision = true;
				}
				else if (buffType[k] == 13)
				{
					enemySpawns = true;
				}
				else if (buffType[k] == 14)
				{
					if (thorns < 1f)
					{
						thorns = 0.333333343f;
					}
				}
				else if (buffType[k] == 15)
				{
					waterWalk = true;
				}
				else if (buffType[k] == 16)
				{
					archery = true;
				}
				else if (buffType[k] == 17)
				{
					detectCreature = true;
				}
				else if (buffType[k] == 18)
				{
					gravControl = true;
				}
				else if (buffType[k] == 30)
				{
					bleed = true;
				}
				else if (buffType[k] == 31)
				{
					confused = true;
				}
				else if (buffType[k] == 32)
				{
					slow = true;
				}
				else if (buffType[k] == 35)
				{
					silence = true;
				}
				else if (buffType[k] == 160)
				{
					dazed = true;
				}
				else if (buffType[k] == 46)
				{
					chilled = true;
				}
				else if (buffType[k] == 47)
				{
					frozen = true;
				}
				else if (buffType[k] == 156)
				{
					stoned = true;
				}
				else if (buffType[k] == 69)
				{
					ichor = true;
					statDefense -= 20;
				}
				else if (buffType[k] == 36)
				{
					brokenArmor = true;
				}
				else if (buffType[k] == 48)
				{
					honey = true;
				}
				else if (buffType[k] == 59)
				{
					shadowDodge = true;
				}
				else if (buffType[k] == 93)
				{
					ammoBox = true;
				}
				else if (buffType[k] == 58)
				{
					palladiumRegen = true;
				}
				else if (buffType[k] == 88)
				{
					chaosState = true;
				}
				else if (buffType[k] == 63)
				{
					moveSpeed += 1f;
				}
				else if (buffType[k] == 104)
				{
					pickSpeed -= 0.25f;
				}
				else if (buffType[k] == 105)
				{
					lifeMagnet = true;
				}
				else if (buffType[k] == 106)
				{
					calmed = true;
				}
				else if (buffType[k] == 121)
				{
					fishingSkill += 15;
				}
				else if (buffType[k] == 122)
				{
					sonarPotion = true;
				}
				else if (buffType[k] == 123)
				{
					cratePotion = true;
				}
				else if (buffType[k] == 107)
				{
					tileSpeed += 0.25f;
					wallSpeed += 0.25f;
					blockRange++;
				}
				else if (buffType[k] == 108)
				{
					kbBuff = true;
				}
				else if (buffType[k] == 109)
				{
					ignoreWater = true;
					accFlipper = true;
				}
				else if (buffType[k] == 110)
				{
					maxMinions++;
				}
				else if (buffType[k] == 150)
				{
					maxMinions++;
				}
				else if (buffType[k] == 111)
				{
					dangerSense = true;
				}
				else if (buffType[k] == 112)
				{
					ammoPotion = true;
				}
				else if (buffType[k] == 113)
				{
					lifeForce = true;
					statLifeMax2 += statLifeMax / 5 / 20 * 20;
				}
				else if (buffType[k] == 114)
				{
					endurance += 0.1f;
				}
				else if (buffType[k] == 115)
				{
					meleeCrit += 10;
					rangedCrit += 10;
					magicCrit += 10;
					thrownCrit += 10;
				}
				else if (buffType[k] == 116)
				{
					inferno = true;
					Lighting.AddLight((int)(base.Center.X / 16f), (int)(base.Center.Y / 16f), 0.65f, 0.4f, 0.1f);
					int num = 24;
					float num2 = 200f;
					bool flag = infernoCounter % 60 == 0;
					int num3 = 10;
					if (whoAmI != Main.myPlayer)
					{
						continue;
					}
					for (int l = 0; l < 200; l++)
					{
						NPC nPC = Main.npc[l];
						if (!nPC.active || nPC.friendly || nPC.damage <= 0 || nPC.dontTakeDamage || nPC.buffImmune[num] || !(Vector2.Distance(base.Center, nPC.Center) <= num2))
						{
							continue;
						}
						if (nPC.HasBuff(num) == -1)
						{
							nPC.AddBuff(num, 120);
						}
						if (flag)
						{
							nPC.StrikeNPC(num3, 0f, 0);
							if (Main.netMode != 0)
							{
								NetMessage.SendData(28, -1, -1, "", l, num3);
							}
						}
					}
					if (!hostile)
					{
						continue;
					}
					for (int m = 0; m < 16; m++)
					{
						Player player = Main.player[m];
						if (player == this || !player.active || player.dead || !player.hostile || player.buffImmune[num] || (player.team == team && player.team != 0) || !(Vector2.Distance(base.Center, player.Center) <= num2))
						{
							continue;
						}
						if (player.HasBuff(num) == -1)
						{
							player.AddBuff(num, 120);
						}
						if (flag)
						{
							player.Hurt(num3, 0, true, false, "");
							if (Main.netMode != 0)
							{
								NetMessage.SendData(26, -1, -1, Lang.deathMsg(name, whoAmI), m, 0f, num3, 1f);
							}
						}
					}
				}
				else if (buffType[k] == 117)
				{
					thrownDamage += 0.1f;
					meleeDamage += 0.1f;
					rangedDamage += 0.1f;
					magicDamage += 0.1f;
					minionDamage += 0.1f;
				}
				else if (buffType[k] == 119)
				{
					loveStruck = true;
				}
				else if (buffType[k] == 120)
				{
					stinky = true;
				}
				else if (buffType[k] == 124)
				{
					resistCold = true;
				}
				else if (buffType[k] == 165)
				{
					lifeRegen += 6;
					statDefense += 8;
					dryadWard = true;
					if (thorns < 1f)
					{
						thorns += 0.2f;
					}
				}
				else if (buffType[k] == 144)
				{
					electrified = true;
					Lighting.AddLight((int)base.Center.X / 16, (int)base.Center.Y / 16, 0.3f, 0.8f, 1.1f);
				}
				else if (buffType[k] == 94)
				{
					manaSick = true;
					manaSickReduction = manaSickLessDmg * ((float)buffTime[k] / (float)manaSickTime);
				}
				else if (buffType[k] >= 95 && buffType[k] <= 97)
				{
					buffTime[k] = 5;
					int num4 = (byte)(1 + buffType[k] - 95);
					if (beetleOrbs > 0 && beetleOrbs != num4)
					{
						if (beetleOrbs > num4)
						{
							DelBuff(k);
							k--;
						}
						else
						{
							for (int n = 0; n < 22; n++)
							{
								if (buffType[n] >= 95 && buffType[n] <= 95 + num4 - 1)
								{
									DelBuff(n);
									n--;
								}
							}
						}
					}
					beetleOrbs = num4;
					if (!beetleDefense)
					{
						beetleOrbs = 0;
						DelBuff(k);
						k--;
					}
					else
					{
						beetleBuff = true;
					}
				}
				else if (buffType[k] >= 170 && buffType[k] <= 172)
				{
					buffTime[k] = 5;
					int num5 = (byte)(1 + buffType[k] - 170);
					if (solarShields > 0 && solarShields != num5)
					{
						if (solarShields > num5)
						{
							DelBuff(k);
							k--;
						}
						else
						{
							for (int num6 = 0; num6 < 22; num6++)
							{
								if (buffType[num6] >= 170 && buffType[num6] <= 170 + num5 - 1)
								{
									DelBuff(num6);
									num6--;
								}
							}
						}
					}
					solarShields = num5;
					if (!setSolar)
					{
						solarShields = 0;
						DelBuff(k);
						k--;
					}
				}
				else if (buffType[k] >= 98 && buffType[k] <= 100)
				{
					int num7 = (byte)(1 + buffType[k] - 98);
					if (beetleOrbs > 0 && beetleOrbs != num7)
					{
						if (beetleOrbs > num7)
						{
							DelBuff(k);
							k--;
						}
						else
						{
							for (int num8 = 0; num8 < 22; num8++)
							{
								if (buffType[num8] >= 98 && buffType[num8] <= 98 + num7 - 1)
								{
									DelBuff(num8);
									num8--;
								}
							}
						}
					}
					beetleOrbs = num7;
					meleeDamage += 0.1f * (float)beetleOrbs;
					meleeSpeed += 0.1f * (float)beetleOrbs;
					if (!beetleOffense)
					{
						beetleOrbs = 0;
						DelBuff(k);
						k--;
					}
					else
					{
						beetleBuff = true;
					}
				}
				else if (buffType[k] >= 176 && buffType[k] <= 178)
				{
					int num9 = nebulaLevelMana;
					int num10 = (byte)(1 + buffType[k] - 176);
					if (num9 > 0 && num9 != num10)
					{
						if (num9 > num10)
						{
							DelBuff(k);
							k--;
						}
						else
						{
							for (int num11 = 0; num11 < 22; num11++)
							{
								if (buffType[num11] >= 176 && buffType[num11] <= 178 + num10 - 1)
								{
									DelBuff(num11);
									num11--;
								}
							}
						}
					}
					nebulaLevelMana = num10;
					if (buffTime[k] == 2 && nebulaLevelMana > 1)
					{
						nebulaLevelMana--;
						buffType[k]--;
						buffTime[k] = 480;
					}
				}
				else if (buffType[k] >= 173 && buffType[k] <= 175)
				{
					int num12 = nebulaLevelLife;
					int num13 = (byte)(1 + buffType[k] - 173);
					if (num12 > 0 && num12 != num13)
					{
						if (num12 > num13)
						{
							DelBuff(k);
							k--;
						}
						else
						{
							for (int num14 = 0; num14 < 22; num14++)
							{
								if (buffType[num14] >= 173 && buffType[num14] <= 175 + num13 - 1)
								{
									DelBuff(num14);
									num14--;
								}
							}
						}
					}
					nebulaLevelLife = num13;
					if (buffTime[k] == 2 && nebulaLevelLife > 1)
					{
						nebulaLevelLife--;
						buffType[k]--;
						buffTime[k] = 480;
					}
					lifeRegen += 10 * nebulaLevelLife;
				}
				else if (buffType[k] >= 179 && buffType[k] <= 181)
				{
					int num15 = nebulaLevelDamage;
					int num16 = (byte)(1 + buffType[k] - 179);
					if (num15 > 0 && num15 != num16)
					{
						if (num15 > num16)
						{
							DelBuff(k);
							k--;
						}
						else
						{
							for (int num17 = 0; num17 < 22; num17++)
							{
								if (buffType[num17] >= 179 && buffType[num17] <= 181 + num16 - 1)
								{
									DelBuff(num17);
									num17--;
								}
							}
						}
					}
					nebulaLevelDamage = num16;
					if (buffTime[k] == 2 && nebulaLevelDamage > 1)
					{
						nebulaLevelDamage--;
						buffType[k]--;
						buffTime[k] = 480;
					}
					float num18 = 0.15f * (float)nebulaLevelDamage;
					meleeDamage += num18;
					rangedDamage += num18;
					magicDamage += num18;
					minionDamage += num18;
					thrownDamage += num18;
				}
				else if (buffType[k] == 62)
				{
					if ((double)statLife <= (double)statLifeMax2 * 0.5)
					{
						Lighting.AddLight((int)(base.Center.X / 16f), (int)(base.Center.Y / 16f), 0.1f, 0.2f, 0.45f);
						iceBarrier = true;
						endurance += 0.25f;
						iceBarrierFrameCounter++;
						if (iceBarrierFrameCounter > 2)
						{
							iceBarrierFrameCounter = 0;
							iceBarrierFrame++;
							if (iceBarrierFrame >= 12)
							{
								iceBarrierFrame = 0;
							}
						}
					}
					else
					{
						DelBuff(k);
						k--;
					}
				}
				else if (buffType[k] == 49)
				{
					for (int num19 = 191; num19 <= 194; num19++)
					{
						if (ownedProjectileCounts[num19] > 0)
						{
							pygmy = true;
						}
					}
					if (!pygmy)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 83)
				{
					if (ownedProjectileCounts[317] > 0)
					{
						raven = true;
					}
					if (!raven)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 64)
				{
					if (ownedProjectileCounts[266] > 0)
					{
						slime = true;
					}
					if (!slime)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 125)
				{
					if (ownedProjectileCounts[373] > 0)
					{
						hornetMinion = true;
					}
					if (!hornetMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 126)
				{
					if (ownedProjectileCounts[375] > 0)
					{
						impMinion = true;
					}
					if (!impMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 133)
				{
					if (ownedProjectileCounts[390] > 0 || ownedProjectileCounts[391] > 0 || ownedProjectileCounts[392] > 0)
					{
						spiderMinion = true;
					}
					if (!spiderMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 134)
				{
					if (ownedProjectileCounts[387] > 0 || ownedProjectileCounts[388] > 0)
					{
						twinsMinion = true;
					}
					if (!twinsMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 135)
				{
					if (ownedProjectileCounts[393] > 0 || ownedProjectileCounts[394] > 0 || ownedProjectileCounts[395] > 0)
					{
						pirateMinion = true;
					}
					if (!pirateMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 139)
				{
					if (ownedProjectileCounts[407] > 0)
					{
						sharknadoMinion = true;
					}
					if (!sharknadoMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 140)
				{
					if (ownedProjectileCounts[423] > 0)
					{
						UFOMinion = true;
					}
					if (!UFOMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 182)
				{
					if (ownedProjectileCounts[613] > 0)
					{
						stardustMinion = true;
					}
					if (!stardustMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 187)
				{
					if (ownedProjectileCounts[623] > 0)
					{
						stardustGuardian = true;
					}
					if (!stardustGuardian)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 188)
				{
					if (ownedProjectileCounts[625] > 0)
					{
						stardustDragon = true;
					}
					if (!stardustDragon)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 161)
				{
					if (ownedProjectileCounts[533] > 0)
					{
						DeadlySphereMinion = true;
					}
					if (!DeadlySphereMinion)
					{
						DelBuff(k);
						k--;
					}
					else
					{
						buffTime[k] = 18000;
					}
				}
				else if (buffType[k] == 90)
				{
					mount.SetMount(0, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 128)
				{
					mount.SetMount(1, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 129)
				{
					mount.SetMount(2, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 130)
				{
					mount.SetMount(3, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 118)
				{
					mount.SetMount(6, this, true);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 138)
				{
					mount.SetMount(6, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 167)
				{
					mount.SetMount(11, this, true);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 166)
				{
					mount.SetMount(11, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 184)
				{
					mount.SetMount(13, this, true);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 185)
				{
					mount.SetMount(13, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 131)
				{
					ignoreWater = true;
					accFlipper = true;
					mount.SetMount(4, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 132)
				{
					mount.SetMount(5, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 168)
				{
					ignoreWater = true;
					accFlipper = true;
					mount.SetMount(12, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 141)
				{
					mount.SetMount(7, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 142)
				{
					mount.SetMount(8, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 143)
				{
					mount.SetMount(9, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 162)
				{
					mount.SetMount(10, this);
					buffTime[k] = 10;
				}
				else if (buffType[k] == 37)
				{
					if (Main.wof >= 0 && Main.npc[Main.wof].type == 113)
					{
						gross = true;
						buffTime[k] = 10;
					}
					else
					{
						DelBuff(k);
						k--;
					}
				}
				else if (buffType[k] == 38)
				{
					buffTime[k] = 10;
					tongued = true;
				}
				else if (buffType[k] == 146)
				{
					moveSpeed += 0.1f;
					moveSpeed *= 1.1f;
					sunflower = true;
				}
				else if (buffType[k] == 19)
				{
					buffTime[k] = 18000;
					lightOrb = true;
					bool flag2 = true;
					if (ownedProjectileCounts[18] > 0)
					{
						flag2 = false;
					}
					if (flag2 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 18, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 155)
				{
					buffTime[k] = 18000;
					crimsonHeart = true;
					bool flag3 = true;
					if (ownedProjectileCounts[500] > 0)
					{
						flag3 = false;
					}
					if (flag3 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 500, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 190)
				{
					buffTime[k] = 18000;
					suspiciouslookingTentacle = true;
					bool flag4 = true;
					if (ownedProjectileCounts[650] > 0)
					{
						flag4 = false;
					}
					if (flag4 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 650, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 27 || buffType[k] == 101 || buffType[k] == 102)
				{
					buffTime[k] = 18000;
					bool flag5 = true;
					int num20 = 72;
					if (buffType[k] == 27)
					{
						blueFairy = true;
					}
					if (buffType[k] == 101)
					{
						num20 = 86;
						redFairy = true;
					}
					if (buffType[k] == 102)
					{
						num20 = 87;
						greenFairy = true;
					}
					if (head == 45 && body == 26 && legs == 25)
					{
						num20 = 72;
					}
					if (ownedProjectileCounts[num20] > 0)
					{
						flag5 = false;
					}
					if (flag5 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, num20, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 40)
				{
					buffTime[k] = 18000;
					bunny = true;
					bool flag6 = true;
					if (ownedProjectileCounts[111] > 0)
					{
						flag6 = false;
					}
					if (flag6 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 111, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 148)
				{
					rabid = true;
					if (Main.rand.Next(1200) == 0)
					{
						int num21 = Main.rand.Next(6);
						float num22 = (float)Main.rand.Next(60, 100) * 0.01f;
						switch (num21)
						{
						case 0:
							AddBuff(22, (int)(60f * num22 * 3f));
							break;
						case 1:
							AddBuff(23, (int)(60f * num22 * 0.75f));
							break;
						case 2:
							AddBuff(31, (int)(60f * num22 * 1.5f));
							break;
						case 3:
							AddBuff(32, (int)(60f * num22 * 3.5f));
							break;
						case 4:
							AddBuff(33, (int)(60f * num22 * 5f));
							break;
						case 5:
							AddBuff(35, (int)(60f * num22 * 1f));
							break;
						}
					}
					meleeDamage += 0.2f;
					magicDamage += 0.2f;
					rangedDamage += 0.2f;
					thrownDamage += 0.2f;
					minionDamage += 0.2f;
				}
				else if (buffType[k] == 41)
				{
					buffTime[k] = 18000;
					penguin = true;
					bool flag7 = true;
					if (ownedProjectileCounts[112] > 0)
					{
						flag7 = false;
					}
					if (flag7 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 112, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 152)
				{
					buffTime[k] = 18000;
					magicLantern = true;
					if (ownedProjectileCounts[492] == 0 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 492, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 91)
				{
					buffTime[k] = 18000;
					puppy = true;
					bool flag8 = true;
					if (ownedProjectileCounts[334] > 0)
					{
						flag8 = false;
					}
					if (flag8 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 334, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 92)
				{
					buffTime[k] = 18000;
					grinch = true;
					bool flag9 = true;
					if (ownedProjectileCounts[353] > 0)
					{
						flag9 = false;
					}
					if (flag9 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 353, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 84)
				{
					buffTime[k] = 18000;
					blackCat = true;
					bool flag10 = true;
					if (ownedProjectileCounts[319] > 0)
					{
						flag10 = false;
					}
					if (flag10 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 319, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 61)
				{
					buffTime[k] = 18000;
					dino = true;
					bool flag11 = true;
					if (ownedProjectileCounts[236] > 0)
					{
						flag11 = false;
					}
					if (flag11 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 236, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 154)
				{
					buffTime[k] = 18000;
					babyFaceMonster = true;
					bool flag12 = true;
					if (ownedProjectileCounts[499] > 0)
					{
						flag12 = false;
					}
					if (flag12 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 499, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 65)
				{
					buffTime[k] = 18000;
					eyeSpring = true;
					bool flag13 = true;
					if (ownedProjectileCounts[268] > 0)
					{
						flag13 = false;
					}
					if (flag13 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 268, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 66)
				{
					buffTime[k] = 18000;
					snowman = true;
					bool flag14 = true;
					if (ownedProjectileCounts[269] > 0)
					{
						flag14 = false;
					}
					if (flag14 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 269, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 42)
				{
					buffTime[k] = 18000;
					turtle = true;
					bool flag15 = true;
					if (ownedProjectileCounts[127] > 0)
					{
						flag15 = false;
					}
					if (flag15 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 127, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 45)
				{
					buffTime[k] = 18000;
					eater = true;
					bool flag16 = true;
					if (ownedProjectileCounts[175] > 0)
					{
						flag16 = false;
					}
					if (flag16 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 175, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 50)
				{
					buffTime[k] = 18000;
					skeletron = true;
					bool flag17 = true;
					if (ownedProjectileCounts[197] > 0)
					{
						flag17 = false;
					}
					if (flag17 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 197, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 51)
				{
					buffTime[k] = 18000;
					hornet = true;
					bool flag18 = true;
					if (ownedProjectileCounts[198] > 0)
					{
						flag18 = false;
					}
					if (flag18 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 198, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 52)
				{
					buffTime[k] = 18000;
					tiki = true;
					bool flag19 = true;
					if (ownedProjectileCounts[199] > 0)
					{
						flag19 = false;
					}
					if (flag19 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 199, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 53)
				{
					buffTime[k] = 18000;
					lizard = true;
					bool flag20 = true;
					if (ownedProjectileCounts[200] > 0)
					{
						flag20 = false;
					}
					if (flag20 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 200, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 54)
				{
					buffTime[k] = 18000;
					parrot = true;
					bool flag21 = true;
					if (ownedProjectileCounts[208] > 0)
					{
						flag21 = false;
					}
					if (flag21 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 208, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 55)
				{
					buffTime[k] = 18000;
					truffle = true;
					bool flag22 = true;
					if (ownedProjectileCounts[209] > 0)
					{
						flag22 = false;
					}
					if (flag22 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 209, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 56)
				{
					buffTime[k] = 18000;
					sapling = true;
					bool flag23 = true;
					if (ownedProjectileCounts[210] > 0)
					{
						flag23 = false;
					}
					if (flag23 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 210, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 85)
				{
					buffTime[k] = 18000;
					cSapling = true;
					bool flag24 = true;
					if (ownedProjectileCounts[324] > 0)
					{
						flag24 = false;
					}
					if (flag24 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 324, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 81)
				{
					buffTime[k] = 18000;
					spider = true;
					bool flag25 = true;
					if (ownedProjectileCounts[313] > 0)
					{
						flag25 = false;
					}
					if (flag25 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 313, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 82)
				{
					buffTime[k] = 18000;
					squashling = true;
					bool flag26 = true;
					if (ownedProjectileCounts[314] > 0)
					{
						flag26 = false;
					}
					if (flag26 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 314, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 57)
				{
					buffTime[k] = 18000;
					wisp = true;
					bool flag27 = true;
					if (ownedProjectileCounts[211] > 0)
					{
						flag27 = false;
					}
					if (flag27 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 211, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 60)
				{
					buffTime[k] = 18000;
					crystalLeaf = true;
					bool flag28 = true;
					for (int num23 = 0; num23 < 1000; num23++)
					{
						if (Main.projectile[num23].active && Main.projectile[num23].owner == whoAmI && Main.projectile[num23].type == 226)
						{
							if (!flag28)
							{
								Main.projectile[num23].Kill();
							}
							flag28 = false;
						}
					}
					if (flag28 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 226, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 127)
				{
					buffTime[k] = 18000;
					zephyrfish = true;
					bool flag29 = true;
					if (ownedProjectileCounts[380] > 0)
					{
						flag29 = false;
					}
					if (flag29 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 380, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 136)
				{
					buffTime[k] = 18000;
					miniMinotaur = true;
					bool flag30 = true;
					if (ownedProjectileCounts[398] > 0)
					{
						flag30 = false;
					}
					if (flag30 && whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), 0f, 0f, 398, 0, 0f, whoAmI);
					}
				}
				else if (buffType[k] == 70)
				{
					venom = true;
				}
				else if (buffType[k] == 20)
				{
					poisoned = true;
				}
				else if (buffType[k] == 21)
				{
					potionDelay = buffTime[k];
				}
				else if (buffType[k] == 22)
				{
					blind = true;
				}
				else if (buffType[k] == 80)
				{
					blackout = true;
				}
				else if (buffType[k] == 23)
				{
					noItems = true;
				}
				else if (buffType[k] == 24)
				{
					onFire = true;
				}
				else if (buffType[k] == 103)
				{
					dripping = true;
				}
				else if (buffType[k] == 137)
				{
					drippingSlime = true;
				}
				else if (buffType[k] == 67)
				{
					burned = true;
				}
				else if (buffType[k] == 68)
				{
					suffocating = true;
				}
				else if (buffType[k] == 39)
				{
					onFire2 = true;
				}
				else if (buffType[k] == 44)
				{
					onFrostBurn = true;
				}
				else if (buffType[k] == 163)
				{
					headcovered = true;
					bleed = true;
				}
				else if (buffType[k] == 164)
				{
					vortexDebuff = true;
				}
				else if (buffType[k] == 145)
				{
					moonLeech = true;
				}
				else if (buffType[k] == 149)
				{
					webbed = true;
					if (velocity.Y != 0f)
					{
						velocity = new Vector2(0f, 1E-06f);
					}
					else
					{
						velocity = Vector2.Zero;
					}
					jumpHeight = 0;
					gravity = 0f;
					moveSpeed = 0f;
					dash = 0;
					noKnockback = true;
					grappling[0] = -1;
					grapCount = 0;
					for (int num24 = 0; num24 < 1000; num24++)
					{
						if (Main.projectile[num24].active && Main.projectile[num24].owner == whoAmI && Main.projectile[num24].aiStyle == 7)
						{
							Main.projectile[num24].Kill();
						}
					}
				}
				else if (buffType[k] == 43)
				{
					paladinBuff = true;
				}
				else if (buffType[k] == 29)
				{
					magicCrit += 2;
					magicDamage += 0.05f;
					statManaMax2 += 20;
					manaCost -= 0.02f;
				}
				else if (buffType[k] == 28)
				{
					if (!Main.dayTime && wolfAcc && !merman)
					{
						lifeRegen++;
						wereWolf = true;
						meleeCrit += 2;
						meleeDamage += 0.051f;
						meleeSpeed += 0.051f;
						statDefense += 3;
						moveSpeed += 0.05f;
					}
					else
					{
						DelBuff(k);
						k--;
					}
				}
				else if (buffType[k] == 33)
				{
					meleeDamage -= 0.051f;
					meleeSpeed -= 0.051f;
					statDefense -= 4;
					moveSpeed -= 0.1f;
				}
				else if (buffType[k] == 25)
				{
					statDefense -= 4;
					meleeCrit += 2;
					meleeDamage += 0.1f;
					meleeSpeed += 0.1f;
				}
				else if (buffType[k] == 26)
				{
					wellFed = true;
					statDefense += 2;
					meleeCrit += 2;
					meleeDamage += 0.05f;
					meleeSpeed += 0.05f;
					magicCrit += 2;
					magicDamage += 0.05f;
					rangedCrit += 2;
					rangedDamage += 0.05f;
					thrownCrit += 2;
					thrownDamage += 0.05f;
					minionDamage += 0.05f;
					minionKB += 0.5f;
					moveSpeed += 0.2f;
				}
				else if (buffType[k] == 71)
				{
					meleeEnchant = 1;
				}
				else if (buffType[k] == 73)
				{
					meleeEnchant = 2;
				}
				else if (buffType[k] == 74)
				{
					meleeEnchant = 3;
				}
				else if (buffType[k] == 75)
				{
					meleeEnchant = 4;
				}
				else if (buffType[k] == 76)
				{
					meleeEnchant = 5;
				}
				else if (buffType[k] == 77)
				{
					meleeEnchant = 6;
				}
				else if (buffType[k] == 78)
				{
					meleeEnchant = 7;
				}
				else if (buffType[k] == 79)
				{
					meleeEnchant = 8;
				}
			}
		}

		public void Counterweight(Vector2 hitPos, int dmg, float kb)
		{
			if (!yoyoGlove && counterWeight <= 0)
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == whoAmI)
				{
					if (Main.projectile[i].counterweight)
					{
						num3++;
					}
					else if (Main.projectile[i].aiStyle == 99)
					{
						num2++;
						num = i;
					}
				}
			}
			if (yoyoGlove && num2 < 2)
			{
				if (num >= 0)
				{
					Vector2 vector = hitPos - base.Center;
					vector.Normalize();
					vector *= 16f;
					Projectile.NewProjectile(base.Center.X, base.Center.Y, vector.X, vector.Y, Main.projectile[num].type, Main.projectile[num].damage, Main.projectile[num].knockBack, whoAmI, 1f);
				}
			}
			else if (num3 < num2)
			{
				Vector2 vector2 = hitPos - base.Center;
				vector2.Normalize();
				vector2 *= 16f;
				float knockBack = (kb + 6f) / 2f;
				if (num3 > 0)
				{
					Projectile.NewProjectile(base.Center.X, base.Center.Y, vector2.X, vector2.Y, counterWeight, (int)((double)dmg * 0.8), knockBack, whoAmI, 1f);
				}
				else
				{
					Projectile.NewProjectile(base.Center.X, base.Center.Y, vector2.X, vector2.Y, counterWeight, (int)((double)dmg * 0.8), knockBack, whoAmI);
				}
			}
		}

		public int beeType()
		{
			if (strongBees && Main.rand.Next(2) == 0)
			{
				makeStrongBee = true;
				return 566;
			}
			makeStrongBee = false;
			return 181;
		}

		public int beeDamage(int dmg)
		{
			if (makeStrongBee)
			{
				return dmg + Main.rand.Next(1, 4);
			}
			return dmg + Main.rand.Next(2);
		}

		public float beeKB(float KB)
		{
			if (makeStrongBee)
			{
				return 0.5f + KB * 1.1f;
			}
			return KB;
		}

		public void Yoraiz0rEye()
		{
			int num = 0;
			num += bodyFrame.Y / 56;
			if (num >= Main.OffsetsPlayerHeadgear.Length)
			{
				num = 0;
			}
			Vector2 vector = new Vector2(3 * direction - ((direction == 1) ? 1 : 0), -11.5f * gravDir) + Vector2.UnitY * gfxOffY + base.Size / 2f + Main.OffsetsPlayerHeadgear[num];
			Vector2 vector2 = new Vector2(3 * shadowDirection[1] - ((direction == 1) ? 1 : 0), -11.5f * gravDir) + base.Size / 2f + Main.OffsetsPlayerHeadgear[num];
			Vector2 value = Vector2.Zero;
			if (mount.Active && mount.Cart)
			{
				int num2 = Math.Sign(velocity.X);
				if (num2 == 0)
				{
					num2 = direction;
				}
				value = new Vector2(MathHelper.Lerp(0f, -8f, fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(fullRotation / ((float)Math.PI / 4f)))).RotatedBy(fullRotation);
				if (num2 == Math.Sign(fullRotation))
				{
					value *= MathHelper.Lerp(1f, 0.6f, Math.Abs(fullRotation / ((float)Math.PI / 4f)));
				}
			}
			if (fullRotation != 0f)
			{
				vector = vector.RotatedBy(fullRotation, fullRotationOrigin);
				vector2 = vector2.RotatedBy(fullRotation, fullRotationOrigin);
			}
			Vector2 vector3 = position + vector + value;
			Vector2 vector4 = oldPosition + vector2 + value;
			float num3 = 1f;
			switch (yoraiz0rEye % 10)
			{
			case 1:
				return;
			case 2:
				num3 = 0.5f;
				break;
			case 3:
				num3 = 0.625f;
				break;
			case 4:
				num3 = 0.75f;
				break;
			case 5:
				num3 = 0.875f;
				break;
			case 6:
				num3 = 1f;
				break;
			case 7:
				num3 = 1.1f;
				break;
			}
			if (yoraiz0rEye < 7)
			{
				DelegateMethods.v3_1 = Main.hslToRgb(Main.rgbToHsl(eyeColor).X, 1f, 0.5f).ToVector3() * 0.5f * num3;
				if (velocity != Vector2.Zero)
				{
					Utils.PlotTileLine(base.Center, base.Center + velocity * 2f, 4f, DelegateMethods.CastLightOpen);
				}
				else
				{
					Utils.PlotTileLine(base.Left, base.Right, 4f, DelegateMethods.CastLightOpen);
				}
			}
			int num4 = (int)Vector2.Distance(vector3, vector4) / 3 + 1;
			if (Vector2.Distance(vector3, vector4) % 3f != 0f)
			{
				num4++;
			}
			for (float num5 = 1f; num5 <= (float)num4; num5 += 1f)
			{
				Dust dust = Main.dust[Dust.NewDust(base.Center, 0, 0, 182)];
				dust.position = Vector2.Lerp(vector4, vector3, num5 / (float)num4);
				dust.noGravity = true;
				dust.velocity = Vector2.Zero;
				dust.customData = this;
				dust.scale = num3;
				dust.shader = GameShaders.Armor.GetSecondaryShader(cYorai, this);
			}
		}

		public void UpdateEquips(int i)
		{
			for (int j = 0; j < 58; j++)
			{
				int type = inventory[j].type;
				if ((type == 15 || type == 707) && accWatch < 1)
				{
					accWatch = 1;
				}
				if ((type == 16 || type == 708) && accWatch < 2)
				{
					accWatch = 2;
				}
				if ((type == 17 || type == 709) && accWatch < 3)
				{
					accWatch = 3;
				}
				if (type == 393)
				{
					accCompass = 1;
				}
				if (type == 18)
				{
					accDepthMeter = 1;
				}
				if (type == 395 || type == 3123 || type == 3124)
				{
					accWatch = 3;
					accDepthMeter = 1;
					accCompass = 1;
				}
				if (type == 3120 || type == 3036 || type == 3123 || type == 3124)
				{
					accFishFinder = true;
				}
				if (type == 3037 || type == 3036 || type == 3123 || type == 3124)
				{
					accWeatherRadio = true;
				}
				if (type == 3096 || type == 3036 || type == 3123 || type == 3124)
				{
					accCalendar = true;
				}
				if (type == 3084 || type == 3122 || type == 3123 || type == 3124)
				{
					accThirdEye = true;
				}
				if (type == 3095 || type == 3122 || type == 3123 || type == 3124)
				{
					accJarOfSouls = true;
				}
				if (type == 3118 || type == 3122 || type == 3123 || type == 3124)
				{
					accCritterGuide = true;
				}
				if (type == 3099 || type == 3121 || type == 3123 || type == 3124)
				{
					accStopwatch = true;
				}
				if (type == 3102 || type == 3121 || type == 3123 || type == 3124)
				{
					accOreFinder = true;
				}
				if (type == 3119 || type == 3121 || type == 3123 || type == 3124)
				{
					accDreamCatcher = true;
				}
			}
			for (int k = 0; k < 8 + extraAccessorySlots; k++)
			{
				if (armor[k].expertOnly && !Main.expertMode)
				{
					continue;
				}
				int type2 = armor[k].type;
				if ((type2 == 15 || type2 == 707) && accWatch < 1)
				{
					accWatch = 1;
				}
				if ((type2 == 16 || type2 == 708) && accWatch < 2)
				{
					accWatch = 2;
				}
				if ((type2 == 17 || type2 == 709) && accWatch < 3)
				{
					accWatch = 3;
				}
				if (type2 == 393)
				{
					accCompass = 1;
				}
				if (type2 == 18)
				{
					accDepthMeter = 1;
				}
				if (type2 == 395 || type2 == 3123 || type2 == 3124)
				{
					accWatch = 3;
					accDepthMeter = 1;
					accCompass = 1;
				}
				if (type2 == 3120 || type2 == 3036 || type2 == 3123 || type2 == 3124)
				{
					accFishFinder = true;
				}
				if (type2 == 3037 || type2 == 3036 || type2 == 3123 || type2 == 3124)
				{
					accWeatherRadio = true;
				}
				if (type2 == 3096 || type2 == 3036 || type2 == 3123 || type2 == 3124)
				{
					accCalendar = true;
				}
				if (type2 == 3084 || type2 == 3122 || type2 == 3123 || type2 == 3124)
				{
					accThirdEye = true;
				}
				if (type2 == 3095 || type2 == 3122 || type2 == 3123 || type2 == 3124)
				{
					accJarOfSouls = true;
				}
				if (type2 == 3118 || type2 == 3122 || type2 == 3123 || type2 == 3124)
				{
					accCritterGuide = true;
				}
				if (type2 == 3099 || type2 == 3121 || type2 == 3123 || type2 == 3124)
				{
					accStopwatch = true;
				}
				if (type2 == 3102 || type2 == 3121 || type2 == 3123 || type2 == 3124)
				{
					accOreFinder = true;
				}
				if (type2 == 3119 || type2 == 3121 || type2 == 3123 || type2 == 3124)
				{
					accDreamCatcher = true;
				}
				if (armor[k].type == 3017 && whoAmI == Main.myPlayer && velocity.Y == 0f && grappling[0] == -1)
				{
					int num = (int)base.Center.X / 16;
					int num2 = (int)(position.Y + (float)height - 1f) / 16;
					if (Main.tile[num, num2] == null)
					{
						Main.tile[num, num2] = new Tile();
					}
					if (!Main.tile[num, num2].active() && Main.tile[num, num2].liquid == 0 && Main.tile[num, num2 + 1] != null && WorldGen.SolidTile(num, num2 + 1))
					{
						Main.tile[num, num2].frameY = 0;
						Main.tile[num, num2].slope(0);
						Main.tile[num, num2].halfBrick(false);
						if (Main.tile[num, num2 + 1].type == 2)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.tile[num, num2].active(true);
								Main.tile[num, num2].type = 3;
								Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(6, 11));
								while (Main.tile[num, num2].frameX == 144)
								{
									Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(6, 11));
								}
							}
							else
							{
								Main.tile[num, num2].active(true);
								Main.tile[num, num2].type = 73;
								Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(6, 21));
								while (Main.tile[num, num2].frameX == 144)
								{
									Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(6, 21));
								}
							}
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, num, num2, 1);
							}
						}
						else if (Main.tile[num, num2 + 1].type == 109)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.tile[num, num2].active(true);
								Main.tile[num, num2].type = 110;
								Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(4, 7));
								while (Main.tile[num, num2].frameX == 90)
								{
									Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(4, 7));
								}
							}
							else
							{
								Main.tile[num, num2].active(true);
								Main.tile[num, num2].type = 113;
								Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(2, 8));
								while (Main.tile[num, num2].frameX == 90)
								{
									Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(2, 8));
								}
							}
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, num, num2, 1);
							}
						}
						else if (Main.tile[num, num2 + 1].type == 60)
						{
							Main.tile[num, num2].active(true);
							Main.tile[num, num2].type = 74;
							Main.tile[num, num2].frameX = (short)(18 * Main.rand.Next(9, 17));
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, num, num2, 1);
							}
						}
					}
				}
				statDefense += armor[k].defense;
				lifeRegen += armor[k].lifeRegen;
				if (armor[k].type == 268)
				{
					accDivingHelm = true;
				}
				if (armor[k].type == 238)
				{
					magicDamage += 0.15f;
				}
				if (armor[k].type == 3212)
				{
					armorPenetration += 5;
				}
				if (armor[k].type == 2277)
				{
					magicDamage += 0.05f;
					meleeDamage += 0.05f;
					rangedDamage += 0.05f;
					thrownDamage += 0.05f;
					magicCrit += 5;
					rangedCrit += 5;
					meleeCrit += 5;
					thrownCrit += 5;
					meleeSpeed += 0.1f;
					moveSpeed += 0.1f;
				}
				if (armor[k].type == 2279)
				{
					magicDamage += 0.06f;
					magicCrit += 6;
					manaCost -= 0.1f;
				}
				if (armor[k].type == 3109)
				{
					nightVision = true;
				}
				if (armor[k].type == 256)
				{
					thrownVelocity += 0.15f;
				}
				if (armor[k].type == 257)
				{
					thrownDamage += 0.15f;
				}
				if (armor[k].type == 258)
				{
					thrownCrit += 10;
				}
				if (armor[k].type == 3374)
				{
					thrownVelocity += 0.2f;
				}
				if (armor[k].type == 3375)
				{
					thrownDamage += 0.2f;
				}
				if (armor[k].type == 3376)
				{
					thrownCrit += 15;
				}
				if (armor[k].type == 2275)
				{
					magicDamage += 0.07f;
					magicCrit += 7;
				}
				if (armor[k].type == 123 || armor[k].type == 124 || armor[k].type == 125)
				{
					magicDamage += 0.07f;
				}
				if (armor[k].type == 151 || armor[k].type == 152 || armor[k].type == 153 || armor[k].type == 959)
				{
					rangedDamage += 0.05f;
				}
				if (armor[k].type == 111 || armor[k].type == 228 || armor[k].type == 229 || armor[k].type == 230 || armor[k].type == 960 || armor[k].type == 961 || armor[k].type == 962)
				{
					statManaMax2 += 20;
				}
				if (armor[k].type == 228 || armor[k].type == 960)
				{
					statManaMax2 += 20;
				}
				if (armor[k].type == 228 || armor[k].type == 229 || armor[k].type == 230 || armor[k].type == 960 || armor[k].type == 961 || armor[k].type == 962)
				{
					magicCrit += 4;
				}
				if (armor[k].type == 100 || armor[k].type == 101 || armor[k].type == 102)
				{
					meleeSpeed += 0.07f;
				}
				if (armor[k].type == 956 || armor[k].type == 957 || armor[k].type == 958)
				{
					meleeSpeed += 0.07f;
				}
				if (armor[k].type == 792 || armor[k].type == 793 || armor[k].type == 794)
				{
					meleeDamage += 0.02f;
					rangedDamage += 0.02f;
					magicDamage += 0.02f;
					thrownDamage += 0.02f;
				}
				if (armor[k].type == 371)
				{
					magicCrit += 9;
					statManaMax2 += 40;
				}
				if (armor[k].type == 372)
				{
					moveSpeed += 0.07f;
					meleeSpeed += 0.12f;
				}
				if (armor[k].type == 373)
				{
					rangedDamage += 0.1f;
					rangedCrit += 6;
				}
				if (armor[k].type == 374)
				{
					magicCrit += 3;
					meleeCrit += 3;
					rangedCrit += 3;
				}
				if (armor[k].type == 375)
				{
					moveSpeed += 0.1f;
				}
				if (armor[k].type == 376)
				{
					magicDamage += 0.15f;
					statManaMax2 += 60;
				}
				if (armor[k].type == 377)
				{
					meleeCrit += 5;
					meleeDamage += 0.1f;
				}
				if (armor[k].type == 378)
				{
					rangedDamage += 0.12f;
					rangedCrit += 7;
				}
				if (armor[k].type == 379)
				{
					rangedDamage += 0.05f;
					meleeDamage += 0.05f;
					magicDamage += 0.05f;
				}
				if (armor[k].type == 380)
				{
					magicCrit += 3;
					meleeCrit += 3;
					rangedCrit += 3;
				}
				if (armor[k].type >= 2367 && armor[k].type <= 2369)
				{
					fishingSkill += 5;
				}
				if (armor[k].type == 400)
				{
					magicDamage += 0.11f;
					magicCrit += 11;
					statManaMax2 += 80;
				}
				if (armor[k].type == 401)
				{
					meleeCrit += 7;
					meleeDamage += 0.14f;
				}
				if (armor[k].type == 402)
				{
					rangedDamage += 0.14f;
					rangedCrit += 8;
				}
				if (armor[k].type == 403)
				{
					rangedDamage += 0.06f;
					meleeDamage += 0.06f;
					magicDamage += 0.06f;
				}
				if (armor[k].type == 404)
				{
					magicCrit += 4;
					meleeCrit += 4;
					rangedCrit += 4;
					moveSpeed += 0.05f;
				}
				if (armor[k].type == 1205)
				{
					meleeDamage += 0.08f;
					meleeSpeed += 0.12f;
				}
				if (armor[k].type == 1206)
				{
					rangedDamage += 0.09f;
					rangedCrit += 9;
				}
				if (armor[k].type == 1207)
				{
					magicDamage += 0.07f;
					magicCrit += 7;
					statManaMax2 += 60;
				}
				if (armor[k].type == 1208)
				{
					meleeDamage += 0.03f;
					rangedDamage += 0.03f;
					magicDamage += 0.03f;
					magicCrit += 2;
					meleeCrit += 2;
					rangedCrit += 2;
				}
				if (armor[k].type == 1209)
				{
					meleeDamage += 0.02f;
					rangedDamage += 0.02f;
					magicDamage += 0.02f;
					magicCrit++;
					meleeCrit++;
					rangedCrit++;
				}
				if (armor[k].type == 1210)
				{
					meleeDamage += 0.07f;
					meleeSpeed += 0.07f;
					moveSpeed += 0.07f;
				}
				if (armor[k].type == 1211)
				{
					rangedCrit += 15;
					moveSpeed += 0.08f;
				}
				if (armor[k].type == 1212)
				{
					magicCrit += 18;
					statManaMax2 += 80;
				}
				if (armor[k].type == 1213)
				{
					magicCrit += 6;
					meleeCrit += 6;
					rangedCrit += 6;
				}
				if (armor[k].type == 1214)
				{
					moveSpeed += 0.11f;
				}
				if (armor[k].type == 1215)
				{
					meleeDamage += 0.08f;
					meleeCrit += 8;
					meleeSpeed += 0.08f;
				}
				if (armor[k].type == 1216)
				{
					rangedDamage += 0.16f;
					rangedCrit += 7;
				}
				if (armor[k].type == 1217)
				{
					magicDamage += 0.16f;
					magicCrit += 7;
					statManaMax2 += 100;
				}
				if (armor[k].type == 1218)
				{
					meleeDamage += 0.04f;
					rangedDamage += 0.04f;
					magicDamage += 0.04f;
					magicCrit += 3;
					meleeCrit += 3;
					rangedCrit += 3;
				}
				if (armor[k].type == 1219)
				{
					meleeDamage += 0.03f;
					rangedDamage += 0.03f;
					magicDamage += 0.03f;
					magicCrit += 3;
					meleeCrit += 3;
					rangedCrit += 3;
					moveSpeed += 0.06f;
				}
				if (armor[k].type == 558)
				{
					magicDamage += 0.12f;
					magicCrit += 12;
					statManaMax2 += 100;
				}
				if (armor[k].type == 559)
				{
					meleeCrit += 10;
					meleeDamage += 0.1f;
					meleeSpeed += 0.1f;
				}
				if (armor[k].type == 553)
				{
					rangedDamage += 0.15f;
					rangedCrit += 8;
				}
				if (armor[k].type == 551)
				{
					magicCrit += 7;
					meleeCrit += 7;
					rangedCrit += 7;
				}
				if (armor[k].type == 552)
				{
					rangedDamage += 0.07f;
					meleeDamage += 0.07f;
					magicDamage += 0.07f;
					moveSpeed += 0.08f;
				}
				if (armor[k].type == 1001)
				{
					meleeDamage += 0.16f;
					meleeCrit += 6;
				}
				if (armor[k].type == 1002)
				{
					rangedDamage += 0.16f;
					ammoCost80 = true;
				}
				if (armor[k].type == 1003)
				{
					statManaMax2 += 80;
					manaCost -= 0.17f;
					magicDamage += 0.16f;
				}
				if (armor[k].type == 1004)
				{
					meleeDamage += 0.05f;
					magicDamage += 0.05f;
					rangedDamage += 0.05f;
					magicCrit += 7;
					meleeCrit += 7;
					rangedCrit += 7;
				}
				if (armor[k].type == 1005)
				{
					magicCrit += 8;
					meleeCrit += 8;
					rangedCrit += 8;
					moveSpeed += 0.05f;
				}
				if (armor[k].type == 2189)
				{
					statManaMax2 += 60;
					manaCost -= 0.13f;
					magicDamage += 0.05f;
					magicCrit += 5;
				}
				if (armor[k].type == 1503)
				{
					magicDamage -= 0.4f;
				}
				if (armor[k].type == 1504)
				{
					magicDamage += 0.07f;
					magicCrit += 7;
				}
				if (armor[k].type == 1505)
				{
					magicDamage += 0.08f;
					moveSpeed += 0.08f;
				}
				if (armor[k].type == 1546)
				{
					rangedCrit += 5;
					arrowDamage += 0.15f;
				}
				if (armor[k].type == 1547)
				{
					rangedCrit += 5;
					bulletDamage += 0.15f;
				}
				if (armor[k].type == 1548)
				{
					rangedCrit += 5;
					rocketDamage += 0.15f;
				}
				if (armor[k].type == 1549)
				{
					rangedCrit += 13;
					rangedDamage += 0.13f;
					ammoCost80 = true;
				}
				if (armor[k].type == 1550)
				{
					rangedCrit += 7;
					moveSpeed += 0.12f;
				}
				if (armor[k].type == 1282)
				{
					statManaMax2 += 20;
					manaCost -= 0.05f;
				}
				if (armor[k].type == 1283)
				{
					statManaMax2 += 40;
					manaCost -= 0.07f;
				}
				if (armor[k].type == 1284)
				{
					statManaMax2 += 40;
					manaCost -= 0.09f;
				}
				if (armor[k].type == 1285)
				{
					statManaMax2 += 60;
					manaCost -= 0.11f;
				}
				if (armor[k].type == 1286)
				{
					statManaMax2 += 60;
					manaCost -= 0.13f;
				}
				if (armor[k].type == 1287)
				{
					statManaMax2 += 80;
					manaCost -= 0.15f;
				}
				if (armor[k].type == 1316 || armor[k].type == 1317 || armor[k].type == 1318)
				{
					aggro += 250;
				}
				if (armor[k].type == 1316)
				{
					meleeDamage += 0.06f;
				}
				if (armor[k].type == 1317)
				{
					meleeDamage += 0.08f;
					meleeCrit += 8;
				}
				if (armor[k].type == 1318)
				{
					meleeCrit += 4;
				}
				if (armor[k].type == 2199 || armor[k].type == 2202)
				{
					aggro += 250;
				}
				if (armor[k].type == 2201)
				{
					aggro += 400;
				}
				if (armor[k].type == 2199)
				{
					meleeDamage += 0.06f;
				}
				if (armor[k].type == 2200)
				{
					meleeDamage += 0.08f;
					meleeCrit += 8;
					meleeSpeed += 0.06f;
					moveSpeed += 0.06f;
				}
				if (armor[k].type == 2201)
				{
					meleeDamage += 0.05f;
					meleeCrit += 5;
				}
				if (armor[k].type == 2202)
				{
					meleeSpeed += 0.06f;
					moveSpeed += 0.06f;
				}
				if (armor[k].type == 684)
				{
					rangedDamage += 0.16f;
					meleeDamage += 0.16f;
				}
				if (armor[k].type == 685)
				{
					meleeCrit += 11;
					rangedCrit += 11;
				}
				if (armor[k].type == 686)
				{
					moveSpeed += 0.08f;
					meleeSpeed += 0.07f;
				}
				if (armor[k].type == 2361)
				{
					maxMinions++;
					minionDamage += 0.04f;
				}
				if (armor[k].type == 2362)
				{
					maxMinions++;
					minionDamage += 0.04f;
				}
				if (armor[k].type == 2363)
				{
					minionDamage += 0.05f;
				}
				if (armor[k].type >= 1158 && armor[k].type <= 1161)
				{
					maxMinions++;
				}
				if (armor[k].type >= 1159 && armor[k].type <= 1161)
				{
					minionDamage += 0.1f;
				}
				if (armor[k].type >= 2370 && armor[k].type <= 2371)
				{
					minionDamage += 0.05f;
					maxMinions++;
				}
				if (armor[k].type == 2372)
				{
					minionDamage += 0.06f;
					maxMinions++;
				}
				if (armor[k].type == 3381 || armor[k].type == 3382 || armor[k].type == 3383)
				{
					if (armor[k].type != 3381)
					{
						maxMinions++;
					}
					maxMinions++;
					minionDamage += 0.22f;
				}
				if (armor[k].type == 2763)
				{
					aggro += 300;
					meleeCrit += 17;
				}
				if (armor[k].type == 2764)
				{
					aggro += 300;
					meleeDamage += 0.22f;
				}
				if (armor[k].type == 2765)
				{
					aggro += 300;
					meleeSpeed += 0.15f;
					moveSpeed += 0.15f;
				}
				if (armor[k].type == 2757)
				{
					rangedCrit += 7;
					rangedDamage += 0.16f;
				}
				if (armor[k].type == 2758)
				{
					ammoCost75 = true;
					rangedCrit += 12;
					rangedDamage += 0.12f;
				}
				if (armor[k].type == 2759)
				{
					rangedCrit += 8;
					rangedDamage += 0.08f;
					moveSpeed += 0.1f;
				}
				if (armor[k].type == 2760)
				{
					statManaMax2 += 60;
					manaCost -= 0.15f;
					magicCrit += 7;
					magicDamage += 0.07f;
				}
				if (armor[k].type == 2761)
				{
					magicDamage += 0.09f;
					magicCrit += 9;
				}
				if (armor[k].type == 2762)
				{
					moveSpeed += 0.1f;
					magicDamage += 0.1f;
				}
				if (armor[k].type >= 1832 && armor[k].type <= 1834)
				{
					maxMinions++;
				}
				if (armor[k].type >= 1832 && armor[k].type <= 1834)
				{
					minionDamage += 0.11f;
				}
				if (armor[k].prefix == 62)
				{
					statDefense++;
				}
				if (armor[k].prefix == 63)
				{
					statDefense += 2;
				}
				if (armor[k].prefix == 64)
				{
					statDefense += 3;
				}
				if (armor[k].prefix == 65)
				{
					statDefense += 4;
				}
				if (armor[k].prefix == 66)
				{
					statManaMax2 += 20;
				}
				if (armor[k].prefix == 67)
				{
					meleeCrit += 2;
					rangedCrit += 2;
					magicCrit += 2;
					thrownCrit += 2;
				}
				if (armor[k].prefix == 68)
				{
					meleeCrit += 4;
					rangedCrit += 4;
					magicCrit += 4;
					thrownCrit += 4;
				}
				if (armor[k].prefix == 69)
				{
					meleeDamage += 0.01f;
					rangedDamage += 0.01f;
					magicDamage += 0.01f;
					minionDamage += 0.01f;
					thrownDamage += 0.01f;
				}
				if (armor[k].prefix == 70)
				{
					meleeDamage += 0.02f;
					rangedDamage += 0.02f;
					magicDamage += 0.02f;
					minionDamage += 0.02f;
					thrownDamage += 0.02f;
				}
				if (armor[k].prefix == 71)
				{
					meleeDamage += 0.03f;
					rangedDamage += 0.03f;
					magicDamage += 0.03f;
					minionDamage += 0.03f;
					thrownDamage += 0.03f;
				}
				if (armor[k].prefix == 72)
				{
					meleeDamage += 0.04f;
					rangedDamage += 0.04f;
					magicDamage += 0.04f;
					minionDamage += 0.04f;
					thrownDamage += 0.04f;
				}
				if (armor[k].prefix == 73)
				{
					moveSpeed += 0.01f;
				}
				if (armor[k].prefix == 74)
				{
					moveSpeed += 0.02f;
				}
				if (armor[k].prefix == 75)
				{
					moveSpeed += 0.03f;
				}
				if (armor[k].prefix == 76)
				{
					moveSpeed += 0.04f;
				}
				if (armor[k].prefix == 77)
				{
					meleeSpeed += 0.01f;
				}
				if (armor[k].prefix == 78)
				{
					meleeSpeed += 0.02f;
				}
				if (armor[k].prefix == 79)
				{
					meleeSpeed += 0.03f;
				}
				if (armor[k].prefix == 80)
				{
					meleeSpeed += 0.04f;
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int l = 3; l < 8 + extraAccessorySlots; l++)
			{
				if (armor[l].expertOnly && !Main.expertMode)
				{
					continue;
				}
				if (armor[l].type == 3015)
				{
					aggro -= 400;
					meleeCrit += 5;
					magicCrit += 5;
					rangedCrit += 5;
					thrownCrit += 5;
					meleeDamage += 0.05f;
					magicDamage += 0.05f;
					rangedDamage += 0.05f;
					thrownDamage += 0.05f;
					minionDamage += 0.05f;
				}
				if (armor[l].type == 3016)
				{
					aggro += 400;
				}
				if (armor[l].type == 2373)
				{
					accFishingLine = true;
				}
				if (armor[l].type == 2374)
				{
					fishingSkill += 10;
				}
				if (armor[l].type == 2375)
				{
					accTackleBox = true;
				}
				if (armor[l].type == 3090)
				{
					npcTypeNoAggro[1] = true;
					npcTypeNoAggro[16] = true;
					npcTypeNoAggro[59] = true;
					npcTypeNoAggro[71] = true;
					npcTypeNoAggro[81] = true;
					npcTypeNoAggro[138] = true;
					npcTypeNoAggro[121] = true;
					npcTypeNoAggro[122] = true;
					npcTypeNoAggro[141] = true;
					npcTypeNoAggro[147] = true;
					npcTypeNoAggro[183] = true;
					npcTypeNoAggro[184] = true;
					npcTypeNoAggro[204] = true;
					npcTypeNoAggro[225] = true;
					npcTypeNoAggro[244] = true;
					npcTypeNoAggro[302] = true;
					npcTypeNoAggro[333] = true;
					npcTypeNoAggro[335] = true;
					npcTypeNoAggro[334] = true;
					npcTypeNoAggro[336] = true;
					npcTypeNoAggro[537] = true;
				}
				if (armor[l].stringColor > 0)
				{
					yoyoString = true;
				}
				if (armor[l].type == 3366)
				{
					counterWeight = 556 + Main.rand.Next(6);
					yoyoGlove = true;
					yoyoString = true;
				}
				if (armor[l].type >= 3309 && armor[l].type <= 3314)
				{
					counterWeight = 556 + armor[l].type - 3309;
				}
				if (armor[l].type == 3334)
				{
					yoyoGlove = true;
				}
				if (armor[l].type == 3337)
				{
					shinyStone = true;
				}
				if (armor[l].type == 3336)
				{
					SporeSac();
					sporeSac = true;
				}
				if (armor[l].type == 2423)
				{
					autoJump = true;
					jumpSpeedBoost += 2.4f;
					extraFall += 15;
				}
				if (armor[l].type == 857)
				{
					doubleJumpSandstorm = true;
				}
				if (armor[l].type == 983)
				{
					doubleJumpSandstorm = true;
					jumpBoost = true;
				}
				if (armor[l].type == 987)
				{
					doubleJumpBlizzard = true;
				}
				if (armor[l].type == 1163)
				{
					doubleJumpBlizzard = true;
					jumpBoost = true;
				}
				if (armor[l].type == 1724)
				{
					doubleJumpFart = true;
				}
				if (armor[l].type == 1863)
				{
					doubleJumpFart = true;
					jumpBoost = true;
				}
				if (armor[l].type == 1164)
				{
					doubleJumpCloud = true;
					doubleJumpSandstorm = true;
					doubleJumpBlizzard = true;
					jumpBoost = true;
				}
				if (armor[l].type == 1250)
				{
					jumpBoost = true;
					doubleJumpCloud = true;
					noFallDmg = true;
				}
				if (armor[l].type == 1252)
				{
					doubleJumpSandstorm = true;
					jumpBoost = true;
					noFallDmg = true;
				}
				if (armor[l].type == 1251)
				{
					doubleJumpBlizzard = true;
					jumpBoost = true;
					noFallDmg = true;
				}
				if (armor[l].type == 3250)
				{
					doubleJumpFart = true;
					jumpBoost = true;
					noFallDmg = true;
				}
				if (armor[l].type == 3252)
				{
					doubleJumpSail = true;
					jumpBoost = true;
					noFallDmg = true;
				}
				if (armor[l].type == 3251)
				{
					jumpBoost = true;
					bee = true;
					noFallDmg = true;
				}
				if (armor[l].type == 1249)
				{
					jumpBoost = true;
					bee = true;
				}
				if (armor[l].type == 3241)
				{
					jumpBoost = true;
					doubleJumpSail = true;
				}
				if (armor[l].type == 1253 && (double)statLife <= (double)statLifeMax2 * 0.5)
				{
					AddBuff(62, 5);
				}
				if (armor[l].type == 1290)
				{
					panic = true;
				}
				if ((armor[l].type == 1300 || armor[l].type == 1858) && (inventory[selectedItem].useAmmo == 14 || inventory[selectedItem].useAmmo == 311 || inventory[selectedItem].useAmmo == 323 || inventory[selectedItem].useAmmo == 23))
				{
					scope = true;
				}
				if (armor[l].type == 1858)
				{
					rangedCrit += 10;
					rangedDamage += 0.1f;
				}
				if (armor[l].type == 1303 && wet)
				{
					Lighting.AddLight((int)base.Center.X / 16, (int)base.Center.Y / 16, 0.9f, 0.2f, 0.6f);
				}
				if (armor[l].type == 1301)
				{
					meleeCrit += 8;
					rangedCrit += 8;
					magicCrit += 8;
					thrownCrit += 8;
					meleeDamage += 0.1f;
					rangedDamage += 0.1f;
					magicDamage += 0.1f;
					minionDamage += 0.1f;
					thrownDamage += 0.1f;
				}
				if (armor[l].type == 982)
				{
					statManaMax2 += 20;
					manaRegenDelayBonus++;
					manaRegenBonus += 25;
				}
				if (armor[l].type == 1595)
				{
					statManaMax2 += 20;
					magicCuffs = true;
				}
				if (armor[l].type == 2219)
				{
					manaMagnet = true;
				}
				if (armor[l].type == 2220)
				{
					manaMagnet = true;
					magicDamage += 0.15f;
				}
				if (armor[l].type == 2221)
				{
					manaMagnet = true;
					magicCuffs = true;
				}
				if (whoAmI == Main.myPlayer && armor[l].type == 1923)
				{
					tileRangeX++;
					tileRangeY++;
				}
				if (armor[l].type == 1247)
				{
					starCloak = true;
					bee = true;
				}
				if (armor[l].type == 1248)
				{
					meleeCrit += 10;
					rangedCrit += 10;
					magicCrit += 10;
					thrownCrit += 10;
				}
				if (armor[l].type == 854)
				{
					discount = true;
				}
				if (armor[l].type == 855)
				{
					coins = true;
				}
				if (armor[l].type == 3033)
				{
					goldRing = true;
				}
				if (armor[l].type == 3034)
				{
					goldRing = true;
					coins = true;
				}
				if (armor[l].type == 3035)
				{
					goldRing = true;
					coins = true;
					discount = true;
				}
				if (armor[l].type == 53)
				{
					doubleJumpCloud = true;
				}
				if (armor[l].type == 3201)
				{
					doubleJumpSail = true;
				}
				if (armor[l].type == 54)
				{
					accRunSpeed = 6f;
				}
				if (armor[l].type == 3068)
				{
					cordage = true;
				}
				if (armor[l].type == 1579)
				{
					accRunSpeed = 6f;
					coldDash = true;
				}
				if (armor[l].type == 3200)
				{
					accRunSpeed = 6f;
					sailDash = true;
				}
				if (armor[l].type == 128)
				{
					rocketBoots = 1;
				}
				if (armor[l].type == 156)
				{
					noKnockback = true;
				}
				if (armor[l].type == 158)
				{
					noFallDmg = true;
				}
				if (armor[l].type == 934)
				{
					carpet = true;
				}
				if (armor[l].type == 953)
				{
					spikedBoots++;
				}
				if (armor[l].type == 975)
				{
					spikedBoots++;
				}
				if (armor[l].type == 976)
				{
					spikedBoots += 2;
				}
				if (armor[l].type == 977)
				{
					dash = 1;
				}
				if (armor[l].type == 3097)
				{
					dash = 2;
				}
				if (armor[l].type == 963)
				{
					blackBelt = true;
				}
				if (armor[l].type == 984)
				{
					blackBelt = true;
					dash = 1;
					spikedBoots = 2;
				}
				if (armor[l].type == 1131)
				{
					gravControl2 = true;
				}
				if (armor[l].type == 1132)
				{
					bee = true;
				}
				if (armor[l].type == 1578)
				{
					bee = true;
					panic = true;
				}
				if (armor[l].type == 3224)
				{
					endurance += 0.17f;
				}
				if (armor[l].type == 3223)
				{
					brainOfConfusion = true;
				}
				if (armor[l].type == 950)
				{
					iceSkate = true;
				}
				if (armor[l].type == 159)
				{
					jumpBoost = true;
				}
				if (armor[l].type == 3225)
				{
					jumpBoost = true;
				}
				if (armor[l].type == 187)
				{
					accFlipper = true;
				}
				if (armor[l].type == 211)
				{
					meleeSpeed += 0.12f;
				}
				if (armor[l].type == 223)
				{
					manaCost -= 0.06f;
				}
				if (armor[l].type == 285)
				{
					moveSpeed += 0.05f;
				}
				if (armor[l].type == 212)
				{
					moveSpeed += 0.1f;
				}
				if (armor[l].type == 267)
				{
					killGuide = true;
				}
				if (armor[l].type == 1307)
				{
					killClothier = true;
				}
				if (armor[l].type == 193)
				{
					fireWalk = true;
				}
				if (armor[l].type == 861)
				{
					accMerman = true;
					wolfAcc = true;
					if (hideVisual[l])
					{
						hideMerman = true;
						hideWolf = true;
					}
				}
				if (armor[l].type == 862)
				{
					starCloak = true;
					longInvince = true;
				}
				if (armor[l].type == 860)
				{
					pStone = true;
				}
				if (armor[l].type == 863)
				{
					waterWalk2 = true;
				}
				if (armor[l].type == 907)
				{
					waterWalk2 = true;
					fireWalk = true;
				}
				if (armor[l].type == 908)
				{
					waterWalk = true;
					fireWalk = true;
					lavaMax += 420;
				}
				if (armor[l].type == 906)
				{
					lavaMax += 420;
				}
				if (armor[l].type == 485)
				{
					wolfAcc = true;
					if (hideVisual[l])
					{
						hideWolf = true;
					}
				}
				if (armor[l].type == 486 && !hideVisual[l])
				{
					rulerLine = true;
				}
				if (armor[l].type == 2799 && !hideVisual[l])
				{
					rulerGrid = true;
				}
				if (armor[l].type == 394)
				{
					accFlipper = true;
					accDivingHelm = true;
				}
				if (armor[l].type == 396)
				{
					noFallDmg = true;
					fireWalk = true;
				}
				if (armor[l].type == 397)
				{
					noKnockback = true;
					fireWalk = true;
				}
				if (armor[l].type == 399)
				{
					jumpBoost = true;
					doubleJumpCloud = true;
				}
				if (armor[l].type == 405)
				{
					accRunSpeed = 6f;
					rocketBoots = 2;
				}
				if (armor[l].type == 1860)
				{
					accFlipper = true;
					accDivingHelm = true;
					if (wet)
					{
						Lighting.AddLight((int)base.Center.X / 16, (int)base.Center.Y / 16, 0.9f, 0.2f, 0.6f);
					}
				}
				if (armor[l].type == 1861)
				{
					arcticDivingGear = true;
					accFlipper = true;
					accDivingHelm = true;
					iceSkate = true;
					if (wet)
					{
						Lighting.AddLight((int)base.Center.X / 16, (int)base.Center.Y / 16, 0.2f, 0.8f, 0.9f);
					}
				}
				if (armor[l].type == 2214)
				{
					flag2 = true;
				}
				if (armor[l].type == 2215)
				{
					flag3 = true;
				}
				if (armor[l].type == 2216)
				{
					autoPaint = true;
				}
				if (armor[l].type == 2217)
				{
					flag = true;
				}
				if (armor[l].type == 3061)
				{
					flag = true;
					flag2 = true;
					autoPaint = true;
					flag3 = true;
				}
				if (armor[l].type == 897)
				{
					kbGlove = true;
					meleeSpeed += 0.12f;
				}
				if (armor[l].type == 1343)
				{
					kbGlove = true;
					meleeSpeed += 0.1f;
					meleeDamage += 0.1f;
					magmaStone = true;
				}
				if (armor[l].type == 1167)
				{
					minionKB += 2f;
					minionDamage += 0.15f;
				}
				if (armor[l].type == 1864)
				{
					minionKB += 2f;
					minionDamage += 0.15f;
					maxMinions++;
				}
				if (armor[l].type == 1845)
				{
					minionDamage += 0.1f;
					maxMinions++;
				}
				if (armor[l].type == 1321)
				{
					magicQuiver = true;
					arrowDamage += 0.1f;
				}
				if (armor[l].type == 1322)
				{
					magmaStone = true;
				}
				if (armor[l].type == 1323)
				{
					lavaRose = true;
				}
				if (armor[l].type == 3333)
				{
					strongBees = true;
				}
				if (armor[l].type == 938)
				{
					noKnockback = true;
					if ((double)statLife > (double)statLifeMax2 * 0.25)
					{
						if (i == Main.myPlayer)
						{
							paladinGive = true;
						}
						else if (miscCounter % 5 == 0)
						{
							int myPlayer = Main.myPlayer;
							if (Main.player[myPlayer].team == team && team != 0)
							{
								float num3 = position.X - Main.player[myPlayer].position.X;
								float num4 = position.Y - Main.player[myPlayer].position.Y;
								float num5 = (float)Math.Sqrt(num3 * num3 + num4 * num4);
								if (num5 < 800f)
								{
									Main.player[myPlayer].AddBuff(43, 10);
								}
							}
						}
					}
				}
				if (armor[l].type == 936)
				{
					kbGlove = true;
					meleeSpeed += 0.12f;
					meleeDamage += 0.12f;
				}
				if (armor[l].type == 898)
				{
					accRunSpeed = 6.75f;
					rocketBoots = 2;
					moveSpeed += 0.08f;
				}
				if (armor[l].type == 1862)
				{
					accRunSpeed = 6.75f;
					rocketBoots = 3;
					moveSpeed += 0.08f;
					iceSkate = true;
				}
				if (armor[l].type == 3110)
				{
					accMerman = true;
					wolfAcc = true;
					if (hideVisual[l])
					{
						hideMerman = true;
						hideWolf = true;
					}
				}
				if (armor[l].type == 1865 || armor[l].type == 3110)
				{
					lifeRegen += 2;
					statDefense += 4;
					meleeSpeed += 0.1f;
					meleeDamage += 0.1f;
					meleeCrit += 2;
					rangedDamage += 0.1f;
					rangedCrit += 2;
					magicDamage += 0.1f;
					magicCrit += 2;
					pickSpeed -= 0.15f;
					minionDamage += 0.1f;
					minionKB += 0.5f;
					thrownDamage += 0.1f;
					thrownCrit += 2;
				}
				if (armor[l].type == 899 && Main.dayTime)
				{
					lifeRegen += 2;
					statDefense += 4;
					meleeSpeed += 0.1f;
					meleeDamage += 0.1f;
					meleeCrit += 2;
					rangedDamage += 0.1f;
					rangedCrit += 2;
					magicDamage += 0.1f;
					magicCrit += 2;
					pickSpeed -= 0.15f;
					minionDamage += 0.1f;
					minionKB += 0.5f;
					thrownDamage += 0.1f;
					thrownCrit += 2;
				}
				if (armor[l].type == 900 && (!Main.dayTime || Main.eclipse))
				{
					lifeRegen += 2;
					statDefense += 4;
					meleeSpeed += 0.1f;
					meleeDamage += 0.1f;
					meleeCrit += 2;
					rangedDamage += 0.1f;
					rangedCrit += 2;
					magicDamage += 0.1f;
					magicCrit += 2;
					pickSpeed -= 0.15f;
					minionDamage += 0.1f;
					minionKB += 0.5f;
					thrownDamage += 0.1f;
					thrownCrit += 2;
				}
				if (armor[l].type == 407)
				{
					blockRange = 1;
				}
				if (armor[l].type == 489)
				{
					magicDamage += 0.15f;
				}
				if (armor[l].type == 490)
				{
					meleeDamage += 0.15f;
				}
				if (armor[l].type == 491)
				{
					rangedDamage += 0.15f;
				}
				if (armor[l].type == 2998)
				{
					minionDamage += 0.15f;
				}
				if (armor[l].type == 935)
				{
					magicDamage += 0.12f;
					meleeDamage += 0.12f;
					rangedDamage += 0.12f;
					minionDamage += 0.12f;
					thrownDamage += 0.12f;
				}
				if (armor[l].type == 492)
				{
					wingTimeMax = 100;
				}
				if (armor[l].type == 493)
				{
					wingTimeMax = 100;
				}
				if (armor[l].type == 748)
				{
					wingTimeMax = 115;
				}
				if (armor[l].type == 749)
				{
					wingTimeMax = 130;
				}
				if (armor[l].type == 761)
				{
					wingTimeMax = 130;
				}
				if (armor[l].type == 785)
				{
					wingTimeMax = 140;
				}
				if (armor[l].type == 786)
				{
					wingTimeMax = 140;
				}
				if (armor[l].type == 821)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 822)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 823)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 2280)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 2494)
				{
					wingTimeMax = 100;
				}
				if (armor[l].type == 2609)
				{
					wingTimeMax = 180;
					ignoreWater = true;
				}
				if (armor[l].type == 948)
				{
					wingTimeMax = 180;
				}
				if (armor[l].type == 1162)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 1165)
				{
					wingTimeMax = 140;
				}
				if (armor[l].type == 1515)
				{
					wingTimeMax = 130;
				}
				if (armor[l].type == 665)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 1583)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 1584)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 1585)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 1586)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 3228)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 3580)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 3582)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 3588)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 3592)
				{
					wingTimeMax = 150;
				}
				if (armor[l].type == 1797)
				{
					wingTimeMax = 180;
				}
				if (armor[l].type == 1830)
				{
					wingTimeMax = 180;
				}
				if (armor[l].type == 1866)
				{
					wingTimeMax = 170;
				}
				if (armor[l].type == 1871)
				{
					wingTimeMax = 170;
				}
				if (armor[l].type == 2770)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 3468)
				{
					wingTimeMax = 180;
				}
				if (armor[l].type == 3469)
				{
					wingTimeMax = 160;
				}
				if (armor[l].type == 3470)
				{
					wingTimeMax = 180;
				}
				if (armor[l].type == 3471)
				{
					wingTimeMax = 220;
				}
				if (armor[l].type == 885)
				{
					buffImmune[30] = true;
				}
				if (armor[l].type == 886)
				{
					buffImmune[36] = true;
				}
				if (armor[l].type == 887)
				{
					buffImmune[20] = true;
				}
				if (armor[l].type == 888)
				{
					buffImmune[22] = true;
				}
				if (armor[l].type == 889)
				{
					buffImmune[32] = true;
				}
				if (armor[l].type == 890)
				{
					buffImmune[35] = true;
				}
				if (armor[l].type == 891)
				{
					buffImmune[23] = true;
				}
				if (armor[l].type == 892)
				{
					buffImmune[33] = true;
				}
				if (armor[l].type == 893)
				{
					buffImmune[31] = true;
				}
				if (armor[l].type == 901)
				{
					buffImmune[33] = true;
					buffImmune[36] = true;
				}
				if (armor[l].type == 902)
				{
					buffImmune[30] = true;
					buffImmune[20] = true;
				}
				if (armor[l].type == 903)
				{
					buffImmune[32] = true;
					buffImmune[31] = true;
				}
				if (armor[l].type == 904)
				{
					buffImmune[35] = true;
					buffImmune[23] = true;
				}
				if (armor[l].type == 1921)
				{
					buffImmune[46] = true;
					buffImmune[47] = true;
				}
				if (armor[l].type == 1612)
				{
					buffImmune[33] = true;
					buffImmune[36] = true;
					buffImmune[30] = true;
					buffImmune[20] = true;
					buffImmune[32] = true;
					buffImmune[31] = true;
					buffImmune[35] = true;
					buffImmune[23] = true;
					buffImmune[22] = true;
				}
				if (armor[l].type == 1613)
				{
					buffImmune[46] = true;
					noKnockback = true;
					fireWalk = true;
					buffImmune[33] = true;
					buffImmune[36] = true;
					buffImmune[30] = true;
					buffImmune[20] = true;
					buffImmune[32] = true;
					buffImmune[31] = true;
					buffImmune[35] = true;
					buffImmune[23] = true;
					buffImmune[22] = true;
				}
				if (armor[l].type == 497)
				{
					accMerman = true;
					if (hideVisual[l])
					{
						hideMerman = true;
					}
				}
				if (armor[l].type == 535)
				{
					pStone = true;
				}
				if (armor[l].type == 536)
				{
					kbGlove = true;
				}
				if (armor[l].type == 532)
				{
					starCloak = true;
				}
				if (armor[l].type == 554)
				{
					longInvince = true;
				}
				if (armor[l].type == 555)
				{
					manaFlower = true;
					manaCost -= 0.08f;
				}
				if (Main.myPlayer != whoAmI)
				{
					continue;
				}
				if (armor[l].type == 576 && Main.rand.Next(10800) == 0 && Main.curMusic > 0 && Main.curMusic <= 39)
				{
					int num6 = 0;
					if (Main.curMusic == 1)
					{
						num6 = 0;
					}
					if (Main.curMusic == 2)
					{
						num6 = 1;
					}
					if (Main.curMusic == 3)
					{
						num6 = 2;
					}
					if (Main.curMusic == 4)
					{
						num6 = 4;
					}
					if (Main.curMusic == 5)
					{
						num6 = 5;
					}
					if (Main.curMusic == 6)
					{
						num6 = 3;
					}
					if (Main.curMusic == 7)
					{
						num6 = 6;
					}
					if (Main.curMusic == 8)
					{
						num6 = 7;
					}
					if (Main.curMusic == 9)
					{
						num6 = 9;
					}
					if (Main.curMusic == 10)
					{
						num6 = 8;
					}
					if (Main.curMusic == 11)
					{
						num6 = 11;
					}
					if (Main.curMusic == 12)
					{
						num6 = 10;
					}
					if (Main.curMusic == 13)
					{
						num6 = 12;
					}
					if (Main.curMusic == 28)
					{
						armor[l].SetDefaults(1963);
					}
					else if (Main.curMusic == 29)
					{
						armor[l].SetDefaults(1610);
					}
					else if (Main.curMusic == 30)
					{
						armor[l].SetDefaults(1963);
					}
					else if (Main.curMusic == 31)
					{
						armor[l].SetDefaults(1964);
					}
					else if (Main.curMusic == 32)
					{
						armor[l].SetDefaults(1965);
					}
					else if (Main.curMusic == 33)
					{
						armor[l].SetDefaults(2742);
					}
					else if (Main.curMusic == 34)
					{
						armor[l].SetDefaults(3370);
					}
					else if (Main.curMusic == 35)
					{
						armor[l].SetDefaults(3236);
					}
					else if (Main.curMusic == 36)
					{
						armor[l].SetDefaults(3237);
					}
					else if (Main.curMusic == 37)
					{
						armor[l].SetDefaults(3235);
					}
					else if (Main.curMusic == 38)
					{
						armor[l].SetDefaults(3044);
					}
					else if (Main.curMusic == 39)
					{
						armor[l].SetDefaults(3371);
					}
					else if (Main.curMusic > 13)
					{
						armor[l].SetDefaults(1596 + Main.curMusic - 14);
					}
					else
					{
						armor[l].SetDefaults(num6 + 562);
					}
				}
				if (armor[l].type >= 562 && armor[l].type <= 574)
				{
					Main.musicBox2 = armor[l].type - 562;
				}
				if (armor[l].type >= 1596 && armor[l].type <= 1609)
				{
					Main.musicBox2 = armor[l].type - 1596 + 13;
				}
				if (armor[l].type == 1610)
				{
					Main.musicBox2 = 27;
				}
				if (armor[l].type == 1963)
				{
					Main.musicBox2 = 28;
				}
				if (armor[l].type == 1964)
				{
					Main.musicBox2 = 29;
				}
				if (armor[l].type == 1965)
				{
					Main.musicBox2 = 30;
				}
				if (armor[l].type == 2742)
				{
					Main.musicBox2 = 31;
				}
				if (armor[l].type == 3044)
				{
					Main.musicBox2 = 32;
				}
				if (armor[l].type == 3235)
				{
					Main.musicBox2 = 33;
				}
				if (armor[l].type == 3236)
				{
					Main.musicBox2 = 34;
				}
				if (armor[l].type == 3237)
				{
					Main.musicBox2 = 35;
				}
				if (armor[l].type == 3370)
				{
					Main.musicBox2 = 36;
				}
				if (armor[l].type == 3371)
				{
					Main.musicBox2 = 37;
				}
			}
			for (int m = 3; m < 8 + extraAccessorySlots; m++)
			{
				if (armor[m].wingSlot > 0)
				{
					if (!hideVisual[m] || (velocity.Y != 0f && !mount.Active))
					{
						wings = armor[m].wingSlot;
					}
					wingsLogic = armor[m].wingSlot;
				}
			}
			for (int n = 13; n < 18 + extraAccessorySlots; n++)
			{
				int type3 = armor[n].type;
				if (armor[n].wingSlot > 0)
				{
					wings = armor[n].wingSlot;
				}
				if (type3 == 861 || type3 == 3110 || type3 == 485)
				{
					hideWolf = false;
					forceWerewolf = true;
				}
				if (((wet && !lavaWet && (!mount.Active || mount.Type != 3)) || !forceWerewolf) && (type3 == 861 || type3 == 3110 || type3 == 497))
				{
					hideMerman = false;
					forceMerman = true;
				}
			}
			if (whoAmI == Main.myPlayer && Main.clock && accWatch < 3)
			{
				accWatch++;
			}
			if (flag2)
			{
				tileSpeed += 0.5f;
			}
			if (flag)
			{
				wallSpeed += 0.5f;
			}
			if (flag3 && whoAmI == Main.myPlayer)
			{
				tileRangeX += 3;
				tileRangeY += 2;
			}
			if (!accThirdEye)
			{
				accThirdEyeCounter = 0;
			}
			if (Main.netMode == 1 && whoAmI == Main.myPlayer)
			{
				for (int num7 = 0; num7 < 16; num7++)
				{
					if (num7 == whoAmI || !Main.player[num7].active || Main.player[num7].dead || Main.player[num7].team != team || Main.player[num7].team == 0)
					{
						continue;
					}
					int num8 = 800;
					if ((Main.player[num7].Center - base.Center).Length() < (float)num8)
					{
						if (Main.player[num7].accWatch > accWatch)
						{
							accWatch = Main.player[num7].accWatch;
						}
						if (Main.player[num7].accCompass > accCompass)
						{
							accCompass = Main.player[num7].accCompass;
						}
						if (Main.player[num7].accDepthMeter > accDepthMeter)
						{
							accDepthMeter = Main.player[num7].accDepthMeter;
						}
						if (Main.player[num7].accFishFinder)
						{
							accFishFinder = true;
						}
						if (Main.player[num7].accWeatherRadio)
						{
							accWeatherRadio = true;
						}
						if (Main.player[num7].accThirdEye)
						{
							accThirdEye = true;
						}
						if (Main.player[num7].accJarOfSouls)
						{
							accJarOfSouls = true;
						}
						if (Main.player[num7].accCalendar)
						{
							accCalendar = true;
						}
						if (Main.player[num7].accStopwatch)
						{
							accStopwatch = true;
						}
						if (Main.player[num7].accOreFinder)
						{
							accOreFinder = true;
						}
						if (Main.player[num7].accCritterGuide)
						{
							accCritterGuide = true;
						}
						if (Main.player[num7].accDreamCatcher)
						{
							accDreamCatcher = true;
						}
					}
				}
			}
			if (!accDreamCatcher && dpsStarted)
			{
				dpsStarted = false;
				dpsEnd = DateTime.Now;
			}
		}

		public void UpdateArmorSets(int i)
		{
			setBonus = "";
			if (body == 67 && legs == 56 && head >= 103 && head <= 105)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Shroomite");
				shroomiteStealth = true;
			}
			if ((head == 52 && body == 32 && legs == 31) || (head == 53 && body == 33 && legs == 32) || (head == 54 && body == 34 && legs == 33) || (head == 55 && body == 35 && legs == 34) || (head == 70 && body == 46 && legs == 42) || (head == 71 && body == 47 && legs == 43) || (head == 166 && body == 173 && legs == 108) || (head == 167 && body == 174 && legs == 109))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Wood");
				statDefense++;
			}
			if ((head == 1 && body == 1 && legs == 1) || ((head == 72 || head == 2) && body == 2 && legs == 2) || (head == 47 && body == 28 && legs == 27))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.MetalTier1");
				statDefense += 2;
			}
			if ((head == 3 && body == 3 && legs == 3) || ((head == 73 || head == 4) && body == 4 && legs == 4) || (head == 48 && body == 29 && legs == 28) || (head == 49 && body == 30 && legs == 29))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.MetalTier2");
				statDefense += 3;
			}
			if (head == 188 && body == 189 && legs == 129)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Fossil");
				thrownCost50 = true;
			}
			if (head == 50 && body == 31 && legs == 30)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Platinum");
				statDefense += 4;
			}
			if (head == 112 && body == 75 && legs == 64)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Pumpkin");
				meleeDamage += 0.1f;
				magicDamage += 0.1f;
				rangedDamage += 0.1f;
				thrownDamage += 0.1f;
			}
			if (head == 22 && body == 14 && legs == 14)
			{
				thrownCost33 = true;
				setBonus = Language.GetTextValue("ArmorSetBonus.Ninja");
			}
			if (head == 157 && body == 105 && legs == 98)
			{
				int num = 0;
				setBonus = Language.GetTextValue("ArmorSetBonus.BeetleDamage");
				beetleOffense = true;
				beetleCounter -= 3f;
				beetleCounter -= beetleCountdown / 10;
				beetleCountdown++;
				if (beetleCounter < 0f)
				{
					beetleCounter = 0f;
				}
				int num2 = 400;
				int num3 = 1200;
				int num4 = 4600;
				if (beetleCounter > (float)(num2 + num3 + num4 + num3))
				{
					beetleCounter = num2 + num3 + num4 + num3;
				}
				if (beetleCounter > (float)(num2 + num3 + num4))
				{
					AddBuff(100, 5, false);
					num = 3;
				}
				else if (beetleCounter > (float)(num2 + num3))
				{
					AddBuff(99, 5, false);
					num = 2;
				}
				else if (beetleCounter > (float)num2)
				{
					AddBuff(98, 5, false);
					num = 1;
				}
				if (num < beetleOrbs)
				{
					beetleCountdown = 0;
				}
				else if (num > beetleOrbs)
				{
					beetleCounter += 200f;
				}
				if (num != beetleOrbs && beetleOrbs > 0)
				{
					for (int j = 0; j < 22; j++)
					{
						if (buffType[j] >= 98 && buffType[j] <= 100 && buffType[j] != 97 + num)
						{
							DelBuff(j);
						}
					}
				}
			}
			else if (head == 157 && body == 106 && legs == 98)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.BeetleDefense");
				beetleDefense = true;
				beetleCounter += 1f;
				int num5 = 180;
				if (beetleCounter >= (float)num5)
				{
					if (beetleOrbs > 0 && beetleOrbs < 3)
					{
						for (int k = 0; k < 22; k++)
						{
							if (buffType[k] >= 95 && buffType[k] <= 96)
							{
								DelBuff(k);
							}
						}
					}
					if (beetleOrbs < 3)
					{
						AddBuff(95 + beetleOrbs, 5, false);
						beetleCounter = 0f;
					}
					else
					{
						beetleCounter = num5;
					}
				}
			}
			if (!beetleDefense && !beetleOffense)
			{
				beetleCounter = 0f;
			}
			else
			{
				beetleFrameCounter++;
				if (beetleFrameCounter >= 1)
				{
					beetleFrameCounter = 0;
					beetleFrame++;
					if (beetleFrame > 2)
					{
						beetleFrame = 0;
					}
				}
				for (int l = beetleOrbs; l < 3; l++)
				{
					beetlePos[l].X = 0f;
					beetlePos[l].Y = 0f;
				}
				for (int m = 0; m < beetleOrbs; m++)
				{
					beetlePos[m] += beetleVel[m];
					beetleVel[m].X += (float)Main.rand.Next(-100, 101) * 0.005f;
					beetleVel[m].Y += (float)Main.rand.Next(-100, 101) * 0.005f;
					float x = beetlePos[m].X;
					float y = beetlePos[m].Y;
					float num6 = (float)Math.Sqrt(x * x + y * y);
					if (num6 > 100f)
					{
						num6 = 20f / num6;
						x *= 0f - num6;
						y *= 0f - num6;
						int num7 = 10;
						beetleVel[m].X = (beetleVel[m].X * (float)(num7 - 1) + x) / (float)num7;
						beetleVel[m].Y = (beetleVel[m].Y * (float)(num7 - 1) + y) / (float)num7;
					}
					else if (num6 > 30f)
					{
						num6 = 10f / num6;
						x *= 0f - num6;
						y *= 0f - num6;
						int num8 = 20;
						beetleVel[m].X = (beetleVel[m].X * (float)(num8 - 1) + x) / (float)num8;
						beetleVel[m].Y = (beetleVel[m].Y * (float)(num8 - 1) + y) / (float)num8;
					}
					x = beetleVel[m].X;
					y = beetleVel[m].Y;
					num6 = (float)Math.Sqrt(x * x + y * y);
					if (num6 > 2f)
					{
						beetleVel[m] *= 0.9f;
					}
					beetlePos[m] -= velocity * 0.25f;
				}
			}
			if (head == 14 && ((body >= 58 && body <= 63) || body == 167))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Wizard");
				magicCrit += 10;
			}
			if (head == 159 && ((body >= 58 && body <= 63) || body == 167))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.MagicHat");
				statManaMax2 += 60;
			}
			if ((head == 5 || head == 74) && (body == 5 || body == 48) && (legs == 5 || legs == 44))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.ShadowScale");
				moveSpeed += 0.15f;
			}
			if (head == 57 && body == 37 && legs == 35)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Crimson");
				crimsonRegen = true;
			}
			if (head == 101 && body == 66 && legs == 55)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.SpectreHealing");
				ghostHeal = true;
			}
			if (head == 156 && body == 66 && legs == 55)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.SpectreDamage");
				ghostHurt = true;
			}
			if (head == 6 && body == 6 && legs == 6)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Meteor");
				spaceGun = true;
			}
			if (head == 46 && body == 27 && legs == 26)
			{
				frostArmor = true;
				setBonus = Language.GetTextValue("ArmorSetBonus.Frost");
				frostBurn = true;
			}
			if ((head == 75 || head == 7) && body == 7 && legs == 7)
			{
				boneArmor = true;
				setBonus = Language.GetTextValue("ArmorSetBonus.Bone");
				ammoCost80 = true;
			}
			if ((head == 76 || head == 8) && (body == 49 || body == 8) && (legs == 45 || legs == 8))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Jungle");
				manaCost -= 0.16f;
			}
			if (head == 9 && body == 9 && legs == 9)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Molten");
				meleeDamage += 0.17f;
			}
			if (head == 11 && body == 20 && legs == 19)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Mining");
				pickSpeed -= 0.3f;
			}
			if ((head == 78 || head == 79 || head == 80) && body == 51 && legs == 47)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Chlorophyte");
				AddBuff(60, 18000);
			}
			else if (crystalLeaf)
			{
				for (int n = 0; n < 22; n++)
				{
					if (buffType[n] == 60)
					{
						DelBuff(n);
					}
				}
			}
			if (head == 99 && body == 65 && legs == 54)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Turtle");
				thorns = 1f;
				turtleThorns = true;
			}
			if (body == 17 && legs == 16)
			{
				if (head == 29)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.CobaltCaster");
					manaCost -= 0.14f;
				}
				else if (head == 30)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.CobaltMelee");
					meleeSpeed += 0.15f;
				}
				else if (head == 31)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.CobaltRanged");
					ammoCost80 = true;
				}
			}
			if (body == 18 && legs == 17)
			{
				if (head == 32)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.MythrilCaster");
					manaCost -= 0.17f;
				}
				else if (head == 33)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.MythrilMelee");
					meleeCrit += 5;
				}
				else if (head == 34)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.MythrilRanged");
					ammoCost80 = true;
				}
			}
			if (body == 19 && legs == 18)
			{
				if (head == 35)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteCaster");
					manaCost -= 0.19f;
				}
				else if (head == 36)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteMelee");
					meleeSpeed += 0.18f;
					moveSpeed += 0.18f;
				}
				else if (head == 37)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteRanged");
					ammoCost75 = true;
				}
			}
			if (body == 54 && legs == 49 && (head == 83 || head == 84 || head == 85))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Palladium");
				onHitRegen = true;
			}
			if (body == 55 && legs == 50 && (head == 86 || head == 87 || head == 88))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Orichalcum");
				onHitPetal = true;
			}
			if (body == 56 && legs == 51 && (head == 89 || head == 90 || head == 91))
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Titanium");
				onHitDodge = true;
			}
			if (body == 24 && legs == 23)
			{
				if (head == 42)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.HallowCaster");
					manaCost -= 0.2f;
				}
				else if (head == 43)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.HallowMelee");
					meleeSpeed += 0.19f;
					moveSpeed += 0.19f;
				}
				else if (head == 41)
				{
					setBonus = Language.GetTextValue("ArmorSetBonus.HallowRanged");
					ammoCost75 = true;
				}
			}
			if (head == 82 && body == 53 && legs == 48)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Tiki");
				maxMinions++;
			}
			if (head == 134 && body == 95 && legs == 79)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Spooky");
				minionDamage += 0.25f;
			}
			if (head == 160 && body == 168 && legs == 103)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Bee");
				minionDamage += 0.1f;
				if (itemAnimation > 0 && inventory[selectedItem].type == 1121)
				{
					AchievementsHelper.HandleSpecialEvent(this, 3);
				}
			}
			if (head == 162 && body == 170 && legs == 105)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Spider");
				minionDamage += 0.12f;
			}
			if (head == 171 && body == 177 && legs == 112)
			{
				setSolar = true;
				setBonus = Language.GetTextValue("ArmorSetBonus.Solar");
				solarCounter++;
				int num9 = 240;
				if (solarCounter >= num9)
				{
					if (solarShields > 0 && solarShields < 3)
					{
						for (int num10 = 0; num10 < 22; num10++)
						{
							if (buffType[num10] >= 170 && buffType[num10] <= 171)
							{
								DelBuff(num10);
							}
						}
					}
					if (solarShields < 3)
					{
						AddBuff(170 + solarShields, 5, false);
						for (int num11 = 0; num11 < 16; num11++)
						{
							Dust dust = Main.dust[Dust.NewDust(position, width, height, 6, 0f, 0f, 100)];
							dust.noGravity = true;
							dust.scale = 1.7f;
							dust.fadeIn = 0.5f;
							dust.velocity *= 5f;
							dust.shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
						}
						solarCounter = 0;
					}
					else
					{
						solarCounter = num9;
					}
				}
				for (int num12 = solarShields; num12 < 3; num12++)
				{
					solarShieldPos[num12] = Vector2.Zero;
				}
				for (int num13 = 0; num13 < solarShields; num13++)
				{
					solarShieldPos[num13] += solarShieldVel[num13];
					Vector2 value = ((float)miscCounter / 100f * ((float)Math.PI * 2f) + (float)num13 * ((float)Math.PI * 2f / (float)solarShields)).ToRotationVector2() * 6f;
					value.X = direction * 20;
					solarShieldVel[num13] = (value - solarShieldPos[num13]) * 0.2f;
				}
				if (dashDelay >= 0)
				{
					solarDashing = false;
					solarDashConsumedFlare = false;
				}
				bool flag = solarDashing && dashDelay < 0;
				if (solarShields > 0 || flag)
				{
					dash = 3;
				}
			}
			else
			{
				solarCounter = 0;
			}
			if (head == 169 && body == 175 && legs == 110)
			{
				setVortex = true;
				setBonus = Language.GetTextValue("ArmorSetBonus.Vortex", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
			}
			else
			{
				vortexStealthActive = false;
			}
			if (head == 170 && body == 176 && legs == 111)
			{
				if (nebulaCD > 0)
				{
					nebulaCD--;
				}
				setNebula = true;
				setBonus = Language.GetTextValue("ArmorSetBonus.Nebula");
			}
			if (head == 189 && body == 190 && legs == 130)
			{
				setBonus = Language.GetTextValue("ArmorSetBonus.Stardust", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
				setStardust = true;
				if (whoAmI == Main.myPlayer)
				{
					if (HasBuff(187) == -1)
					{
						AddBuff(187, 3600);
					}
					if (ownedProjectileCounts[623] < 1)
					{
						Projectile.NewProjectile(base.Center.X, base.Center.Y, 0f, -1f, 623, 0, 0f, Main.myPlayer);
					}
				}
			}
			else if (HasBuff(187) != -1)
			{
				DelBuff(HasBuff(187));
			}
		}

		public void UpdateSocialShadow()
		{
			for (int num = 2; num > 0; num--)
			{
				shadowDirection[num] = shadowDirection[num - 1];
			}
			shadowDirection[0] = direction;
			shadowCount++;
			if (shadowCount == 1)
			{
				shadowPos[2] = shadowPos[1];
				shadowRotation[2] = shadowRotation[1];
				shadowOrigin[2] = shadowOrigin[1];
			}
			else if (shadowCount == 2)
			{
				shadowPos[1] = shadowPos[0];
				shadowRotation[1] = shadowRotation[0];
				shadowOrigin[1] = shadowOrigin[0];
			}
			else if (shadowCount >= 3)
			{
				shadowCount = 0;
				shadowPos[0] = position;
				shadowPos[0].Y += gfxOffY;
				shadowRotation[0] = fullRotation;
				shadowOrigin[0] = fullRotationOrigin;
			}
		}

		public void UpdateTeleportVisuals()
		{
			if (!(teleportTime > 0f))
			{
				return;
			}
			if (teleportStyle == 0)
			{
				if ((float)Main.rand.Next(100) <= 100f * teleportTime * 2f)
				{
					int num = Dust.NewDust(new Vector2(getRect().X, getRect().Y), getRect().Width, getRect().Height, 159);
					Main.dust[num].scale = teleportTime * 1.5f;
					Main.dust[num].noGravity = true;
					Main.dust[num].velocity *= 1.1f;
				}
			}
			else if (teleportStyle == 1)
			{
				if ((float)Main.rand.Next(100) <= 100f * teleportTime)
				{
					int num2 = Dust.NewDust(new Vector2(getRect().X, getRect().Y), getRect().Width, getRect().Height, 164);
					Main.dust[num2].scale = teleportTime * 1.5f;
					Main.dust[num2].noGravity = true;
					Main.dust[num2].velocity *= 1.1f;
				}
			}
			else if (teleportStyle == 2)
			{
				teleportTime = 0.005f;
			}
			else if (teleportStyle == 3)
			{
				teleportTime = 0.005f;
			}
			else if (teleportStyle == 4)
			{
				teleportTime -= 0.02f;
				if ((float)Main.rand.Next(100) <= 100f * teleportTime)
				{
					Dust dust = Main.dust[Dust.NewDust(position, width, height, 263)];
					dust.color = PortalHelper.GetPortalColor(lastPortalColorIndex);
					dust.noLight = true;
					dust.noGravity = true;
					dust.scale = 1.2f;
					dust.fadeIn = 0.4f;
				}
			}
			teleportTime -= 0.005f;
		}

		public void UpdateBiomes()
		{
			ZoneDungeon = false;
			if (Main.dungeonTiles >= 250 && (double)position.Y > Main.worldSurface * 16.0)
			{
				int num = (int)position.X / 16;
				int num2 = (int)position.Y / 16;
				if (Main.wallDungeon[Main.tile[num, num2].wall])
				{
					ZoneDungeon = true;
				}
			}
			if (Main.sandTiles > 1000 && position.Y > 3200f)
			{
				Point point = base.Center.ToTileCoordinates();
				Tile tileSafely = Framing.GetTileSafely(point.X, point.Y);
				if (WallID.Sets.Conversion.Sandstone[tileSafely.wall] || WallID.Sets.Conversion.HardenedSand[tileSafely.wall])
				{
					ZoneUndergroundDesert = true;
				}
			}
			else
			{
				ZoneUndergroundDesert = false;
			}
			ZoneCorrupt = (Main.evilTiles >= 200);
			ZoneHoly = (Main.holyTiles >= 100);
			ZoneMeteor = (Main.meteorTiles >= 50);
			ZoneJungle = (Main.jungleTiles >= 80);
			ZoneSnow = (Main.snowTiles >= 300);
			ZoneCrimson = (Main.bloodTiles >= 200);
			ZoneWaterCandle = (Main.waterCandles > 0);
			ZonePeaceCandle = (Main.peaceCandles > 0);
			ZoneDesert = (Main.sandTiles > 1000);
			ZoneGlowshroom = (Main.shroomTiles > 100);
			int num3 = 4000;
			if (Main.maxTilesX == 1750)
			{
				num3 = 2400;
			}
			bool flag2 = ZoneTowerStardust = false;
			bool flag4 = ZoneTowerNebula = flag2;
			bool zoneTowerSolar = ZoneTowerVortex = flag4;
			ZoneTowerSolar = zoneTowerSolar;
			Vector2 value = Vector2.Zero;
			Vector2 value2 = Vector2.Zero;
			Vector2 value3 = Vector2.Zero;
			Vector2 value4 = Vector2.Zero;
			for (int i = 0; i < 200; i++)
			{
				if (!Main.npc[i].active)
				{
					continue;
				}
				if (Main.npc[i].type == 493)
				{
					if (Distance(Main.npc[i].Center) <= (float)num3)
					{
						ZoneTowerStardust = true;
						value4 = Main.npc[i].Center;
					}
				}
				else if (Main.npc[i].type == 507)
				{
					if (Distance(Main.npc[i].Center) <= (float)num3)
					{
						ZoneTowerNebula = true;
						value3 = Main.npc[i].Center;
					}
				}
				else if (Main.npc[i].type == 422)
				{
					if (Distance(Main.npc[i].Center) <= (float)num3)
					{
						ZoneTowerVortex = true;
						value2 = Main.npc[i].Center;
					}
				}
				else if (Main.npc[i].type == 517 && Distance(Main.npc[i].Center) <= (float)num3)
				{
					ZoneTowerSolar = true;
					value = Main.npc[i].Center;
				}
			}
			ManageSpecialBiomeVisuals("Stardust", ZoneTowerStardust, value4 - new Vector2(0f, 10f));
			ManageSpecialBiomeVisuals("Nebula", ZoneTowerNebula, value3 - new Vector2(0f, 10f));
			ManageSpecialBiomeVisuals("Vortex", ZoneTowerVortex, value2 - new Vector2(0f, 10f));
			ManageSpecialBiomeVisuals("Solar", ZoneTowerSolar, value - new Vector2(0f, 10f));
			ManageSpecialBiomeVisuals("MoonLord", NPC.AnyNPCs(398));
			ManageSpecialBiomeVisuals("BloodMoon", Main.bloodMoon);
			if (!dead)
			{
				Point point2 = base.Center.ToTileCoordinates();
				if (WorldGen.InWorld(point2.X, point2.Y, 1))
				{
					int num4 = 0;
					if (Main.tile[point2.X, point2.Y] != null)
					{
						num4 = Main.tile[point2.X, point2.Y].wall;
					}
					switch (num4)
					{
					case 86:
						AchievementsHelper.HandleSpecialEvent(this, 12);
						break;
					case 62:
						AchievementsHelper.HandleSpecialEvent(this, 13);
						break;
					}
				}
				if (_funkytownCheckCD > 0)
				{
					_funkytownCheckCD--;
				}
				if (position.Y / 16f > (float)(Main.maxTilesY - 200))
				{
					AchievementsHelper.HandleSpecialEvent(this, 14);
				}
				else if (_funkytownCheckCD == 0 && (double)(position.Y / 16f) < Main.worldSurface && Main.shroomTiles >= 200)
				{
					AchievementsHelper.HandleSpecialEvent(this, 15);
				}
			}
			else
			{
				_funkytownCheckCD = 100;
			}
		}

		public void ManageSpecialBiomeVisuals(string biomeName, bool inZone, Vector2 activationSource = default(Vector2))
		{
			if (SkyManager.Instance[biomeName] != null && inZone != SkyManager.Instance[biomeName].IsActive())
			{
				if (inZone)
				{
					SkyManager.Instance.Activate(biomeName, activationSource);
				}
				else
				{
					SkyManager.Instance.Deactivate(biomeName);
				}
			}
			if (inZone != Filters.Scene[biomeName].IsActive())
			{
				if (inZone)
				{
					Filters.Scene.Activate(biomeName, activationSource);
				}
				else
				{
					Filters.Scene[biomeName].Deactivate();
				}
			}
			else if (inZone)
			{
				Filters.Scene[biomeName].TargetPosition = activationSource;
			}
		}

		public void UpdateDead()
		{
			_portalPhysicsTime = 0;
			MountFishronSpecialCounter = 0f;
			gem = -1;
			slippy = false;
			slippy2 = false;
			powerrun = false;
			wings = 0;
			wingsLogic = 0;
			face = (neck = (back = (front = (handoff = (handon = (shoe = (waist = (balloon = (shield = 0)))))))));
			poisoned = false;
			venom = false;
			onFire = false;
			dripping = false;
			drippingSlime = false;
			burned = false;
			suffocating = false;
			onFire2 = false;
			onFrostBurn = false;
			blind = false;
			blackout = false;
			loveStruck = false;
			dryadWard = false;
			stinky = false;
			resistCold = false;
			electrified = false;
			moonLeech = false;
			headcovered = false;
			vortexDebuff = false;
			setSolar = (setVortex = (setNebula = (setStardust = false)));
			nebulaLevelDamage = (nebulaLevelLife = (nebulaLevelMana = 0));
			trapDebuffSource = false;
			yoraiz0rEye = 0;
			yoraiz0rDarkness = false;
			gravDir = 1f;
			for (int i = 0; i < 22; i++)
			{
				if (buffType[i] <= 0 || !Main.persistentBuff[buffType[i]])
				{
					buffTime[i] = 0;
					buffType[i] = 0;
				}
			}
			if (whoAmI == Main.myPlayer)
			{
				Main.npcChatText = "";
				Main.editSign = false;
			}
			grappling[0] = -1;
			grappling[1] = -1;
			grappling[2] = -1;
			sign = -1;
			talkNPC = -1;
			Main.npcChatCornerItem = 0;
			statLife = 0;
			channel = false;
			potionDelay = 0;
			chest = -1;
			changeItem = -1;
			itemAnimation = 0;
			immuneAlpha += 2;
			if (immuneAlpha > 255)
			{
				immuneAlpha = 255;
			}
			headPosition += headVelocity;
			bodyPosition += bodyVelocity;
			legPosition += legVelocity;
			headRotation += headVelocity.X * 0.1f;
			bodyRotation += bodyVelocity.X * 0.1f;
			legRotation += legVelocity.X * 0.1f;
			headVelocity.Y += 0.1f;
			bodyVelocity.Y += 0.1f;
			legVelocity.Y += 0.1f;
			headVelocity.X *= 0.99f;
			bodyVelocity.X *= 0.99f;
			legVelocity.X *= 0.99f;
			for (int j = 0; j < npcTypeNoAggro.Length; j++)
			{
				npcTypeNoAggro[j] = false;
			}
			if (difficulty == 2)
			{
				if (respawnTimer > 0)
				{
					respawnTimer--;
				}
				else if (whoAmI == Main.myPlayer || Main.netMode == 2)
				{
					ghost = true;
				}
				return;
			}
			respawnTimer--;
			if (respawnTimer <= 0 && Main.myPlayer == whoAmI)
			{
				if (Main.mouseItem.type > 0)
				{
					Main.playerInventory = true;
				}
				Spawn();
			}
		}

		public void UpdatePet(int i)
		{
			if (i != Main.myPlayer || miscEquips[0].buffType < 1 || miscEquips[0].stack < 1)
			{
				return;
			}
			int num = miscEquips[0].buffType;
			if ((Main.vanityPet[num] || Main.lightPet[num]) && !hideMisc[0] && (miscEquips[0].type != 603 || Main.cEd))
			{
				int num2 = HasBuff(num);
				if (num2 == -1)
				{
					AddBuff(num, 3600);
					Main.PlaySound(2, (int)position.X, (int)position.Y, miscEquips[0].useSound);
				}
			}
		}

		public void UpdatePetLight(int i)
		{
			if (i != Main.myPlayer || miscEquips[1].buffType < 1 || miscEquips[1].stack < 1)
			{
				return;
			}
			int num = miscEquips[1].buffType;
			if ((!Main.vanityPet[num] && !Main.lightPet[num]) || hideMisc[1] || (miscEquips[1].type == 603 && !Main.cEd))
			{
				return;
			}
			int num2 = HasBuff(num);
			if (num == 27 && num2 == -1)
			{
				num2 = HasBuff(102);
			}
			if (num == 27 && num2 == -1)
			{
				num2 = HasBuff(101);
			}
			if (num2 == -1)
			{
				if (num == 27)
				{
					num = Utils.SelectRandom<int>(Main.rand, 27, 102, 101);
				}
				AddBuff(num, 3600);
				Main.PlaySound(2, (int)position.X, (int)position.Y, miscEquips[1].useSound);
			}
		}

		public void TogglePet()
		{
			hideMisc[0] = !hideMisc[0];
			if (hideMisc[0])
			{
				ClearBuff(miscEquips[0].buffType);
			}
		}

		public void ToggleLight()
		{
			hideMisc[1] = !hideMisc[1];
			if (hideMisc[1])
			{
				ClearBuff(miscEquips[1].buffType);
				if (miscEquips[1].buffType == 27)
				{
					ClearBuff(102);
					ClearBuff(101);
				}
			}
		}

		public void SmartCursorLookup()
		{
			if (whoAmI != Main.myPlayer)
			{
				return;
			}
			Main.smartDigShowing = false;
			if (!Main.smartDigEnabled)
			{
				return;
			}
			Item ıtem = inventory[selectedItem];
			Vector2 mouse = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
			if (gravDir == -1f)
			{
				mouse.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
			}
			int num = tileTargetX;
			int num2 = tileTargetY;
			if (num < 10)
			{
				num = 10;
			}
			if (num > Main.maxTilesX - 10)
			{
				num = Main.maxTilesX - 10;
			}
			if (num2 < 10)
			{
				num2 = 10;
			}
			if (num2 > Main.maxTilesY - 10)
			{
				num2 = Main.maxTilesY - 10;
			}
			bool flag = false;
			if (Main.tile[num, num2] == null)
			{
				return;
			}
			if (Main.tile[num, num2].active())
			{
				switch (Main.tile[num, num2].type)
				{
				case 4:
				case 10:
				case 11:
				case 13:
				case 21:
				case 29:
				case 33:
				case 49:
				case 50:
				case 55:
				case 79:
				case 85:
				case 88:
				case 97:
				case 104:
				case 125:
				case 132:
				case 136:
				case 139:
				case 144:
				case 174:
				case 207:
				case 209:
				case 212:
				case 216:
				case 219:
				case 237:
				case 287:
				case 334:
				case 335:
				case 338:
				case 354:
				case 386:
				case 387:
				case 411:
					flag = true;
					break;
				case 314:
					if (gravDir == 1f)
					{
						flag = true;
					}
					break;
				}
			}
			int tileBoost = ıtem.tileBoost;
			int num3 = 0;
			if (ıtem.type == 1071 || ıtem.type == 1543 || ıtem.type == 1072 || ıtem.type == 1544)
			{
				for (int i = 0; i < 58; i++)
				{
					if (inventory[i].stack > 0 && inventory[i].paint > 0)
					{
						num3 = inventory[i].paint;
						break;
					}
				}
			}
			int value = (int)(position.X / 16f) - tileRangeX - tileBoost + 1;
			int value2 = (int)((position.X + (float)width) / 16f) + tileRangeX + tileBoost - 1;
			int value3 = (int)(position.Y / 16f) - tileRangeY - tileBoost + 1;
			int value4 = (int)((position.Y + (float)height) / 16f) + tileRangeY + tileBoost - 2;
			value = Utils.Clamp(value, 10, Main.maxTilesX - 10);
			value2 = Utils.Clamp(value2, 10, Main.maxTilesX - 10);
			value3 = Utils.Clamp(value3, 10, Main.maxTilesY - 10);
			value4 = Utils.Clamp(value4, 10, Main.maxTilesY - 10);
			if (flag && num >= value && num <= value2 && num2 >= value3 && num2 <= value4)
			{
				return;
			}
			List<Tuple<int, int>> list = new List<Tuple<int, int>>();
			for (int j = 0; j < grapCount; j++)
			{
				Projectile projectile = Main.projectile[grappling[j]];
				int item = (int)projectile.Center.X / 16;
				int item2 = (int)projectile.Center.Y / 16;
				list.Add(new Tuple<int, int>(item, item2));
			}
			int fX = -1;
			int fY = -1;
			if (!SmartCursorSettings.SmartAxeAfterPickaxe)
			{
				SmartCursor_Axe(ıtem, ref mouse, value, value2, value3, value4, ref fX, ref fY);
			}
			if (ıtem.pick > 0 && fX == -1 && fY == -1)
			{
				Vector2 vector = mouse - base.Center;
				int num4 = Math.Sign(vector.X);
				int num5 = Math.Sign(vector.Y);
				if (Math.Abs(vector.X) > Math.Abs(vector.Y) * 3f)
				{
					num5 = 0;
					mouse.Y = base.Center.Y;
				}
				if (Math.Abs(vector.Y) > Math.Abs(vector.X) * 3f)
				{
					num4 = 0;
					mouse.X = base.Center.X;
				}
				int num139 = (int)base.Center.X / 16;
				int num140 = (int)base.Center.Y / 16;
				List<Tuple<int, int>> list2 = new List<Tuple<int, int>>();
				List<Tuple<int, int>> list3 = new List<Tuple<int, int>>();
				int num6 = 1;
				if (num5 == -1 && num4 != 0)
				{
					num6 = -1;
				}
				int num7 = (int)((position.X + (float)(width / 2) + (float)((width / 2 - 1) * num4)) / 16f);
				int num8 = (int)(((double)position.Y + 0.1) / 16.0);
				if (num6 == -1)
				{
					num8 = (int)((position.Y + (float)height - 1f) / 16f);
				}
				int num9 = width / 16 + ((width % 16 != 0) ? 1 : 0);
				int num10 = height / 16 + ((height % 16 != 0) ? 1 : 0);
				if (num4 != 0)
				{
					for (int k = 0; k < num10; k++)
					{
						if (Main.tile[num7, num8 + k * num6] == null)
						{
							return;
						}
						list2.Add(new Tuple<int, int>(num7, num8 + k * num6));
					}
				}
				if (num5 != 0)
				{
					for (int l = 0; l < num9; l++)
					{
						if (Main.tile[(int)(position.X / 16f) + l, num8] == null)
						{
							return;
						}
						list2.Add(new Tuple<int, int>((int)(position.X / 16f) + l, num8));
					}
				}
				int num11 = (int)((mouse.X + (float)((width / 2 - 1) * num4)) / 16f);
				int num12 = (int)(((double)mouse.Y + 0.1 - (double)(height / 2 + 1)) / 16.0);
				if (num6 == -1)
				{
					num12 = (int)((mouse.Y + (float)(height / 2) - 1f) / 16f);
				}
				if (gravDir == -1f && num5 == 0)
				{
					num12++;
				}
				if (num12 < 10)
				{
					num12 = 10;
				}
				if (num12 > Main.maxTilesY - 10)
				{
					num12 = Main.maxTilesY - 10;
				}
				int num13 = width / 16 + ((width % 16 != 0) ? 1 : 0);
				int num14 = height / 16 + ((height % 16 != 0) ? 1 : 0);
				if (num4 != 0)
				{
					for (int m = 0; m < num14; m++)
					{
						if (Main.tile[num11, num12 + m * num6] == null)
						{
							return;
						}
						list3.Add(new Tuple<int, int>(num11, num12 + m * num6));
					}
				}
				if (num5 != 0)
				{
					for (int n = 0; n < num13; n++)
					{
						if (Main.tile[(int)((mouse.X - (float)(width / 2)) / 16f) + n, num12] == null)
						{
							return;
						}
						list3.Add(new Tuple<int, int>((int)((mouse.X - (float)(width / 2)) / 16f) + n, num12));
					}
				}
				List<Tuple<int, int>> list4 = new List<Tuple<int, int>>();
				while (list2.Count > 0)
				{
					Tuple<int, int> tuple = list2[0];
					Tuple<int, int> tuple2 = list3[0];
					Tuple<int, int> col;
					if (!Collision.TupleHitLine(tuple.Item1, tuple.Item2, tuple2.Item1, tuple2.Item2, num4 * (int)gravDir, -num5 * (int)gravDir, list, out col))
					{
						list2.Remove(tuple);
						list3.Remove(tuple2);
						continue;
					}
					if (col.Item1 != tuple2.Item1 || col.Item2 != tuple2.Item2)
					{
						list4.Add(col);
					}
					Tile tile = Main.tile[col.Item1, col.Item2];
					if (!tile.inActive() && tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type] && !list.Contains(col))
					{
						list4.Add(col);
					}
					list2.Remove(tuple);
					list3.Remove(tuple2);
				}
				List<Tuple<int, int>> list5 = new List<Tuple<int, int>>();
				for (int num15 = 0; num15 < list4.Count; num15++)
				{
					if (!WorldGen.CanKillTile(list4[num15].Item1, list4[num15].Item2))
					{
						list5.Add(list4[num15]);
					}
				}
				for (int num16 = 0; num16 < list5.Count; num16++)
				{
					list4.Remove(list5[num16]);
				}
				list5.Clear();
				if (list4.Count > 0)
				{
					float num17 = -1f;
					Tuple<int, int> tuple3 = list4[0];
					for (int num18 = 0; num18 < list4.Count; num18++)
					{
						float num19 = Vector2.Distance(new Vector2(list4[num18].Item1, list4[num18].Item2) * 16f + Vector2.One * 8f, base.Center);
						if (num17 == -1f || num19 < num17)
						{
							num17 = num19;
							tuple3 = list4[num18];
						}
					}
					if (Collision.InTileBounds(tuple3.Item1, tuple3.Item2, value, value3, value2, value4))
					{
						fX = tuple3.Item1;
						fY = tuple3.Item2;
					}
				}
				list2.Clear();
				list3.Clear();
				list4.Clear();
			}
			if (SmartCursorSettings.SmartAxeAfterPickaxe)
			{
				SmartCursor_Axe(ıtem, ref mouse, value, value2, value3, value4, ref fX, ref fY);
			}
			if ((ıtem.type == 509 || ıtem.type == 850 || ıtem.type == 851) && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list6 = new List<Tuple<int, int>>();
				int num20 = 0;
				if (ıtem.type == 509)
				{
					num20 = 1;
				}
				if (ıtem.type == 850)
				{
					num20 = 2;
				}
				if (ıtem.type == 851)
				{
					num20 = 3;
				}
				bool flag2 = false;
				if (Main.tile[num, num2].wire() && num20 == 1)
				{
					flag2 = true;
				}
				if (Main.tile[num, num2].wire2() && num20 == 2)
				{
					flag2 = true;
				}
				if (Main.tile[num, num2].wire3() && num20 == 3)
				{
					flag2 = true;
				}
				if (!flag2)
				{
					for (int num21 = value; num21 <= value2; num21++)
					{
						for (int num22 = value3; num22 <= value4; num22++)
						{
							Tile tile2 = Main.tile[num21, num22];
							if ((!tile2.wire() || num20 != 1) && (!tile2.wire2() || num20 != 2) && (!tile2.wire3() || num20 != 3))
							{
								continue;
							}
							if (num20 == 1)
							{
								if (!Main.tile[num21 - 1, num22].wire())
								{
									list6.Add(new Tuple<int, int>(num21 - 1, num22));
								}
								if (!Main.tile[num21 + 1, num22].wire())
								{
									list6.Add(new Tuple<int, int>(num21 + 1, num22));
								}
								if (!Main.tile[num21, num22 - 1].wire())
								{
									list6.Add(new Tuple<int, int>(num21, num22 - 1));
								}
								if (!Main.tile[num21, num22 + 1].wire())
								{
									list6.Add(new Tuple<int, int>(num21, num22 + 1));
								}
							}
							if (num20 == 2)
							{
								if (!Main.tile[num21 - 1, num22].wire2())
								{
									list6.Add(new Tuple<int, int>(num21 - 1, num22));
								}
								if (!Main.tile[num21 + 1, num22].wire2())
								{
									list6.Add(new Tuple<int, int>(num21 + 1, num22));
								}
								if (!Main.tile[num21, num22 - 1].wire2())
								{
									list6.Add(new Tuple<int, int>(num21, num22 - 1));
								}
								if (!Main.tile[num21, num22 + 1].wire2())
								{
									list6.Add(new Tuple<int, int>(num21, num22 + 1));
								}
							}
							if (num20 == 3)
							{
								if (!Main.tile[num21 - 1, num22].wire3())
								{
									list6.Add(new Tuple<int, int>(num21 - 1, num22));
								}
								if (!Main.tile[num21 + 1, num22].wire3())
								{
									list6.Add(new Tuple<int, int>(num21 + 1, num22));
								}
								if (!Main.tile[num21, num22 - 1].wire3())
								{
									list6.Add(new Tuple<int, int>(num21, num22 - 1));
								}
								if (!Main.tile[num21, num22 + 1].wire3())
								{
									list6.Add(new Tuple<int, int>(num21, num22 + 1));
								}
							}
						}
					}
				}
				if (list6.Count > 0)
				{
					float num23 = -1f;
					Tuple<int, int> tuple4 = list6[0];
					for (int num24 = 0; num24 < list6.Count; num24++)
					{
						float num25 = Vector2.Distance(new Vector2(list6[num24].Item1, list6[num24].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num23 == -1f || num25 < num23)
						{
							num23 = num25;
							tuple4 = list6[num24];
						}
					}
					if (Collision.InTileBounds(tuple4.Item1, tuple4.Item2, value, value3, value2, value4))
					{
						fX = tuple4.Item1;
						fY = tuple4.Item2;
					}
				}
				list6.Clear();
			}
			if (ıtem.hammer > 0 && fX == -1 && fY == -1)
			{
				Vector2 vector2 = mouse - base.Center;
				int num26 = Math.Sign(vector2.X);
				int num27 = Math.Sign(vector2.Y);
				if (Math.Abs(vector2.X) > Math.Abs(vector2.Y) * 3f)
				{
					num27 = 0;
					mouse.Y = base.Center.Y;
				}
				if (Math.Abs(vector2.Y) > Math.Abs(vector2.X) * 3f)
				{
					num26 = 0;
					mouse.X = base.Center.X;
				}
				int num141 = (int)base.Center.X / 16;
				int num142 = (int)base.Center.Y / 16;
				List<Tuple<int, int>> list7 = new List<Tuple<int, int>>();
				List<Tuple<int, int>> list8 = new List<Tuple<int, int>>();
				int num28 = 1;
				if (num27 == -1 && num26 != 0)
				{
					num28 = -1;
				}
				int num29 = (int)((position.X + (float)(width / 2) + (float)((width / 2 - 1) * num26)) / 16f);
				int num30 = (int)(((double)position.Y + 0.1) / 16.0);
				if (num28 == -1)
				{
					num30 = (int)((position.Y + (float)height - 1f) / 16f);
				}
				int num31 = width / 16 + ((width % 16 != 0) ? 1 : 0);
				int num32 = height / 16 + ((height % 16 != 0) ? 1 : 0);
				if (num26 != 0)
				{
					for (int num33 = 0; num33 < num32; num33++)
					{
						if (Main.tile[num29, num30 + num33 * num28] == null)
						{
							return;
						}
						list7.Add(new Tuple<int, int>(num29, num30 + num33 * num28));
					}
				}
				if (num27 != 0)
				{
					for (int num34 = 0; num34 < num31; num34++)
					{
						if (Main.tile[(int)(position.X / 16f) + num34, num30] == null)
						{
							return;
						}
						list7.Add(new Tuple<int, int>((int)(position.X / 16f) + num34, num30));
					}
				}
				int num35 = (int)((mouse.X + (float)((width / 2 - 1) * num26)) / 16f);
				int num36 = (int)(((double)mouse.Y + 0.1 - (double)(height / 2 + 1)) / 16.0);
				if (num28 == -1)
				{
					num36 = (int)((mouse.Y + (float)(height / 2) - 1f) / 16f);
				}
				if (gravDir == -1f && num27 == 0)
				{
					num36++;
				}
				if (num36 < 10)
				{
					num36 = 10;
				}
				if (num36 > Main.maxTilesY - 10)
				{
					num36 = Main.maxTilesY - 10;
				}
				int num37 = width / 16 + ((width % 16 != 0) ? 1 : 0);
				int num38 = height / 16 + ((height % 16 != 0) ? 1 : 0);
				if (num26 != 0)
				{
					for (int num39 = 0; num39 < num38; num39++)
					{
						if (Main.tile[num35, num36 + num39 * num28] == null)
						{
							return;
						}
						list8.Add(new Tuple<int, int>(num35, num36 + num39 * num28));
					}
				}
				if (num27 != 0)
				{
					for (int num40 = 0; num40 < num37; num40++)
					{
						if (Main.tile[(int)((mouse.X - (float)(width / 2)) / 16f) + num40, num36] == null)
						{
							return;
						}
						list8.Add(new Tuple<int, int>((int)((mouse.X - (float)(width / 2)) / 16f) + num40, num36));
					}
				}
				List<Tuple<int, int>> list9 = new List<Tuple<int, int>>();
				while (list7.Count > 0)
				{
					Tuple<int, int> tuple5 = list7[0];
					Tuple<int, int> tuple6 = list8[0];
					Tuple<int, int> tuple7 = Collision.TupleHitLineWall(tuple5.Item1, tuple5.Item2, tuple6.Item1, tuple6.Item2);
					if (tuple7.Item1 == -1 || tuple7.Item2 == -1)
					{
						list7.Remove(tuple5);
						list8.Remove(tuple6);
						continue;
					}
					if (tuple7.Item1 != tuple6.Item1 || tuple7.Item2 != tuple6.Item2)
					{
						list9.Add(tuple7);
					}
					Tile tile29 = Main.tile[tuple7.Item1, tuple7.Item2];
					if (Collision.HitWallSubstep(tuple7.Item1, tuple7.Item2))
					{
						list9.Add(tuple7);
					}
					list7.Remove(tuple5);
					list8.Remove(tuple6);
				}
				if (list9.Count > 0)
				{
					float num41 = -1f;
					Tuple<int, int> tuple8 = list9[0];
					for (int num42 = 0; num42 < list9.Count; num42++)
					{
						float num43 = Vector2.Distance(new Vector2(list9[num42].Item1, list9[num42].Item2) * 16f + Vector2.One * 8f, base.Center);
						if (num41 == -1f || num43 < num41)
						{
							num41 = num43;
							tuple8 = list9[num42];
						}
					}
					if (Collision.InTileBounds(tuple8.Item1, tuple8.Item2, value, value3, value2, value4))
					{
						poundRelease = false;
						fX = tuple8.Item1;
						fY = tuple8.Item2;
					}
				}
				list9.Clear();
				list7.Clear();
				list8.Clear();
			}
			if (ıtem.hammer > 0 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list10 = new List<Tuple<int, int>>();
				for (int num44 = value; num44 <= value2; num44++)
				{
					for (int num45 = value3; num45 <= value4; num45++)
					{
						if (Main.tile[num44, num45].wall > 0 && Collision.HitWallSubstep(num44, num45))
						{
							list10.Add(new Tuple<int, int>(num44, num45));
						}
					}
				}
				if (list10.Count > 0)
				{
					float num46 = -1f;
					Tuple<int, int> tuple9 = list10[0];
					for (int num47 = 0; num47 < list10.Count; num47++)
					{
						float num48 = Vector2.Distance(new Vector2(list10[num47].Item1, list10[num47].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num46 == -1f || num48 < num46)
						{
							num46 = num48;
							tuple9 = list10[num47];
						}
					}
					if (Collision.InTileBounds(tuple9.Item1, tuple9.Item2, value, value3, value2, value4))
					{
						poundRelease = false;
						fX = tuple9.Item1;
						fY = tuple9.Item2;
					}
				}
				list10.Clear();
			}
			if (ıtem.type == 510 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list11 = new List<Tuple<int, int>>();
				for (int num49 = value; num49 <= value2; num49++)
				{
					for (int num50 = value3; num50 <= value4; num50++)
					{
						Tile tile3 = Main.tile[num49, num50];
						if (tile3.wire() || tile3.wire2() || tile3.wire3())
						{
							list11.Add(new Tuple<int, int>(num49, num50));
						}
					}
				}
				if (list11.Count > 0)
				{
					float num51 = -1f;
					Tuple<int, int> tuple10 = list11[0];
					for (int num52 = 0; num52 < list11.Count; num52++)
					{
						float num53 = Vector2.Distance(new Vector2(list11[num52].Item1, list11[num52].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num51 == -1f || num53 < num51)
						{
							num51 = num53;
							tuple10 = list11[num52];
						}
					}
					if (Collision.InTileBounds(tuple10.Item1, tuple10.Item2, value, value3, value2, value4))
					{
						fX = tuple10.Item1;
						fY = tuple10.Item2;
					}
				}
				list11.Clear();
			}
			if (ıtem.createTile == 19 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list12 = new List<Tuple<int, int>>();
				bool flag3 = false;
				if (Main.tile[num, num2].active() && Main.tile[num, num2].type == 19)
				{
					flag3 = true;
				}
				if (!flag3)
				{
					for (int num54 = value; num54 <= value2; num54++)
					{
						for (int num55 = value3; num55 <= value4; num55++)
						{
							Tile tile4 = Main.tile[num54, num55];
							if (tile4.active() && tile4.type == 19)
							{
								int num56 = tile4.slope();
								if (num56 != 2 && !Main.tile[num54 - 1, num55 - 1].active())
								{
									list12.Add(new Tuple<int, int>(num54 - 1, num55 - 1));
								}
								if (!Main.tile[num54 - 1, num55].active())
								{
									list12.Add(new Tuple<int, int>(num54 - 1, num55));
								}
								if (num56 != 1 && !Main.tile[num54 - 1, num55 + 1].active())
								{
									list12.Add(new Tuple<int, int>(num54 - 1, num55 + 1));
								}
								if (num56 != 1 && !Main.tile[num54 + 1, num55 - 1].active())
								{
									list12.Add(new Tuple<int, int>(num54 + 1, num55 - 1));
								}
								if (!Main.tile[num54 + 1, num55].active())
								{
									list12.Add(new Tuple<int, int>(num54 + 1, num55));
								}
								if (num56 != 2 && !Main.tile[num54 + 1, num55 + 1].active())
								{
									list12.Add(new Tuple<int, int>(num54 + 1, num55 + 1));
								}
							}
							if (!tile4.active())
							{
								int num57 = 0;
								int num58 = 0;
								num57 = 0;
								num58 = 1;
								Tile tile5 = Main.tile[num54 + num57, num55 + num58];
								if (tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type])
								{
									list12.Add(new Tuple<int, int>(num54, num55));
								}
								num57 = -1;
								num58 = 0;
								tile5 = Main.tile[num54 + num57, num55 + num58];
								if (tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type])
								{
									list12.Add(new Tuple<int, int>(num54, num55));
								}
								num57 = 1;
								num58 = 0;
								tile5 = Main.tile[num54 + num57, num55 + num58];
								if (tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type])
								{
									list12.Add(new Tuple<int, int>(num54, num55));
								}
							}
						}
					}
				}
				if (list12.Count > 0)
				{
					float num59 = -1f;
					Tuple<int, int> tuple11 = list12[0];
					for (int num60 = 0; num60 < list12.Count; num60++)
					{
						float num61 = Vector2.Distance(new Vector2(list12[num60].Item1, list12[num60].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num59 == -1f || num61 < num59)
						{
							num59 = num61;
							tuple11 = list12[num60];
						}
					}
					if (Collision.InTileBounds(tuple11.Item1, tuple11.Item2, value, value3, value2, value4))
					{
						fX = tuple11.Item1;
						fY = tuple11.Item2;
					}
				}
				list12.Clear();
			}
			if ((ıtem.type == 2340 || ıtem.type == 2739) && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list13 = new List<Tuple<int, int>>();
				bool flag4 = false;
				if (Main.tile[num, num2].active() && Main.tile[num, num2].type == 314)
				{
					flag4 = true;
				}
				if (!flag4)
				{
					for (int num62 = value; num62 <= value2; num62++)
					{
						for (int num63 = value3; num63 <= value4; num63++)
						{
							Tile tile6 = Main.tile[num62, num63];
							if (tile6.active() && tile6.type == 314)
							{
								bool flag5 = Main.tile[num62 + 1, num63 + 1].active() && Main.tile[num62 + 1, num63 + 1].type == 314;
								bool flag6 = Main.tile[num62 + 1, num63 - 1].active() && Main.tile[num62 + 1, num63 - 1].type == 314;
								bool flag7 = Main.tile[num62 - 1, num63 + 1].active() && Main.tile[num62 - 1, num63 + 1].type == 314;
								bool flag8 = Main.tile[num62 - 1, num63 - 1].active() && Main.tile[num62 - 1, num63 - 1].type == 314;
								if ((!Main.tile[num62 - 1, num63 - 1].active() || Main.tileCut[Main.tile[num62 - 1, num63 - 1].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num62 - 1, num63 - 1].type]) && (flag5 || !flag6))
								{
									list13.Add(new Tuple<int, int>(num62 - 1, num63 - 1));
								}
								if (!Main.tile[num62 - 1, num63].active() || Main.tileCut[Main.tile[num62 - 1, num63].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num62 - 1, num63].type])
								{
									list13.Add(new Tuple<int, int>(num62 - 1, num63));
								}
								if ((!Main.tile[num62 - 1, num63 + 1].active() || Main.tileCut[Main.tile[num62 - 1, num63 + 1].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num62 - 1, num63 + 1].type]) && (flag6 || !flag5))
								{
									list13.Add(new Tuple<int, int>(num62 - 1, num63 + 1));
								}
								if ((!Main.tile[num62 + 1, num63 - 1].active() || Main.tileCut[Main.tile[num62 + 1, num63 - 1].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num62 + 1, num63 - 1].type]) && (flag7 || !flag8))
								{
									list13.Add(new Tuple<int, int>(num62 + 1, num63 - 1));
								}
								if (!Main.tile[num62 + 1, num63].active() || Main.tileCut[Main.tile[num62 + 1, num63].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num62 + 1, num63].type])
								{
									list13.Add(new Tuple<int, int>(num62 + 1, num63));
								}
								if ((!Main.tile[num62 + 1, num63 + 1].active() || Main.tileCut[Main.tile[num62 + 1, num63 + 1].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num62 + 1, num63 + 1].type]) && (flag8 || !flag7))
								{
									list13.Add(new Tuple<int, int>(num62 + 1, num63 + 1));
								}
							}
						}
					}
				}
				if (list13.Count > 0)
				{
					float num64 = -1f;
					Tuple<int, int> tuple12 = list13[0];
					for (int num65 = 0; num65 < list13.Count; num65++)
					{
						if ((!Main.tile[list13[num65].Item1, list13[num65].Item2 - 1].active() || Main.tile[list13[num65].Item1, list13[num65].Item2 - 1].type != 314) && (!Main.tile[list13[num65].Item1, list13[num65].Item2 + 1].active() || Main.tile[list13[num65].Item1, list13[num65].Item2 + 1].type != 314))
						{
							float num66 = Vector2.Distance(new Vector2(list13[num65].Item1, list13[num65].Item2) * 16f + Vector2.One * 8f, mouse);
							if (num64 == -1f || num66 < num64)
							{
								num64 = num66;
								tuple12 = list13[num65];
							}
						}
					}
					if (Collision.InTileBounds(tuple12.Item1, tuple12.Item2, value, value3, value2, value4) && num64 != -1f)
					{
						fX = tuple12.Item1;
						fY = tuple12.Item2;
					}
				}
				list13.Clear();
			}
			if (ıtem.type == 2492 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list14 = new List<Tuple<int, int>>();
				bool flag9 = false;
				if (Main.tile[num, num2].active() && Main.tile[num, num2].type == 314)
				{
					flag9 = true;
				}
				if (!flag9)
				{
					for (int num67 = value; num67 <= value2; num67++)
					{
						for (int num68 = value3; num68 <= value4; num68++)
						{
							Tile tile7 = Main.tile[num67, num68];
							if (tile7.active() && tile7.type == 314)
							{
								if (!Main.tile[num67 - 1, num68].active() || Main.tileCut[Main.tile[num67 - 1, num68].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num67 - 1, num68].type])
								{
									list14.Add(new Tuple<int, int>(num67 - 1, num68));
								}
								if (!Main.tile[num67 + 1, num68].active() || Main.tileCut[Main.tile[num67 + 1, num68].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num67 + 1, num68].type])
								{
									list14.Add(new Tuple<int, int>(num67 + 1, num68));
								}
							}
						}
					}
				}
				if (list14.Count > 0)
				{
					float num69 = -1f;
					Tuple<int, int> tuple13 = list14[0];
					for (int num70 = 0; num70 < list14.Count; num70++)
					{
						if ((!Main.tile[list14[num70].Item1, list14[num70].Item2 - 1].active() || Main.tile[list14[num70].Item1, list14[num70].Item2 - 1].type != 314) && (!Main.tile[list14[num70].Item1, list14[num70].Item2 + 1].active() || Main.tile[list14[num70].Item1, list14[num70].Item2 + 1].type != 314))
						{
							float num71 = Vector2.Distance(new Vector2(list14[num70].Item1, list14[num70].Item2) * 16f + Vector2.One * 8f, mouse);
							if (num69 == -1f || num71 < num69)
							{
								num69 = num71;
								tuple13 = list14[num70];
							}
						}
					}
					if (Collision.InTileBounds(tuple13.Item1, tuple13.Item2, value, value3, value2, value4) && num69 != -1f)
					{
						fX = tuple13.Item1;
						fY = tuple13.Item2;
					}
				}
				list14.Clear();
			}
			if (ıtem.createWall > 0 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list15 = new List<Tuple<int, int>>();
				for (int num72 = value; num72 <= value2; num72++)
				{
					for (int num73 = value3; num73 <= value4; num73++)
					{
						Tile tile8 = Main.tile[num72, num73];
						if (tile8.wall == 0 && (!tile8.active() || !Main.tileSolid[tile8.type] || Main.tileSolidTop[tile8.type]) && Collision.CanHitWithCheck(position, width, height, new Vector2(num72, num73) * 16f, 16, 16, DelegateMethods.NotDoorStand))
						{
							bool flag10 = false;
							if (Main.tile[num72 - 1, num73].active() || Main.tile[num72 - 1, num73].wall > 0)
							{
								flag10 = true;
							}
							if (Main.tile[num72 + 1, num73].active() || Main.tile[num72 + 1, num73].wall > 0)
							{
								flag10 = true;
							}
							if (Main.tile[num72, num73 - 1].active() || Main.tile[num72, num73 - 1].wall > 0)
							{
								flag10 = true;
							}
							if (Main.tile[num72, num73 + 1].active() || Main.tile[num72, num73 + 1].wall > 0)
							{
								flag10 = true;
							}
							if (Main.tile[num72, num73].active() && Main.tile[num72, num73].type == 11 && (Main.tile[num72, num73].frameX < 18 || Main.tile[num72, num73].frameX >= 54))
							{
								flag10 = false;
							}
							if (flag10)
							{
								list15.Add(new Tuple<int, int>(num72, num73));
							}
						}
					}
				}
				if (list15.Count > 0)
				{
					float num74 = -1f;
					Tuple<int, int> tuple14 = list15[0];
					for (int num75 = 0; num75 < list15.Count; num75++)
					{
						float num76 = Vector2.Distance(new Vector2(list15[num75].Item1, list15[num75].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num74 == -1f || num76 < num74)
						{
							num74 = num76;
							tuple14 = list15[num75];
						}
					}
					if (Collision.InTileBounds(tuple14.Item1, tuple14.Item2, value, value3, value2, value4))
					{
						fX = tuple14.Item1;
						fY = tuple14.Item2;
					}
				}
				list15.Clear();
			}
			if (SmartCursorSettings.SmartBlocksEnabled && ıtem.createTile > -1 && ıtem.type != 213 && Main.tileSolid[ıtem.createTile] && !Main.tileSolidTop[ıtem.createTile] && !Main.tileFrameImportant[ıtem.createTile] && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list16 = new List<Tuple<int, int>>();
				bool flag11 = false;
				if (Main.tile[num, num2].active())
				{
					flag11 = true;
				}
				if (!Collision.InTileBounds(num, num2, value, value3, value2, value4))
				{
					flag11 = true;
				}
				if (!flag11)
				{
					for (int num77 = value; num77 <= value2; num77++)
					{
						for (int num78 = value3; num78 <= value4; num78++)
						{
							Tile tile9 = Main.tile[num77, num78];
							if (!tile9.active() || Main.tileCut[tile9.type] || TileID.Sets.BreakableWhenPlacing[tile9.type])
							{
								bool flag12 = false;
								if (Main.tile[num77 - 1, num78].active() && Main.tileSolid[Main.tile[num77 - 1, num78].type] && !Main.tileSolidTop[Main.tile[num77 - 1, num78].type])
								{
									flag12 = true;
								}
								if (Main.tile[num77 + 1, num78].active() && Main.tileSolid[Main.tile[num77 + 1, num78].type] && !Main.tileSolidTop[Main.tile[num77 + 1, num78].type])
								{
									flag12 = true;
								}
								if (Main.tile[num77, num78 - 1].active() && Main.tileSolid[Main.tile[num77, num78 - 1].type] && !Main.tileSolidTop[Main.tile[num77, num78 - 1].type])
								{
									flag12 = true;
								}
								if (Main.tile[num77, num78 + 1].active() && Main.tileSolid[Main.tile[num77, num78 + 1].type] && !Main.tileSolidTop[Main.tile[num77, num78 + 1].type])
								{
									flag12 = true;
								}
								if (flag12)
								{
									list16.Add(new Tuple<int, int>(num77, num78));
								}
							}
						}
					}
				}
				if (list16.Count > 0)
				{
					float num79 = -1f;
					Tuple<int, int> tuple15 = list16[0];
					for (int num80 = 0; num80 < list16.Count; num80++)
					{
						if (Collision.EmptyTile(list16[num80].Item1, list16[num80].Item2))
						{
							float num81 = Vector2.Distance(new Vector2(list16[num80].Item1, list16[num80].Item2) * 16f + Vector2.One * 8f, mouse);
							if (num79 == -1f || num81 < num79)
							{
								num79 = num81;
								tuple15 = list16[num80];
							}
						}
					}
					if (Collision.InTileBounds(tuple15.Item1, tuple15.Item2, value, value3, value2, value4) && num79 != -1f)
					{
						fX = tuple15.Item1;
						fY = tuple15.Item2;
					}
				}
				list16.Clear();
			}
			if ((ıtem.type == 1072 || ıtem.type == 1544) && num3 != 0 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list17 = new List<Tuple<int, int>>();
				for (int num82 = value; num82 <= value2; num82++)
				{
					for (int num83 = value3; num83 <= value4; num83++)
					{
						Tile tile10 = Main.tile[num82, num83];
						if (tile10.wall > 0 && tile10.wallColor() != num3 && (!tile10.active() || !Main.tileSolid[tile10.type] || Main.tileSolidTop[tile10.type]))
						{
							list17.Add(new Tuple<int, int>(num82, num83));
						}
					}
				}
				if (list17.Count > 0)
				{
					float num84 = -1f;
					Tuple<int, int> tuple16 = list17[0];
					for (int num85 = 0; num85 < list17.Count; num85++)
					{
						float num86 = Vector2.Distance(new Vector2(list17[num85].Item1, list17[num85].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num84 == -1f || num86 < num84)
						{
							num84 = num86;
							tuple16 = list17[num85];
						}
					}
					if (Collision.InTileBounds(tuple16.Item1, tuple16.Item2, value, value3, value2, value4))
					{
						fX = tuple16.Item1;
						fY = tuple16.Item2;
					}
				}
				list17.Clear();
			}
			if ((ıtem.type == 1071 || ıtem.type == 1543) && num3 != 0 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list18 = new List<Tuple<int, int>>();
				for (int num87 = value; num87 <= value2; num87++)
				{
					for (int num88 = value3; num88 <= value4; num88++)
					{
						Tile tile11 = Main.tile[num87, num88];
						if (tile11.active() && tile11.color() != num3)
						{
							list18.Add(new Tuple<int, int>(num87, num88));
						}
					}
				}
				if (list18.Count > 0)
				{
					float num89 = -1f;
					Tuple<int, int> tuple17 = list18[0];
					for (int num90 = 0; num90 < list18.Count; num90++)
					{
						float num91 = Vector2.Distance(new Vector2(list18[num90].Item1, list18[num90].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num89 == -1f || num91 < num89)
						{
							num89 = num91;
							tuple17 = list18[num90];
						}
					}
					if (Collision.InTileBounds(tuple17.Item1, tuple17.Item2, value, value3, value2, value4))
					{
						fX = tuple17.Item1;
						fY = tuple17.Item2;
					}
				}
				list18.Clear();
			}
			if ((ıtem.type == 1100 || ıtem.type == 1545) && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list19 = new List<Tuple<int, int>>();
				for (int num92 = value; num92 <= value2; num92++)
				{
					for (int num93 = value3; num93 <= value4; num93++)
					{
						Tile tile12 = Main.tile[num92, num93];
						if ((tile12.active() && tile12.color() > 0) || (tile12.wall > 0 && tile12.wallColor() > 0))
						{
							list19.Add(new Tuple<int, int>(num92, num93));
						}
					}
				}
				if (list19.Count > 0)
				{
					float num94 = -1f;
					Tuple<int, int> tuple18 = list19[0];
					for (int num95 = 0; num95 < list19.Count; num95++)
					{
						float num96 = Vector2.Distance(new Vector2(list19[num95].Item1, list19[num95].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num94 == -1f || num96 < num94)
						{
							num94 = num96;
							tuple18 = list19[num95];
						}
					}
					if (Collision.InTileBounds(tuple18.Item1, tuple18.Item2, value, value3, value2, value4))
					{
						fX = tuple18.Item1;
						fY = tuple18.Item2;
					}
				}
				list19.Clear();
			}
			if (ıtem.type == 27 && fX == -1 && fY == -1 && value3 > 20)
			{
				List<Tuple<int, int>> list20 = new List<Tuple<int, int>>();
				for (int num97 = value; num97 <= value2; num97++)
				{
					for (int num98 = value3; num98 <= value4; num98++)
					{
						Tile tile13 = Main.tile[num97, num98];
						Tile tile14 = Main.tile[num97, num98 - 1];
						Tile tile15 = Main.tile[num97, num98 + 1];
						Tile tile16 = Main.tile[num97 - 1, num98];
						Tile tile17 = Main.tile[num97 + 1, num98];
						Tile tile18 = Main.tile[num97 - 2, num98];
						Tile tile19 = Main.tile[num97 + 2, num98];
						if ((tile13.active() && !Main.tileCut[tile13.type] && !TileID.Sets.BreakableWhenPlacing[tile13.type]) || (tile14.active() && !Main.tileCut[tile14.type] && !TileID.Sets.BreakableWhenPlacing[tile14.type]) || (tile16.active() && tile16.type == 20) || (tile17.active() && tile17.type == 20) || (tile18.active() && tile18.type == 20) || (tile19.active() && tile19.type == 20) || !tile15.active() || !WorldGen.SolidTile2(tile15))
						{
							continue;
						}
						switch (tile15.type)
						{
						case 60:
							if (WorldGen.EmptyTileCheck(num97 - 2, num97 + 2, num98 - 20, num98, 20))
							{
								list20.Add(new Tuple<int, int>(num97, num98));
							}
							break;
						case 2:
						case 23:
						case 53:
						case 109:
						case 112:
						case 116:
						case 147:
						case 199:
						case 234:
							if (tile16.liquid == 0 && tile13.liquid == 0 && tile17.liquid == 0 && WorldGen.EmptyTileCheck(num97 - 2, num97 + 2, num98 - 20, num98, 20))
							{
								list20.Add(new Tuple<int, int>(num97, num98));
							}
							break;
						}
					}
				}
				List<Tuple<int, int>> list21 = new List<Tuple<int, int>>();
				for (int num99 = 0; num99 < list20.Count; num99++)
				{
					bool flag13 = false;
					for (int num100 = -1; num100 < 2; num100 += 2)
					{
						Tile tile20 = Main.tile[list20[num99].Item1 + num100, list20[num99].Item2 + 1];
						if (tile20.active())
						{
							switch (tile20.type)
							{
							case 2:
							case 23:
							case 53:
							case 60:
							case 109:
							case 112:
							case 116:
							case 147:
							case 199:
							case 234:
								flag13 = true;
								break;
							}
						}
					}
					if (!flag13)
					{
						list21.Add(list20[num99]);
					}
				}
				for (int num101 = 0; num101 < list21.Count; num101++)
				{
					list20.Remove(list21[num101]);
				}
				list21.Clear();
				if (list20.Count > 0)
				{
					float num102 = -1f;
					Tuple<int, int> tuple19 = list20[0];
					for (int num103 = 0; num103 < list20.Count; num103++)
					{
						float num104 = Vector2.Distance(new Vector2(list20[num103].Item1, list20[num103].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num102 == -1f || num104 < num102)
						{
							num102 = num104;
							tuple19 = list20[num103];
						}
					}
					if (Collision.InTileBounds(tuple19.Item1, tuple19.Item2, value, value3, value2, value4))
					{
						fX = tuple19.Item1;
						fY = tuple19.Item2;
					}
				}
				list20.Clear();
			}
			if (ıtem.type == 205 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list22 = new List<Tuple<int, int>>();
				for (int num105 = value; num105 <= value2; num105++)
				{
					for (int num106 = value3; num106 <= value4; num106++)
					{
						Tile tile21 = Main.tile[num105, num106];
						if (tile21.liquid <= 0)
						{
							continue;
						}
						int num107 = tile21.liquidType();
						int num108 = 0;
						for (int num109 = num105 - 1; num109 <= num105 + 1; num109++)
						{
							for (int num110 = num106 - 1; num110 <= num106 + 1; num110++)
							{
								if (Main.tile[num109, num110].liquidType() == num107)
								{
									num108 += Main.tile[num109, num110].liquid;
								}
							}
						}
						if (num108 > 100)
						{
							list22.Add(new Tuple<int, int>(num105, num106));
						}
					}
				}
				if (list22.Count > 0)
				{
					float num111 = -1f;
					Tuple<int, int> tuple20 = list22[0];
					for (int num112 = 0; num112 < list22.Count; num112++)
					{
						float num113 = Vector2.Distance(new Vector2(list22[num112].Item1, list22[num112].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num111 == -1f || num113 < num111)
						{
							num111 = num113;
							tuple20 = list22[num112];
						}
					}
					if (Collision.InTileBounds(tuple20.Item1, tuple20.Item2, value, value3, value2, value4))
					{
						fX = tuple20.Item1;
						fY = tuple20.Item2;
					}
				}
				list22.Clear();
			}
			if (ıtem.type == 849 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list23 = new List<Tuple<int, int>>();
				for (int num114 = value; num114 <= value2; num114++)
				{
					for (int num115 = value3; num115 <= value4; num115++)
					{
						Tile tile22 = Main.tile[num114, num115];
						if ((tile22.wire() || tile22.wire2() || tile22.wire3()) && !tile22.actuator() && tile22.active())
						{
							list23.Add(new Tuple<int, int>(num114, num115));
						}
					}
				}
				if (list23.Count > 0)
				{
					float num116 = -1f;
					Tuple<int, int> tuple21 = list23[0];
					for (int num117 = 0; num117 < list23.Count; num117++)
					{
						float num118 = Vector2.Distance(new Vector2(list23[num117].Item1, list23[num117].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num116 == -1f || num118 < num116)
						{
							num116 = num118;
							tuple21 = list23[num117];
						}
					}
					if (Collision.InTileBounds(tuple21.Item1, tuple21.Item2, value, value3, value2, value4))
					{
						fX = tuple21.Item1;
						fY = tuple21.Item2;
					}
				}
				list23.Clear();
			}
			if (ıtem.createTile == 82 && fX == -1 && fY == -1)
			{
				int placeStyle = ıtem.placeStyle;
				List<Tuple<int, int>> list24 = new List<Tuple<int, int>>();
				for (int num119 = value; num119 <= value2; num119++)
				{
					for (int num120 = value3; num120 <= value4; num120++)
					{
						Tile tile23 = Main.tile[num119, num120];
						Tile tile24 = Main.tile[num119, num120 + 1];
						if ((tile23.active() && !TileID.Sets.BreakableWhenPlacing[tile23.type] && (!Main.tileCut[tile23.type] || tile23.type == 82 || tile23.type == 83)) || !tile24.nactive() || tile24.halfBrick() || tile24.slope() != 0)
						{
							continue;
						}
						switch (placeStyle)
						{
						case 0:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 2 && tile24.type != 109) || tile23.liquid > 0)
							{
								continue;
							}
							break;
						case 1:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 60) || tile23.liquid > 0)
							{
								continue;
							}
							break;
						case 2:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 0 && tile24.type != 59) || tile23.liquid > 0)
							{
								continue;
							}
							break;
						case 3:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 203 && tile24.type != 199 && tile24.type != 23 && tile24.type != 25) || tile23.liquid > 0)
							{
								continue;
							}
							break;
						case 4:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 53 && tile24.type != 116) || (tile23.liquid > 0 && tile23.lava()))
							{
								continue;
							}
							break;
						case 5:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 57) || (tile23.liquid > 0 && !tile23.lava()))
							{
								continue;
							}
							break;
						case 6:
							if ((tile24.type != 78 && tile24.type != 380 && tile24.type != 147 && tile24.type != 161 && tile24.type != 163 && tile24.type != 164 && tile24.type != 200) || (tile23.liquid > 0 && tile23.lava()))
							{
								continue;
							}
							break;
						}
						list24.Add(new Tuple<int, int>(num119, num120));
					}
				}
				if (list24.Count > 0)
				{
					float num121 = -1f;
					Tuple<int, int> tuple22 = list24[0];
					for (int num122 = 0; num122 < list24.Count; num122++)
					{
						float num123 = Vector2.Distance(new Vector2(list24[num122].Item1, list24[num122].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num121 == -1f || num123 < num121)
						{
							num121 = num123;
							tuple22 = list24[num122];
						}
					}
					if (Collision.InTileBounds(tuple22.Item1, tuple22.Item2, value, value3, value2, value4))
					{
						fX = tuple22.Item1;
						fY = tuple22.Item2;
					}
				}
				list24.Clear();
			}
			if (ıtem.createTile == 380 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list25 = new List<Tuple<int, int>>();
				bool flag14 = false;
				if (Main.tile[num, num2].active() && Main.tile[num, num2].type == 380)
				{
					flag14 = true;
				}
				if (!flag14)
				{
					for (int num124 = value; num124 <= value2; num124++)
					{
						for (int num125 = value3; num125 <= value4; num125++)
						{
							Tile tile25 = Main.tile[num124, num125];
							if (tile25.active() && tile25.type == 380)
							{
								if (!Main.tile[num124 - 1, num125].active() || Main.tileCut[Main.tile[num124 - 1, num125].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num124 - 1, num125].type])
								{
									list25.Add(new Tuple<int, int>(num124 - 1, num125));
								}
								if (!Main.tile[num124 + 1, num125].active() || Main.tileCut[Main.tile[num124 + 1, num125].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[num124 + 1, num125].type])
								{
									list25.Add(new Tuple<int, int>(num124 + 1, num125));
								}
							}
						}
					}
				}
				if (list25.Count > 0)
				{
					float num126 = -1f;
					Tuple<int, int> tuple23 = list25[0];
					for (int num127 = 0; num127 < list25.Count; num127++)
					{
						float num128 = Vector2.Distance(new Vector2(list25[num127].Item1, list25[num127].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num126 == -1f || num128 < num126)
						{
							num126 = num128;
							tuple23 = list25[num127];
						}
					}
					if (Collision.InTileBounds(tuple23.Item1, tuple23.Item2, value, value3, value2, value4) && num126 != -1f)
					{
						fX = tuple23.Item1;
						fY = tuple23.Item2;
					}
				}
				list25.Clear();
			}
			if (ıtem.createTile == 78 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list26 = new List<Tuple<int, int>>();
				bool flag15 = false;
				if (Main.tile[num, num2].active())
				{
					flag15 = true;
				}
				if (!Collision.InTileBounds(num, num2, value, value3, value2, value4))
				{
					flag15 = true;
				}
				if (!flag15)
				{
					for (int num129 = value; num129 <= value2; num129++)
					{
						for (int num130 = value3; num130 <= value4; num130++)
						{
							Tile tile26 = Main.tile[num129, num130];
							Tile tile27 = Main.tile[num129, num130 + 1];
							if ((!tile26.active() || Main.tileCut[tile26.type] || TileID.Sets.BreakableWhenPlacing[tile26.type]) && tile27.nactive() && !tile27.halfBrick() && tile27.slope() == 0 && Main.tileSolid[tile27.type])
							{
								list26.Add(new Tuple<int, int>(num129, num130));
							}
						}
					}
				}
				if (list26.Count > 0)
				{
					float num131 = -1f;
					Tuple<int, int> tuple24 = list26[0];
					for (int num132 = 0; num132 < list26.Count; num132++)
					{
						if (Collision.EmptyTile(list26[num132].Item1, list26[num132].Item2, true))
						{
							float num133 = Vector2.Distance(new Vector2(list26[num132].Item1, list26[num132].Item2) * 16f + Vector2.One * 8f, mouse);
							if (num131 == -1f || num133 < num131)
							{
								num131 = num133;
								tuple24 = list26[num132];
							}
						}
					}
					if (Collision.InTileBounds(tuple24.Item1, tuple24.Item2, value, value3, value2, value4) && num131 != -1f)
					{
						fX = tuple24.Item1;
						fY = tuple24.Item2;
					}
				}
				list26.Clear();
			}
			if (ıtem.type == 213 && fX == -1 && fY == -1)
			{
				List<Tuple<int, int>> list27 = new List<Tuple<int, int>>();
				for (int num134 = value; num134 <= value2; num134++)
				{
					for (int num135 = value3; num135 <= value4; num135++)
					{
						Tile tile28 = Main.tile[num134, num135];
						bool flag16 = !Main.tile[num134 - 1, num135].active() || !Main.tile[num134, num135 + 1].active() || !Main.tile[num134 + 1, num135].active() || !Main.tile[num134, num135 - 1].active();
						bool flag17 = !Main.tile[num134 - 1, num135 - 1].active() || !Main.tile[num134 - 1, num135 + 1].active() || !Main.tile[num134 + 1, num135 + 1].active() || !Main.tile[num134 + 1, num135 - 1].active();
						if (tile28.active() && !tile28.inActive() && (tile28.type == 0 || tile28.type == 1) && (flag16 || (tile28.type == 0 && flag17)))
						{
							list27.Add(new Tuple<int, int>(num134, num135));
						}
					}
				}
				if (list27.Count > 0)
				{
					float num136 = -1f;
					Tuple<int, int> tuple25 = list27[0];
					for (int num137 = 0; num137 < list27.Count; num137++)
					{
						float num138 = Vector2.Distance(new Vector2(list27[num137].Item1, list27[num137].Item2) * 16f + Vector2.One * 8f, mouse);
						if (num136 == -1f || num138 < num136)
						{
							num136 = num138;
							tuple25 = list27[num137];
						}
					}
					if (Collision.InTileBounds(tuple25.Item1, tuple25.Item2, value, value3, value2, value4))
					{
						fX = tuple25.Item1;
						fY = tuple25.Item2;
					}
				}
				list27.Clear();
			}
			if (fX != -1 && fY != -1)
			{
				Main.smartDigX = (tileTargetX = fX);
				Main.smartDigY = (tileTargetY = fY);
				Main.smartDigShowing = true;
			}
			list.Clear();
		}

		private static void SmartCursor_Axe(Item item, ref Vector2 mouse, int LX, int HX, int LY, int HY, ref int fX, ref int fY)
		{
			if (item.axe <= 0 || fX != -1 || fY != -1)
			{
				return;
			}
			float num = -1f;
			for (int i = LX; i <= HX; i++)
			{
				for (int j = LY; j <= HY; j++)
				{
					if (!Main.tile[i, j].active())
					{
						continue;
					}
					Tile tile = Main.tile[i, j];
					if (!Main.tileAxe[tile.type])
					{
						continue;
					}
					int num2 = i;
					int k = j;
					if (tile.type == 5)
					{
						if (Collision.InTileBounds(num2 + 1, k, LX, LY, HX, HY))
						{
							if (Main.tile[num2, k].frameY >= 198 && Main.tile[num2, k].frameX == 44)
							{
								num2++;
							}
							if (Main.tile[num2, k].frameX == 66 && Main.tile[num2, k].frameY <= 44)
							{
								num2++;
							}
							if (Main.tile[num2, k].frameX == 44 && Main.tile[num2, k].frameY >= 132 && Main.tile[num2, k].frameY <= 176)
							{
								num2++;
							}
						}
						if (Collision.InTileBounds(num2 - 1, k, LX, LY, HX, HY))
						{
							if (Main.tile[num2, k].frameY >= 198 && Main.tile[num2, k].frameX == 66)
							{
								num2--;
							}
							if (Main.tile[num2, k].frameX == 88 && Main.tile[num2, k].frameY >= 66 && Main.tile[num2, k].frameY <= 110)
							{
								num2--;
							}
							if (Main.tile[num2, k].frameX == 22 && Main.tile[num2, k].frameY >= 132 && Main.tile[num2, k].frameY <= 176)
							{
								num2--;
							}
						}
						for (; Main.tile[num2, k].active() && Main.tile[num2, k].type == 5 && Main.tile[num2, k + 1].type == 5 && Collision.InTileBounds(num2, k + 1, LX, LY, HX, HY); k++)
						{
						}
					}
					if (tile.type == 80)
					{
						if (Collision.InTileBounds(num2 + 1, k, LX, LY, HX, HY))
						{
							if (Main.tile[num2, k].frameX == 54)
							{
								num2++;
							}
							if (Main.tile[num2, k].frameX == 108 && Main.tile[num2, k].frameY == 36)
							{
								num2++;
							}
						}
						if (Collision.InTileBounds(num2 - 1, k, LX, LY, HX, HY))
						{
							if (Main.tile[num2, k].frameX == 36)
							{
								num2--;
							}
							if (Main.tile[num2, k].frameX == 108 && Main.tile[num2, k].frameY == 18)
							{
								num2--;
							}
						}
						for (; Main.tile[num2, k].active() && Main.tile[num2, k].type == 80 && Main.tile[num2, k + 1].type == 80 && Collision.InTileBounds(num2, k + 1, LX, LY, HX, HY); k++)
						{
						}
					}
					if (tile.type == 323 || tile.type == 72)
					{
						for (; Main.tile[num2, k].active() && ((Main.tile[num2, k].type == 323 && Main.tile[num2, k + 1].type == 323) || (Main.tile[num2, k].type == 72 && Main.tile[num2, k + 1].type == 72)) && Collision.InTileBounds(num2, k + 1, LX, LY, HX, HY); k++)
						{
						}
					}
					float num3 = Vector2.Distance(new Vector2(num2, k) * 16f + Vector2.One * 8f, mouse);
					if (num == -1f || num3 < num)
					{
						num = num3;
						fX = num2;
						fY = k;
					}
				}
			}
		}

		public void SmartitemLookup()
		{
			if (controlTorch && itemAnimation == 0)
			{
				int num = 0;
				int num2 = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
				int num3 = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
				if (gravDir == -1f)
				{
					num3 = (int)((Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY) / 16f);
				}
				int num4 = -10;
				int num5 = -10;
				int num6 = -10;
				int num7 = -10;
				int num8 = -10;
				for (int i = 0; i < 50; i++)
				{
					if (inventory[i].pick > 0 && num4 == -10)
					{
						num4 = inventory[i].tileBoost;
					}
					if (inventory[i].axe > 0 && num5 == -10)
					{
						num5 = inventory[i].tileBoost;
					}
					if (inventory[i].hammer > 0 && num6 == -10)
					{
						num6 = inventory[i].tileBoost;
					}
					if ((inventory[i].type == 929 || inventory[i].type == 1338 || inventory[i].type == 1345) && num7 == -10)
					{
						num7 = inventory[i].tileBoost;
					}
					if ((inventory[i].type == 424 || inventory[i].type == 1103) && num8 == -10)
					{
						num8 = inventory[i].tileBoost;
					}
				}
				int num9 = 0;
				int num10 = 0;
				if (position.X / 16f >= (float)num2)
				{
					num9 = (int)(position.X / 16f) - num2;
				}
				if ((position.X + (float)width) / 16f <= (float)num2)
				{
					num9 = num2 - (int)((position.X + (float)width) / 16f);
				}
				if (position.Y / 16f >= (float)num3)
				{
					num10 = (int)(position.Y / 16f) - num3;
				}
				if ((position.Y + (float)height) / 16f <= (float)num3)
				{
					num10 = num3 - (int)((position.Y + (float)height) / 16f);
				}
				bool flag = false;
				bool flag2 = false;
				try
				{
					flag2 = (Main.tile[num2, num3].liquid > 0);
					if (Main.tile[num2, num3].active())
					{
						int type = Main.tile[num2, num3].type;
						if (type == 219 && num9 <= num8 + tileRangeX && num10 <= num8 + tileRangeY)
						{
							num = 7;
							flag = true;
						}
						else if (type == 209 && num9 <= num7 + tileRangeX && num10 <= num7 + tileRangeY)
						{
							num = 6;
							flag = true;
						}
						else if (Main.tileHammer[type] && num9 <= num6 + tileRangeX && num10 <= num6 + tileRangeY)
						{
							num = 1;
							flag = true;
						}
						else if (Main.tileAxe[type] && num9 <= num5 + tileRangeX && num10 <= num5 + tileRangeY)
						{
							num = 2;
							flag = true;
						}
						else if (num9 <= num4 + tileRangeX && num10 <= num4 + tileRangeY)
						{
							num = 3;
							flag = true;
						}
					}
					else if (flag2 && wet)
					{
						num = 4;
						flag = true;
					}
				}
				catch
				{
				}
				if (!flag && wet)
				{
					num = 4;
				}
				if (num == 0 || num == 4)
				{
					float num11 = Math.Abs((float)Main.mouseX + Main.screenPosition.X - (position.X + (float)(width / 2)));
					float num12 = Math.Abs((float)Main.mouseY + Main.screenPosition.Y - (position.Y + (float)(height / 2))) * 1.3f;
					float num13 = (float)Math.Sqrt(num11 * num11 + num12 * num12);
					if (num13 > 200f)
					{
						num = 5;
					}
				}
				for (int j = 0; j < 50; j++)
				{
					int type2 = inventory[j].type;
					switch (num)
					{
					case 0:
						switch (type2)
						{
						case 8:
						case 427:
						case 428:
						case 429:
						case 430:
						case 431:
						case 432:
						case 433:
						case 523:
						case 974:
						case 1245:
						case 1333:
						case 2274:
						case 3004:
						case 3045:
						case 3114:
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						case 282:
						case 286:
						case 3002:
						case 3112:
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							break;
						}
						break;
					case 1:
						if (inventory[j].hammer > 0)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					case 2:
						if (inventory[j].axe > 0)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					case 3:
						if (inventory[j].pick > 0)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					case 4:
						if (inventory[j].type != 282 && inventory[j].type != 286 && inventory[j].type != 3002 && inventory[j].type != 3112 && inventory[j].type != 930 && (type2 == 8 || type2 == 427 || type2 == 428 || type2 == 429 || type2 == 430 || type2 == 431 || type2 == 432 || type2 == 433 || type2 == 974 || type2 == 1245 || type2 == 2274 || type2 == 3004 || type2 == 3045 || type2 == 3114))
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							if (inventory[selectedItem].createTile != 4)
							{
								selectedItem = j;
							}
							break;
						}
						if ((type2 == 282 || type2 == 286 || type2 == 3002 || type2 == 3112) && flag2)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						if (type2 == 930 && flag2)
						{
							bool flag4 = false;
							for (int num16 = 57; num16 >= 0; num16--)
							{
								if (inventory[num16].ammo == inventory[j].useAmmo)
								{
									flag4 = true;
									break;
								}
							}
							if (flag4)
							{
								if (nonTorch == -1)
								{
									nonTorch = selectedItem;
								}
								selectedItem = j;
								return;
							}
						}
						else if (type2 == 1333 || type2 == 523)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					case 5:
						switch (type2)
						{
						case 8:
						case 427:
						case 428:
						case 429:
						case 430:
						case 431:
						case 432:
						case 433:
						case 523:
						case 974:
						case 1245:
						case 1333:
						case 2274:
						case 3004:
						case 3045:
						case 3114:
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							if (inventory[selectedItem].createTile != 4)
							{
								selectedItem = j;
							}
							break;
						case 930:
						{
							bool flag3 = false;
							for (int num15 = 57; num15 >= 0; num15--)
							{
								if (inventory[num15].ammo == inventory[j].useAmmo)
								{
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								if (nonTorch == -1)
								{
									nonTorch = selectedItem;
								}
								selectedItem = j;
								return;
							}
							break;
						}
						case 282:
						case 286:
						case 3002:
						case 3112:
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					case 6:
					{
						int num14 = 929;
						if (Main.tile[num2, num3].frameX >= 144)
						{
							num14 = 1345;
						}
						else if (Main.tile[num2, num3].frameX >= 72)
						{
							num14 = 1338;
						}
						if (type2 == num14)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					}
					case 7:
						if (ItemID.Sets.ExtractinatorMode[type2] >= 0)
						{
							if (nonTorch == -1)
							{
								nonTorch = selectedItem;
							}
							selectedItem = j;
							return;
						}
						break;
					}
				}
			}
			else if (nonTorch > -1 && itemAnimation == 0)
			{
				selectedItem = nonTorch;
				nonTorch = -1;
			}
		}

		public void ResetEffects()
		{
			if (extraAccessory && (Main.expertMode || Main.gameMenu))
			{
				extraAccessorySlots = 1;
			}
			else
			{
				extraAccessorySlots = 0;
			}
			arcticDivingGear = false;
			strongBees = false;
			armorPenetration = 0;
			shroomiteStealth = false;
			statDefense = 0;
			accWatch = 0;
			accCompass = 0;
			accDepthMeter = 0;
			accDivingHelm = false;
			lifeRegen = 0;
			manaCost = 1f;
			meleeSpeed = 1f;
			meleeDamage = 1f;
			rangedDamage = 1f;
			thrownDamage = 1f;
			magicDamage = 1f;
			minionDamage = 1f;
			meleeCrit = 4;
			rangedCrit = 4;
			magicCrit = 4;
			thrownCrit = 4;
			thrownVelocity = 1f;
			minionKB = 0f;
			moveSpeed = 1f;
			boneArmor = false;
			honey = false;
			frostArmor = false;
			rocketBoots = 0;
			fireWalk = false;
			noKnockback = false;
			jumpBoost = false;
			noFallDmg = false;
			accFlipper = false;
			spawnMax = false;
			spaceGun = false;
			killGuide = false;
			killClothier = false;
			lavaImmune = false;
			gills = false;
			slowFall = false;
			findTreasure = false;
			invis = false;
			nightVision = false;
			enemySpawns = false;
			thorns = 0f;
			aggro = 0;
			waterWalk = false;
			waterWalk2 = false;
			detectCreature = false;
			gravControl = false;
			bee = false;
			gravControl2 = false;
			statLifeMax2 = statLifeMax;
			statManaMax2 = statManaMax;
			ammoCost80 = false;
			ammoCost75 = false;
			thrownCost50 = false;
			thrownCost33 = false;
			manaRegenBuff = false;
			arrowDamage = 1f;
			bulletDamage = 1f;
			rocketDamage = 1f;
			yoraiz0rEye = 0;
			yoraiz0rDarkness = false;
			suspiciouslookingTentacle = false;
			crimsonHeart = false;
			lightOrb = false;
			blueFairy = false;
			redFairy = false;
			greenFairy = false;
			wisp = false;
			bunny = false;
			turtle = false;
			eater = false;
			skeletron = false;
			hornet = false;
			zephyrfish = false;
			tiki = false;
			lizard = false;
			parrot = false;
			sapling = false;
			cSapling = false;
			truffle = false;
			yoyoGlove = false;
			counterWeight = 0;
			stringColor = 0;
			yoyoString = false;
			shadowDodge = false;
			palladiumRegen = false;
			chaosState = false;
			onHitDodge = false;
			onHitRegen = false;
			onHitPetal = false;
			iceBarrier = false;
			maxMinions = 1;
			ammoBox = false;
			ammoPotion = false;
			penguin = false;
			sporeSac = false;
			shinyStone = false;
			magicLantern = false;
			rabid = false;
			sunflower = false;
			wellFed = false;
			inferno = false;
			manaMagnet = false;
			lifeMagnet = false;
			lifeForce = false;
			dangerSense = false;
			endurance = 0f;
			calmed = false;
			beetleOrbs = 0;
			beetleBuff = false;
			miniMinotaur = false;
			goldRing = false;
			solarShields = 0;
			GoingDownWithGrapple = false;
			fishingSkill = 0;
			cratePotion = false;
			sonarPotion = false;
			accTackleBox = false;
			accFishingLine = false;
			accFishFinder = false;
			accWeatherRadio = false;
			accThirdEye = false;
			accJarOfSouls = false;
			accCalendar = false;
			accStopwatch = false;
			accOreFinder = false;
			accCritterGuide = false;
			accDreamCatcher = false;
			wallSpeed = 1f;
			tileSpeed = 1f;
			autoPaint = false;
			babyFaceMonster = false;
			manaSick = false;
			puppy = false;
			grinch = false;
			blackCat = false;
			spider = false;
			squashling = false;
			magicCuffs = false;
			coldDash = false;
			sailDash = false;
			cordage = false;
			magicQuiver = false;
			magmaStone = false;
			lavaRose = false;
			eyeSpring = false;
			snowman = false;
			scope = false;
			panic = false;
			brainOfConfusion = false;
			dino = false;
			crystalLeaf = false;
			pygmy = false;
			raven = false;
			slime = false;
			hornetMinion = false;
			impMinion = false;
			twinsMinion = false;
			spiderMinion = false;
			pirateMinion = false;
			sharknadoMinion = false;
			stardustMinion = false;
			stardustGuardian = false;
			stardustDragon = false;
			UFOMinion = false;
			DeadlySphereMinion = false;
			chilled = false;
			dazed = false;
			frozen = false;
			stoned = false;
			webbed = false;
			ichor = false;
			manaRegenBonus = 0;
			manaRegenDelayBonus = 0;
			carpet = false;
			iceSkate = false;
			dash = 0;
			spikedBoots = 0;
			blackBelt = false;
			lavaMax = 0;
			archery = false;
			poisoned = false;
			venom = false;
			blind = false;
			blackout = false;
			onFire = false;
			dripping = false;
			drippingSlime = false;
			burned = false;
			suffocating = false;
			onFire2 = false;
			onFrostBurn = false;
			frostBurn = false;
			noItems = false;
			blockRange = 0;
			pickSpeed = 1f;
			wereWolf = false;
			rulerGrid = false;
			rulerLine = false;
			bleed = false;
			confused = false;
			wings = 0;
			wingsLogic = 0;
			wingTimeMax = 0;
			brokenArmor = false;
			silence = false;
			slow = false;
			gross = false;
			tongued = false;
			kbGlove = false;
			kbBuff = false;
			starCloak = false;
			longInvince = false;
			pStone = false;
			manaFlower = false;
			crimsonRegen = false;
			ghostHeal = false;
			ghostHurt = false;
			turtleArmor = false;
			turtleThorns = false;
			spiderArmor = false;
			loveStruck = false;
			stinky = false;
			dryadWard = false;
			resistCold = false;
			electrified = false;
			moonLeech = false;
			headcovered = false;
			vortexDebuff = false;
			setVortex = (setNebula = (setStardust = false));
			nebulaLevelDamage = (nebulaLevelLife = (nebulaLevelMana = 0));
			ignoreWater = false;
			meleeEnchant = 0;
			discount = false;
			coins = false;
			doubleJumpSail = false;
			doubleJumpSandstorm = false;
			doubleJumpBlizzard = false;
			doubleJumpFart = false;
			doubleJumpUnicorn = false;
			paladinBuff = false;
			paladinGive = false;
			autoJump = false;
			justJumped = false;
			jumpSpeedBoost = 0f;
			extraFall = 0;
			if (phantasmTime > 0)
			{
				phantasmTime--;
			}
			for (int i = 0; i < npcTypeNoAggro.Length; i++)
			{
				npcTypeNoAggro[i] = false;
			}
			for (int j = 0; j < ownedProjectileCounts.Length; j++)
			{
				ownedProjectileCounts[j] = 0;
			}
			if (whoAmI == Main.myPlayer)
			{
				tileRangeX = 5;
				tileRangeY = 4;
			}
			mount.CheckMountBuff(this);
		}

		public void UpdateImmunity()
		{
			if (immune)
			{
				immuneTime--;
				if (immuneTime <= 0)
				{
					immune = false;
				}
				immuneAlpha += immuneAlphaDirection * 50;
				if (immuneAlpha <= 50)
				{
					immuneAlphaDirection = 1;
				}
				else if (immuneAlpha >= 205)
				{
					immuneAlphaDirection = -1;
				}
			}
			else
			{
				immuneAlpha = 0;
			}
			for (int i = 0; i < hurtCooldowns.Length; i++)
			{
				if (hurtCooldowns[i] > 0)
				{
					hurtCooldowns[i]--;
				}
			}
		}

		public void UpdateLifeRegen()
		{
			bool flag = false;
			if (shinyStone && (double)Math.Abs(velocity.X) < 0.05 && (double)Math.Abs(velocity.Y) < 0.05 && itemAnimation == 0)
			{
				flag = true;
			}
			if (poisoned)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 4;
			}
			if (venom)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 12;
			}
			if (onFire)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 8;
			}
			if (onFrostBurn)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 12;
			}
			if (onFire2)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 12;
			}
			if (burned)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 60;
				moveSpeed *= 0.5f;
			}
			if (suffocating)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 40;
			}
			if (electrified)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 8;
				if (controlLeft || controlRight)
				{
					lifeRegen -= 32;
				}
			}
			if (tongued && Main.expertMode)
			{
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
				lifeRegenTime = 0;
				lifeRegen -= 100;
			}
			if (honey && lifeRegen < 0)
			{
				lifeRegen += 4;
				if (lifeRegen > 0)
				{
					lifeRegen = 0;
				}
			}
			if (lifeRegen < 0 && nebulaLevelLife > 0)
			{
				lifeRegen = 0;
			}
			if (flag && lifeRegen < 0)
			{
				lifeRegen /= 2;
			}
			lifeRegenTime++;
			if (crimsonRegen)
			{
				lifeRegenTime++;
			}
			if (soulDrain > 0)
			{
				lifeRegenTime += 2;
			}
			if (flag)
			{
				if (lifeRegenTime > 90 && lifeRegenTime < 1800)
				{
					lifeRegenTime = 1800;
				}
				lifeRegenTime += 4;
				lifeRegen += 4;
			}
			if (honey)
			{
				lifeRegenTime += 2;
				lifeRegen += 2;
			}
			if (soulDrain > 0)
			{
				int num = (5 + soulDrain) / 2;
				lifeRegenTime += num;
				lifeRegen += num;
			}
			if (whoAmI == Main.myPlayer && Main.campfire)
			{
				lifeRegen++;
			}
			if (whoAmI == Main.myPlayer && Main.heartLantern)
			{
				lifeRegen += 2;
			}
			if (bleed)
			{
				lifeRegenTime = 0;
			}
			float num2 = 0f;
			if (lifeRegenTime >= 300)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 600)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 900)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 1200)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 1500)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 1800)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 2400)
			{
				num2 += 1f;
			}
			if (lifeRegenTime >= 3000)
			{
				num2 += 1f;
			}
			if (flag)
			{
				float num3 = lifeRegenTime - 3000;
				num3 /= 300f;
				if (num3 > 0f)
				{
					if (num3 > 30f)
					{
						num3 = 30f;
					}
					num2 += num3;
				}
			}
			else if (lifeRegenTime >= 3600)
			{
				num2 += 1f;
				lifeRegenTime = 3600;
			}
			num2 = ((velocity.X != 0f && grappling[0] <= 0) ? (num2 * 0.5f) : (num2 * 1.25f));
			if (crimsonRegen)
			{
				num2 *= 1.5f;
			}
			if (shinyStone)
			{
				num2 *= 1.1f;
			}
			if (whoAmI == Main.myPlayer && Main.campfire)
			{
				num2 *= 1.1f;
			}
			if (Main.expertMode && !wellFed)
			{
				num2 = ((!shinyStone) ? (num2 / 2f) : (num2 * 0.75f));
			}
			if (rabid)
			{
				num2 = ((!shinyStone) ? (num2 / 2f) : (num2 * 0.75f));
			}
			float num4 = (float)statLifeMax2 / 400f * 0.85f + 0.15f;
			num2 *= num4;
			lifeRegen += (int)Math.Round(num2);
			lifeRegenCount += lifeRegen;
			if (palladiumRegen)
			{
				lifeRegenCount += 6;
			}
			if (flag && lifeRegen > 0 && statLife < statLifeMax2)
			{
				lifeRegenCount++;
				if (flag && (Main.rand.Next(30000) < lifeRegenTime || Main.rand.Next(30) == 0))
				{
					int num5 = Dust.NewDust(position, width, height, 55, 0f, 0f, 200, default(Color), 0.5f);
					Main.dust[num5].noGravity = true;
					Main.dust[num5].velocity *= 0.75f;
					Main.dust[num5].fadeIn = 1.3f;
					Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
					vector.Normalize();
					vector *= (float)Main.rand.Next(50, 100) * 0.04f;
					Main.dust[num5].velocity = vector;
					vector.Normalize();
					vector *= 34f;
					Main.dust[num5].position = base.Center - vector;
				}
			}
			while (lifeRegenCount >= 120)
			{
				lifeRegenCount -= 120;
				if (statLife < statLifeMax2)
				{
					statLife++;
					if (crimsonRegen)
					{
						for (int i = 0; i < 10; i++)
						{
							int num6 = Dust.NewDust(position, width, height, 5, 0f, 0f, 175, default(Color), 1.75f);
							Main.dust[num6].noGravity = true;
							Main.dust[num6].velocity *= 0.75f;
							int num7 = Main.rand.Next(-40, 41);
							int num8 = Main.rand.Next(-40, 41);
							Main.dust[num6].position.X += num7;
							Main.dust[num6].position.Y += num8;
							Main.dust[num6].velocity.X = (float)(-num7) * 0.075f;
							Main.dust[num6].velocity.Y = (float)(-num8) * 0.075f;
						}
					}
				}
				if (statLife > statLifeMax2)
				{
					statLife = statLifeMax2;
				}
			}
			if (burned || suffocating || (tongued && Main.expertMode))
			{
				while (lifeRegenCount <= -600)
				{
					lifeRegenCount += 600;
					statLife -= 5;
					CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.LifeRegen, string.Concat(5), false, true);
					if (statLife <= 0 && whoAmI == Main.myPlayer)
					{
						if (suffocating)
						{
							KillMe(10.0, 0, false, Language.GetTextValue("DeathText.Suffocated", Main.player[Main.myPlayer].name));
						}
						else
						{
							KillMe(10.0, 0, false, Language.GetTextValue("DeathText.Burned", Main.player[Main.myPlayer].name));
						}
					}
				}
				return;
			}
			while (lifeRegenCount <= -120)
			{
				if (lifeRegenCount <= -480)
				{
					lifeRegenCount += 480;
					statLife -= 4;
					CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.LifeRegen, string.Concat(4), false, true);
				}
				else if (lifeRegenCount <= -360)
				{
					lifeRegenCount += 360;
					statLife -= 3;
					CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.LifeRegen, string.Concat(3), false, true);
				}
				else if (lifeRegenCount <= -240)
				{
					lifeRegenCount += 240;
					statLife -= 2;
					CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.LifeRegen, string.Concat(2), false, true);
				}
				else
				{
					lifeRegenCount += 120;
					statLife--;
					CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), CombatText.LifeRegen, string.Concat(1), false, true);
				}
				if (statLife <= 0 && whoAmI == Main.myPlayer)
				{
					if (poisoned || venom)
					{
						KillMe(10.0, 0, false, Language.GetTextValue("DeathText.Poisoned", Main.player[Main.myPlayer].name));
					}
					else if (electrified)
					{
						KillMe(10.0, 0, false, Language.GetTextValue("DeathText.Electrocuted", Main.player[Main.myPlayer].name));
					}
					else
					{
						KillMe(10.0, 0, false, Language.GetTextValue("DeathText.Burned", Main.player[Main.myPlayer].name));
					}
				}
			}
		}

		public void UpdateManaRegen()
		{
			if (nebulaLevelMana > 0)
			{
				int num = 6;
				nebulaManaCounter += nebulaLevelMana;
				if (nebulaManaCounter >= num)
				{
					nebulaManaCounter -= num;
					statMana++;
					if (statMana >= statManaMax2)
					{
						statMana = statManaMax2;
					}
				}
			}
			else
			{
				nebulaManaCounter = 0;
			}
			if (manaRegenDelay > 0)
			{
				manaRegenDelay--;
				manaRegenDelay -= manaRegenDelayBonus;
				if ((velocity.X == 0f && velocity.Y == 0f) || grappling[0] >= 0 || manaRegenBuff)
				{
					manaRegenDelay--;
				}
			}
			if (manaRegenBuff && manaRegenDelay > 20)
			{
				manaRegenDelay = 20;
			}
			if (manaRegenDelay <= 0)
			{
				manaRegenDelay = 0;
				manaRegen = statManaMax2 / 7 + 1 + manaRegenBonus;
				if ((velocity.X == 0f && velocity.Y == 0f) || grappling[0] >= 0 || manaRegenBuff)
				{
					manaRegen += statManaMax2 / 2;
				}
				float num2 = (float)statMana / (float)statManaMax2 * 0.8f + 0.2f;
				if (manaRegenBuff)
				{
					num2 = 1f;
				}
				manaRegen = (int)((double)((float)manaRegen * num2) * 1.15);
			}
			else
			{
				manaRegen = 0;
			}
			manaRegenCount += manaRegen;
			while (manaRegenCount >= 120)
			{
				bool flag = false;
				manaRegenCount -= 120;
				if (statMana < statManaMax2)
				{
					statMana++;
					flag = true;
				}
				if (statMana < statManaMax2)
				{
					continue;
				}
				if (whoAmI == Main.myPlayer && flag)
				{
					Main.PlaySound(25);
					for (int i = 0; i < 5; i++)
					{
						int num3 = Dust.NewDust(position, width, height, 45, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
						Main.dust[num3].noLight = true;
						Main.dust[num3].noGravity = true;
						Main.dust[num3].velocity *= 0.5f;
					}
				}
				statMana = statManaMax2;
			}
		}

		public void UpdateJumpHeight()
		{
			if (mount.Active)
			{
				jumpHeight = mount.JumpHeight(velocity.X);
				jumpSpeed = mount.JumpSpeed(velocity.X);
			}
			else
			{
				if (jumpBoost)
				{
					jumpHeight = 20;
					jumpSpeed = 6.51f;
				}
				if (wereWolf)
				{
					jumpHeight += 2;
					jumpSpeed += 0.2f;
				}
				jumpSpeed += jumpSpeedBoost;
			}
			if (sticky)
			{
				jumpHeight /= 10;
				jumpSpeed /= 5f;
			}
			if (dazed)
			{
				jumpHeight /= 5;
				jumpSpeed /= 2f;
			}
		}

		public void FindPulley()
		{
			if (!controlUp && !controlDown)
			{
				return;
			}
			int num = (int)(position.X + (float)(width / 2)) / 16;
			int num2 = (int)(position.Y - 8f) / 16;
			if (Main.tile[num, num2] == null || !Main.tile[num, num2].active() || !Main.tileRope[Main.tile[num, num2].type])
			{
				return;
			}
			float num3 = position.Y;
			if (Main.tile[num, num2 - 1] == null)
			{
				Main.tile[num, num2 - 1] = new Tile();
			}
			if (Main.tile[num, num2 + 1] == null)
			{
				Main.tile[num, num2 + 1] = new Tile();
			}
			if ((!Main.tile[num, num2 - 1].active() || !Main.tileRope[Main.tile[num, num2 - 1].type]) && (!Main.tile[num, num2 + 1].active() || !Main.tileRope[Main.tile[num, num2 + 1].type]))
			{
				num3 = num2 * 16 + 22;
			}
			float num4 = num * 16 + 8 - width / 2 + 6 * direction;
			int num5 = num * 16 + 8 - width / 2 + 6;
			int num6 = num * 16 + 8 - width / 2;
			int num7 = num * 16 + 8 - width / 2 + -6;
			int num8 = 1;
			float num9 = Math.Abs(position.X - (float)num5);
			if (Math.Abs(position.X - (float)num6) < num9)
			{
				num8 = 2;
				num9 = Math.Abs(position.X - (float)num6);
			}
			if (Math.Abs(position.X - (float)num7) < num9)
			{
				num8 = 3;
				num9 = Math.Abs(position.X - (float)num7);
			}
			if (num8 == 1)
			{
				num4 = num5;
				pulleyDir = 2;
				direction = 1;
			}
			if (num8 == 2)
			{
				num4 = num6;
				pulleyDir = 1;
			}
			if (num8 == 3)
			{
				num4 = num7;
				pulleyDir = 2;
				direction = -1;
			}
			if (!Collision.SolidCollision(new Vector2(num4, position.Y), width, height))
			{
				if (whoAmI == Main.myPlayer)
				{
					Main.cameraX = Main.cameraX + position.X - num4;
				}
				pulley = true;
				position.X = num4;
				gfxOffY = position.Y - num3;
				stepSpeed = 2.5f;
				position.Y = num3;
				velocity.X = 0f;
				return;
			}
			num4 = num5;
			pulleyDir = 2;
			direction = 1;
			if (!Collision.SolidCollision(new Vector2(num4, position.Y), width, height))
			{
				if (whoAmI == Main.myPlayer)
				{
					Main.cameraX = Main.cameraX + position.X - num4;
				}
				pulley = true;
				position.X = num4;
				gfxOffY = position.Y - num3;
				stepSpeed = 2.5f;
				position.Y = num3;
				velocity.X = 0f;
				return;
			}
			num4 = num7;
			pulleyDir = 2;
			direction = -1;
			if (!Collision.SolidCollision(new Vector2(num4, position.Y), width, height))
			{
				if (whoAmI == Main.myPlayer)
				{
					Main.cameraX = Main.cameraX + position.X - num4;
				}
				pulley = true;
				position.X = num4;
				gfxOffY = position.Y - num3;
				stepSpeed = 2.5f;
				position.Y = num3;
				velocity.X = 0f;
			}
		}

		public void HorizontalMovement()
		{
			if (chilled)
			{
				accRunSpeed = maxRunSpeed;
			}
			bool flag = (itemAnimation == 0 || inventory[selectedItem].useTurn) && mount.AllowDirectionChange;
			if (trackBoost != 0f)
			{
				velocity.X += trackBoost;
				trackBoost = 0f;
				if (velocity.X < 0f)
				{
					if (velocity.X < 0f - maxRunSpeed)
					{
						velocity.X = 0f - maxRunSpeed;
					}
				}
				else if (velocity.X > maxRunSpeed)
				{
					velocity.X = maxRunSpeed;
				}
			}
			if (controlLeft && velocity.X > 0f - maxRunSpeed)
			{
				if (!mount.Active || !mount.Cart || velocity.Y == 0f)
				{
					if (velocity.X > runSlowdown)
					{
						velocity.X -= runSlowdown;
					}
					velocity.X -= runAcceleration;
				}
				if (onWrongGround)
				{
					if (velocity.X < 0f - runSlowdown)
					{
						velocity.X += runSlowdown;
					}
					else
					{
						velocity.X = 0f;
					}
				}
				if (mount.Active && mount.Cart && !onWrongGround)
				{
					if (velocity.X < 0f && flag)
					{
						direction = -1;
					}
					else if (itemAnimation <= 0 && velocity.Y == 0f)
					{
						Main.PlaySound(2, (int)position.X + width / 2, (int)position.Y + height / 2, 55);
						DelegateMethods.Minecart.rotation = fullRotation;
						DelegateMethods.Minecart.rotationOrigin = fullRotationOrigin;
						if ((double)Math.Abs(velocity.X) > (double)maxRunSpeed * 0.66)
						{
							if (Main.rand.Next(2) == 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position + velocity * 0.66f, width, height, 1);
							}
							if (Main.rand.Next(2) == 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position + velocity * 0.33f, width, height, 1);
							}
							if (Main.rand.Next(2) == 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position, width, height, 1);
							}
						}
						else if ((double)Math.Abs(velocity.X) > (double)maxRunSpeed * 0.33)
						{
							if (Main.rand.Next(3) != 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position + velocity * 0.5f, width, height, 1);
							}
							if (Main.rand.Next(3) != 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position, width, height, 1);
							}
						}
						else
						{
							Minecart.WheelSparks(mount.MinecartDust, position, width, height, 1);
						}
					}
				}
				else if (!sandStorm && (itemAnimation == 0 || inventory[selectedItem].useTurn) && mount.AllowDirectionChange)
				{
					direction = -1;
				}
			}
			else if (controlRight && velocity.X < maxRunSpeed)
			{
				if (!mount.Active || !mount.Cart || velocity.Y == 0f)
				{
					if (velocity.X < 0f - runSlowdown)
					{
						velocity.X += runSlowdown;
					}
					velocity.X += runAcceleration;
				}
				if (onWrongGround)
				{
					if (velocity.X > runSlowdown)
					{
						velocity.X -= runSlowdown;
					}
					else
					{
						velocity.X = 0f;
					}
				}
				if (mount.Active && mount.Cart && !onWrongGround)
				{
					if (velocity.X > 0f && flag)
					{
						direction = 1;
					}
					else if (itemAnimation <= 0 && velocity.Y == 0f)
					{
						Main.PlaySound(2, (int)position.X + width / 2, (int)position.Y + height / 2, 55);
						DelegateMethods.Minecart.rotation = fullRotation;
						DelegateMethods.Minecart.rotationOrigin = fullRotationOrigin;
						if ((double)Math.Abs(velocity.X) > (double)maxRunSpeed * 0.66)
						{
							if (Main.rand.Next(2) == 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position + velocity * 0.66f, width, height, 1);
							}
							if (Main.rand.Next(2) == 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position + velocity * 0.33f, width, height, 1);
							}
							if (Main.rand.Next(2) == 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position, width, height, 1);
							}
						}
						else if ((double)Math.Abs(velocity.X) > (double)maxRunSpeed * 0.33)
						{
							if (Main.rand.Next(3) != 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position + velocity * 0.5f, width, height, 1);
							}
							if (Main.rand.Next(3) != 0)
							{
								Minecart.WheelSparks(mount.MinecartDust, position, width, height, 1);
							}
						}
						else
						{
							Minecart.WheelSparks(mount.MinecartDust, position, width, height, 1);
						}
					}
				}
				else if (!sandStorm && (itemAnimation == 0 || inventory[selectedItem].useTurn) && mount.AllowDirectionChange)
				{
					direction = 1;
				}
			}
			else if (controlLeft && velocity.X > 0f - accRunSpeed && dashDelay >= 0)
			{
				if (mount.Active && mount.Cart)
				{
					if (velocity.X < 0f)
					{
						direction = -1;
					}
				}
				else if ((itemAnimation == 0 || inventory[selectedItem].useTurn) && mount.AllowDirectionChange)
				{
					direction = -1;
				}
				if (velocity.Y == 0f || wingsLogic > 0 || mount.CanFly)
				{
					if (velocity.X > runSlowdown)
					{
						velocity.X -= runSlowdown;
					}
					velocity.X -= runAcceleration * 0.2f;
					if (wingsLogic > 0)
					{
						velocity.X -= runAcceleration * 0.2f;
					}
				}
				if (onWrongGround)
				{
					if (velocity.X < runSlowdown)
					{
						velocity.X += runSlowdown;
					}
					else
					{
						velocity.X = 0f;
					}
				}
				if (velocity.X < (0f - (accRunSpeed + maxRunSpeed)) / 2f && velocity.Y == 0f && !mount.Active)
				{
					int num = 0;
					if (gravDir == -1f)
					{
						num -= height;
					}
					if (runSoundDelay == 0 && velocity.Y == 0f)
					{
						Main.PlaySound(17, (int)position.X, (int)position.Y);
						runSoundDelay = 9;
					}
					if (wings == 3)
					{
						int num2 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)height + (float)num), width + 8, 4, 186, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num2].velocity *= 0.025f;
						Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						num2 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)height + (float)num), width + 8, 4, 186, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num2].velocity *= 0.2f;
						Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
					}
					else if (sailDash)
					{
						for (int i = 0; i < 4; i++)
						{
							int num3 = Dust.NewDust(new Vector2(position.X - 4f, position.Y), width + 8, height, 253, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 100, default(Color), 1.5f);
							Main.dust[num3].noGravity = true;
							Main.dust[num3].velocity.X = Main.dust[num3].velocity.X * 0.2f;
							Main.dust[num3].velocity.Y = Main.dust[num3].velocity.Y * 0.2f;
							Main.dust[num3].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
							Main.dust[num3].scale += (float)Main.rand.Next(-5, 3) * 0.1f;
							Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							vector.Normalize();
							vector *= (float)Main.rand.Next(81) * 0.1f;
						}
					}
					else if (coldDash)
					{
						for (int j = 0; j < 2; j++)
						{
							int num4 = (j != 0) ? Dust.NewDust(new Vector2(position.X + (float)(width / 2), position.Y + (float)height + gfxOffY), width / 2, 6, 76, 0f, 0f, 0, default(Color), 1.35f) : Dust.NewDust(new Vector2(position.X, position.Y + (float)height + gfxOffY), width / 2, 6, 76, 0f, 0f, 0, default(Color), 1.35f);
							Main.dust[num4].scale *= 1f + (float)Main.rand.Next(20, 40) * 0.01f;
							Main.dust[num4].noGravity = true;
							Main.dust[num4].noLight = true;
							Main.dust[num4].velocity *= 0.001f;
							Main.dust[num4].velocity.Y -= 0.003f;
							Main.dust[num4].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
						}
					}
					else
					{
						int num5 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)height + (float)num), width + 8, 4, 16, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num5].velocity.X = Main.dust[num5].velocity.X * 0.2f;
						Main.dust[num5].velocity.Y = Main.dust[num5].velocity.Y * 0.2f;
						Main.dust[num5].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
					}
				}
			}
			else if (controlRight && velocity.X < accRunSpeed && dashDelay >= 0)
			{
				if (mount.Active && mount.Cart)
				{
					if (velocity.X > 0f)
					{
						direction = -1;
					}
				}
				else if ((itemAnimation == 0 || inventory[selectedItem].useTurn) && mount.AllowDirectionChange)
				{
					direction = 1;
				}
				if (velocity.Y == 0f || wingsLogic > 0 || mount.CanFly)
				{
					if (velocity.X < 0f - runSlowdown)
					{
						velocity.X += runSlowdown;
					}
					velocity.X += runAcceleration * 0.2f;
					if (wingsLogic > 0)
					{
						velocity.X += runAcceleration * 0.2f;
					}
				}
				if (onWrongGround)
				{
					if (velocity.X > runSlowdown)
					{
						velocity.X -= runSlowdown;
					}
					else
					{
						velocity.X = 0f;
					}
				}
				if (velocity.X > (accRunSpeed + maxRunSpeed) / 2f && velocity.Y == 0f && !mount.Active)
				{
					int num6 = 0;
					if (gravDir == -1f)
					{
						num6 -= height;
					}
					if (runSoundDelay == 0 && velocity.Y == 0f)
					{
						Main.PlaySound(17, (int)position.X, (int)position.Y);
						runSoundDelay = 9;
					}
					if (wings == 3)
					{
						int num7 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)height + (float)num6), width + 8, 4, 186, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num7].velocity *= 0.025f;
						Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						num7 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)height + (float)num6), width + 8, 4, 186, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num7].velocity *= 0.2f;
						Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
					}
					else if (sailDash)
					{
						for (int k = 0; k < 4; k++)
						{
							int num8 = Dust.NewDust(new Vector2(position.X - 4f, position.Y), width + 8, height, 253, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 100, default(Color), 1.5f);
							Main.dust[num8].noGravity = true;
							Main.dust[num8].velocity.X = Main.dust[num8].velocity.X * 0.2f;
							Main.dust[num8].velocity.Y = Main.dust[num8].velocity.Y * 0.2f;
							Main.dust[num8].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
							Main.dust[num8].scale += (float)Main.rand.Next(-5, 3) * 0.1f;
							Vector2 vector2 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							vector2.Normalize();
							vector2 *= (float)Main.rand.Next(81) * 0.1f;
						}
					}
					else if (coldDash)
					{
						for (int l = 0; l < 2; l++)
						{
							int num9 = (l != 0) ? Dust.NewDust(new Vector2(position.X + (float)(width / 2), position.Y + (float)height + gfxOffY), width / 2, 6, 76, 0f, 0f, 0, default(Color), 1.35f) : Dust.NewDust(new Vector2(position.X, position.Y + (float)height + gfxOffY), width / 2, 6, 76, 0f, 0f, 0, default(Color), 1.35f);
							Main.dust[num9].scale *= 1f + (float)Main.rand.Next(20, 40) * 0.01f;
							Main.dust[num9].noGravity = true;
							Main.dust[num9].noLight = true;
							Main.dust[num9].velocity *= 0.001f;
							Main.dust[num9].velocity.Y -= 0.003f;
							Main.dust[num9].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
						}
					}
					else
					{
						int num10 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)height + (float)num6), width + 8, 4, 16, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num10].velocity.X = Main.dust[num10].velocity.X * 0.2f;
						Main.dust[num10].velocity.Y = Main.dust[num10].velocity.Y * 0.2f;
						Main.dust[num10].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
					}
				}
			}
			else if (mount.Active && mount.Cart && Math.Abs(velocity.X) >= 1f)
			{
				if (onWrongGround)
				{
					if (velocity.X > 0f)
					{
						if (velocity.X > runSlowdown)
						{
							velocity.X -= runSlowdown;
						}
						else
						{
							velocity.X = 0f;
						}
					}
					else if (velocity.X < 0f)
					{
						if (velocity.X < 0f - runSlowdown)
						{
							velocity.X += runSlowdown;
						}
						else
						{
							velocity.X = 0f;
						}
					}
				}
				if (velocity.X > maxRunSpeed)
				{
					velocity.X = maxRunSpeed;
				}
				if (velocity.X < 0f - maxRunSpeed)
				{
					velocity.X = 0f - maxRunSpeed;
				}
			}
			else if (velocity.Y == 0f)
			{
				if (velocity.X > runSlowdown)
				{
					velocity.X -= runSlowdown;
				}
				else if (velocity.X < 0f - runSlowdown)
				{
					velocity.X += runSlowdown;
				}
				else
				{
					velocity.X = 0f;
				}
			}
			else if (!PortalPhysicsEnabled)
			{
				if ((double)velocity.X > (double)runSlowdown * 0.5)
				{
					velocity.X -= runSlowdown * 0.5f;
				}
				else if ((double)velocity.X < (double)(0f - runSlowdown) * 0.5)
				{
					velocity.X += runSlowdown * 0.5f;
				}
				else
				{
					velocity.X = 0f;
				}
			}
			if (!mount.Active || mount.Type != 10 || !(Math.Abs(velocity.X) > mount.DashSpeed - mount.RunSpeed / 2f))
			{
				return;
			}
			Rectangle rect = getRect();
			if (direction == 1)
			{
				rect.Offset(width - 1, 0);
			}
			rect.Width = 2;
			rect.Inflate(6, 12);
			int num11 = 0;
			NPC nPC;
			while (true)
			{
				if (num11 >= 200)
				{
					return;
				}
				nPC = Main.npc[num11];
				if (nPC.active && !nPC.dontTakeDamage && !nPC.friendly && nPC.immune[whoAmI] == 0)
				{
					Rectangle rect2 = nPC.getRect();
					if (rect.Intersects(rect2) && (nPC.noTileCollide || Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height)))
					{
						break;
					}
				}
				num11++;
			}
			float num12 = 80f * minionDamage;
			float num13 = 10f;
			int num14 = direction;
			if (velocity.X < 0f)
			{
				num14 = -1;
			}
			if (velocity.X > 0f)
			{
				num14 = 1;
			}
			if (whoAmI == Main.myPlayer)
			{
				nPC.StrikeNPC((int)num12, num13, num14);
				if (Main.netMode != 0)
				{
					NetMessage.SendData(28, -1, -1, "", num11, num12, num13, num14);
				}
			}
			nPC.immune[whoAmI] = 30;
			immune = true;
			immuneTime = 6;
		}

		public void JumpMovement()
		{
			if (mount.Active && mount.Type == 3 && wetSlime == 0 && velocity.Y > 0f)
			{
				Rectangle rect = getRect();
				rect.Offset(0, height - 1);
				rect.Height = 2;
				rect.Inflate(12, 6);
				for (int i = 0; i < 200; i++)
				{
					NPC nPC = Main.npc[i];
					if (!nPC.active || nPC.dontTakeDamage || nPC.friendly || nPC.immune[whoAmI] != 0)
					{
						continue;
					}
					Rectangle rect2 = nPC.getRect();
					if (!rect.Intersects(rect2) || (!nPC.noTileCollide && !Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height)))
					{
						continue;
					}
					float num = 40f * minionDamage;
					float num2 = 5f;
					int num3 = direction;
					if (velocity.X < 0f)
					{
						num3 = -1;
					}
					if (velocity.X > 0f)
					{
						num3 = 1;
					}
					if (whoAmI == Main.myPlayer)
					{
						nPC.StrikeNPC((int)num, num2, num3);
						if (Main.netMode != 0)
						{
							NetMessage.SendData(28, -1, -1, "", i, num, num2, num3);
						}
					}
					nPC.immune[whoAmI] = 10;
					velocity.Y = -10f;
					immune = true;
					immuneTime = 6;
					break;
				}
			}
			if (controlJump)
			{
				bool flag = false;
				if (mount.Active && mount.Type == 3 && wetSlime > 0)
				{
					flag = true;
				}
				if (jump > 0)
				{
					if (velocity.Y == 0f)
					{
						jump = 0;
					}
					else
					{
						velocity.Y = (0f - jumpSpeed) * gravDir;
						if (merman && (!mount.Active || !mount.Cart))
						{
							if (swimTime <= 10)
							{
								swimTime = 30;
							}
						}
						else
						{
							jump--;
						}
					}
				}
				else if ((sliding || velocity.Y == 0f || flag || jumpAgainCloud || jumpAgainSandstorm || jumpAgainBlizzard || jumpAgainFart || jumpAgainSail || jumpAgainUnicorn || (wet && accFlipper && (!mount.Active || !mount.Cart))) && (releaseJump || (autoJump && (velocity.Y == 0f || sliding))))
				{
					if (sliding || velocity.Y == 0f)
					{
						justJumped = true;
					}
					bool flag2 = false;
					if (wet && accFlipper)
					{
						if (swimTime == 0)
						{
							swimTime = 30;
						}
						flag2 = true;
					}
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					bool flag6 = false;
					bool flag7 = false;
					if (!flag)
					{
						if (jumpAgainUnicorn)
						{
							flag7 = true;
							jumpAgainUnicorn = false;
						}
						else if (jumpAgainSandstorm)
						{
							flag3 = true;
							jumpAgainSandstorm = false;
						}
						else if (jumpAgainBlizzard)
						{
							flag4 = true;
							jumpAgainBlizzard = false;
						}
						else if (jumpAgainFart)
						{
							jumpAgainFart = false;
							flag5 = true;
						}
						else if (jumpAgainSail)
						{
							jumpAgainSail = false;
							flag6 = true;
						}
						else
						{
							jumpAgainCloud = false;
						}
					}
					canRocket = false;
					rocketRelease = false;
					if ((velocity.Y == 0f || sliding || (autoJump && justJumped)) && doubleJumpCloud)
					{
						jumpAgainCloud = true;
					}
					if ((velocity.Y == 0f || sliding || (autoJump && justJumped)) && doubleJumpSandstorm)
					{
						jumpAgainSandstorm = true;
					}
					if ((velocity.Y == 0f || sliding || (autoJump && justJumped)) && doubleJumpBlizzard)
					{
						jumpAgainBlizzard = true;
					}
					if ((velocity.Y == 0f || sliding || (autoJump && justJumped)) && doubleJumpFart)
					{
						jumpAgainFart = true;
					}
					if ((velocity.Y == 0f || sliding || (autoJump && justJumped)) && doubleJumpSail)
					{
						jumpAgainSail = true;
					}
					if ((velocity.Y == 0f || sliding || (autoJump && justJumped)) && doubleJumpUnicorn)
					{
						jumpAgainUnicorn = true;
					}
					if (velocity.Y == 0f || flag2 || sliding || flag)
					{
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = jumpHeight;
						if (sliding)
						{
							velocity.X = 3 * -slideDir;
						}
					}
					else if (flag3)
					{
						dJumpEffectSandstorm = true;
						int height2 = height;
						float gravDir2 = gravDir;
						float num14 = -1f;
						Main.PlaySound(16, (int)position.X, (int)position.Y);
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = jumpHeight * 3;
					}
					else if (flag4)
					{
						dJumpEffectBlizzard = true;
						int height3 = height;
						float gravDir3 = gravDir;
						float num15 = -1f;
						Main.PlaySound(16, (int)position.X, (int)position.Y);
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = (int)((double)jumpHeight * 1.5);
					}
					else if (flag6)
					{
						dJumpEffectSail = true;
						int num4 = height;
						if (gravDir == -1f)
						{
							num4 = 0;
						}
						Main.PlaySound(16, (int)position.X, (int)position.Y);
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = (int)((double)jumpHeight * 1.25);
						for (int j = 0; j < 30; j++)
						{
							int num5 = Dust.NewDust(new Vector2(position.X, position.Y + (float)num4), width, 12, 253, velocity.X * 0.3f, velocity.Y * 0.3f, 100, default(Color), 1.5f);
							if (j % 2 == 0)
							{
								Main.dust[num5].velocity.X += (float)Main.rand.Next(30, 71) * 0.1f;
							}
							else
							{
								Main.dust[num5].velocity.X -= (float)Main.rand.Next(30, 71) * 0.1f;
							}
							Main.dust[num5].velocity.Y += (float)Main.rand.Next(-10, 31) * 0.1f;
							Main.dust[num5].noGravity = true;
							Main.dust[num5].scale += (float)Main.rand.Next(-10, 41) * 0.01f;
							Main.dust[num5].velocity *= Main.dust[num5].scale * 0.7f;
							Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							vector.Normalize();
							vector *= (float)Main.rand.Next(81) * 0.1f;
						}
					}
					else if (flag5)
					{
						dJumpEffectFart = true;
						int num6 = height;
						if (gravDir == -1f)
						{
							num6 = 0;
						}
						Main.PlaySound(2, (int)position.X, (int)position.Y, 16);
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = jumpHeight * 2;
						for (int k = 0; k < 10; k++)
						{
							int num7 = Dust.NewDust(new Vector2(position.X - 34f, position.Y + (float)num6 - 16f), 102, 32, 188, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 100, default(Color), 1.5f);
							Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.5f - velocity.X * 0.1f;
							Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.5f - velocity.Y * 0.3f;
						}
						int num8 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 16f, position.Y + (float)num6 - 16f), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(435, 438));
						Main.gore[num8].velocity.X = Main.gore[num8].velocity.X * 0.1f - velocity.X * 0.1f;
						Main.gore[num8].velocity.Y = Main.gore[num8].velocity.Y * 0.1f - velocity.Y * 0.05f;
						num8 = Gore.NewGore(new Vector2(position.X - 36f, position.Y + (float)num6 - 16f), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(435, 438));
						Main.gore[num8].velocity.X = Main.gore[num8].velocity.X * 0.1f - velocity.X * 0.1f;
						Main.gore[num8].velocity.Y = Main.gore[num8].velocity.Y * 0.1f - velocity.Y * 0.05f;
						num8 = Gore.NewGore(new Vector2(position.X + (float)width + 4f, position.Y + (float)num6 - 16f), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(435, 438));
						Main.gore[num8].velocity.X = Main.gore[num8].velocity.X * 0.1f - velocity.X * 0.1f;
						Main.gore[num8].velocity.Y = Main.gore[num8].velocity.Y * 0.1f - velocity.Y * 0.05f;
					}
					else if (flag7)
					{
						dJumpEffectUnicorn = true;
						int height4 = height;
						float gravDir4 = gravDir;
						float num16 = -1f;
						Main.PlaySound(16, (int)position.X, (int)position.Y);
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = jumpHeight * 2;
						Vector2 center = base.Center;
						Vector2 value = new Vector2(50f, 20f);
						float num9 = (float)Math.PI * 2f * Main.rand.NextFloat();
						for (int l = 0; l < 5; l++)
						{
							for (float num10 = 0f; num10 < 14f; num10 += 1f)
							{
								Dust dust = Main.dust[Dust.NewDust(center, 0, 0, Utils.SelectRandom<int>(Main.rand, 176, 177, 179))];
								Vector2 value2 = Vector2.UnitY.RotatedBy(num10 * ((float)Math.PI * 2f) / 14f + num9);
								value2 *= 0.2f * (float)l;
								dust.position = center + value2 * value;
								dust.velocity = value2 + new Vector2(0f, gravDir * 4f);
								dust.noGravity = true;
								dust.scale = 1f + Main.rand.NextFloat() * 0.8f;
								dust.fadeIn = Main.rand.NextFloat() * 2f;
								dust.shader = GameShaders.Armor.GetSecondaryShader(cMount, this);
							}
						}
					}
					else
					{
						dJumpEffectCloud = true;
						int num11 = height;
						if (gravDir == -1f)
						{
							num11 = 0;
						}
						Main.PlaySound(16, (int)position.X, (int)position.Y);
						velocity.Y = (0f - jumpSpeed) * gravDir;
						jump = (int)((double)jumpHeight * 0.75);
						for (int m = 0; m < 10; m++)
						{
							int num12 = Dust.NewDust(new Vector2(position.X - 34f, position.Y + (float)num11 - 16f), 102, 32, 16, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 100, default(Color), 1.5f);
							Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.5f - velocity.X * 0.1f;
							Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.5f - velocity.Y * 0.3f;
						}
						int num13 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 16f, position.Y + (float)num11 - 16f), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(11, 14));
						Main.gore[num13].velocity.X = Main.gore[num13].velocity.X * 0.1f - velocity.X * 0.1f;
						Main.gore[num13].velocity.Y = Main.gore[num13].velocity.Y * 0.1f - velocity.Y * 0.05f;
						num13 = Gore.NewGore(new Vector2(position.X - 36f, position.Y + (float)num11 - 16f), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(11, 14));
						Main.gore[num13].velocity.X = Main.gore[num13].velocity.X * 0.1f - velocity.X * 0.1f;
						Main.gore[num13].velocity.Y = Main.gore[num13].velocity.Y * 0.1f - velocity.Y * 0.05f;
						num13 = Gore.NewGore(new Vector2(position.X + (float)width + 4f, position.Y + (float)num11 - 16f), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(11, 14));
						Main.gore[num13].velocity.X = Main.gore[num13].velocity.X * 0.1f - velocity.X * 0.1f;
						Main.gore[num13].velocity.Y = Main.gore[num13].velocity.Y * 0.1f - velocity.Y * 0.05f;
					}
				}
				releaseJump = false;
			}
			else
			{
				jump = 0;
				releaseJump = true;
				rocketRelease = true;
			}
		}

		public void DashMovement()
		{
			if (dash == 2 && eocDash > 0)
			{
				if (eocHit < 0)
				{
					Rectangle rectangle = new Rectangle((int)((double)position.X + (double)velocity.X * 0.5 - 4.0), (int)((double)position.Y + (double)velocity.Y * 0.5 - 4.0), width + 8, height + 8);
					for (int i = 0; i < 200; i++)
					{
						if (!Main.npc[i].active || Main.npc[i].dontTakeDamage || Main.npc[i].friendly)
						{
							continue;
						}
						NPC nPC = Main.npc[i];
						Rectangle rect = nPC.getRect();
						if (!rectangle.Intersects(rect) || (!nPC.noTileCollide && !Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height)))
						{
							continue;
						}
						float num = 30f * meleeDamage;
						float num2 = 9f;
						bool crit = false;
						if (kbGlove)
						{
							num2 *= 2f;
						}
						if (kbBuff)
						{
							num2 *= 1.5f;
						}
						if (Main.rand.Next(100) < meleeCrit)
						{
							crit = true;
						}
						int num3 = direction;
						if (velocity.X < 0f)
						{
							num3 = -1;
						}
						if (velocity.X > 0f)
						{
							num3 = 1;
						}
						if (whoAmI == Main.myPlayer)
						{
							nPC.StrikeNPC((int)num, num2, num3, crit);
							if (Main.netMode != 0)
							{
								NetMessage.SendData(28, -1, -1, "", i, num, num2, num3);
							}
						}
						eocDash = 10;
						dashDelay = 30;
						velocity.X = -num3 * 9;
						velocity.Y = -4f;
						immune = true;
						immuneTime = 4;
						eocHit = i;
					}
				}
				else if ((!controlLeft || !(velocity.X < 0f)) && (!controlRight || !(velocity.X > 0f)))
				{
					velocity.X *= 0.95f;
				}
			}
			if (dash == 3 && dashDelay < 0 && whoAmI == Main.myPlayer)
			{
				Rectangle rectangle2 = new Rectangle((int)((double)position.X + (double)velocity.X * 0.5 - 4.0), (int)((double)position.Y + (double)velocity.Y * 0.5 - 4.0), width + 8, height + 8);
				for (int j = 0; j < 200; j++)
				{
					if (!Main.npc[j].active || Main.npc[j].dontTakeDamage || Main.npc[j].friendly || Main.npc[j].immune[whoAmI] > 0)
					{
						continue;
					}
					NPC nPC2 = Main.npc[j];
					Rectangle rect2 = nPC2.getRect();
					if (!rectangle2.Intersects(rect2) || (!nPC2.noTileCollide && !Collision.CanHit(position, width, height, nPC2.position, nPC2.width, nPC2.height)))
					{
						continue;
					}
					if (!solarDashConsumedFlare)
					{
						solarDashConsumedFlare = true;
						ConsumeSolarFlare();
					}
					float num4 = 150f * meleeDamage;
					float num5 = 9f;
					bool crit2 = false;
					if (kbGlove)
					{
						num5 *= 2f;
					}
					if (kbBuff)
					{
						num5 *= 1.5f;
					}
					if (Main.rand.Next(100) < meleeCrit)
					{
						crit2 = true;
					}
					int num6 = direction;
					if (velocity.X < 0f)
					{
						num6 = -1;
					}
					if (velocity.X > 0f)
					{
						num6 = 1;
					}
					if (whoAmI == Main.myPlayer)
					{
						nPC2.StrikeNPC((int)num4, num5, num6, crit2);
						if (Main.netMode != 0)
						{
							NetMessage.SendData(28, -1, -1, "", j, num4, num5, num6);
						}
						int num7 = Projectile.NewProjectile(base.Center.X, base.Center.Y, 0f, 0f, 608, 150, 15f, Main.myPlayer);
						Main.projectile[num7].Kill();
					}
					nPC2.immune[whoAmI] = 6;
					immune = true;
					immuneTime = 4;
				}
			}
			if (dashDelay > 0)
			{
				if (eocDash > 0)
				{
					eocDash--;
				}
				if (eocDash == 0)
				{
					eocHit = -1;
				}
				dashDelay--;
			}
			else if (dashDelay < 0)
			{
				float num8 = 12f;
				float num9 = 0.992f;
				float num10 = Math.Max(accRunSpeed, maxRunSpeed);
				float num11 = 0.96f;
				int num12 = 20;
				if (dash == 1)
				{
					for (int k = 0; k < 2; k++)
					{
						int num13 = (velocity.Y != 0f) ? Dust.NewDust(new Vector2(position.X, position.Y + (float)(height / 2) - 8f), width, 16, 31, 0f, 0f, 100, default(Color), 1.4f) : Dust.NewDust(new Vector2(position.X, position.Y + (float)height - 4f), width, 8, 31, 0f, 0f, 100, default(Color), 1.4f);
						Main.dust[num13].velocity *= 0.1f;
						Main.dust[num13].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num13].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
					}
				}
				else if (dash == 2)
				{
					for (int l = 0; l < 0; l++)
					{
						int num14 = (velocity.Y != 0f) ? Dust.NewDust(new Vector2(position.X, position.Y + (float)(height / 2) - 8f), width, 16, 31, 0f, 0f, 100, default(Color), 1.4f) : Dust.NewDust(new Vector2(position.X, position.Y + (float)height - 4f), width, 8, 31, 0f, 0f, 100, default(Color), 1.4f);
						Main.dust[num14].velocity *= 0.1f;
						Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
					}
					num9 = 0.985f;
					num11 = 0.94f;
					num12 = 30;
				}
				else if (dash == 3)
				{
					for (int m = 0; m < 4; m++)
					{
						int num15 = Dust.NewDust(new Vector2(position.X, position.Y + 4f), width, height - 8, 6, 0f, 0f, 100, default(Color), 1.7f);
						Main.dust[num15].velocity *= 0.1f;
						Main.dust[num15].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num15].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
						Main.dust[num15].noGravity = true;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num15].fadeIn = 0.5f;
						}
					}
					num8 = 14f;
					num9 = 0.985f;
					num11 = 0.94f;
					num12 = 20;
				}
				else if (dash == 4)
				{
					for (int n = 0; n < 2; n++)
					{
						int num16 = Dust.NewDust(new Vector2(position.X, position.Y + 4f), width, height - 8, 229, 0f, 0f, 100, default(Color), 1.2f);
						Main.dust[num16].velocity *= 0.1f;
						Main.dust[num16].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num16].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						Main.dust[num16].noGravity = true;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num16].fadeIn = 0.3f;
						}
					}
					num9 = 0.985f;
					num11 = 0.94f;
					num12 = 20;
				}
				if (dash <= 0)
				{
					return;
				}
				vortexStealthActive = false;
				if (velocity.X > num8 || velocity.X < 0f - num8)
				{
					velocity.X *= num9;
					return;
				}
				if (velocity.X > num10 || velocity.X < 0f - num10)
				{
					velocity.X *= num11;
					return;
				}
				dashDelay = num12;
				if (velocity.X < 0f)
				{
					velocity.X = 0f - num10;
				}
				else if (velocity.X > 0f)
				{
					velocity.X = num10;
				}
			}
			else
			{
				if (dash <= 0 || mount.Active)
				{
					return;
				}
				if (dash == 1)
				{
					int num17 = 0;
					bool flag = false;
					if (dashTime > 0)
					{
						dashTime--;
					}
					if (dashTime < 0)
					{
						dashTime++;
					}
					if (controlRight && releaseRight)
					{
						if (dashTime > 0)
						{
							num17 = 1;
							flag = true;
							dashTime = 0;
						}
						else
						{
							dashTime = 15;
						}
					}
					else if (controlLeft && releaseLeft)
					{
						if (dashTime < 0)
						{
							num17 = -1;
							flag = true;
							dashTime = 0;
						}
						else
						{
							dashTime = -15;
						}
					}
					if (flag)
					{
						velocity.X = 16.9f * (float)num17;
						Point point = (base.Center + new Vector2(num17 * width / 2 + 2, gravDir * (float)(-height) / 2f + gravDir * 2f)).ToTileCoordinates();
						Point point2 = (base.Center + new Vector2(num17 * width / 2 + 2, 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
						{
							velocity.X /= 2f;
						}
						dashDelay = -1;
						for (int num18 = 0; num18 < 20; num18++)
						{
							int num19 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 31, 0f, 0f, 100, default(Color), 2f);
							Main.dust[num19].position.X += Main.rand.Next(-5, 6);
							Main.dust[num19].position.Y += Main.rand.Next(-5, 6);
							Main.dust[num19].velocity *= 0.2f;
							Main.dust[num19].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num19].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
						}
						int num20 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 34f), default(Vector2), Main.rand.Next(61, 64));
						Main.gore[num20].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num20].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num20].velocity *= 0.4f;
						num20 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 14f), default(Vector2), Main.rand.Next(61, 64));
						Main.gore[num20].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num20].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num20].velocity *= 0.4f;
					}
				}
				else if (dash == 2)
				{
					int num21 = 0;
					bool flag2 = false;
					if (dashTime > 0)
					{
						dashTime--;
					}
					if (dashTime < 0)
					{
						dashTime++;
					}
					if (controlRight && releaseRight)
					{
						if (dashTime > 0)
						{
							num21 = 1;
							flag2 = true;
							dashTime = 0;
						}
						else
						{
							dashTime = 15;
						}
					}
					else if (controlLeft && releaseLeft)
					{
						if (dashTime < 0)
						{
							num21 = -1;
							flag2 = true;
							dashTime = 0;
						}
						else
						{
							dashTime = -15;
						}
					}
					if (flag2)
					{
						velocity.X = 14.5f * (float)num21;
						Point point3 = (base.Center + new Vector2(num21 * width / 2 + 2, gravDir * (float)(-height) / 2f + gravDir * 2f)).ToTileCoordinates();
						Point point4 = (base.Center + new Vector2(num21 * width / 2 + 2, 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point3.X, point3.Y) || WorldGen.SolidOrSlopedTile(point4.X, point4.Y))
						{
							velocity.X /= 2f;
						}
						dashDelay = -1;
						eocDash = 15;
						for (int num22 = 0; num22 < 0; num22++)
						{
							int num23 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 31, 0f, 0f, 100, default(Color), 2f);
							Main.dust[num23].position.X += Main.rand.Next(-5, 6);
							Main.dust[num23].position.Y += Main.rand.Next(-5, 6);
							Main.dust[num23].velocity *= 0.2f;
							Main.dust[num23].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num23].shader = GameShaders.Armor.GetSecondaryShader(cShield, this);
						}
					}
				}
				else
				{
					if (dash != 3)
					{
						return;
					}
					int num24 = 0;
					bool flag3 = false;
					if (dashTime > 0)
					{
						dashTime--;
					}
					if (dashTime < 0)
					{
						dashTime++;
					}
					if (controlRight && releaseRight)
					{
						if (dashTime > 0)
						{
							num24 = 1;
							flag3 = true;
							dashTime = 0;
							solarDashing = true;
							solarDashConsumedFlare = false;
						}
						else
						{
							dashTime = 15;
						}
					}
					else if (controlLeft && releaseLeft)
					{
						if (dashTime < 0)
						{
							num24 = -1;
							flag3 = true;
							dashTime = 0;
							solarDashing = true;
							solarDashConsumedFlare = false;
						}
						else
						{
							dashTime = -15;
						}
					}
					if (flag3)
					{
						velocity.X = 21.9f * (float)num24;
						Point point5 = (base.Center + new Vector2(num24 * width / 2 + 2, gravDir * (float)(-height) / 2f + gravDir * 2f)).ToTileCoordinates();
						Point point6 = (base.Center + new Vector2(num24 * width / 2 + 2, 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
						{
							velocity.X /= 2f;
						}
						dashDelay = -1;
						for (int num25 = 0; num25 < 20; num25++)
						{
							int num26 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 6, 0f, 0f, 100, default(Color), 2f);
							Main.dust[num26].position.X += Main.rand.Next(-5, 6);
							Main.dust[num26].position.Y += Main.rand.Next(-5, 6);
							Main.dust[num26].velocity *= 0.2f;
							Main.dust[num26].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num26].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
							Main.dust[num26].noGravity = true;
							Main.dust[num26].fadeIn = 0.5f;
						}
					}
				}
			}
		}

		public void WallslideMovement()
		{
			sliding = false;
			if (slideDir == 0 || spikedBoots <= 0 || mount.Active || ((!controlLeft || slideDir != -1) && (!controlRight || slideDir != 1)))
			{
				return;
			}
			bool flag = false;
			float num = position.X;
			if (slideDir == 1)
			{
				num += (float)width;
			}
			num += (float)slideDir;
			float num2 = position.Y + (float)height + 1f;
			if (gravDir < 0f)
			{
				num2 = position.Y - 1f;
			}
			num /= 16f;
			num2 /= 16f;
			if (WorldGen.SolidTile((int)num, (int)num2) && WorldGen.SolidTile((int)num, (int)num2 - 1))
			{
				flag = true;
			}
			if (spikedBoots >= 2)
			{
				if (!flag || ((!(velocity.Y > 0f) || gravDir != 1f) && (!(velocity.Y < gravity) || gravDir != -1f)))
				{
					return;
				}
				float num3 = gravity;
				if (slowFall)
				{
					num3 = ((!controlUp) ? (gravity / 3f * gravDir) : (gravity / 10f * gravDir));
				}
				fallStart = (int)(position.Y / 16f);
				if ((controlDown && gravDir == 1f) || (controlUp && gravDir == -1f))
				{
					velocity.Y = 4f * gravDir;
					int num4 = Dust.NewDust(new Vector2(position.X + (float)(width / 2) + (float)((width / 2 - 4) * slideDir), position.Y + (float)(height / 2) + (float)(height / 2 - 4) * gravDir), 8, 8, 31);
					if (slideDir < 0)
					{
						Main.dust[num4].position.X -= 10f;
					}
					if (gravDir < 0f)
					{
						Main.dust[num4].position.Y -= 12f;
					}
					Main.dust[num4].velocity *= 0.1f;
					Main.dust[num4].scale *= 1.2f;
					Main.dust[num4].noGravity = true;
					Main.dust[num4].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
				}
				else if (gravDir == -1f)
				{
					velocity.Y = (0f - num3 + 1E-05f) * gravDir;
				}
				else
				{
					velocity.Y = (0f - num3 + 1E-05f) * gravDir;
				}
				sliding = true;
			}
			else if ((flag && (double)velocity.Y > 0.5 && gravDir == 1f) || ((double)velocity.Y < -0.5 && gravDir == -1f))
			{
				fallStart = (int)(position.Y / 16f);
				if (controlDown)
				{
					velocity.Y = 4f * gravDir;
				}
				else
				{
					velocity.Y = 0.5f * gravDir;
				}
				sliding = true;
				int num5 = Dust.NewDust(new Vector2(position.X + (float)(width / 2) + (float)((width / 2 - 4) * slideDir), position.Y + (float)(height / 2) + (float)(height / 2 - 4) * gravDir), 8, 8, 31);
				if (slideDir < 0)
				{
					Main.dust[num5].position.X -= 10f;
				}
				if (gravDir < 0f)
				{
					Main.dust[num5].position.Y -= 12f;
				}
				Main.dust[num5].velocity *= 0.1f;
				Main.dust[num5].scale *= 1.2f;
				Main.dust[num5].noGravity = true;
				Main.dust[num5].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
			}
		}

		public void CarpetMovement()
		{
			bool flag = false;
			if (grappling[0] == -1 && carpet && !jumpAgainCloud && !jumpAgainSandstorm && !jumpAgainBlizzard && !jumpAgainFart && !jumpAgainSail && !jumpAgainUnicorn && jump == 0 && velocity.Y != 0f && rocketTime == 0 && wingTime == 0f && !mount.Active)
			{
				if (controlJump && canCarpet)
				{
					canCarpet = false;
					carpetTime = 300;
				}
				if (carpetTime > 0 && controlJump)
				{
					fallStart = (int)(position.Y / 16f);
					flag = true;
					carpetTime--;
					float num = gravity;
					if (gravDir == 1f && velocity.Y > 0f - num)
					{
						velocity.Y = 0f - (num + 1E-06f);
					}
					else if (gravDir == -1f && velocity.Y < num)
					{
						velocity.Y = num + 1E-06f;
					}
					carpetFrameCounter += 1f + Math.Abs(velocity.X * 0.5f);
					if (carpetFrameCounter > 8f)
					{
						carpetFrameCounter = 0f;
						carpetFrame++;
					}
					if (carpetFrame < 0)
					{
						carpetFrame = 0;
					}
					if (carpetFrame > 5)
					{
						carpetFrame = 0;
					}
				}
			}
			if (!flag)
			{
				carpetFrame = -1;
			}
			else
			{
				slowFall = false;
			}
		}

		public void DoubleJumpVisuals()
		{
			if (dJumpEffectCloud && doubleJumpCloud && !jumpAgainCloud && (jumpAgainSandstorm || !doubleJumpSandstorm) && ((gravDir == 1f && velocity.Y < 0f) || (gravDir == -1f && velocity.Y > 0f)))
			{
				int num = height;
				if (gravDir == -1f)
				{
					num = -6;
				}
				int num2 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)num), width + 8, 4, 16, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 100, default(Color), 1.5f);
				Main.dust[num2].velocity.X = Main.dust[num2].velocity.X * 0.5f - velocity.X * 0.1f;
				Main.dust[num2].velocity.Y = Main.dust[num2].velocity.Y * 0.5f - velocity.Y * 0.3f;
			}
			if (dJumpEffectSandstorm && doubleJumpSandstorm && !jumpAgainSandstorm && ((gravDir == 1f && velocity.Y < 0f) || (gravDir == -1f && velocity.Y > 0f)))
			{
				int num3 = height;
				if (gravDir == -1f)
				{
					num3 = -6;
				}
				float num4 = ((float)jump / 75f + 1f) / 2f;
				for (int i = 0; i < 3; i++)
				{
					int num5 = Dust.NewDust(new Vector2(position.X, position.Y + (float)(num3 / 2)), width, 32, 124, velocity.X * 0.3f, velocity.Y * 0.3f, 150, default(Color), 1f * num4);
					Main.dust[num5].velocity *= 0.5f * num4;
					Main.dust[num5].fadeIn = 1.5f * num4;
				}
				sandStorm = true;
				if (miscCounter % 3 == 0)
				{
					int num6 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 18f, position.Y + (float)(num3 / 2)), new Vector2(0f - velocity.X, 0f - velocity.Y), Main.rand.Next(220, 223), num4);
					Main.gore[num6].velocity = velocity * 0.3f * num4;
					Main.gore[num6].alpha = 100;
				}
			}
			if (dJumpEffectFart && doubleJumpFart && !jumpAgainFart && ((gravDir == 1f && velocity.Y < 0f) || (gravDir == -1f && velocity.Y > 0f)))
			{
				int num7 = height;
				if (gravDir == -1f)
				{
					num7 = -6;
				}
				int num8 = Dust.NewDust(new Vector2(position.X - 4f, position.Y + (float)num7), width + 8, 4, 188, (0f - velocity.X) * 0.5f, velocity.Y * 0.5f, 100, default(Color), 1.5f);
				Main.dust[num8].velocity.X = Main.dust[num8].velocity.X * 0.5f - velocity.X * 0.1f;
				Main.dust[num8].velocity.Y = Main.dust[num8].velocity.Y * 0.5f - velocity.Y * 0.3f;
				Main.dust[num8].velocity *= 0.5f;
			}
			if (dJumpEffectUnicorn && doubleJumpUnicorn && !jumpAgainUnicorn && ((gravDir == 1f && velocity.Y < 0f) || (gravDir == -1f && velocity.Y > 0f)))
			{
				Dust dust = Main.dust[Dust.NewDust(position, width, height, Utils.SelectRandom<int>(Main.rand, 176, 177, 179))];
				dust.velocity = Vector2.Zero;
				dust.noGravity = true;
				dust.scale = 0.5f + Main.rand.NextFloat() * 0.8f;
				dust.fadeIn = 1f + Main.rand.NextFloat() * 2f;
				dust.shader = GameShaders.Armor.GetSecondaryShader(cMount, this);
			}
			if (dJumpEffectSail && doubleJumpSail && !jumpAgainSail && ((gravDir == 1f && velocity.Y < 1f) || (gravDir == -1f && velocity.Y > 1f)))
			{
				int num9 = 1;
				if (jump > 0)
				{
					num9 = 2;
				}
				int num10 = height - 6;
				if (gravDir == -1f)
				{
					num10 = 6;
				}
				for (int j = 0; j < num9; j++)
				{
					int num11 = Dust.NewDust(new Vector2(position.X, position.Y + (float)num10), width, 12, 253, velocity.X * 0.3f, velocity.Y * 0.3f, 100, default(Color), 1.5f);
					Main.dust[num11].scale += (float)Main.rand.Next(-5, 3) * 0.1f;
					if (jump <= 0)
					{
						Main.dust[num11].scale *= 0.8f;
					}
					else
					{
						Main.dust[num11].velocity -= velocity / 5f;
					}
					Main.dust[num11].noGravity = true;
					Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
					vector.Normalize();
					vector *= (float)Main.rand.Next(81) * 0.1f;
				}
			}
			if (!dJumpEffectBlizzard || !doubleJumpBlizzard || jumpAgainBlizzard || ((gravDir != 1f || !(velocity.Y < 0f)) && (gravDir != -1f || !(velocity.Y > 0f))))
			{
				return;
			}
			int num12 = height - 6;
			if (gravDir == -1f)
			{
				num12 = 6;
			}
			for (int k = 0; k < 2; k++)
			{
				int num13 = Dust.NewDust(new Vector2(position.X, position.Y + (float)num12), width, 12, 76, velocity.X * 0.3f, velocity.Y * 0.3f);
				Main.dust[num13].velocity *= 0.1f;
				if (k == 0)
				{
					Main.dust[num13].velocity += velocity * 0.03f;
				}
				else
				{
					Main.dust[num13].velocity -= velocity * 0.03f;
				}
				Main.dust[num13].noLight = true;
			}
			for (int l = 0; l < 3; l++)
			{
				int num14 = Dust.NewDust(new Vector2(position.X, position.Y + (float)num12), width, 12, 76, velocity.X * 0.3f, velocity.Y * 0.3f);
				Main.dust[num14].fadeIn = 1.5f;
				Main.dust[num14].velocity *= 0.6f;
				Main.dust[num14].velocity += velocity * 0.8f;
				Main.dust[num14].noGravity = true;
				Main.dust[num14].noLight = true;
			}
			for (int m = 0; m < 3; m++)
			{
				int num15 = Dust.NewDust(new Vector2(position.X, position.Y + (float)num12), width, 12, 76, velocity.X * 0.3f, velocity.Y * 0.3f);
				Main.dust[num15].fadeIn = 1.5f;
				Main.dust[num15].velocity *= 0.6f;
				Main.dust[num15].velocity -= velocity * 0.8f;
				Main.dust[num15].noGravity = true;
				Main.dust[num15].noLight = true;
			}
		}

		public void WingMovement()
		{
			if (wingsLogic == 4 && controlUp)
			{
				velocity.Y -= 0.2f * gravDir;
				if (gravDir == 1f)
				{
					if (velocity.Y > 0f)
					{
						velocity.Y -= 1f;
					}
					else if (velocity.Y > 0f - jumpSpeed)
					{
						velocity.Y -= 0.2f;
					}
					if (velocity.Y < (0f - jumpSpeed) * 3f)
					{
						velocity.Y = (0f - jumpSpeed) * 3f;
					}
				}
				else
				{
					if (velocity.Y < 0f)
					{
						velocity.Y += 1f;
					}
					else if (velocity.Y < jumpSpeed)
					{
						velocity.Y += 0.2f;
					}
					if (velocity.Y > jumpSpeed * 3f)
					{
						velocity.Y = jumpSpeed * 3f;
					}
				}
				wingTime -= 2f;
				return;
			}
			float num = 0.1f;
			float num2 = 0.5f;
			float num3 = 1.5f;
			float num4 = 0.5f;
			float num5 = 0.1f;
			if (wingsLogic == 26)
			{
				num2 = 0.75f;
				num5 = 0.15f;
				num4 = 1f;
				num3 = 2.5f;
				num = 0.125f;
			}
			if (wingsLogic == 29 || wingsLogic == 32)
			{
				num2 = 0.85f;
				num5 = 0.15f;
				num4 = 1f;
				num3 = 3f;
				num = 0.135f;
			}
			if (wingsLogic == 30 || wingsLogic == 31)
			{
				num4 = 1f;
				num3 = 2f;
				num = 0.15f;
			}
			velocity.Y -= num * gravDir;
			if (gravDir == 1f)
			{
				if (velocity.Y > 0f)
				{
					velocity.Y -= num2;
				}
				else if (velocity.Y > (0f - jumpSpeed) * num4)
				{
					velocity.Y -= num5;
				}
				if (velocity.Y < (0f - jumpSpeed) * num3)
				{
					velocity.Y = (0f - jumpSpeed) * num3;
				}
			}
			else
			{
				if (velocity.Y < 0f)
				{
					velocity.Y += num2;
				}
				else if (velocity.Y < jumpSpeed * num4)
				{
					velocity.Y += num5;
				}
				if (velocity.Y > jumpSpeed * num3)
				{
					velocity.Y = jumpSpeed * num3;
				}
			}
			if ((wingsLogic == 22 || wingsLogic == 28 || wingsLogic == 30 || wingsLogic == 31) && controlDown && !controlLeft && !controlRight)
			{
				wingTime -= 0.5f;
			}
			else
			{
				wingTime -= 1f;
			}
		}

		public void MoonLeechRope()
		{
			int num = -1;
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].type == 456 && Main.projectile[i].ai[1] == (float)whoAmI)
				{
					num = i;
					break;
				}
			}
			if (num != -1 && !(Main.projectile[num].ai[0] < 0f))
			{
				Projectile projectile = Main.projectile[num];
				Vector2 value = new Vector2(0f, 216f);
				Vector2 value2 = Main.npc[(int)Math.Abs(projectile.ai[0]) - 1].Center - base.Center + value;
				if (value2.Length() > 200f)
				{
					Vector2 value3 = Vector2.Normalize(value2);
					position += value3 * (value2.Length() - 200f);
				}
			}
		}

		public void WOFTongue()
		{
			if (Main.wof < 0 || !Main.npc[Main.wof].active)
			{
				return;
			}
			float num = Main.npc[Main.wof].position.X + 40f;
			if (Main.npc[Main.wof].direction > 0)
			{
				num -= 96f;
			}
			if (position.X + (float)width > num && position.X < num + 140f && gross)
			{
				noKnockback = false;
				Hurt(50, Main.npc[Main.wof].direction, false, false, Lang.deathMsg(name, -1, -1, -1, 4));
			}
			if (!gross && position.Y > (float)((Main.maxTilesY - 250) * 16) && position.X > num - 1920f && position.X < num + 1920f)
			{
				AddBuff(37, 10);
				Main.PlaySound(4, (int)Main.npc[Main.wof].position.X, (int)Main.npc[Main.wof].position.Y, 10);
			}
			if (gross)
			{
				if (position.Y < (float)((Main.maxTilesY - 200) * 16))
				{
					AddBuff(38, 10);
				}
				if (Main.npc[Main.wof].direction < 0)
				{
					if (position.X + (float)(width / 2) > Main.npc[Main.wof].position.X + (float)(Main.npc[Main.wof].width / 2) + 40f)
					{
						AddBuff(38, 10);
					}
				}
				else if (position.X + (float)(width / 2) < Main.npc[Main.wof].position.X + (float)(Main.npc[Main.wof].width / 2) - 40f)
				{
					AddBuff(38, 10);
				}
			}
			if (!tongued)
			{
				return;
			}
			controlHook = false;
			controlUseItem = false;
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].aiStyle == 7)
				{
					Main.projectile[i].Kill();
				}
			}
			Vector2 vector = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
			float num2 = Main.npc[Main.wof].position.X + (float)(Main.npc[Main.wof].width / 2) - vector.X;
			float num3 = Main.npc[Main.wof].position.Y + (float)(Main.npc[Main.wof].height / 2) - vector.Y;
			float num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
			if (num4 > 3000f)
			{
				KillMe(1000.0, 0, false, Language.GetTextValue("DeathText.TriedToEscape", Main.player[Main.myPlayer].name));
			}
			else if (Main.npc[Main.wof].position.X < 608f || Main.npc[Main.wof].position.X > (float)((Main.maxTilesX - 38) * 16))
			{
				KillMe(1000.0, 0, false, Language.GetTextValue("DeathText.WasLicked", Main.player[Main.myPlayer].name));
			}
		}

		public void StatusPlayer(NPC npc)
		{
			if (Main.expertMode && ((npc.type == 266 && Main.rand.Next(3) == 0) || npc.type == 267))
			{
				int num = Main.rand.Next(9);
				if (num == 2 || num == 4)
				{
					num = Main.rand.Next(9);
				}
				float num2 = (float)Main.rand.Next(75, 150) * 0.01f;
				switch (num)
				{
				case 0:
					AddBuff(20, (int)(60f * num2 * 3.5f));
					break;
				case 1:
					AddBuff(22, (int)(60f * num2 * 2f));
					break;
				case 2:
					AddBuff(23, (int)(60f * num2 * 0.5f));
					break;
				case 3:
					AddBuff(30, (int)(60f * num2 * 5f));
					break;
				case 4:
					AddBuff(31, (int)(60f * num2 * 1f));
					break;
				case 5:
					AddBuff(32, (int)(60f * num2 * 3.5f));
					break;
				case 6:
					AddBuff(33, (int)(60f * num2 * 7.5f));
					break;
				case 7:
					AddBuff(35, (int)(60f * num2 * 1f));
					break;
				case 8:
					AddBuff(36, (int)((double)(60f * num2) * 6.5));
					break;
				}
			}
			if (npc.type == 159 || npc.type == 158)
			{
				AddBuff(30, Main.rand.Next(300, 600));
			}
			if (npc.type == 525)
			{
				AddBuff(39, 420);
			}
			if (npc.type == 526)
			{
				AddBuff(69, 420);
			}
			if (npc.type == 527)
			{
				AddBuff(31, 840);
			}
			if (Main.expertMode && (npc.type == 49 || npc.type == 93 || npc.type == 51 || npc.type == 152) && Main.rand.Next(10) == 0)
			{
				AddBuff(148, Main.rand.Next(1800, 5400));
			}
			if (Main.expertMode && npc.type == 222)
			{
				AddBuff(20, Main.rand.Next(60, 240));
			}
			if (Main.expertMode && (npc.type == 210 || npc.type == 211))
			{
				AddBuff(20, Main.rand.Next(60, 180));
			}
			if (Main.expertMode && npc.type == 35)
			{
				AddBuff(30, Main.rand.Next(180, 300));
			}
			if (Main.expertMode && npc.type == 36 && Main.rand.Next(2) == 0)
			{
				AddBuff(32, Main.rand.Next(30, 60));
			}
			if (npc.type >= 269 && npc.type <= 272)
			{
				if (Main.rand.Next(3) == 0)
				{
					AddBuff(30, 600);
				}
				else if (Main.rand.Next(3) == 0)
				{
					AddBuff(32, 300);
				}
			}
			if (npc.type >= 273 && npc.type <= 276 && Main.rand.Next(2) == 0)
			{
				AddBuff(36, 600);
			}
			if (npc.type >= 277 && npc.type <= 280)
			{
				AddBuff(24, 600);
			}
			if (npc.type == 371)
			{
				AddBuff(103, 60 * Main.rand.Next(3, 8));
			}
			if (npc.type == 370 && Main.expertMode)
			{
				int num3 = Utils.SelectRandom<int>(Main.rand, 0, 148, 30);
				if (num3 != 0)
				{
					AddBuff(num3, 60 * Main.rand.Next(3, 11));
				}
			}
			if (((npc.type == 1 && npc.name == "Black Slime") || npc.type == 81 || npc.type == 79) && Main.rand.Next(4) == 0)
			{
				AddBuff(22, 900);
			}
			if ((npc.type == 23 || npc.type == 25) && Main.rand.Next(3) == 0)
			{
				AddBuff(24, 420);
			}
			if ((npc.type == 34 || npc.type == 83 || npc.type == 84) && Main.rand.Next(3) == 0)
			{
				AddBuff(23, 240);
			}
			if ((npc.type == 104 || npc.type == 102) && Main.rand.Next(8) == 0)
			{
				AddBuff(30, 2700);
			}
			if (npc.type == 75 && Main.rand.Next(10) == 0)
			{
				AddBuff(35, 420);
			}
			if ((npc.type == 163 || npc.type == 238) && Main.rand.Next(10) == 0)
			{
				AddBuff(70, 480);
			}
			if ((npc.type == 79 || npc.type == 103) && Main.rand.Next(5) == 0)
			{
				AddBuff(35, 420);
			}
			if ((npc.type == 75 || npc.type == 78 || npc.type == 82) && Main.rand.Next(8) == 0)
			{
				AddBuff(32, 900);
			}
			if ((npc.type == 93 || npc.type == 109 || npc.type == 80) && Main.rand.Next(14) == 0)
			{
				AddBuff(31, 300);
			}
			if (npc.type >= 305 && npc.type <= 314 && Main.rand.Next(10) == 0)
			{
				AddBuff(33, 3600);
			}
			if (npc.type == 77 && Main.rand.Next(6) == 0)
			{
				AddBuff(36, 7200);
			}
			if (npc.type == 112 && Main.rand.Next(20) == 0)
			{
				AddBuff(33, 18000);
			}
			if (npc.type == 182 && Main.rand.Next(25) == 0)
			{
				AddBuff(33, 7200);
			}
			if (npc.type == 141 && Main.rand.Next(2) == 0)
			{
				AddBuff(20, 600);
			}
			if (npc.type == 147 && !frozen && Main.rand.Next(12) == 0)
			{
				AddBuff(46, 600);
			}
			if (npc.type == 150)
			{
				if (Main.rand.Next(2) == 0)
				{
					AddBuff(46, 900);
				}
				if (!frozen && Main.rand.Next(35) == 0)
				{
					AddBuff(47, 60);
				}
				else if (!frozen && Main.expertMode && Main.rand.Next(35) == 0)
				{
					AddBuff(47, 60);
				}
			}
			if (npc.type == 184)
			{
				AddBuff(46, 1200);
				if (!frozen && Main.rand.Next(15) == 0)
				{
					AddBuff(47, 60);
				}
				else if (!frozen && Main.expertMode && Main.rand.Next(25) == 0)
				{
					AddBuff(47, 60);
				}
			}
		}

		public void GrappleMovement()
		{
			if (grappling[0] < 0)
			{
				return;
			}
			if (Main.myPlayer == whoAmI && mount.Active)
			{
				mount.Dismount(this);
			}
			canCarpet = true;
			carpetFrame = -1;
			wingFrame = 1;
			if (velocity.Y == 0f || (wet && (double)velocity.Y > -0.02 && (double)velocity.Y < 0.02))
			{
				wingFrame = 0;
			}
			if (wings == 4)
			{
				wingFrame = 3;
			}
			if (wings == 30)
			{
				wingFrame = 0;
			}
			wingTime = wingTimeMax;
			rocketTime = rocketTimeMax;
			rocketDelay = 0;
			rocketFrame = false;
			canRocket = false;
			rocketRelease = false;
			fallStart = (int)(position.Y / 16f);
			int num = -1;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < grapCount; i++)
			{
				Projectile projectile = Main.projectile[grappling[i]];
				num2 += projectile.position.X + (float)(projectile.width / 2);
				num3 += projectile.position.Y + (float)(projectile.height / 2);
				if (projectile.type == 403)
				{
					num = i;
				}
				else if (projectile.type == 446)
				{
					Vector2 vector = new Vector2(controlRight.ToInt() - controlLeft.ToInt(), (float)(controlDown.ToInt() - controlUp.ToInt()) * gravDir);
					if (vector != Vector2.Zero)
					{
						vector.Normalize();
					}
					vector *= 100f;
					Vector2 vector2 = Vector2.Normalize(base.Center - projectile.Center + vector);
					if (float.IsNaN(vector2.X) || float.IsNaN(vector2.Y))
					{
						vector2 = -Vector2.UnitY;
					}
					float num4 = 200f;
					num2 += vector2.X * num4;
					num3 += vector2.Y * num4;
				}
			}
			num2 /= (float)grapCount;
			num3 /= (float)grapCount;
			Vector2 vector3 = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
			float num5 = num2 - vector3.X;
			float num6 = num3 - vector3.Y;
			float num7 = (float)Math.Sqrt(num5 * num5 + num6 * num6);
			float num8 = 11f;
			if (Main.projectile[grappling[0]].type == 315)
			{
				num8 = 16f;
			}
			if (Main.projectile[grappling[0]].type >= 646 && Main.projectile[grappling[0]].type <= 649)
			{
				num8 = 13f;
			}
			float num9 = num7;
			num9 = ((!(num7 > num8)) ? 1f : (num8 / num7));
			num5 *= num9;
			num6 *= num9;
			if (num6 > 0f)
			{
				GoingDownWithGrapple = true;
			}
			velocity.X = num5;
			velocity.Y = num6;
			if (num != -1)
			{
				Projectile projectile2 = Main.projectile[grappling[num]];
				if (projectile2.position.X < position.X + (float)width && projectile2.position.X + (float)projectile2.width >= position.X && projectile2.position.Y < position.Y + (float)height && projectile2.position.Y + (float)projectile2.height >= position.Y)
				{
					int num10 = (int)(projectile2.position.X + (float)(projectile2.width / 2)) / 16;
					int num11 = (int)(projectile2.position.Y + (float)(projectile2.height / 2)) / 16;
					velocity = Vector2.Zero;
					if (Main.tile[num10, num11].type == 314)
					{
						Vector2 Position = default(Vector2);
						Position.X = projectile2.position.X + (float)(projectile2.width / 2) - (float)(width / 2);
						Position.Y = projectile2.position.Y + (float)(projectile2.height / 2) - (float)(height / 2);
						grappling[0] = -1;
						grapCount = 0;
						for (int j = 0; j < 1000; j++)
						{
							if (Main.projectile[j].active && Main.projectile[j].owner == whoAmI && Main.projectile[j].aiStyle == 7)
							{
								Main.projectile[j].Kill();
							}
						}
						int num12 = 13;
						if (miscEquips[2].stack > 0 && miscEquips[2].mountType >= 0 && MountID.Sets.Cart[miscEquips[2].mountType] && (!miscEquips[2].expertOnly || Main.expertMode))
						{
							num12 = miscEquips[2].mountType;
						}
						int num13 = height + Mount.GetHeightBoost(num12);
						if (Minecart.GetOnTrack(num10, num11, ref Position, width, num13) && !Collision.SolidCollision(Position, width, num13 - 20))
						{
							position = Position;
							DelegateMethods.Minecart.rotation = fullRotation;
							DelegateMethods.Minecart.rotationOrigin = fullRotationOrigin;
							mount.SetMount(num12, this, minecartLeft);
							Minecart.WheelSparks(mount.MinecartDust, position, width, height, 25);
						}
					}
				}
			}
			if (itemAnimation == 0)
			{
				if (velocity.X > 0f)
				{
					ChangeDir(1);
				}
				if (velocity.X < 0f)
				{
					ChangeDir(-1);
				}
			}
			if (controlJump)
			{
				if (!releaseJump)
				{
					return;
				}
				if ((velocity.Y == 0f || (wet && (double)velocity.Y > -0.02 && (double)velocity.Y < 0.02)) && !controlDown)
				{
					velocity.Y = 0f - jumpSpeed;
					jump = jumpHeight / 2;
					releaseJump = false;
				}
				else
				{
					velocity.Y += 0.01f;
					releaseJump = false;
				}
				if (doubleJumpCloud)
				{
					jumpAgainCloud = true;
				}
				if (doubleJumpSandstorm)
				{
					jumpAgainSandstorm = true;
				}
				if (doubleJumpBlizzard)
				{
					jumpAgainBlizzard = true;
				}
				if (doubleJumpFart)
				{
					jumpAgainFart = true;
				}
				if (doubleJumpSail)
				{
					jumpAgainSail = true;
				}
				if (doubleJumpUnicorn)
				{
					jumpAgainUnicorn = true;
				}
				grappling[0] = 0;
				grapCount = 0;
				for (int k = 0; k < 1000; k++)
				{
					if (Main.projectile[k].active && Main.projectile[k].owner == whoAmI && Main.projectile[k].aiStyle == 7)
					{
						Main.projectile[k].Kill();
					}
				}
			}
			else
			{
				releaseJump = true;
			}
		}

		public void StickyMovement()
		{
			bool flag = false;
			if (mount.Type == 6 && Math.Abs(velocity.X) > 5f)
			{
				flag = true;
			}
			if (mount.Type == 13 && Math.Abs(velocity.X) > 5f)
			{
				flag = true;
			}
			if (mount.Type == 11 && Math.Abs(velocity.X) > 5f)
			{
				flag = true;
			}
			int num = width / 2;
			int num2 = height / 2;
			new Vector2(position.X + (float)(width / 2) - (float)(num / 2), position.Y + (float)(height / 2) - (float)(num2 / 2));
			Vector2 vector = Collision.StickyTiles(position, velocity, width, height);
			if (vector.Y != -1f && vector.X != -1f)
			{
				int num3 = (int)vector.X;
				int num4 = (int)vector.Y;
				int type = Main.tile[num3, num4].type;
				if (whoAmI == Main.myPlayer && type == 51 && (velocity.X != 0f || velocity.Y != 0f))
				{
					stickyBreak++;
					if (stickyBreak > Main.rand.Next(20, 100) || flag)
					{
						stickyBreak = 0;
						WorldGen.KillTile(num3, num4);
						if (Main.netMode == 1 && !Main.tile[num3, num4].active() && Main.netMode == 1)
						{
							NetMessage.SendData(17, -1, -1, "", 0, num3, num4);
						}
					}
				}
				if (flag)
				{
					return;
				}
				fallStart = (int)(position.Y / 16f);
				if (type != 229)
				{
					jump = 0;
				}
				if (velocity.X > 1f)
				{
					velocity.X = 1f;
				}
				if (velocity.X < -1f)
				{
					velocity.X = -1f;
				}
				if (velocity.Y > 1f)
				{
					velocity.Y = 1f;
				}
				if (velocity.Y < -5f)
				{
					velocity.Y = -5f;
				}
				if ((double)velocity.X > 0.75 || (double)velocity.X < -0.75)
				{
					velocity.X *= 0.85f;
				}
				else
				{
					velocity.X *= 0.6f;
				}
				if (velocity.Y < 0f)
				{
					velocity.Y *= 0.96f;
				}
				else
				{
					velocity.Y *= 0.3f;
				}
				if (type != 229 || Main.rand.Next(5) != 0 || (!((double)velocity.Y > 0.15) && !(velocity.Y < 0f)))
				{
					return;
				}
				if ((float)(num3 * 16) < position.X + (float)(width / 2))
				{
					int num5 = Dust.NewDust(new Vector2(position.X - 4f, num4 * 16), 4, 16, 153, 0f, 0f, 50);
					Main.dust[num5].scale += (float)Main.rand.Next(0, 6) * 0.1f;
					Main.dust[num5].velocity *= 0.1f;
					Main.dust[num5].noGravity = true;
				}
				else
				{
					int num6 = Dust.NewDust(new Vector2(position.X + (float)width - 2f, num4 * 16), 4, 16, 153, 0f, 0f, 50);
					Main.dust[num6].scale += (float)Main.rand.Next(0, 6) * 0.1f;
					Main.dust[num6].velocity *= 0.1f;
					Main.dust[num6].noGravity = true;
				}
				if (Main.tile[num3, num4 + 1] != null && Main.tile[num3, num4 + 1].type == 229 && position.Y + (float)height > (float)((num4 + 1) * 16))
				{
					if ((float)(num3 * 16) < position.X + (float)(width / 2))
					{
						int num7 = Dust.NewDust(new Vector2(position.X - 4f, num4 * 16 + 16), 4, 16, 153, 0f, 0f, 50);
						Main.dust[num7].scale += (float)Main.rand.Next(0, 6) * 0.1f;
						Main.dust[num7].velocity *= 0.1f;
						Main.dust[num7].noGravity = true;
					}
					else
					{
						int num8 = Dust.NewDust(new Vector2(position.X + (float)width - 2f, num4 * 16 + 16), 4, 16, 153, 0f, 0f, 50);
						Main.dust[num8].scale += (float)Main.rand.Next(0, 6) * 0.1f;
						Main.dust[num8].velocity *= 0.1f;
						Main.dust[num8].noGravity = true;
					}
				}
				if (Main.tile[num3, num4 + 2] != null && Main.tile[num3, num4 + 2].type == 229 && position.Y + (float)height > (float)((num4 + 2) * 16))
				{
					if ((float)(num3 * 16) < position.X + (float)(width / 2))
					{
						int num9 = Dust.NewDust(new Vector2(position.X - 4f, num4 * 16 + 32), 4, 16, 153, 0f, 0f, 50);
						Main.dust[num9].scale += (float)Main.rand.Next(0, 6) * 0.1f;
						Main.dust[num9].velocity *= 0.1f;
						Main.dust[num9].noGravity = true;
					}
					else
					{
						int num10 = Dust.NewDust(new Vector2(position.X + (float)width - 2f, num4 * 16 + 32), 4, 16, 153, 0f, 0f, 50);
						Main.dust[num10].scale += (float)Main.rand.Next(0, 6) * 0.1f;
						Main.dust[num10].velocity *= 0.1f;
						Main.dust[num10].noGravity = true;
					}
				}
			}
			else
			{
				stickyBreak = 0;
			}
		}

		public bool IsStackingItems()
		{
			for (int i = 0; i < inventoryChestStack.Length; i++)
			{
				if (inventoryChestStack[i])
				{
					if (inventory[i].type != 0 && inventory[i].stack != 0)
					{
						return true;
					}
					inventoryChestStack[i] = false;
				}
			}
			return false;
		}

		public void QuickStackAllChests()
		{
			if (IsStackingItems())
			{
				return;
			}
			if (Main.netMode == 1)
			{
				for (int i = 10; i < 50; i++)
				{
					if (inventory[i].type > 0 && inventory[i].stack > 0 && !inventory[i].favorited && (inventory[i].type < 71 || inventory[i].type > 74))
					{
						NetMessage.SendData(5, -1, -1, "", whoAmI, i, (int)inventory[i].prefix);
						NetMessage.SendData(85, -1, -1, "", i);
						inventoryChestStack[i] = true;
					}
				}
				return;
			}
			bool flag = false;
			for (int j = 10; j < 50; j++)
			{
				if (inventory[j].type > 0 && inventory[j].stack > 0 && !inventory[j].favorited)
				{
					int type = inventory[j].type;
					int stack = inventory[j].stack;
					inventory[j] = Chest.PutItemInNearbyChest(inventory[j], base.Center);
					if (inventory[j].type != type || inventory[j].stack != stack)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				Main.PlaySound(7);
			}
		}

		public void CheckDrowning()
		{
			bool flag = Collision.DrownCollision(position, width, height, gravDir);
			if (armor[0].type == 250)
			{
				flag = true;
			}
			if (inventory[selectedItem].type == 186)
			{
				try
				{
					int num = (int)((position.X + (float)(width / 2) + (float)(6 * direction)) / 16f);
					int num2 = 0;
					if (gravDir == -1f)
					{
						num2 = height;
					}
					int num3 = (int)((position.Y + (float)num2 - 44f * gravDir) / 16f);
					if (Main.tile[num, num3].liquid < 128)
					{
						if (Main.tile[num, num3] == null)
						{
							Main.tile[num, num3] = new Tile();
						}
						if (!Main.tile[num, num3].active() || !Main.tileSolid[Main.tile[num, num3].type] || Main.tileSolidTop[Main.tile[num, num3].type])
						{
							flag = false;
						}
					}
				}
				catch
				{
				}
			}
			if (gills)
			{
				flag = false;
			}
			if (Main.myPlayer == whoAmI)
			{
				if (merman)
				{
					flag = false;
				}
				if (flag)
				{
					breathCD++;
					int num4 = 7;
					if (inventory[selectedItem].type == 186)
					{
						num4 *= 2;
					}
					if (accDivingHelm)
					{
						num4 *= 4;
					}
					if (breathCD >= num4)
					{
						breathCD = 0;
						breath--;
						if (breath == 0)
						{
							Main.PlaySound(23);
						}
						if (breath <= 0)
						{
							lifeRegenTime = 0;
							breath = 0;
							statLife -= 2;
							if (statLife <= 0)
							{
								statLife = 0;
								KillMe(10.0, 0, false, Lang.deathMsg(name, -1, -1, -1, 1));
							}
						}
					}
				}
				else
				{
					breath += 3;
					if (breath > breathMax)
					{
						breath = breathMax;
					}
					breathCD = 0;
				}
			}
			if (flag && Main.rand.Next(20) == 0 && !lavaWet && !honeyWet)
			{
				int num5 = 0;
				if (gravDir == -1f)
				{
					num5 += height - 12;
				}
				if (inventory[selectedItem].type == 186)
				{
					Dust.NewDust(new Vector2(position.X + (float)(10 * direction) + 4f, position.Y + (float)num5 - 54f * gravDir), width - 8, 8, 34, 0f, 0f, 0, default(Color), 1.2f);
				}
				else
				{
					Dust.NewDust(new Vector2(position.X + (float)(12 * direction), position.Y + (float)num5 + 4f * gravDir), width - 8, 8, 34, 0f, 0f, 0, default(Color), 1.2f);
				}
			}
		}

		public void CheckIceBreak()
		{
			if (!(velocity.Y > 7f))
			{
				return;
			}
			Vector2 vector = position + velocity;
			int num = (int)(vector.X / 16f);
			int num2 = (int)((vector.X + (float)width) / 16f);
			int num3 = (int)((position.Y + (float)height + 1f) / 16f);
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num3 + 1; j++)
				{
					if (Main.tile[i, j].nactive() && Main.tile[i, j].type == 162 && !WorldGen.SolidTile(i, j - 1))
					{
						WorldGen.KillTile(i, j);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(17, -1, -1, "", 0, i, j);
						}
					}
				}
			}
		}

		public void SlopeDownMovement()
		{
			sloping = false;
			float y = velocity.Y;
			Vector4 vector = Collision.WalkDownSlope(position, velocity, width, height, gravity * gravDir);
			position.X = vector.X;
			position.Y = vector.Y;
			velocity.X = vector.Z;
			velocity.Y = vector.W;
			if (velocity.Y != y)
			{
				sloping = true;
			}
		}

		public void HoneyCollision(bool fallThrough, bool ignorePlats)
		{
			int height = (!onTrack) ? base.height : (base.height - 20);
			Vector2 velocity = base.velocity;
			base.velocity = Collision.TileCollision(position, base.velocity, width, height, fallThrough, ignorePlats, (int)gravDir);
			Vector2 vector = base.velocity * 0.25f;
			if (base.velocity.X != velocity.X)
			{
				vector.X = base.velocity.X;
			}
			if (base.velocity.Y != velocity.Y)
			{
				vector.Y = base.velocity.Y;
			}
			position += vector;
		}

		public void WaterCollision(bool fallThrough, bool ignorePlats)
		{
			int height = (!onTrack) ? base.height : (base.height - 20);
			Vector2 velocity = base.velocity;
			base.velocity = Collision.TileCollision(position, base.velocity, width, height, fallThrough, ignorePlats, (int)gravDir);
			Vector2 vector = base.velocity * 0.5f;
			if (base.velocity.X != velocity.X)
			{
				vector.X = base.velocity.X;
			}
			if (base.velocity.Y != velocity.Y)
			{
				vector.Y = base.velocity.Y;
			}
			position += vector;
		}

		public void DryCollision(bool fallThrough, bool ignorePlats)
		{
			int height = (!onTrack) ? base.height : (base.height - 10);
			if (base.velocity.Length() > 16f)
			{
				Vector2 vector = Collision.TileCollision(position, base.velocity, width, height, fallThrough, ignorePlats, (int)gravDir);
				float num = base.velocity.Length();
				Vector2 value = Vector2.Normalize(base.velocity);
				if (vector.Y == 0f)
				{
					value.Y = 0f;
				}
				Vector2 zero = Vector2.Zero;
				bool flag = mount.Type == 7 || mount.Type == 8 || mount.Type == 12;
				Vector2 zero2 = Vector2.Zero;
				while (num > 0f)
				{
					float num2 = num;
					if (num2 > 16f)
					{
						num2 = 16f;
					}
					num -= num2;
					Vector2 vector2 = base.velocity = value * num2;
					SlopeDownMovement();
					vector2 = base.velocity;
					if (base.velocity.Y == gravity && (!mount.Active || (!mount.Cart && !flag)))
					{
						Collision.StepDown(ref position, ref vector2, width, base.height, ref stepSpeed, ref gfxOffY, (int)gravDir, waterWalk || waterWalk2);
					}
					if (gravDir == -1f)
					{
						if ((carpetFrame != -1 || base.velocity.Y <= gravity) && !controlUp)
						{
							Collision.StepUp(ref position, ref vector2, width, base.height, ref stepSpeed, ref gfxOffY, (int)gravDir, controlUp);
						}
					}
					else if (flag || ((carpetFrame != -1 || base.velocity.Y >= gravity) && !controlDown && !mount.Cart))
					{
						Collision.StepUp(ref position, ref vector2, width, base.height, ref stepSpeed, ref gfxOffY, (int)gravDir, controlUp);
					}
					Vector2 vector3 = Collision.TileCollision(position, vector2, width, height, fallThrough, ignorePlats, (int)gravDir);
					if (Collision.up && gravDir == 1f)
					{
						jump = 0;
					}
					if (waterWalk || waterWalk2)
					{
						Vector2 velocity = base.velocity;
						vector3 = Collision.WaterCollision(position, vector3, width, base.height, fallThrough, false, waterWalk);
						if (velocity != base.velocity)
						{
							fallStart = (int)(position.Y / 16f);
						}
					}
					position += vector3;
					bool falling = false;
					if (vector3.Y > gravity)
					{
						falling = true;
					}
					if (vector3.Y < 0f - gravity)
					{
						falling = true;
					}
					base.velocity = vector3;
					UpdateTouchingTiles();
					TryBouncingBlocks(falling);
					TryLandingOnDetonator();
					SlopingCollision(fallThrough);
					vector3 = base.velocity;
					zero += vector3;
				}
				base.velocity = zero;
				return;
			}
			base.velocity = Collision.TileCollision(position, base.velocity, width, height, fallThrough, ignorePlats, (int)gravDir);
			if (Collision.up && gravDir == 1f)
			{
				jump = 0;
			}
			if (waterWalk || waterWalk2)
			{
				Vector2 velocity2 = base.velocity;
				base.velocity = Collision.WaterCollision(position, base.velocity, width, base.height, fallThrough, false, waterWalk);
				if (velocity2 != base.velocity)
				{
					fallStart = (int)(position.Y / 16f);
				}
			}
			position += base.velocity;
		}

		public void SlopingCollision(bool fallThrough)
		{
			if (controlDown || grappling[0] >= 0 || gravDir == -1f)
			{
				stairFall = true;
			}
			Vector4 vector = Collision.SlopeCollision(position, velocity, width, height, gravity, stairFall);
			if (Collision.stairFall)
			{
				stairFall = true;
			}
			else if (!fallThrough)
			{
				stairFall = false;
			}
			if (Collision.stair && Math.Abs(vector.Y - position.Y) > 8f + Math.Abs(velocity.X))
			{
				gfxOffY -= vector.Y - position.Y;
				stepSpeed = 4f;
			}
			float y = velocity.Y;
			position.X = vector.X;
			position.Y = vector.Y;
			velocity.X = vector.Z;
			velocity.Y = vector.W;
			if (gravDir == -1f && velocity.Y == 0.0101f)
			{
				velocity.Y = 0f;
			}
		}

		public void FloorVisuals(bool Falling)
		{
			int num = (int)((position.X + (float)(width / 2)) / 16f);
			int num2 = (int)((position.Y + (float)height) / 16f);
			if (gravDir == -1f)
			{
				num2 = (int)(position.Y - 0.1f) / 16;
			}
			int num3 = -1;
			if (Main.tile[num - 1, num2] == null)
			{
				Main.tile[num - 1, num2] = new Tile();
			}
			if (Main.tile[num + 1, num2] == null)
			{
				Main.tile[num + 1, num2] = new Tile();
			}
			if (Main.tile[num, num2] == null)
			{
				Main.tile[num, num2] = new Tile();
			}
			if (Main.tile[num, num2].nactive() && Main.tileSolid[Main.tile[num, num2].type])
			{
				num3 = Main.tile[num, num2].type;
			}
			else if (Main.tile[num - 1, num2].nactive() && Main.tileSolid[Main.tile[num - 1, num2].type])
			{
				num3 = Main.tile[num - 1, num2].type;
			}
			else if (Main.tile[num + 1, num2].nactive() && Main.tileSolid[Main.tile[num + 1, num2].type])
			{
				num3 = Main.tile[num + 1, num2].type;
			}
			if (num3 <= -1)
			{
				slippy = false;
				slippy2 = false;
				sticky = false;
				powerrun = false;
				return;
			}
			sticky = (num3 == 229);
			slippy = (num3 == 161 || num3 == 162 || num3 == 163 || num3 == 164 || num3 == 200 || num3 == 127);
			slippy2 = (num3 == 197);
			powerrun = (num3 == 198);
			if (Main.tile[num - 1, num2].slope() != 0 || Main.tile[num, num2].slope() != 0 || Main.tile[num + 1, num2].slope() != 0)
			{
				num3 = -1;
			}
			if (wet || mount.Cart || (num3 != 147 && num3 != 25 && num3 != 53 && num3 != 189 && num3 != 0 && num3 != 123 && num3 != 57 && num3 != 112 && num3 != 116 && num3 != 196 && num3 != 193 && num3 != 195 && num3 != 197 && num3 != 199 && num3 != 229 && num3 != 371))
			{
				return;
			}
			int num4 = 1;
			if (Falling)
			{
				num4 = 20;
			}
			for (int i = 0; i < num4; i++)
			{
				bool flag = true;
				int num5 = 76;
				if (num3 == 53)
				{
					num5 = 32;
				}
				if (num3 == 189)
				{
					num5 = 16;
				}
				if (num3 == 0)
				{
					num5 = 0;
				}
				if (num3 == 123)
				{
					num5 = 53;
				}
				if (num3 == 57)
				{
					num5 = 36;
				}
				if (num3 == 112)
				{
					num5 = 14;
				}
				if (num3 == 116)
				{
					num5 = 51;
				}
				if (num3 == 196)
				{
					num5 = 108;
				}
				if (num3 == 193)
				{
					num5 = 4;
				}
				if (num3 == 195 || num3 == 199)
				{
					num5 = 5;
				}
				if (num3 == 197)
				{
					num5 = 4;
				}
				if (num3 == 229)
				{
					num5 = 153;
				}
				if (num3 == 371)
				{
					num5 = 243;
				}
				if (num3 == 25)
				{
					num5 = 37;
				}
				if (num5 == 32 && Main.rand.Next(2) == 0)
				{
					flag = false;
				}
				if (num5 == 14 && Main.rand.Next(2) == 0)
				{
					flag = false;
				}
				if (num5 == 51 && Main.rand.Next(2) == 0)
				{
					flag = false;
				}
				if (num5 == 36 && Main.rand.Next(2) == 0)
				{
					flag = false;
				}
				if (num5 == 0 && Main.rand.Next(3) != 0)
				{
					flag = false;
				}
				if (num5 == 53 && Main.rand.Next(3) != 0)
				{
					flag = false;
				}
				Color newColor = default(Color);
				if (num3 == 193)
				{
					newColor = new Color(30, 100, 255, 100);
				}
				if (num3 == 197)
				{
					newColor = new Color(97, 200, 255, 100);
				}
				if (!Falling)
				{
					float num6 = Math.Abs(velocity.X) / 3f;
					if ((float)Main.rand.Next(100) > num6 * 100f)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					continue;
				}
				float num7 = velocity.X;
				if (num7 > 6f)
				{
					num7 = 6f;
				}
				if (num7 < -6f)
				{
					num7 = -6f;
				}
				if (velocity.X == 0f && !Falling)
				{
					continue;
				}
				int num8 = Dust.NewDust(new Vector2(position.X, position.Y + (float)height - 2f), width, 6, num5, 0f, 0f, 50, newColor);
				if (gravDir == -1f)
				{
					Main.dust[num8].position.Y -= height + 4;
				}
				if (num5 == 76)
				{
					Main.dust[num8].scale += (float)Main.rand.Next(3) * 0.1f;
					Main.dust[num8].noLight = true;
				}
				if (num5 == 16 || num5 == 108 || num5 == 153)
				{
					Main.dust[num8].scale += (float)Main.rand.Next(6) * 0.1f;
				}
				if (num5 == 37)
				{
					Main.dust[num8].scale += 0.25f;
					Main.dust[num8].alpha = 50;
				}
				if (num5 == 5)
				{
					Main.dust[num8].scale += (float)Main.rand.Next(2, 8) * 0.1f;
				}
				Main.dust[num8].noGravity = true;
				if (num4 > 1)
				{
					Main.dust[num8].velocity.X *= 1.2f;
					Main.dust[num8].velocity.Y *= 0.8f;
					Main.dust[num8].velocity.Y -= 1f;
					Main.dust[num8].velocity *= 0.8f;
					Main.dust[num8].scale += (float)Main.rand.Next(3) * 0.1f;
					Main.dust[num8].velocity.X = (Main.dust[num8].position.X - (position.X + (float)(width / 2))) * 0.2f;
					if (Main.dust[num8].velocity.Y > 0f)
					{
						Main.dust[num8].velocity.Y *= -1f;
					}
					Main.dust[num8].velocity.X += num7 * 0.3f;
				}
				else
				{
					Main.dust[num8].velocity *= 0.2f;
				}
				Main.dust[num8].position.X -= num7 * 1f;
				if (gravDir == -1f)
				{
					Main.dust[num8].velocity.Y *= -1f;
				}
			}
		}

		public void BordersMovement()
		{
			if (position.X < Main.leftWorld + 640f + 16f)
			{
				position.X = Main.leftWorld + 640f + 16f;
				velocity.X = 0f;
			}
			if (position.X + (float)width > Main.rightWorld - 640f - 32f)
			{
				position.X = Main.rightWorld - 640f - 32f - (float)width;
				velocity.X = 0f;
			}
			if (position.Y < Main.topWorld + 640f + 16f)
			{
				position.Y = Main.topWorld + 640f + 16f;
				if ((double)velocity.Y < 0.11)
				{
					velocity.Y = 0.11f;
				}
				gravDir = 1f;
				AchievementsHelper.HandleSpecialEvent(this, 11);
			}
			if (position.Y > Main.bottomWorld - 640f - 32f - (float)height)
			{
				position.Y = Main.bottomWorld - 640f - 32f - (float)height;
				velocity.Y = 0f;
			}
			if (position.Y > Main.bottomWorld - 640f - 150f - (float)height)
			{
				AchievementsHelper.HandleSpecialEvent(this, 10);
			}
		}

		public void CollectTaxes()
		{
			int num = Item.buyPrice(0, 0, 0, 50);
			int num2 = Item.buyPrice(0, 10);
			if (!NPC.taxCollector || taxMoney >= num2)
			{
				return;
			}
			int num3 = 0;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].homeless && NPC.TypeToNum(Main.npc[i].type) > 0)
				{
					num3++;
				}
			}
			taxMoney += num * num3;
			if (taxMoney > num2)
			{
				taxMoney = num2;
			}
		}

		public void Update(int i)
		{
			if (launcherWait > 0)
			{
				launcherWait--;
			}
			maxFallSpeed = 10f;
			gravity = defaultGravity;
			jumpHeight = 15;
			jumpSpeed = 5.01f;
			maxRunSpeed = 3f;
			runAcceleration = 0.08f;
			runSlowdown = 0.2f;
			accRunSpeed = maxRunSpeed;
			if (!mount.Active || !mount.Cart)
			{
				onWrongGround = false;
			}
			heldProj = -1;
			if (PortalPhysicsEnabled)
			{
				maxFallSpeed = 35f;
			}
			if (wet)
			{
				if (honeyWet)
				{
					gravity = 0.1f;
					maxFallSpeed = 3f;
				}
				else if (merman)
				{
					gravity = 0.3f;
					maxFallSpeed = 7f;
				}
				else
				{
					gravity = 0.2f;
					maxFallSpeed = 5f;
					jumpHeight = 30;
					jumpSpeed = 6.01f;
				}
			}
			if (vortexDebuff)
			{
				gravity = 0f;
			}
			maxFallSpeed += 0.01f;
			bool flag = false;
			if (Main.myPlayer == i)
			{
				TileObject.objectPreview.Reset();
			}
			if (active)
			{
				if (ghostDmg > 0f)
				{
					ghostDmg -= 2.5f;
				}
				if (ghostDmg < 0f)
				{
					ghostDmg = 0f;
				}
				if (Main.expertMode)
				{
					if (lifeSteal < 70f)
					{
						lifeSteal += 0.5f;
					}
					if (lifeSteal > 70f)
					{
						lifeSteal = 70f;
					}
				}
				else
				{
					if (lifeSteal < 80f)
					{
						lifeSteal += 0.6f;
					}
					if (lifeSteal > 80f)
					{
						lifeSteal = 80f;
					}
				}
				if (mount.Active)
				{
					base.position.Y += height;
					height = 42 + mount.HeightBoost;
					base.position.Y -= height;
					if (mount.Type == 0)
					{
						int num = (int)(base.position.X + (float)(width / 2)) / 16;
						int j = (int)(base.position.Y + (float)(height / 2) - 14f) / 16;
						Lighting.AddLight(num, j, 0.5f, 0.2f, 0.05f);
						Lighting.AddLight(num + direction, j, 0.5f, 0.2f, 0.05f);
						Lighting.AddLight(num + direction * 2, j, 0.5f, 0.2f, 0.05f);
					}
				}
				else
				{
					base.position.Y += height;
					height = 42;
					base.position.Y -= height;
				}
				Main.numPlayers++;
				outOfRange = false;
				if (whoAmI != Main.myPlayer)
				{
					int num2 = (int)(base.position.X + (float)(width / 2)) / 16;
					int num3 = (int)(base.position.Y + (float)(height / 2)) / 16;
					if (!WorldGen.InWorld(num2, num3, 4))
					{
						flag = true;
					}
					else if (Main.tile[num2, num3] == null)
					{
						flag = true;
					}
					else if (Main.tile[num2 - 3, num3] == null)
					{
						flag = true;
					}
					else if (Main.tile[num2 + 3, num3] == null)
					{
						flag = true;
					}
					else if (Main.tile[num2, num3 - 3] == null)
					{
						flag = true;
					}
					else if (Main.tile[num2, num3 + 3] == null)
					{
						flag = true;
					}
					if (flag)
					{
						outOfRange = true;
						numMinions = 0;
						slotsMinions = 0f;
						itemAnimation = 0;
						PlayerFrame();
					}
				}
				if (tankPet >= 0)
				{
					if (!tankPetReset)
					{
						tankPetReset = true;
					}
					else
					{
						tankPet = -1;
					}
				}
			}
			if (chatOverhead.timeLeft > 0)
			{
				chatOverhead.timeLeft--;
			}
			if (!active || flag)
			{
				return;
			}
			miscCounter++;
			if (miscCounter >= 300)
			{
				miscCounter = 0;
			}
			infernoCounter++;
			if (infernoCounter >= 180)
			{
				infernoCounter = 0;
			}
			float num4 = Main.maxTilesX / 4200;
			if (Main.maxTilesX == 1750)
			{
				num4 = (float)Main.maxTilesX / 4200f;
			}
			num4 *= num4;
			float num5 = (float)((double)(base.position.Y / 16f - (60f + 10f * num4)) / (Main.worldSurface / 6.0));
			if ((double)num5 < 0.25)
			{
				num5 = 0.25f;
			}
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			gravity *= num5;
			maxRegenDelay = (1f - (float)statMana / (float)statManaMax2) * 60f * 4f + 45f;
			maxRegenDelay *= 0.7f;
			UpdateSocialShadow();
			UpdateTeleportVisuals();
			whoAmI = i;
			if (whoAmI == Main.myPlayer)
			{
				TryPortalJumping();
			}
			if (runSoundDelay > 0)
			{
				runSoundDelay--;
			}
			if (attackCD > 0)
			{
				attackCD--;
			}
			if (itemAnimation == 0)
			{
				attackCD = 0;
			}
			if (potionDelay > 0)
			{
				potionDelay--;
			}
			if (i == Main.myPlayer)
			{
				if (trashItem.type >= 1522 && trashItem.type <= 1527)
				{
					trashItem.SetDefaults();
				}
				UpdateBiomes();
				UpdateMinionTarget();
			}
			if (ghost)
			{
				Ghost();
				return;
			}
			if (dead)
			{
				UpdateDead();
				return;
			}
			if (i == Main.myPlayer)
			{
				controlUp = false;
				controlLeft = false;
				controlDown = false;
				controlRight = false;
				controlJump = false;
				controlUseItem = false;
				controlUseTile = false;
				controlThrow = false;
				controlInv = false;
				controlHook = false;
				controlTorch = false;
				controlSmart = false;
				controlMount = false;
				mapStyle = false;
				mapAlphaDown = false;
				mapAlphaUp = false;
				mapFullScreen = false;
				mapZoomIn = false;
				mapZoomOut = false;
				bool flag2 = false;
				bool flag3 = false;
				Keys[] pressedKeys = Main.keyState.GetPressedKeys();
				for (int k = 0; k < pressedKeys.Length; k++)
				{
					if (pressedKeys[k] == Keys.LeftShift || pressedKeys[k] == Keys.RightShift)
					{
						flag2 = true;
					}
					else if (pressedKeys[k] == Keys.LeftAlt || pressedKeys[k] == Keys.RightAlt)
					{
						flag3 = true;
					}
				}
				if (Main.hasFocus)
				{
					if (!Main.chatMode && !Main.editSign && !Main.editChest && !Main.blockInput)
					{
						if (Main.blockKey != 0)
						{
							bool flag4 = false;
							for (int l = 0; l < pressedKeys.Length; l++)
							{
								if (pressedKeys[l] == Main.blockKey)
								{
									pressedKeys[l] = Keys.None;
									flag4 = true;
								}
							}
							if (!flag4)
							{
								Main.blockKey = Keys.None;
							}
						}
						bool flag5 = false;
						bool flag6 = false;
						for (int m = 0; m < pressedKeys.Length; m++)
						{
							string a = string.Concat(pressedKeys[m]);
							if (pressedKeys[m] == Keys.Tab && ((flag2 && SocialAPI.Mode == SocialMode.Steam) || flag3))
							{
								continue;
							}
							if (a == Main.cUp)
							{
								controlUp = true;
							}
							if (a == Main.cLeft)
							{
								controlLeft = true;
							}
							if (a == Main.cDown)
							{
								controlDown = true;
							}
							if (a == Main.cRight)
							{
								controlRight = true;
							}
							if (a == Main.cJump)
							{
								controlJump = true;
							}
							if (a == Main.cThrowItem)
							{
								controlThrow = true;
							}
							if (a == Main.cInv)
							{
								controlInv = true;
							}
							if (a == Main.cBuff)
							{
								QuickBuff();
							}
							if (a == Main.cHeal)
							{
								flag6 = true;
							}
							if (a == Main.cMana)
							{
								flag5 = true;
							}
							if (a == Main.cHook)
							{
								controlHook = true;
							}
							if (a == Main.cTorch)
							{
								controlTorch = true;
							}
							if (a == Main.cSmart)
							{
								controlSmart = true;
							}
							if (a == Main.cMount)
							{
								controlMount = true;
							}
							if (Main.mapEnabled)
							{
								if (a == Main.cMapZoomIn)
								{
									mapZoomIn = true;
								}
								if (a == Main.cMapZoomOut)
								{
									mapZoomOut = true;
								}
								if (a == Main.cMapAlphaUp)
								{
									mapAlphaUp = true;
								}
								if (a == Main.cMapAlphaDown)
								{
									mapAlphaDown = true;
								}
								if (a == Main.cMapFull)
								{
									mapFullScreen = true;
								}
								if (a == Main.cMapStyle)
								{
									mapStyle = true;
								}
							}
						}
						if (Main.gamePad)
						{
							GamePadState state = GamePad.GetState(PlayerIndex.One);
							if (state.DPad.Up == ButtonState.Pressed)
							{
								controlUp = true;
							}
							if (state.DPad.Down == ButtonState.Pressed)
							{
								controlDown = true;
							}
							if (state.DPad.Left == ButtonState.Pressed)
							{
								controlLeft = true;
							}
							if (state.DPad.Right == ButtonState.Pressed)
							{
								controlRight = true;
							}
							if (state.Triggers.Left > 0f)
							{
								controlJump = true;
							}
							if (state.Triggers.Right > 0f)
							{
								controlUseItem = true;
							}
							Main.mouseX = (int)((float)(Main.screenWidth / 2) + state.ThumbSticks.Right.X * (float)tileRangeX * 16f);
							Main.mouseY = (int)((float)(Main.screenHeight / 2) - state.ThumbSticks.Right.Y * (float)tileRangeX * 16f);
							if (state.ThumbSticks.Right.X == 0f)
							{
								Main.mouseX = Main.screenWidth / 2 + direction * 2;
							}
						}
						if (Main.mapFullscreen)
						{
							if (controlUp)
							{
								Main.mapFullscreenPos.Y -= 1f * (16f / Main.mapFullscreenScale);
							}
							if (controlDown)
							{
								Main.mapFullscreenPos.Y += 1f * (16f / Main.mapFullscreenScale);
							}
							if (controlLeft)
							{
								Main.mapFullscreenPos.X -= 1f * (16f / Main.mapFullscreenScale);
							}
							if (controlRight)
							{
								Main.mapFullscreenPos.X += 1f * (16f / Main.mapFullscreenScale);
							}
							controlUp = false;
							controlLeft = false;
							controlDown = false;
							controlRight = false;
							controlJump = false;
							controlUseItem = false;
							controlUseTile = false;
							controlThrow = false;
							controlHook = false;
							controlTorch = false;
							controlSmart = false;
							controlMount = false;
						}
						if (flag6)
						{
							if (releaseQuickHeal)
							{
								QuickHeal();
							}
							releaseQuickHeal = false;
						}
						else
						{
							releaseQuickHeal = true;
						}
						if (flag5)
						{
							if (releaseQuickMana)
							{
								QuickMana();
							}
							releaseQuickMana = false;
						}
						else
						{
							releaseQuickMana = true;
						}
						if (controlLeft && controlRight)
						{
							controlLeft = false;
							controlRight = false;
						}
						if (Main.cSmartToggle)
						{
							if (controlSmart && releaseSmart)
							{
								Main.PlaySound(12);
								Main.smartDigEnabled = !Main.smartDigEnabled;
							}
						}
						else
						{
							if (Main.smartDigEnabled != controlSmart)
							{
								Main.PlaySound(12);
							}
							Main.smartDigEnabled = controlSmart;
						}
						if (controlSmart)
						{
							releaseSmart = false;
						}
						else
						{
							releaseSmart = true;
						}
						if (controlMount)
						{
							if (releaseMount)
							{
								QuickMount();
							}
							releaseMount = false;
						}
						else
						{
							releaseMount = true;
						}
						if (Main.mapFullscreen)
						{
							if (mapZoomIn)
							{
								Main.mapFullscreenScale *= 1.05f;
							}
							if (mapZoomOut)
							{
								Main.mapFullscreenScale *= 0.95f;
							}
						}
						else
						{
							if (Main.mapStyle == 1)
							{
								if (mapZoomIn)
								{
									Main.mapMinimapScale *= 1.025f;
								}
								if (mapZoomOut)
								{
									Main.mapMinimapScale *= 0.975f;
								}
								if (mapAlphaUp)
								{
									Main.mapMinimapAlpha += 0.015f;
								}
								if (mapAlphaDown)
								{
									Main.mapMinimapAlpha -= 0.015f;
								}
							}
							else if (Main.mapStyle == 2)
							{
								if (mapZoomIn)
								{
									Main.mapOverlayScale *= 1.05f;
								}
								if (mapZoomOut)
								{
									Main.mapOverlayScale *= 0.95f;
								}
								if (mapAlphaUp)
								{
									Main.mapOverlayAlpha += 0.015f;
								}
								if (mapAlphaDown)
								{
									Main.mapOverlayAlpha -= 0.015f;
								}
							}
							if (mapStyle)
							{
								if (releaseMapStyle)
								{
									Main.PlaySound(12);
									Main.mapStyle++;
									if (Main.mapStyle > 2)
									{
										Main.mapStyle = 0;
									}
								}
								releaseMapStyle = false;
							}
							else
							{
								releaseMapStyle = true;
							}
						}
						if (mapFullScreen)
						{
							if (releaseMapFullscreen)
							{
								if (Main.mapFullscreen)
								{
									Main.PlaySound(11);
									Main.mapFullscreen = false;
								}
								else
								{
									Main.playerInventory = false;
									talkNPC = -1;
									Main.npcChatCornerItem = 0;
									Main.PlaySound(10);
									float num6 = Main.mapFullscreenScale = 2.5f;
									Main.mapFullscreen = true;
									Main.resetMapFull = true;
									Main.buffString = string.Empty;
								}
							}
							releaseMapFullscreen = false;
						}
						else
						{
							releaseMapFullscreen = true;
						}
					}
					if (confused)
					{
						bool flag7 = controlLeft;
						bool flag8 = controlUp;
						controlLeft = controlRight;
						controlRight = flag7;
						controlUp = controlRight;
						controlDown = flag8;
					}
					else if (cartFlip)
					{
						if (controlRight || controlLeft)
						{
							bool flag9 = controlLeft;
							controlLeft = controlRight;
							controlRight = flag9;
						}
						else
						{
							cartFlip = false;
						}
					}
					for (int n = 0; n < doubleTapCardinalTimer.Length; n++)
					{
						doubleTapCardinalTimer[n]--;
						if (doubleTapCardinalTimer[n] < 0)
						{
							doubleTapCardinalTimer[n] = 0;
						}
					}
					for (int num7 = 0; num7 < 4; num7++)
					{
						bool flag10 = false;
						bool flag11 = false;
						switch (num7)
						{
						case 0:
							flag10 = (controlDown && releaseDown);
							flag11 = controlDown;
							break;
						case 1:
							flag10 = (controlUp && releaseUp);
							flag11 = controlUp;
							break;
						case 2:
							flag10 = (controlRight && releaseRight);
							flag11 = controlRight;
							break;
						case 3:
							flag10 = (controlLeft && releaseLeft);
							flag11 = controlLeft;
							break;
						}
						if (flag10)
						{
							if (doubleTapCardinalTimer[num7] > 0)
							{
								KeyDoubleTap(num7);
							}
							else
							{
								doubleTapCardinalTimer[num7] = 15;
							}
						}
						if (flag11)
						{
							holdDownCardinalTimer[num7]++;
							KeyHoldDown(num7, holdDownCardinalTimer[num7]);
						}
						else
						{
							holdDownCardinalTimer[num7] = 0;
						}
					}
					if (Main.mouseLeft)
					{
						if (!Main.blockMouse && !mouseInterface)
						{
							controlUseItem = true;
						}
					}
					else
					{
						Main.blockMouse = false;
					}
					if (Main.mouseRight && !mouseInterface && !Main.blockMouse)
					{
						controlUseTile = true;
					}
					if (controlInv)
					{
						if (releaseInventory)
						{
							if (Main.mapFullscreen)
							{
								Main.mapFullscreen = false;
								releaseInventory = false;
								Main.PlaySound(11);
							}
							else
							{
								ToggleInv();
							}
						}
						releaseInventory = false;
					}
					else
					{
						releaseInventory = true;
					}
					if (delayUseItem)
					{
						if (!controlUseItem)
						{
							delayUseItem = false;
						}
						controlUseItem = false;
					}
					if (itemAnimation == 0 && itemTime == 0)
					{
						dropItemCheck();
						int num8 = selectedItem;
						bool flag12 = false;
						if (!Main.chatMode && selectedItem != 58 && !Main.editSign && !Main.editChest)
						{
							if (Main.keyState.IsKeyDown(Keys.D1))
							{
								selectedItem = 0;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D2))
							{
								selectedItem = 1;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D3))
							{
								selectedItem = 2;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D4))
							{
								selectedItem = 3;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D5))
							{
								selectedItem = 4;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D6))
							{
								selectedItem = 5;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D7))
							{
								selectedItem = 6;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D8))
							{
								selectedItem = 7;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D9))
							{
								selectedItem = 8;
								flag12 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D0))
							{
								selectedItem = 9;
								flag12 = true;
							}
							if (controlTorch && flag12)
							{
								if (selectedItem != nonTorch)
								{
									Main.PlaySound(12);
								}
								nonTorch = selectedItem;
								selectedItem = num8;
								flag12 = false;
							}
						}
						bool flag13 = Main.hairWindow;
						if (flag13)
						{
							int y = Main.screenHeight / 2 + 60;
							int x = Main.screenWidth / 2 - Main.hairStyleBackTexture.Width / 2;
							flag13 = new Rectangle(x, y, Main.hairStyleBackTexture.Width, Main.hairStyleBackTexture.Height).Contains(Main.MouseScreen.ToPoint());
						}
						if (flag12 && CaptureManager.Instance.Active)
						{
							CaptureManager.Instance.Active = false;
						}
						if (num8 != selectedItem)
						{
							Main.PlaySound(12);
						}
						if (Main.mapFullscreen)
						{
							int num9 = (Main.mouseState.ScrollWheelValue - Main.oldMouseWheel) / 120;
							Main.mapFullscreenScale *= 1f + (float)num9 * 0.3f;
						}
						else if (CaptureManager.Instance.Active)
						{
							CaptureManager.Instance.Scrolling();
						}
						else if (!flag13)
						{
							if (!Main.playerInventory)
							{
								int num10;
								for (num10 = (Main.mouseState.ScrollWheelValue - Main.oldMouseWheel) / 120; num10 > 9; num10 -= 10)
								{
								}
								for (; num10 < 0; num10 += 10)
								{
								}
								selectedItem -= num10;
								if (num10 != 0)
								{
									Main.PlaySound(12);
								}
								if (changeItem >= 0)
								{
									if (selectedItem != changeItem)
									{
										Main.PlaySound(12);
									}
									selectedItem = changeItem;
									changeItem = -1;
								}
								if (itemAnimation == 0)
								{
									while (selectedItem > 9)
									{
										selectedItem -= 10;
									}
									while (selectedItem < 0)
									{
										selectedItem += 10;
									}
								}
							}
							else
							{
								int num11 = (Main.mouseState.ScrollWheelValue - Main.oldMouseWheel) / 120;
								bool flag14 = true;
								if (Main.recBigList)
								{
									int num12 = 42;
									int num13 = 340;
									int num14 = 310;
									int num15 = (Main.screenWidth - num14 - 280) / num12;
									int num16 = (Main.screenHeight - num13 - 20) / num12;
									if (new Rectangle(num14, num13, num15 * num12, num16 * num12).Contains(Main.MouseScreen.ToPoint()))
									{
										num11 *= -1;
										int num17 = Math.Sign(num11);
										while (num11 != 0)
										{
											if (num11 < 0)
											{
												Main.recStart -= num15;
												if (Main.recStart < 0)
												{
													Main.recStart = 0;
												}
											}
											else
											{
												Main.recStart += num15;
												if (Main.recStart > Main.numAvailableRecipes - num15)
												{
													Main.recStart = Main.numAvailableRecipes - num15;
												}
											}
											num11 -= num17;
										}
									}
								}
								if (flag14)
								{
									Main.focusRecipe += num11;
									if (Main.focusRecipe > Main.numAvailableRecipes - 1)
									{
										Main.focusRecipe = Main.numAvailableRecipes - 1;
									}
									if (Main.focusRecipe < 0)
									{
										Main.focusRecipe = 0;
									}
								}
							}
						}
					}
					else
					{
						bool flag15 = false;
						if (!Main.chatMode && selectedItem != 58 && !Main.editSign && !Main.editChest)
						{
							int num18 = -1;
							if (Main.keyState.IsKeyDown(Keys.D1))
							{
								num18 = 0;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D2))
							{
								num18 = 1;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D3))
							{
								num18 = 2;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D4))
							{
								num18 = 3;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D5))
							{
								num18 = 4;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D6))
							{
								num18 = 5;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D7))
							{
								num18 = 6;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D8))
							{
								num18 = 7;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D9))
							{
								num18 = 8;
								flag15 = true;
							}
							if (Main.keyState.IsKeyDown(Keys.D0))
							{
								num18 = 9;
								flag15 = true;
							}
							if (flag15)
							{
								if (num18 != nonTorch)
								{
									Main.PlaySound(12);
								}
								nonTorch = num18;
							}
						}
					}
				}
				if (selectedItem == 58)
				{
					nonTorch = -1;
				}
				else
				{
					SmartitemLookup();
				}
				if (stoned != lastStoned)
				{
					if (whoAmI == Main.myPlayer && stoned)
					{
						int damage = (int)(20.0 * (double)Main.damageMultiplier);
						Hurt(damage, 0, false, false, Lang.deathMsg(name, -1, -1, -1, 4));
					}
					Main.PlaySound(0, (int)base.position.X, (int)base.position.Y);
					for (int num19 = 0; num19 < 20; num19++)
					{
						int num20 = Dust.NewDust(base.position, width, height, 1);
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num20].noGravity = true;
						}
					}
				}
				lastStoned = stoned;
				if (frozen || webbed || stoned)
				{
					controlJump = false;
					controlDown = false;
					controlLeft = false;
					controlRight = false;
					controlUp = false;
					controlUseItem = false;
					controlUseTile = false;
					controlThrow = false;
					gravDir = 1f;
				}
				if (!controlThrow)
				{
					releaseThrow = true;
				}
				else
				{
					releaseThrow = false;
				}
				if (Main.netMode == 1)
				{
					bool flag16 = false;
					if (controlUp != Main.clientPlayer.controlUp)
					{
						flag16 = true;
					}
					if (controlDown != Main.clientPlayer.controlDown)
					{
						flag16 = true;
					}
					if (controlLeft != Main.clientPlayer.controlLeft)
					{
						flag16 = true;
					}
					if (controlRight != Main.clientPlayer.controlRight)
					{
						flag16 = true;
					}
					if (controlJump != Main.clientPlayer.controlJump)
					{
						flag16 = true;
					}
					if (controlUseItem != Main.clientPlayer.controlUseItem)
					{
						flag16 = true;
					}
					if (selectedItem != Main.clientPlayer.selectedItem)
					{
						flag16 = true;
					}
					if (flag16)
					{
						NetMessage.SendData(13, -1, -1, "", Main.myPlayer);
					}
				}
				if (Main.playerInventory)
				{
					AdjTiles();
				}
				if (chest != -1)
				{
					if (chest != -2)
					{
						flyingPigChest = -1;
					}
					if (flyingPigChest >= 0)
					{
						if (!Main.projectile[flyingPigChest].active || Main.projectile[flyingPigChest].type != 525)
						{
							Main.PlaySound(2, -1, -1, 59);
							chest = -1;
							Recipe.FindRecipes();
						}
						else
						{
							int num21 = (int)(((double)base.position.X + (double)width * 0.5) / 16.0);
							int num22 = (int)(((double)base.position.Y + (double)height * 0.5) / 16.0);
							chestX = (int)Main.projectile[flyingPigChest].Center.X / 16;
							chestY = (int)Main.projectile[flyingPigChest].Center.Y / 16;
							if (num21 < chestX - tileRangeX || num21 > chestX + tileRangeX + 1 || num22 < chestY - tileRangeY || num22 > chestY + tileRangeY + 1)
							{
								if (chest != -1)
								{
									Main.PlaySound(2, -1, -1, 59);
								}
								chest = -1;
								Recipe.FindRecipes();
							}
						}
					}
					else
					{
						int num23 = (int)(((double)base.position.X + (double)width * 0.5) / 16.0);
						int num24 = (int)(((double)base.position.Y + (double)height * 0.5) / 16.0);
						if (num23 < chestX - tileRangeX || num23 > chestX + tileRangeX + 1 || num24 < chestY - tileRangeY || num24 > chestY + tileRangeY + 1)
						{
							if (chest != -1)
							{
								Main.PlaySound(11);
							}
							chest = -1;
							Recipe.FindRecipes();
						}
						else if (!Main.tile[chestX, chestY].active())
						{
							Main.PlaySound(11);
							chest = -1;
							Recipe.FindRecipes();
						}
					}
				}
				else
				{
					flyingPigChest = -1;
				}
				if (base.velocity.Y <= 0f)
				{
					fallStart2 = (int)(base.position.Y / 16f);
				}
				if (base.velocity.Y == 0f)
				{
					int num25 = 25;
					num25 += extraFall;
					int num26 = (int)(base.position.Y / 16f) - fallStart;
					if (mount.CanFly)
					{
						num26 = 0;
					}
					if (mount.Cart && Minecart.OnTrack(base.position, width, height))
					{
						num26 = 0;
					}
					if (mount.Type == 1)
					{
						num26 = 0;
					}
					mount.FatigueRecovery();
					bool flag17 = false;
					for (int num27 = 3; num27 < 10; num27++)
					{
						if (armor[num27].stack > 0 && armor[num27].wingSlot > -1)
						{
							flag17 = true;
						}
					}
					if (stoned)
					{
						int num28 = (int)(((float)num26 * gravDir - 2f) * 20f);
						if (num28 > 0)
						{
							Hurt(num28, 0, false, false, Lang.deathMsg(name, -1, -1, -1, 4));
							immune = false;
						}
					}
					else if (((gravDir == 1f && num26 > num25) || (gravDir == -1f && num26 < -num25)) && !noFallDmg && !flag17)
					{
						immune = false;
						int num29 = (int)((float)num26 * gravDir - (float)num25) * 10;
						if (mount.Active)
						{
							num29 = (int)((float)num29 * mount.FallDamage);
						}
						Hurt(num29, 0, false, false, Lang.deathMsg(name, -1, -1, -1, 0));
						if (!dead && statLife <= statLifeMax2 / 10)
						{
							AchievementsHelper.HandleSpecialEvent(this, 8);
						}
					}
					fallStart = (int)(base.position.Y / 16f);
				}
				if (jump > 0 || rocketDelay > 0 || wet || slowFall || (double)num5 < 0.8 || tongued)
				{
					fallStart = (int)(base.position.Y / 16f);
				}
			}
			if (Main.netMode != 1)
			{
				if (chest == -1 && lastChest >= 0 && Main.chest[lastChest] != null && Main.chest[lastChest] != null)
				{
					int x2 = Main.chest[lastChest].x;
					int y2 = Main.chest[lastChest].y;
					NPC.BigMimicSummonCheck(x2, y2);
				}
				lastChest = chest;
			}
			if (mouseInterface)
			{
				delayUseItem = true;
			}
			tileTargetX = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
			tileTargetY = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
			if (gravDir == -1f)
			{
				tileTargetY = (int)((Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY) / 16f);
			}
			if (tileTargetX >= Main.maxTilesX - 5)
			{
				tileTargetX = Main.maxTilesX - 5;
			}
			if (tileTargetY >= Main.maxTilesY - 5)
			{
				tileTargetY = Main.maxTilesY - 5;
			}
			if (tileTargetX < 5)
			{
				tileTargetX = 5;
			}
			if (tileTargetY < 5)
			{
				tileTargetY = 5;
			}
			if (Main.tile[tileTargetX - 1, tileTargetY] == null)
			{
				Main.tile[tileTargetX - 1, tileTargetY] = new Tile();
			}
			if (Main.tile[tileTargetX + 1, tileTargetY] == null)
			{
				Main.tile[tileTargetX + 1, tileTargetY] = new Tile();
			}
			if (Main.tile[tileTargetX, tileTargetY] == null)
			{
				Main.tile[tileTargetX, tileTargetY] = new Tile();
			}
			if (!Main.tile[tileTargetX, tileTargetY].active())
			{
				if (Main.tile[tileTargetX - 1, tileTargetY].active() && Main.tile[tileTargetX - 1, tileTargetY].type == 323)
				{
					int frameY = Main.tile[tileTargetX - 1, tileTargetY].frameY;
					if (frameY < -4)
					{
						tileTargetX++;
					}
					if (frameY > 4)
					{
						tileTargetX--;
					}
				}
				else if (Main.tile[tileTargetX + 1, tileTargetY].active() && Main.tile[tileTargetX + 1, tileTargetY].type == 323)
				{
					int frameY2 = Main.tile[tileTargetX + 1, tileTargetY].frameY;
					if (frameY2 < -4)
					{
						tileTargetX++;
					}
					if (frameY2 > 4)
					{
						tileTargetX--;
					}
				}
			}
			SmartCursorLookup();
			UpdateImmunity();
			if (petalTimer > 0)
			{
				petalTimer--;
			}
			if (shadowDodgeTimer > 0)
			{
				shadowDodgeTimer--;
			}
			if (jump > 0 || base.velocity.Y != 0f)
			{
				slippy = false;
				slippy2 = false;
				powerrun = false;
				sticky = false;
			}
			potionDelayTime = Item.potionDelay;
			restorationDelayTime = Item.restorationDelay;
			if (pStone)
			{
				potionDelayTime = (int)((double)potionDelayTime * 0.75);
				restorationDelayTime = (int)((double)restorationDelayTime * 0.75);
			}
			if (yoraiz0rEye > 0)
			{
				Yoraiz0rEye();
			}
			ResetEffects();
			UpdateDyes(i);
			meleeCrit += inventory[selectedItem].crit;
			magicCrit += inventory[selectedItem].crit;
			rangedCrit += inventory[selectedItem].crit;
			thrownCrit += inventory[selectedItem].crit;
			if (whoAmI == Main.myPlayer)
			{
				Main.musicBox2 = -1;
				if (Main.waterCandles > 0)
				{
					AddBuff(86, 2, false);
				}
				if (Main.peaceCandles > 0)
				{
					AddBuff(157, 2, false);
				}
				if (Main.campfire)
				{
					AddBuff(87, 2, false);
				}
				if (Main.starInBottle)
				{
					AddBuff(158, 2, false);
				}
				if (Main.heartLantern)
				{
					AddBuff(89, 2, false);
				}
				if (Main.sunflower)
				{
					AddBuff(146, 2, false);
				}
				if (hasBanner)
				{
					AddBuff(147, 2, false);
				}
			}
			for (int num30 = 0; num30 < 191; num30++)
			{
				buffImmune[num30] = false;
			}
			UpdateBuffs(i);
			if (whoAmI == Main.myPlayer)
			{
				if (!onFire && !poisoned)
				{
					trapDebuffSource = false;
				}
				UpdatePet(i);
				UpdatePetLight(i);
			}
			bool flag18 = wet && !lavaWet && (!mount.Active || mount.Type != 3);
			if (accMerman && flag18)
			{
				releaseJump = true;
				wings = 0;
				merman = true;
				accFlipper = true;
				AddBuff(34, 2);
			}
			else
			{
				merman = false;
			}
			if (!flag18 && forceWerewolf)
			{
				forceMerman = false;
			}
			if (forceMerman && flag18)
			{
				wings = 0;
			}
			accMerman = false;
			hideMerman = false;
			forceMerman = false;
			if (wolfAcc && !merman && !Main.dayTime && !wereWolf)
			{
				AddBuff(28, 60);
			}
			wolfAcc = false;
			hideWolf = false;
			forceWerewolf = false;
			if (whoAmI == Main.myPlayer)
			{
				for (int num31 = 0; num31 < 22; num31++)
				{
					if (buffType[num31] > 0 && buffTime[num31] <= 0)
					{
						DelBuff(num31);
					}
				}
			}
			beetleDefense = false;
			beetleOffense = false;
			doubleJumpCloud = false;
			setSolar = false;
			head = armor[0].headSlot;
			body = armor[1].bodySlot;
			legs = armor[2].legSlot;
			handon = -1;
			handoff = -1;
			back = -1;
			front = -1;
			shoe = -1;
			waist = -1;
			shield = -1;
			neck = -1;
			face = -1;
			balloon = -1;
			if (MountFishronSpecialCounter > 0f)
			{
				MountFishronSpecialCounter -= 1f;
			}
			if (_portalPhysicsTime > 0)
			{
				_portalPhysicsTime--;
			}
			UpdateEquips(i);
			if (base.velocity.Y == 0f || controlJump)
			{
				portalPhysicsFlag = false;
			}
			if (inventory[selectedItem].type == 3384 || portalPhysicsFlag)
			{
				_portalPhysicsTime = 30;
			}
			if (mount.Active)
			{
				mount.UpdateEffects(this);
			}
			gemCount++;
			if (gemCount >= 10)
			{
				gem = -1;
				gemCount = 0;
				for (int num32 = 0; num32 <= 58; num32++)
				{
					if (inventory[num32].type == 0 || inventory[num32].stack == 0)
					{
						inventory[num32].type = 0;
						inventory[num32].stack = 0;
						inventory[num32].netID = 0;
					}
					if (inventory[num32].type >= 1522 && inventory[num32].type <= 1527)
					{
						gem = inventory[num32].type - 1522;
					}
				}
			}
			if (!vortexStealthActive)
			{
				float num33 = 0f;
				float num34 = 0f;
				float num35 = 0f;
				switch (head)
				{
				case 11:
					num33 = 0.92f;
					num34 = 0.8f;
					num35 = 0.65f;
					break;
				case 169:
					num33 = 0f;
					num34 = 0.36f;
					num35 = 0.4f;
					break;
				case 170:
					num33 = 0.4f;
					num34 = 0.16f;
					num35 = 0.36f;
					break;
				case 171:
					num33 = 0.5f;
					num34 = 0.25f;
					num35 = 0.05f;
					break;
				case 189:
					num33 = 0.9f;
					num34 = 0.9f;
					num35 = 0.7f;
					break;
				case 178:
					num33 = 0.1f;
					num34 = 0.2f;
					num35 = 0.3f;
					break;
				}
				float num36 = 0f;
				float num37 = 0f;
				float num38 = 0f;
				switch (body)
				{
				case 175:
					num36 = 0f;
					num37 = 0.36f;
					num38 = 0.4f;
					break;
				case 176:
					num36 = 0.4f;
					num37 = 0.16f;
					num38 = 0.36f;
					break;
				case 177:
					num36 = 0.5f;
					num37 = 0.25f;
					num38 = 0.05f;
					break;
				case 190:
					num33 = 0.9f;
					num34 = 0.9f;
					num35 = 0.7f;
					break;
				}
				float num39 = 0f;
				float num40 = 0f;
				float num41 = 0f;
				switch (legs)
				{
				case 110:
					num39 = 0f;
					num40 = 0.36f;
					num41 = 0.4f;
					break;
				case 111:
					num39 = 0.4f;
					num40 = 0.16f;
					num41 = 0.36f;
					break;
				case 112:
					num39 = 0.5f;
					num40 = 0.25f;
					num41 = 0.05f;
					break;
				case 130:
					num33 = 0.9f;
					num34 = 0.9f;
					num35 = 0.7f;
					break;
				}
				if (num33 != 0f || num34 != 0f || num35 != 0f)
				{
					float num42 = 1f;
					if (num33 == num36 && num34 == num37 && num35 == num38)
					{
						num42 += 0.5f;
					}
					if (num33 == num39 && num34 == num40 && num35 == num41)
					{
						num42 += 0.5f;
					}
					Vector2 spinningpoint = new Vector2(width / 2 + 8 * direction, 2f);
					if (fullRotation != 0f)
					{
						spinningpoint = spinningpoint.RotatedBy(fullRotation, fullRotationOrigin);
					}
					int i2 = (int)(base.position.X + spinningpoint.X) / 16;
					int j2 = (int)(base.position.Y + spinningpoint.Y) / 16;
					Lighting.AddLight(i2, j2, num33 * num42, num34 * num42, num35 * num42);
				}
				if (num36 != 0f || num37 != 0f || num38 != 0f)
				{
					float num43 = 1f;
					if (num36 == num33 && num37 == num34 && num38 == num35)
					{
						num43 += 0.5f;
					}
					if (num36 == num39 && num37 == num40 && num38 == num41)
					{
						num43 += 0.5f;
					}
					Vector2 spinningpoint2 = new Vector2(width / 2 + 8, height / 2);
					if (fullRotation != 0f)
					{
						spinningpoint2 = spinningpoint2.RotatedBy(fullRotation, fullRotationOrigin);
					}
					int i3 = (int)(base.position.X + spinningpoint2.X) / 16;
					int j3 = (int)(base.position.Y + spinningpoint2.Y) / 16;
					Lighting.AddLight(i3, j3, num36 * num43, num37 * num43, num38 * num43);
				}
				if (num39 != 0f || num40 != 0f || num41 != 0f)
				{
					float num44 = 1f;
					if (num39 == num36 && num40 == num37 && num41 == num38)
					{
						num44 += 0.5f;
					}
					if (num39 == num33 && num40 == num34 && num41 == num35)
					{
						num44 += 0.5f;
					}
					Vector2 spinningpoint3 = new Vector2(width / 2 + 8 * direction, (float)height * 0.75f);
					if (fullRotation != 0f)
					{
						spinningpoint3 = spinningpoint3.RotatedBy(fullRotation, fullRotationOrigin);
					}
					int i4 = (int)(base.position.X + spinningpoint3.X) / 16;
					int j4 = (int)(base.position.Y + spinningpoint3.Y) / 16;
					Lighting.AddLight(i4, j4, num39 * num44, num40 * num44, num41 * num44);
				}
			}
			UpdateArmorSets(i);
			if ((merman || forceMerman) && flag18)
			{
				wings = 0;
			}
			if (invis)
			{
				if (itemAnimation == 0 && aggro > -750)
				{
					aggro = -750;
				}
				else if (aggro > -250)
				{
					aggro = -250;
				}
			}
			if (inventory[selectedItem].type == 3106)
			{
				if (itemAnimation > 0)
				{
					stealthTimer = 15;
					if (stealth > 0f)
					{
						stealth += 0.1f;
					}
				}
				else if ((double)base.velocity.X > -0.1 && (double)base.velocity.X < 0.1 && (double)base.velocity.Y > -0.1 && (double)base.velocity.Y < 0.1 && !mount.Active)
				{
					if (stealthTimer == 0 && stealth > 0f)
					{
						stealth -= 0.02f;
						if ((double)stealth <= 0.0)
						{
							stealth = 0f;
							if (Main.netMode == 1)
							{
								NetMessage.SendData(84, -1, -1, "", whoAmI);
							}
						}
					}
				}
				else
				{
					if (stealth > 0f)
					{
						stealth += 0.1f;
					}
					if (mount.Active)
					{
						stealth = 1f;
					}
				}
				if (stealth > 1f)
				{
					stealth = 1f;
				}
				meleeDamage += (1f - stealth) * 3f;
				meleeCrit += (int)((1f - stealth) * 30f);
				if (meleeCrit > 100)
				{
					meleeCrit = 100;
				}
				aggro -= (int)((1f - stealth) * 750f);
				if (stealthTimer > 0)
				{
					stealthTimer--;
				}
			}
			else if (shroomiteStealth)
			{
				if (itemAnimation > 0)
				{
					stealthTimer = 5;
				}
				if ((double)base.velocity.X > -0.1 && (double)base.velocity.X < 0.1 && (double)base.velocity.Y > -0.1 && (double)base.velocity.Y < 0.1 && !mount.Active)
				{
					if (stealthTimer == 0 && stealth > 0f)
					{
						stealth -= 0.015f;
						if ((double)stealth <= 0.0)
						{
							stealth = 0f;
							if (Main.netMode == 1)
							{
								NetMessage.SendData(84, -1, -1, "", whoAmI);
							}
						}
					}
				}
				else
				{
					float num45 = Math.Abs(base.velocity.X) + Math.Abs(base.velocity.Y);
					stealth += num45 * 0.0075f;
					if (stealth > 1f)
					{
						stealth = 1f;
					}
					if (mount.Active)
					{
						stealth = 1f;
					}
				}
				rangedDamage += (1f - stealth) * 0.6f;
				rangedCrit += (int)((1f - stealth) * 10f);
				aggro -= (int)((1f - stealth) * 750f);
				if (stealthTimer > 0)
				{
					stealthTimer--;
				}
			}
			else if (setVortex)
			{
				bool flag19 = false;
				if (vortexStealthActive)
				{
					float num46 = stealth;
					stealth -= 0.04f;
					if (stealth < 0f)
					{
						stealth = 0f;
					}
					else
					{
						flag19 = true;
					}
					if (stealth == 0f && num46 != stealth && Main.netMode == 1)
					{
						NetMessage.SendData(84, -1, -1, "", whoAmI);
					}
					rangedDamage += (1f - stealth) * 0.8f;
					rangedCrit += (int)((1f - stealth) * 20f);
					aggro -= (int)((1f - stealth) * 1200f);
					moveSpeed *= 0.3f;
					if (mount.Active)
					{
						vortexStealthActive = false;
					}
				}
				else
				{
					float num47 = stealth;
					stealth += 0.04f;
					if (stealth > 1f)
					{
						stealth = 1f;
					}
					else
					{
						flag19 = true;
					}
					if (stealth == 1f && num47 != stealth && Main.netMode == 1)
					{
						NetMessage.SendData(84, -1, -1, "", whoAmI);
					}
				}
				if (flag19)
				{
					if (Main.rand.Next(2) == 0)
					{
						Vector2 vector = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust = Main.dust[Dust.NewDust(base.Center - vector * 30f, 0, 0, 229)];
						dust.noGravity = true;
						dust.position = base.Center - vector * Main.rand.Next(5, 11);
						dust.velocity = vector.RotatedBy(1.5707963705062866) * 4f;
						dust.scale = 0.5f + Main.rand.NextFloat();
						dust.fadeIn = 0.5f;
					}
					if (Main.rand.Next(2) == 0)
					{
						Vector2 vector2 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust2 = Main.dust[Dust.NewDust(base.Center - vector2 * 30f, 0, 0, 240)];
						dust2.noGravity = true;
						dust2.position = base.Center - vector2 * 12f;
						dust2.velocity = vector2.RotatedBy(-1.5707963705062866) * 2f;
						dust2.scale = 0.5f + Main.rand.NextFloat();
						dust2.fadeIn = 0.5f;
					}
				}
			}
			else
			{
				stealth = 1f;
			}
			if (manaSick)
			{
				magicDamage *= 1f - manaSickReduction;
			}
			if (inventory[selectedItem].type == 1947)
			{
				meleeSpeed = (1f + meleeSpeed) / 2f;
			}
			if ((double)pickSpeed < 0.3)
			{
				pickSpeed = 0.3f;
			}
			if (meleeSpeed > 3f)
			{
				meleeSpeed = 3f;
			}
			if ((double)moveSpeed > 1.6)
			{
				moveSpeed = 1.6f;
			}
			if (tileSpeed > 3f)
			{
				tileSpeed = 3f;
			}
			tileSpeed = 1f / tileSpeed;
			if (wallSpeed > 3f)
			{
				wallSpeed = 3f;
			}
			wallSpeed = 1f / wallSpeed;
			if (statManaMax2 > 400)
			{
				statManaMax2 = 400;
			}
			if (statDefense < 0)
			{
				statDefense = 0;
			}
			if (dazed)
			{
				moveSpeed /= 3f;
			}
			else if (slow)
			{
				moveSpeed /= 2f;
			}
			else if (chilled)
			{
				moveSpeed *= 0.75f;
			}
			meleeSpeed = 1f / meleeSpeed;
			UpdateLifeRegen();
			soulDrain = 0;
			UpdateManaRegen();
			if (manaRegenCount < 0)
			{
				manaRegenCount = 0;
			}
			if (statMana > statManaMax2)
			{
				statMana = statManaMax2;
			}
			runAcceleration *= moveSpeed;
			maxRunSpeed *= moveSpeed;
			UpdateJumpHeight();
			for (int num48 = 0; num48 < 22; num48++)
			{
				if (buffType[num48] > 0 && buffTime[num48] > 0 && buffImmune[buffType[num48]])
				{
					DelBuff(num48);
				}
			}
			if (brokenArmor)
			{
				statDefense /= 2;
			}
			lastTileRangeX = tileRangeX;
			lastTileRangeY = tileRangeY;
			if (mount.Active && mount.BlockExtraJumps)
			{
				jumpAgainCloud = false;
				jumpAgainSandstorm = false;
				jumpAgainBlizzard = false;
				jumpAgainFart = false;
				jumpAgainSail = false;
				jumpAgainUnicorn = false;
			}
			else
			{
				if (!doubleJumpCloud)
				{
					jumpAgainCloud = false;
				}
				else if (base.velocity.Y == 0f || sliding)
				{
					jumpAgainCloud = true;
				}
				if (!doubleJumpSandstorm)
				{
					jumpAgainSandstorm = false;
				}
				else if (base.velocity.Y == 0f || sliding)
				{
					jumpAgainSandstorm = true;
				}
				if (!doubleJumpBlizzard)
				{
					jumpAgainBlizzard = false;
				}
				else if (base.velocity.Y == 0f || sliding)
				{
					jumpAgainBlizzard = true;
				}
				if (!doubleJumpFart)
				{
					jumpAgainFart = false;
				}
				else if (base.velocity.Y == 0f || sliding)
				{
					jumpAgainFart = true;
				}
				if (!doubleJumpSail)
				{
					jumpAgainSail = false;
				}
				else if (base.velocity.Y == 0f || sliding)
				{
					jumpAgainSail = true;
				}
				if (!doubleJumpUnicorn)
				{
					jumpAgainUnicorn = false;
				}
				else if (base.velocity.Y == 0f || sliding)
				{
					jumpAgainUnicorn = true;
				}
			}
			if (!carpet)
			{
				canCarpet = false;
				carpetFrame = -1;
			}
			else if (base.velocity.Y == 0f || sliding)
			{
				canCarpet = true;
				carpetTime = 0;
				carpetFrame = -1;
				carpetFrameCounter = 0f;
			}
			if (gravDir == -1f)
			{
				canCarpet = false;
			}
			if (ropeCount > 0)
			{
				ropeCount--;
			}
			if (!pulley && !frozen && !webbed && !stoned && !controlJump && gravDir == 1f && ropeCount == 0 && grappling[0] == -1 && !tongued && !mount.Active)
			{
				FindPulley();
			}
			if (pulley)
			{
				if (mount.Active)
				{
					pulley = false;
				}
				sandStorm = false;
				dJumpEffectCloud = false;
				dJumpEffectSandstorm = false;
				dJumpEffectBlizzard = false;
				dJumpEffectFart = false;
				dJumpEffectSail = false;
				dJumpEffectUnicorn = false;
				int num49 = (int)(base.position.X + (float)(width / 2)) / 16;
				int num50 = (int)(base.position.Y - 8f) / 16;
				bool flag20 = false;
				if (pulleyDir == 0)
				{
					pulleyDir = 1;
				}
				if (pulleyDir == 1)
				{
					if (direction == -1 && controlLeft && (releaseLeft || leftTimer == 0))
					{
						pulleyDir = 2;
						flag20 = true;
					}
					else if ((direction == 1 && controlRight && releaseRight) || rightTimer == 0)
					{
						pulleyDir = 2;
						flag20 = true;
					}
					else
					{
						if (direction == 1 && controlLeft)
						{
							direction = -1;
							flag20 = true;
						}
						if (direction == -1 && controlRight)
						{
							direction = 1;
							flag20 = true;
						}
					}
				}
				else if (pulleyDir == 2)
				{
					if (direction == 1 && controlLeft)
					{
						flag20 = true;
						int num51 = num49 * 16 + 8 - width / 2;
						if (!Collision.SolidCollision(new Vector2(num51, base.position.Y), width, height))
						{
							pulleyDir = 1;
							direction = -1;
							flag20 = true;
						}
					}
					if (direction == -1 && controlRight)
					{
						flag20 = true;
						int num52 = num49 * 16 + 8 - width / 2;
						if (!Collision.SolidCollision(new Vector2(num52, base.position.Y), width, height))
						{
							pulleyDir = 1;
							direction = 1;
							flag20 = true;
						}
					}
				}
				bool flag21 = false;
				if (!flag20 && ((controlLeft && (releaseLeft || leftTimer == 0)) || (controlRight && (releaseRight || rightTimer == 0))))
				{
					int num53 = 1;
					if (controlLeft)
					{
						num53 = -1;
					}
					int num54 = num49 + num53;
					if (Main.tile[num54, num50].active() && Main.tileRope[Main.tile[num54, num50].type])
					{
						pulleyDir = 1;
						direction = num53;
						int num55 = num54 * 16 + 8 - width / 2;
						float y3 = base.position.Y;
						y3 = num50 * 16 + 22;
						if ((!Main.tile[num54, num50 - 1].active() || !Main.tileRope[Main.tile[num54, num50 - 1].type]) && (!Main.tile[num54, num50 + 1].active() || !Main.tileRope[Main.tile[num54, num50 + 1].type]))
						{
							y3 = num50 * 16 + 22;
						}
						if (Collision.SolidCollision(new Vector2(num55, y3), width, height))
						{
							pulleyDir = 2;
							direction = -num53;
							num55 = ((direction != 1) ? (num54 * 16 + 8 - width / 2 + -6) : (num54 * 16 + 8 - width / 2 + 6));
						}
						if (i == Main.myPlayer)
						{
							Main.cameraX = Main.cameraX + base.position.X - (float)num55;
						}
						base.position.X = num55;
						gfxOffY = base.position.Y - y3;
						base.position.Y = y3;
						flag21 = true;
					}
				}
				if (!flag21 && !flag20 && !controlUp && ((controlLeft && releaseLeft) || (controlRight && releaseRight)))
				{
					pulley = false;
					if (controlLeft && base.velocity.X == 0f)
					{
						base.velocity.X = -1f;
					}
					if (controlRight && base.velocity.X == 0f)
					{
						base.velocity.X = 1f;
					}
				}
				if (base.velocity.X != 0f)
				{
					pulley = false;
				}
				if (Main.tile[num49, num50] == null)
				{
					Main.tile[num49, num50] = new Tile();
				}
				if (!Main.tile[num49, num50].active() || !Main.tileRope[Main.tile[num49, num50].type])
				{
					pulley = false;
				}
				if (gravDir != 1f)
				{
					pulley = false;
				}
				if (frozen || webbed || stoned)
				{
					pulley = false;
				}
				if (!pulley)
				{
					base.velocity.Y -= gravity;
				}
				if (controlJump)
				{
					pulley = false;
					jump = jumpHeight;
					base.velocity.Y = 0f - jumpSpeed;
				}
			}
			if (pulley)
			{
				fallStart = (int)base.position.Y / 16;
				wingFrame = 0;
				if (wings == 4)
				{
					wingFrame = 3;
				}
				int num56 = (int)(base.position.X + (float)(width / 2)) / 16;
				int num57 = (int)(base.position.Y - 16f) / 16;
				int num58 = (int)(base.position.Y - 8f) / 16;
				bool flag22 = true;
				bool flag23 = false;
				if ((Main.tile[num56, num58 - 1].active() && Main.tileRope[Main.tile[num56, num58 - 1].type]) || (Main.tile[num56, num58 + 1].active() && Main.tileRope[Main.tile[num56, num58 + 1].type]))
				{
					flag23 = true;
				}
				if (Main.tile[num56, num57] == null)
				{
					Main.tile[num56, num57] = new Tile();
				}
				if (!Main.tile[num56, num57].active() || !Main.tileRope[Main.tile[num56, num57].type])
				{
					flag22 = false;
					if (base.velocity.Y < 0f)
					{
						base.velocity.Y = 0f;
					}
				}
				if (flag23)
				{
					if (controlUp && flag22)
					{
						float x3 = base.position.X;
						float y4 = base.position.Y - Math.Abs(base.velocity.Y) - 2f;
						if (Collision.SolidCollision(new Vector2(x3, y4), width, height))
						{
							x3 = num56 * 16 + 8 - width / 2 + 6;
							if (!Collision.SolidCollision(new Vector2(x3, y4), width, (int)((float)height + Math.Abs(base.velocity.Y) + 2f)))
							{
								if (i == Main.myPlayer)
								{
									Main.cameraX = Main.cameraX + base.position.X - x3;
								}
								pulleyDir = 2;
								direction = 1;
								base.position.X = x3;
								base.velocity.X = 0f;
							}
							else
							{
								x3 = num56 * 16 + 8 - width / 2 + -6;
								if (!Collision.SolidCollision(new Vector2(x3, y4), width, (int)((float)height + Math.Abs(base.velocity.Y) + 2f)))
								{
									if (i == Main.myPlayer)
									{
										Main.cameraX = Main.cameraX + base.position.X - x3;
									}
									pulleyDir = 2;
									direction = -1;
									base.position.X = x3;
									base.velocity.X = 0f;
								}
							}
						}
						if (base.velocity.Y > 0f)
						{
							base.velocity.Y *= 0.7f;
						}
						if (base.velocity.Y > -3f)
						{
							base.velocity.Y -= 0.2f;
						}
						else
						{
							base.velocity.Y -= 0.02f;
						}
						if (base.velocity.Y < -8f)
						{
							base.velocity.Y = -8f;
						}
					}
					else if (controlDown)
					{
						float x4 = base.position.X;
						float y5 = base.position.Y;
						if (Collision.SolidCollision(new Vector2(x4, y5), width, (int)((float)height + Math.Abs(base.velocity.Y) + 2f)))
						{
							x4 = num56 * 16 + 8 - width / 2 + 6;
							if (!Collision.SolidCollision(new Vector2(x4, y5), width, (int)((float)height + Math.Abs(base.velocity.Y) + 2f)))
							{
								if (i == Main.myPlayer)
								{
									Main.cameraX = Main.cameraX + base.position.X - x4;
								}
								pulleyDir = 2;
								direction = 1;
								base.position.X = x4;
								base.velocity.X = 0f;
							}
							else
							{
								x4 = num56 * 16 + 8 - width / 2 + -6;
								if (!Collision.SolidCollision(new Vector2(x4, y5), width, (int)((float)height + Math.Abs(base.velocity.Y) + 2f)))
								{
									if (i == Main.myPlayer)
									{
										Main.cameraX = Main.cameraX + base.position.X - x4;
									}
									pulleyDir = 2;
									direction = -1;
									base.position.X = x4;
									base.velocity.X = 0f;
								}
							}
						}
						if (base.velocity.Y < 0f)
						{
							base.velocity.Y *= 0.7f;
						}
						if (base.velocity.Y < 3f)
						{
							base.velocity.Y += 0.2f;
						}
						else
						{
							base.velocity.Y += 0.1f;
						}
						if (base.velocity.Y > maxFallSpeed)
						{
							base.velocity.Y = maxFallSpeed;
						}
					}
					else
					{
						base.velocity.Y *= 0.7f;
						if ((double)base.velocity.Y > -0.1 && (double)base.velocity.Y < 0.1)
						{
							base.velocity.Y = 0f;
						}
					}
				}
				else if (controlDown)
				{
					ropeCount = 10;
					pulley = false;
					base.velocity.Y = 1f;
				}
				else
				{
					base.velocity.Y = 0f;
					base.position.Y = num57 * 16 + 22;
				}
				float num59 = num56 * 16 + 8 - width / 2;
				if (pulleyDir == 1)
				{
					num59 = num56 * 16 + 8 - width / 2;
				}
				if (pulleyDir == 2)
				{
					num59 = num56 * 16 + 8 - width / 2 + 6 * direction;
				}
				if (i == Main.myPlayer)
				{
					Main.cameraX = Main.cameraX + base.position.X - num59;
				}
				base.position.X = num59;
				pulleyFrameCounter += Math.Abs(base.velocity.Y * 0.75f);
				if (base.velocity.Y != 0f)
				{
					pulleyFrameCounter += 0.75f;
				}
				if (pulleyFrameCounter > 10f)
				{
					pulleyFrame++;
					pulleyFrameCounter = 0f;
				}
				if (pulleyFrame > 1)
				{
					pulleyFrame = 0;
				}
				canCarpet = true;
				carpetFrame = -1;
				wingTime = wingTimeMax;
				rocketTime = rocketTimeMax;
				rocketDelay = 0;
				rocketFrame = false;
				canRocket = false;
				rocketRelease = false;
				DashMovement();
			}
			else if (grappling[0] == -1 && !tongued)
			{
				if (wingsLogic > 0 && base.velocity.Y != 0f && !merman)
				{
					if (wingsLogic == 1 || wingsLogic == 2)
					{
						accRunSpeed = 6.25f;
					}
					if (wingsLogic == 4)
					{
						accRunSpeed = 6.5f;
					}
					if (wingsLogic == 5 || wingsLogic == 6 || wingsLogic == 13 || wingsLogic == 15)
					{
						accRunSpeed = 6.75f;
					}
					if (wingsLogic == 7 || wingsLogic == 8)
					{
						accRunSpeed = 7f;
					}
					if (wingsLogic == 9 || wingsLogic == 10 || wingsLogic == 11 || wingsLogic == 20 || wingsLogic == 21 || wingsLogic == 23 || wingsLogic == 24)
					{
						accRunSpeed = 7.5f;
					}
					if (wingsLogic == 22)
					{
						if (controlDown && controlJump && wingTime > 0f)
						{
							accRunSpeed = 10f;
							runAcceleration *= 10f;
						}
						else
						{
							accRunSpeed = 6.25f;
						}
					}
					if (wingsLogic == 30 || wingsLogic == 31)
					{
						if (controlDown && controlJump && wingTime > 0f)
						{
							accRunSpeed = 12f;
							runAcceleration *= 12f;
						}
						else
						{
							accRunSpeed = 6.5f;
							runAcceleration *= 1.5f;
						}
					}
					if (wingsLogic == 26)
					{
						accRunSpeed = 8f;
						runAcceleration *= 2f;
					}
					if (wingsLogic == 29 || wingsLogic == 32)
					{
						accRunSpeed = 9f;
						runAcceleration *= 2.5f;
					}
					if (wingsLogic == 12)
					{
						accRunSpeed = 7.75f;
					}
					if (wingsLogic == 16 || wingsLogic == 17 || wingsLogic == 18 || wingsLogic == 19 || wingsLogic == 34 || wingsLogic == 3 || wingsLogic == 28 || wingsLogic == 33 || wingsLogic == 34 || wingsLogic == 35 || wingsLogic == 36)
					{
						accRunSpeed = 7f;
					}
				}
				if (sticky)
				{
					maxRunSpeed *= 0.25f;
					runAcceleration *= 0.25f;
					runSlowdown *= 2f;
					if (base.velocity.X > maxRunSpeed)
					{
						base.velocity.X = maxRunSpeed;
					}
					if (base.velocity.X < 0f - maxRunSpeed)
					{
						base.velocity.X = 0f - maxRunSpeed;
					}
				}
				else if (powerrun)
				{
					maxRunSpeed *= 3.5f;
					runAcceleration *= 1f;
					runSlowdown *= 2f;
				}
				else if (slippy2)
				{
					runAcceleration *= 0.6f;
					runSlowdown = 0f;
					if (iceSkate)
					{
						runAcceleration *= 3.5f;
						maxRunSpeed *= 1.25f;
					}
				}
				else if (slippy)
				{
					runAcceleration *= 0.7f;
					if (iceSkate)
					{
						runAcceleration *= 3.5f;
						maxRunSpeed *= 1.25f;
					}
					else
					{
						runSlowdown *= 0.1f;
					}
				}
				if (sandStorm)
				{
					runAcceleration *= 1.5f;
					maxRunSpeed *= 2f;
				}
				if (dJumpEffectBlizzard && doubleJumpBlizzard)
				{
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}
				if (dJumpEffectFart && doubleJumpFart)
				{
					runAcceleration *= 3f;
					maxRunSpeed *= 1.75f;
				}
				if (dJumpEffectUnicorn && doubleJumpUnicorn)
				{
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}
				if (dJumpEffectSail && doubleJumpSail)
				{
					runAcceleration *= 1.5f;
					maxRunSpeed *= 1.25f;
				}
				if (carpetFrame != -1)
				{
					runAcceleration *= 1.25f;
					maxRunSpeed *= 1.5f;
				}
				if (inventory[selectedItem].type == 3106 && stealth < 1f)
				{
					float num60 = maxRunSpeed / 2f * (1f - stealth);
					maxRunSpeed -= num60;
					accRunSpeed = maxRunSpeed;
				}
				if (mount.Active)
				{
					rocketBoots = 0;
					wings = 0;
					wingsLogic = 0;
					maxRunSpeed = mount.RunSpeed;
					accRunSpeed = mount.DashSpeed;
					runAcceleration = mount.Acceleration;
					if (mount.Type == 12 && !MountFishronSpecial)
					{
						runAcceleration /= 2f;
						maxRunSpeed /= 2f;
					}
					mount.AbilityRecovery();
					if (mount.Cart && base.velocity.Y == 0f)
					{
						if (!Minecart.OnTrack(base.position, width, height))
						{
							fullRotation = 0f;
							onWrongGround = true;
							runSlowdown = 0.2f;
							if ((controlLeft && releaseLeft) || (controlRight && releaseRight))
							{
								mount.Dismount(this);
							}
						}
						else
						{
							runSlowdown = runAcceleration;
							onWrongGround = false;
						}
					}
					if (mount.Type == 8)
					{
						mount.UpdateDrill(this, controlUp, controlDown);
					}
				}
				HorizontalMovement();
				if (gravControl)
				{
					if (controlUp && releaseUp)
					{
						if (gravDir == 1f)
						{
							gravDir = -1f;
							fallStart = (int)(base.position.Y / 16f);
							jump = 0;
							Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 8);
						}
						else
						{
							gravDir = 1f;
							fallStart = (int)(base.position.Y / 16f);
							jump = 0;
							Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 8);
						}
					}
				}
				else if (gravControl2)
				{
					if (controlUp && releaseUp && base.velocity.Y == 0f)
					{
						if (gravDir == 1f)
						{
							gravDir = -1f;
							fallStart = (int)(base.position.Y / 16f);
							jump = 0;
							Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 8);
						}
						else
						{
							gravDir = 1f;
							fallStart = (int)(base.position.Y / 16f);
							jump = 0;
							Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 8);
						}
					}
				}
				else
				{
					gravDir = 1f;
				}
				if (base.velocity.Y == 0f && mount.Active && mount.CanHover && controlUp && releaseUp)
				{
					base.velocity.Y = 0f - (mount.Acceleration + gravity + 0.001f);
				}
				if (controlUp)
				{
					releaseUp = false;
				}
				else
				{
					releaseUp = true;
				}
				sandStorm = false;
				JumpMovement();
				if (wingsLogic == 0)
				{
					wingTime = 0f;
				}
				if (rocketBoots == 0)
				{
					rocketTime = 0;
				}
				if (jump == 0)
				{
					dJumpEffectCloud = false;
					dJumpEffectSandstorm = false;
					dJumpEffectBlizzard = false;
					dJumpEffectFart = false;
					dJumpEffectSail = false;
					dJumpEffectUnicorn = false;
				}
				DashMovement();
				WallslideMovement();
				CarpetMovement();
				DoubleJumpVisuals();
				if (wings > 0 || mount.Active)
				{
					sandStorm = false;
				}
				if (((gravDir == 1f && base.velocity.Y > 0f - jumpSpeed) || (gravDir == -1f && base.velocity.Y < jumpSpeed)) && base.velocity.Y != 0f)
				{
					canRocket = true;
				}
				bool flag24 = false;
				if (((base.velocity.Y == 0f || sliding) && releaseJump) || (autoJump && justJumped))
				{
					mount.ResetFlightTime(base.velocity.X);
					wingTime = wingTimeMax;
				}
				if (wingsLogic > 0 && controlJump && wingTime > 0f && !jumpAgainCloud && jump == 0 && base.velocity.Y != 0f)
				{
					flag24 = true;
				}
				if ((wingsLogic == 22 || wingsLogic == 28 || wingsLogic == 30 || wingsLogic == 32 || wingsLogic == 29 || wingsLogic == 33 || wingsLogic == 35) && controlJump && controlDown && wingTime > 0f)
				{
					flag24 = true;
				}
				if (frozen || webbed || stoned)
				{
					if (mount.Active)
					{
						mount.Dismount(this);
					}
					base.velocity.Y += gravity;
					if (base.velocity.Y > maxFallSpeed)
					{
						base.velocity.Y = maxFallSpeed;
					}
					sandStorm = false;
					dJumpEffectCloud = false;
					dJumpEffectSandstorm = false;
					dJumpEffectBlizzard = false;
					dJumpEffectFart = false;
					dJumpEffectSail = false;
					dJumpEffectUnicorn = false;
				}
				else
				{
					if (flag24)
					{
						if (wings == 10 && Main.rand.Next(2) == 0)
						{
							int num61 = 4;
							if (direction == 1)
							{
								num61 = -40;
							}
							int num62 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num61, base.position.Y + (float)(height / 2) - 15f), 30, 30, 76, 0f, 0f, 50, default(Color), 0.6f);
							Main.dust[num62].fadeIn = 1.1f;
							Main.dust[num62].noGravity = true;
							Main.dust[num62].noLight = true;
							Main.dust[num62].velocity *= 0.3f;
							Main.dust[num62].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						}
						if (wings == 34 && Main.rand.Next(2) == 0)
						{
							int num63 = 4;
							if (direction == 1)
							{
								num63 = -40;
							}
							int num64 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num63, base.position.Y + (float)(height / 2) - 15f), 30, 30, 261, 0f, 0f, 50, default(Color), 0.6f);
							Main.dust[num64].fadeIn = 1.1f;
							Main.dust[num64].noGravity = true;
							Main.dust[num64].noLight = true;
							Main.dust[num64].velocity *= 0.3f;
							Main.dust[num64].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						}
						if (wings == 9 && Main.rand.Next(2) == 0)
						{
							int num65 = 4;
							if (direction == 1)
							{
								num65 = -40;
							}
							int num66 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num65, base.position.Y + (float)(height / 2) - 15f), 30, 30, 6, 0f, 0f, 200, default(Color), 2f);
							Main.dust[num66].noGravity = true;
							Main.dust[num66].velocity *= 0.3f;
							Main.dust[num66].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						}
						if (wings == 6 && Main.rand.Next(4) == 0)
						{
							int num67 = 4;
							if (direction == 1)
							{
								num67 = -40;
							}
							int num68 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num67, base.position.Y + (float)(height / 2) - 15f), 30, 30, 55, 0f, 0f, 200);
							Main.dust[num68].velocity *= 0.3f;
							Main.dust[num68].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						}
						if (wings == 5 && Main.rand.Next(3) == 0)
						{
							int num69 = 6;
							if (direction == 1)
							{
								num69 = -30;
							}
							int num70 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num69, base.position.Y), 18, height, 58, 0f, 0f, 255, default(Color), 1.2f);
							Main.dust[num70].velocity *= 0.3f;
							Main.dust[num70].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						}
						if (wings == 26)
						{
							int num71 = 6;
							if (direction == 1)
							{
								num71 = -30;
							}
							int num72 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num71, base.position.Y), 18, height, 217, 0f, 0f, 100, default(Color), 1.4f);
							Main.dust[num72].noGravity = true;
							Main.dust[num72].noLight = true;
							Main.dust[num72].velocity /= 4f;
							Main.dust[num72].velocity -= base.velocity;
							Main.dust[num72].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
							if (Main.rand.Next(2) == 0)
							{
								num71 = -24;
								if (direction == 1)
								{
									num71 = 12;
								}
								float num73 = base.position.Y;
								if (gravDir == -1f)
								{
									num73 += (float)(height / 2);
								}
								num72 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num71, num73), 12, height / 2, 217, 0f, 0f, 100, default(Color), 1.4f);
								Main.dust[num72].noGravity = true;
								Main.dust[num72].noLight = true;
								Main.dust[num72].velocity /= 4f;
								Main.dust[num72].velocity -= base.velocity;
								Main.dust[num72].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
							}
						}
						if (wings == 29 && Main.rand.Next(3) == 0)
						{
							int num74 = 4;
							if (direction == 1)
							{
								num74 = -40;
							}
							int num75 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num74, base.position.Y + (float)(height / 2) - 15f), 30, 30, 6, 0f, 0f, 100, default(Color), 2.4f);
							Main.dust[num75].noGravity = true;
							Main.dust[num75].velocity *= 0.3f;
							if (Main.rand.Next(10) == 0)
							{
								Main.dust[num75].fadeIn = 2f;
							}
							Main.dust[num75].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
						}
						if (wings == 31)
						{
							if (Main.rand.Next(6) == 0)
							{
								int num76 = 4;
								if (direction == 1)
								{
									num76 = -40;
								}
								Dust dust3 = Main.dust[Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num76, base.position.Y + (float)(height / 2) - 15f), 30, 30, 86)];
								dust3.noGravity = true;
								dust3.scale = 1f;
								dust3.fadeIn = 1.2f;
								dust3.velocity *= 0.2f;
								dust3.noLight = true;
								dust3.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
							}
							if (Main.rand.Next(3) == 0)
							{
								int num77 = 4;
								if (direction == 1)
								{
									num77 = -40;
								}
								Dust dust4 = Main.dust[Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num77, base.position.Y + (float)(height / 2) - 15f), 30, 30, 240)];
								dust4.noGravity = true;
								dust4.scale = 1.2f;
								dust4.velocity *= 0.2f;
								dust4.alpha = 200;
								dust4.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
							}
							if (Main.rand.Next(2) == 0)
							{
								if (Main.rand.Next(6) == 0)
								{
									int num78 = -24;
									if (direction == 1)
									{
										num78 = 12;
									}
									float num79 = base.position.Y;
									if (gravDir == -1f)
									{
										num79 += (float)(height / 2);
									}
									Dust dust5 = Main.dust[Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num78, num79), 12, height / 2, 86)];
									dust5.noGravity = true;
									dust5.scale = 1f;
									dust5.fadeIn = 1.2f;
									dust5.velocity *= 0.2f;
									dust5.noLight = true;
									dust5.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
								if (Main.rand.Next(3) == 0)
								{
									int num78 = -24;
									if (direction == 1)
									{
										num78 = 12;
									}
									float num80 = base.position.Y;
									if (gravDir == -1f)
									{
										num80 += (float)(height / 2);
									}
									Dust dust6 = Main.dust[Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num78, num80), 12, height / 2, 240)];
									dust6.noGravity = true;
									dust6.scale = 1.2f;
									dust6.velocity *= 0.2f;
									dust6.alpha = 200;
									dust6.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
							}
						}
						WingMovement();
					}
					if (wings == 4)
					{
						if (flag24 || jump > 0)
						{
							rocketDelay2--;
							if (rocketDelay2 <= 0)
							{
								Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 13);
								rocketDelay2 = 60;
							}
							int num81 = 2;
							if (controlUp)
							{
								num81 = 4;
							}
							for (int num82 = 0; num82 < num81; num82++)
							{
								int type = 6;
								if (head == 41)
								{
									int body2 = body;
									int num269 = 24;
								}
								float scale = 1.75f;
								int alpha = 100;
								float x5 = base.position.X + (float)(width / 2) + 16f;
								if (direction > 0)
								{
									x5 = base.position.X + (float)(width / 2) - 26f;
								}
								float num83 = base.position.Y + (float)height - 18f;
								if (num82 == 1 || num82 == 3)
								{
									x5 = base.position.X + (float)(width / 2) + 8f;
									if (direction > 0)
									{
										x5 = base.position.X + (float)(width / 2) - 20f;
									}
									num83 += 6f;
								}
								if (num82 > 1)
								{
									num83 += base.velocity.Y;
								}
								int num84 = Dust.NewDust(new Vector2(x5, num83), 8, 8, type, 0f, 0f, alpha, default(Color), scale);
								Main.dust[num84].velocity.X *= 0.1f;
								Main.dust[num84].velocity.Y = Main.dust[num84].velocity.Y * 1f + 2f * gravDir - base.velocity.Y * 0.3f;
								Main.dust[num84].noGravity = true;
								Main.dust[num84].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								if (num81 == 4)
								{
									Main.dust[num84].velocity.Y += 6f;
								}
							}
							wingFrameCounter++;
							if (wingFrameCounter > 4)
							{
								wingFrame++;
								wingFrameCounter = 0;
								if (wingFrame >= 3)
								{
									wingFrame = 0;
								}
							}
						}
						else if (!controlJump || base.velocity.Y == 0f)
						{
							wingFrame = 3;
						}
					}
					else if (wings == 28)
					{
						if (base.velocity.Y != 0f)
						{
							Lighting.AddLight(base.Bottom, 0.3f, 0.1f, 0.4f);
						}
					}
					else if (wings == 22)
					{
						if (!controlJump)
						{
							wingFrame = 0;
							wingFrameCounter = 0;
						}
						else if (wingTime > 0f)
						{
							if (controlDown)
							{
								if (base.velocity.X != 0f)
								{
									wingFrameCounter++;
									int num85 = 2;
									if (wingFrameCounter < num85)
									{
										wingFrame = 1;
									}
									else if (wingFrameCounter < num85 * 2)
									{
										wingFrame = 2;
									}
									else if (wingFrameCounter < num85 * 3)
									{
										wingFrame = 3;
									}
									else if (wingFrameCounter < num85 * 4 - 1)
									{
										wingFrame = 2;
									}
									else
									{
										wingFrame = 2;
										wingFrameCounter = 0;
									}
								}
								else
								{
									wingFrameCounter++;
									int num86 = 6;
									if (wingFrameCounter < num86)
									{
										wingFrame = 4;
									}
									else if (wingFrameCounter < num86 * 2)
									{
										wingFrame = 5;
									}
									else if (wingFrameCounter < num86 * 3 - 1)
									{
										wingFrame = 4;
									}
									else
									{
										wingFrame = 4;
										wingFrameCounter = 0;
									}
								}
							}
							else
							{
								wingFrameCounter++;
								int num87 = 2;
								if (wingFrameCounter < num87)
								{
									wingFrame = 4;
								}
								else if (wingFrameCounter < num87 * 2)
								{
									wingFrame = 5;
								}
								else if (wingFrameCounter < num87 * 3)
								{
									wingFrame = 6;
								}
								else if (wingFrameCounter < num87 * 4 - 1)
								{
									wingFrame = 5;
								}
								else
								{
									wingFrame = 5;
									wingFrameCounter = 0;
								}
							}
						}
						else
						{
							wingFrameCounter++;
							int num88 = 6;
							if (wingFrameCounter < num88)
							{
								wingFrame = 4;
							}
							else if (wingFrameCounter < num88 * 2)
							{
								wingFrame = 5;
							}
							else if (wingFrameCounter < num88 * 3 - 1)
							{
								wingFrame = 4;
							}
							else
							{
								wingFrame = 4;
								wingFrameCounter = 0;
							}
						}
					}
					else if (wings == 12)
					{
						if (flag24 || jump > 0)
						{
							wingFrameCounter++;
							int num89 = 5;
							if (wingFrameCounter < num89)
							{
								wingFrame = 1;
							}
							else if (wingFrameCounter < num89 * 2)
							{
								wingFrame = 2;
							}
							else if (wingFrameCounter < num89 * 3)
							{
								wingFrame = 3;
							}
							else if (wingFrameCounter < num89 * 4 - 1)
							{
								wingFrame = 2;
							}
							else
							{
								wingFrame = 2;
								wingFrameCounter = 0;
							}
						}
						else if (base.velocity.Y != 0f)
						{
							wingFrame = 2;
						}
						else
						{
							wingFrame = 0;
						}
					}
					else if (wings == 24)
					{
						if (flag24 || jump > 0)
						{
							wingFrameCounter++;
							int num90 = 1;
							if (wingFrameCounter < num90)
							{
								wingFrame = 1;
							}
							else if (wingFrameCounter < num90 * 2)
							{
								wingFrame = 2;
							}
							else if (wingFrameCounter < num90 * 3)
							{
								wingFrame = 3;
							}
							else
							{
								wingFrame = 2;
								if (wingFrameCounter >= num90 * 4 - 1)
								{
									wingFrameCounter = 0;
								}
							}
						}
						else if (base.velocity.Y != 0f)
						{
							if (controlJump)
							{
								wingFrameCounter++;
								int num91 = 3;
								if (wingFrameCounter < num91)
								{
									wingFrame = 1;
								}
								else if (wingFrameCounter < num91 * 2)
								{
									wingFrame = 2;
								}
								else if (wingFrameCounter < num91 * 3)
								{
									wingFrame = 3;
								}
								else
								{
									wingFrame = 2;
									if (wingFrameCounter >= num91 * 4 - 1)
									{
										wingFrameCounter = 0;
									}
								}
							}
							else if (wingTime == 0f)
							{
								wingFrame = 0;
							}
							else
							{
								wingFrame = 1;
							}
						}
						else
						{
							wingFrame = 0;
						}
					}
					else if (wings == 30)
					{
						bool flag25 = false;
						if (flag24 || jump > 0)
						{
							wingFrameCounter++;
							int num92 = 2;
							if (wingFrameCounter >= num92 * 3)
							{
								wingFrameCounter = 0;
							}
							wingFrame = 1 + wingFrameCounter / num92;
							flag25 = true;
						}
						else if (base.velocity.Y != 0f)
						{
							if (controlJump)
							{
								wingFrameCounter++;
								int num93 = 2;
								if (wingFrameCounter >= num93 * 3)
								{
									wingFrameCounter = 0;
								}
								wingFrame = 1 + wingFrameCounter / num93;
								flag25 = true;
							}
							else if (wingTime == 0f)
							{
								wingFrame = 0;
							}
							else
							{
								wingFrame = 0;
							}
						}
						else
						{
							wingFrame = 0;
						}
						if (flag25)
						{
							for (int num94 = 0; num94 < 4; num94++)
							{
								if (Main.rand.Next(4) == 0)
								{
									Vector2 value = (-0.745398164f + (float)Math.PI / 8f * (float)num94 + 0.03f * (float)num94).ToRotationVector2() * new Vector2(-direction * 20, 20f);
									Dust dust7 = Main.dust[Dust.NewDust(base.Center, 0, 0, 229, 0f, 0f, 100, Color.White, 0.8f)];
									dust7.noGravity = true;
									dust7.position = base.Center + value;
									dust7.velocity = DirectionTo(dust7.position) * 2f;
									if (Main.rand.Next(10) != 0)
									{
										dust7.customData = this;
									}
									else
									{
										dust7.fadeIn = 0.5f;
									}
									dust7.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
							}
							for (int num95 = 0; num95 < 4; num95++)
							{
								if (Main.rand.Next(8) == 0)
								{
									Vector2 value2 = (-0.7053982f + (float)Math.PI / 8f * (float)num95 + 0.03f * (float)num95).ToRotationVector2() * new Vector2(direction * 20, 24f) + new Vector2((float)(-direction) * 16f, 0f);
									Dust dust8 = Main.dust[Dust.NewDust(base.Center, 0, 0, 229, 0f, 0f, 100, Color.White, 0.5f)];
									dust8.noGravity = true;
									dust8.position = base.Center + value2;
									dust8.velocity = Vector2.Normalize(dust8.position - base.Center - new Vector2((float)(-direction) * 16f, 0f)) * 2f;
									dust8.position += dust8.velocity * 5f;
									if (Main.rand.Next(10) != 0)
									{
										dust8.customData = this;
									}
									else
									{
										dust8.fadeIn = 0.5f;
									}
									dust8.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
							}
						}
					}
					else if (wings == 34)
					{
						if (flag24 || jump > 0)
						{
							wingFrameCounter++;
							int num96 = 4;
							if (wingFrameCounter >= num96 * 6)
							{
								wingFrameCounter = 0;
							}
							wingFrame = wingFrameCounter / num96;
						}
						else if (base.velocity.Y != 0f)
						{
							if (controlJump)
							{
								wingFrameCounter++;
								int num97 = 9;
								if (wingFrameCounter >= num97 * 6)
								{
									wingFrameCounter = 0;
								}
								wingFrame = wingFrameCounter / num97;
							}
							else
							{
								wingFrameCounter++;
								int num98 = 6;
								if (wingFrameCounter >= num98 * 6)
								{
									wingFrameCounter = 0;
								}
								wingFrame = wingFrameCounter / num98;
							}
						}
						else
						{
							wingFrameCounter++;
							int num99 = 4;
							if (wingFrameCounter >= num99 * 6)
							{
								wingFrameCounter = 0;
							}
							wingFrame = wingFrameCounter / num99;
						}
					}
					else if (wings == 33)
					{
						bool flag26 = false;
						if (flag24 || jump > 0)
						{
							flag26 = true;
						}
						else if (base.velocity.Y != 0f && controlJump)
						{
							flag26 = true;
						}
						if (flag26)
						{
							Color newColor = Main.hslToRgb(Main.rgbToHsl(eyeColor).X, 1f, 0.5f);
							int num100 = (direction != 1) ? (-4) : 0;
							for (int num101 = 0; num101 < 2; num101++)
							{
								Dust dust9 = Main.dust[Dust.NewDust(base.position, width, height, 182, base.velocity.X, base.velocity.Y, 127, newColor)];
								dust9.noGravity = true;
								dust9.fadeIn = 1f;
								dust9.scale = 1f;
								dust9.noLight = true;
								switch (num101)
								{
								case 0:
									dust9.position = new Vector2(base.position.X + (float)num100, base.position.Y + (float)height);
									dust9.velocity.X = dust9.velocity.X * 1f - 2f - base.velocity.X * 0.3f;
									dust9.velocity.Y = dust9.velocity.Y * 1f + 2f * gravDir - base.velocity.Y * 0.3f;
									break;
								case 1:
									dust9.position = new Vector2(base.position.X + (float)width + (float)num100, base.position.Y + (float)height);
									dust9.velocity.X = dust9.velocity.X * 1f + 2f - base.velocity.X * 0.3f;
									dust9.velocity.Y = dust9.velocity.Y * 1f + 2f * gravDir - base.velocity.Y * 0.3f;
									break;
								}
								Dust dust10 = Dust.CloneDust(dust9);
								dust10.scale *= 0.65f;
								dust10.fadeIn *= 0.65f;
								dust10.color = new Color(255, 255, 255, 255);
								dust9.noLight = true;
								dust9.shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
							}
						}
					}
					else
					{
						int num102 = 4;
						if (wings == 32)
						{
							num102 = 3;
						}
						if (flag24 || jump > 0)
						{
							wingFrameCounter++;
							if (wingFrameCounter > num102)
							{
								wingFrame++;
								wingFrameCounter = 0;
								if (wingFrame >= 4)
								{
									wingFrame = 0;
								}
							}
						}
						else if (base.velocity.Y != 0f)
						{
							wingFrame = 1;
							if (wings == 32)
							{
								wingFrame = 3;
							}
							if (wings == 29 && Main.rand.Next(5) == 0)
							{
								int num103 = 4;
								if (direction == 1)
								{
									num103 = -40;
								}
								int num104 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num103, base.position.Y + (float)(height / 2) - 15f), 30, 30, 6, 0f, 0f, 100, default(Color), 2.4f);
								Main.dust[num104].noGravity = true;
								Main.dust[num104].velocity *= 0.3f;
								if (Main.rand.Next(10) == 0)
								{
									Main.dust[num104].fadeIn = 2f;
								}
								Main.dust[num104].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
							}
						}
						else
						{
							wingFrame = 0;
						}
					}
					if (wingsLogic > 0 && rocketBoots > 0 && base.velocity.Y != 0f)
					{
						wingTime += rocketTime * 6;
						rocketTime = 0;
					}
					if (flag24 && wings != 4 && wings != 22 && wings != 0 && wings != 24 && wings != 28 && wings != 30 && wings != 33)
					{
						if (wingFrame == 3)
						{
							if (!flapSound)
							{
								Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 32);
							}
							flapSound = true;
						}
						else
						{
							flapSound = false;
						}
					}
					if (base.velocity.Y == 0f || sliding || (autoJump && justJumped))
					{
						rocketTime = rocketTimeMax;
					}
					if ((wingTime == 0f || wingsLogic == 0) && rocketBoots > 0 && controlJump && rocketDelay == 0 && canRocket && rocketRelease && !jumpAgainCloud)
					{
						if (rocketTime > 0)
						{
							rocketTime--;
							rocketDelay = 10;
							if (rocketDelay2 <= 0)
							{
								if (rocketBoots == 1)
								{
									Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 13);
									rocketDelay2 = 30;
								}
								else if (rocketBoots == 2 || rocketBoots == 3)
								{
									Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 24);
									rocketDelay2 = 15;
								}
							}
						}
						else
						{
							canRocket = false;
						}
					}
					if (rocketDelay2 > 0)
					{
						rocketDelay2--;
					}
					if (rocketDelay == 0)
					{
						rocketFrame = false;
					}
					if (rocketDelay > 0)
					{
						int num105 = height;
						if (gravDir == -1f)
						{
							num105 = 4;
						}
						rocketFrame = true;
						for (int num106 = 0; num106 < 2; num106++)
						{
							int type2 = 6;
							float scale2 = 2.5f;
							int alpha2 = 100;
							if (rocketBoots == 2)
							{
								type2 = 16;
								scale2 = 1.5f;
								alpha2 = 20;
							}
							else if (rocketBoots == 3)
							{
								type2 = 76;
								scale2 = 1f;
								alpha2 = 20;
							}
							else if (socialShadow)
							{
								type2 = 27;
								scale2 = 1.5f;
							}
							if (num106 == 0)
							{
								int num107 = Dust.NewDust(new Vector2(base.position.X - 4f, base.position.Y + (float)num105 - 10f), 8, 8, type2, 0f, 0f, alpha2, default(Color), scale2);
								Main.dust[num107].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
								if (rocketBoots == 1)
								{
									Main.dust[num107].noGravity = true;
								}
								Main.dust[num107].velocity.X = Main.dust[num107].velocity.X * 1f - 2f - base.velocity.X * 0.3f;
								Main.dust[num107].velocity.Y = Main.dust[num107].velocity.Y * 1f + 2f * gravDir - base.velocity.Y * 0.3f;
								if (rocketBoots == 2)
								{
									Main.dust[num107].velocity *= 0.1f;
								}
								if (rocketBoots == 3)
								{
									Main.dust[num107].velocity *= 0.05f;
									Main.dust[num107].velocity.Y += 0.15f;
									Main.dust[num107].noLight = true;
									if (Main.rand.Next(2) == 0)
									{
										Main.dust[num107].noGravity = true;
										Main.dust[num107].scale = 1.75f;
									}
								}
								continue;
							}
							int num108 = Dust.NewDust(new Vector2(base.position.X + (float)width - 4f, base.position.Y + (float)num105 - 10f), 8, 8, type2, 0f, 0f, alpha2, default(Color), scale2);
							Main.dust[num108].shader = GameShaders.Armor.GetSecondaryShader(cShoe, this);
							if (rocketBoots == 1)
							{
								Main.dust[num108].noGravity = true;
							}
							Main.dust[num108].velocity.X = Main.dust[num108].velocity.X * 1f + 2f - base.velocity.X * 0.3f;
							Main.dust[num108].velocity.Y = Main.dust[num108].velocity.Y * 1f + 2f * gravDir - base.velocity.Y * 0.3f;
							if (rocketBoots == 2)
							{
								Main.dust[num108].velocity *= 0.1f;
							}
							if (rocketBoots == 3)
							{
								Main.dust[num108].velocity *= 0.05f;
								Main.dust[num108].velocity.Y += 0.15f;
								Main.dust[num108].noLight = true;
								if (Main.rand.Next(2) == 0)
								{
									Main.dust[num108].noGravity = true;
									Main.dust[num108].scale = 1.75f;
								}
							}
						}
						if (rocketDelay == 0)
						{
							releaseJump = true;
						}
						rocketDelay--;
						base.velocity.Y -= 0.1f * gravDir;
						if (gravDir == 1f)
						{
							if (base.velocity.Y > 0f)
							{
								base.velocity.Y -= 0.5f;
							}
							else if ((double)base.velocity.Y > (double)(0f - jumpSpeed) * 0.5)
							{
								base.velocity.Y -= 0.1f;
							}
							if (base.velocity.Y < (0f - jumpSpeed) * 1.5f)
							{
								base.velocity.Y = (0f - jumpSpeed) * 1.5f;
							}
						}
						else
						{
							if (base.velocity.Y < 0f)
							{
								base.velocity.Y += 0.5f;
							}
							else if ((double)base.velocity.Y < (double)jumpSpeed * 0.5)
							{
								base.velocity.Y += 0.1f;
							}
							if (base.velocity.Y > jumpSpeed * 1.5f)
							{
								base.velocity.Y = jumpSpeed * 1.5f;
							}
						}
					}
					else if (!flag24)
					{
						if (mount.CanHover)
						{
							mount.Hover(this);
						}
						else if (mount.CanFly && controlJump && jump == 0)
						{
							if (mount.Flight())
							{
								if (controlDown)
								{
									base.velocity.Y *= 0.9f;
									if (base.velocity.Y > -1f && (double)base.velocity.Y < 0.5)
									{
										base.velocity.Y = 1E-05f;
									}
								}
								else
								{
									if (base.velocity.Y > 0f)
									{
										base.velocity.Y -= 0.5f;
									}
									else if ((double)base.velocity.Y > (double)(0f - jumpSpeed) * 1.5)
									{
										base.velocity.Y -= 0.1f;
									}
									if (base.velocity.Y < (0f - jumpSpeed) * 1.5f)
									{
										base.velocity.Y = (0f - jumpSpeed) * 1.5f;
									}
								}
							}
							else
							{
								base.velocity.Y += gravity / 3f * gravDir;
								if (gravDir == 1f)
								{
									if (base.velocity.Y > maxFallSpeed / 3f && !controlDown)
									{
										base.velocity.Y = maxFallSpeed / 3f;
									}
								}
								else if (base.velocity.Y < (0f - maxFallSpeed) / 3f && !controlUp)
								{
									base.velocity.Y = (0f - maxFallSpeed) / 3f;
								}
							}
						}
						else if (slowFall && ((!controlDown && gravDir == 1f) || (!controlDown && gravDir == -1f)))
						{
							if ((controlUp && gravDir == 1f) || (controlUp && gravDir == -1f))
							{
								gravity = gravity / 10f * gravDir;
							}
							else
							{
								gravity = gravity / 3f * gravDir;
							}
							base.velocity.Y += gravity;
						}
						else if (wingsLogic > 0 && controlJump && base.velocity.Y > 0f)
						{
							fallStart = (int)(base.position.Y / 16f);
							if (base.velocity.Y > 0f)
							{
								if (wings == 10 && Main.rand.Next(3) == 0)
								{
									int num109 = 4;
									if (direction == 1)
									{
										num109 = -40;
									}
									int num110 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num109, base.position.Y + (float)(height / 2) - 15f), 30, 30, 76, 0f, 0f, 50, default(Color), 0.6f);
									Main.dust[num110].fadeIn = 1.1f;
									Main.dust[num110].noGravity = true;
									Main.dust[num110].noLight = true;
									Main.dust[num110].velocity *= 0.3f;
									Main.dust[num110].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
								if (wings == 34 && Main.rand.Next(3) == 0)
								{
									int num111 = 4;
									if (direction == 1)
									{
										num111 = -40;
									}
									int num112 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num111, base.position.Y + (float)(height / 2) - 15f), 30, 30, 261, 0f, 0f, 50, default(Color), 0.6f);
									Main.dust[num112].fadeIn = 1.1f;
									Main.dust[num112].noGravity = true;
									Main.dust[num112].noLight = true;
									Main.dust[num112].velocity *= 0.3f;
									Main.dust[num112].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
								if (wings == 9 && Main.rand.Next(3) == 0)
								{
									int num113 = 8;
									if (direction == 1)
									{
										num113 = -40;
									}
									int num114 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num113, base.position.Y + (float)(height / 2) - 15f), 30, 30, 6, 0f, 0f, 200, default(Color), 2f);
									Main.dust[num114].noGravity = true;
									Main.dust[num114].velocity *= 0.3f;
									Main.dust[num114].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
								if (wings == 29 && Main.rand.Next(3) == 0)
								{
									int num115 = 8;
									if (direction == 1)
									{
										num115 = -40;
									}
									int num116 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num115, base.position.Y + (float)(height / 2) - 15f), 30, 30, 6, 0f, 0f, 100, default(Color), 2.4f);
									Main.dust[num116].noGravity = true;
									Main.dust[num116].velocity *= 0.3f;
									if (Main.rand.Next(10) == 0)
									{
										Main.dust[num116].fadeIn = 2f;
									}
									Main.dust[num116].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
								if (wings == 6)
								{
									if (Main.rand.Next(10) == 0)
									{
										int num117 = 4;
										if (direction == 1)
										{
											num117 = -40;
										}
										int num118 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num117, base.position.Y + (float)(height / 2) - 12f), 30, 20, 55, 0f, 0f, 200);
										Main.dust[num118].velocity *= 0.3f;
										Main.dust[num118].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
									}
								}
								else if (wings == 5 && Main.rand.Next(6) == 0)
								{
									int num119 = 6;
									if (direction == 1)
									{
										num119 = -30;
									}
									int num120 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num119, base.position.Y), 18, height, 58, 0f, 0f, 255, default(Color), 1.2f);
									Main.dust[num120].velocity *= 0.3f;
									Main.dust[num120].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
								}
								if (wings == 4)
								{
									rocketDelay2--;
									if (rocketDelay2 <= 0)
									{
										Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 13);
										rocketDelay2 = 60;
									}
									int type3 = 6;
									float scale3 = 1.5f;
									int alpha3 = 100;
									float x6 = base.position.X + (float)(width / 2) + 16f;
									if (direction > 0)
									{
										x6 = base.position.X + (float)(width / 2) - 26f;
									}
									float num121 = base.position.Y + (float)height - 18f;
									if (Main.rand.Next(2) == 1)
									{
										x6 = base.position.X + (float)(width / 2) + 8f;
										if (direction > 0)
										{
											x6 = base.position.X + (float)(width / 2) - 20f;
										}
										num121 += 6f;
									}
									int num122 = Dust.NewDust(new Vector2(x6, num121), 8, 8, type3, 0f, 0f, alpha3, default(Color), scale3);
									Main.dust[num122].velocity.X *= 0.3f;
									Main.dust[num122].velocity.Y += 10f;
									Main.dust[num122].noGravity = true;
									Main.dust[num122].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
									wingFrameCounter++;
									if (wingFrameCounter > 4)
									{
										wingFrame++;
										wingFrameCounter = 0;
										if (wingFrame >= 3)
										{
											wingFrame = 0;
										}
									}
								}
								else if (wings != 22 && wings != 28)
								{
									if (wings == 30)
									{
										wingFrameCounter++;
										int num123 = 5;
										if (wingFrameCounter >= num123 * 3)
										{
											wingFrameCounter = 0;
										}
										wingFrame = 1 + wingFrameCounter / num123;
									}
									else if (wings == 34)
									{
										wingFrameCounter++;
										int num124 = 7;
										if (wingFrameCounter >= num124 * 6)
										{
											wingFrameCounter = 0;
										}
										wingFrame = wingFrameCounter / num124;
									}
									else if (wings == 26)
									{
										int num125 = 6;
										if (direction == 1)
										{
											num125 = -30;
										}
										int num126 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num125, base.position.Y), 18, height, 217, 0f, 0f, 100, default(Color), 1.4f);
										Main.dust[num126].noGravity = true;
										Main.dust[num126].noLight = true;
										Main.dust[num126].velocity /= 4f;
										Main.dust[num126].velocity -= base.velocity;
										Main.dust[num126].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
										if (Main.rand.Next(2) == 0)
										{
											num125 = -24;
											if (direction == 1)
											{
												num125 = 12;
											}
											float num127 = base.position.Y;
											if (gravDir == -1f)
											{
												num127 += (float)(height / 2);
											}
											num126 = Dust.NewDust(new Vector2(base.position.X + (float)(width / 2) + (float)num125, num127), 12, height / 2, 217, 0f, 0f, 100, default(Color), 1.4f);
											Main.dust[num126].noGravity = true;
											Main.dust[num126].noLight = true;
											Main.dust[num126].velocity /= 4f;
											Main.dust[num126].velocity -= base.velocity;
											Main.dust[num126].shader = GameShaders.Armor.GetSecondaryShader(cWings, this);
										}
										wingFrame = 2;
									}
									else if (wings != 24)
									{
										if (wings == 12)
										{
											wingFrame = 3;
										}
										else
										{
											wingFrame = 2;
										}
									}
								}
							}
							base.velocity.Y += gravity / 3f * gravDir;
							if (gravDir == 1f)
							{
								if (base.velocity.Y > maxFallSpeed / 3f && !controlDown)
								{
									base.velocity.Y = maxFallSpeed / 3f;
								}
							}
							else if (base.velocity.Y < (0f - maxFallSpeed) / 3f && !controlUp)
							{
								base.velocity.Y = (0f - maxFallSpeed) / 3f;
							}
						}
						else if (cartRampTime <= 0)
						{
							base.velocity.Y += gravity * gravDir;
						}
						else
						{
							cartRampTime--;
						}
					}
					if (!mount.Active || mount.Type != 5)
					{
						if (gravDir == 1f)
						{
							if (base.velocity.Y > maxFallSpeed)
							{
								base.velocity.Y = maxFallSpeed;
							}
							if (slowFall && base.velocity.Y > maxFallSpeed / 3f && !controlDown)
							{
								base.velocity.Y = maxFallSpeed / 3f;
							}
							if (slowFall && base.velocity.Y > maxFallSpeed / 5f && controlUp)
							{
								base.velocity.Y = maxFallSpeed / 10f;
							}
						}
						else
						{
							if (base.velocity.Y < 0f - maxFallSpeed)
							{
								base.velocity.Y = 0f - maxFallSpeed;
							}
							if (slowFall && base.velocity.Y < (0f - maxFallSpeed) / 3f && !controlDown)
							{
								base.velocity.Y = (0f - maxFallSpeed) / 3f;
							}
							if (slowFall && base.velocity.Y < (0f - maxFallSpeed) / 5f && controlUp)
							{
								base.velocity.Y = (0f - maxFallSpeed) / 10f;
							}
						}
					}
				}
			}
			if (mount.Active)
			{
				wingFrame = 0;
			}
			if ((wingsLogic == 22 || wingsLogic == 28 || wingsLogic == 30 || wingsLogic == 32 || wingsLogic == 33 || wingsLogic == 35) && controlDown && controlJump && wingTime > 0f && !merman)
			{
				base.velocity.Y *= 0.9f;
				if (base.velocity.Y > -2f && base.velocity.Y < 1f)
				{
					base.velocity.Y = 1E-05f;
				}
			}
			GrabItems(i);
			if (!Main.mapFullscreen)
			{
				if (base.position.X / 16f - (float)tileRangeX <= (float)tileTargetX && (base.position.X + (float)width) / 16f + (float)tileRangeX - 1f >= (float)tileTargetX && base.position.Y / 16f - (float)tileRangeY <= (float)tileTargetY && (base.position.Y + (float)height) / 16f + (float)tileRangeY - 2f >= (float)tileTargetY)
				{
					if (Main.tile[tileTargetX, tileTargetY] == null)
					{
						Main.tile[tileTargetX, tileTargetY] = new Tile();
					}
					if (Main.tile[tileTargetX, tileTargetY].active())
					{
						if (Main.tile[tileTargetX, tileTargetY].type == 79)
						{
							noThrow = 2;
							showItemIcon = true;
							int num128 = Main.tile[tileTargetX, tileTargetY].frameY / 36;
							switch (num128)
							{
							case 0:
								showItemIcon2 = 224;
								break;
							case 1:
								showItemIcon2 = 644;
								break;
							case 2:
								showItemIcon2 = 645;
								break;
							case 3:
								showItemIcon2 = 646;
								break;
							case 4:
								showItemIcon2 = 920;
								break;
							case 5:
								showItemIcon2 = 1470;
								break;
							case 6:
								showItemIcon2 = 1471;
								break;
							case 7:
								showItemIcon2 = 1472;
								break;
							case 8:
								showItemIcon2 = 1473;
								break;
							case 9:
								showItemIcon2 = 1719;
								break;
							case 10:
								showItemIcon2 = 1720;
								break;
							case 11:
								showItemIcon2 = 1721;
								break;
							case 12:
								showItemIcon2 = 1722;
								break;
							case 13:
							case 14:
							case 15:
							case 16:
							case 17:
							case 18:
								showItemIcon2 = 2066 + num128 - 13;
								break;
							default:
								if (num128 >= 19 && num128 <= 20)
								{
									showItemIcon2 = 2139 + num128 - 19;
									break;
								}
								switch (num128)
								{
								case 21:
									showItemIcon2 = 2231;
									break;
								case 22:
									showItemIcon2 = 2520;
									break;
								case 23:
									showItemIcon2 = 2538;
									break;
								case 24:
									showItemIcon2 = 2553;
									break;
								case 25:
									showItemIcon2 = 2568;
									break;
								case 26:
									showItemIcon2 = 2669;
									break;
								case 27:
									showItemIcon2 = 2811;
									break;
								case 28:
									showItemIcon2 = 3162;
									break;
								case 29:
									showItemIcon2 = 3164;
									break;
								case 30:
									showItemIcon2 = 3163;
									break;
								default:
									showItemIcon2 = 646;
									break;
								}
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 33)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 105;
							int num129 = Main.tile[tileTargetX, tileTargetY].frameY / 22;
							if (num129 == 1)
							{
								showItemIcon2 = 1405;
							}
							if (num129 == 2)
							{
								showItemIcon2 = 1406;
							}
							if (num129 == 3)
							{
								showItemIcon2 = 1407;
							}
							if (num129 >= 4 && num129 <= 13)
							{
								showItemIcon2 = 2045 + num129 - 4;
							}
							if (num129 >= 14 && num129 <= 16)
							{
								showItemIcon2 = 2153 + num129 - 14;
							}
							if (num129 == 17)
							{
								showItemIcon2 = 2236;
							}
							if (num129 == 18)
							{
								showItemIcon2 = 2523;
							}
							if (num129 == 19)
							{
								showItemIcon2 = 2542;
							}
							if (num129 == 20)
							{
								showItemIcon2 = 2556;
							}
							if (num129 == 21)
							{
								showItemIcon2 = 2571;
							}
							if (num129 == 22)
							{
								showItemIcon2 = 2648;
							}
							if (num129 == 23)
							{
								showItemIcon2 = 2649;
							}
							if (num129 == 24)
							{
								showItemIcon2 = 2650;
							}
							switch (num129)
							{
							case 25:
								showItemIcon2 = 2651;
								break;
							case 26:
								showItemIcon2 = 2818;
								break;
							case 27:
								showItemIcon2 = 3171;
								break;
							case 28:
								showItemIcon2 = 3173;
								break;
							case 29:
								showItemIcon2 = 3172;
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 21)
						{
							Tile tile = Main.tile[tileTargetX, tileTargetY];
							int num130 = tileTargetX;
							int num131 = tileTargetY;
							if (tile.frameX % 36 != 0)
							{
								num130--;
							}
							if (tile.frameY % 36 != 0)
							{
								num131--;
							}
							int num132 = Chest.FindChest(num130, num131);
							showItemIcon2 = -1;
							if (num132 < 0)
							{
								showItemIconText = Lang.chestType[0].Value;
							}
							else
							{
								if (Main.chest[num132].name != "")
								{
									showItemIconText = Main.chest[num132].name;
								}
								else
								{
									showItemIconText = Lang.chestType[tile.frameX / 36].Value;
								}
								if (showItemIconText == Lang.chestType[tile.frameX / 36].Value)
								{
									showItemIcon2 = Chest.chestTypeToIcon[tile.frameX / 36];
									showItemIconText = "";
								}
							}
							noThrow = 2;
							showItemIcon = true;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 88)
						{
							Tile tile2 = Main.tile[tileTargetX, tileTargetY];
							int num133 = tileTargetX;
							int num134 = tileTargetY;
							num133 -= tile2.frameX % 54 / 18;
							if (tile2.frameY % 36 != 0)
							{
								num134--;
							}
							int num135 = Chest.FindChest(num133, num134);
							showItemIcon2 = -1;
							if (num135 < 0)
							{
								showItemIconText = Lang.dresserType[0].Value;
							}
							else
							{
								if (Main.chest[num135].name != "")
								{
									showItemIconText = Main.chest[num135].name;
								}
								else
								{
									showItemIconText = Lang.dresserType[tile2.frameX / 54].Value;
								}
								if (showItemIconText == Lang.dresserType[tile2.frameX / 54].Value)
								{
									showItemIcon2 = Chest.dresserTypeToIcon[tile2.frameX / 54];
									showItemIconText = "";
								}
							}
							noThrow = 2;
							showItemIcon = true;
							if (Main.tile[tileTargetX, tileTargetY].frameY > 0)
							{
								showItemIcon2 = 269;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 10 || Main.tile[tileTargetX, tileTargetY].type == 11)
						{
							noThrow = 2;
							showItemIcon = true;
							int num136 = Main.tile[tileTargetX, tileTargetY].frameY;
							int num137 = 0;
							while (num136 >= 54)
							{
								num136 -= 54;
								num137++;
							}
							switch (num137)
							{
							case 0:
								showItemIcon2 = 25;
								break;
							case 9:
								showItemIcon2 = 837;
								break;
							case 10:
								showItemIcon2 = 912;
								break;
							case 11:
								showItemIcon2 = 1141;
								break;
							case 12:
								showItemIcon2 = 1137;
								break;
							case 13:
								showItemIcon2 = 1138;
								break;
							case 14:
								showItemIcon2 = 1139;
								break;
							case 15:
								showItemIcon2 = 1140;
								break;
							case 16:
								showItemIcon2 = 1411;
								break;
							case 17:
								showItemIcon2 = 1412;
								break;
							case 18:
								showItemIcon2 = 1413;
								break;
							case 19:
								showItemIcon2 = 1458;
								break;
							case 20:
							case 21:
							case 22:
							case 23:
								showItemIcon2 = 1709 + num137 - 20;
								break;
							default:
								switch (num137)
								{
								case 24:
									showItemIcon2 = 1793;
									break;
								case 25:
									showItemIcon2 = 1815;
									break;
								case 26:
									showItemIcon2 = 1924;
									break;
								case 27:
									showItemIcon2 = 2044;
									break;
								case 28:
									showItemIcon2 = 2265;
									break;
								case 29:
									showItemIcon2 = 2528;
									break;
								case 30:
									showItemIcon2 = 2561;
									break;
								case 31:
									showItemIcon2 = 2576;
									break;
								case 32:
									showItemIcon2 = 2815;
									break;
								case 33:
									showItemIcon2 = 3129;
									break;
								case 34:
									showItemIcon2 = 3131;
									break;
								case 35:
									showItemIcon2 = 3130;
									break;
								case 4:
								case 5:
								case 6:
								case 7:
								case 8:
									showItemIcon2 = 812 + num137;
									break;
								default:
									showItemIcon2 = 649 + num137;
									break;
								}
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 104)
						{
							noThrow = 2;
							showItemIcon = true;
							switch (Main.tile[tileTargetX, tileTargetY].frameX / 36)
							{
							case 0:
								showItemIcon2 = 359;
								break;
							case 1:
								showItemIcon2 = 2237;
								break;
							case 2:
								showItemIcon2 = 2238;
								break;
							case 3:
								showItemIcon2 = 2239;
								break;
							case 4:
								showItemIcon2 = 2240;
								break;
							case 5:
								showItemIcon2 = 2241;
								break;
							case 6:
								showItemIcon2 = 2560;
								break;
							case 7:
								showItemIcon2 = 2575;
								break;
							case 8:
								showItemIcon2 = 2591;
								break;
							case 9:
								showItemIcon2 = 2592;
								break;
							case 10:
								showItemIcon2 = 2593;
								break;
							case 11:
								showItemIcon2 = 2594;
								break;
							case 12:
								showItemIcon2 = 2595;
								break;
							case 13:
								showItemIcon2 = 2596;
								break;
							case 14:
								showItemIcon2 = 2597;
								break;
							case 15:
								showItemIcon2 = 2598;
								break;
							case 16:
								showItemIcon2 = 2599;
								break;
							case 17:
								showItemIcon2 = 2600;
								break;
							case 18:
								showItemIcon2 = 2601;
								break;
							case 19:
								showItemIcon2 = 2602;
								break;
							case 20:
								showItemIcon2 = 2603;
								break;
							case 21:
								showItemIcon2 = 2604;
								break;
							case 22:
								showItemIcon2 = 2605;
								break;
							case 23:
								showItemIcon2 = 2606;
								break;
							case 24:
								showItemIcon2 = 2809;
								break;
							case 25:
								showItemIcon2 = 3126;
								break;
							case 26:
								showItemIcon2 = 3128;
								break;
							case 27:
								showItemIcon2 = 3127;
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 356)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 3064;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 377)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 3198;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 209)
						{
							noThrow = 2;
							showItemIcon = true;
							if (Main.tile[tileTargetX, tileTargetY].frameX < 72)
							{
								showItemIcon2 = 928;
							}
							else if (Main.tile[tileTargetX, tileTargetY].frameX < 144)
							{
								showItemIcon2 = 1337;
							}
							else if (Main.tile[tileTargetX, tileTargetY].frameX < 216)
							{
								showItemIcon2 = 3369;
							}
							int num138;
							for (num138 = Main.tile[tileTargetX, tileTargetY].frameX / 18; num138 >= 4; num138 -= 4)
							{
							}
							if (num138 < 2)
							{
								showItemIconR = true;
							}
							else
							{
								showItemIconR = false;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 216)
						{
							noThrow = 2;
							showItemIcon = true;
							int num139 = Main.tile[tileTargetX, tileTargetY].frameY;
							int num140 = 0;
							while (num139 >= 40)
							{
								num139 -= 40;
								num140++;
							}
							showItemIcon2 = 970 + num140;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 387 || Main.tile[tileTargetX, tileTargetY].type == 386)
						{
							noThrow = 2;
							showItemIcon = true;
							int x7 = 0;
							int y6 = 0;
							WorldGen.GetTopLeftAndStyles(ref x7, ref y6, 2, 1 + (Main.tile[tileTargetX, tileTargetY].type == 386).ToInt(), 18, 18);
							showItemIcon2 = 3239;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 389 || Main.tile[tileTargetX, tileTargetY].type == 388)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 3240;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 335)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 2700;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 410)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 3536 + Math.Min(Main.tile[tileTargetX, tileTargetY].frameX / 36, 3);
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 411 && Main.tile[tileTargetX, tileTargetY].frameX < 36)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 3545;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 338)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 2738;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 219 && (inventory[selectedItem].type == 424 || inventory[selectedItem].type == 1103))
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = inventory[selectedItem].type;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 212)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 949;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 314 && gravDir == 1f)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 2343;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 215)
						{
							noThrow = 2;
							showItemIcon = true;
							int num141 = Main.tile[tileTargetX, tileTargetY].frameX / 54;
							switch (num141)
							{
							case 0:
								showItemIcon2 = 966;
								break;
							case 6:
								showItemIcon2 = 3050;
								break;
							default:
								showItemIcon2 = 3046 + num141 - 1;
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 4)
						{
							noThrow = 2;
							showItemIcon = true;
							int num142 = Main.tile[tileTargetX, tileTargetY].frameY / 22;
							switch (num142)
							{
							case 0:
								showItemIcon2 = 8;
								break;
							case 8:
								showItemIcon2 = 523;
								break;
							case 9:
								showItemIcon2 = 974;
								break;
							case 10:
								showItemIcon2 = 1245;
								break;
							case 11:
								showItemIcon2 = 1333;
								break;
							case 12:
								showItemIcon2 = 2274;
								break;
							case 13:
								showItemIcon2 = 3004;
								break;
							case 14:
								showItemIcon2 = 3045;
								break;
							case 15:
								showItemIcon2 = 3114;
								break;
							default:
								showItemIcon2 = 426 + num142;
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 13)
						{
							noThrow = 2;
							showItemIcon = true;
							switch (Main.tile[tileTargetX, tileTargetY].frameX / 18)
							{
							case 1:
								showItemIcon2 = 28;
								break;
							case 2:
								showItemIcon2 = 110;
								break;
							case 3:
								showItemIcon2 = 350;
								break;
							case 4:
								showItemIcon2 = 351;
								break;
							case 5:
								showItemIcon2 = 2234;
								break;
							case 6:
								showItemIcon2 = 2244;
								break;
							case 7:
								showItemIcon2 = 2257;
								break;
							case 8:
								showItemIcon2 = 2258;
								break;
							default:
								showItemIcon2 = 31;
								break;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 29)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 87;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 97)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 346;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 49)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 148;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 174)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 713;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 50)
						{
							noThrow = 2;
							if (Main.tile[tileTargetX, tileTargetY].frameX == 90)
							{
								showItemIcon = true;
								showItemIcon2 = 165;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 139)
						{
							noThrow = 2;
							int num143 = tileTargetX;
							int num144 = tileTargetY;
							int num145 = 0;
							for (int num146 = Main.tile[num143, num144].frameY / 18; num146 >= 2; num146 -= 2)
							{
								num145++;
							}
							showItemIcon = true;
							if (num145 == 28)
							{
								showItemIcon2 = 1963;
							}
							else if (num145 == 29)
							{
								showItemIcon2 = 1964;
							}
							else if (num145 == 30)
							{
								showItemIcon2 = 1965;
							}
							else if (num145 == 31)
							{
								showItemIcon2 = 2742;
							}
							else if (num145 == 32)
							{
								showItemIcon2 = 3044;
							}
							else if (num145 == 33)
							{
								showItemIcon2 = 3235;
							}
							else if (num145 == 34)
							{
								showItemIcon2 = 3236;
							}
							else if (num145 == 35)
							{
								showItemIcon2 = 3237;
							}
							else if (num145 == 36)
							{
								showItemIcon2 = 3370;
							}
							else if (num145 == 37)
							{
								showItemIcon2 = 3371;
							}
							else if (num145 >= 13)
							{
								showItemIcon2 = 1596 + num145 - 13;
							}
							else
							{
								showItemIcon2 = 562 + num145;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 207)
						{
							noThrow = 2;
							int num147 = tileTargetX;
							int num148 = tileTargetY;
							int num149 = 0;
							for (int num150 = Main.tile[num147, num148].frameX / 18; num150 >= 2; num150 -= 2)
							{
								num149++;
							}
							showItemIcon = true;
							switch (num149)
							{
							case 0:
								showItemIcon2 = 909;
								break;
							case 1:
								showItemIcon2 = 910;
								break;
							case 2:
								showItemIcon2 = 940;
								break;
							case 3:
								showItemIcon2 = 941;
								break;
							case 4:
								showItemIcon2 = 942;
								break;
							case 5:
								showItemIcon2 = 943;
								break;
							case 6:
								showItemIcon2 = 944;
								break;
							case 7:
								showItemIcon2 = 945;
								break;
							}
						}
						if (Main.tileSign[Main.tile[tileTargetX, tileTargetY].type])
						{
							noThrow = 2;
							int num151 = Main.tile[tileTargetX, tileTargetY].frameX / 18;
							int num152 = Main.tile[tileTargetX, tileTargetY].frameY / 18;
							num151 %= 2;
							int num153 = tileTargetX - num151;
							int num154 = tileTargetY - num152;
							Main.signBubble = true;
							Main.signX = num153 * 16 + 16;
							Main.signY = num154 * 16;
							int num155 = Sign.ReadSign(num153, num154);
							if (num155 != -1)
							{
								Main.signHover = num155;
							}
							if (num155 != -1)
							{
								Main.signHover = num155;
								showItemIcon = false;
								showItemIcon2 = -1;
							}
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 237)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 1293;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 125)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 487;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 354)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 2999;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 287)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 2177;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 132)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 513;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 136)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 538;
						}
						if (Main.tile[tileTargetX, tileTargetY].type == 144)
						{
							noThrow = 2;
							showItemIcon = true;
							showItemIcon2 = 583 + Main.tile[tileTargetX, tileTargetY].frameX / 18;
						}
						if (controlUseTile)
						{
							if (Main.tile[tileTargetX, tileTargetY].type == 212 && launcherWait <= 0)
							{
								int num156 = tileTargetX;
								int num157 = tileTargetY;
								bool flag27 = false;
								for (int num158 = 0; num158 < 58; num158++)
								{
									if (inventory[num158].type == 949 && inventory[num158].stack > 0)
									{
										inventory[num158].stack--;
										if (inventory[num158].stack <= 0)
										{
											inventory[num158].SetDefaults();
										}
										flag27 = true;
										break;
									}
								}
								if (flag27)
								{
									launcherWait = 10;
									Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 11);
									int num159 = Main.tile[num156, num157].frameX / 18;
									int num160 = 0;
									while (num159 >= 3)
									{
										num160++;
										num159 -= 3;
									}
									num159 = num156 - num159;
									int num161;
									for (num161 = Main.tile[num156, num157].frameY / 18; num161 >= 3; num161 -= 3)
									{
									}
									num161 = num157 - num161;
									float num162 = 12f + (float)Main.rand.Next(450) * 0.01f;
									float num163 = Main.rand.Next(85, 105);
									float num164 = Main.rand.Next(-35, 11);
									int type4 = 166;
									int damage2 = 35;
									float knockBack = 3.5f;
									Vector2 vector3 = new Vector2((num159 + 2) * 16 - 8, (num161 + 2) * 16 - 8);
									if (num160 == 0)
									{
										num163 *= -1f;
										vector3.X -= 12f;
									}
									else
									{
										vector3.X += 12f;
									}
									float num165 = num163;
									float num166 = num164;
									float num167 = (float)Math.Sqrt(num165 * num165 + num166 * num166);
									num167 = num162 / num167;
									num165 *= num167;
									num166 *= num167;
									Projectile.NewProjectile(vector3.X, vector3.Y, num165, num166, type4, damage2, knockBack, Main.myPlayer);
								}
							}
							if (releaseUseTile)
							{
								if (Main.tile[tileTargetX, tileTargetY].type == 132 || Main.tile[tileTargetX, tileTargetY].type == 136 || Main.tile[tileTargetX, tileTargetY].type == 144)
								{
									Wiring.HitSwitch(tileTargetX, tileTargetY);
									NetMessage.SendData(59, -1, -1, "", tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 139)
								{
									Main.PlaySound(28, tileTargetX * 16, tileTargetY * 16, 0);
									WorldGen.SwitchMB(tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 215)
								{
									Main.PlaySound(28, tileTargetX * 16, tileTargetY * 16, 0);
									int num168 = Main.tile[tileTargetX, tileTargetY].frameX % 54 / 18;
									int num169 = Main.tile[tileTargetX, tileTargetY].frameY % 36 / 18;
									int num170 = tileTargetX - num168;
									int num171 = tileTargetY - num169;
									int num172 = 36;
									if (Main.tile[num170, num171].frameY >= 36)
									{
										num172 = -36;
									}
									for (int num173 = num170; num173 < num170 + 3; num173++)
									{
										for (int num174 = num171; num174 < num171 + 2; num174++)
										{
											Main.tile[num173, num174].frameY = (short)(Main.tile[num173, num174].frameY + num172);
										}
									}
									NetMessage.SendTileSquare(-1, num170 + 1, num171 + 1, 3);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 207)
								{
									Main.PlaySound(28, tileTargetX * 16, tileTargetY * 16, 0);
									WorldGen.SwitchFountain(tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 410)
								{
									Main.PlaySound(28, tileTargetX * 16, tileTargetY * 16, 0);
									WorldGen.SwitchMonolith(tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 216)
								{
									WorldGen.LaunchRocket(tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 386 || Main.tile[tileTargetX, tileTargetY].type == 387)
								{
									bool value3 = Main.tile[tileTargetX, tileTargetY].type == 387;
									int num175 = WorldGen.ShiftTrapdoor(tileTargetX, tileTargetY, (float)(tileTargetY * 16) > base.Center.Y).ToInt();
									if (num175 == 0)
									{
										num175 = -WorldGen.ShiftTrapdoor(tileTargetX, tileTargetY, (float)(tileTargetY * 16) <= base.Center.Y).ToInt();
									}
									if (num175 != 0)
									{
										NetMessage.SendData(19, -1, -1, "", 2 + value3.ToInt(), tileTargetX, tileTargetY, num175 * Math.Sign((float)(tileTargetY * 16) - base.Center.Y));
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 388 || Main.tile[tileTargetX, tileTargetY].type == 389)
								{
									bool flag28 = Main.tile[tileTargetX, tileTargetY].type == 389;
									WorldGen.ShiftTallGate(tileTargetX, tileTargetY, flag28);
									NetMessage.SendData(19, -1, -1, "", 4 + flag28.ToInt(), tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 335)
								{
									WorldGen.LaunchRocketSmall(tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 411 && Main.tile[tileTargetX, tileTargetY].frameX < 36)
								{
									Wiring.HitSwitch(tileTargetX, tileTargetY);
									NetMessage.SendData(59, -1, -1, "", tileTargetX, tileTargetY);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 338)
								{
									int num176 = tileTargetX;
									int num177 = tileTargetY;
									if (Main.tile[num176, num177].frameY == 18)
									{
										num177--;
									}
									bool flag29 = false;
									for (int num178 = 0; num178 < 1000; num178++)
									{
										if (Main.projectile[num178].active && Main.projectile[num178].aiStyle == 73 && Main.projectile[num178].ai[0] == (float)num176 && Main.projectile[num178].ai[1] == (float)num177)
										{
											flag29 = true;
											break;
										}
									}
									if (!flag29)
									{
										Projectile.NewProjectile(num176 * 16 + 8, num177 * 16 + 2, 0f, 0f, 419 + Main.rand.Next(4), 0, 0f, whoAmI, num176, num177);
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 4 || Main.tile[tileTargetX, tileTargetY].type == 13 || Main.tile[tileTargetX, tileTargetY].type == 33 || Main.tile[tileTargetX, tileTargetY].type == 49 || (Main.tile[tileTargetX, tileTargetY].type == 50 && Main.tile[tileTargetX, tileTargetY].frameX == 90) || Main.tile[tileTargetX, tileTargetY].type == 174)
								{
									WorldGen.KillTile(tileTargetX, tileTargetY);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY);
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 334)
								{
									if (ItemFitsWeaponRack(inventory[selectedItem]))
									{
										PlaceWeapon(tileTargetX, tileTargetY);
									}
									else
									{
										int num179 = tileTargetX;
										int num180 = tileTargetY;
										if (Main.tile[tileTargetX, tileTargetY].frameY == 0)
										{
											num180++;
										}
										if (Main.tile[tileTargetX, tileTargetY].frameY == 36)
										{
											num180--;
										}
										int frameX = Main.tile[tileTargetX, num180].frameX;
										int num181 = Main.tile[tileTargetX, num180].frameX;
										int num182 = 0;
										while (num181 >= 5000)
										{
											num181 -= 5000;
											num182++;
										}
										if (num182 != 0)
										{
											num181 = (num182 - 1) * 18;
										}
										num181 %= 54;
										if (num181 == 18)
										{
											frameX = Main.tile[tileTargetX - 1, num180].frameX;
											num179--;
										}
										if (num181 == 36)
										{
											frameX = Main.tile[tileTargetX - 2, num180].frameX;
											num179 -= 2;
										}
										if (frameX >= 5000)
										{
											WorldGen.KillTile(tileTargetX, num180, true);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, num180, 1f);
											}
										}
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 395)
								{
									if (ItemFitsItemFrame(inventory[selectedItem]) && !inventory[selectedItem].favorited)
									{
										PlaceItemInFrame(tileTargetX, tileTargetY);
									}
									else
									{
										int num183 = tileTargetX;
										int num184 = tileTargetY;
										if (Main.tile[num183, num184].frameX % 36 != 0)
										{
											num183--;
										}
										if (Main.tile[num183, num184].frameY % 36 != 0)
										{
											num184--;
										}
										int num185 = TEItemFrame.Find(num183, num184);
										if (num185 != -1 && ((TEItemFrame)TileEntity.ByID[num185]).item.stack > 0)
										{
											WorldGen.KillTile(tileTargetX, num184, true);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, num184, 1f);
											}
										}
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 125)
								{
									AddBuff(29, 36000);
									Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 4);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 377)
								{
									AddBuff(159, 36000);
									Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 37);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 354)
								{
									AddBuff(150, 36000);
									Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 4);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 287)
								{
									AddBuff(93, 36000);
									Main.PlaySound(7, (int)base.position.X, (int)base.position.Y);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 356)
								{
									if (!Main.fastForwardTime && (Main.netMode == 1 || Main.sundialCooldown == 0))
									{
										Main.Sundialing();
										Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, 4);
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 79)
								{
									int num186 = tileTargetX;
									int num187 = tileTargetY;
									num186 += Main.tile[tileTargetX, tileTargetY].frameX / 18 * -1;
									if (Main.tile[tileTargetX, tileTargetY].frameX >= 72)
									{
										num186 += 4;
										num186++;
									}
									else
									{
										num186 += 2;
									}
									int num188 = Main.tile[tileTargetX, tileTargetY].frameY / 18;
									int num189 = 0;
									while (num188 > 1)
									{
										num188 -= 2;
										num189++;
									}
									num187 -= num188;
									num187 += 2;
									FindSpawn();
									if (SpawnX == num186 && SpawnY == num187)
									{
										RemoveSpawn();
										Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
									}
									else if (CheckSpawn(num186, num187))
									{
										ChangeSpawn(num186, num187);
										Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
									}
								}
								else if (Main.tileSign[Main.tile[tileTargetX, tileTargetY].type])
								{
									bool flag30 = true;
									if (sign >= 0)
									{
										int num190 = Sign.ReadSign(tileTargetX, tileTargetY);
										if (num190 == sign)
										{
											sign = -1;
											Main.npcChatText = "";
											Main.editSign = false;
											Main.PlaySound(11);
											flag30 = false;
										}
									}
									if (flag30)
									{
										if (Main.netMode == 0)
										{
											talkNPC = -1;
											Main.npcChatCornerItem = 0;
											Main.playerInventory = false;
											Main.editSign = false;
											Main.PlaySound(10);
											int num191 = sign = Sign.ReadSign(tileTargetX, tileTargetY);
											Main.npcChatText = Main.sign[num191].text;
										}
										else
										{
											int num192 = Main.tile[tileTargetX, tileTargetY].frameX / 18;
											int num193 = Main.tile[tileTargetX, tileTargetY].frameY / 18;
											while (num192 > 1)
											{
												num192 -= 2;
											}
											int num194 = tileTargetX - num192;
											int num195 = tileTargetY - num193;
											if (Main.tileSign[Main.tile[num194, num195].type])
											{
												NetMessage.SendData(46, -1, -1, "", num194, num195);
											}
										}
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 104)
								{
									string text = "AM";
									double num196 = Main.time;
									if (!Main.dayTime)
									{
										num196 += 54000.0;
									}
									num196 = num196 / 86400.0 * 24.0;
									double num197 = 7.5;
									num196 = num196 - num197 - 12.0;
									if (num196 < 0.0)
									{
										num196 += 24.0;
									}
									if (num196 >= 12.0)
									{
										text = "PM";
									}
									int num198 = (int)num196;
									double num199 = num196 - (double)num198;
									num199 = (int)(num199 * 60.0);
									string text2 = string.Concat(num199);
									if (num199 < 10.0)
									{
										text2 = "0" + text2;
									}
									if (num198 > 12)
									{
										num198 -= 12;
									}
									if (num198 == 0)
									{
										num198 = 12;
									}
									string newText = "Time: " + num198 + ":" + text2 + " " + text;
									Main.NewText(newText, byte.MaxValue, 240, 20);
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 237)
								{
									bool flag31 = false;
									if (!NPC.AnyNPCs(245) && Main.hardMode && NPC.downedPlantBoss)
									{
										for (int num200 = 0; num200 < 58; num200++)
										{
											if (inventory[num200].type == 1293)
											{
												inventory[num200].stack--;
												if (inventory[num200].stack <= 0)
												{
													inventory[num200].SetDefaults();
												}
												flag31 = true;
												break;
											}
										}
									}
									if (flag31)
									{
										Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
										if (Main.netMode != 1)
										{
											NPC.SpawnOnPlayer(i, 245);
										}
										else
										{
											NetMessage.SendData(61, -1, -1, "", whoAmI, 245f);
										}
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 10)
								{
									int num201 = tileTargetX;
									int num202 = tileTargetY;
									if (Main.tile[num201, num202].frameY >= 594 && Main.tile[num201, num202].frameY <= 646)
									{
										int num203 = 1141;
										for (int num204 = 0; num204 < 58; num204++)
										{
											if (inventory[num204].type == num203 && inventory[num204].stack > 0)
											{
												inventory[num204].stack--;
												if (inventory[num204].stack <= 0)
												{
													inventory[num204] = new Item();
												}
												WorldGen.UnlockDoor(num201, num202);
												if (Main.netMode == 1)
												{
													NetMessage.SendData(52, -1, -1, "", whoAmI, 2f, num201, num202);
												}
											}
										}
									}
									else
									{
										WorldGen.OpenDoor(tileTargetX, tileTargetY, direction);
										NetMessage.SendData(19, -1, -1, "", 0, tileTargetX, tileTargetY, direction);
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 11 && WorldGen.CloseDoor(tileTargetX, tileTargetY))
								{
									NetMessage.SendData(19, -1, -1, "", 1, tileTargetX, tileTargetY, direction);
								}
								if (Main.tile[tileTargetX, tileTargetY].type == 88)
								{
									if (Main.tile[tileTargetX, tileTargetY].frameY == 0)
									{
										Main.CancelClothesWindow(true);
										Main.mouseRightRelease = false;
										int num205 = Main.tile[tileTargetX, tileTargetY].frameX / 18;
										num205 %= 3;
										num205 = tileTargetX - num205;
										int num206 = tileTargetY - Main.tile[tileTargetX, tileTargetY].frameY / 18;
										if (sign > -1)
										{
											Main.PlaySound(11);
											sign = -1;
											Main.editSign = false;
											Main.npcChatText = string.Empty;
										}
										if (Main.editChest)
										{
											Main.PlaySound(12);
											Main.editChest = false;
											Main.npcChatText = string.Empty;
										}
										if (editedChestName)
										{
											NetMessage.SendData(33, -1, -1, Main.chest[chest].name, chest, 1f);
											editedChestName = false;
										}
										if (Main.netMode == 1)
										{
											if (num205 == chestX && num206 == chestY && chest != -1)
											{
												chest = -1;
												Recipe.FindRecipes();
												Main.PlaySound(11);
											}
											else
											{
												NetMessage.SendData(31, -1, -1, "", num205, num206);
												Main.stackSplit = 600;
											}
										}
										else
										{
											flyingPigChest = -1;
											int num207 = Chest.FindChest(num205, num206);
											if (num207 != -1)
											{
												Main.stackSplit = 600;
												if (num207 == chest)
												{
													chest = -1;
													Recipe.FindRecipes();
													Main.PlaySound(11);
												}
												else if (num207 != chest && chest == -1)
												{
													chest = num207;
													Main.playerInventory = true;
													Main.recBigList = false;
													Main.PlaySound(10);
													chestX = num205;
													chestY = num206;
												}
												else
												{
													chest = num207;
													Main.playerInventory = true;
													Main.recBigList = false;
													Main.PlaySound(12);
													chestX = num205;
													chestY = num206;
												}
												Recipe.FindRecipes();
											}
										}
									}
									else
									{
										Main.playerInventory = false;
										chest = -1;
										Recipe.FindRecipes();
										Main.dresserX = tileTargetX;
										Main.dresserY = tileTargetY;
										Main.OpenClothesWindow();
									}
								}
								if (Main.tile[tileTargetX, tileTargetY].type == 209)
								{
									WorldGen.SwitchCannon(tileTargetX, tileTargetY);
								}
								else if ((Main.tile[tileTargetX, tileTargetY].type == 21 || Main.tile[tileTargetX, tileTargetY].type == 29 || Main.tile[tileTargetX, tileTargetY].type == 97) && talkNPC == -1)
								{
									Main.mouseRightRelease = false;
									int num208 = 0;
									int num209;
									for (num209 = Main.tile[tileTargetX, tileTargetY].frameX / 18; num209 > 1; num209 -= 2)
									{
									}
									num209 = tileTargetX - num209;
									int num210 = tileTargetY - Main.tile[tileTargetX, tileTargetY].frameY / 18;
									if (Main.tile[tileTargetX, tileTargetY].type == 29)
									{
										num208 = 1;
									}
									else if (Main.tile[tileTargetX, tileTargetY].type == 97)
									{
										num208 = 2;
									}
									if (sign > -1)
									{
										Main.PlaySound(11);
										sign = -1;
										Main.editSign = false;
										Main.npcChatText = string.Empty;
									}
									if (Main.editChest)
									{
										Main.PlaySound(12);
										Main.editChest = false;
										Main.npcChatText = string.Empty;
									}
									if (editedChestName)
									{
										NetMessage.SendData(33, -1, -1, Main.chest[chest].name, chest, 1f);
										editedChestName = false;
									}
									if (Main.netMode == 1 && num208 == 0 && (Main.tile[num209, num210].frameX < 72 || Main.tile[num209, num210].frameX > 106) && (Main.tile[num209, num210].frameX < 144 || Main.tile[num209, num210].frameX > 178) && (Main.tile[num209, num210].frameX < 828 || Main.tile[num209, num210].frameX > 1006) && (Main.tile[num209, num210].frameX < 1296 || Main.tile[num209, num210].frameX > 1330) && (Main.tile[num209, num210].frameX < 1368 || Main.tile[num209, num210].frameX > 1402) && (Main.tile[num209, num210].frameX < 1440 || Main.tile[num209, num210].frameX > 1474))
									{
										if (num209 == chestX && num210 == chestY && chest != -1)
										{
											chest = -1;
											Recipe.FindRecipes();
											Main.PlaySound(11);
										}
										else
										{
											NetMessage.SendData(31, -1, -1, "", num209, num210);
											Main.stackSplit = 600;
										}
									}
									else
									{
										int num211 = -1;
										switch (num208)
										{
										case 1:
											num211 = -2;
											break;
										case 2:
											num211 = -3;
											break;
										default:
										{
											bool flag32 = false;
											if (Chest.isLocked(num209, num210))
											{
												int num212 = 327;
												if (Main.tile[num209, num210].frameX >= 144 && Main.tile[num209, num210].frameX <= 178)
												{
													num212 = 329;
												}
												if (Main.tile[num209, num210].frameX >= 828 && Main.tile[num209, num210].frameX <= 1006)
												{
													int num213 = Main.tile[num209, num210].frameX / 18;
													int num214 = 0;
													while (num213 >= 2)
													{
														num213 -= 2;
														num214++;
													}
													num214 -= 23;
													num212 = 1533 + num214;
												}
												flag32 = true;
												for (int num215 = 0; num215 < 58; num215++)
												{
													if (inventory[num215].type != num212 || inventory[num215].stack <= 0 || !Chest.Unlock(num209, num210))
													{
														continue;
													}
													if (num212 != 329)
													{
														inventory[num215].stack--;
														if (inventory[num215].stack <= 0)
														{
															inventory[num215] = new Item();
														}
													}
													if (Main.netMode == 1)
													{
														NetMessage.SendData(52, -1, -1, "", whoAmI, 1f, num209, num210);
													}
												}
											}
											if (!flag32)
											{
												num211 = Chest.FindChest(num209, num210);
											}
											break;
										}
										}
										if (num211 != -1)
										{
											Main.stackSplit = 600;
											if (num211 == chest)
											{
												chest = -1;
												Main.PlaySound(11);
											}
											else if (num211 != chest && chest == -1)
											{
												chest = num211;
												Main.playerInventory = true;
												Main.recBigList = false;
												Main.PlaySound(10);
												chestX = num209;
												chestY = num210;
												if (Main.tile[num209, num210].frameX >= 36 && Main.tile[num209, num210].frameX < 72)
												{
													AchievementsHelper.HandleSpecialEvent(this, 16);
												}
											}
											else
											{
												chest = num211;
												Main.playerInventory = true;
												Main.recBigList = false;
												Main.PlaySound(12);
												chestX = num209;
												chestY = num210;
											}
											Recipe.FindRecipes();
										}
									}
								}
								else if (Main.tile[tileTargetX, tileTargetY].type == 314 && gravDir == 1f)
								{
									bool flag33 = true;
									if (mount.Active)
									{
										if (mount.Cart)
										{
											flag33 = false;
										}
										else
										{
											mount.Dismount(this);
										}
									}
									if (flag33)
									{
										Vector2 vector4 = new Vector2((float)Main.mouseX + Main.screenPosition.X, (float)Main.mouseY + Main.screenPosition.Y);
										if (direction > 0)
										{
											minecartLeft = false;
										}
										else
										{
											minecartLeft = true;
										}
										grappling[0] = -1;
										grapCount = 0;
										for (int num216 = 0; num216 < 1000; num216++)
										{
											if (Main.projectile[num216].active && Main.projectile[num216].owner == whoAmI && Main.projectile[num216].aiStyle == 7)
											{
												Main.projectile[num216].Kill();
											}
										}
										Projectile.NewProjectile(vector4.X, vector4.Y, 0f, 0f, 403, 0, 0f, whoAmI);
									}
								}
							}
							releaseUseTile = false;
						}
						else
						{
							releaseUseTile = true;
						}
					}
				}
				else
				{
					if (Main.tile[tileTargetX, tileTargetY] == null)
					{
						Main.tile[tileTargetX, tileTargetY] = new Tile();
					}
					if (Main.tile[tileTargetX, tileTargetY].type == 21)
					{
						Tile tile3 = Main.tile[tileTargetX, tileTargetY];
						int num217 = tileTargetX;
						int num218 = tileTargetY;
						if (tile3.frameX % 36 != 0)
						{
							num217--;
						}
						if (tile3.frameY % 36 != 0)
						{
							num218--;
						}
						int num219 = Chest.FindChest(num217, num218);
						showItemIcon2 = -1;
						if (num219 < 0)
						{
							showItemIconText = Lang.chestType[0].Value;
						}
						else
						{
							if (Main.chest[num219].name != "")
							{
								showItemIconText = Main.chest[num219].name;
							}
							else
							{
								showItemIconText = Lang.chestType[tile3.frameX / 36].Value;
							}
							if (showItemIconText == Lang.chestType[tile3.frameX / 36].Value)
							{
								showItemIcon2 = Chest.chestTypeToIcon[tile3.frameX / 36];
								showItemIconText = "";
							}
						}
						noThrow = 2;
						showItemIcon = true;
						if (showItemIconText == "")
						{
							showItemIcon = false;
							showItemIcon2 = 0;
						}
					}
					if (Main.tile[tileTargetX, tileTargetY].type == 88)
					{
						Tile tile4 = Main.tile[tileTargetX, tileTargetY];
						int num220 = tileTargetX;
						int num221 = tileTargetY;
						num220 -= tile4.frameX % 54 / 18;
						if (tile4.frameY % 36 != 0)
						{
							num221--;
						}
						int num222 = Chest.FindChest(num220, num221);
						showItemIcon2 = -1;
						if (num222 < 0)
						{
							showItemIconText = Lang.dresserType[0].Value;
						}
						else
						{
							if (Main.chest[num222].name != "")
							{
								showItemIconText = Main.chest[num222].name;
							}
							else
							{
								showItemIconText = Lang.dresserType[tile4.frameX / 54].Value;
							}
							if (showItemIconText == Lang.dresserType[tile4.frameX / 54].Value)
							{
								showItemIcon2 = Chest.dresserTypeToIcon[tile4.frameX / 54];
								showItemIconText = "";
							}
						}
						noThrow = 2;
						showItemIcon = true;
						if (showItemIconText == "")
						{
							showItemIcon = false;
							showItemIcon2 = 0;
						}
					}
					if (Main.tileSign[Main.tile[tileTargetX, tileTargetY].type])
					{
						noThrow = 2;
						int num223 = Main.tile[tileTargetX, tileTargetY].frameX / 18;
						int num224 = Main.tile[tileTargetX, tileTargetY].frameY / 18;
						num223 %= 2;
						int num225 = tileTargetX - num223;
						int num226 = tileTargetY - num224;
						Main.signBubble = true;
						Main.signX = num225 * 16 + 16;
						Main.signY = num226 * 16;
						int num227 = Sign.ReadSign(num225, num226);
						if (num227 != -1)
						{
							Main.signHover = num227;
							showItemIcon = false;
							showItemIcon2 = -1;
						}
					}
				}
			}
			if (tongued)
			{
				bool flag34 = false;
				if (Main.wof >= 0)
				{
					float num228 = Main.npc[Main.wof].position.X + (float)(Main.npc[Main.wof].width / 2);
					num228 += (float)(Main.npc[Main.wof].direction * 200);
					float num229 = Main.npc[Main.wof].position.Y + (float)(Main.npc[Main.wof].height / 2);
					Vector2 vector5 = new Vector2(base.position.X + (float)width * 0.5f, base.position.Y + (float)height * 0.5f);
					float num230 = num228 - vector5.X;
					float num231 = num229 - vector5.Y;
					float num232 = (float)Math.Sqrt(num230 * num230 + num231 * num231);
					float num233 = 11f;
					float num234 = num232;
					if (num232 > num233)
					{
						num234 = num233 / num232;
					}
					else
					{
						num234 = 1f;
						flag34 = true;
					}
					num230 *= num234;
					num231 *= num234;
					base.velocity.X = num230;
					base.velocity.Y = num231;
				}
				else
				{
					flag34 = true;
				}
				if (flag34 && Main.myPlayer == whoAmI)
				{
					for (int num235 = 0; num235 < 22; num235++)
					{
						if (buffType[num235] == 38)
						{
							DelBuff(num235);
						}
					}
				}
			}
			if (Main.myPlayer == whoAmI)
			{
				WOFTongue();
				if (controlHook)
				{
					if (releaseHook)
					{
						QuickGrapple();
					}
					releaseHook = false;
				}
				else
				{
					releaseHook = true;
				}
				if (talkNPC >= 0)
				{
					Rectangle rectangle = new Rectangle((int)(base.position.X + (float)(width / 2) - (float)(tileRangeX * 16)), (int)(base.position.Y + (float)(height / 2) - (float)(tileRangeY * 16)), tileRangeX * 16 * 2, tileRangeY * 16 * 2);
					Rectangle value4 = new Rectangle((int)Main.npc[talkNPC].position.X, (int)Main.npc[talkNPC].position.Y, Main.npc[talkNPC].width, Main.npc[talkNPC].height);
					if (!rectangle.Intersects(value4) || chest != -1 || !Main.npc[talkNPC].active)
					{
						if (chest == -1)
						{
							Main.PlaySound(11);
						}
						talkNPC = -1;
						Main.npcChatCornerItem = 0;
						Main.npcChatText = "";
					}
				}
				if (sign >= 0)
				{
					Rectangle value5 = new Rectangle((int)(base.position.X + (float)(width / 2) - (float)(tileRangeX * 16)), (int)(base.position.Y + (float)(height / 2) - (float)(tileRangeY * 16)), tileRangeX * 16 * 2, tileRangeY * 16 * 2);
					try
					{
						bool flag35 = false;
						if (Main.sign[sign] == null)
						{
							flag35 = true;
						}
						if (!flag35 && !new Rectangle(Main.sign[sign].x * 16, Main.sign[sign].y * 16, 32, 32).Intersects(value5))
						{
							flag35 = true;
						}
						if (flag35)
						{
							Main.PlaySound(11);
							sign = -1;
							Main.editSign = false;
							Main.npcChatText = "";
						}
					}
					catch
					{
						Main.PlaySound(11);
						sign = -1;
						Main.editSign = false;
						Main.npcChatText = "";
					}
				}
				if (Main.editSign)
				{
					if (sign == -1)
					{
						Main.editSign = false;
					}
					else
					{
						Main.npcChatText = Main.GetInputText(Main.npcChatText);
						if (Main.inputTextEnter)
						{
							byte[] bytes = new byte[1]
							{
								10
							};
							Main.npcChatText += Encoding.ASCII.GetString(bytes);
						}
						else if (Main.inputTextEscape)
						{
							Main.PlaySound(12);
							Main.editSign = false;
							Main.blockKey = Keys.Escape;
							Main.npcChatText = Main.sign[sign].text;
						}
					}
				}
				else if (Main.editChest)
				{
					string ınputText = Main.GetInputText(Main.npcChatText);
					if (Main.player[Main.myPlayer].chest == -1)
					{
						Main.editChest = false;
					}
					else if (Main.inputTextEnter)
					{
						Main.PlaySound(12);
						Main.editChest = false;
						int num236 = Main.player[Main.myPlayer].chest;
						if (Main.npcChatText == Main.defaultChestName)
						{
							Main.npcChatText = "";
						}
						if (Main.chest[num236].name != Main.npcChatText)
						{
							Main.chest[num236].name = Main.npcChatText;
							if (Main.netMode == 1)
							{
								editedChestName = true;
							}
						}
					}
					else if (Main.inputTextEscape)
					{
						Main.PlaySound(12);
						Main.editChest = false;
						Main.npcChatText = string.Empty;
						Main.blockKey = Keys.Escape;
					}
					else if (ınputText.Length <= 20)
					{
						Main.npcChatText = ınputText;
					}
				}
				if (mount.Active && mount.Cart && Math.Abs(base.velocity.X) > 4f)
				{
					Rectangle rectangle2 = new Rectangle((int)base.position.X, (int)base.position.Y, width, height);
					for (int num237 = 0; num237 < 200; num237++)
					{
						if (!Main.npc[num237].active || Main.npc[num237].friendly || Main.npc[num237].damage <= 0 || Main.npc[num237].immune[i] != 0 || !rectangle2.Intersects(new Rectangle((int)Main.npc[num237].position.X, (int)Main.npc[num237].position.Y, Main.npc[num237].width, Main.npc[num237].height)))
						{
							continue;
						}
						float num238 = meleeCrit;
						if (num238 < (float)rangedCrit)
						{
							num238 = rangedCrit;
						}
						if (num238 < (float)magicCrit)
						{
							num238 = magicCrit;
						}
						bool crit = false;
						if ((float)Main.rand.Next(1, 101) <= num238)
						{
							crit = true;
						}
						float num239 = Math.Abs(base.velocity.X) / maxRunSpeed;
						int num240 = Main.DamageVar(25f + 55f * num239);
						if (mount.Type == 11)
						{
							num240 = Main.DamageVar(50f + 100f * num239);
						}
						if (mount.Type == 13)
						{
							num240 = Main.DamageVar(15f + 30f * num239);
						}
						float num241 = 5f + 25f * num239;
						int num242 = 1;
						if (base.velocity.X < 0f)
						{
							num242 = -1;
						}
						if (whoAmI == Main.myPlayer)
						{
							Main.npc[num237].StrikeNPC(num240, num241, num242, crit);
							if (Main.netMode != 0)
							{
								NetMessage.SendData(28, -1, -1, "", num237, num240, num241, num242);
							}
						}
						Main.npc[num237].immune[i] = 30;
						if (!Main.npc[num237].active)
						{
							AchievementsHelper.HandleSpecialEvent(this, 9);
						}
					}
				}
				Rectangle rectangle3 = new Rectangle((int)base.position.X, (int)base.position.Y, width, height);
				for (int num243 = 0; num243 < 200; num243++)
				{
					if (!Main.npc[num243].active || Main.npc[num243].friendly || Main.npc[num243].damage <= 0)
					{
						continue;
					}
					int num244 = -1;
					int type5 = Main.npc[num243].type;
					if (type5 == 398 || type5 == 400 || type5 == 397 || type5 == 396 || type5 == 401)
					{
						num244 = 1;
					}
					if ((num244 == -1 && immune) || (dash == 2 && num243 == eocHit && eocDash > 0) || npcTypeNoAggro[Main.npc[num243].type])
					{
						continue;
					}
					float num245 = 1f;
					Rectangle value6 = new Rectangle((int)Main.npc[num243].position.X, (int)Main.npc[num243].position.Y, Main.npc[num243].width, Main.npc[num243].height);
					if (Main.npc[num243].type >= 430 && Main.npc[num243].type <= 436 && Main.npc[num243].ai[2] > 5f)
					{
						int num246 = 34;
						if (Main.npc[num243].spriteDirection < 0)
						{
							value6.X -= num246;
							value6.Width += num246;
						}
						else
						{
							value6.Width += num246;
						}
						num245 *= 1.25f;
					}
					else if (Main.npc[num243].type >= 494 && Main.npc[num243].type <= 495 && Main.npc[num243].ai[2] > 5f)
					{
						int num247 = 18;
						if (Main.npc[num243].spriteDirection < 0)
						{
							value6.X -= num247;
							value6.Width += num247;
						}
						else
						{
							value6.Width += num247;
						}
						num245 *= 1.25f;
					}
					else if (Main.npc[num243].type == 460)
					{
						Rectangle rectangle4 = new Rectangle(0, 0, 30, 14);
						rectangle4.X = (int)Main.npc[num243].Center.X;
						if (Main.npc[num243].direction < 0)
						{
							rectangle4.X -= rectangle4.Width;
						}
						rectangle4.Y = (int)Main.npc[num243].position.Y + Main.npc[num243].height - 20;
						if (rectangle3.Intersects(rectangle4))
						{
							value6 = rectangle4;
							num245 *= 1.35f;
						}
					}
					else if (Main.npc[num243].type == 417 && Main.npc[num243].ai[0] == 6f && Main.npc[num243].ai[3] > 0f && Main.npc[num243].ai[3] < 4f)
					{
						Rectangle rectangle5 = Utils.CenteredRectangle(Main.npc[num243].Center, new Vector2(100f));
						if (rectangle3.Intersects(rectangle5))
						{
							value6 = rectangle5;
							num245 *= 1.35f;
						}
					}
					else if (Main.npc[num243].type == 466)
					{
						Rectangle rectangle6 = new Rectangle(0, 0, 30, 8);
						rectangle6.X = (int)Main.npc[num243].Center.X;
						if (Main.npc[num243].direction < 0)
						{
							rectangle6.X -= rectangle6.Width;
						}
						rectangle6.Y = (int)Main.npc[num243].position.Y + Main.npc[num243].height - 32;
						if (rectangle3.Intersects(rectangle6))
						{
							value6 = rectangle6;
							num245 *= 1.75f;
						}
					}
					if (!rectangle3.Intersects(value6) || npcTypeNoAggro[Main.npc[num243].type])
					{
						continue;
					}
					int num248 = -1;
					if (Main.npc[num243].position.X + (float)(Main.npc[num243].width / 2) < base.position.X + (float)(width / 2))
					{
						num248 = 1;
					}
					int num249 = Main.DamageVar((float)Main.npc[num243].damage * num245);
					int num250 = Item.NPCtoBanner(Main.npc[num243].BannerID());
					if (num250 > 0 && NPCBannerBuff[num250])
					{
						num249 = ((!Main.expertMode) ? ((int)((double)num249 * 0.75)) : ((int)((double)num249 * 0.5)));
					}
					if (whoAmI == Main.myPlayer && thorns > 0f && !immune && !Main.npc[num243].dontTakeDamage)
					{
						int num251 = (int)((float)num249 * thorns);
						int num252 = 10;
						if (turtleThorns)
						{
							num251 = num249;
						}
						Main.npc[num243].StrikeNPC(num251, num252, -num248);
						if (Main.netMode != 0)
						{
							NetMessage.SendData(28, -1, -1, "", num243, num251, num252, -num248);
						}
					}
					if (resistCold && Main.npc[num243].coldDamage)
					{
						num249 = (int)((float)num249 * 0.7f);
					}
					if (!immune)
					{
						StatusPlayer(Main.npc[num243]);
					}
					Hurt(num249, num248, false, false, Lang.deathMsg(name, -1, num243), false, num244);
				}
				Vector2 vector6 = (mount.Active && mount.Cart) ? Collision.HurtTiles(base.position, base.velocity, width, height - 16, fireWalk) : Collision.HurtTiles(base.position, base.velocity, width, height, fireWalk);
				if (vector6.Y == 0f && !fireWalk)
				{
					foreach (Point touchedTile in TouchedTiles)
					{
						Tile tile5 = Main.tile[touchedTile.X, touchedTile.Y];
						if (tile5 != null && tile5.active() && tile5.nactive() && !fireWalk && TileID.Sets.TouchDamageHot[tile5.type] != 0)
						{
							vector6.Y = TileID.Sets.TouchDamageHot[tile5.type];
							vector6.X = ((!(base.Center.X / 16f < (float)touchedTile.X + 0.5f)) ? 1 : (-1));
							break;
						}
					}
				}
				if (vector6.Y == 20f)
				{
					AddBuff(67, 20);
				}
				else if (vector6.Y == 15f)
				{
					if (suffocateDelay < 5)
					{
						suffocateDelay++;
					}
					else
					{
						AddBuff(68, 1);
					}
				}
				else if (vector6.Y != 0f)
				{
					int damage3 = Main.DamageVar(vector6.Y);
					Hurt(damage3, 0, false, false, Lang.deathMsg(name, -1, -1, -1, 3), false, 0);
				}
				else
				{
					suffocateDelay = 0;
				}
			}
			if (controlRight)
			{
				releaseRight = false;
			}
			else
			{
				releaseRight = true;
				rightTimer = 7;
			}
			if (controlLeft)
			{
				releaseLeft = false;
			}
			else
			{
				releaseLeft = true;
				leftTimer = 7;
			}
			releaseDown = !controlDown;
			if (rightTimer > 0)
			{
				rightTimer--;
			}
			else if (controlRight)
			{
				rightTimer = 7;
			}
			if (leftTimer > 0)
			{
				leftTimer--;
			}
			else if (controlLeft)
			{
				leftTimer = 7;
			}
			GrappleMovement();
			StickyMovement();
			CheckDrowning();
			if (gravDir == -1f)
			{
				waterWalk = false;
				waterWalk2 = false;
			}
			int num253 = height;
			if (waterWalk)
			{
				num253 -= 6;
			}
			bool flag36 = Collision.LavaCollision(base.position, width, num253);
			if (flag36)
			{
				if (!lavaImmune && Main.myPlayer == i && !immune)
				{
					if (lavaTime > 0)
					{
						lavaTime--;
					}
					else if (lavaRose)
					{
						Hurt(50, 0, false, false, Lang.deathMsg(name, -1, -1, -1, 2));
						AddBuff(24, 210);
					}
					else
					{
						Hurt(80, 0, false, false, Lang.deathMsg(name, -1, -1, -1, 2));
						AddBuff(24, 420);
					}
				}
				lavaWet = true;
			}
			else
			{
				lavaWet = false;
				if (lavaTime < lavaMax)
				{
					lavaTime++;
				}
			}
			if (lavaTime > lavaMax)
			{
				lavaTime = lavaMax;
			}
			if (waterWalk2 && !waterWalk)
			{
				num253 -= 6;
			}
			bool flag37 = Collision.WetCollision(base.position, width, height);
			bool flag38 = Collision.honey;
			if (flag38)
			{
				AddBuff(48, 1800);
				honeyWet = true;
			}
			if (flag37)
			{
				if (onFire && !lavaWet)
				{
					for (int num254 = 0; num254 < 22; num254++)
					{
						if (buffType[num254] == 24)
						{
							DelBuff(num254);
						}
					}
				}
				if (!wet)
				{
					if (wetCount == 0)
					{
						wetCount = 10;
						if (!flag36)
						{
							if (honeyWet)
							{
								for (int num255 = 0; num255 < 20; num255++)
								{
									int num256 = Dust.NewDust(new Vector2(base.position.X - 6f, base.position.Y + (float)(height / 2) - 8f), width + 12, 24, 152);
									Main.dust[num256].velocity.Y -= 1f;
									Main.dust[num256].velocity.X *= 2.5f;
									Main.dust[num256].scale = 1.3f;
									Main.dust[num256].alpha = 100;
									Main.dust[num256].noGravity = true;
								}
								Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
							}
							else
							{
								for (int num257 = 0; num257 < 50; num257++)
								{
									int num258 = Dust.NewDust(new Vector2(base.position.X - 6f, base.position.Y + (float)(height / 2) - 8f), width + 12, 24, Dust.dustWater());
									Main.dust[num258].velocity.Y -= 3f;
									Main.dust[num258].velocity.X *= 2.5f;
									Main.dust[num258].scale = 0.8f;
									Main.dust[num258].alpha = 100;
									Main.dust[num258].noGravity = true;
								}
								Main.PlaySound(19, (int)base.position.X, (int)base.position.Y, 0);
							}
						}
						else
						{
							for (int num259 = 0; num259 < 20; num259++)
							{
								int num260 = Dust.NewDust(new Vector2(base.position.X - 6f, base.position.Y + (float)(height / 2) - 8f), width + 12, 24, 35);
								Main.dust[num260].velocity.Y -= 1.5f;
								Main.dust[num260].velocity.X *= 2.5f;
								Main.dust[num260].scale = 1.3f;
								Main.dust[num260].alpha = 100;
								Main.dust[num260].noGravity = true;
							}
							Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
						}
					}
					wet = true;
				}
			}
			else if (wet)
			{
				wet = false;
				if (jump > jumpHeight / 5 && wetSlime == 0)
				{
					jump = jumpHeight / 5;
				}
				if (wetCount == 0)
				{
					wetCount = 10;
					if (!lavaWet)
					{
						if (honeyWet)
						{
							for (int num261 = 0; num261 < 20; num261++)
							{
								int num262 = Dust.NewDust(new Vector2(base.position.X - 6f, base.position.Y + (float)(height / 2) - 8f), width + 12, 24, 152);
								Main.dust[num262].velocity.Y -= 1f;
								Main.dust[num262].velocity.X *= 2.5f;
								Main.dust[num262].scale = 1.3f;
								Main.dust[num262].alpha = 100;
								Main.dust[num262].noGravity = true;
							}
							Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
						}
						else
						{
							for (int num263 = 0; num263 < 50; num263++)
							{
								int num264 = Dust.NewDust(new Vector2(base.position.X - 6f, base.position.Y + (float)(height / 2)), width + 12, 24, Dust.dustWater());
								Main.dust[num264].velocity.Y -= 4f;
								Main.dust[num264].velocity.X *= 2.5f;
								Main.dust[num264].scale = 0.8f;
								Main.dust[num264].alpha = 100;
								Main.dust[num264].noGravity = true;
							}
							Main.PlaySound(19, (int)base.position.X, (int)base.position.Y, 0);
						}
					}
					else
					{
						for (int num265 = 0; num265 < 20; num265++)
						{
							int num266 = Dust.NewDust(new Vector2(base.position.X - 6f, base.position.Y + (float)(height / 2) - 8f), width + 12, 24, 35);
							Main.dust[num266].velocity.Y -= 1.5f;
							Main.dust[num266].velocity.X *= 2.5f;
							Main.dust[num266].scale = 1.3f;
							Main.dust[num266].alpha = 100;
							Main.dust[num266].noGravity = true;
						}
						Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
					}
				}
			}
			if (!flag38)
			{
				honeyWet = false;
			}
			if (!wet)
			{
				lavaWet = false;
				honeyWet = false;
			}
			if (wetCount > 0)
			{
				wetCount--;
			}
			if (wetSlime > 0)
			{
				wetSlime--;
			}
			if (wet && mount.Active)
			{
				switch (mount.Type)
				{
				case 5:
				case 7:
					if (whoAmI == Main.myPlayer)
					{
						mount.Dismount(this);
					}
					break;
				case 3:
					wetSlime = 30;
					if (base.velocity.Y > 2f)
					{
						base.velocity.Y *= 0.9f;
					}
					base.velocity.Y -= 0.5f;
					if (base.velocity.Y < -4f)
					{
						base.velocity.Y = -4f;
					}
					break;
				}
			}
			if (Main.expertMode && ZoneSnow && wet && !lavaWet && !honeyWet && !arcticDivingGear)
			{
				AddBuff(46, 150);
			}
			float num267 = 1f + Math.Abs(base.velocity.X) / 3f;
			if (gfxOffY > 0f)
			{
				gfxOffY -= num267 * stepSpeed;
				if (gfxOffY < 0f)
				{
					gfxOffY = 0f;
				}
			}
			else if (gfxOffY < 0f)
			{
				gfxOffY += num267 * stepSpeed;
				if (gfxOffY > 0f)
				{
					gfxOffY = 0f;
				}
			}
			if (gfxOffY > 32f)
			{
				gfxOffY = 32f;
			}
			if (gfxOffY < -32f)
			{
				gfxOffY = -32f;
			}
			if (Main.myPlayer == i && !iceSkate)
			{
				CheckIceBreak();
			}
			SlopeDownMovement();
			bool flag39 = mount.Type == 7 || mount.Type == 8 || mount.Type == 12;
			if (base.velocity.Y == gravity && (!mount.Active || (!mount.Cart && !flag39)))
			{
				Collision.StepDown(ref base.position, ref base.velocity, width, height, ref stepSpeed, ref gfxOffY, (int)gravDir, waterWalk || waterWalk2);
			}
			if (gravDir == -1f)
			{
				if ((carpetFrame != -1 || base.velocity.Y <= gravity) && !controlUp)
				{
					Collision.StepUp(ref base.position, ref base.velocity, width, height, ref stepSpeed, ref gfxOffY, (int)gravDir, controlUp);
				}
			}
			else if (flag39 || ((carpetFrame != -1 || base.velocity.Y >= gravity) && !controlDown && !mount.Cart))
			{
				Collision.StepUp(ref base.position, ref base.velocity, width, height, ref stepSpeed, ref gfxOffY, (int)gravDir, controlUp);
			}
			oldPosition = base.position;
			oldDirection = direction;
			bool falling = false;
			if (base.velocity.Y > gravity)
			{
				falling = true;
			}
			if (base.velocity.Y < 0f - gravity)
			{
				falling = true;
			}
			Vector2 velocity = base.velocity;
			slideDir = 0;
			bool ignorePlats = false;
			bool fallThrough = controlDown;
			if (gravDir == -1f || (mount.Active && mount.Cart) || GoingDownWithGrapple)
			{
				ignorePlats = true;
				fallThrough = true;
			}
			onTrack = false;
			bool flag40 = false;
			if (mount.Active && mount.Cart)
			{
				float num268 = (ignoreWater || merman) ? 1f : (honeyWet ? 0.25f : ((!wet) ? 1f : 0.5f));
				base.velocity *= num268;
				DelegateMethods.Minecart.rotation = fullRotation;
				DelegateMethods.Minecart.rotationOrigin = fullRotationOrigin;
				BitsByte bitsByte = Minecart.TrackCollision(ref base.position, ref base.velocity, ref lastBoost, width, height, controlDown, controlUp, fallStart2, false, mount.MinecartDust);
				if (bitsByte[0])
				{
					onTrack = true;
					gfxOffY = Minecart.TrackRotation(ref fullRotation, base.position + base.velocity, width, height, controlDown, controlUp, mount.MinecartDust);
					fullRotationOrigin = new Vector2(width / 2, height);
				}
				if (bitsByte[1])
				{
					if (controlLeft || controlRight)
					{
						if (cartFlip)
						{
							cartFlip = false;
						}
						else
						{
							cartFlip = true;
						}
					}
					if (base.velocity.X > 0f)
					{
						direction = 1;
					}
					else if (base.velocity.X < 0f)
					{
						direction = -1;
					}
					Main.PlaySound(2, (int)base.position.X + width / 2, (int)base.position.Y + height / 2, 56);
				}
				base.velocity /= num268;
				if (bitsByte[3] && whoAmI == Main.myPlayer)
				{
					flag40 = true;
				}
				if (bitsByte[2])
				{
					cartRampTime = (int)(Math.Abs(base.velocity.X) / mount.RunSpeed * 20f);
				}
				if (bitsByte[4])
				{
					trackBoost -= 4f;
				}
				if (bitsByte[5])
				{
					trackBoost += 4f;
				}
			}
			bool flag41 = whoAmI == Main.myPlayer && !mount.Active;
			Vector2 position = base.position;
			if (vortexDebuff)
			{
				base.velocity.Y = base.velocity.Y * 0.8f + (float)Math.Cos(base.Center.X % 120f / 120f * ((float)Math.PI * 2f)) * 5f * 0.2f;
			}
			if (tongued)
			{
				base.position += base.velocity;
				flag41 = false;
			}
			else if (honeyWet && !ignoreWater)
			{
				HoneyCollision(fallThrough, ignorePlats);
			}
			else if (wet && !merman && !ignoreWater)
			{
				WaterCollision(fallThrough, ignorePlats);
			}
			else
			{
				DryCollision(fallThrough, ignorePlats);
				if (mount.Active && mount.Type == 3 && base.velocity.Y != 0f && !SlimeDontHyperJump)
				{
					Vector2 velocity2 = base.velocity;
					base.velocity.X = 0f;
					DryCollision(fallThrough, ignorePlats);
					base.velocity.X = velocity2.X;
				}
			}
			UpdateTouchingTiles();
			TryBouncingBlocks(falling);
			TryLandingOnDetonator();
			SlopingCollision(fallThrough);
			if (flag41 && base.velocity.Y == 0f)
			{
				AchievementsHelper.HandleRunning(Math.Abs(base.position.X - position.X));
			}
			if (flag40)
			{
				NetMessage.SendData(13, -1, -1, "", whoAmI);
				Minecart.HitTrackSwitch(new Vector2(base.position.X, base.position.Y), width, height);
			}
			if (velocity.X != base.velocity.X)
			{
				if (velocity.X < 0f)
				{
					slideDir = -1;
				}
				else if (velocity.X > 0f)
				{
					slideDir = 1;
				}
			}
			if (gravDir == 1f && Collision.up)
			{
				base.velocity.Y = 0.01f;
				if (!merman)
				{
					jump = 0;
				}
			}
			else if (gravDir == -1f && Collision.down)
			{
				base.velocity.Y = -0.01f;
				if (!merman)
				{
					jump = 0;
				}
			}
			if (base.velocity.Y == 0f && grappling[0] == -1)
			{
				FloorVisuals(falling);
			}
			if (whoAmI == Main.myPlayer)
			{
				Collision.SwitchTiles(base.position, width, height, oldPosition, 1);
			}
			BordersMovement();
			numMinions = 0;
			slotsMinions = 0f;
			if (altFunctionUse == 0 && selectedItem != 58 && controlUseTile && releaseUseItem && !controlUseItem && !mouseInterface && !CaptureManager.Instance.Active && inventory[selectedItem].type == 3384)
			{
				altFunctionUse = 1;
				controlUseItem = true;
			}
			if (!controlUseItem && altFunctionUse == 1)
			{
				altFunctionUse = 0;
			}
			if (Main.ignoreErrors)
			{
				try
				{
					ItemCheck(i);
				}
				catch
				{
				}
			}
			else
			{
				ItemCheck(i);
			}
			PlayerFrame();
			if (mount.Type == 8)
			{
				mount.UseDrill(this);
			}
			if (statLife > statLifeMax2)
			{
				statLife = statLifeMax2;
			}
			if (statMana > statManaMax2)
			{
				statMana = statManaMax2;
			}
			grappling[0] = -1;
			grapCount = 0;
		}

		private void TryLandingOnDetonator()
		{
			if (whoAmI == Main.myPlayer && velocity.Y >= 3f)
			{
				Point point = (base.Bottom + new Vector2(0f, 0.01f)).ToTileCoordinates();
				Tile tileSafely = Framing.GetTileSafely(point.X, point.Y);
				if (tileSafely.active() && tileSafely.type == 411 && tileSafely.frameY == 0 && tileSafely.frameX < 36)
				{
					Wiring.HitSwitch(point.X, point.Y);
					NetMessage.SendData(59, -1, -1, "", point.X, point.Y);
				}
			}
		}

		private void TryBouncingBlocks(bool Falling)
		{
			if ((!(velocity.Y >= 5f) && !(velocity.Y <= -5f)) || wet)
			{
				return;
			}
			int num = 0;
			bool flag = false;
			foreach (Point touchedTile in TouchedTiles)
			{
				Tile tile = Main.tile[touchedTile.X, touchedTile.Y];
				if (tile != null && tile.active() && tile.nactive() && Main.tileBouncy[tile.type])
				{
					flag = true;
					num = touchedTile.Y;
					break;
				}
			}
			if (flag)
			{
				velocity.Y *= -0.8f;
				if (controlJump)
				{
					velocity.Y = MathHelper.Clamp(velocity.Y, -13f, 13f);
				}
				position.Y = num * 16 - ((velocity.Y < 0f) ? height : (-16));
				FloorVisuals(Falling);
				velocity.Y = MathHelper.Clamp(velocity.Y, -20f, 20f);
				if (velocity.Y * gravDir < 0f)
				{
					fallStart = (int)position.Y / 16;
				}
			}
		}

		private void GrabItems(int i)
		{
			for (int j = 0; j < 400; j++)
			{
				if (!Main.item[j].active || Main.item[j].noGrabDelay != 0 || Main.item[j].owner != i)
				{
					continue;
				}
				int num = defaultItemGrabRange;
				if (goldRing && Main.item[j].type >= 71 && Main.item[j].type <= 74)
				{
					num += Item.coinGrabRange;
				}
				if (manaMagnet && (Main.item[j].type == 184 || Main.item[j].type == 1735 || Main.item[j].type == 1868))
				{
					num += Item.manaGrabRange;
				}
				if (lifeMagnet && (Main.item[j].type == 58 || Main.item[j].type == 1734 || Main.item[j].type == 1867))
				{
					num += Item.lifeGrabRange;
				}
				if (ItemID.Sets.NebulaPickup[Main.item[j].type])
				{
					num += 100;
				}
				if (new Rectangle((int)position.X, (int)position.Y, width, height).Intersects(new Rectangle((int)Main.item[j].position.X, (int)Main.item[j].position.Y, Main.item[j].width, Main.item[j].height)))
				{
					if (i != Main.myPlayer || (inventory[selectedItem].type == 0 && itemAnimation > 0))
					{
						continue;
					}
					if (ItemID.Sets.NebulaPickup[Main.item[j].type])
					{
						Main.PlaySound(7, (int)position.X, (int)position.Y);
						int num2 = Main.item[j].buffType;
						Main.item[j] = new Item();
						if (Main.netMode == 1)
						{
							NetMessage.SendData(102, -1, -1, "", i, num2, base.Center.X, base.Center.Y);
							NetMessage.SendData(21, -1, -1, "", j);
						}
						else
						{
							NebulaLevelup(num2);
						}
					}
					if (Main.item[j].type == 58 || Main.item[j].type == 1734 || Main.item[j].type == 1867)
					{
						Main.PlaySound(7, (int)position.X, (int)position.Y);
						statLife += 20;
						if (Main.myPlayer == whoAmI)
						{
							HealEffect(20);
						}
						if (statLife > statLifeMax2)
						{
							statLife = statLifeMax2;
						}
						Main.item[j] = new Item();
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", j);
						}
					}
					else if (Main.item[j].type == 184 || Main.item[j].type == 1735 || Main.item[j].type == 1868)
					{
						Main.PlaySound(7, (int)position.X, (int)position.Y);
						statMana += 100;
						if (Main.myPlayer == whoAmI)
						{
							ManaEffect(100);
						}
						if (statMana > statManaMax2)
						{
							statMana = statManaMax2;
						}
						Main.item[j] = new Item();
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", j);
						}
					}
					else
					{
						Main.item[j] = GetItem(i, Main.item[j]);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", j);
						}
					}
				}
				else
				{
					if (!new Rectangle((int)position.X - num, (int)position.Y - num, width + num * 2, height + num * 2).Intersects(new Rectangle((int)Main.item[j].position.X, (int)Main.item[j].position.Y, Main.item[j].width, Main.item[j].height)) || !ItemSpace(Main.item[j]))
					{
						continue;
					}
					Main.item[j].beingGrabbed = true;
					if (manaMagnet && (Main.item[j].type == 184 || Main.item[j].type == 1735 || Main.item[j].type == 1868))
					{
						float num3 = 12f;
						Vector2 vector = new Vector2(Main.item[j].position.X + (float)(Main.item[j].width / 2), Main.item[j].position.Y + (float)(Main.item[j].height / 2));
						float num4 = base.Center.X - vector.X;
						float num5 = base.Center.Y - vector.Y;
						float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
						num6 = num3 / num6;
						num4 *= num6;
						num5 *= num6;
						int num7 = 5;
						Main.item[j].velocity.X = (Main.item[j].velocity.X * (float)(num7 - 1) + num4) / (float)num7;
						Main.item[j].velocity.Y = (Main.item[j].velocity.Y * (float)(num7 - 1) + num5) / (float)num7;
						continue;
					}
					if (lifeMagnet && (Main.item[j].type == 58 || Main.item[j].type == 1734 || Main.item[j].type == 1867))
					{
						float num8 = 15f;
						Vector2 vector2 = new Vector2(Main.item[j].position.X + (float)(Main.item[j].width / 2), Main.item[j].position.Y + (float)(Main.item[j].height / 2));
						float num9 = base.Center.X - vector2.X;
						float num10 = base.Center.Y - vector2.Y;
						float num11 = (float)Math.Sqrt(num9 * num9 + num10 * num10);
						num11 = num8 / num11;
						num9 *= num11;
						num10 *= num11;
						int num12 = 5;
						Main.item[j].velocity.X = (Main.item[j].velocity.X * (float)(num12 - 1) + num9) / (float)num12;
						Main.item[j].velocity.Y = (Main.item[j].velocity.Y * (float)(num12 - 1) + num10) / (float)num12;
						continue;
					}
					if (goldRing && Main.item[j].type >= 71 && Main.item[j].type <= 74)
					{
						float num13 = 12f;
						Vector2 vector3 = new Vector2(Main.item[j].position.X + (float)(Main.item[j].width / 2), Main.item[j].position.Y + (float)(Main.item[j].height / 2));
						float num14 = base.Center.X - vector3.X;
						float num15 = base.Center.Y - vector3.Y;
						float num16 = (float)Math.Sqrt(num14 * num14 + num15 * num15);
						num16 = num13 / num16;
						num14 *= num16;
						num15 *= num16;
						int num17 = 5;
						Main.item[j].velocity.X = (Main.item[j].velocity.X * (float)(num17 - 1) + num14) / (float)num17;
						Main.item[j].velocity.Y = (Main.item[j].velocity.Y * (float)(num17 - 1) + num15) / (float)num17;
						continue;
					}
					if (ItemID.Sets.NebulaPickup[Main.item[j].type])
					{
						float num18 = 12f;
						Vector2 vector4 = new Vector2(Main.item[j].position.X + (float)(Main.item[j].width / 2), Main.item[j].position.Y + (float)(Main.item[j].height / 2));
						float num19 = base.Center.X - vector4.X;
						float num20 = base.Center.Y - vector4.Y;
						float num21 = (float)Math.Sqrt(num19 * num19 + num20 * num20);
						num21 = num18 / num21;
						num19 *= num21;
						num20 *= num21;
						int num22 = 5;
						Main.item[j].velocity.X = (Main.item[j].velocity.X * (float)(num22 - 1) + num19) / (float)num22;
						Main.item[j].velocity.Y = (Main.item[j].velocity.Y * (float)(num22 - 1) + num20) / (float)num22;
						continue;
					}
					if ((double)position.X + (double)width * 0.5 > (double)Main.item[j].position.X + (double)Main.item[j].width * 0.5)
					{
						if (Main.item[j].velocity.X < itemGrabSpeedMax + velocity.X)
						{
							Main.item[j].velocity.X += itemGrabSpeed;
						}
						if (Main.item[j].velocity.X < 0f)
						{
							Main.item[j].velocity.X += itemGrabSpeed * 0.75f;
						}
					}
					else
					{
						if (Main.item[j].velocity.X > 0f - itemGrabSpeedMax + velocity.X)
						{
							Main.item[j].velocity.X -= itemGrabSpeed;
						}
						if (Main.item[j].velocity.X > 0f)
						{
							Main.item[j].velocity.X -= itemGrabSpeed * 0.75f;
						}
					}
					if ((double)position.Y + (double)height * 0.5 > (double)Main.item[j].position.Y + (double)Main.item[j].height * 0.5)
					{
						if (Main.item[j].velocity.Y < itemGrabSpeedMax)
						{
							Main.item[j].velocity.Y += itemGrabSpeed;
						}
						if (Main.item[j].velocity.Y < 0f)
						{
							Main.item[j].velocity.Y += itemGrabSpeed * 0.75f;
						}
					}
					else
					{
						if (Main.item[j].velocity.Y > 0f - itemGrabSpeedMax)
						{
							Main.item[j].velocity.Y -= itemGrabSpeed;
						}
						if (Main.item[j].velocity.Y > 0f)
						{
							Main.item[j].velocity.Y -= itemGrabSpeed * 0.75f;
						}
					}
				}
			}
		}

		public bool SellItem(int price, int stack)
		{
			if (price <= 0)
			{
				return false;
			}
			Item[] array = new Item[58];
			for (int i = 0; i < 58; i++)
			{
				array[i] = new Item();
				array[i] = inventory[i].Clone();
			}
			int num = price / 5;
			num *= stack;
			if (num < 1)
			{
				num = 1;
			}
			bool flag = false;
			while (num >= 1000000 && !flag)
			{
				int num2 = -1;
				for (int num3 = 53; num3 >= 0; num3--)
				{
					if (num2 == -1 && (inventory[num3].type == 0 || inventory[num3].stack == 0))
					{
						num2 = num3;
					}
					while (inventory[num3].type == 74 && inventory[num3].stack < inventory[num3].maxStack && num >= 1000000)
					{
						inventory[num3].stack++;
						num -= 1000000;
						DoCoins(num3);
						if (inventory[num3].stack == 0 && num2 == -1)
						{
							num2 = num3;
						}
					}
				}
				if (num >= 1000000)
				{
					if (num2 == -1)
					{
						flag = true;
						continue;
					}
					inventory[num2].SetDefaults(74);
					num -= 1000000;
				}
			}
			while (num >= 10000 && !flag)
			{
				int num4 = -1;
				for (int num5 = 53; num5 >= 0; num5--)
				{
					if (num4 == -1 && (inventory[num5].type == 0 || inventory[num5].stack == 0))
					{
						num4 = num5;
					}
					while (inventory[num5].type == 73 && inventory[num5].stack < inventory[num5].maxStack && num >= 10000)
					{
						inventory[num5].stack++;
						num -= 10000;
						DoCoins(num5);
						if (inventory[num5].stack == 0 && num4 == -1)
						{
							num4 = num5;
						}
					}
				}
				if (num >= 10000)
				{
					if (num4 == -1)
					{
						flag = true;
						continue;
					}
					inventory[num4].SetDefaults(73);
					num -= 10000;
				}
			}
			while (num >= 100 && !flag)
			{
				int num6 = -1;
				for (int num7 = 53; num7 >= 0; num7--)
				{
					if (num6 == -1 && (inventory[num7].type == 0 || inventory[num7].stack == 0))
					{
						num6 = num7;
					}
					while (inventory[num7].type == 72 && inventory[num7].stack < inventory[num7].maxStack && num >= 100)
					{
						inventory[num7].stack++;
						num -= 100;
						DoCoins(num7);
						if (inventory[num7].stack == 0 && num6 == -1)
						{
							num6 = num7;
						}
					}
				}
				if (num >= 100)
				{
					if (num6 == -1)
					{
						flag = true;
						continue;
					}
					inventory[num6].SetDefaults(72);
					num -= 100;
				}
			}
			while (num >= 1 && !flag)
			{
				int num8 = -1;
				for (int num9 = 53; num9 >= 0; num9--)
				{
					if (num8 == -1 && (inventory[num9].type == 0 || inventory[num9].stack == 0))
					{
						num8 = num9;
					}
					while (inventory[num9].type == 71 && inventory[num9].stack < inventory[num9].maxStack && num >= 1)
					{
						inventory[num9].stack++;
						num--;
						DoCoins(num9);
						if (inventory[num9].stack == 0 && num8 == -1)
						{
							num8 = num9;
						}
					}
				}
				if (num >= 1)
				{
					if (num8 == -1)
					{
						flag = true;
						continue;
					}
					inventory[num8].SetDefaults(71);
					num--;
				}
			}
			if (flag)
			{
				for (int j = 0; j < 58; j++)
				{
					inventory[j] = array[j].Clone();
				}
				return false;
			}
			return true;
		}

		public bool BuyItem(int price)
		{
			bool overFlowing;
			long num = Utils.CoinsCount(out overFlowing, inventory, 58, 57, 56, 55, 54);
			long num2 = Utils.CoinsCount(out overFlowing, bank.item);
			long num3 = Utils.CoinsCount(out overFlowing, bank2.item);
			long num4 = Utils.CoinsCombineStacks(out overFlowing, num, num2, num3);
			if (num4 < price)
			{
				return false;
			}
			List<Item[]> list = new List<Item[]>();
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			List<Point> list2 = new List<Point>();
			List<Point> list3 = new List<Point>();
			List<Point> list4 = new List<Point>();
			List<Point> list5 = new List<Point>();
			list.Add(inventory);
			list.Add(bank.item);
			list.Add(bank2.item);
			for (int i = 0; i < list.Count; i++)
			{
				dictionary[i] = new List<int>();
			}
			dictionary[0] = new List<int>
			{
				58,
				57,
				56,
				55,
				54
			};
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = 0; k < list[j].Length; k++)
				{
					if (!dictionary[j].Contains(k) && list[j][k].type >= 71 && list[j][k].type <= 74)
					{
						list3.Add(new Point(j, k));
					}
				}
			}
			int num5 = 0;
			for (int num6 = list[num5].Length - 1; num6 >= 0; num6--)
			{
				if (!dictionary[num5].Contains(num6) && (list[num5][num6].type == 0 || list[num5][num6].stack == 0))
				{
					list2.Add(new Point(num5, num6));
				}
			}
			num5 = 1;
			for (int num7 = list[num5].Length - 1; num7 >= 0; num7--)
			{
				if (!dictionary[num5].Contains(num7) && (list[num5][num7].type == 0 || list[num5][num7].stack == 0))
				{
					list4.Add(new Point(num5, num7));
				}
			}
			num5 = 2;
			for (int num8 = list[num5].Length - 1; num8 >= 0; num8--)
			{
				if (!dictionary[num5].Contains(num8) && (list[num5][num8].type == 0 || list[num5][num8].stack == 0))
				{
					list5.Add(new Point(num5, num8));
				}
			}
			long num9 = price;
			Dictionary<Point, Item> dictionary2 = new Dictionary<Point, Item>();
			while (num9 > 0)
			{
				long num10 = 1000000L;
				for (int l = 0; l < 4; l++)
				{
					if (num9 >= num10)
					{
						foreach (Point item2 in list3)
						{
							if (list[item2.X][item2.Y].type == 74 - l)
							{
								long num11 = num9 / num10;
								dictionary2[item2] = list[item2.X][item2.Y].Clone();
								if (num11 < list[item2.X][item2.Y].stack)
								{
									list[item2.X][item2.Y].stack -= (int)num11;
								}
								else
								{
									list[item2.X][item2.Y].SetDefaults();
									list2.Add(item2);
								}
								num9 -= num10 * (dictionary2[item2].stack - list[item2.X][item2.Y].stack);
							}
						}
					}
					num10 /= 100;
				}
				if (num9 <= 0)
				{
					continue;
				}
				if (list2.Count > 0)
				{
					list2.Sort(DelegateMethods.CompareYReverse);
					Point item = new Point(-1, -1);
					for (int m = 0; m < list.Count; m++)
					{
						num10 = 10000L;
						for (int n = 0; n < 3; n++)
						{
							if (num9 >= num10)
							{
								foreach (Point item3 in list3)
								{
									if (item3.X == m && list[item3.X][item3.Y].type == 74 - n && list[item3.X][item3.Y].stack >= 1)
									{
										List<Point> list6 = list2;
										if (m == 1 && list4.Count > 0)
										{
											list6 = list4;
										}
										if (m == 2 && list5.Count > 0)
										{
											list6 = list5;
										}
										if (--list[item3.X][item3.Y].stack <= 0)
										{
											list[item3.X][item3.Y].SetDefaults();
											list6.Add(item3);
										}
										dictionary2[list6[0]] = list[list6[0].X][list6[0].Y].Clone();
										list[list6[0].X][list6[0].Y].SetDefaults(73 - n);
										list[list6[0].X][list6[0].Y].stack = 100;
										item = list6[0];
										list6.RemoveAt(0);
										break;
									}
								}
							}
							if (item.X != -1 || item.Y != -1)
							{
								break;
							}
							num10 /= 100;
						}
						for (int num12 = 0; num12 < 2; num12++)
						{
							if (item.X == -1 && item.Y == -1)
							{
								foreach (Point item4 in list3)
								{
									if (item4.X == m && list[item4.X][item4.Y].type == 73 + num12 && list[item4.X][item4.Y].stack >= 1)
									{
										List<Point> list7 = list2;
										if (m == 1 && list4.Count > 0)
										{
											list7 = list4;
										}
										if (m == 2 && list5.Count > 0)
										{
											list7 = list5;
										}
										if (--list[item4.X][item4.Y].stack <= 0)
										{
											list[item4.X][item4.Y].SetDefaults();
											list7.Add(item4);
										}
										dictionary2[list7[0]] = list[list7[0].X][list7[0].Y].Clone();
										list[list7[0].X][list7[0].Y].SetDefaults(72 + num12);
										list[list7[0].X][list7[0].Y].stack = 100;
										item = list7[0];
										list7.RemoveAt(0);
										break;
									}
								}
							}
						}
						if (item.X != -1 && item.Y != -1)
						{
							list3.Add(item);
							break;
						}
					}
					list2.Sort(DelegateMethods.CompareYReverse);
					list4.Sort(DelegateMethods.CompareYReverse);
					list5.Sort(DelegateMethods.CompareYReverse);
					continue;
				}
				foreach (KeyValuePair<Point, Item> item5 in dictionary2)
				{
					list[item5.Key.X][item5.Key.Y] = item5.Value.Clone();
				}
				return false;
			}
			return true;
		}

		public bool BuyItemOld(int price)
		{
			if (price == 0)
			{
				return true;
			}
			long num = 0L;
			int num2 = price;
			Item[] array = new Item[54];
			for (int i = 0; i < 54; i++)
			{
				array[i] = new Item();
				array[i] = inventory[i].Clone();
				if (inventory[i].type == 71)
				{
					num += inventory[i].stack;
				}
				if (inventory[i].type == 72)
				{
					num += inventory[i].stack * 100;
				}
				if (inventory[i].type == 73)
				{
					num += inventory[i].stack * 10000;
				}
				if (inventory[i].type == 74)
				{
					num += inventory[i].stack * 1000000;
				}
			}
			if (num >= price)
			{
				num2 = price;
				while (num2 > 0)
				{
					if (num2 >= 1000000)
					{
						for (int j = 0; j < 54; j++)
						{
							if (inventory[j].type != 74)
							{
								continue;
							}
							while (inventory[j].stack > 0 && num2 >= 1000000)
							{
								num2 -= 1000000;
								inventory[j].stack--;
								if (inventory[j].stack == 0)
								{
									inventory[j].type = 0;
								}
							}
						}
					}
					if (num2 >= 10000)
					{
						for (int k = 0; k < 54; k++)
						{
							if (inventory[k].type != 73)
							{
								continue;
							}
							while (inventory[k].stack > 0 && num2 >= 10000)
							{
								num2 -= 10000;
								inventory[k].stack--;
								if (inventory[k].stack == 0)
								{
									inventory[k].type = 0;
								}
							}
						}
					}
					if (num2 >= 100)
					{
						for (int l = 0; l < 54; l++)
						{
							if (inventory[l].type != 72)
							{
								continue;
							}
							while (inventory[l].stack > 0 && num2 >= 100)
							{
								num2 -= 100;
								inventory[l].stack--;
								if (inventory[l].stack == 0)
								{
									inventory[l].type = 0;
								}
							}
						}
					}
					if (num2 >= 1)
					{
						for (int m = 0; m < 54; m++)
						{
							if (inventory[m].type != 71)
							{
								continue;
							}
							while (inventory[m].stack > 0 && num2 >= 1)
							{
								num2--;
								inventory[m].stack--;
								if (inventory[m].stack == 0)
								{
									inventory[m].type = 0;
								}
							}
						}
					}
					if (num2 <= 0)
					{
						continue;
					}
					int num3 = -1;
					for (int num4 = 53; num4 >= 0; num4--)
					{
						if (inventory[num4].type == 0 || inventory[num4].stack == 0)
						{
							num3 = num4;
							break;
						}
					}
					if (num3 >= 0)
					{
						bool flag = true;
						if (num2 >= 10000)
						{
							for (int n = 0; n < 58; n++)
							{
								if (inventory[n].type == 74 && inventory[n].stack >= 1)
								{
									inventory[n].stack--;
									if (inventory[n].stack == 0)
									{
										inventory[n].type = 0;
									}
									inventory[num3].SetDefaults(73);
									inventory[num3].stack = 100;
									flag = false;
									break;
								}
							}
						}
						else if (num2 >= 100)
						{
							for (int num5 = 0; num5 < 54; num5++)
							{
								if (inventory[num5].type == 73 && inventory[num5].stack >= 1)
								{
									inventory[num5].stack--;
									if (inventory[num5].stack == 0)
									{
										inventory[num5].type = 0;
									}
									inventory[num3].SetDefaults(72);
									inventory[num3].stack = 100;
									flag = false;
									break;
								}
							}
						}
						else if (num2 >= 1)
						{
							for (int num6 = 0; num6 < 54; num6++)
							{
								if (inventory[num6].type == 72 && inventory[num6].stack >= 1)
								{
									inventory[num6].stack--;
									if (inventory[num6].stack == 0)
									{
										inventory[num6].type = 0;
									}
									inventory[num3].SetDefaults(71);
									inventory[num3].stack = 100;
									flag = false;
									break;
								}
							}
						}
						if (!flag)
						{
							continue;
						}
						if (num2 < 10000)
						{
							for (int num7 = 0; num7 < 54; num7++)
							{
								if (inventory[num7].type == 73 && inventory[num7].stack >= 1)
								{
									inventory[num7].stack--;
									if (inventory[num7].stack == 0)
									{
										inventory[num7].type = 0;
									}
									inventory[num3].SetDefaults(72);
									inventory[num3].stack = 100;
									flag = false;
									break;
								}
							}
						}
						if (!flag || num2 >= 1000000)
						{
							continue;
						}
						for (int num8 = 0; num8 < 54; num8++)
						{
							if (inventory[num8].type == 74 && inventory[num8].stack >= 1)
							{
								inventory[num8].stack--;
								if (inventory[num8].stack == 0)
								{
									inventory[num8].type = 0;
								}
								inventory[num3].SetDefaults(73);
								inventory[num3].stack = 100;
								flag = false;
								break;
							}
						}
						continue;
					}
					for (int num9 = 0; num9 < 54; num9++)
					{
						inventory[num9] = array[num9].Clone();
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public void AdjTiles()
		{
			int num = 4;
			int num2 = 3;
			for (int i = 0; i < 419; i++)
			{
				oldAdjTile[i] = adjTile[i];
				adjTile[i] = false;
			}
			oldAdjWater = adjWater;
			adjWater = false;
			oldAdjHoney = adjHoney;
			adjHoney = false;
			oldAdjLava = adjLava;
			adjLava = false;
			alchemyTable = false;
			int num3 = (int)((position.X + (float)(width / 2)) / 16f);
			int num4 = (int)((position.Y + (float)height) / 16f);
			for (int j = num3 - num; j <= num3 + num; j++)
			{
				for (int k = num4 - num2; k < num4 + num2; k++)
				{
					if (Main.tile[j, k].active())
					{
						adjTile[Main.tile[j, k].type] = true;
						if (Main.tile[j, k].type == 302)
						{
							adjTile[17] = true;
						}
						if (Main.tile[j, k].type == 77)
						{
							adjTile[17] = true;
						}
						if (Main.tile[j, k].type == 133)
						{
							adjTile[17] = true;
							adjTile[77] = true;
						}
						if (Main.tile[j, k].type == 134)
						{
							adjTile[16] = true;
						}
						if (Main.tile[j, k].type == 354)
						{
							adjTile[14] = true;
						}
						if (Main.tile[j, k].type == 355)
						{
							adjTile[13] = true;
							adjTile[14] = true;
							alchemyTable = true;
						}
					}
					if (Main.tile[j, k].liquid > 200 && Main.tile[j, k].liquidType() == 0)
					{
						adjWater = true;
					}
					if (Main.tile[j, k].liquid > 200 && Main.tile[j, k].liquidType() == 2)
					{
						adjHoney = true;
					}
					if (Main.tile[j, k].liquid > 200 && Main.tile[j, k].liquidType() == 1)
					{
						adjLava = true;
					}
				}
			}
			if (!Main.playerInventory)
			{
				return;
			}
			bool flag = false;
			for (int l = 0; l < 419; l++)
			{
				if (oldAdjTile[l] != adjTile[l])
				{
					flag = true;
					break;
				}
			}
			if (adjWater != oldAdjWater)
			{
				flag = true;
			}
			if (adjHoney != oldAdjHoney)
			{
				flag = true;
			}
			if (adjLava != oldAdjLava)
			{
				flag = true;
			}
			if (flag)
			{
				Recipe.FindRecipes();
			}
		}

		public void PlayerFrame()
		{
			if (swimTime > 0)
			{
				swimTime--;
				if (!wet)
				{
					swimTime = 0;
				}
			}
			head = armor[0].headSlot;
			body = armor[1].bodySlot;
			legs = armor[2].legSlot;
			for (int i = 3; i < 8 + extraAccessorySlots; i++)
			{
				if (armor[i].shieldSlot == 5 && eocDash > 0)
				{
					shield = armor[i].shieldSlot;
				}
				if ((shield > 0 && armor[i].frontSlot >= 1 && armor[i].frontSlot <= 4) || (front >= 1 && front <= 4 && armor[i].shieldSlot > 0))
				{
					continue;
				}
				if (armor[i].wingSlot > 0)
				{
					if (hideVisual[i] && (velocity.Y == 0f || mount.Active))
					{
						continue;
					}
					wings = armor[i].wingSlot;
				}
				if (!hideVisual[i])
				{
					if (armor[i].stringColor > 0)
					{
						stringColor = armor[i].stringColor;
					}
					if (armor[i].handOnSlot > 0)
					{
						handon = armor[i].handOnSlot;
					}
					if (armor[i].handOffSlot > 0)
					{
						handoff = armor[i].handOffSlot;
					}
					if (armor[i].backSlot > 0)
					{
						back = armor[i].backSlot;
						front = -1;
					}
					if (armor[i].frontSlot > 0)
					{
						front = armor[i].frontSlot;
					}
					if (armor[i].shoeSlot > 0)
					{
						shoe = armor[i].shoeSlot;
					}
					if (armor[i].waistSlot > 0)
					{
						waist = armor[i].waistSlot;
					}
					if (armor[i].shieldSlot > 0)
					{
						shield = armor[i].shieldSlot;
					}
					if (armor[i].neckSlot > 0)
					{
						neck = armor[i].neckSlot;
					}
					if (armor[i].faceSlot > 0)
					{
						face = armor[i].faceSlot;
					}
					if (armor[i].balloonSlot > 0)
					{
						balloon = armor[i].balloonSlot;
					}
					if (armor[i].type == 3580)
					{
						yoraiz0rEye = i - 2;
					}
					if (armor[i].type == 3581)
					{
						yoraiz0rDarkness = true;
					}
				}
			}
			for (int j = 13; j < 18 + extraAccessorySlots; j++)
			{
				if (armor[j].stringColor > 0)
				{
					stringColor = armor[j].stringColor;
				}
				if (armor[j].handOnSlot > 0)
				{
					handon = armor[j].handOnSlot;
				}
				if (armor[j].handOffSlot > 0)
				{
					handoff = armor[j].handOffSlot;
				}
				if (armor[j].backSlot > 0)
				{
					back = armor[j].backSlot;
					front = -1;
				}
				if (armor[j].frontSlot > 0)
				{
					front = armor[j].frontSlot;
				}
				if (armor[j].shoeSlot > 0)
				{
					shoe = armor[j].shoeSlot;
				}
				if (armor[j].waistSlot > 0)
				{
					waist = armor[j].waistSlot;
				}
				if (armor[j].shieldSlot > 0)
				{
					shield = armor[j].shieldSlot;
				}
				if (armor[j].neckSlot > 0)
				{
					neck = armor[j].neckSlot;
				}
				if (armor[j].faceSlot > 0)
				{
					face = armor[j].faceSlot;
				}
				if (armor[j].balloonSlot > 0)
				{
					balloon = armor[j].balloonSlot;
				}
				if (armor[j].wingSlot > 0)
				{
					wings = armor[j].wingSlot;
				}
				if (armor[j].type == 3580)
				{
					yoraiz0rEye = j - 2;
				}
				if (armor[j].type == 3581)
				{
					yoraiz0rDarkness = true;
				}
			}
			if (armor[10].headSlot >= 0)
			{
				head = armor[10].headSlot;
			}
			if (armor[11].bodySlot >= 0)
			{
				body = armor[11].bodySlot;
			}
			if (armor[12].legSlot >= 0)
			{
				legs = armor[12].legSlot;
			}
			wearsRobe = false;
			int num = SetMatch(1, body, Male, ref wearsRobe);
			if (num != -1)
			{
				legs = num;
			}
			bool somethingSpecial = false;
			num = SetMatch(2, legs, Male, ref somethingSpecial);
			if (num != -1)
			{
				legs = num;
			}
			if (body == 93)
			{
				shield = 0;
				handoff = 0;
			}
			if (legs == 67)
			{
				shoe = 0;
			}
			if ((wereWolf || forceWerewolf) && !hideWolf)
			{
				legs = 20;
				body = 21;
				head = 38;
			}
			bool flag = wet && !lavaWet && (!mount.Active || mount.Type != 3);
			if (merman || forceMerman)
			{
				if (!hideMerman)
				{
					head = 39;
					legs = 21;
					body = 22;
				}
				if (flag)
				{
					wings = 0;
				}
			}
			socialShadow = false;
			socialGhost = false;
			if (head == 101 && body == 66 && legs == 55)
			{
				socialGhost = true;
			}
			if (head == 156 && body == 66 && legs == 55)
			{
				socialGhost = true;
			}
			if (head == 99 && body == 65 && legs == 54)
			{
				turtleArmor = true;
			}
			if (head == 162 && body == 170 && legs == 105)
			{
				spiderArmor = true;
			}
			if ((head == 75 || head == 7) && body == 7 && legs == 7)
			{
				boneArmor = true;
			}
			if (wings > 0)
			{
				back = -1;
				front = -1;
			}
			if (head > 0 && face != 7)
			{
				face = -1;
			}
			if (webbed || frozen || stoned || (Main.gamePaused && !Main.gameMenu))
			{
				return;
			}
			if (((body == 68 && legs == 57 && head == 106) || (body == 74 && legs == 63 && head == 106)) && Main.rand.Next(10) == 0)
			{
				int num2 = Dust.NewDust(new Vector2(position.X - velocity.X * 2f, position.Y - 2f - velocity.Y * 2f), width, height, 43, 0f, 0f, 100, new Color(255, 0, 255), 0.3f);
				Main.dust[num2].fadeIn = 0.8f;
				Main.dust[num2].noGravity = true;
				Main.dust[num2].velocity *= 2f;
				Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
			}
			if (wings == 27)
			{
				float num3 = 0.4f * stealth;
				Lighting.AddLight((int)base.Center.X / 16, (int)base.Center.Y / 16, num3, num3 * 0.9f, num3 * 0.2f);
			}
			if (head == 5 && body == 5 && legs == 5)
			{
				socialShadow = true;
			}
			if (head == 5 && body == 5 && legs == 5 && Main.rand.Next(10) == 0)
			{
				int num4 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 14, 0f, 0f, 200, default(Color), 1.2f);
				Main.dust[num4].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
			}
			if (head == 76 && body == 49 && legs == 45)
			{
				socialShadow = true;
			}
			if (head == 74 && body == 48 && legs == 44)
			{
				socialShadow = true;
			}
			if (head == 74 && body == 48 && legs == 44 && Main.rand.Next(10) == 0)
			{
				int num5 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 14, 0f, 0f, 200, default(Color), 1.2f);
				Main.dust[num5].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
			}
			if (head == 57 && body == 37 && legs == 35)
			{
				int maxValue = 10;
				if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) > 1f)
				{
					maxValue = 2;
				}
				if (Main.rand.Next(maxValue) == 0)
				{
					int num6 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 115, 0f, 0f, 140, default(Color), 0.75f);
					Main.dust[num6].noGravity = true;
					Main.dust[num6].fadeIn = 1.5f;
					Main.dust[num6].velocity *= 0.3f;
					Main.dust[num6].velocity += velocity * 0.2f;
					Main.dust[num6].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
				}
			}
			if (head == 6 && body == 6 && legs == 6 && Math.Abs(velocity.X) + Math.Abs(velocity.Y) > 1f && !rocketFrame)
			{
				for (int k = 0; k < 2; k++)
				{
					int num7 = Dust.NewDust(new Vector2(position.X - velocity.X * 2f, position.Y - 2f - velocity.Y * 2f), width, height, 6, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num7].noGravity = true;
					Main.dust[num7].noLight = true;
					Main.dust[num7].velocity.X -= velocity.X * 0.5f;
					Main.dust[num7].velocity.Y -= velocity.Y * 0.5f;
					Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
				}
			}
			if (head == 8 && body == 8 && legs == 8 && Math.Abs(velocity.X) + Math.Abs(velocity.Y) > 1f)
			{
				int num8 = Dust.NewDust(new Vector2(position.X - velocity.X * 2f, position.Y - 2f - velocity.Y * 2f), width, height, 40, 0f, 0f, 50, default(Color), 1.4f);
				Main.dust[num8].noGravity = true;
				Main.dust[num8].velocity.X = velocity.X * 0.25f;
				Main.dust[num8].velocity.Y = velocity.Y * 0.25f;
				Main.dust[num8].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
			}
			if (head == 9 && body == 9 && legs == 9 && Math.Abs(velocity.X) + Math.Abs(velocity.Y) > 1f && !rocketFrame)
			{
				for (int l = 0; l < 2; l++)
				{
					int num9 = Dust.NewDust(new Vector2(position.X - velocity.X * 2f, position.Y - 2f - velocity.Y * 2f), width, height, 6, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num9].noGravity = true;
					Main.dust[num9].noLight = true;
					Main.dust[num9].velocity.X -= velocity.X * 0.5f;
					Main.dust[num9].velocity.Y -= velocity.Y * 0.5f;
					Main.dust[num9].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
				}
			}
			if (body == 18 && legs == 17 && (head == 32 || head == 33 || head == 34) && Main.rand.Next(10) == 0)
			{
				int num10 = Dust.NewDust(new Vector2(position.X - velocity.X * 2f, position.Y - 2f - velocity.Y * 2f), width, height, 43, 0f, 0f, 100, default(Color), 0.3f);
				Main.dust[num10].fadeIn = 0.8f;
				Main.dust[num10].velocity *= 0f;
				Main.dust[num10].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
			}
			if (body == 24 && legs == 23 && (head == 42 || head == 43 || head == 41) && velocity.X != 0f && velocity.Y != 0f && Main.rand.Next(10) == 0)
			{
				int num11 = Dust.NewDust(new Vector2(position.X - velocity.X * 2f, position.Y - 2f - velocity.Y * 2f), width, height, 43, 0f, 0f, 100, default(Color), 0.3f);
				Main.dust[num11].fadeIn = 0.8f;
				Main.dust[num11].velocity *= 0f;
				Main.dust[num11].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
			}
			if (body == 36 && head == 56 && velocity.X != 0f && velocity.Y == 0f)
			{
				for (int m = 0; m < 2; m++)
				{
					int num12 = Dust.NewDust(new Vector2(position.X, position.Y + (float)((gravDir == 1f) ? (height - 2) : (-4))), width, 6, 106, 0f, 0f, 100, default(Color), 0.1f);
					Main.dust[num12].fadeIn = 1f;
					Main.dust[num12].noGravity = true;
					Main.dust[num12].velocity *= 0.2f;
					Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
				}
			}
			if (body == 27 && head == 46 && legs == 26)
			{
				frostArmor = true;
				if (velocity.X != 0f && velocity.Y == 0f && miscCounter % 2 == 0)
				{
					for (int n = 0; n < 2; n++)
					{
						int num13 = (n != 0) ? Dust.NewDust(new Vector2(position.X + (float)(width / 2), position.Y + (float)height + gfxOffY), width / 2, 6, 76, 0f, 0f, 0, default(Color), 1.35f) : Dust.NewDust(new Vector2(position.X, position.Y + (float)height + gfxOffY), width / 2, 6, 76, 0f, 0f, 0, default(Color), 1.35f);
						Main.dust[num13].scale *= 1f + (float)Main.rand.Next(20, 40) * 0.01f;
						Main.dust[num13].noGravity = true;
						Main.dust[num13].noLight = true;
						Main.dust[num13].velocity *= 0.001f;
						Main.dust[num13].velocity.Y -= 0.003f;
						Main.dust[num13].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
					}
				}
			}
			bodyFrame.Width = 40;
			bodyFrame.Height = 56;
			legFrame.Width = 40;
			legFrame.Height = 56;
			bodyFrame.X = 0;
			legFrame.X = 0;
			if (mount.Active)
			{
				legFrameCounter = 0.0;
				legFrame.Y = legFrame.Height * 6;
				if (velocity.Y != 0f)
				{
					if (mount.FlyTime > 0 && jump == 0 && controlJump && !mount.CanHover)
					{
						if (mount.Type == 0)
						{
							if (direction > 0)
							{
								if (Main.rand.Next(4) == 0)
								{
									int num14 = Dust.NewDust(new Vector2(base.Center.X - 22f, position.Y + (float)height - 6f), 20, 10, 64, velocity.X * 0.25f, velocity.Y * 0.25f, 255);
									Main.dust[num14].velocity *= 0.1f;
									Main.dust[num14].noLight = true;
								}
								if (Main.rand.Next(4) == 0)
								{
									int num15 = Dust.NewDust(new Vector2(base.Center.X + 12f, position.Y + (float)height - 6f), 20, 10, 64, velocity.X * 0.25f, velocity.Y * 0.25f, 255);
									Main.dust[num15].velocity *= 0.1f;
									Main.dust[num15].noLight = true;
								}
							}
							else
							{
								if (Main.rand.Next(4) == 0)
								{
									int num16 = Dust.NewDust(new Vector2(base.Center.X - 32f, position.Y + (float)height - 6f), 20, 10, 64, velocity.X * 0.25f, velocity.Y * 0.25f, 255);
									Main.dust[num16].velocity *= 0.1f;
									Main.dust[num16].noLight = true;
								}
								if (Main.rand.Next(4) == 0)
								{
									int num17 = Dust.NewDust(new Vector2(base.Center.X + 2f, position.Y + (float)height - 6f), 20, 10, 64, velocity.X * 0.25f, velocity.Y * 0.25f, 255);
									Main.dust[num17].velocity *= 0.1f;
									Main.dust[num17].noLight = true;
								}
							}
						}
						mount.UpdateFrame(this, 3, velocity);
					}
					else if (wet)
					{
						mount.UpdateFrame(this, 4, velocity);
					}
					else
					{
						mount.UpdateFrame(this, 2, velocity);
					}
				}
				else if (velocity.X == 0f)
				{
					mount.UpdateFrame(this, 0, velocity);
				}
				else
				{
					mount.UpdateFrame(this, 1, velocity);
				}
			}
			else if (swimTime > 0)
			{
				legFrameCounter += 2.0;
				while (legFrameCounter > 8.0)
				{
					legFrameCounter -= 8.0;
					legFrame.Y += legFrame.Height;
				}
				if (legFrame.Y < legFrame.Height * 7)
				{
					legFrame.Y = legFrame.Height * 19;
				}
				else if (legFrame.Y > legFrame.Height * 19)
				{
					legFrame.Y = legFrame.Height * 7;
				}
			}
			else if (velocity.Y != 0f || grappling[0] > -1)
			{
				legFrameCounter = 0.0;
				legFrame.Y = legFrame.Height * 5;
				if (wings == 22 || wings == 28)
				{
					legFrame.Y = 0;
				}
			}
			else if (velocity.X != 0f)
			{
				if ((slippy || slippy2) && !controlLeft && !controlRight)
				{
					legFrameCounter = 0.0;
					legFrame.Y = 0;
				}
				else
				{
					legFrameCounter += (double)Math.Abs(velocity.X) * 1.3;
					while (legFrameCounter > 8.0)
					{
						legFrameCounter -= 8.0;
						legFrame.Y += legFrame.Height;
					}
					if (legFrame.Y < legFrame.Height * 7)
					{
						legFrame.Y = legFrame.Height * 19;
					}
					else if (legFrame.Y > legFrame.Height * 19)
					{
						legFrame.Y = legFrame.Height * 7;
					}
				}
			}
			else
			{
				legFrameCounter = 0.0;
				legFrame.Y = 0;
			}
			if (carpetFrame >= 0)
			{
				legFrameCounter = 0.0;
				legFrame.Y = 0;
			}
			if (sandStorm)
			{
				if (miscCounter % 4 == 0 && itemAnimation == 0)
				{
					ChangeDir(direction * -1);
					if (inventory[selectedItem].holdStyle == 2)
					{
						if (inventory[selectedItem].type == 946)
						{
							itemLocation.X = position.X + (float)width * 0.5f - (float)(16 * direction);
						}
						if (inventory[selectedItem].type == 186)
						{
							itemLocation.X = position.X + (float)width * 0.5f + (float)(6 * direction);
							itemRotation = 0.79f * (float)(-direction);
						}
					}
				}
				legFrameCounter = 0.0;
				legFrame.Y = 0;
			}
			if (itemAnimation > 0 && inventory[selectedItem].useStyle != 10)
			{
				if (inventory[selectedItem].useStyle == 1 || inventory[selectedItem].type == 0)
				{
					if ((double)itemAnimation < (double)itemAnimationMax * 0.333)
					{
						bodyFrame.Y = bodyFrame.Height * 3;
					}
					else if ((double)itemAnimation < (double)itemAnimationMax * 0.666)
					{
						bodyFrame.Y = bodyFrame.Height * 2;
					}
					else
					{
						bodyFrame.Y = bodyFrame.Height;
					}
				}
				else if (inventory[selectedItem].useStyle == 2)
				{
					if ((double)itemAnimation > (double)itemAnimationMax * 0.5)
					{
						bodyFrame.Y = bodyFrame.Height * 3;
					}
					else
					{
						bodyFrame.Y = bodyFrame.Height * 2;
					}
				}
				else if (inventory[selectedItem].useStyle == 3)
				{
					if ((double)itemAnimation > (double)itemAnimationMax * 0.666)
					{
						bodyFrame.Y = bodyFrame.Height * 3;
					}
					else
					{
						bodyFrame.Y = bodyFrame.Height * 3;
					}
				}
				else if (inventory[selectedItem].useStyle == 4)
				{
					bodyFrame.Y = bodyFrame.Height * 2;
				}
				else
				{
					if (inventory[selectedItem].useStyle != 5)
					{
						return;
					}
					if (inventory[selectedItem].type == 281 || inventory[selectedItem].type == 986)
					{
						bodyFrame.Y = bodyFrame.Height * 2;
						return;
					}
					float num18 = itemRotation * (float)direction;
					bodyFrame.Y = bodyFrame.Height * 3;
					if ((double)num18 < -0.75)
					{
						bodyFrame.Y = bodyFrame.Height * 2;
						if (gravDir == -1f)
						{
							bodyFrame.Y = bodyFrame.Height * 4;
						}
					}
					if ((double)num18 > 0.6)
					{
						bodyFrame.Y = bodyFrame.Height * 4;
						if (gravDir == -1f)
						{
							bodyFrame.Y = bodyFrame.Height * 2;
						}
					}
				}
			}
			else if (mount.Active)
			{
				bodyFrameCounter = 0.0;
				bodyFrame.Y = bodyFrame.Height * mount.BodyFrame;
			}
			else if (pulley)
			{
				if (pulleyDir == 2)
				{
					bodyFrame.Y = bodyFrame.Height;
				}
				else
				{
					bodyFrame.Y = bodyFrame.Height * 2;
				}
			}
			else if (inventory[selectedItem].holdStyle == 1 && (!wet || !inventory[selectedItem].noWet))
			{
				bodyFrame.Y = bodyFrame.Height * 3;
			}
			else if (inventory[selectedItem].holdStyle == 2 && (!wet || !inventory[selectedItem].noWet))
			{
				bodyFrame.Y = bodyFrame.Height * 2;
			}
			else if (inventory[selectedItem].holdStyle == 3)
			{
				bodyFrame.Y = bodyFrame.Height * 3;
			}
			else if (grappling[0] >= 0)
			{
				sandStorm = false;
				dJumpEffectCloud = false;
				dJumpEffectSandstorm = false;
				dJumpEffectBlizzard = false;
				dJumpEffectFart = false;
				dJumpEffectSail = false;
				dJumpEffectUnicorn = false;
				Vector2 vector = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
				float num19 = 0f;
				float num20 = 0f;
				for (int num21 = 0; num21 < grapCount; num21++)
				{
					num19 += Main.projectile[grappling[num21]].position.X + (float)(Main.projectile[grappling[num21]].width / 2);
					num20 += Main.projectile[grappling[num21]].position.Y + (float)(Main.projectile[grappling[num21]].height / 2);
				}
				num19 /= (float)grapCount;
				num20 /= (float)grapCount;
				num19 -= vector.X;
				num20 -= vector.Y;
				if (num20 < 0f && Math.Abs(num20) > Math.Abs(num19))
				{
					bodyFrame.Y = bodyFrame.Height * 2;
					if (gravDir == -1f)
					{
						bodyFrame.Y = bodyFrame.Height * 4;
					}
				}
				else if (num20 > 0f && Math.Abs(num20) > Math.Abs(num19))
				{
					bodyFrame.Y = bodyFrame.Height * 4;
					if (gravDir == -1f)
					{
						bodyFrame.Y = bodyFrame.Height * 2;
					}
				}
				else
				{
					bodyFrame.Y = bodyFrame.Height * 3;
				}
			}
			else if (swimTime > 0)
			{
				if (swimTime > 20)
				{
					bodyFrame.Y = 0;
				}
				else if (swimTime > 10)
				{
					bodyFrame.Y = bodyFrame.Height * 5;
				}
				else
				{
					bodyFrame.Y = 0;
				}
			}
			else if (velocity.Y != 0f)
			{
				if (sliding)
				{
					bodyFrame.Y = bodyFrame.Height * 3;
				}
				else if (sandStorm || carpetFrame >= 0)
				{
					bodyFrame.Y = bodyFrame.Height * 6;
				}
				else if (eocDash > 0)
				{
					bodyFrame.Y = bodyFrame.Height * 6;
				}
				else if (wings > 0)
				{
					if (wings == 22 || wings == 28)
					{
						bodyFrame.Y = 0;
					}
					else if (velocity.Y > 0f)
					{
						if (controlJump)
						{
							bodyFrame.Y = bodyFrame.Height * 6;
						}
						else
						{
							bodyFrame.Y = bodyFrame.Height * 5;
						}
					}
					else
					{
						bodyFrame.Y = bodyFrame.Height * 6;
					}
				}
				else
				{
					bodyFrame.Y = bodyFrame.Height * 5;
				}
				bodyFrameCounter = 0.0;
			}
			else if (velocity.X != 0f)
			{
				bodyFrameCounter += (double)Math.Abs(velocity.X) * 1.5;
				bodyFrame.Y = legFrame.Y;
			}
			else
			{
				bodyFrameCounter = 0.0;
				bodyFrame.Y = 0;
			}
		}

		public static int SetMatch(int armorslot, int type, bool male, ref bool somethingSpecial)
		{
			int num = -1;
			if (armorslot == 1)
			{
				switch (type)
				{
				case 15:
					num = 88;
					break;
				case 36:
					num = 89;
					break;
				case 41:
					num = 97;
					break;
				case 42:
					num = 90;
					break;
				case 58:
					num = 91;
					break;
				case 59:
					num = 92;
					break;
				case 60:
					num = 93;
					break;
				case 61:
					num = 94;
					break;
				case 62:
					num = 95;
					break;
				case 63:
					num = 96;
					break;
				case 77:
					num = 121;
					break;
				case 165:
					num = (male ? 99 : 118);
					break;
				case 166:
					num = (male ? 100 : 119);
					break;
				case 167:
					num = (male ? 101 : 102);
					break;
				case 180:
					num = 115;
					break;
				case 181:
					num = 116;
					break;
				case 183:
					num = 123;
					break;
				case 191:
					num = 131;
					break;
				}
				if (num != -1)
				{
					somethingSpecial = true;
				}
			}
			if (armorslot == 2)
			{
				switch (type)
				{
				case 83:
					if (male)
					{
						num = 117;
					}
					break;
				case 84:
					if (male)
					{
						num = 120;
					}
					break;
				}
			}
			return num;
		}

		public void Teleport(Vector2 newPos, int Style = 0, int extraInfo = 0)
		{
			try
			{
				grappling[0] = -1;
				grapCount = 0;
				for (int i = 0; i < 1000; i++)
				{
					if (Main.projectile[i].active && Main.projectile[i].owner == whoAmI && Main.projectile[i].aiStyle == 7)
					{
						Main.projectile[i].Kill();
					}
				}
				int extraInfo2 = 0;
				if (Style == 4)
				{
					extraInfo2 = lastPortalColorIndex;
				}
				Main.TeleportEffect(getRect(), Style, extraInfo2);
				float num = Vector2.Distance(position, newPos);
				position = newPos;
				fallStart = (int)(position.Y / 16f);
				if (whoAmI == Main.myPlayer)
				{
					if (num < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 100f)
					{
						int time = 0;
						if (Style == 1)
						{
							time = 10;
						}
						Main.SetCameraLerp(0.1f, time);
					}
					else
					{
						Main.BlackFadeIn = 255;
						Lighting.BlackOut();
						Main.screenLastPosition = Main.screenPosition;
						Main.screenPosition.X = position.X + (float)(width / 2) - (float)(Main.screenWidth / 2);
						Main.screenPosition.Y = position.Y + (float)(height / 2) - (float)(Main.screenHeight / 2);
						Main.quickBG = 10;
					}
					if (Main.mapTime < 5)
					{
						Main.mapTime = 5;
					}
					Main.maxQ = true;
					Main.renderNow = true;
				}
				if (Style == 4)
				{
					lastPortalColorIndex = extraInfo;
					extraInfo2 = lastPortalColorIndex;
					portalPhysicsFlag = true;
					gravity = 0f;
				}
				for (int j = 0; j < 3; j++)
				{
					UpdateSocialShadow();
				}
				oldPosition = position;
				Main.TeleportEffect(getRect(), Style, extraInfo2);
				teleportTime = 1f;
				teleportStyle = Style;
			}
			catch
			{
			}
		}

		public void Spawn()
		{
			Main.InitLifeBytes();
			if (whoAmI == Main.myPlayer)
			{
				if (Main.mapTime < 5)
				{
					Main.mapTime = 5;
				}
				Main.quickBG = 10;
				FindSpawn();
				if (!CheckSpawn(SpawnX, SpawnY))
				{
					SpawnX = -1;
					SpawnY = -1;
				}
				Main.maxQ = true;
			}
			if (Main.netMode == 1 && whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(12, -1, -1, "", Main.myPlayer);
				Main.gameMenu = false;
			}
			headPosition = Vector2.Zero;
			bodyPosition = Vector2.Zero;
			legPosition = Vector2.Zero;
			headRotation = 0f;
			bodyRotation = 0f;
			legRotation = 0f;
			lavaTime = lavaMax;
			if (statLife <= 0)
			{
				int num = statLifeMax2 / 2;
				statLife = 100;
				if (num > statLife)
				{
					statLife = num;
				}
				breath = breathMax;
				if (spawnMax)
				{
					statLife = statLifeMax2;
					statMana = statManaMax2;
				}
			}
			immune = true;
			dead = false;
			immuneTime = 0;
			active = true;
			if (SpawnX >= 0 && SpawnY >= 0)
			{
				position.X = SpawnX * 16 + 8 - width / 2;
				position.Y = SpawnY * 16 - height;
			}
			else
			{
				position.X = Main.spawnTileX * 16 + 8 - width / 2;
				position.Y = Main.spawnTileY * 16 - height;
				for (int i = Main.spawnTileX - 1; i < Main.spawnTileX + 2; i++)
				{
					for (int j = Main.spawnTileY - 3; j < Main.spawnTileY; j++)
					{
						if (Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type])
						{
							WorldGen.KillTile(i, j);
						}
						if (Main.tile[i, j].liquid > 0)
						{
							Main.tile[i, j].lava(false);
							Main.tile[i, j].liquid = 0;
							WorldGen.SquareTileFrame(i, j);
						}
					}
				}
			}
			wet = false;
			wetCount = 0;
			lavaWet = false;
			fallStart = (int)(position.Y / 16f);
			fallStart2 = fallStart;
			velocity.X = 0f;
			velocity.Y = 0f;
			for (int k = 0; k < 3; k++)
			{
				UpdateSocialShadow();
			}
			oldPosition = position;
			talkNPC = -1;
			if (whoAmI == Main.myPlayer)
			{
				Main.npcChatCornerItem = 0;
			}
			if (pvpDeath)
			{
				pvpDeath = false;
				immuneTime = 300;
				statLife = statLifeMax;
			}
			else
			{
				immuneTime = 60;
			}
			if (whoAmI == Main.myPlayer)
			{
				Main.BlackFadeIn = 255;
				Main.renderNow = true;
				if (Main.netMode == 1)
				{
					Netplay.newRecent();
				}
				Main.screenPosition.X = position.X + (float)(width / 2) - (float)(Main.screenWidth / 2);
				Main.screenPosition.Y = position.Y + (float)(height / 2) - (float)(Main.screenHeight / 2);
			}
		}

		public void ShadowDodge()
		{
			immune = true;
			immuneTime = 80;
			if (longInvince)
			{
				immuneTime += 40;
			}
			if (whoAmI != Main.myPlayer)
			{
				return;
			}
			for (int i = 0; i < 22; i++)
			{
				if (buffTime[i] > 0 && buffType[i] == 59)
				{
					DelBuff(i);
				}
			}
			NetMessage.SendData(62, -1, -1, "", whoAmI, 2f);
		}

		public void NinjaDodge()
		{
			immune = true;
			immuneTime = 80;
			if (longInvince)
			{
				immuneTime += 40;
			}
			for (int i = 0; i < 100; i++)
			{
				int num = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num].position.X += Main.rand.Next(-20, 21);
				Main.dust[num].position.Y += Main.rand.Next(-20, 21);
				Main.dust[num].velocity *= 0.4f;
				Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
				Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(cWaist, this);
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[num].noGravity = true;
				}
			}
			int num2 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num2].scale = 1.5f;
			Main.gore[num2].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity *= 0.4f;
			num2 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num2].scale = 1.5f;
			Main.gore[num2].velocity.X = 1.5f + (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity.Y = 1.5f + (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity *= 0.4f;
			num2 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num2].scale = 1.5f;
			Main.gore[num2].velocity.X = -1.5f - (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity.Y = 1.5f + (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity *= 0.4f;
			num2 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num2].scale = 1.5f;
			Main.gore[num2].velocity.X = 1.5f + (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity.Y = -1.5f - (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity *= 0.4f;
			num2 = Gore.NewGore(new Vector2(position.X + (float)(width / 2) - 24f, position.Y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num2].scale = 1.5f;
			Main.gore[num2].velocity.X = -1.5f - (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity.Y = -1.5f - (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num2].velocity *= 0.4f;
			if (whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(62, -1, -1, "", whoAmI, 1f);
			}
		}

		public double Hurt(int Damage, int hitDirection, bool pvp = false, bool quiet = false, string deathText = " was slain...", bool Crit = false, int cooldownCounter = -1)
		{
			bool flag = !immune;
			if (cooldownCounter == 0)
			{
				flag = (hurtCooldowns[cooldownCounter] <= 0);
			}
			if (cooldownCounter == 1)
			{
				flag = (hurtCooldowns[cooldownCounter] <= 0);
			}
			if (flag)
			{
				if (whoAmI == Main.myPlayer && blackBelt && Main.rand.Next(10) == 0)
				{
					NinjaDodge();
					return 0.0;
				}
				if (whoAmI == Main.myPlayer && shadowDodge)
				{
					ShadowDodge();
					return 0.0;
				}
				if (whoAmI == Main.myPlayer && panic)
				{
					AddBuff(63, 300);
				}
				stealth = 1f;
				if (Main.netMode == 1)
				{
					NetMessage.SendData(84, -1, -1, "", whoAmI);
				}
				int num = Damage;
				double num2 = Main.CalculatePlayerDamage(num, statDefense);
				if (Crit)
				{
					num *= 2;
				}
				if (num2 >= 1.0)
				{
					if (invis)
					{
						for (int i = 0; i < 22; i++)
						{
							if (buffType[i] == 10)
							{
								DelBuff(i);
							}
						}
					}
					num2 = (int)((double)(1f - endurance) * num2);
					if (num2 < 1.0)
					{
						num2 = 1.0;
					}
					if (ConsumeSolarFlare())
					{
						float num3 = 0.3f;
						num2 = (int)((double)(1f - num3) * num2);
						if (num2 < 1.0)
						{
							num2 = 1.0;
						}
						if (whoAmI == Main.myPlayer)
						{
							int num4 = Projectile.NewProjectile(base.Center.X, base.Center.Y, 0f, 0f, 608, 150, 15f, Main.myPlayer);
							Main.projectile[num4].Kill();
						}
					}
					if (beetleDefense && beetleOrbs > 0)
					{
						float num5 = 0.15f * (float)beetleOrbs;
						num2 = (int)((double)(1f - num5) * num2);
						beetleOrbs--;
						for (int j = 0; j < 22; j++)
						{
							if (buffType[j] >= 95 && buffType[j] <= 97)
							{
								DelBuff(j);
							}
						}
						if (beetleOrbs > 0)
						{
							AddBuff(95 + beetleOrbs - 1, 5, false);
						}
						beetleCounter = 0f;
						if (num2 < 1.0)
						{
							num2 = 1.0;
						}
					}
					if (magicCuffs)
					{
						int num6 = num;
						statMana += num6;
						if (statMana > statManaMax2)
						{
							statMana = statManaMax2;
						}
						ManaEffect(num6);
					}
					if (paladinBuff && whoAmI != Main.myPlayer)
					{
						int damage = (int)(num2 * 0.25);
						num2 = (int)(num2 * 0.75);
						if (Main.player[Main.myPlayer].paladinGive)
						{
							int myPlayer = Main.myPlayer;
							if (Main.player[myPlayer].team == team && team != 0)
							{
								float num7 = position.X - Main.player[myPlayer].position.X;
								float num8 = position.Y - Main.player[myPlayer].position.Y;
								float num9 = (float)Math.Sqrt(num7 * num7 + num8 * num8);
								if (num9 < 800f)
								{
									Main.player[myPlayer].Hurt(damage, 0, false, false, "");
								}
							}
						}
					}
					if (brainOfConfusion && Main.myPlayer == whoAmI)
					{
						for (int k = 0; k < 200; k++)
						{
							if (!Main.npc[k].active || Main.npc[k].friendly)
							{
								continue;
							}
							int num10 = 300;
							num10 += (int)num2 * 2;
							if (Main.rand.Next(500) < num10)
							{
								float num11 = (Main.npc[k].Center - base.Center).Length();
								float num12 = Main.rand.Next(200 + (int)num2 / 2, 301 + (int)num2 * 2);
								if (num12 > 500f)
								{
									num12 = 500f + (num12 - 500f) * 0.75f;
								}
								if (num12 > 700f)
								{
									num12 = 700f + (num12 - 700f) * 0.5f;
								}
								if (num12 > 900f)
								{
									num12 = 900f + (num12 - 900f) * 0.25f;
								}
								if (num11 < num12)
								{
									float num13 = Main.rand.Next(90 + (int)num2 / 3, 300 + (int)num2 / 2);
									Main.npc[k].AddBuff(31, (int)num13);
								}
							}
						}
						Projectile.NewProjectile(base.Center.X + (float)Main.rand.Next(-40, 40), base.Center.Y - (float)Main.rand.Next(20, 60), velocity.X * 0.3f, velocity.Y * 0.3f, 565, 0, 0f, whoAmI);
					}
					if (Main.netMode == 1 && whoAmI == Main.myPlayer && !quiet)
					{
						int number = 0;
						if (Crit)
						{
							number = 1;
						}
						int num14 = 0;
						if (pvp)
						{
							num14 = 1;
						}
						NetMessage.SendData(13, -1, -1, "", whoAmI);
						NetMessage.SendData(16, -1, -1, "", whoAmI);
						NetMessage.SendData(26, -1, -1, "", whoAmI, hitDirection, Damage, num14, number, cooldownCounter);
					}
					Color color = Crit ? CombatText.DamagedFriendlyCrit : CombatText.DamagedFriendly;
					CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, width, height), color, string.Concat((int)num2), Crit);
					statLife -= (int)num2;
					switch (cooldownCounter)
					{
					case -1:
						immune = true;
						if (num2 == 1.0)
						{
							immuneTime = 20;
							if (longInvince)
							{
								immuneTime += 20;
							}
						}
						else
						{
							immuneTime = 40;
							if (longInvince)
							{
								immuneTime += 40;
							}
						}
						if (pvp)
						{
							immuneTime = 8;
						}
						break;
					case 0:
						if (num2 == 1.0)
						{
							hurtCooldowns[cooldownCounter] = (longInvince ? 40 : 20);
						}
						else
						{
							hurtCooldowns[cooldownCounter] = (longInvince ? 80 : 40);
						}
						break;
					case 1:
						if (num2 == 1.0)
						{
							hurtCooldowns[cooldownCounter] = (longInvince ? 40 : 20);
						}
						else
						{
							hurtCooldowns[cooldownCounter] = (longInvince ? 80 : 40);
						}
						break;
					}
					lifeRegenTime = 0;
					if (whoAmI == Main.myPlayer)
					{
						if (starCloak)
						{
							for (int l = 0; l < 3; l++)
							{
								float x = position.X + (float)Main.rand.Next(-400, 400);
								float y = position.Y - (float)Main.rand.Next(500, 800);
								Vector2 vector = new Vector2(x, y);
								float num15 = position.X + (float)(width / 2) - vector.X;
								float num16 = position.Y + (float)(height / 2) - vector.Y;
								num15 += (float)Main.rand.Next(-100, 101);
								int num17 = 23;
								float num18 = (float)Math.Sqrt(num15 * num15 + num16 * num16);
								num18 = (float)num17 / num18;
								num15 *= num18;
								num16 *= num18;
								int num19 = Projectile.NewProjectile(x, y, num15, num16, 92, 30, 5f, whoAmI);
								Main.projectile[num19].ai[1] = position.Y;
							}
						}
						if (bee)
						{
							int num20 = 1;
							if (Main.rand.Next(3) == 0)
							{
								num20++;
							}
							if (Main.rand.Next(3) == 0)
							{
								num20++;
							}
							if (strongBees && Main.rand.Next(3) == 0)
							{
								num20++;
							}
							for (int m = 0; m < num20; m++)
							{
								float speedX = (float)Main.rand.Next(-35, 36) * 0.02f;
								float speedY = (float)Main.rand.Next(-35, 36) * 0.02f;
								Projectile.NewProjectile(position.X, position.Y, speedX, speedY, beeType(), beeDamage(7), beeKB(0f), Main.myPlayer);
							}
						}
					}
					if (!noKnockback && hitDirection != 0 && (!mount.Active || !mount.Cart))
					{
						velocity.X = 4.5f * (float)hitDirection;
						velocity.Y = -3.5f;
					}
					if (stoned)
					{
						Main.PlaySound(0, (int)position.X, (int)position.Y);
					}
					else if (frostArmor)
					{
						Main.PlaySound(2, (int)position.X, (int)position.Y, 27);
					}
					else if (wereWolf)
					{
						Main.PlaySound(3, (int)position.X, (int)position.Y, 6);
					}
					else if (boneArmor)
					{
						Main.PlaySound(3, (int)position.X, (int)position.Y, 2);
					}
					else if (!Male)
					{
						Main.PlaySound(20, (int)position.X, (int)position.Y);
					}
					else
					{
						Main.PlaySound(1, (int)position.X, (int)position.Y);
					}
					if (statLife > 0)
					{
						for (int n = 0; (double)n < num2 / (double)statLifeMax2 * 100.0; n++)
						{
							if (stoned)
							{
								Dust.NewDust(position, width, height, 1, 2 * hitDirection, -2f);
							}
							else if (frostArmor)
							{
								int num21 = Dust.NewDust(position, width, height, 135, 2 * hitDirection, -2f);
								Main.dust[num21].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
							}
							else if (boneArmor)
							{
								int num22 = Dust.NewDust(position, width, height, 26, 2 * hitDirection, -2f);
								Main.dust[num22].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
							}
							else
							{
								Dust.NewDust(position, width, height, 5, 2 * hitDirection, -2f);
							}
						}
					}
					else
					{
						statLife = 0;
						if (whoAmI == Main.myPlayer)
						{
							KillMe(num2, hitDirection, pvp, deathText);
						}
					}
				}
				if (pvp)
				{
					num2 = Main.CalculateDamage(num, statDefense);
				}
				return num2;
			}
			return 0.0;
		}

		public void KillMeForGood()
		{
			bool ısCloudSave = Main.ActivePlayerFileData.IsCloudSave;
			if (FileUtilities.Exists(Main.playerPathName, ısCloudSave))
			{
				FileUtilities.Delete(Main.playerPathName, ısCloudSave);
			}
			if (FileUtilities.Exists(Main.playerPathName + ".bak", ısCloudSave))
			{
				FileUtilities.Delete(Main.playerPathName + ".bak", ısCloudSave);
			}
			Main.ActivePlayerFileData = new PlayerFileData();
		}

		public void KillMe(double dmg, int hitDirection, bool pvp = false, string deathText = " was slain...")
		{
			if (dead)
			{
				return;
			}
			if (pvp)
			{
				pvpDeath = true;
			}
			if (trapDebuffSource)
			{
				AchievementsHelper.HandleSpecialEvent(this, 4);
			}
			lastDeathPostion = base.Center;
			lastDeathTime = DateTime.Now;
			showLastDeath = true;
			bool overFlowing;
			int coinsOwned = (int)Utils.CoinsCount(out overFlowing, inventory);
			if (Main.myPlayer == whoAmI)
			{
				lostCoins = coinsOwned;
				lostCoinString = Main.ValueToCoins(lostCoins);
			}
			if (Main.myPlayer == whoAmI)
			{
				Main.mapFullscreen = false;
			}
			if (Main.myPlayer == whoAmI)
			{
				trashItem.SetDefaults();
				if (difficulty == 0)
				{
					for (int i = 0; i < 59; i++)
					{
						if (inventory[i].stack > 0 && inventory[i].type >= 1522 && inventory[i].type <= 1527)
						{
							int num = Item.NewItem((int)position.X, (int)position.Y, width, height, inventory[i].type);
							Main.item[num].netDefaults(inventory[i].netID);
							Main.item[num].Prefix(inventory[i].prefix);
							Main.item[num].stack = inventory[i].stack;
							Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
							Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
							Main.item[num].noGrabDelay = 100;
							Main.item[num].favorited = false;
							Main.item[num].newAndShiny = false;
							if (Main.netMode == 1)
							{
								NetMessage.SendData(21, -1, -1, "", num);
							}
							inventory[i].SetDefaults();
						}
					}
				}
				else if (difficulty == 1)
				{
					DropItems();
				}
				else if (difficulty == 2)
				{
					DropItems();
					KillMeForGood();
				}
			}
			Main.PlaySound(5, (int)position.X, (int)position.Y);
			headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			if (stoned)
			{
				headPosition = Vector2.Zero;
				bodyPosition = Vector2.Zero;
				legPosition = Vector2.Zero;
			}
			for (int j = 0; j < 100; j++)
			{
				if (stoned)
				{
					Dust.NewDust(position, width, height, 1, 2 * hitDirection, -2f);
				}
				else if (frostArmor)
				{
					int num2 = Dust.NewDust(position, width, height, 135, 2 * hitDirection, -2f);
					Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
				}
				else if (boneArmor)
				{
					int num3 = Dust.NewDust(position, width, height, 26, 2 * hitDirection, -2f);
					Main.dust[num3].shader = GameShaders.Armor.GetSecondaryShader(ArmorSetDye(), this);
				}
				else
				{
					Dust.NewDust(position, width, height, 5, 2 * hitDirection, -2f);
				}
			}
			mount.Dismount(this);
			dead = true;
			respawnTimer = 600;
			bool flag = false;
			if (Main.netMode != 0 && !pvp)
			{
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == 13 || Main.npc[k].type == 14 || Main.npc[k].type == 15) && Math.Abs(base.Center.X - Main.npc[k].Center.X) + Math.Abs(base.Center.Y - Main.npc[k].Center.Y) < 4000f)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				respawnTimer += 600;
			}
			if (Main.expertMode)
			{
				respawnTimer = (int)((double)respawnTimer * 1.5);
			}
			immuneAlpha = 0;
			palladiumRegen = false;
			iceBarrier = false;
			crystalLeaf = false;
			if (Main.netMode == 2)
			{
				NetMessage.SendData(25, -1, -1, deathText, 16, 225f, 25f, 25f);
			}
			else if (Main.netMode == 0)
			{
				Main.NewText(deathText, 225, 25, 25);
			}
			if (Main.netMode == 1 && whoAmI == Main.myPlayer)
			{
				int num4 = 0;
				if (pvp)
				{
					num4 = 1;
				}
				NetMessage.SendData(44, -1, -1, deathText, whoAmI, hitDirection, (int)dmg, num4);
			}
			if (whoAmI == Main.myPlayer && difficulty == 0)
			{
				if (!pvp)
				{
					DropCoins();
				}
				else
				{
					lostCoins = 0;
					lostCoinString = Main.ValueToCoins(lostCoins);
				}
			}
			DropTombstone(coinsOwned, deathText, hitDirection);
			if (whoAmI == Main.myPlayer)
			{
				try
				{
					WorldGen.saveToonWhilePlaying();
				}
				catch
				{
				}
			}
		}

		public void DropTombstone(int coinsOwned, string deathText, int hitDirection)
		{
			if (Main.netMode != 1)
			{
				float num;
				for (num = (float)Main.rand.Next(-35, 36) * 0.1f; num < 2f && num > -2f; num += (float)Main.rand.Next(-30, 31) * 0.1f)
				{
				}
				int num2 = Main.rand.Next(6);
				if (coinsOwned <= 100000)
				{
					num2 = ((num2 != 0) ? (200 + num2) : 43);
				}
				else
				{
					num2 = Main.rand.Next(5);
					num2 += 527;
				}
				int num3 = Projectile.NewProjectile(position.X + (float)(width / 2), position.Y + (float)(height / 2), (float)Main.rand.Next(10, 30) * 0.1f * (float)hitDirection + num, (float)Main.rand.Next(-40, -20) * 0.1f, num2, 0, 0f, Main.myPlayer);
				Main.projectile[num3].miscText = deathText;
			}
		}

		public bool ItemSpace(Item newItem)
		{
			if (newItem.uniqueStack && HasItem(newItem.type))
			{
				return false;
			}
			if (newItem.type == 58 || newItem.type == 184 || newItem.type == 1734 || newItem.type == 1735 || newItem.type == 1867 || newItem.type == 1868)
			{
				return true;
			}
			if (ItemID.Sets.NebulaPickup[newItem.type])
			{
				return true;
			}
			int num = 50;
			if (newItem.type == 71 || newItem.type == 72 || newItem.type == 73 || newItem.type == 74)
			{
				num = 54;
			}
			for (int i = 0; i < num; i++)
			{
				if (inventory[i].type == 0)
				{
					return true;
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (inventory[j].type > 0 && inventory[j].stack < inventory[j].maxStack && newItem.IsTheSameAs(inventory[j]))
				{
					return true;
				}
			}
			if (newItem.ammo > 0 && !newItem.notAmmo)
			{
				if (newItem.type != 75 && newItem.type != 169 && newItem.type != 23 && newItem.type != 408 && newItem.type != 370 && newItem.type != 1246)
				{
					for (int k = 54; k < 58; k++)
					{
						if (inventory[k].type == 0)
						{
							return true;
						}
					}
				}
				for (int l = 54; l < 58; l++)
				{
					if (inventory[l].type > 0 && inventory[l].stack < inventory[l].maxStack && newItem.IsTheSameAs(inventory[l]))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void DoCoins(int i)
		{
			if (inventory[i].stack != 100 || (inventory[i].type != 71 && inventory[i].type != 72 && inventory[i].type != 73))
			{
				return;
			}
			inventory[i].SetDefaults(inventory[i].type + 1);
			for (int j = 0; j < 54; j++)
			{
				if (inventory[j].IsTheSameAs(inventory[i]) && j != i && inventory[j].type == inventory[i].type && inventory[j].stack < inventory[j].maxStack)
				{
					inventory[j].stack++;
					inventory[i].SetDefaults();
					inventory[i].active = false;
					inventory[i].type = 0;
					inventory[i].stack = 0;
					DoCoins(j);
				}
			}
		}

		public Item FillAmmo(int plr, Item newItem, bool noText = false)
		{
			for (int i = 54; i < 58; i++)
			{
				if (inventory[i].type <= 0 || inventory[i].stack >= inventory[i].maxStack || !newItem.IsTheSameAs(inventory[i]))
				{
					continue;
				}
				Main.PlaySound(7, (int)position.X, (int)position.Y);
				if (newItem.stack + inventory[i].stack <= inventory[i].maxStack)
				{
					inventory[i].stack += newItem.stack;
					if (!noText)
					{
						ItemText.NewText(newItem, newItem.stack);
					}
					DoCoins(i);
					if (plr == Main.myPlayer)
					{
						Recipe.FindRecipes();
					}
					return new Item();
				}
				newItem.stack -= inventory[i].maxStack - inventory[i].stack;
				if (!noText)
				{
					ItemText.NewText(newItem, inventory[i].maxStack - inventory[i].stack);
				}
				inventory[i].stack = inventory[i].maxStack;
				DoCoins(i);
				if (plr == Main.myPlayer)
				{
					Recipe.FindRecipes();
				}
			}
			if (newItem.bait <= 0 && newItem.type != 169 && newItem.type != 75 && newItem.type != 23 && newItem.type != 408 && newItem.type != 370 && newItem.type != 1246 && newItem.type != 154 && !newItem.notAmmo)
			{
				for (int j = 54; j < 58; j++)
				{
					if (inventory[j].type == 0)
					{
						inventory[j] = newItem;
						if (!noText)
						{
							ItemText.NewText(newItem, newItem.stack);
						}
						DoCoins(j);
						Main.PlaySound(7, (int)position.X, (int)position.Y);
						if (plr == Main.myPlayer)
						{
							Recipe.FindRecipes();
						}
						return new Item();
					}
				}
			}
			return newItem;
		}

		public Item GetItem(int plr, Item newItem, bool longText = false, bool noText = false)
		{
			bool flag = newItem.type >= 71 && newItem.type <= 74;
			Item ıtem = newItem;
			int num = 50;
			if (newItem.noGrabDelay > 0)
			{
				return ıtem;
			}
			int num2 = 0;
			if (newItem.uniqueStack && HasItem(newItem.type))
			{
				return ıtem;
			}
			if (newItem.type == 71 || newItem.type == 72 || newItem.type == 73 || newItem.type == 74)
			{
				num2 = -4;
				num = 54;
			}
			if (((ıtem.ammo > 0 || ıtem.bait > 0) && !ıtem.notAmmo) || ıtem.type == 530)
			{
				ıtem = FillAmmo(plr, ıtem, noText);
				if (ıtem.type == 0 || ıtem.stack == 0)
				{
					return new Item();
				}
			}
			for (int i = num2; i < 50; i++)
			{
				int num3 = i;
				if (num3 < 0)
				{
					num3 = 54 + i;
				}
				if (inventory[num3].type <= 0 || inventory[num3].stack >= inventory[num3].maxStack || !ıtem.IsTheSameAs(inventory[num3]))
				{
					continue;
				}
				if (flag)
				{
					Main.PlaySound(38, (int)position.X, (int)position.Y);
				}
				else
				{
					Main.PlaySound(7, (int)position.X, (int)position.Y);
				}
				if (ıtem.stack + inventory[num3].stack <= inventory[num3].maxStack)
				{
					inventory[num3].stack += ıtem.stack;
					if (!noText)
					{
						ItemText.NewText(newItem, ıtem.stack, false, longText);
					}
					DoCoins(num3);
					if (plr == Main.myPlayer)
					{
						Recipe.FindRecipes();
					}
					AchievementsHelper.NotifyItemPickup(this, ıtem);
					return new Item();
				}
				AchievementsHelper.NotifyItemPickup(this, ıtem, inventory[num3].maxStack - inventory[num3].stack);
				ıtem.stack -= inventory[num3].maxStack - inventory[num3].stack;
				if (!noText)
				{
					ItemText.NewText(newItem, inventory[num3].maxStack - inventory[num3].stack, false, longText);
				}
				inventory[num3].stack = inventory[num3].maxStack;
				DoCoins(num3);
				if (plr == Main.myPlayer)
				{
					Recipe.FindRecipes();
				}
			}
			if (newItem.type != 71 && newItem.type != 72 && newItem.type != 73 && newItem.type != 74 && newItem.useStyle > 0)
			{
				for (int j = 0; j < 10; j++)
				{
					if (inventory[j].type == 0)
					{
						inventory[j] = ıtem;
						if (!noText)
						{
							ItemText.NewText(newItem, newItem.stack, false, longText);
						}
						DoCoins(j);
						if (flag)
						{
							Main.PlaySound(38, (int)position.X, (int)position.Y);
						}
						else
						{
							Main.PlaySound(7, (int)position.X, (int)position.Y);
						}
						if (plr == Main.myPlayer)
						{
							Recipe.FindRecipes();
						}
						AchievementsHelper.NotifyItemPickup(this, ıtem);
						return new Item();
					}
				}
			}
			if (newItem.favorited)
			{
				for (int k = 0; k < num; k++)
				{
					if (inventory[k].type == 0)
					{
						inventory[k] = ıtem;
						if (!noText)
						{
							ItemText.NewText(newItem, newItem.stack, false, longText);
						}
						DoCoins(k);
						if (flag)
						{
							Main.PlaySound(38, (int)position.X, (int)position.Y);
						}
						else
						{
							Main.PlaySound(7, (int)position.X, (int)position.Y);
						}
						if (plr == Main.myPlayer)
						{
							Recipe.FindRecipes();
						}
						AchievementsHelper.NotifyItemPickup(this, ıtem);
						return new Item();
					}
				}
			}
			else
			{
				for (int num4 = num - 1; num4 >= 0; num4--)
				{
					if (inventory[num4].type == 0)
					{
						inventory[num4] = ıtem;
						if (!noText)
						{
							ItemText.NewText(newItem, newItem.stack, false, longText);
						}
						DoCoins(num4);
						if (flag)
						{
							Main.PlaySound(38, (int)position.X, (int)position.Y);
						}
						else
						{
							Main.PlaySound(7, (int)position.X, (int)position.Y);
						}
						if (plr == Main.myPlayer)
						{
							Recipe.FindRecipes();
						}
						AchievementsHelper.NotifyItemPickup(this, ıtem);
						return new Item();
					}
				}
			}
			return ıtem;
		}

		public void PlaceThing()
		{
			if ((inventory[selectedItem].type == 1071 || inventory[selectedItem].type == 1543) && position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY)
			{
				int num = tileTargetX;
				int num2 = tileTargetY;
				if (Main.tile[num, num2] != null && Main.tile[num, num2].active())
				{
					showItemIcon = true;
					if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
					{
						int num3 = -1;
						int num4 = -1;
						for (int i = 0; i < 58; i++)
						{
							if (inventory[i].stack > 0 && inventory[i].paint > 0)
							{
								num3 = inventory[i].paint;
								num4 = i;
								break;
							}
						}
						if (num3 > 0 && Main.tile[num, num2].color() != num3 && WorldGen.paintTile(num, num2, (byte)num3, true))
						{
							int num5 = num4;
							inventory[num5].stack--;
							if (inventory[num5].stack <= 0)
							{
								inventory[num5].SetDefaults();
							}
							itemTime = inventory[selectedItem].useTime;
						}
					}
				}
			}
			if ((inventory[selectedItem].type == 1072 || inventory[selectedItem].type == 1544) && position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY)
			{
				int num6 = tileTargetX;
				int num7 = tileTargetY;
				if (Main.tile[num6, num7] != null && Main.tile[num6, num7].wall > 0)
				{
					showItemIcon = true;
					if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
					{
						int num8 = -1;
						int num9 = -1;
						for (int j = 0; j < 58; j++)
						{
							if (inventory[j].stack > 0 && inventory[j].paint > 0)
							{
								num8 = inventory[j].paint;
								num9 = j;
								break;
							}
						}
						if (num8 > 0 && Main.tile[num6, num7].wallColor() != num8 && WorldGen.paintWall(num6, num7, (byte)num8, true))
						{
							int num10 = num9;
							inventory[num10].stack--;
							if (inventory[num10].stack <= 0)
							{
								inventory[num10].SetDefaults();
							}
							itemTime = inventory[selectedItem].useTime;
						}
					}
				}
			}
			if ((inventory[selectedItem].type == 1100 || inventory[selectedItem].type == 1545) && position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY)
			{
				int num11 = tileTargetX;
				int num12 = tileTargetY;
				if (Main.tile[num11, num12] != null && ((Main.tile[num11, num12].wallColor() > 0 && Main.tile[num11, num12].wall > 0) || (Main.tile[num11, num12].color() > 0 && Main.tile[num11, num12].active())))
				{
					showItemIcon = true;
					if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
					{
						if (Main.tile[num11, num12].color() > 0 && Main.tile[num11, num12].active() && WorldGen.paintTile(num11, num12, 0, true))
						{
							itemTime = inventory[selectedItem].useTime;
						}
						else if (Main.tile[num11, num12].wallColor() > 0 && Main.tile[num11, num12].wall > 0 && WorldGen.paintWall(num11, num12, 0, true))
						{
							itemTime = inventory[selectedItem].useTime;
						}
					}
				}
			}
			if ((inventory[selectedItem].type == 929 || inventory[selectedItem].type == 1338 || inventory[selectedItem].type == 1345) && position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY)
			{
				int num13 = tileTargetX;
				int num14 = tileTargetY;
				if (Main.tile[num13, num14].active() && Main.tile[num13, num14].type == 209)
				{
					int num15 = 0;
					if (Main.tile[num13, num14].frameX < 72)
					{
						if (inventory[selectedItem].type == 929)
						{
							num15 = 1;
						}
					}
					else if (Main.tile[num13, num14].frameX < 144)
					{
						if (inventory[selectedItem].type == 1338)
						{
							num15 = 2;
						}
					}
					else if (Main.tile[num13, num14].frameX < 288 && inventory[selectedItem].type == 1345)
					{
						num15 = 3;
					}
					if (num15 > 0)
					{
						showItemIcon = true;
						if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
						{
							int num16 = Main.tile[num13, num14].frameX / 18;
							int num17 = 0;
							int num18 = 0;
							while (num16 >= 4)
							{
								num17++;
								num16 -= 4;
							}
							num16 = num13 - num16;
							int num19;
							for (num19 = Main.tile[num13, num14].frameY / 18; num19 >= 3; num19 -= 3)
							{
								num18++;
							}
							num19 = num14 - num19;
							itemTime = inventory[selectedItem].useTime;
							float num20 = 14f;
							float num21 = 0f;
							float num22 = 0f;
							int type = 162;
							if (num15 == 2)
							{
								type = 281;
							}
							if (num15 == 3)
							{
								type = 178;
							}
							int damage = inventory[selectedItem].damage;
							int num23 = 8;
							if (num18 == 0)
							{
								num21 = 10f;
								num22 = 0f;
							}
							if (num18 == 1)
							{
								num21 = 7.5f;
								num22 = -2.5f;
							}
							if (num18 == 2)
							{
								num21 = 5f;
								num22 = -5f;
							}
							if (num18 == 3)
							{
								num21 = 2.75f;
								num22 = -6f;
							}
							if (num18 == 4)
							{
								num21 = 0f;
								num22 = -10f;
							}
							if (num18 == 5)
							{
								num21 = -2.75f;
								num22 = -6f;
							}
							if (num18 == 6)
							{
								num21 = -5f;
								num22 = -5f;
							}
							if (num18 == 7)
							{
								num21 = -7.5f;
								num22 = -2.5f;
							}
							if (num18 == 8)
							{
								num21 = -10f;
								num22 = 0f;
							}
							Vector2 vector = new Vector2((num16 + 2) * 16, (num19 + 2) * 16);
							float num24 = num21;
							float num25 = num22;
							float num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
							num26 = num20 / num26;
							num24 *= num26;
							num25 *= num26;
							Projectile.NewProjectile(vector.X, vector.Y, num24, num25, type, damage, num23, Main.myPlayer);
						}
					}
				}
			}
			if (inventory[selectedItem].type >= 1874 && inventory[selectedItem].type <= 1905 && Main.tile[tileTargetX, tileTargetY].active() && Main.tile[tileTargetX, tileTargetY].type == 171 && position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY && itemTime == 0 && itemAnimation > 0 && controlUseItem)
			{
				int type2 = inventory[selectedItem].type;
				if (type2 >= 1874 && type2 <= 1877)
				{
					type2 -= 1873;
					if (WorldGen.checkXmasTreeDrop(tileTargetX, tileTargetY, 0) != type2)
					{
						itemTime = inventory[selectedItem].useTime;
						WorldGen.dropXmasTree(tileTargetX, tileTargetY, 0);
						WorldGen.setXmasTree(tileTargetX, tileTargetY, 0, type2);
						int num27 = tileTargetX;
						int num28 = tileTargetY;
						if (Main.tile[tileTargetX, tileTargetY].frameX < 10)
						{
							num27 -= Main.tile[tileTargetX, tileTargetY].frameX;
							num28 -= Main.tile[tileTargetX, tileTargetY].frameY;
						}
						NetMessage.SendTileSquare(-1, num27, num28, 1);
					}
				}
				else if (type2 >= 1878 && type2 <= 1883)
				{
					type2 -= 1877;
					if (WorldGen.checkXmasTreeDrop(tileTargetX, tileTargetY, 1) != type2)
					{
						itemTime = inventory[selectedItem].useTime;
						WorldGen.dropXmasTree(tileTargetX, tileTargetY, 1);
						WorldGen.setXmasTree(tileTargetX, tileTargetY, 1, type2);
						int num29 = tileTargetX;
						int num30 = tileTargetY;
						if (Main.tile[tileTargetX, tileTargetY].frameX < 10)
						{
							num29 -= Main.tile[tileTargetX, tileTargetY].frameX;
							num30 -= Main.tile[tileTargetX, tileTargetY].frameY;
						}
						NetMessage.SendTileSquare(-1, num29, num30, 1);
					}
				}
				else if (type2 >= 1884 && type2 <= 1894)
				{
					type2 -= 1883;
					if (WorldGen.checkXmasTreeDrop(tileTargetX, tileTargetY, 2) != type2)
					{
						itemTime = inventory[selectedItem].useTime;
						WorldGen.dropXmasTree(tileTargetX, tileTargetY, 2);
						WorldGen.setXmasTree(tileTargetX, tileTargetY, 2, type2);
						int num31 = tileTargetX;
						int num32 = tileTargetY;
						if (Main.tile[tileTargetX, tileTargetY].frameX < 10)
						{
							num31 -= Main.tile[tileTargetX, tileTargetY].frameX;
							num32 -= Main.tile[tileTargetX, tileTargetY].frameY;
						}
						NetMessage.SendTileSquare(-1, num31, num32, 1);
					}
				}
				else if (type2 >= 1895 && type2 <= 1905)
				{
					type2 -= 1894;
					if (WorldGen.checkXmasTreeDrop(tileTargetX, tileTargetY, 3) != type2)
					{
						itemTime = inventory[selectedItem].useTime;
						WorldGen.dropXmasTree(tileTargetX, tileTargetY, 3);
						WorldGen.setXmasTree(tileTargetX, tileTargetY, 3, type2);
						int num33 = tileTargetX;
						int num34 = tileTargetY;
						if (Main.tile[tileTargetX, tileTargetY].frameX < 10)
						{
							num33 -= Main.tile[tileTargetX, tileTargetY].frameX;
							num34 -= Main.tile[tileTargetX, tileTargetY].frameY;
						}
						NetMessage.SendTileSquare(-1, num33, num34, 1);
					}
				}
			}
			if (ItemID.Sets.ExtractinatorMode[inventory[selectedItem].type] >= 0 && Main.tile[tileTargetX, tileTargetY].active() && Main.tile[tileTargetX, tileTargetY].type == 219)
			{
				if (position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY && itemTime == 0 && itemAnimation > 0 && controlUseItem)
				{
					itemTime = inventory[selectedItem].useTime;
					Main.PlaySound(7);
					int extractType = ItemID.Sets.ExtractinatorMode[inventory[selectedItem].type];
					ExtractinatorUse(extractType);
				}
			}
			else if (inventory[selectedItem].createTile >= 0 && position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetX && (position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f + (float)blockRange >= (float)tileTargetX && position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost - (float)blockRange <= (float)tileTargetY && (position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f + (float)blockRange >= (float)tileTargetY)
			{
				showItemIcon = true;
				bool flag = false;
				if (Main.tile[tileTargetX, tileTargetY].liquid > 0 && Main.tile[tileTargetX, tileTargetY].lava())
				{
					if (Main.tileSolid[inventory[selectedItem].createTile])
					{
						flag = true;
					}
					else if (!TileObjectData.CheckLiquidPlacement(inventory[selectedItem].createTile, inventory[selectedItem].placeStyle, Main.tile[tileTargetX, tileTargetY]))
					{
						flag = true;
					}
				}
				bool flag2 = true;
				if (inventory[selectedItem].tileWand > 0)
				{
					int tileWand = inventory[selectedItem].tileWand;
					flag2 = false;
					for (int k = 0; k < 58; k++)
					{
						if (tileWand == inventory[k].type && inventory[k].stack > 0)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (Main.tileRope[inventory[selectedItem].createTile] && flag2 && Main.tile[tileTargetX, tileTargetY].active() && Main.tileRope[Main.tile[tileTargetX, tileTargetY].type])
				{
					int num35 = tileTargetY;
					int num36 = tileTargetX;
					int createTile = inventory[selectedItem].createTile;
					while (Main.tile[num36, num35].active() && Main.tileRope[Main.tile[num36, num35].type] && num35 < Main.maxTilesX - 5 && Main.tile[num36, num35 + 2] != null && !Main.tile[num36, num35 + 1].lava())
					{
						num35++;
						if (Main.tile[num36, num35] == null)
						{
							flag2 = false;
							num35 = tileTargetY;
						}
					}
					if (!Main.tile[num36, num35].active())
					{
						tileTargetY = num35;
					}
				}
				if (flag2 && ((!Main.tile[tileTargetX, tileTargetY].active() && !flag) || Main.tileCut[Main.tile[tileTargetX, tileTargetY].type] || (Main.tile[tileTargetX, tileTargetY].type >= 373 && Main.tile[tileTargetX, tileTargetY].type <= 375) || inventory[selectedItem].createTile == 199 || inventory[selectedItem].createTile == 23 || inventory[selectedItem].createTile == 2 || inventory[selectedItem].createTile == 109 || inventory[selectedItem].createTile == 60 || inventory[selectedItem].createTile == 70 || TileID.Sets.BreakableWhenPlacing[Main.tile[tileTargetX, tileTargetY].type]) && itemTime == 0 && itemAnimation > 0 && controlUseItem)
				{
					bool flag3 = false;
					bool flag4 = false;
					TileObject objectData = default(TileObject);
					if (TileObjectData.CustomPlace(inventory[selectedItem].createTile, inventory[selectedItem].placeStyle) && inventory[selectedItem].createTile != 82)
					{
						flag4 = true;
						flag3 = TileObject.CanPlace(tileTargetX, tileTargetY, (ushort)inventory[selectedItem].createTile, inventory[selectedItem].placeStyle, direction, out objectData);
					}
					else
					{
						if (inventory[selectedItem].type == 213)
						{
							if (Main.tile[tileTargetX, tileTargetY].type == 0 || Main.tile[tileTargetX, tileTargetY].type == 1)
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 23 || inventory[selectedItem].createTile == 2 || inventory[selectedItem].createTile == 109 || inventory[selectedItem].createTile == 199)
						{
							if (Main.tile[tileTargetX, tileTargetY].nactive() && Main.tile[tileTargetX, tileTargetY].type == 0)
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 227)
						{
							flag3 = true;
						}
						else if (inventory[selectedItem].createTile >= 373 && inventory[selectedItem].createTile <= 375)
						{
							int num37 = tileTargetX;
							int num38 = tileTargetY - 1;
							if (Main.tile[num37, num38].nactive() && Main.tileSolid[Main.tile[num37, num38].type] && !Main.tileSolidTop[Main.tile[num37, num38].type])
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 60 || inventory[selectedItem].createTile == 70)
						{
							if (Main.tile[tileTargetX, tileTargetY].nactive() && Main.tile[tileTargetX, tileTargetY].type == 59)
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 4 || inventory[selectedItem].createTile == 136)
						{
							if (Main.tile[tileTargetX, tileTargetY].wall > 0)
							{
								flag3 = true;
							}
							else
							{
								if (!WorldGen.SolidTileNoAttach(tileTargetX, tileTargetY + 1) && !WorldGen.SolidTileNoAttach(tileTargetX - 1, tileTargetY) && !WorldGen.SolidTileNoAttach(tileTargetX + 1, tileTargetY))
								{
									if (!WorldGen.SolidTileNoAttach(tileTargetX, tileTargetY + 1) && (Main.tile[tileTargetX, tileTargetY + 1].halfBrick() || Main.tile[tileTargetX, tileTargetY + 1].slope() != 0))
									{
										if (Main.tile[tileTargetX, tileTargetY + 1].type != 19)
										{
											WorldGen.SlopeTile(tileTargetX, tileTargetY + 1);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY + 1);
											}
										}
									}
									else if (!WorldGen.SolidTileNoAttach(tileTargetX, tileTargetY + 1) && !WorldGen.SolidTileNoAttach(tileTargetX - 1, tileTargetY) && (Main.tile[tileTargetX - 1, tileTargetY].halfBrick() || Main.tile[tileTargetX - 1, tileTargetY].slope() != 0))
									{
										if (Main.tile[tileTargetX, tileTargetY + 1].type != 19)
										{
											WorldGen.SlopeTile(tileTargetX - 1, tileTargetY);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 14, tileTargetX - 1, tileTargetY);
											}
										}
									}
									else if (!WorldGen.SolidTileNoAttach(tileTargetX, tileTargetY + 1) && !WorldGen.SolidTileNoAttach(tileTargetX - 1, tileTargetY) && !WorldGen.SolidTileNoAttach(tileTargetX + 1, tileTargetY) && (Main.tile[tileTargetX + 1, tileTargetY].halfBrick() || Main.tile[tileTargetX + 1, tileTargetY].slope() != 0) && Main.tile[tileTargetX, tileTargetY + 1].type != 19)
									{
										WorldGen.SlopeTile(tileTargetX + 1, tileTargetY);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 14, tileTargetX + 1, tileTargetY);
										}
									}
								}
								int num39 = Main.tile[tileTargetX, tileTargetY + 1].type;
								if (Main.tile[tileTargetX, tileTargetY].halfBrick())
								{
									num39 = -1;
								}
								int num40 = Main.tile[tileTargetX - 1, tileTargetY].type;
								int num41 = Main.tile[tileTargetX + 1, tileTargetY].type;
								int num42 = Main.tile[tileTargetX - 1, tileTargetY - 1].type;
								int num43 = Main.tile[tileTargetX + 1, tileTargetY - 1].type;
								int num44 = Main.tile[tileTargetX - 1, tileTargetY - 1].type;
								int num45 = Main.tile[tileTargetX + 1, tileTargetY + 1].type;
								if (!Main.tile[tileTargetX, tileTargetY + 1].nactive())
								{
									num39 = -1;
								}
								if (!Main.tile[tileTargetX - 1, tileTargetY].nactive())
								{
									num40 = -1;
								}
								if (!Main.tile[tileTargetX + 1, tileTargetY].nactive())
								{
									num41 = -1;
								}
								if (!Main.tile[tileTargetX - 1, tileTargetY - 1].nactive())
								{
									num42 = -1;
								}
								if (!Main.tile[tileTargetX + 1, tileTargetY - 1].nactive())
								{
									num43 = -1;
								}
								if (!Main.tile[tileTargetX - 1, tileTargetY + 1].nactive())
								{
									num44 = -1;
								}
								if (!Main.tile[tileTargetX + 1, tileTargetY + 1].nactive())
								{
									num45 = -1;
								}
								if (num39 >= 0 && Main.tileSolid[num39] && (!Main.tileNoAttach[num39] || num39 == 19))
								{
									flag3 = true;
								}
								else if ((num40 >= 0 && Main.tileSolid[num40] && !Main.tileNoAttach[num40]) || (num40 == 5 && num42 == 5 && num44 == 5) || num40 == 124)
								{
									flag3 = true;
								}
								else if ((num41 >= 0 && Main.tileSolid[num41] && !Main.tileNoAttach[num41]) || (num41 == 5 && num43 == 5 && num45 == 5) || num41 == 124)
								{
									flag3 = true;
								}
							}
						}
						else if (inventory[selectedItem].createTile == 78 || inventory[selectedItem].createTile == 98 || inventory[selectedItem].createTile == 100 || inventory[selectedItem].createTile == 173 || inventory[selectedItem].createTile == 174 || inventory[selectedItem].createTile == 324)
						{
							if (Main.tile[tileTargetX, tileTargetY + 1].nactive() && (Main.tileSolid[Main.tile[tileTargetX, tileTargetY + 1].type] || Main.tileTable[Main.tile[tileTargetX, tileTargetY + 1].type]))
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 13 || inventory[selectedItem].createTile == 29 || inventory[selectedItem].createTile == 33 || inventory[selectedItem].createTile == 49 || inventory[selectedItem].createTile == 50 || inventory[selectedItem].createTile == 103)
						{
							if (Main.tile[tileTargetX, tileTargetY + 1].nactive() && Main.tileTable[Main.tile[tileTargetX, tileTargetY + 1].type])
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 275 || inventory[selectedItem].createTile == 276 || inventory[selectedItem].createTile == 277)
						{
							flag3 = true;
						}
						else if (inventory[selectedItem].createTile == 51 || inventory[selectedItem].createTile == 330 || inventory[selectedItem].createTile == 331 || inventory[selectedItem].createTile == 332 || inventory[selectedItem].createTile == 333 || inventory[selectedItem].createTile == 336 || inventory[selectedItem].createTile == 340 || inventory[selectedItem].createTile == 342 || inventory[selectedItem].createTile == 341 || inventory[selectedItem].createTile == 343 || inventory[selectedItem].createTile == 344 || inventory[selectedItem].createTile == 379 || inventory[selectedItem].createTile == 351)
						{
							if (Main.tile[tileTargetX + 1, tileTargetY].active() || Main.tile[tileTargetX + 1, tileTargetY].wall > 0 || Main.tile[tileTargetX - 1, tileTargetY].active() || Main.tile[tileTargetX - 1, tileTargetY].wall > 0 || Main.tile[tileTargetX, tileTargetY + 1].active() || Main.tile[tileTargetX, tileTargetY + 1].wall > 0 || Main.tile[tileTargetX, tileTargetY - 1].active() || Main.tile[tileTargetX, tileTargetY - 1].wall > 0)
							{
								flag3 = true;
							}
						}
						else if (inventory[selectedItem].createTile == 314)
						{
							for (int l = tileTargetX - 1; l <= tileTargetX + 1; l++)
							{
								for (int m = tileTargetY - 1; m <= tileTargetY + 1; m++)
								{
									Tile tile = Main.tile[l, m];
									if (tile.active() || tile.wall > 0)
									{
										flag3 = true;
										break;
									}
								}
							}
						}
						else
						{
							Tile tile2 = Main.tile[tileTargetX - 1, tileTargetY];
							Tile tile3 = Main.tile[tileTargetX + 1, tileTargetY];
							Tile tile4 = Main.tile[tileTargetX, tileTargetY - 1];
							Tile tile5 = Main.tile[tileTargetX, tileTargetY + 1];
							if ((tile3.active() && (Main.tileSolid[tile3.type] || Main.tileRope[tile3.type] || tile3.type == 314)) || tile3.wall > 0 || (tile2.active() && (Main.tileSolid[tile2.type] || Main.tileRope[tile2.type] || tile2.type == 314)) || tile2.wall > 0 || (tile5.active() && (Main.tileSolid[tile5.type] || tile5.type == 124 || Main.tileRope[tile5.type] || tile5.type == 314)) || tile5.wall > 0 || (tile4.active() && (Main.tileSolid[tile4.type] || tile4.type == 124 || Main.tileRope[tile4.type] || tile4.type == 314)) || tile4.wall > 0)
							{
								flag3 = true;
							}
						}
						if (inventory[selectedItem].type == 213 && Main.tile[tileTargetX, tileTargetY].active())
						{
							int num46 = tileTargetX;
							int num47 = tileTargetY;
							if (Main.tile[num46, num47].type == 3 || Main.tile[num46, num47].type == 73 || Main.tile[num46, num47].type == 84)
							{
								WorldGen.KillTile(tileTargetX, tileTargetY);
								if (!Main.tile[tileTargetX, tileTargetY].active() && Main.netMode == 1)
								{
									NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY);
								}
							}
							else if (Main.tile[num46, num47].type == 83)
							{
								bool flag5 = false;
								int num48 = Main.tile[num46, num47].frameX / 18;
								if (num48 == 0 && Main.dayTime)
								{
									flag5 = true;
								}
								if (num48 == 1 && !Main.dayTime)
								{
									flag5 = true;
								}
								if (num48 == 3 && !Main.dayTime && (Main.bloodMoon || Main.moonPhase == 0))
								{
									flag5 = true;
								}
								if (num48 == 4 && (Main.raining || Main.cloudAlpha > 0f))
								{
									flag5 = true;
								}
								if (num48 == 5 && !Main.raining && Main.dayTime && Main.time > 40500.0)
								{
									flag5 = true;
								}
								if (flag5)
								{
									WorldGen.KillTile(tileTargetX, tileTargetY);
									NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY);
								}
							}
						}
						if (Main.tileAlch[inventory[selectedItem].createTile])
						{
							flag3 = true;
						}
						if (Main.tile[tileTargetX, tileTargetY].active() && (Main.tileCut[Main.tile[tileTargetX, tileTargetY].type] || TileID.Sets.BreakableWhenPlacing[Main.tile[tileTargetX, tileTargetY].type] || (Main.tile[tileTargetX, tileTargetY].type >= 373 && Main.tile[tileTargetX, tileTargetY].type <= 375)))
						{
							if (Main.tile[tileTargetX, tileTargetY].type != inventory[selectedItem].createTile)
							{
								if ((Main.tile[tileTargetX, tileTargetY + 1].type != 78 && Main.tile[tileTargetX, tileTargetY + 1].type != 380) || ((Main.tile[tileTargetX, tileTargetY].type == 3 || Main.tile[tileTargetX, tileTargetY].type == 73) && Main.tileAlch[inventory[selectedItem].createTile]))
								{
									WorldGen.KillTile(tileTargetX, tileTargetY);
									if (!Main.tile[tileTargetX, tileTargetY].active() && Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 4, tileTargetX, tileTargetY);
									}
								}
								else
								{
									flag3 = false;
								}
							}
							else
							{
								flag3 = false;
							}
						}
						if (!flag3 && inventory[selectedItem].createTile == 19)
						{
							for (int n = tileTargetX - 1; n <= tileTargetX + 1; n++)
							{
								for (int num49 = tileTargetY - 1; num49 <= tileTargetY + 1; num49++)
								{
									if (Main.tile[n, num49].active())
									{
										flag3 = true;
										break;
									}
								}
							}
						}
					}
					if (flag3)
					{
						int num50 = inventory[selectedItem].placeStyle;
						if (!flag4)
						{
							if (inventory[selectedItem].createTile == 36)
							{
								num50 = Main.rand.Next(7);
							}
							if (inventory[selectedItem].createTile == 212 && direction > 0)
							{
								num50 = 1;
							}
							if (inventory[selectedItem].createTile == 141)
							{
								num50 = Main.rand.Next(2);
							}
							if (inventory[selectedItem].createTile == 128 || inventory[selectedItem].createTile == 269 || inventory[selectedItem].createTile == 334)
							{
								num50 = ((direction >= 0) ? 1 : (-1));
							}
							if (inventory[selectedItem].createTile == 241 && inventory[selectedItem].placeStyle == 0)
							{
								num50 = Main.rand.Next(0, 9);
							}
							if (inventory[selectedItem].createTile == 35 && inventory[selectedItem].placeStyle == 0)
							{
								num50 = Main.rand.Next(9);
							}
						}
						if (inventory[selectedItem].createTile == 314 && num50 == 2 && direction == 1)
						{
							num50++;
						}
						int[,] array = new int[11, 11];
						if (autoPaint)
						{
							for (int num51 = 0; num51 < 11; num51++)
							{
								for (int num52 = 0; num52 < 11; num52++)
								{
									int num53 = tileTargetX - 5 + num51;
									int num54 = tileTargetY - 5 + num52;
									if (Main.tile[num53, num54].active())
									{
										array[num51, num52] = Main.tile[num53, num54].type;
									}
									else
									{
										array[num51, num52] = -1;
									}
								}
							}
						}
						bool forced = false;
						bool flag6;
						if (flag4)
						{
							flag6 = TileObject.Place(objectData);
							WorldGen.SquareTileFrame(tileTargetX, tileTargetY);
							Main.PlaySound(0, tileTargetX * 16, tileTargetY * 16);
						}
						else
						{
							flag6 = WorldGen.PlaceTile(tileTargetX, tileTargetY, inventory[selectedItem].createTile, false, forced, whoAmI, num50);
						}
						if (inventory[selectedItem].type == 213 && !flag6 && Main.tile[tileTargetX, tileTargetY].type == 1 && Main.tile[tileTargetX, tileTargetY].active())
						{
							int num55 = 0;
							int num56 = 0;
							Point point = base.Center.ToTileCoordinates();
							Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
							WorldUtils.Gen(new Point(point.X - 25, point.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(182, 180, 179, 183, 181, 381).Output(dictionary));
							foreach (KeyValuePair<ushort, int> item in dictionary)
							{
								if (item.Value > num56)
								{
									num56 = item.Value;
									num55 = item.Key;
								}
							}
							if (num56 == 0)
							{
								num55 = Utils.SelectRandom<int>(Main.rand, 182, 180, 179, 183, 181);
							}
							if (num55 != 0)
							{
								Main.tile[tileTargetX, tileTargetY].type = (ushort)num55;
								WorldGen.SquareTileFrame(tileTargetX, tileTargetY);
								NetMessage.SendTileSquare(-1, tileTargetX, tileTargetY, 1);
								flag6 = true;
							}
						}
						if (flag6)
						{
							itemTime = (int)((float)inventory[selectedItem].useTime * tileSpeed);
							if (flag4)
							{
								TileObjectData.CallPostPlacementPlayerHook(tileTargetX, tileTargetY, inventory[selectedItem].createTile, num50, direction, objectData);
								if (Main.netMode == 1 && !Main.tileContainer[inventory[selectedItem].createTile])
								{
									NetMessage.SendObjectPlacment(-1, tileTargetX, tileTargetY, objectData.type, objectData.style, objectData.alternate, objectData.random, direction);
								}
							}
							else
							{
								NetMessage.SendData(17, -1, -1, "", 1, tileTargetX, tileTargetY, inventory[selectedItem].createTile, num50);
								if (inventory[selectedItem].createTile == 15)
								{
									if (direction == 1)
									{
										Main.tile[tileTargetX, tileTargetY].frameX += 18;
										Main.tile[tileTargetX, tileTargetY - 1].frameX += 18;
									}
									if (Main.netMode == 1)
									{
										NetMessage.SendTileSquare(-1, tileTargetX - 1, tileTargetY - 1, 3);
									}
								}
								else if ((inventory[selectedItem].createTile == 79 || inventory[selectedItem].createTile == 90) && Main.netMode == 1)
								{
									NetMessage.SendTileSquare(-1, tileTargetX, tileTargetY, 5);
								}
							}
							if (inventory[selectedItem].createTile == 137)
							{
								if (direction == 1)
								{
									Main.tile[tileTargetX, tileTargetY].frameX += 18;
								}
								if (Main.netMode == 1)
								{
									NetMessage.SendTileSquare(-1, tileTargetX, tileTargetY, 1);
								}
							}
							if (inventory[selectedItem].createTile == 19 && Main.smartDigEnabled)
							{
								int num57 = tileTargetX;
								int num58 = tileTargetY;
								int num59 = -1;
								int num60 = 0;
								int num61 = 0;
								bool flag7 = true;
								for (int num62 = -1; num62 < 2; num62++)
								{
									for (int num63 = -1; num63 < 2; num63++)
									{
										if ((num62 != 0 || num63 != 0) && Main.tile[num57 + num62, num58 + num63].type == 19)
										{
											flag7 = false;
										}
									}
								}
								if (!flag7)
								{
									Tile tile6 = Main.tile[num57 - 1, num58 - 1];
									if (tile6.active() && tile6.type == 19 && tile6.slope() != 2)
									{
										num60++;
									}
									tile6 = Main.tile[num57 - 1, num58 + 1];
									if (tile6.active() && tile6.type == 19 && tile6.slope() != 1)
									{
										num61++;
									}
									tile6 = Main.tile[num57 + 1, num58 - 1];
									if (tile6.active() && tile6.type == 19 && tile6.slope() != 1)
									{
										num61++;
									}
									tile6 = Main.tile[num57 + 1, num58 + 1];
									if (tile6.active() && tile6.type == 19 && tile6.slope() != 2)
									{
										num60++;
									}
									tile6 = Main.tile[num57 - 1, num58];
									if (WorldGen.SolidTile(tile6))
									{
										num60++;
										if (tile6.type == 19 && tile6.slope() == 0)
										{
											num60++;
										}
									}
									tile6 = Main.tile[num57 + 1, num58];
									if (WorldGen.SolidTile(tile6))
									{
										num61++;
										if (tile6.type == 19 && tile6.slope() == 0)
										{
											num61++;
										}
									}
									if (num60 > num61)
									{
										num59 = 1;
									}
									else if (num61 > num60)
									{
										num59 = 2;
									}
									tile6 = Main.tile[num57 - 1, num58];
									if (tile6.active() && tile6.type == 19)
									{
										num59 = 0;
									}
									tile6 = Main.tile[num57 + 1, num58];
									if (tile6.active() && tile6.type == 19)
									{
										num59 = 0;
									}
									int num64 = 0;
									int num65 = 0;
									if (num59 != -1)
									{
										WorldGen.SlopeTile(num57, num58, num59);
										int num66 = Main.tile[num57, num58].slope();
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY, num66);
										}
										if (num59 == 1)
										{
											num64 = -1;
											num65 = -1;
										}
										else
										{
											num64 = 1;
											num65 = -1;
										}
										tile6 = Main.tile[num57 + num64, num58 + num65];
										if (tile6.active() && tile6.type == 19 && tile6.slope() == 0 && (!Main.tile[num57 + num64 + num64, num58 + num65].active() || Main.tile[num57 + num64 + num64, num58 + num65].type != 19 || !Main.tile[num57 + num64 + num64, num58 + num65].halfBrick()))
										{
											WorldGen.SlopeTile(num57 + num64, num58 + num65, num59);
											num66 = tile6.slope();
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 14, num57 + num64, num58 + num65, num66);
											}
										}
										if (num59 == 1)
										{
											num64 = 1;
											num65 = 1;
										}
										else
										{
											num64 = -1;
											num65 = 1;
										}
										tile6 = Main.tile[num57 + num64, num58 + num65];
										if (tile6.active() && tile6.type == 19 && tile6.slope() == 0 && WorldGen.PlatformProperSides(num57 + num64, num58 + num65, true) <= 0)
										{
											WorldGen.SlopeTile(num57 + num64, num58 + num65, num59);
											num66 = tile6.slope();
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 14, num57 + num64, num58 + num65, num66);
											}
										}
									}
									else
									{
										num59 = 0;
										int num67 = 0;
										num64 = -1;
										tile6 = Main.tile[num57 + num64, num58];
										if (tile6.active() && tile6.type == 19 && tile6.slope() != 0)
										{
											int num68 = (tile6.slope() == 1).ToDirectionInt() * num64;
											num59 = ((num68 != -1) ? tile6.slope() : 0);
											bool flag8 = true;
											if (Main.tile[num57 + num64 * 2, num58 + num68].active() && Main.tile[num57 + num64 * 2, num58].type == 19 && num59 == Main.tile[num57 + num64 * 2, num58 + num68].slope())
											{
												flag8 = false;
											}
											if (Main.tile[num57, num58 - num68].active() && Main.tile[num57, num58 - num68].type == 19 && tile6.slope() == Main.tile[num57, num58 - num68].slope())
											{
												flag8 = false;
											}
											if (flag8)
											{
												WorldGen.SlopeTile(num57 + num64, num58, num59);
												num67 = tile6.slope();
												if (Main.netMode == 1)
												{
													NetMessage.SendData(17, -1, -1, "", 14, num57 + num64, num58, num67);
												}
											}
										}
										num64 = 1;
										num65 = 0;
										tile6 = Main.tile[num57 + num64, num58 + num65];
										if (tile6.active() && tile6.type == 19 && tile6.slope() != 0)
										{
											int num69 = (tile6.slope() == 1).ToDirectionInt() * num64;
											num59 = ((num69 != -1) ? tile6.slope() : 0);
											bool flag9 = true;
											if (Main.tile[num57 + num64 * 2, num58 + num69].active() && Main.tile[num57 + num64 * 2, num58].type == 19 && num59 == Main.tile[num57 + num64 * 2, num58 + num69].slope())
											{
												flag9 = false;
											}
											if (Main.tile[num57, num58 - num69].active() && Main.tile[num57, num58 - num69].type == 19 && tile6.slope() == Main.tile[num57, num58 - num69].slope())
											{
												flag9 = false;
											}
											if (flag9)
											{
												WorldGen.SlopeTile(num57 + num64, num58, num59);
												num67 = tile6.slope();
												if (Main.netMode == 1)
												{
													NetMessage.SendData(17, -1, -1, "", 14, num57 + num64, num58, num67);
												}
											}
										}
										if (num60 == num61 && WorldGen.PlatformProperSides(num57, num58) == 0)
										{
											tile6 = Main.tile[num57, num58 + 1];
											if (tile6.active() && !tile6.halfBrick() && tile6.slope() == 0 && Main.tileSolid[tile6.type])
											{
												num59 = ((direction != 1) ? 1 : 2);
												WorldGen.SlopeTile(num57, num58, num59);
												num67 = Main.tile[num57, num58].slope();
												if (Main.netMode == 1)
												{
													NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY, num67);
												}
											}
										}
									}
								}
							}
							if (Main.tileSolid[inventory[selectedItem].createTile] && inventory[selectedItem].createTile != 19)
							{
								int num70 = tileTargetX;
								int num71 = tileTargetY + 1;
								if (Main.tile[num70, num71] != null && Main.tile[num70, num71].type != 19 && (Main.tile[num70, num71].topSlope() || Main.tile[num70, num71].halfBrick()))
								{
									WorldGen.SlopeTile(num70, num71);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 14, num70, num71);
									}
								}
								num70 = tileTargetX;
								num71 = tileTargetY - 1;
								if (Main.tile[num70, num71] != null && Main.tile[num70, num71].type != 19 && Main.tile[num70, num71].bottomSlope())
								{
									WorldGen.SlopeTile(num70, num71);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 14, num70, num71);
									}
								}
							}
							if (Main.tileSolid[inventory[selectedItem].createTile])
							{
								for (int num72 = tileTargetX - 1; num72 <= tileTargetX + 1; num72++)
								{
									for (int num73 = tileTargetY - 1; num73 <= tileTargetY + 1; num73++)
									{
										if (!Main.tile[num72, num73].active() || inventory[selectedItem].createTile == Main.tile[num72, num73].type || (Main.tile[num72, num73].type != 2 && Main.tile[num72, num73].type != 23 && Main.tile[num72, num73].type != 60 && Main.tile[num72, num73].type != 70 && Main.tile[num72, num73].type != 109 && Main.tile[num72, num73].type != 199))
										{
											continue;
										}
										bool flag10 = true;
										for (int num74 = num72 - 1; num74 <= num72 + 1; num74++)
										{
											for (int num75 = num73 - 1; num75 <= num73 + 1; num75++)
											{
												if (!WorldGen.SolidTile(num74, num75))
												{
													flag10 = false;
												}
											}
										}
										if (flag10)
										{
											WorldGen.KillTile(num72, num73, true);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 0, num72, num73, 1f);
											}
										}
									}
								}
							}
							if (autoPaint)
							{
								for (int num76 = 0; num76 < 11; num76++)
								{
									for (int num77 = 0; num77 < 11; num77++)
									{
										int num78 = tileTargetX - 5 + num76;
										int num79 = tileTargetY - 5 + num77;
										if ((!Main.tile[num78, num79].active() && array[num76, num77] == -1) || (Main.tile[num78, num79].active() && array[num76, num77] == Main.tile[num78, num79].type))
										{
											continue;
										}
										int num80 = -1;
										int num81 = -1;
										for (int num82 = 0; num82 < 58; num82++)
										{
											if (inventory[num82].stack > 0 && inventory[num82].paint > 0)
											{
												num80 = inventory[num82].paint;
												num81 = num82;
												break;
											}
										}
										if (num80 > 0 && Main.tile[num78, num79].color() != num80 && WorldGen.paintTile(num78, num79, (byte)num80, true))
										{
											int num83 = num81;
											inventory[num83].stack--;
											if (inventory[num83].stack <= 0)
											{
												inventory[num83].SetDefaults();
											}
											itemTime = (int)((float)inventory[selectedItem].useTime * tileSpeed);
										}
									}
								}
							}
						}
					}
				}
			}
			if (inventory[selectedItem].createWall < 0 || !(position.X / 16f - (float)tileRangeX - (float)inventory[selectedItem].tileBoost <= (float)tileTargetX) || !((position.X + (float)width) / 16f + (float)tileRangeX + (float)inventory[selectedItem].tileBoost - 1f >= (float)tileTargetX) || !(position.Y / 16f - (float)tileRangeY - (float)inventory[selectedItem].tileBoost <= (float)tileTargetY) || !((position.Y + (float)height) / 16f + (float)tileRangeY + (float)inventory[selectedItem].tileBoost - 2f >= (float)tileTargetY))
			{
				return;
			}
			showItemIcon = true;
			if (itemTime != 0 || itemAnimation <= 0 || !controlUseItem || (!Main.tile[tileTargetX + 1, tileTargetY].active() && Main.tile[tileTargetX + 1, tileTargetY].wall <= 0 && !Main.tile[tileTargetX - 1, tileTargetY].active() && Main.tile[tileTargetX - 1, tileTargetY].wall <= 0 && !Main.tile[tileTargetX, tileTargetY + 1].active() && Main.tile[tileTargetX, tileTargetY + 1].wall <= 0 && !Main.tile[tileTargetX, tileTargetY - 1].active() && Main.tile[tileTargetX, tileTargetY - 1].wall <= 0) || Main.tile[tileTargetX, tileTargetY].wall == inventory[selectedItem].createWall)
			{
				return;
			}
			if (SmartCursorSettings.SmartWallReplacement && Main.tile[tileTargetX, tileTargetY].wall != 0 && WorldGen.NearFriendlyWall(tileTargetX, tileTargetY))
			{
				WorldGen.KillWall(tileTargetX, tileTargetY);
				if (Main.tile[tileTargetX, tileTargetY].wall == 0 && Main.netMode == 1)
				{
					NetMessage.SendData(17, -1, -1, "", 2, tileTargetX, tileTargetY);
				}
				if (inventory[selectedItem].consumable)
				{
					inventory[selectedItem].stack++;
				}
				itemTime = (int)((float)inventory[selectedItem].useTime * wallSpeed);
				return;
			}
			WorldGen.PlaceWall(tileTargetX, tileTargetY, inventory[selectedItem].createWall);
			if (Main.tile[tileTargetX, tileTargetY].wall != inventory[selectedItem].createWall)
			{
				return;
			}
			itemTime = (int)((float)inventory[selectedItem].useTime * wallSpeed);
			if (Main.netMode == 1)
			{
				NetMessage.SendData(17, -1, -1, "", 3, tileTargetX, tileTargetY, inventory[selectedItem].createWall);
			}
			if (inventory[selectedItem].stack > 1)
			{
				int createWall = inventory[selectedItem].createWall;
				for (int num84 = 0; num84 < 4; num84++)
				{
					int num85 = tileTargetX;
					int num86 = tileTargetY;
					if (num84 == 0)
					{
						num85--;
					}
					if (num84 == 1)
					{
						num85++;
					}
					if (num84 == 2)
					{
						num86--;
					}
					if (num84 == 3)
					{
						num86++;
					}
					if (Main.tile[num85, num86].wall != 0)
					{
						continue;
					}
					int num87 = 0;
					for (int num88 = 0; num88 < 4; num88++)
					{
						int num89 = num85;
						int num90 = num86;
						if (num88 == 0)
						{
							num89--;
						}
						if (num88 == 1)
						{
							num89++;
						}
						if (num88 == 2)
						{
							num90--;
						}
						if (num88 == 3)
						{
							num90++;
						}
						if (Main.tile[num89, num90].wall == createWall)
						{
							num87++;
						}
					}
					if (num87 != 4)
					{
						continue;
					}
					WorldGen.PlaceWall(num85, num86, createWall);
					if (Main.tile[num85, num86].wall != createWall)
					{
						continue;
					}
					inventory[selectedItem].stack--;
					if (inventory[selectedItem].stack == 0)
					{
						inventory[selectedItem].SetDefaults();
					}
					if (Main.netMode == 1)
					{
						NetMessage.SendData(17, -1, -1, "", 3, num85, num86, createWall);
					}
					if (!autoPaint)
					{
						continue;
					}
					int num91 = num85;
					int num92 = num86;
					int num93 = -1;
					int num94 = -1;
					for (int num95 = 0; num95 < 58; num95++)
					{
						if (inventory[num95].stack > 0 && inventory[num95].paint > 0)
						{
							num93 = inventory[num95].paint;
							num94 = num95;
							break;
						}
					}
					if (num93 > 0 && Main.tile[num91, num92].wallColor() != num93 && WorldGen.paintWall(num91, num92, (byte)num93, true))
					{
						int num96 = num94;
						inventory[num96].stack--;
						if (inventory[num96].stack <= 0)
						{
							inventory[num96].SetDefaults();
						}
						itemTime = (int)((float)inventory[selectedItem].useTime * wallSpeed);
					}
				}
			}
			if (!autoPaint)
			{
				return;
			}
			int num97 = tileTargetX;
			int num98 = tileTargetY;
			int num99 = -1;
			int num100 = -1;
			for (int num101 = 0; num101 < 58; num101++)
			{
				if (inventory[num101].stack > 0 && inventory[num101].paint > 0)
				{
					num99 = inventory[num101].paint;
					num100 = num101;
					break;
				}
			}
			if (num99 > 0 && Main.tile[num97, num98].wallColor() != num99 && WorldGen.paintWall(num97, num98, (byte)num99, true))
			{
				int num102 = num100;
				inventory[num102].stack--;
				if (inventory[num102].stack <= 0)
				{
					inventory[num102].SetDefaults();
				}
				itemTime = (int)((float)inventory[selectedItem].useTime * wallSpeed);
			}
		}

		private static void ExtractinatorUse(int extractType)
		{
			int num = 5000;
			int num2 = 25;
			int num3 = 50;
			int num4 = -1;
			if (extractType == 1)
			{
				num /= 3;
				num2 *= 2;
				num3 /= 2;
				num4 = 10;
			}
			int num5 = -1;
			int num6 = 1;
			if (num4 != -1 && Main.rand.Next(num4) == 0)
			{
				num5 = 3380;
				if (Main.rand.Next(5) == 0)
				{
					num6 += Main.rand.Next(2);
				}
				if (Main.rand.Next(10) == 0)
				{
					num6 += Main.rand.Next(3);
				}
				if (Main.rand.Next(15) == 0)
				{
					num6 += Main.rand.Next(4);
				}
			}
			else if (Main.rand.Next(2) == 0)
			{
				if (Main.rand.Next(12000) == 0)
				{
					num5 = 74;
					if (Main.rand.Next(14) == 0)
					{
						num6 += Main.rand.Next(0, 2);
					}
					if (Main.rand.Next(14) == 0)
					{
						num6 += Main.rand.Next(0, 2);
					}
					if (Main.rand.Next(14) == 0)
					{
						num6 += Main.rand.Next(0, 2);
					}
				}
				else if (Main.rand.Next(800) == 0)
				{
					num5 = 73;
					if (Main.rand.Next(6) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0)
					{
						num6 += Main.rand.Next(1, 20);
					}
				}
				else if (Main.rand.Next(60) == 0)
				{
					num5 = 72;
					if (Main.rand.Next(4) == 0)
					{
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(4) == 0)
					{
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(4) == 0)
					{
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(4) == 0)
					{
						num6 += Main.rand.Next(5, 25);
					}
				}
				else
				{
					num5 = 71;
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(10, 25);
					}
				}
			}
			else if (num != -1 && Main.rand.Next(num) == 0)
			{
				num5 = 1242;
			}
			else if (num2 != -1 && Main.rand.Next(num2) == 0)
			{
				switch (Main.rand.Next(6))
				{
				case 0:
					num5 = 181;
					break;
				case 1:
					num5 = 180;
					break;
				case 2:
					num5 = 177;
					break;
				case 3:
					num5 = 179;
					break;
				case 4:
					num5 = 178;
					break;
				default:
					num5 = 182;
					break;
				}
				if (Main.rand.Next(20) == 0)
				{
					num6 += Main.rand.Next(0, 2);
				}
				if (Main.rand.Next(30) == 0)
				{
					num6 += Main.rand.Next(0, 3);
				}
				if (Main.rand.Next(40) == 0)
				{
					num6 += Main.rand.Next(0, 4);
				}
				if (Main.rand.Next(50) == 0)
				{
					num6 += Main.rand.Next(0, 5);
				}
				if (Main.rand.Next(60) == 0)
				{
					num6 += Main.rand.Next(0, 6);
				}
			}
			else if (num3 != -1 && Main.rand.Next(num3) == 0)
			{
				num5 = 999;
				if (Main.rand.Next(20) == 0)
				{
					num6 += Main.rand.Next(0, 2);
				}
				if (Main.rand.Next(30) == 0)
				{
					num6 += Main.rand.Next(0, 3);
				}
				if (Main.rand.Next(40) == 0)
				{
					num6 += Main.rand.Next(0, 4);
				}
				if (Main.rand.Next(50) == 0)
				{
					num6 += Main.rand.Next(0, 5);
				}
				if (Main.rand.Next(60) == 0)
				{
					num6 += Main.rand.Next(0, 6);
				}
			}
			else if (Main.rand.Next(3) == 0)
			{
				if (Main.rand.Next(5000) == 0)
				{
					num5 = 74;
					if (Main.rand.Next(10) == 0)
					{
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0)
					{
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0)
					{
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0)
					{
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0)
					{
						num6 += Main.rand.Next(0, 3);
					}
				}
				else if (Main.rand.Next(400) == 0)
				{
					num5 = 73;
					if (Main.rand.Next(5) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0)
					{
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0)
					{
						num6 += Main.rand.Next(1, 20);
					}
				}
				else if (Main.rand.Next(30) == 0)
				{
					num5 = 72;
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(3) == 0)
					{
						num6 += Main.rand.Next(5, 25);
					}
				}
				else
				{
					num5 = 71;
					if (Main.rand.Next(2) == 0)
					{
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(2) == 0)
					{
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(2) == 0)
					{
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(2) == 0)
					{
						num6 += Main.rand.Next(10, 25);
					}
				}
			}
			else
			{
				switch (Main.rand.Next(8))
				{
				case 0:
					num5 = 12;
					break;
				case 1:
					num5 = 11;
					break;
				case 2:
					num5 = 14;
					break;
				case 3:
					num5 = 13;
					break;
				case 4:
					num5 = 699;
					break;
				case 5:
					num5 = 700;
					break;
				case 6:
					num5 = 701;
					break;
				default:
					num5 = 702;
					break;
				}
				if (Main.rand.Next(20) == 0)
				{
					num6 += Main.rand.Next(0, 2);
				}
				if (Main.rand.Next(30) == 0)
				{
					num6 += Main.rand.Next(0, 3);
				}
				if (Main.rand.Next(40) == 0)
				{
					num6 += Main.rand.Next(0, 4);
				}
				if (Main.rand.Next(50) == 0)
				{
					num6 += Main.rand.Next(0, 5);
				}
				if (Main.rand.Next(60) == 0)
				{
					num6 += Main.rand.Next(0, 6);
				}
			}
			if (num5 > 0)
			{
				Vector2 vector = Main.ReverseGravitySupport(Main.MouseScreen) + Main.screenPosition;
				int number = Item.NewItem((int)vector.X, (int)vector.Y, 1, 1, num5, num6, false, -1);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
			}
		}

		public void ChangeDir(int dir)
		{
			if (!pulley || pulleyDir != 2)
			{
				direction = dir;
			}
			else
			{
				if (pulleyDir == 2 && dir == direction)
				{
					return;
				}
				int num = (int)(position.X + (float)(width / 2)) / 16;
				int num2 = num * 16 + 8 - width / 2;
				if (!Collision.SolidCollision(new Vector2(num2, position.Y), width, height))
				{
					if (whoAmI == Main.myPlayer)
					{
						Main.cameraX = Main.cameraX + position.X - (float)num2;
					}
					pulleyDir = 1;
					position.X = num2;
					direction = dir;
				}
			}
		}

		public Rectangle getRect()
		{
			return new Rectangle((int)position.X, (int)position.Y, width, height);
		}

		private void pumpkinSword(int i, int dmg, float kb)
		{
			int num = Main.rand.Next(100, 300);
			int num2 = Main.rand.Next(100, 300);
			num = ((Main.rand.Next(2) != 0) ? (num + (Main.maxScreenW / 2 - num)) : (num - (Main.maxScreenW / 2 + num)));
			num2 = ((Main.rand.Next(2) != 0) ? (num2 + (Main.maxScreenH / 2 - num2)) : (num2 - (Main.maxScreenH / 2 + num2)));
			num += (int)position.X;
			num2 += (int)position.Y;
			float num3 = 8f;
			Vector2 vector = new Vector2(num, num2);
			float num4 = Main.npc[i].position.X - vector.X;
			float num5 = Main.npc[i].position.Y - vector.Y;
			float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
			num6 = num3 / num6;
			num4 *= num6;
			num5 *= num6;
			Projectile.NewProjectile(num, num2, num4, num5, 321, dmg, kb, whoAmI, i);
		}

		public void PutItemInInventory(int type, int selItem = -1)
		{
			for (int i = 0; i < 58; i++)
			{
				Item ıtem = inventory[i];
				if (ıtem.stack > 0 && ıtem.type == type && ıtem.stack < ıtem.maxStack)
				{
					ıtem.stack++;
					return;
				}
			}
			if (selItem >= 0 && (inventory[selItem].type == 0 || inventory[selItem].stack <= 0))
			{
				inventory[selItem].SetDefaults(type);
				return;
			}
			Item ıtem2 = new Item();
			ıtem2.SetDefaults(type);
			Item ıtem3 = GetItem(whoAmI, ıtem2);
			if (ıtem3.stack > 0)
			{
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, type, 1, false, 0, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
			}
			else
			{
				ıtem2.position.X = base.Center.X - (float)(ıtem2.width / 2);
				ıtem2.position.Y = base.Center.Y - (float)(ıtem2.height / 2);
				ıtem2.active = true;
				ItemText.NewText(ıtem2, 0);
			}
		}

		public bool SummonItemCheck()
		{
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && ((inventory[selectedItem].type == 43 && Main.npc[i].type == 4) || (inventory[selectedItem].type == 70 && Main.npc[i].type == 13) || ((inventory[selectedItem].type == 560) & (Main.npc[i].type == 50)) || (inventory[selectedItem].type == 544 && Main.npc[i].type == 125) || (inventory[selectedItem].type == 544 && Main.npc[i].type == 126) || (inventory[selectedItem].type == 556 && Main.npc[i].type == 134) || (inventory[selectedItem].type == 557 && Main.npc[i].type == 127) || (inventory[selectedItem].type == 1133 && Main.npc[i].type == 222) || (inventory[selectedItem].type == 1331 && Main.npc[i].type == 266)))
				{
					return false;
				}
			}
			return true;
		}

		public int FishingLevel()
		{
			int num = 0;
			int fishingPole = inventory[selectedItem].fishingPole;
			if (fishingPole == 0)
			{
				for (int i = 0; i < 58; i++)
				{
					if (inventory[i].fishingPole > fishingPole)
					{
						fishingPole = inventory[i].fishingPole;
					}
				}
			}
			for (int j = 0; j < 58; j++)
			{
				if (inventory[j].stack > 0 && inventory[j].bait > 0)
				{
					if (inventory[j].type == 2673)
					{
						return -1;
					}
					num = inventory[j].bait;
					break;
				}
			}
			if (num == 0 || fishingPole == 0)
			{
				return 0;
			}
			int num2 = num + fishingPole + fishingSkill;
			if (Main.raining)
			{
				num2 = (int)((float)num2 * 1.2f);
			}
			if (Main.cloudBGAlpha > 0f)
			{
				num2 = (int)((float)num2 * 1.1f);
			}
			if (Main.dayTime && (Main.time < 5400.0 || Main.time > 48600.0))
			{
				num2 = (int)((float)num2 * 1.3f);
			}
			if (Main.dayTime && Main.time > 16200.0 && Main.time < 37800.0)
			{
				num2 = (int)((float)num2 * 0.8f);
			}
			if (!Main.dayTime && Main.time > 6480.0 && Main.time < 25920.0)
			{
				num2 = (int)((float)num2 * 0.8f);
			}
			if (Main.moonPhase == 0)
			{
				num2 = (int)((float)num2 * 1.1f);
			}
			if (Main.moonPhase == 1 || Main.moonPhase == 7)
			{
				num2 = (int)((float)num2 * 1.05f);
			}
			if (Main.moonPhase == 3 || Main.moonPhase == 5)
			{
				num2 = (int)((float)num2 * 0.95f);
			}
			if (Main.moonPhase == 4)
			{
				num2 = (int)((float)num2 * 0.9f);
			}
			return num2;
		}

		public bool HasUnityPotion()
		{
			for (int i = 0; i < 58; i++)
			{
				if (inventory[i].type == 2997 && inventory[i].stack > 0)
				{
					return true;
				}
			}
			return false;
		}

		public void TakeUnityPotion()
		{
			int num = 0;
			while (true)
			{
				if (num < 400)
				{
					if (inventory[num].type == 2997 && inventory[num].stack > 0)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			inventory[num].stack--;
			if (inventory[num].stack <= 0)
			{
				inventory[num].SetDefaults();
			}
		}

		public void UnityTeleport(Vector2 telePos)
		{
			int num = 3;
			if (Main.netMode == 0)
			{
				Teleport(telePos, num);
			}
			else
			{
				NetMessage.SendData(65, -1, -1, "", 2, whoAmI, telePos.X, telePos.Y, num);
			}
		}

		private void SporeSac()
		{
			int damage = 70;
			float knockBack = 1.5f;
			if (Main.rand.Next(15) != 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == whoAmI && (Main.projectile[i].type == 567 || Main.projectile[i].type == 568))
				{
					num++;
				}
			}
			if (Main.rand.Next(15) < num || num >= 10)
			{
				return;
			}
			int num2 = 50;
			int num3 = 24;
			int num4 = 90;
			int num5 = 0;
			Vector2 center;
			while (true)
			{
				if (num5 >= num2)
				{
					return;
				}
				int num6 = Main.rand.Next(200 - num5 * 2, 400 + num5 * 2);
				center = base.Center;
				center.X += Main.rand.Next(-num6, num6 + 1);
				center.Y += Main.rand.Next(-num6, num6 + 1);
				if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3))
				{
					center.X += num3 / 2;
					center.Y += num3 / 2;
					if (Collision.CanHit(new Vector2(base.Center.X, position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(base.Center.X, position.Y - 50f), 1, 1, center, 1, 1))
					{
						int num7 = (int)center.X / 16;
						int num8 = (int)center.Y / 16;
						bool flag = false;
						if (Main.rand.Next(3) == 0 && Main.tile[num7, num8] != null && Main.tile[num7, num8].wall > 0)
						{
							flag = true;
						}
						else
						{
							center.X -= num4 / 2;
							center.Y -= num4 / 2;
							if (Collision.SolidCollision(center, num4, num4))
							{
								center.X += num4 / 2;
								center.Y += num4 / 2;
								flag = true;
							}
						}
						if (flag)
						{
							for (int j = 0; j < 1000; j++)
							{
								if (Main.projectile[j].active && Main.projectile[j].owner == whoAmI && Main.projectile[j].aiStyle == 105 && (center - Main.projectile[j].Center).Length() < 48f)
								{
									flag = false;
									break;
								}
							}
							if (flag && Main.myPlayer == whoAmI)
							{
								break;
							}
						}
					}
				}
				num5++;
			}
			Projectile.NewProjectile(center.X, center.Y, 0f, 0f, 567 + Main.rand.Next(2), damage, knockBack, whoAmI);
		}

		public void ItemCheck(int i)
		{
			if (webbed || frozen || stoned)
			{
				return;
			}
			bool flag = false;
			float num = mount.PlayerOffsetHitbox;
			Item ıtem = inventory[selectedItem];
			if (mount.Active)
			{
				if (mount.Type == 8)
				{
					noItems = true;
					if (controlUseItem)
					{
						channel = true;
						if (releaseUseItem)
						{
							mount.UseAbility(this, Vector2.Zero, true);
						}
						releaseUseItem = false;
					}
				}
				if (whoAmI == Main.myPlayer && gravDir == -1f)
				{
					mount.Dismount(this);
				}
			}
			int weaponDamage = GetWeaponDamage(ıtem);
			if (ıtem.autoReuse && !noItems)
			{
				releaseUseItem = true;
				if (itemAnimation == 1 && ıtem.stack > 0)
				{
					if (ıtem.shoot > 0 && whoAmI != Main.myPlayer && controlUseItem && ıtem.useStyle == 5)
					{
						ApplyAnimation(ıtem);
						if (ıtem.useSound > 0)
						{
							Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, ıtem.useSound);
						}
					}
					else
					{
						itemAnimation = 0;
					}
				}
			}
			if (ıtem.fishingPole > 0)
			{
				ıtem.holdStyle = 0;
				if (itemTime == 0 && itemAnimation == 0)
				{
					for (int j = 0; j < 1000; j++)
					{
						if (Main.projectile[j].active && Main.projectile[j].owner == whoAmI && Main.projectile[j].bobber)
						{
							ıtem.holdStyle = 1;
						}
					}
				}
			}
			if (itemAnimation == 0 && altFunctionUse == 2)
			{
				altFunctionUse = 0;
			}
			if (itemAnimation == 0 && reuseDelay > 0)
			{
				itemAnimation = reuseDelay;
				itemTime = reuseDelay;
				reuseDelay = 0;
			}
			if (controlUseItem && releaseUseItem && (ıtem.headSlot > 0 || ıtem.bodySlot > 0 || ıtem.legSlot > 0))
			{
				if (ıtem.useStyle == 0)
				{
					releaseUseItem = false;
				}
				if (base.position.X / 16f - (float)tileRangeX - (float)ıtem.tileBoost <= (float)tileTargetX && (base.position.X + (float)width) / 16f + (float)tileRangeX + (float)ıtem.tileBoost - 1f >= (float)tileTargetX && base.position.Y / 16f - (float)tileRangeY - (float)ıtem.tileBoost <= (float)tileTargetY && (base.position.Y + (float)height) / 16f + (float)tileRangeY + (float)ıtem.tileBoost - 2f >= (float)tileTargetY)
				{
					int num2 = tileTargetX;
					int num3 = tileTargetY;
					if (Main.tile[num2, num3].active() && (Main.tile[num2, num3].type == 128 || Main.tile[num2, num3].type == 269))
					{
						int frameY = Main.tile[num2, num3].frameY;
						int num4 = 0;
						if (ıtem.bodySlot >= 0)
						{
							num4 = 1;
						}
						if (ıtem.legSlot >= 0)
						{
							num4 = 2;
						}
						frameY /= 18;
						while (num4 > frameY)
						{
							num3++;
							frameY = Main.tile[num2, num3].frameY;
							frameY /= 18;
						}
						while (num4 < frameY)
						{
							num3--;
							frameY = Main.tile[num2, num3].frameY;
							frameY /= 18;
						}
						int num5;
						for (num5 = Main.tile[num2, num3].frameX; num5 >= 100; num5 -= 100)
						{
						}
						if (num5 >= 36)
						{
							num5 -= 36;
						}
						num2 -= num5 / 18;
						int num6 = Main.tile[num2, num3].frameX;
						WorldGen.KillTile(num2, num3, true);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(17, -1, -1, "", 0, num2, num3, 1f);
						}
						while (num6 >= 100)
						{
							num6 -= 100;
						}
						if (frameY == 0 && ıtem.headSlot >= 0)
						{
							Main.blockMouse = true;
							Main.tile[num2, num3].frameX = (short)(num6 + ıtem.headSlot * 100);
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, num2, num3, 1);
							}
							ıtem.stack--;
							if (ıtem.stack <= 0)
							{
								ıtem.SetDefaults();
								Main.mouseItem.SetDefaults();
							}
							if (selectedItem == 58)
							{
								Main.mouseItem = ıtem.Clone();
							}
							releaseUseItem = false;
							mouseInterface = true;
						}
						else if (frameY == 1 && ıtem.bodySlot >= 0)
						{
							Main.blockMouse = true;
							Main.tile[num2, num3].frameX = (short)(num6 + ıtem.bodySlot * 100);
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, num2, num3, 1);
							}
							ıtem.stack--;
							if (ıtem.stack <= 0)
							{
								ıtem.SetDefaults();
								Main.mouseItem.SetDefaults();
							}
							if (selectedItem == 58)
							{
								Main.mouseItem = ıtem.Clone();
							}
							releaseUseItem = false;
							mouseInterface = true;
						}
						else if (frameY == 2 && ıtem.legSlot >= 0)
						{
							Main.blockMouse = true;
							Main.tile[num2, num3].frameX = (short)(num6 + ıtem.legSlot * 100);
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, num2, num3, 1);
							}
							ıtem.stack--;
							if (ıtem.stack <= 0)
							{
								ıtem.SetDefaults();
								Main.mouseItem.SetDefaults();
							}
							if (selectedItem == 58)
							{
								Main.mouseItem = ıtem.Clone();
							}
							releaseUseItem = false;
							mouseInterface = true;
						}
					}
				}
			}
			if (Main.myPlayer == i && itemAnimation == 0 && TileObjectData.CustomPlace(ıtem.createTile, ıtem.placeStyle))
			{
				TileObject objectData;
				TileObject.CanPlace(tileTargetX, tileTargetY, ıtem.createTile, ıtem.placeStyle, direction, out objectData, true);
			}
			if (controlUseItem && itemAnimation == 0 && releaseUseItem && ıtem.useStyle > 0)
			{
				if (altFunctionUse == 1)
				{
					altFunctionUse = 2;
				}
				bool flag2 = true;
				if (ıtem.shoot == 0)
				{
					itemRotation = 0f;
				}
				if (ıtem.type == 3335 && (extraAccessory || !Main.expertMode))
				{
					flag2 = false;
				}
				if (pulley && ıtem.fishingPole > 0)
				{
					flag2 = false;
				}
				if (wet && (ıtem.shoot == 85 || ıtem.shoot == 15 || ıtem.shoot == 34))
				{
					flag2 = false;
				}
				if (ıtem.makeNPC > 0 && !NPC.CanReleaseNPCs(whoAmI))
				{
					flag2 = false;
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 603 && !Main.cEd)
				{
					flag2 = false;
				}
				if (ıtem.type == 1071 || ıtem.type == 1072)
				{
					bool flag3 = false;
					for (int k = 0; k < 58; k++)
					{
						if (inventory[k].paint > 0)
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						flag2 = false;
					}
				}
				if (noItems)
				{
					flag2 = false;
				}
				if (ıtem.tileWand > 0)
				{
					int tileWand = ıtem.tileWand;
					flag2 = false;
					for (int l = 0; l < 58; l++)
					{
						if (tileWand == inventory[l].type && inventory[l].stack > 0)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (ıtem.fishingPole > 0)
				{
					for (int m = 0; m < 1000; m++)
					{
						if (!Main.projectile[m].active || Main.projectile[m].owner != whoAmI || !Main.projectile[m].bobber)
						{
							continue;
						}
						flag2 = false;
						if (whoAmI != Main.myPlayer || Main.projectile[m].ai[0] != 0f)
						{
							continue;
						}
						Main.projectile[m].ai[0] = 1f;
						float num7 = -10f;
						if (Main.projectile[m].wet && Main.projectile[m].velocity.Y > num7)
						{
							Main.projectile[m].velocity.Y = num7;
						}
						Main.projectile[m].netUpdate2 = true;
						if (!(Main.projectile[m].ai[1] < 0f) || Main.projectile[m].localAI[1] == 0f)
						{
							continue;
						}
						bool flag4 = false;
						int num8 = 0;
						for (int n = 0; n < 58; n++)
						{
							if (inventory[n].stack <= 0 || inventory[n].bait <= 0)
							{
								continue;
							}
							bool flag5 = false;
							int num9 = 1 + inventory[n].bait / 5;
							if (num9 < 1)
							{
								num9 = 1;
							}
							if (accTackleBox)
							{
								num9++;
							}
							if (Main.rand.Next(num9) == 0)
							{
								flag5 = true;
							}
							if (Main.projectile[m].localAI[1] < 0f)
							{
								flag5 = true;
							}
							if (Main.projectile[m].localAI[1] > 0f)
							{
								Item ıtem2 = new Item();
								ıtem2.SetDefaults((int)Main.projectile[m].localAI[1]);
								if (ıtem2.rare < 0)
								{
									flag5 = false;
								}
							}
							if (flag5)
							{
								num8 = inventory[n].type;
								inventory[n].stack--;
								if (inventory[n].stack <= 0)
								{
									inventory[n].SetDefaults();
								}
							}
							flag4 = true;
							break;
						}
						if (!flag4)
						{
							continue;
						}
						if (num8 == 2673)
						{
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(whoAmI, 370);
							}
							else
							{
								NetMessage.SendData(61, -1, -1, "", whoAmI, 370f);
							}
							Main.projectile[m].ai[0] = 2f;
						}
						else if (Main.rand.Next(7) == 0 && !accFishingLine)
						{
							Main.projectile[m].ai[0] = 2f;
						}
						else
						{
							Main.projectile[m].ai[1] = Main.projectile[m].localAI[1];
						}
						Main.projectile[m].netUpdate = true;
					}
				}
				if (ıtem.shoot == 6 || ıtem.shoot == 19 || ıtem.shoot == 33 || ıtem.shoot == 52 || ıtem.shoot == 113 || ıtem.shoot == 320 || ıtem.shoot == 333 || ıtem.shoot == 383 || ıtem.shoot == 491)
				{
					for (int num10 = 0; num10 < 1000; num10++)
					{
						if (Main.projectile[num10].active && Main.projectile[num10].owner == Main.myPlayer && Main.projectile[num10].type == ıtem.shoot)
						{
							flag2 = false;
						}
					}
				}
				if (ıtem.shoot == 106)
				{
					int num11 = 0;
					for (int num12 = 0; num12 < 1000; num12++)
					{
						if (Main.projectile[num12].active && Main.projectile[num12].owner == Main.myPlayer && Main.projectile[num12].type == ıtem.shoot)
						{
							num11++;
						}
					}
					if (num11 >= ıtem.stack)
					{
						flag2 = false;
					}
				}
				if (ıtem.shoot == 272)
				{
					int num13 = 0;
					for (int num14 = 0; num14 < 1000; num14++)
					{
						if (Main.projectile[num14].active && Main.projectile[num14].owner == Main.myPlayer && Main.projectile[num14].type == ıtem.shoot)
						{
							num13++;
						}
					}
					if (num13 >= ıtem.stack)
					{
						flag2 = false;
					}
				}
				if (ıtem.shoot == 13 || ıtem.shoot == 32 || (ıtem.shoot >= 230 && ıtem.shoot <= 235) || ıtem.shoot == 315 || ıtem.shoot == 331 || ıtem.shoot == 372)
				{
					for (int num15 = 0; num15 < 1000; num15++)
					{
						if (Main.projectile[num15].active && Main.projectile[num15].owner == Main.myPlayer && Main.projectile[num15].type == ıtem.shoot && Main.projectile[num15].ai[0] != 2f)
						{
							flag2 = false;
						}
					}
				}
				if (ıtem.shoot == 332)
				{
					int num16 = 0;
					for (int num17 = 0; num17 < 1000; num17++)
					{
						if (Main.projectile[num17].active && Main.projectile[num17].owner == Main.myPlayer && Main.projectile[num17].type == ıtem.shoot && Main.projectile[num17].ai[0] != 2f)
						{
							num16++;
						}
					}
					if (num16 >= 3)
					{
						flag2 = false;
					}
				}
				if (ıtem.potion && flag2)
				{
					if (potionDelay <= 0)
					{
						if (ıtem.type == 227)
						{
							potionDelay = restorationDelayTime;
							AddBuff(21, potionDelay);
						}
						else
						{
							potionDelay = potionDelayTime;
							AddBuff(21, potionDelay);
						}
					}
					else
					{
						flag2 = false;
					}
				}
				if (ıtem.mana > 0 && silence)
				{
					flag2 = false;
				}
				if (ıtem.mana > 0 && flag2)
				{
					bool flag6 = false;
					if (ıtem.type == 2795)
					{
						flag6 = true;
					}
					if (ıtem.type != 127 || !spaceGun)
					{
						if (statMana >= (int)((float)ıtem.mana * manaCost))
						{
							if (!flag6)
							{
								statMana -= (int)((float)ıtem.mana * manaCost);
							}
						}
						else if (manaFlower)
						{
							QuickMana();
							if (statMana >= (int)((float)ıtem.mana * manaCost))
							{
								if (!flag6)
								{
									statMana -= (int)((float)ıtem.mana * manaCost);
								}
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							flag2 = false;
						}
					}
					if (whoAmI == Main.myPlayer && ıtem.buffType != 0 && flag2)
					{
						AddBuff(ıtem.buffType, ıtem.buffTime);
					}
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 603 && Main.cEd)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 669)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 115)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3060)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3062)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3577)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 425)
				{
					int num18 = Main.rand.Next(3);
					if (num18 == 0)
					{
						num18 = 27;
					}
					if (num18 == 1)
					{
						num18 = 101;
					}
					if (num18 == 2)
					{
						num18 = 102;
					}
					for (int num19 = 0; num19 < 22; num19++)
					{
						if (buffType[num19] == 27 || buffType[num19] == 101 || buffType[num19] == 102)
						{
							DelBuff(num19);
							num19--;
						}
					}
					AddBuff(num18, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 753)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 994)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1169)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1170)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1171)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1172)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1180)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1181)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1182)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1183)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1242)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1157)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1309)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1311)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1837)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1312)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1798)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1799)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1802)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1810)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1927)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 1959)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2364)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2365)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3043)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2420)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2535)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2551)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2584)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2587)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2621)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 2749)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3249)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3474)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && ıtem.type == 3531)
				{
					AddBuff(ıtem.buffType, 3600);
				}
				if (whoAmI == Main.myPlayer && gravDir == 1f && ıtem.mountType != -1 && mount.CanMount(ıtem.mountType, this))
				{
					mount.SetMount(ıtem.mountType, this);
				}
				if (ıtem.type == 43 && Main.dayTime)
				{
					flag2 = false;
				}
				if (ıtem.type == 544 && Main.dayTime)
				{
					flag2 = false;
				}
				if (ıtem.type == 556 && Main.dayTime)
				{
					flag2 = false;
				}
				if (ıtem.type == 557 && Main.dayTime)
				{
					flag2 = false;
				}
				if (ıtem.type == 70 && !ZoneCorrupt)
				{
					flag2 = false;
				}
				if (ıtem.type == 1133 && !ZoneJungle)
				{
					flag2 = false;
				}
				if (ıtem.type == 1844 && (Main.dayTime || Main.pumpkinMoon || Main.snowMoon))
				{
					flag2 = false;
				}
				if (ıtem.type == 1958 && (Main.dayTime || Main.pumpkinMoon || Main.snowMoon))
				{
					flag2 = false;
				}
				if (ıtem.type == 2767 && (!Main.dayTime || Main.eclipse || !Main.hardMode))
				{
					flag2 = false;
				}
				if (ıtem.type == 3601 && (!NPC.downedGolemBoss || !Main.hardMode || NPC.AnyDanger() || NPC.AnyoneNearCultists()))
				{
					flag2 = false;
				}
				if (!SummonItemCheck())
				{
					flag2 = false;
				}
				if (ıtem.shoot == 17 && flag2 && i == Main.myPlayer)
				{
					int num20 = (int)((float)Main.mouseX + Main.screenPosition.X) / 16;
					int num21 = (int)((float)Main.mouseY + Main.screenPosition.Y) / 16;
					if (gravDir == -1f)
					{
						num21 = (int)(Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY) / 16;
					}
					Tile tile = Main.tile[num20, num21];
					if (tile.active() && (tile.type == 0 || tile.type == 2 || tile.type == 23 || tile.type == 109 || tile.type == 199))
					{
						WorldGen.KillTile(num20, num21, false, false, true);
						if (!Main.tile[num20, num21].active())
						{
							if (Main.netMode == 1)
							{
								NetMessage.SendData(17, -1, -1, "", 4, num20, num21);
							}
						}
						else
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					flag2 = HasAmmo(ıtem, flag2);
				}
				if (flag2)
				{
					if (ıtem.pick > 0 || ıtem.axe > 0 || ıtem.hammer > 0)
					{
						toolTime = 1;
					}
					if (grappling[0] > -1)
					{
						pulley = false;
						pulleyDir = 1;
						if (controlRight)
						{
							direction = 1;
						}
						else if (controlLeft)
						{
							direction = -1;
						}
					}
					channel = ıtem.channel;
					attackCD = 0;
					ApplyAnimation(ıtem);
					if (ıtem.useSound > 0)
					{
						Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, ıtem.useSound);
					}
				}
				if (flag2 && whoAmI == Main.myPlayer && ıtem.shoot >= 0 && ıtem.shoot < 651 && (ProjectileID.Sets.LightPet[ıtem.shoot] || Main.projPet[ıtem.shoot]))
				{
					if (ProjectileID.Sets.MinionSacrificable[ıtem.shoot])
					{
						List<int> list = new List<int>();
						float num22 = 0f;
						for (int num23 = 0; num23 < 1000; num23++)
						{
							if (!Main.projectile[num23].active || Main.projectile[num23].owner != i || !Main.projectile[num23].minion)
							{
								continue;
							}
							int num24;
							for (num24 = 0; num24 < list.Count; num24++)
							{
								if (Main.projectile[list[num24]].minionSlots > Main.projectile[num23].minionSlots)
								{
									list.Insert(num24, num23);
									break;
								}
							}
							if (num24 == list.Count)
							{
								list.Add(num23);
							}
							num22 += Main.projectile[num23].minionSlots;
						}
						float num25 = ItemID.Sets.StaffMinionSlotsRequired[ıtem.type];
						float num26 = 0f;
						int num27 = 388;
						int num28 = -1;
						for (int num29 = 0; num29 < list.Count; num29++)
						{
							int type = Main.projectile[list[num29]].type;
							if (type == 626)
							{
								list.RemoveAt(num29);
								num29--;
							}
							if (type == 627)
							{
								if (Main.projectile[(int)Main.projectile[list[num29]].localAI[1]].type == 628)
								{
									num28 = list[num29];
								}
								list.RemoveAt(num29);
								num29--;
							}
						}
						if (num28 != -1)
						{
							list.Add(num28);
							list.Add(Projectile.GetByUUID(Main.projectile[num28].owner, Main.projectile[num28].ai[0]));
						}
						for (int num30 = 0; num30 < list.Count; num30++)
						{
							if (!(num22 - num26 > (float)maxMinions - num25))
							{
								break;
							}
							int type2 = Main.projectile[list[num30]].type;
							if (type2 == num27 || type2 == 625 || type2 == 628 || type2 == 623)
							{
								continue;
							}
							if (type2 == 388 && num27 == 387)
							{
								num27 = 388;
							}
							if (type2 == 387 && num27 == 388)
							{
								num27 = 387;
							}
							num26 += Main.projectile[list[num30]].minionSlots;
							if (type2 == 626 || type2 == 627)
							{
								int byUUID = Projectile.GetByUUID(Main.projectile[list[num30]].owner, Main.projectile[list[num30]].ai[0]);
								if (byUUID >= 0)
								{
									Projectile projectile = Main.projectile[byUUID];
									if (projectile.type != 625)
									{
										projectile.localAI[1] = Main.projectile[list[num30]].localAI[1];
									}
									projectile = Main.projectile[(int)Main.projectile[list[num30]].localAI[1]];
									projectile.ai[0] = Main.projectile[list[num30]].ai[0];
									projectile.ai[1] = 1f;
									projectile.netUpdate = true;
								}
							}
							Main.projectile[list[num30]].Kill();
						}
						list.Clear();
						if (num22 + num25 >= 9f)
						{
							AchievementsHelper.HandleSpecialEvent(this, 6);
						}
					}
					else
					{
						for (int num31 = 0; num31 < 1000; num31++)
						{
							if (Main.projectile[num31].active && Main.projectile[num31].owner == i && Main.projectile[num31].type == ıtem.shoot)
							{
								Main.projectile[num31].Kill();
							}
							if (ıtem.shoot == 72)
							{
								if (Main.projectile[num31].active && Main.projectile[num31].owner == i && Main.projectile[num31].type == 86)
								{
									Main.projectile[num31].Kill();
								}
								if (Main.projectile[num31].active && Main.projectile[num31].owner == i && Main.projectile[num31].type == 87)
								{
									Main.projectile[num31].Kill();
								}
							}
						}
					}
				}
			}
			if (!controlUseItem)
			{
				bool channel2 = channel;
				channel = false;
			}
			if (itemAnimation > 0)
			{
				if (ıtem.melee)
				{
					itemAnimationMax = (int)((float)ıtem.useAnimation * meleeSpeed);
				}
				else
				{
					itemAnimationMax = ıtem.useAnimation;
				}
				if (ıtem.mana > 0 && !flag && (ıtem.type != 127 || !spaceGun))
				{
					manaRegenDelay = (int)maxRegenDelay;
				}
				if (Main.dedServ)
				{
					itemHeight = ıtem.height;
					itemWidth = ıtem.width;
				}
				else
				{
					itemHeight = Main.itemTexture[ıtem.type].Height;
					itemWidth = Main.itemTexture[ıtem.type].Width;
				}
				itemAnimation--;
				if (!Main.dedServ)
				{
					if (ıtem.useStyle == 1)
					{
						if (ıtem.type > -1 && Item.claw[ıtem.type])
						{
							if ((double)itemAnimation < (double)itemAnimationMax * 0.333)
							{
								float num32 = 10f;
								itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - num32) * (float)direction;
								itemLocation.Y = base.position.Y + 26f + num;
							}
							else if ((double)itemAnimation < (double)itemAnimationMax * 0.666)
							{
								float num33 = 8f;
								itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - num33) * (float)direction;
								num33 = 24f;
								itemLocation.Y = base.position.Y + num33 + num;
							}
							else
							{
								float num34 = 6f;
								itemLocation.X = base.position.X + (float)width * 0.5f - ((float)Main.itemTexture[ıtem.type].Width * 0.5f - num34) * (float)direction;
								num34 = 20f;
								itemLocation.Y = base.position.Y + num34 + num;
							}
							itemRotation = ((float)itemAnimation / (float)itemAnimationMax - 0.5f) * (float)(-direction) * 3.5f - (float)direction * 0.3f;
						}
						else
						{
							if ((double)itemAnimation < (double)itemAnimationMax * 0.333)
							{
								float num35 = 10f;
								if (Main.itemTexture[ıtem.type].Width > 32)
								{
									num35 = 14f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 52)
								{
									num35 = 24f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 64)
								{
									num35 = 28f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 92)
								{
									num35 = 38f;
								}
								if (ıtem.type == 2330 || ıtem.type == 2320 || ıtem.type == 2341)
								{
									num35 += 8f;
								}
								itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - num35) * (float)direction;
								itemLocation.Y = base.position.Y + 24f + num;
							}
							else if ((double)itemAnimation < (double)itemAnimationMax * 0.666)
							{
								float num36 = 10f;
								if (Main.itemTexture[ıtem.type].Width > 32)
								{
									num36 = 18f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 52)
								{
									num36 = 24f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 64)
								{
									num36 = 28f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 92)
								{
									num36 = 38f;
								}
								if (ıtem.type == 2330 || ıtem.type == 2320 || ıtem.type == 2341)
								{
									num36 += 4f;
								}
								itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - num36) * (float)direction;
								num36 = 10f;
								if (Main.itemTexture[ıtem.type].Height > 32)
								{
									num36 = 8f;
								}
								if (Main.itemTexture[ıtem.type].Height > 52)
								{
									num36 = 12f;
								}
								if (Main.itemTexture[ıtem.type].Height > 64)
								{
									num36 = 14f;
								}
								if (ıtem.type == 2330 || ıtem.type == 2320 || ıtem.type == 2341)
								{
									num36 += 4f;
								}
								itemLocation.Y = base.position.Y + num36 + num;
							}
							else
							{
								float num37 = 6f;
								if (Main.itemTexture[ıtem.type].Width > 32)
								{
									num37 = 14f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 48)
								{
									num37 = 18f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 52)
								{
									num37 = 24f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 64)
								{
									num37 = 28f;
								}
								if (Main.itemTexture[ıtem.type].Width >= 92)
								{
									num37 = 38f;
								}
								if (ıtem.type == 2330 || ıtem.type == 2320 || ıtem.type == 2341)
								{
									num37 += 4f;
								}
								itemLocation.X = base.position.X + (float)width * 0.5f - ((float)Main.itemTexture[ıtem.type].Width * 0.5f - num37) * (float)direction;
								num37 = 10f;
								if (Main.itemTexture[ıtem.type].Height > 32)
								{
									num37 = 10f;
								}
								if (Main.itemTexture[ıtem.type].Height > 52)
								{
									num37 = 12f;
								}
								if (Main.itemTexture[ıtem.type].Height > 64)
								{
									num37 = 14f;
								}
								if (ıtem.type == 2330 || ıtem.type == 2320 || ıtem.type == 2341)
								{
									num37 += 4f;
								}
								itemLocation.Y = base.position.Y + num37 + num;
							}
							itemRotation = ((float)itemAnimation / (float)itemAnimationMax - 0.5f) * (float)(-direction) * 3.5f - (float)direction * 0.3f;
						}
						if (gravDir == -1f)
						{
							itemRotation = 0f - itemRotation;
							itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y);
						}
					}
					else if (ıtem.useStyle == 2)
					{
						itemRotation = (float)itemAnimation / (float)itemAnimationMax * (float)direction * 2f + -1.4f * (float)direction;
						if ((double)itemAnimation < (double)itemAnimationMax * 0.5)
						{
							itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - 9f - itemRotation * 12f * (float)direction) * (float)direction;
							itemLocation.Y = base.position.Y + 38f + itemRotation * (float)direction * 4f + num;
						}
						else
						{
							itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - 9f - itemRotation * 16f * (float)direction) * (float)direction;
							itemLocation.Y = base.position.Y + 38f + itemRotation * (float)direction + num;
						}
						if (gravDir == -1f)
						{
							itemRotation = 0f - itemRotation;
							itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y);
						}
					}
					else if (ıtem.useStyle == 3)
					{
						if ((double)itemAnimation > (double)itemAnimationMax * 0.666)
						{
							itemLocation.X = -1000f;
							itemLocation.Y = -1000f;
							itemRotation = -1.3f * (float)direction;
						}
						else
						{
							itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - 4f) * (float)direction;
							itemLocation.Y = base.position.Y + 24f + num;
							float num38 = (float)itemAnimation / (float)itemAnimationMax * (float)Main.itemTexture[ıtem.type].Width * (float)direction * ıtem.scale * 1.2f - (float)(10 * direction);
							if (num38 > -4f && direction == -1)
							{
								num38 = -8f;
							}
							if (num38 < 4f && direction == 1)
							{
								num38 = 8f;
							}
							itemLocation.X -= num38;
							itemRotation = 0.8f * (float)direction;
						}
						if (gravDir == -1f)
						{
							itemRotation = 0f - itemRotation;
							itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y);
						}
					}
					else if (ıtem.useStyle == 4)
					{
						int num39 = 0;
						if (ıtem.type == 3601)
						{
							num39 = 10;
						}
						itemRotation = 0f;
						itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f - 9f - itemRotation * 14f * (float)direction - 4f - (float)num39) * (float)direction;
						itemLocation.Y = base.position.Y + (float)Main.itemTexture[ıtem.type].Height * 0.5f + 4f + num;
						if (gravDir == -1f)
						{
							itemRotation = 0f - itemRotation;
							itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y);
						}
					}
					else if (ıtem.useStyle == 5)
					{
						if (Item.staff[ıtem.type])
						{
							float scaleFactor = 6f;
							if (ıtem.type == 3476)
							{
								scaleFactor = 14f;
							}
							itemLocation = MountedCenter;
							itemLocation += itemRotation.ToRotationVector2() * scaleFactor * direction;
						}
						else
						{
							itemLocation.X = base.position.X + (float)width * 0.5f - (float)Main.itemTexture[ıtem.type].Width * 0.5f - (float)(direction * 2);
							itemLocation.Y = MountedCenter.Y - (float)Main.itemTexture[ıtem.type].Height * 0.5f;
						}
					}
				}
			}
			else if (ıtem.holdStyle == 1 && !pulley)
			{
				if (Main.dedServ)
				{
					itemLocation.X = base.position.X + (float)width * 0.5f + 20f * (float)direction;
				}
				else if (ıtem.type == 930)
				{
					itemLocation.X = base.position.X + (float)(width / 2) * 0.5f - 12f - (float)(2 * direction);
					float x = base.position.X + (float)(width / 2) + (float)(38 * direction);
					if (direction == 1)
					{
						x -= 10f;
					}
					float y = MountedCenter.Y - 4f * gravDir;
					if (gravDir == -1f)
					{
						y -= 8f;
					}
					RotateRelativePoint(ref x, ref y);
					int num40 = 0;
					for (int num41 = 54; num41 < 58; num41++)
					{
						if (inventory[num41].stack > 0 && inventory[num41].ammo == 931)
						{
							num40 = inventory[num41].type;
							break;
						}
					}
					if (num40 == 0)
					{
						for (int num42 = 0; num42 < 54; num42++)
						{
							if (inventory[num42].stack > 0 && inventory[num42].ammo == 931)
							{
								num40 = inventory[num42].type;
								break;
							}
						}
					}
					switch (num40)
					{
					case 931:
						num40 = 127;
						break;
					case 1614:
						num40 = 187;
						break;
					}
					if (num40 > 0)
					{
						int num43 = Dust.NewDust(new Vector2(x, y + gfxOffY), 6, 6, num40, 0f, 0f, 100, default(Color), 1.6f);
						Main.dust[num43].noGravity = true;
						Main.dust[num43].velocity.Y -= 4f * gravDir;
					}
				}
				else if (ıtem.type == 968)
				{
					itemLocation.X = base.position.X + (float)width * 0.5f + (float)(8 * direction);
					if (whoAmI == Main.myPlayer)
					{
						int num44 = (int)(itemLocation.X + (float)Main.itemTexture[ıtem.type].Width * 0.8f * (float)direction) / 16;
						int num45 = (int)(itemLocation.Y + num + (float)(Main.itemTexture[ıtem.type].Height / 2)) / 16;
						if (Main.tile[num44, num45] == null)
						{
							Main.tile[num44, num45] = new Tile();
						}
						if (Main.tile[num44, num45].active() && Main.tile[num44, num45].type == 215 && Main.tile[num44, num45].frameY < 54)
						{
							miscTimer++;
							if (Main.rand.Next(5) == 0)
							{
								miscTimer++;
							}
							if (miscTimer > 900)
							{
								miscTimer = 0;
								ıtem.SetDefaults(969);
								if (selectedItem == 58)
								{
									Main.mouseItem.SetDefaults(969);
								}
								for (int num46 = 0; num46 < 58; num46++)
								{
									if (inventory[num46].type == ıtem.type && num46 != selectedItem && inventory[num46].stack < inventory[num46].maxStack)
									{
										Main.PlaySound(7);
										inventory[num46].stack++;
										ıtem.SetDefaults();
										if (selectedItem == 58)
										{
											Main.mouseItem.SetDefaults();
										}
									}
								}
							}
						}
						else
						{
							miscTimer = 0;
						}
					}
				}
				else if (ıtem.type == 856)
				{
					itemLocation.X = base.position.X + (float)width * 0.5f + (float)(4 * direction);
				}
				else if (ıtem.fishingPole > 0)
				{
					itemLocation.X = base.position.X + (float)width * 0.5f + (float)Main.itemTexture[ıtem.type].Width * 0.18f * (float)direction;
				}
				else
				{
					itemLocation.X = base.position.X + (float)width * 0.5f + ((float)Main.itemTexture[ıtem.type].Width * 0.5f + 2f) * (float)direction;
					if (ıtem.type == 282 || ıtem.type == 286 || ıtem.type == 3112)
					{
						itemLocation.X -= direction * 2;
						itemLocation.Y += 4f;
					}
					else if (ıtem.type == 3002)
					{
						itemLocation.X -= 4 * direction;
						itemLocation.Y += 2f;
					}
				}
				itemLocation.Y = base.position.Y + 24f + num;
				if (ıtem.type == 856)
				{
					itemLocation.Y = base.position.Y + 34f + num;
				}
				if (ıtem.type == 930)
				{
					itemLocation.Y = base.position.Y + 9f + num;
				}
				if (ıtem.fishingPole > 0)
				{
					itemLocation.Y += 4f;
				}
				else if (ıtem.type == 3476)
				{
					itemLocation.X = base.Center.X + (float)(14 * direction);
					itemLocation.Y = MountedCenter.Y;
				}
				itemRotation = 0f;
				if (gravDir == -1f)
				{
					itemRotation = 0f - itemRotation;
					itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y) + num;
					if (ıtem.type == 930)
					{
						itemLocation.Y -= 24f;
					}
				}
			}
			else if (ıtem.holdStyle == 2 && !pulley)
			{
				if (ıtem.type == 946)
				{
					itemRotation = 0f;
					itemLocation.X = base.position.X + (float)width * 0.5f - (float)(16 * direction);
					itemLocation.Y = base.position.Y + 22f + num;
					fallStart = (int)(base.position.Y / 16f);
					if (gravDir == -1f)
					{
						itemRotation = 0f - itemRotation;
						itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y);
						if (velocity.Y < -2f)
						{
							velocity.Y = -2f;
						}
					}
					else if (velocity.Y > 2f)
					{
						velocity.Y = 2f;
					}
				}
				else
				{
					itemLocation.X = base.position.X + (float)width * 0.5f + (float)(6 * direction);
					itemLocation.Y = base.position.Y + 16f + num;
					itemRotation = 0.79f * (float)(-direction);
					if (gravDir == -1f)
					{
						itemRotation = 0f - itemRotation;
						itemLocation.Y = base.position.Y + (float)height + (base.position.Y - itemLocation.Y);
					}
				}
			}
			else if (ıtem.holdStyle == 3 && !pulley && !Main.dedServ)
			{
				itemLocation.X = base.position.X + (float)width * 0.5f - (float)Main.itemTexture[ıtem.type].Width * 0.5f - (float)(direction * 2);
				itemLocation.Y = MountedCenter.Y - (float)Main.itemTexture[ıtem.type].Height * 0.5f;
				itemRotation = 0f;
			}
			if ((((ıtem.type == 974 || ıtem.type == 8 || ıtem.type == 1245 || ıtem.type == 2274 || ıtem.type == 3004 || ıtem.type == 3045 || ıtem.type == 3114 || (ıtem.type >= 427 && ıtem.type <= 433)) && !wet) || ıtem.type == 523 || ıtem.type == 1333) && !pulley)
			{
				float num47 = 1f;
				float num48 = 0.95f;
				float num49 = 0.8f;
				int num50 = 0;
				if (ıtem.type == 523)
				{
					num50 = 8;
				}
				else if (ıtem.type == 974)
				{
					num50 = 9;
				}
				else if (ıtem.type == 1245)
				{
					num50 = 10;
				}
				else if (ıtem.type == 1333)
				{
					num50 = 11;
				}
				else if (ıtem.type == 2274)
				{
					num50 = 12;
				}
				else if (ıtem.type == 3004)
				{
					num50 = 13;
				}
				else if (ıtem.type == 3045)
				{
					num50 = 14;
				}
				else if (ıtem.type == 3114)
				{
					num50 = 15;
				}
				else if (ıtem.type >= 427)
				{
					num50 = ıtem.type - 426;
				}
				switch (num50)
				{
				case 1:
					num47 = 0f;
					num48 = 0.1f;
					num49 = 1.3f;
					break;
				case 2:
					num47 = 1f;
					num48 = 0.1f;
					num49 = 0.1f;
					break;
				case 3:
					num47 = 0f;
					num48 = 1f;
					num49 = 0.1f;
					break;
				case 4:
					num47 = 0.9f;
					num48 = 0f;
					num49 = 0.9f;
					break;
				case 5:
					num47 = 1.3f;
					num48 = 1.3f;
					num49 = 1.3f;
					break;
				case 6:
					num47 = 0.9f;
					num48 = 0.9f;
					num49 = 0f;
					break;
				case 7:
					num47 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
					num48 = 0.3f;
					num49 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
					break;
				case 8:
					num49 = 0.7f;
					num47 = 0.85f;
					num48 = 1f;
					break;
				case 9:
					num49 = 1f;
					num47 = 0.7f;
					num48 = 0.85f;
					break;
				case 10:
					num49 = 0f;
					num47 = 1f;
					num48 = 0.5f;
					break;
				case 11:
					num49 = 0.8f;
					num47 = 1.25f;
					num48 = 1.25f;
					break;
				case 12:
					num47 *= 0.75f;
					num48 *= 1.3499999f;
					num49 *= 1.5f;
					break;
				case 13:
					num47 = 0.95f;
					num48 = 0.65f;
					num49 = 1.3f;
					break;
				case 14:
					num47 = (float)Main.DiscoR / 255f;
					num48 = (float)Main.DiscoG / 255f;
					num49 = (float)Main.DiscoB / 255f;
					break;
				case 15:
					num47 = 1f;
					num48 = 0f;
					num49 = 1f;
					break;
				}
				int num51 = num50;
				switch (num51)
				{
				case 0:
					num51 = 6;
					break;
				case 8:
					num51 = 75;
					break;
				case 9:
					num51 = 135;
					break;
				case 10:
					num51 = 158;
					break;
				case 11:
					num51 = 169;
					break;
				case 12:
					num51 = 156;
					break;
				case 13:
					num51 = 234;
					break;
				case 14:
					num51 = 66;
					break;
				case 15:
					num51 = 242;
					break;
				default:
					num51 = 58 + num51;
					break;
				}
				int maxValue = 30;
				if (itemAnimation > 0)
				{
					maxValue = 7;
				}
				if (direction == -1)
				{
					if (Main.rand.Next(maxValue) == 0)
					{
						int num52 = Dust.NewDust(new Vector2(itemLocation.X - 16f, itemLocation.Y - 14f * gravDir), 4, 4, num51, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num52].noGravity = true;
						}
						Main.dust[num52].velocity *= 0.3f;
						Main.dust[num52].velocity.Y -= 1.5f;
						Main.dust[num52].position = RotatedRelativePoint(Main.dust[num52].position);
						if (num51 == 66)
						{
							Main.dust[num52].color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
							Main.dust[num52].noGravity = true;
						}
					}
					Vector2 position = RotatedRelativePoint(new Vector2(itemLocation.X - 12f + velocity.X, itemLocation.Y - 14f + velocity.Y));
					Lighting.AddLight(position, num47, num48, num49);
				}
				else
				{
					if (Main.rand.Next(maxValue) == 0)
					{
						int num53 = Dust.NewDust(new Vector2(itemLocation.X + 6f, itemLocation.Y - 14f * gravDir), 4, 4, num51, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num53].noGravity = true;
						}
						Main.dust[num53].velocity *= 0.3f;
						Main.dust[num53].velocity.Y -= 1.5f;
						Main.dust[num53].position = RotatedRelativePoint(Main.dust[num53].position);
						if (num51 == 66)
						{
							Main.dust[num53].color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
							Main.dust[num53].noGravity = true;
						}
					}
					Vector2 position2 = RotatedRelativePoint(new Vector2(itemLocation.X + 12f + velocity.X, itemLocation.Y - 14f + velocity.Y));
					Lighting.AddLight(position2, num47, num48, num49);
				}
			}
			if ((ıtem.type == 105 || ıtem.type == 713) && !wet && !pulley)
			{
				int maxValue2 = 20;
				if (itemAnimation > 0)
				{
					maxValue2 = 7;
				}
				if (direction == -1)
				{
					if (Main.rand.Next(maxValue2) == 0)
					{
						int num54 = Dust.NewDust(new Vector2(itemLocation.X - 12f, itemLocation.Y - 20f * gravDir), 4, 4, 6, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num54].noGravity = true;
						}
						Main.dust[num54].velocity *= 0.3f;
						Main.dust[num54].velocity.Y -= 1.5f;
						Main.dust[num54].position = RotatedRelativePoint(Main.dust[num54].position);
					}
					Vector2 position3 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position3, 1f, 0.95f, 0.8f);
				}
				else
				{
					if (Main.rand.Next(maxValue2) == 0)
					{
						int num55 = Dust.NewDust(new Vector2(itemLocation.X + 4f, itemLocation.Y - 20f * gravDir), 4, 4, 6, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num55].noGravity = true;
						}
						Main.dust[num55].velocity *= 0.3f;
						Main.dust[num55].velocity.Y -= 1.5f;
						Main.dust[num55].position = RotatedRelativePoint(Main.dust[num55].position);
					}
					Vector2 position4 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position4, 1f, 0.95f, 0.8f);
				}
			}
			else if (ıtem.type == 148 && !wet)
			{
				int maxValue3 = 10;
				if (itemAnimation > 0)
				{
					maxValue3 = 7;
				}
				if (direction == -1)
				{
					if (Main.rand.Next(maxValue3) == 0)
					{
						int num56 = Dust.NewDust(new Vector2(itemLocation.X - 12f, itemLocation.Y - 20f * gravDir), 4, 4, 172, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num56].noGravity = true;
						}
						Main.dust[num56].velocity *= 0.3f;
						Main.dust[num56].velocity.Y -= 1.5f;
						Main.dust[num56].position = RotatedRelativePoint(Main.dust[num56].position);
					}
					Vector2 position5 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position5, 0f, 0.5f, 1f);
				}
				else
				{
					if (Main.rand.Next(maxValue3) == 0)
					{
						int num57 = Dust.NewDust(new Vector2(itemLocation.X + 4f, itemLocation.Y - 20f * gravDir), 4, 4, 172, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num57].noGravity = true;
						}
						Main.dust[num57].velocity *= 0.3f;
						Main.dust[num57].velocity.Y -= 1.5f;
						Main.dust[num57].position = RotatedRelativePoint(Main.dust[num57].position);
					}
					Vector2 position6 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position6, 0f, 0.5f, 1f);
				}
			}
			else if (ıtem.type == 3117 && !wet)
			{
				itemLocation.X -= direction * 4;
				int maxValue4 = 10;
				if (itemAnimation > 0)
				{
					maxValue4 = 7;
				}
				if (direction == -1)
				{
					if (Main.rand.Next(maxValue4) == 0)
					{
						int num58 = Dust.NewDust(new Vector2(itemLocation.X - 10f, itemLocation.Y - 20f * gravDir), 4, 4, 242, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num58].noGravity = true;
						}
						Main.dust[num58].velocity *= 0.3f;
						Main.dust[num58].velocity.Y -= 1.5f;
						Main.dust[num58].position = RotatedRelativePoint(Main.dust[num58].position);
					}
					Vector2 position7 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position7, 0.9f, 0.1f, 0.75f);
				}
				else
				{
					if (Main.rand.Next(maxValue4) == 0)
					{
						int num59 = Dust.NewDust(new Vector2(itemLocation.X + 6f, itemLocation.Y - 20f * gravDir), 4, 4, 242, 0f, 0f, 100);
						if (Main.rand.Next(3) != 0)
						{
							Main.dust[num59].noGravity = true;
						}
						Main.dust[num59].velocity *= 0.3f;
						Main.dust[num59].velocity.Y -= 1.5f;
						Main.dust[num59].position = RotatedRelativePoint(Main.dust[num59].position);
					}
					Vector2 position8 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position8, 0.9f, 0.1f, 0.75f);
				}
			}
			if (ıtem.type == 282 && !pulley)
			{
				if (direction == -1)
				{
					Vector2 position9 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position9, 0.7f, 1f, 0.8f);
				}
				else
				{
					Vector2 position10 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position10, 0.7f, 1f, 0.8f);
				}
			}
			if (ıtem.type == 3002 && !pulley)
			{
				float r = 1.05f;
				float g = 0.95f;
				float b = 0.55f;
				if (direction == -1)
				{
					Vector2 position11 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position11, r, g, b);
				}
				else
				{
					Vector2 position12 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position12, r, g, b);
				}
				spelunkerTimer++;
				if (spelunkerTimer >= 10)
				{
					spelunkerTimer = 0;
					int num60 = 30;
					int num61 = (int)base.Center.X / 16;
					int num62 = (int)base.Center.Y / 16;
					for (int num63 = num61 - num60; num63 <= num61 + num60; num63++)
					{
						for (int num64 = num62 - num60; num64 <= num62 + num60; num64++)
						{
							if (Main.rand.Next(4) != 0 || !(new Vector2(num61 - num63, num62 - num64).Length() < (float)num60) || num63 <= 0 || num63 >= Main.maxTilesX - 1 || num64 <= 0 || num64 >= Main.maxTilesY - 1 || Main.tile[num63, num64] == null || !Main.tile[num63, num64].active())
							{
								continue;
							}
							bool flag7 = false;
							if (Main.tile[num63, num64].type == 185 && Main.tile[num63, num64].frameY == 18)
							{
								if (Main.tile[num63, num64].frameX >= 576 && Main.tile[num63, num64].frameX <= 882)
								{
									flag7 = true;
								}
							}
							else if (Main.tile[num63, num64].type == 186 && Main.tile[num63, num64].frameX >= 864 && Main.tile[num63, num64].frameX <= 1170)
							{
								flag7 = true;
							}
							if (flag7 || Main.tileSpelunker[Main.tile[num63, num64].type] || (Main.tileAlch[Main.tile[num63, num64].type] && Main.tile[num63, num64].type != 82))
							{
								int num65 = Dust.NewDust(new Vector2(num63 * 16, num64 * 16), 16, 16, 204, 0f, 0f, 150, default(Color), 0.3f);
								Main.dust[num65].fadeIn = 0.75f;
								Main.dust[num65].velocity *= 0.1f;
								Main.dust[num65].noLight = true;
							}
						}
					}
				}
			}
			if (ıtem.type == 286 && !pulley)
			{
				if (direction == -1)
				{
					Vector2 position13 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position13, 0.7f, 0.8f, 1f);
				}
				else
				{
					Vector2 position14 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position14, 0.7f, 0.8f, 1f);
				}
			}
			if (ıtem.type == 3112 && !pulley)
			{
				if (direction == -1)
				{
					Vector2 position15 = RotatedRelativePoint(new Vector2(itemLocation.X - 16f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position15, 1f, 0.6f, 0.85f);
				}
				else
				{
					Vector2 position16 = RotatedRelativePoint(new Vector2(itemLocation.X + 6f + velocity.X, itemLocation.Y - 14f));
					Lighting.AddLight(position16, 1f, 0.6f, 0.85f);
				}
			}
			if (controlUseItem)
			{
				releaseUseItem = false;
			}
			else
			{
				releaseUseItem = true;
			}
			if (itemTime > 0)
			{
				itemTime--;
				if (itemTime == 0 && whoAmI == Main.myPlayer)
				{
					int type3 = ıtem.type;
					if (type3 == 65 || type3 == 676 || type3 == 723 || type3 == 724 || type3 == 989 || type3 == 1226 || type3 == 1227)
					{
						Main.PlaySound(25);
						for (int num66 = 0; num66 < 5; num66++)
						{
							int num67 = Dust.NewDust(base.position, width, height, 45, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
							Main.dust[num67].noLight = true;
							Main.dust[num67].noGravity = true;
							Main.dust[num67].velocity *= 0.5f;
						}
					}
				}
			}
			if (i == Main.myPlayer)
			{
				bool flag8 = true;
				int type4 = ıtem.type;
				if ((type4 == 65 || type4 == 676 || type4 == 723 || type4 == 724 || type4 == 757 || type4 == 674 || type4 == 675 || type4 == 989 || type4 == 1226 || type4 == 1227) && itemAnimation != itemAnimationMax - 1)
				{
					flag8 = false;
				}
				if (ıtem.shoot > 0 && itemAnimation > 0 && itemTime == 0 && flag8)
				{
					int shoot = ıtem.shoot;
					float speed = ıtem.shootSpeed;
					if (inventory[selectedItem].thrown && speed < 16f)
					{
						speed *= thrownVelocity;
						if (speed > 16f)
						{
							speed = 16f;
						}
					}
					if (ıtem.melee && shoot != 25 && shoot != 26 && shoot != 35)
					{
						speed /= meleeSpeed;
					}
					bool canShoot = false;
					int Damage = weaponDamage;
					float KnockBack = ıtem.knockBack;
					if (shoot == 13 || shoot == 32 || shoot == 315 || (shoot >= 230 && shoot <= 235) || shoot == 331)
					{
						grappling[0] = -1;
						grapCount = 0;
						for (int num68 = 0; num68 < 1000; num68++)
						{
							if (Main.projectile[num68].active && Main.projectile[num68].owner == i)
							{
								if (Main.projectile[num68].type == 13)
								{
									Main.projectile[num68].Kill();
								}
								if (Main.projectile[num68].type == 331)
								{
									Main.projectile[num68].Kill();
								}
								if (Main.projectile[num68].type == 315)
								{
									Main.projectile[num68].Kill();
								}
								if (Main.projectile[num68].type >= 230 && Main.projectile[num68].type <= 235)
								{
									Main.projectile[num68].Kill();
								}
							}
						}
					}
					if (ıtem.useAmmo > 0)
					{
						PickAmmo(ıtem, ref shoot, ref speed, ref canShoot, ref Damage, ref KnockBack, ItemID.Sets.gunProj[ıtem.type]);
					}
					else
					{
						canShoot = true;
					}
					if (ıtem.type == 3475 || ıtem.type == 3540)
					{
						KnockBack = ıtem.knockBack;
						Damage = weaponDamage;
						speed = ıtem.shootSpeed;
					}
					if (ıtem.type == 71)
					{
						canShoot = false;
					}
					if (ıtem.type == 72)
					{
						canShoot = false;
					}
					if (ıtem.type == 73)
					{
						canShoot = false;
					}
					if (ıtem.type == 74)
					{
						canShoot = false;
					}
					if (ıtem.type == 1254 && shoot == 14)
					{
						shoot = 242;
					}
					if (ıtem.type == 1255 && shoot == 14)
					{
						shoot = 242;
					}
					if (ıtem.type == 1265 && shoot == 14)
					{
						shoot = 242;
					}
					if (ıtem.type == 3542)
					{
						if (Main.rand.Next(100) < 20)
						{
							shoot++;
							Damage *= 3;
						}
						else
						{
							speed -= 1f;
						}
					}
					if (shoot == 73)
					{
						for (int num69 = 0; num69 < 1000; num69++)
						{
							if (Main.projectile[num69].active && Main.projectile[num69].owner == i)
							{
								if (Main.projectile[num69].type == 73)
								{
									shoot = 74;
								}
								if (shoot == 74 && Main.projectile[num69].type == 74)
								{
									canShoot = false;
								}
							}
						}
					}
					if (canShoot)
					{
						KnockBack = GetWeaponKnockback(ıtem, KnockBack);
						if (shoot == 228)
						{
							KnockBack = 0f;
						}
						if (shoot == 1 && ıtem.type == 120)
						{
							shoot = 2;
						}
						if (ıtem.type == 682)
						{
							shoot = 117;
						}
						if (ıtem.type == 725)
						{
							shoot = 120;
						}
						if (ıtem.type == 2796)
						{
							shoot = 442;
						}
						if (ıtem.type == 2223)
						{
							shoot = 357;
						}
						itemTime = ıtem.useTime;
						Vector2 vector = RotatedRelativePoint(MountedCenter);
						Vector2 value = Vector2.UnitX.RotatedBy(fullRotation);
						Vector2 vector2 = Main.MouseWorld - vector;
						if (vector2 != Vector2.Zero)
						{
							vector2.Normalize();
						}
						float num70 = Vector2.Dot(value, vector2);
						if (num70 > 0f)
						{
							ChangeDir(1);
						}
						else
						{
							ChangeDir(-1);
						}
						if (ıtem.type == 3094 || ıtem.type == 3378 || ıtem.type == 3543)
						{
							vector.Y = base.position.Y + (float)(height / 3);
						}
						if (shoot == 9)
						{
							vector = new Vector2(base.position.X + (float)width * 0.5f + (float)(Main.rand.Next(201) * -direction) + ((float)Main.mouseX + Main.screenPosition.X - base.position.X), MountedCenter.Y - 600f);
							KnockBack = 0f;
							Damage *= 2;
						}
						if (ıtem.type == 986 || ıtem.type == 281)
						{
							vector.X += 6 * direction;
							vector.Y -= 6f * gravDir;
						}
						if (ıtem.type == 3007)
						{
							vector.X -= 4 * direction;
							vector.Y -= 1f * gravDir;
						}
						float num71 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
						float num72 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
						if (gravDir == -1f)
						{
							num72 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
						}
						float num73 = (float)Math.Sqrt(num71 * num71 + num72 * num72);
						float num74 = num73;
						if ((float.IsNaN(num71) && float.IsNaN(num72)) || (num71 == 0f && num72 == 0f))
						{
							num71 = direction;
							num72 = 0f;
							num73 = speed;
						}
						else
						{
							num73 = speed / num73;
						}
						if (ıtem.type == 1929 || ıtem.type == 2270)
						{
							num71 += (float)Main.rand.Next(-50, 51) * 0.03f / num73;
							num72 += (float)Main.rand.Next(-50, 51) * 0.03f / num73;
						}
						num71 *= num73;
						num72 *= num73;
						if (ıtem.type == 757)
						{
							Damage = (int)((float)Damage * 1.25f);
						}
						if (shoot == 250)
						{
							for (int num75 = 0; num75 < 1000; num75++)
							{
								if (Main.projectile[num75].active && Main.projectile[num75].owner == whoAmI && (Main.projectile[num75].type == 250 || Main.projectile[num75].type == 251))
								{
									Main.projectile[num75].Kill();
								}
							}
						}
						if (shoot == 12)
						{
							vector.X += num71 * 3f;
							vector.Y += num72 * 3f;
						}
						if (ıtem.useStyle == 5)
						{
							if (ıtem.type == 3029)
							{
								Vector2 vector3 = new Vector2(num71, num72);
								vector3.X = (float)Main.mouseX + Main.screenPosition.X - vector.X;
								vector3.Y = (float)Main.mouseY + Main.screenPosition.Y - vector.Y - 1000f;
								itemRotation = (float)Math.Atan2(vector3.Y * (float)direction, vector3.X * (float)direction);
								NetMessage.SendData(13, -1, -1, "", whoAmI);
								NetMessage.SendData(41, -1, -1, "", whoAmI);
							}
							else
							{
								itemRotation = (float)Math.Atan2(num72 * (float)direction, num71 * (float)direction) - fullRotation;
								NetMessage.SendData(13, -1, -1, "", whoAmI);
								NetMessage.SendData(41, -1, -1, "", whoAmI);
							}
						}
						if (shoot == 17)
						{
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							if (gravDir == -1f)
							{
								vector.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
							}
						}
						if (shoot == 76)
						{
							shoot += Main.rand.Next(3);
							num74 /= (float)(Main.screenHeight / 2);
							if (num74 > 1f)
							{
								num74 = 1f;
							}
							float num76 = num71 + (float)Main.rand.Next(-40, 41) * 0.01f;
							float num77 = num72 + (float)Main.rand.Next(-40, 41) * 0.01f;
							num76 *= num74 + 0.25f;
							num77 *= num74 + 0.25f;
							int num78 = Projectile.NewProjectile(vector.X, vector.Y, num76, num77, shoot, Damage, KnockBack, i);
							Main.projectile[num78].ai[1] = 1f;
							num74 = num74 * 2f - 1f;
							if (num74 < -1f)
							{
								num74 = -1f;
							}
							if (num74 > 1f)
							{
								num74 = 1f;
							}
							Main.projectile[num78].ai[0] = num74;
							NetMessage.SendData(27, -1, -1, "", num78);
						}
						else if (ıtem.type == 3029)
						{
							int num79 = 3;
							if (Main.rand.Next(3) == 0)
							{
								num79++;
							}
							for (int num80 = 0; num80 < num79; num80++)
							{
								vector = new Vector2(base.position.X + (float)width * 0.5f + (float)(Main.rand.Next(201) * -direction) + ((float)Main.mouseX + Main.screenPosition.X - base.position.X), MountedCenter.Y - 600f);
								vector.X = (vector.X * 10f + base.Center.X) / 11f + (float)Main.rand.Next(-100, 101);
								vector.Y -= 150 * num80;
								num71 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
								num72 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
								if (num72 < 0f)
								{
									num72 *= -1f;
								}
								if (num72 < 20f)
								{
									num72 = 20f;
								}
								num73 = (float)Math.Sqrt(num71 * num71 + num72 * num72);
								num73 = speed / num73;
								num71 *= num73;
								num72 *= num73;
								float num81 = num71 + (float)Main.rand.Next(-40, 41) * 0.03f;
								float speedY = num72 + (float)Main.rand.Next(-40, 41) * 0.03f;
								num81 *= (float)Main.rand.Next(75, 150) * 0.01f;
								vector.X += Main.rand.Next(-50, 51);
								int num82 = Projectile.NewProjectile(vector.X, vector.Y, num81, speedY, shoot, Damage, KnockBack, i);
								Main.projectile[num82].noDropItem = true;
							}
						}
						else if (ıtem.type == 98 || ıtem.type == 533)
						{
							float speedX = num71 + (float)Main.rand.Next(-40, 41) * 0.01f;
							float speedY2 = num72 + (float)Main.rand.Next(-40, 41) * 0.01f;
							Projectile.NewProjectile(vector.X, vector.Y, speedX, speedY2, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 1319)
						{
							float speedX2 = num71 + (float)Main.rand.Next(-40, 41) * 0.02f;
							float speedY3 = num72 + (float)Main.rand.Next(-40, 41) * 0.02f;
							int num83 = Projectile.NewProjectile(vector.X, vector.Y, speedX2, speedY3, shoot, Damage, KnockBack, i);
							Main.projectile[num83].ranged = true;
							Main.projectile[num83].thrown = false;
						}
						else if (ıtem.type == 3107)
						{
							float speedX3 = num71 + (float)Main.rand.Next(-40, 41) * 0.02f;
							float speedY4 = num72 + (float)Main.rand.Next(-40, 41) * 0.02f;
							Projectile.NewProjectile(vector.X, vector.Y, speedX3, speedY4, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 3053)
						{
							Vector2 value2 = new Vector2(num71, num72);
							value2.Normalize();
							Vector2 value3 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							value3.Normalize();
							value2 = value2 * 4f + value3;
							value2.Normalize();
							value2 *= ıtem.shootSpeed;
							float num84 = (float)Main.rand.Next(10, 80) * 0.001f;
							if (Main.rand.Next(2) == 0)
							{
								num84 *= -1f;
							}
							float num85 = (float)Main.rand.Next(10, 80) * 0.001f;
							if (Main.rand.Next(2) == 0)
							{
								num85 *= -1f;
							}
							Projectile.NewProjectile(vector.X, vector.Y, value2.X, value2.Y, shoot, Damage, KnockBack, i, num85, num84);
						}
						else if (ıtem.type == 3019)
						{
							Vector2 vector4 = new Vector2(num71, num72);
							float num86 = vector4.Length();
							vector4.X += (float)Main.rand.Next(-100, 101) * 0.01f * num86 * 0.15f;
							vector4.Y += (float)Main.rand.Next(-100, 101) * 0.01f * num86 * 0.15f;
							float num87 = num71 + (float)Main.rand.Next(-40, 41) * 0.03f;
							float num88 = num72 + (float)Main.rand.Next(-40, 41) * 0.03f;
							vector4.Normalize();
							vector4 *= num86;
							num87 *= (float)Main.rand.Next(50, 150) * 0.01f;
							num88 *= (float)Main.rand.Next(50, 150) * 0.01f;
							Vector2 vector5 = new Vector2(num87, num88);
							vector5.X += (float)Main.rand.Next(-100, 101) * 0.025f;
							vector5.Y += (float)Main.rand.Next(-100, 101) * 0.025f;
							vector5.Normalize();
							vector5 *= num86;
							num87 = vector5.X;
							num88 = vector5.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num87, num88, shoot, Damage, KnockBack, i, vector4.X, vector4.Y);
						}
						else if (ıtem.type == 2797)
						{
							Vector2 vector6 = Vector2.Normalize(new Vector2(num71, num72)) * 40f * ıtem.scale;
							if (Collision.CanHit(vector, 0, 0, vector + vector6, 0, 0))
							{
								vector += vector6;
							}
							float ai = new Vector2(num71, num72).ToRotation();
							float num89 = (float)Math.PI * 2f / 3f;
							int num90 = Main.rand.Next(4, 5);
							if (Main.rand.Next(4) == 0)
							{
								num90++;
							}
							for (int num91 = 0; num91 < num90; num91++)
							{
								float scaleFactor2 = (float)Main.rand.NextDouble() * 0.2f + 0.05f;
								Vector2 vector7 = new Vector2(num71, num72).RotatedBy(num89 * (float)Main.rand.NextDouble() - num89 / 2f) * scaleFactor2;
								int num92 = Projectile.NewProjectile(vector.X, vector.Y, vector7.X, vector7.Y, 444, Damage, KnockBack, i, ai);
								Main.projectile[num92].localAI[0] = shoot;
								Main.projectile[num92].localAI[1] = speed;
							}
						}
						else if (ıtem.type == 2270)
						{
							float num93 = num71 + (float)Main.rand.Next(-40, 41) * 0.05f;
							float num94 = num72 + (float)Main.rand.Next(-40, 41) * 0.05f;
							if (Main.rand.Next(3) == 0)
							{
								num93 *= 1f + (float)Main.rand.Next(-30, 31) * 0.02f;
								num94 *= 1f + (float)Main.rand.Next(-30, 31) * 0.02f;
							}
							Projectile.NewProjectile(vector.X, vector.Y, num93, num94, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 1930)
						{
							int num95 = 2 + Main.rand.Next(3);
							for (int num96 = 0; num96 < num95; num96++)
							{
								float num97 = num71;
								float num98 = num72;
								float num99 = 0.025f * (float)num96;
								num97 += (float)Main.rand.Next(-35, 36) * num99;
								num98 += (float)Main.rand.Next(-35, 36) * num99;
								num73 = (float)Math.Sqrt(num97 * num97 + num98 * num98);
								num73 = speed / num73;
								num97 *= num73;
								num98 *= num73;
								float x2 = vector.X + num71 * (float)(num95 - num96) * 1.75f;
								float y2 = vector.Y + num72 * (float)(num95 - num96) * 1.75f;
								Projectile.NewProjectile(x2, y2, num97, num98, shoot, Damage, KnockBack, i, Main.rand.Next(0, 10 * (num96 + 1)));
							}
						}
						else if (ıtem.type == 1931)
						{
							int num100 = 2;
							for (int num101 = 0; num101 < num100; num101++)
							{
								vector = new Vector2(base.position.X + (float)width * 0.5f + (float)(Main.rand.Next(201) * -direction) + ((float)Main.mouseX + Main.screenPosition.X - base.position.X), MountedCenter.Y - 600f);
								vector.X = (vector.X + base.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
								vector.Y -= 100 * num101;
								num71 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
								num72 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
								if (num72 < 0f)
								{
									num72 *= -1f;
								}
								if (num72 < 20f)
								{
									num72 = 20f;
								}
								num73 = (float)Math.Sqrt(num71 * num71 + num72 * num72);
								num73 = speed / num73;
								num71 *= num73;
								num72 *= num73;
								float speedX4 = num71 + (float)Main.rand.Next(-40, 41) * 0.02f;
								float speedY5 = num72 + (float)Main.rand.Next(-40, 41) * 0.02f;
								Projectile.NewProjectile(vector.X, vector.Y, speedX4, speedY5, shoot, Damage, KnockBack, i, 0f, Main.rand.Next(5));
							}
						}
						else if (ıtem.type == 2750)
						{
							int num102 = 1;
							for (int num103 = 0; num103 < num102; num103++)
							{
								vector = new Vector2(base.position.X + (float)width * 0.5f + (float)(Main.rand.Next(201) * -direction) + ((float)Main.mouseX + Main.screenPosition.X - base.position.X), MountedCenter.Y - 600f);
								vector.X = (vector.X + base.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
								vector.Y -= 100 * num103;
								num71 = (float)Main.mouseX + Main.screenPosition.X - vector.X + (float)Main.rand.Next(-40, 41) * 0.03f;
								num72 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
								if (num72 < 0f)
								{
									num72 *= -1f;
								}
								if (num72 < 20f)
								{
									num72 = 20f;
								}
								num73 = (float)Math.Sqrt(num71 * num71 + num72 * num72);
								num73 = speed / num73;
								num71 *= num73;
								num72 *= num73;
								float num104 = num71;
								float num105 = num72 + (float)Main.rand.Next(-40, 41) * 0.02f;
								Projectile.NewProjectile(vector.X, vector.Y, num104 * 0.75f, num105 * 0.75f, shoot + Main.rand.Next(3), Damage, KnockBack, i, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
							}
						}
						else if (ıtem.type == 3570)
						{
							int num106 = 3;
							for (int num107 = 0; num107 < num106; num107++)
							{
								vector = new Vector2(base.position.X + (float)width * 0.5f + (float)(Main.rand.Next(201) * -direction) + ((float)Main.mouseX + Main.screenPosition.X - base.position.X), MountedCenter.Y - 600f);
								vector.X = (vector.X + base.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
								vector.Y -= 100 * num107;
								num71 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
								num72 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
								float ai2 = num72 + vector.Y;
								if (num72 < 0f)
								{
									num72 *= -1f;
								}
								if (num72 < 20f)
								{
									num72 = 20f;
								}
								num73 = (float)Math.Sqrt(num71 * num71 + num72 * num72);
								num73 = speed / num73;
								num71 *= num73;
								num72 *= num73;
								Vector2 vector8 = new Vector2(num71, num72) / 2f;
								Projectile.NewProjectile(vector.X, vector.Y, vector8.X, vector8.Y, shoot, Damage, KnockBack, i, 0f, ai2);
							}
						}
						else if (ıtem.type == 3065)
						{
							Vector2 value4 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
							float num108 = value4.Y;
							if (num108 > base.Center.Y - 200f)
							{
								num108 = base.Center.Y - 200f;
							}
							for (int num109 = 0; num109 < 3; num109++)
							{
								vector = base.Center + new Vector2(-Main.rand.Next(0, 401) * direction, -600f);
								vector.Y -= 100 * num109;
								Vector2 vector9 = value4 - vector;
								if (vector9.Y < 0f)
								{
									vector9.Y *= -1f;
								}
								if (vector9.Y < 20f)
								{
									vector9.Y = 20f;
								}
								vector9.Normalize();
								vector9 *= speed;
								num71 = vector9.X;
								num72 = vector9.Y;
								float speedX5 = num71;
								float speedY6 = num72 + (float)Main.rand.Next(-40, 41) * 0.02f;
								Projectile.NewProjectile(vector.X, vector.Y, speedX5, speedY6, shoot, Damage * 2, KnockBack, i, 0f, num108);
							}
						}
						else if (ıtem.type == 2624)
						{
							float num110 = (float)Math.PI / 10f;
							int num111 = 5;
							Vector2 vector10 = new Vector2(num71, num72);
							vector10.Normalize();
							vector10 *= 40f;
							bool flag9 = Collision.CanHit(vector, 0, 0, vector + vector10, 0, 0);
							for (int num112 = 0; num112 < num111; num112++)
							{
								float num113 = (float)num112 - ((float)num111 - 1f) / 2f;
								Vector2 vector11 = vector10.RotatedBy(num110 * num113);
								if (!flag9)
								{
									vector11 -= vector10;
								}
								int num114 = Projectile.NewProjectile(vector.X + vector11.X, vector.Y + vector11.Y, num71, num72, shoot, Damage, KnockBack, i);
								Main.projectile[num114].noDropItem = true;
							}
						}
						else if (ıtem.type == 1929)
						{
							float speedX6 = num71 + (float)Main.rand.Next(-40, 41) * 0.03f;
							float speedY7 = num72 + (float)Main.rand.Next(-40, 41) * 0.03f;
							Projectile.NewProjectile(vector.X, vector.Y, speedX6, speedY7, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 1553)
						{
							float speedX7 = num71 + (float)Main.rand.Next(-40, 41) * 0.005f;
							float speedY8 = num72 + (float)Main.rand.Next(-40, 41) * 0.005f;
							Projectile.NewProjectile(vector.X, vector.Y, speedX7, speedY8, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 518)
						{
							float num115 = num71;
							float num116 = num72;
							num115 += (float)Main.rand.Next(-40, 41) * 0.04f;
							num116 += (float)Main.rand.Next(-40, 41) * 0.04f;
							Projectile.NewProjectile(vector.X, vector.Y, num115, num116, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 1265)
						{
							float num117 = num71;
							float num118 = num72;
							num117 += (float)Main.rand.Next(-30, 31) * 0.03f;
							num118 += (float)Main.rand.Next(-30, 31) * 0.03f;
							Projectile.NewProjectile(vector.X, vector.Y, num117, num118, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 534)
						{
							int num119 = Main.rand.Next(4, 6);
							for (int num120 = 0; num120 < num119; num120++)
							{
								float num121 = num71;
								float num122 = num72;
								num121 += (float)Main.rand.Next(-40, 41) * 0.05f;
								num122 += (float)Main.rand.Next(-40, 41) * 0.05f;
								Projectile.NewProjectile(vector.X, vector.Y, num121, num122, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 2188)
						{
							int num123 = 4;
							if (Main.rand.Next(3) == 0)
							{
								num123++;
							}
							if (Main.rand.Next(4) == 0)
							{
								num123++;
							}
							if (Main.rand.Next(5) == 0)
							{
								num123++;
							}
							for (int num124 = 0; num124 < num123; num124++)
							{
								float num125 = num71;
								float num126 = num72;
								float num127 = 0.05f * (float)num124;
								num125 += (float)Main.rand.Next(-35, 36) * num127;
								num126 += (float)Main.rand.Next(-35, 36) * num127;
								num73 = (float)Math.Sqrt(num125 * num125 + num126 * num126);
								num73 = speed / num73;
								num125 *= num73;
								num126 *= num73;
								float x3 = vector.X;
								float y3 = vector.Y;
								Projectile.NewProjectile(x3, y3, num125, num126, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 1308)
						{
							int num128 = 3;
							if (Main.rand.Next(3) == 0)
							{
								num128++;
							}
							for (int num129 = 0; num129 < num128; num129++)
							{
								float num130 = num71;
								float num131 = num72;
								float num132 = 0.05f * (float)num129;
								num130 += (float)Main.rand.Next(-35, 36) * num132;
								num131 += (float)Main.rand.Next(-35, 36) * num132;
								num73 = (float)Math.Sqrt(num130 * num130 + num131 * num131);
								num73 = speed / num73;
								num130 *= num73;
								num131 *= num73;
								float x4 = vector.X;
								float y4 = vector.Y;
								Projectile.NewProjectile(x4, y4, num130, num131, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 1258)
						{
							float num133 = num71;
							float num134 = num72;
							num133 += (float)Main.rand.Next(-40, 41) * 0.01f;
							num134 += (float)Main.rand.Next(-40, 41) * 0.01f;
							vector.X += (float)Main.rand.Next(-40, 41) * 0.05f;
							vector.Y += (float)Main.rand.Next(-45, 36) * 0.05f;
							Projectile.NewProjectile(vector.X, vector.Y, num133, num134, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 964)
						{
							int num135 = Main.rand.Next(3, 5);
							for (int num136 = 0; num136 < num135; num136++)
							{
								float num137 = num71;
								float num138 = num72;
								num137 += (float)Main.rand.Next(-35, 36) * 0.04f;
								num138 += (float)Main.rand.Next(-35, 36) * 0.04f;
								Projectile.NewProjectile(vector.X, vector.Y, num137, num138, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 1569)
						{
							int num139 = 4;
							if (Main.rand.Next(2) == 0)
							{
								num139++;
							}
							if (Main.rand.Next(4) == 0)
							{
								num139++;
							}
							if (Main.rand.Next(8) == 0)
							{
								num139++;
							}
							if (Main.rand.Next(16) == 0)
							{
								num139++;
							}
							for (int num140 = 0; num140 < num139; num140++)
							{
								float num141 = num71;
								float num142 = num72;
								float num143 = 0.05f * (float)num140;
								num141 += (float)Main.rand.Next(-35, 36) * num143;
								num142 += (float)Main.rand.Next(-35, 36) * num143;
								num73 = (float)Math.Sqrt(num141 * num141 + num142 * num142);
								num73 = speed / num73;
								num141 *= num73;
								num142 *= num73;
								float x5 = vector.X;
								float y5 = vector.Y;
								Projectile.NewProjectile(x5, y5, num141, num142, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 1572 || ıtem.type == 2366 || ıtem.type == 3571 || ıtem.type == 3569)
						{
							int shoot2 = ıtem.shoot;
							for (int num144 = 0; num144 < 1000; num144++)
							{
								if (Main.projectile[num144].owner == whoAmI && Main.projectile[num144].type == shoot2)
								{
									Main.projectile[num144].Kill();
								}
							}
							bool flag10 = ıtem.type == 3571 || ıtem.type == 3569;
							int num145 = (int)((float)Main.mouseX + Main.screenPosition.X) / 16;
							int num146 = (int)((float)Main.mouseY + Main.screenPosition.Y) / 16;
							if (gravDir == -1f)
							{
								num146 = (int)(Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY) / 16;
							}
							if (!flag10)
							{
								for (; num146 < Main.maxTilesY - 10 && Main.tile[num145, num146] != null && !WorldGen.SolidTile2(num145, num146) && Main.tile[num145 - 1, num146] != null && !WorldGen.SolidTile2(num145 - 1, num146) && Main.tile[num145 + 1, num146] != null && !WorldGen.SolidTile2(num145 + 1, num146); num146++)
								{
								}
								num146--;
							}
							Projectile.NewProjectile((float)Main.mouseX + Main.screenPosition.X, num146 * 16 - 24, 0f, 15f, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 1244 || ıtem.type == 1256)
						{
							int num147 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
							Main.projectile[num147].ai[0] = (float)Main.mouseX + Main.screenPosition.X;
							Main.projectile[num147].ai[1] = (float)Main.mouseY + Main.screenPosition.Y;
						}
						else if (ıtem.type == 1229)
						{
							int num148 = Main.rand.Next(2, 4);
							if (Main.rand.Next(5) == 0)
							{
								num148++;
							}
							for (int num149 = 0; num149 < num148; num149++)
							{
								float num150 = num71;
								float num151 = num72;
								if (num149 > 0)
								{
									num150 += (float)Main.rand.Next(-35, 36) * 0.04f;
									num151 += (float)Main.rand.Next(-35, 36) * 0.04f;
								}
								if (num149 > 1)
								{
									num150 += (float)Main.rand.Next(-35, 36) * 0.04f;
									num151 += (float)Main.rand.Next(-35, 36) * 0.04f;
								}
								if (num149 > 2)
								{
									num150 += (float)Main.rand.Next(-35, 36) * 0.04f;
									num151 += (float)Main.rand.Next(-35, 36) * 0.04f;
								}
								int num152 = Projectile.NewProjectile(vector.X, vector.Y, num150, num151, shoot, Damage, KnockBack, i);
								Main.projectile[num152].noDropItem = true;
							}
						}
						else if (ıtem.type == 1121)
						{
							int num153 = Main.rand.Next(1, 4);
							if (Main.rand.Next(6) == 0)
							{
								num153++;
							}
							if (Main.rand.Next(6) == 0)
							{
								num153++;
							}
							if (strongBees && Main.rand.Next(3) == 0)
							{
								num153++;
							}
							for (int num154 = 0; num154 < num153; num154++)
							{
								float num155 = num71;
								float num156 = num72;
								num155 += (float)Main.rand.Next(-35, 36) * 0.02f;
								num156 += (float)Main.rand.Next(-35, 36) * 0.02f;
								int num157 = Projectile.NewProjectile(vector.X, vector.Y, num155, num156, beeType(), beeDamage(Damage), beeKB(KnockBack), i);
								Main.projectile[num157].magic = true;
							}
						}
						else if (ıtem.type == 1155)
						{
							int num158 = Main.rand.Next(2, 5);
							if (Main.rand.Next(5) == 0)
							{
								num158++;
							}
							if (Main.rand.Next(5) == 0)
							{
								num158++;
							}
							for (int num159 = 0; num159 < num158; num159++)
							{
								float num160 = num71;
								float num161 = num72;
								num160 += (float)Main.rand.Next(-35, 36) * 0.02f;
								num161 += (float)Main.rand.Next(-35, 36) * 0.02f;
								Projectile.NewProjectile(vector.X, vector.Y, num160, num161, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 1801)
						{
							int num162 = Main.rand.Next(1, 4);
							for (int num163 = 0; num163 < num162; num163++)
							{
								float num164 = num71;
								float num165 = num72;
								num164 += (float)Main.rand.Next(-35, 36) * 0.05f;
								num165 += (float)Main.rand.Next(-35, 36) * 0.05f;
								Projectile.NewProjectile(vector.X, vector.Y, num164, num165, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 679)
						{
							for (int num166 = 0; num166 < 6; num166++)
							{
								float num167 = num71;
								float num168 = num72;
								num167 += (float)Main.rand.Next(-40, 41) * 0.05f;
								num168 += (float)Main.rand.Next(-40, 41) * 0.05f;
								Projectile.NewProjectile(vector.X, vector.Y, num167, num168, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 2623)
						{
							for (int num169 = 0; num169 < 3; num169++)
							{
								float num170 = num71;
								float num171 = num72;
								num170 += (float)Main.rand.Next(-40, 41) * 0.1f;
								num171 += (float)Main.rand.Next(-40, 41) * 0.1f;
								Projectile.NewProjectile(vector.X, vector.Y, num170, num171, shoot, Damage, KnockBack, i);
							}
						}
						else if (ıtem.type == 3210)
						{
							Vector2 vector12 = new Vector2(num71, num72);
							vector12.X += (float)Main.rand.Next(-30, 31) * 0.04f;
							vector12.Y += (float)Main.rand.Next(-30, 31) * 0.03f;
							vector12.Normalize();
							vector12 *= (float)Main.rand.Next(70, 91) * 0.1f;
							vector12.X += (float)Main.rand.Next(-30, 31) * 0.04f;
							vector12.Y += (float)Main.rand.Next(-30, 31) * 0.03f;
							Projectile.NewProjectile(vector.X, vector.Y, vector12.X, vector12.Y, shoot, Damage, KnockBack, i, Main.rand.Next(20));
						}
						else if (ıtem.type == 434)
						{
							float num172 = num71;
							float num173 = num72;
							if (itemAnimation < 5)
							{
								num172 += (float)Main.rand.Next(-40, 41) * 0.01f;
								num173 += (float)Main.rand.Next(-40, 41) * 0.01f;
								num172 *= 1.1f;
								num173 *= 1.1f;
							}
							else if (itemAnimation < 10)
							{
								num172 += (float)Main.rand.Next(-20, 21) * 0.01f;
								num173 += (float)Main.rand.Next(-20, 21) * 0.01f;
								num172 *= 1.05f;
								num173 *= 1.05f;
							}
							Projectile.NewProjectile(vector.X, vector.Y, num172, num173, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 1157)
						{
							shoot = Main.rand.Next(191, 195);
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							int num174 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
							Main.projectile[num174].localAI[0] = 30f;
						}
						else if (ıtem.type == 1802)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 2364 || ıtem.type == 2365)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 2535)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Vector2 spinningpoint = new Vector2(num71, num72);
							spinningpoint = spinningpoint.RotatedBy(1.5707963705062866);
							Projectile.NewProjectile(vector.X + spinningpoint.X, vector.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, shoot, Damage, KnockBack, i);
							spinningpoint = spinningpoint.RotatedBy(-3.1415927410125732);
							Projectile.NewProjectile(vector.X + spinningpoint.X, vector.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, shoot + 1, Damage, KnockBack, i);
						}
						else if (ıtem.type == 2551)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + Main.rand.Next(3), Damage, KnockBack, i);
						}
						else if (ıtem.type == 2584)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + Main.rand.Next(3), Damage, KnockBack, i);
						}
						else if (ıtem.type == 2621)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 2749 || ıtem.type == 3249 || ıtem.type == 3474)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 3531)
						{
							int num175 = -1;
							int num176 = -1;
							for (int num177 = 0; num177 < 1000; num177++)
							{
								if (Main.projectile[num177].active && Main.projectile[num177].owner == Main.myPlayer)
								{
									if (num175 == -1 && Main.projectile[num177].type == 625)
									{
										num175 = num177;
									}
									if (num176 == -1 && Main.projectile[num177].type == 628)
									{
										num176 = num177;
									}
									if (num175 != -1 && num176 != -1)
									{
										break;
									}
								}
							}
							if (num175 == -1 && num176 == -1)
							{
								num71 = 0f;
								num72 = 0f;
								vector.X = (float)Main.mouseX + Main.screenPosition.X;
								vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
								int num178 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
								num178 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + 1, Damage, KnockBack, i, num178);
								int num179 = num178;
								num178 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + 2, Damage, KnockBack, i, num178);
								Main.projectile[num179].localAI[1] = num178;
								num179 = num178;
								num178 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + 3, Damage, KnockBack, i, num178);
								Main.projectile[num179].localAI[1] = num178;
							}
							else if (num175 != -1 && num176 != -1)
							{
								int num180 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + 1, Damage, KnockBack, i, Projectile.GetByUUID(Main.myPlayer, Main.projectile[num176].ai[0]));
								int num181 = num180;
								num180 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot + 2, Damage, KnockBack, i, num180);
								Main.projectile[num181].localAI[1] = num180;
								Main.projectile[num181].netUpdate = true;
								Main.projectile[num181].ai[1] = 1f;
								Main.projectile[num180].localAI[1] = num176;
								Main.projectile[num180].netUpdate = true;
								Main.projectile[num180].ai[1] = 1f;
								Main.projectile[num176].ai[0] = Main.projectile[num180].projUUID;
								Main.projectile[num176].netUpdate = true;
								Main.projectile[num176].ai[1] = 1f;
							}
						}
						else if (ıtem.type == 1309)
						{
							num71 = 0f;
							num72 = 0f;
							vector.X = (float)Main.mouseX + Main.screenPosition.X;
							vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.shoot > 0 && (Main.projPet[ıtem.shoot] || ıtem.shoot == 72 || ıtem.shoot == 18 || ıtem.shoot == 500 || ıtem.shoot == 650) && !ıtem.summon)
						{
							for (int num182 = 0; num182 < 1000; num182++)
							{
								if (!Main.projectile[num182].active || Main.projectile[num182].owner != whoAmI)
								{
									continue;
								}
								if (ıtem.shoot == 72)
								{
									if (Main.projectile[num182].type == 72 || Main.projectile[num182].type == 86 || Main.projectile[num182].type == 87)
									{
										Main.projectile[num182].Kill();
									}
								}
								else if (ıtem.shoot == Main.projectile[num182].type)
								{
									Main.projectile[num182].Kill();
								}
							}
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 3006)
						{
							Vector2 vector13 = default(Vector2);
							vector13.X = (float)Main.mouseX + Main.screenPosition.X;
							vector13.Y = (float)Main.mouseY + Main.screenPosition.Y;
							while (Collision.CanHitLine(base.position, width, height, vector, 1, 1))
							{
								vector.X += num71;
								vector.Y += num72;
								if ((vector - vector13).Length() < 20f + Math.Abs(num71) + Math.Abs(num72))
								{
									vector = vector13;
									break;
								}
							}
							Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 3014)
						{
							Vector2 vector14 = default(Vector2);
							vector14.X = (float)Main.mouseX + Main.screenPosition.X;
							vector14.Y = (float)Main.mouseY + Main.screenPosition.Y;
							while (Collision.CanHitLine(base.position, width, height, vector, 1, 1))
							{
								vector.X += num71;
								vector.Y += num72;
								if ((vector - vector14).Length() < 20f + Math.Abs(num71) + Math.Abs(num72))
								{
									vector = vector14;
									break;
								}
							}
							bool flag11 = false;
							int num183 = (int)vector.Y / 16;
							int i2 = (int)vector.X / 16;
							int num184;
							for (num184 = num183; num183 < Main.maxTilesY - 10 && num183 - num184 < 30 && !WorldGen.SolidTile(i2, num183); num183++)
							{
							}
							if (!WorldGen.SolidTile(i2, num183))
							{
								flag11 = true;
							}
							float num185 = num183 * 16;
							num183 = num184;
							while (num183 > 10 && num184 - num183 < 30 && !WorldGen.SolidTile(i2, num183))
							{
								num183--;
							}
							float num186 = num183 * 16 + 16;
							float num187 = num185 - num186;
							int num188 = 10;
							if (num187 > (float)(16 * num188))
							{
								num187 = 16 * num188;
							}
							num186 = num185 - num187;
							vector.X = (int)(vector.X / 16f) * 16;
							if (!flag11)
							{
								Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, shoot, Damage, KnockBack, i, num186, num187);
							}
						}
						else if (ıtem.type == 3384)
						{
							int num189 = (altFunctionUse == 2) ? 1 : 0;
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i, 0f, num189);
						}
						else if (ıtem.type == 3473)
						{
							float ai3 = (Main.rand.NextFloat() - 0.5f) * ((float)Math.PI / 4f);
							Vector2 vector15 = new Vector2(num71, num72);
							Projectile.NewProjectile(vector.X, vector.Y, vector15.X, vector15.Y, shoot, Damage, KnockBack, i, 0f, ai3);
						}
						else if (ıtem.type == 3542)
						{
							float num190 = (Main.rand.NextFloat() - 0.5f) * ((float)Math.PI / 4f) * 0.7f;
							for (int num191 = 0; num191 < 10; num191++)
							{
								if (Collision.CanHit(vector, 0, 0, vector + new Vector2(num71, num72).RotatedBy(num190) * 100f, 0, 0))
								{
									break;
								}
								num190 = (Main.rand.NextFloat() - 0.5f) * ((float)Math.PI / 4f) * 0.7f;
							}
							Vector2 vector16 = new Vector2(num71, num72).RotatedBy(num190) * (0.95f + Main.rand.NextFloat() * 0.3f);
							Projectile.NewProjectile(vector.X, vector.Y, vector16.X, vector16.Y, shoot, Damage, KnockBack, i);
						}
						else if (ıtem.type == 3475)
						{
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, 615, Damage, KnockBack, i, 5 * Main.rand.Next(0, 20));
						}
						else if (ıtem.type == 3540)
						{
							Projectile.NewProjectile(vector.X, vector.Y, num71, num72, 630, Damage, KnockBack, i);
						}
						else if (ıtem.type == 3546)
						{
							for (int num192 = 0; num192 < 2; num192++)
							{
								float num193 = num71;
								float num194 = num72;
								num193 += (float)Main.rand.Next(-40, 41) * 0.05f;
								num194 += (float)Main.rand.Next(-40, 41) * 0.05f;
								Vector2 vector17 = vector + Vector2.Normalize(new Vector2(num193, num194).RotatedBy(-(float)Math.PI / 2f * (float)direction)) * 6f;
								Projectile.NewProjectile(vector17.X, vector17.Y, num193, num194, 167 + Main.rand.Next(4), Damage, KnockBack, i, 0f, 1f);
							}
						}
						else if (ıtem.type == 3350)
						{
							float num195 = num71;
							float num196 = num72;
							num195 += (float)Main.rand.Next(-1, 2) * 0.5f;
							num196 += (float)Main.rand.Next(-1, 2) * 0.5f;
							if (Collision.CanHitLine(base.Center, 0, 0, vector + new Vector2(num195, num196) * 2f, 0, 0))
							{
								vector += new Vector2(num195, num196);
							}
							Projectile.NewProjectile(vector.X, vector.Y - gravDir * 4f, num195, num196, shoot, Damage, KnockBack, i, 0f, (float)Main.rand.Next(12) / 6f);
						}
						else
						{
							int num197 = Projectile.NewProjectile(vector.X, vector.Y, num71, num72, shoot, Damage, KnockBack, i);
							if (ıtem.type == 726)
							{
								Main.projectile[num197].magic = true;
							}
							if (ıtem.type == 724 || ıtem.type == 676)
							{
								Main.projectile[num197].melee = true;
							}
							if (shoot == 80)
							{
								Main.projectile[num197].ai[0] = tileTargetX;
								Main.projectile[num197].ai[1] = tileTargetY;
							}
							if (shoot == 442)
							{
								Main.projectile[num197].ai[0] = tileTargetX;
								Main.projectile[num197].ai[1] = tileTargetY;
							}
							if ((thrownCost50 || thrownCost33) && inventory[selectedItem].thrown)
							{
								Main.projectile[num197].noDropItem = true;
							}
							if (Main.projectile[num197].aiStyle == 99)
							{
								AchievementsHelper.HandleSpecialEvent(this, 7);
							}
						}
					}
					else if (ıtem.useStyle == 5)
					{
						itemRotation = 0f;
						NetMessage.SendData(41, -1, -1, "", whoAmI);
					}
				}
				if (whoAmI == Main.myPlayer && (ıtem.type == 509 || ıtem.type == 510 || ıtem.type == 849 || ıtem.type == 850 || ıtem.type == 851) && base.position.X / 16f - (float)tileRangeX - (float)ıtem.tileBoost - (float)blockRange <= (float)tileTargetX && (base.position.X + (float)width) / 16f + (float)tileRangeX + (float)ıtem.tileBoost - 1f + (float)blockRange >= (float)tileTargetX && base.position.Y / 16f - (float)tileRangeY - (float)ıtem.tileBoost - (float)blockRange <= (float)tileTargetY && (base.position.Y + (float)height) / 16f + (float)tileRangeY + (float)ıtem.tileBoost - 2f + (float)blockRange >= (float)tileTargetY)
				{
					showItemIcon = true;
					if (itemAnimation > 0 && itemTime == 0 && controlUseItem)
					{
						int i3 = tileTargetX;
						int j2 = tileTargetY;
						if (ıtem.type == 509)
						{
							int num198 = -1;
							for (int num199 = 0; num199 < 58; num199++)
							{
								if (inventory[num199].stack > 0 && inventory[num199].type == 530)
								{
									num198 = num199;
									break;
								}
							}
							if (num198 >= 0 && WorldGen.PlaceWire(i3, j2))
							{
								inventory[num198].stack--;
								if (inventory[num198].stack <= 0)
								{
									inventory[num198].SetDefaults();
								}
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 5, tileTargetX, tileTargetY);
							}
						}
						else if (ıtem.type == 850)
						{
							int num200 = -1;
							for (int num201 = 0; num201 < 58; num201++)
							{
								if (inventory[num201].stack > 0 && inventory[num201].type == 530)
								{
									num200 = num201;
									break;
								}
							}
							if (num200 >= 0 && WorldGen.PlaceWire2(i3, j2))
							{
								inventory[num200].stack--;
								if (inventory[num200].stack <= 0)
								{
									inventory[num200].SetDefaults();
								}
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 10, tileTargetX, tileTargetY);
							}
						}
						if (ıtem.type == 851)
						{
							int num202 = -1;
							for (int num203 = 0; num203 < 58; num203++)
							{
								if (inventory[num203].stack > 0 && inventory[num203].type == 530)
								{
									num202 = num203;
									break;
								}
							}
							if (num202 >= 0 && WorldGen.PlaceWire3(i3, j2))
							{
								inventory[num202].stack--;
								if (inventory[num202].stack <= 0)
								{
									inventory[num202].SetDefaults();
								}
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 12, tileTargetX, tileTargetY);
							}
						}
						else if (ıtem.type == 510)
						{
							if (WorldGen.KillActuator(i3, j2))
							{
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 9, tileTargetX, tileTargetY);
							}
							else if (WorldGen.KillWire3(i3, j2))
							{
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 13, tileTargetX, tileTargetY);
							}
							else if (WorldGen.KillWire2(i3, j2))
							{
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 11, tileTargetX, tileTargetY);
							}
							else if (WorldGen.KillWire(i3, j2))
							{
								itemTime = ıtem.useTime;
								NetMessage.SendData(17, -1, -1, "", 6, tileTargetX, tileTargetY);
							}
						}
						else if (ıtem.type == 849 && ıtem.stack > 0 && WorldGen.PlaceActuator(i3, j2))
						{
							itemTime = ıtem.useTime;
							NetMessage.SendData(17, -1, -1, "", 8, tileTargetX, tileTargetY);
							ıtem.stack--;
							if (ıtem.stack <= 0)
							{
								ıtem.SetDefaults();
							}
						}
					}
				}
				if (itemAnimation > 0 && itemTime == 0 && (ıtem.type == 507 || ıtem.type == 508))
				{
					itemTime = ıtem.useTime;
					Vector2 vector18 = new Vector2(base.position.X + (float)width * 0.5f, base.position.Y + (float)height * 0.5f);
					float num204 = (float)Main.mouseX + Main.screenPosition.X - vector18.X;
					float num205 = (float)Main.mouseY + Main.screenPosition.Y - vector18.Y;
					float num206 = (float)Math.Sqrt(num204 * num204 + num205 * num205);
					num206 /= (float)(Main.screenHeight / 2);
					if (num206 > 1f)
					{
						num206 = 1f;
					}
					num206 = num206 * 2f - 1f;
					if (num206 < -1f)
					{
						num206 = -1f;
					}
					if (num206 > 1f)
					{
						num206 = 1f;
					}
					Main.harpNote = num206;
					int style = 26;
					if (ıtem.type == 507)
					{
						style = 35;
					}
					Main.PlaySound(2, (int)base.position.X, (int)base.position.Y, style);
					NetMessage.SendData(58, -1, -1, "", whoAmI, num206);
				}
				if (((ıtem.type >= 205 && ıtem.type <= 207) || ıtem.type == 1128 || ıtem.type == 3031 || ıtem.type == 3032) && base.position.X / 16f - (float)tileRangeX - (float)ıtem.tileBoost <= (float)tileTargetX && (base.position.X + (float)width) / 16f + (float)tileRangeX + (float)ıtem.tileBoost - 1f >= (float)tileTargetX && base.position.Y / 16f - (float)tileRangeY - (float)ıtem.tileBoost <= (float)tileTargetY && (base.position.Y + (float)height) / 16f + (float)tileRangeY + (float)ıtem.tileBoost - 2f >= (float)tileTargetY)
				{
					showItemIcon = true;
					if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
					{
						if (ıtem.type == 205 || (ıtem.type == 3032 && Main.tile[tileTargetX, tileTargetY].liquidType() == 0))
						{
							int num207 = Main.tile[tileTargetX, tileTargetY].liquidType();
							int num208 = 0;
							for (int num209 = tileTargetX - 1; num209 <= tileTargetX + 1; num209++)
							{
								for (int num210 = tileTargetY - 1; num210 <= tileTargetY + 1; num210++)
								{
									if (Main.tile[num209, num210].liquidType() == num207)
									{
										num208 += Main.tile[num209, num210].liquid;
									}
								}
							}
							if (Main.tile[tileTargetX, tileTargetY].liquid > 0 && (num208 > 100 || ıtem.type == 3032))
							{
								int liquidType = Main.tile[tileTargetX, tileTargetY].liquidType();
								if (ıtem.type != 3032)
								{
									if (!Main.tile[tileTargetX, tileTargetY].lava())
									{
										if (Main.tile[tileTargetX, tileTargetY].honey())
										{
											ıtem.stack--;
											PutItemInInventory(1128, selectedItem);
										}
										else
										{
											ıtem.stack--;
											PutItemInInventory(206, selectedItem);
										}
									}
									else
									{
										ıtem.stack--;
										PutItemInInventory(207, selectedItem);
									}
								}
								Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
								itemTime = ıtem.useTime;
								int num211 = Main.tile[tileTargetX, tileTargetY].liquid;
								Main.tile[tileTargetX, tileTargetY].liquid = 0;
								Main.tile[tileTargetX, tileTargetY].lava(false);
								Main.tile[tileTargetX, tileTargetY].honey(false);
								WorldGen.SquareTileFrame(tileTargetX, tileTargetY, false);
								if (Main.netMode == 1)
								{
									NetMessage.sendWater(tileTargetX, tileTargetY);
								}
								else
								{
									Liquid.AddWater(tileTargetX, tileTargetY);
								}
								for (int num212 = tileTargetX - 1; num212 <= tileTargetX + 1; num212++)
								{
									for (int num213 = tileTargetY - 1; num213 <= tileTargetY + 1; num213++)
									{
										if (num211 < 256 && Main.tile[num212, num213].liquidType() == num207)
										{
											int num214 = Main.tile[num212, num213].liquid;
											if (num214 + num211 > 255)
											{
												num214 = 255 - num211;
											}
											num211 += num214;
											Main.tile[num212, num213].liquid -= (byte)num214;
											Main.tile[num212, num213].liquidType(liquidType);
											if (Main.tile[num212, num213].liquid == 0)
											{
												Main.tile[num212, num213].lava(false);
												Main.tile[num212, num213].honey(false);
											}
											WorldGen.SquareTileFrame(num212, num213, false);
											if (Main.netMode == 1)
											{
												NetMessage.sendWater(num212, num213);
											}
											else
											{
												Liquid.AddWater(num212, num213);
											}
										}
									}
								}
							}
						}
						else if (Main.tile[tileTargetX, tileTargetY].liquid < 200 && (!Main.tile[tileTargetX, tileTargetY].nactive() || !Main.tileSolid[Main.tile[tileTargetX, tileTargetY].type] || Main.tileSolidTop[Main.tile[tileTargetX, tileTargetY].type]))
						{
							if (ıtem.type == 207)
							{
								if (Main.tile[tileTargetX, tileTargetY].liquid == 0 || Main.tile[tileTargetX, tileTargetY].liquidType() == 1)
								{
									Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
									Main.tile[tileTargetX, tileTargetY].liquidType(1);
									Main.tile[tileTargetX, tileTargetY].liquid = byte.MaxValue;
									WorldGen.SquareTileFrame(tileTargetX, tileTargetY);
									ıtem.stack--;
									PutItemInInventory(205, selectedItem);
									itemTime = ıtem.useTime;
									if (Main.netMode == 1)
									{
										NetMessage.sendWater(tileTargetX, tileTargetY);
									}
								}
							}
							else if (ıtem.type == 206 || ıtem.type == 3031)
							{
								if (Main.tile[tileTargetX, tileTargetY].liquid == 0 || Main.tile[tileTargetX, tileTargetY].liquidType() == 0)
								{
									Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
									Main.tile[tileTargetX, tileTargetY].liquidType(0);
									Main.tile[tileTargetX, tileTargetY].liquid = byte.MaxValue;
									WorldGen.SquareTileFrame(tileTargetX, tileTargetY);
									if (ıtem.type != 3031)
									{
										ıtem.stack--;
										PutItemInInventory(205, selectedItem);
									}
									itemTime = ıtem.useTime;
									if (Main.netMode == 1)
									{
										NetMessage.sendWater(tileTargetX, tileTargetY);
									}
								}
							}
							else if (ıtem.type == 1128 && (Main.tile[tileTargetX, tileTargetY].liquid == 0 || Main.tile[tileTargetX, tileTargetY].liquidType() == 2))
							{
								Main.PlaySound(19, (int)base.position.X, (int)base.position.Y);
								Main.tile[tileTargetX, tileTargetY].liquidType(2);
								Main.tile[tileTargetX, tileTargetY].liquid = byte.MaxValue;
								WorldGen.SquareTileFrame(tileTargetX, tileTargetY);
								ıtem.stack--;
								PutItemInInventory(205, selectedItem);
								itemTime = ıtem.useTime;
								if (Main.netMode == 1)
								{
									NetMessage.sendWater(tileTargetX, tileTargetY);
								}
							}
						}
					}
				}
				if (!channel)
				{
					toolTime = itemTime;
				}
				else
				{
					toolTime--;
					if (toolTime < 0)
					{
						if (ıtem.pick > 0)
						{
							toolTime = ıtem.useTime;
						}
						else
						{
							toolTime = (int)((float)ıtem.useTime * pickSpeed);
						}
					}
				}
				if ((ıtem.pick > 0 || ıtem.axe > 0 || ıtem.hammer > 0) && base.position.X / 16f - (float)tileRangeX - (float)ıtem.tileBoost <= (float)tileTargetX && (base.position.X + (float)width) / 16f + (float)tileRangeX + (float)ıtem.tileBoost - 1f >= (float)tileTargetX && base.position.Y / 16f - (float)tileRangeY - (float)ıtem.tileBoost <= (float)tileTargetY && (base.position.Y + (float)height) / 16f + (float)tileRangeY + (float)ıtem.tileBoost - 2f >= (float)tileTargetY)
				{
					int num215 = -1;
					int num216 = 0;
					bool flag12 = true;
					showItemIcon = true;
					if (toolTime == 0 && itemAnimation > 0 && controlUseItem && (!Main.tile[tileTargetX, tileTargetY].active() || (!Main.tileHammer[Main.tile[tileTargetX, tileTargetY].type] && !Main.tileSolid[Main.tile[tileTargetX, tileTargetY].type] && Main.tile[tileTargetX, tileTargetY].type != 314 && Main.tile[tileTargetX, tileTargetY].type != 351)))
					{
						poundRelease = false;
					}
					if (Main.tile[tileTargetX, tileTargetY].active())
					{
						if ((ıtem.pick > 0 && !Main.tileAxe[Main.tile[tileTargetX, tileTargetY].type] && !Main.tileHammer[Main.tile[tileTargetX, tileTargetY].type]) || (ıtem.axe > 0 && Main.tileAxe[Main.tile[tileTargetX, tileTargetY].type]) || (ıtem.hammer > 0 && Main.tileHammer[Main.tile[tileTargetX, tileTargetY].type]))
						{
							flag12 = false;
						}
						if (toolTime == 0 && itemAnimation > 0 && controlUseItem)
						{
							num215 = hitTile.HitObject(tileTargetX, tileTargetY, 1);
							if (Main.tileNoFail[Main.tile[tileTargetX, tileTargetY].type])
							{
								num216 = 100;
							}
							if (Main.tileHammer[Main.tile[tileTargetX, tileTargetY].type])
							{
								flag12 = false;
								if (ıtem.hammer > 0)
								{
									num216 += ıtem.hammer;
									if (!WorldGen.CanKillTile(tileTargetX, tileTargetY))
									{
										num216 = 0;
									}
									if (Main.tile[tileTargetX, tileTargetY].type == 26 && (ıtem.hammer < 80 || !Main.hardMode))
									{
										num216 = 0;
										Hurt(statLife / 2, -direction, false, false, Lang.deathMsg(name, -1, -1, -1, 4));
									}
									AchievementsHelper.CurrentlyMining = true;
									if (hitTile.AddDamage(num215, num216) >= 100)
									{
										hitTile.Clear(num215);
										WorldGen.KillTile(tileTargetX, tileTargetY);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY);
										}
									}
									else
									{
										WorldGen.KillTile(tileTargetX, tileTargetY, true);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY, 1f);
										}
									}
									if (num216 != 0)
									{
										hitTile.Prune();
									}
									itemTime = ıtem.useTime;
									AchievementsHelper.CurrentlyMining = false;
								}
							}
							else if (Main.tileAxe[Main.tile[tileTargetX, tileTargetY].type])
							{
								num216 = ((Main.tile[tileTargetX, tileTargetY].type != 80) ? (num216 + ıtem.axe) : (num216 + ıtem.axe * 3));
								if (ıtem.axe > 0)
								{
									AchievementsHelper.CurrentlyMining = true;
									if (!WorldGen.CanKillTile(tileTargetX, tileTargetY))
									{
										num216 = 0;
									}
									if (hitTile.AddDamage(num215, num216) >= 100)
									{
										hitTile.Clear(num215);
										WorldGen.KillTile(tileTargetX, tileTargetY);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY);
										}
									}
									else
									{
										WorldGen.KillTile(tileTargetX, tileTargetY, true);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, tileTargetY, 1f);
										}
									}
									if (num216 != 0)
									{
										hitTile.Prune();
									}
									itemTime = ıtem.useTime;
									AchievementsHelper.CurrentlyMining = false;
								}
							}
							else if (ıtem.pick > 0)
							{
								PickTile(tileTargetX, tileTargetY, ıtem.pick);
								itemTime = (int)((float)ıtem.useTime * pickSpeed);
							}
							if (ıtem.pick > 0)
							{
								itemTime = (int)((float)ıtem.useTime * pickSpeed);
							}
							if (ıtem.hammer > 0 && Main.tile[tileTargetX, tileTargetY].active() && ((Main.tileSolid[Main.tile[tileTargetX, tileTargetY].type] && Main.tile[tileTargetX, tileTargetY].type != 10) || Main.tile[tileTargetX, tileTargetY].type == 314 || Main.tile[tileTargetX, tileTargetY].type == 351) && poundRelease)
							{
								flag12 = false;
								itemTime = ıtem.useTime;
								num216 += (int)((double)ıtem.hammer * 1.25);
								num216 = 100;
								if (Main.tile[tileTargetX, tileTargetY - 1].active() && Main.tile[tileTargetX, tileTargetY - 1].type == 10)
								{
									num216 = 0;
								}
								if (Main.tile[tileTargetX, tileTargetY + 1].active() && Main.tile[tileTargetX, tileTargetY + 1].type == 10)
								{
									num216 = 0;
								}
								if (hitTile.AddDamage(num215, num216) >= 100)
								{
									hitTile.Clear(num215);
									if (poundRelease)
									{
										int num217 = tileTargetX;
										int num218 = tileTargetY;
										if (Main.tile[num217, num218].type == 19)
										{
											if (Main.tile[num217, num218].halfBrick())
											{
												WorldGen.PoundTile(num217, num218);
												if (Main.netMode == 1)
												{
													NetMessage.SendData(17, -1, -1, "", 7, tileTargetX, tileTargetY, 1f);
												}
											}
											else
											{
												int num219 = 1;
												int slope = 2;
												if (Main.tile[num217 + 1, num218 - 1].type == 19 || Main.tile[num217 - 1, num218 + 1].type == 19 || (WorldGen.SolidTile(num217 + 1, num218) && !WorldGen.SolidTile(num217 - 1, num218)))
												{
													num219 = 2;
													slope = 1;
												}
												if (Main.tile[num217, num218].slope() == 0)
												{
													WorldGen.SlopeTile(num217, num218, num219);
													int num220 = Main.tile[num217, num218].slope();
													if (Main.netMode == 1)
													{
														NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY, num220);
													}
												}
												else if (Main.tile[num217, num218].slope() == num219)
												{
													WorldGen.SlopeTile(num217, num218, slope);
													int num221 = Main.tile[num217, num218].slope();
													if (Main.netMode == 1)
													{
														NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY, num221);
													}
												}
												else
												{
													WorldGen.SlopeTile(num217, num218);
													int num222 = Main.tile[num217, num218].slope();
													if (Main.netMode == 1)
													{
														NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY, num222);
													}
													WorldGen.PoundTile(num217, num218);
													if (Main.netMode == 1)
													{
														NetMessage.SendData(17, -1, -1, "", 7, tileTargetX, tileTargetY, 1f);
													}
												}
											}
										}
										else if (Main.tile[num217, num218].type == 314)
										{
											if (Minecart.FrameTrack(num217, num218, true) && Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 15, tileTargetX, tileTargetY, 1f);
											}
										}
										else if (Main.tile[num217, num218].type == 137)
										{
											if (Main.tile[num217, num218].frameX == 18)
											{
												Main.tile[num217, num218].frameX = 0;
											}
											else
											{
												Main.tile[num217, num218].frameX = 18;
											}
											if (Main.netMode == 1)
											{
												NetMessage.SendTileSquare(-1, tileTargetX, tileTargetY, 1);
											}
										}
										else if ((Main.tile[num217, num218].halfBrick() || Main.tile[num217, num218].slope() != 0) && !Main.tileSolidTop[Main.tile[tileTargetX, tileTargetY].type])
										{
											int num223 = 1;
											int num224 = 1;
											int num225 = 2;
											if ((WorldGen.SolidTile(num217 + 1, num218) || Main.tile[num217 + 1, num218].slope() == 1 || Main.tile[num217 + 1, num218].slope() == 3) && !WorldGen.SolidTile(num217 - 1, num218))
											{
												num224 = 2;
												num225 = 1;
											}
											if (WorldGen.SolidTile(num217, num218 - 1) && !WorldGen.SolidTile(num217, num218 + 1))
											{
												num223 = -1;
											}
											if (num223 == 1)
											{
												if (Main.tile[num217, num218].slope() == 0)
												{
													WorldGen.SlopeTile(num217, num218, num224);
												}
												else if (Main.tile[num217, num218].slope() == num224)
												{
													WorldGen.SlopeTile(num217, num218, num225);
												}
												else if (Main.tile[num217, num218].slope() == num225)
												{
													WorldGen.SlopeTile(num217, num218, num224 + 2);
												}
												else if (Main.tile[num217, num218].slope() == num224 + 2)
												{
													WorldGen.SlopeTile(num217, num218, num225 + 2);
												}
												else
												{
													WorldGen.SlopeTile(num217, num218);
												}
											}
											else if (Main.tile[num217, num218].slope() == 0)
											{
												WorldGen.SlopeTile(num217, num218, num224 + 2);
											}
											else if (Main.tile[num217, num218].slope() == num224 + 2)
											{
												WorldGen.SlopeTile(num217, num218, num225 + 2);
											}
											else if (Main.tile[num217, num218].slope() == num225 + 2)
											{
												WorldGen.SlopeTile(num217, num218, num224);
											}
											else if (Main.tile[num217, num218].slope() == num224)
											{
												WorldGen.SlopeTile(num217, num218, num225);
											}
											else
											{
												WorldGen.SlopeTile(num217, num218);
											}
											int num226 = Main.tile[num217, num218].slope();
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 14, tileTargetX, tileTargetY, num226);
											}
										}
										else
										{
											WorldGen.PoundTile(num217, num218);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 7, tileTargetX, tileTargetY, 1f);
											}
										}
										poundRelease = false;
									}
								}
								else
								{
									WorldGen.KillTile(tileTargetX, tileTargetY, true, true);
									Main.PlaySound(0, tileTargetX * 16, tileTargetY * 16);
								}
							}
							else
							{
								poundRelease = false;
							}
						}
					}
					if (releaseUseItem)
					{
						poundRelease = true;
					}
					int num227 = tileTargetX;
					int num228 = tileTargetY;
					bool flag13 = true;
					if (Main.tile[num227, num228].wall > 0)
					{
						if (!Main.wallHouse[Main.tile[num227, num228].wall])
						{
							for (int num229 = num227 - 1; num229 < num227 + 2; num229++)
							{
								for (int num230 = num228 - 1; num230 < num228 + 2; num230++)
								{
									if (Main.tile[num229, num230].wall != Main.tile[num227, num228].wall)
									{
										flag13 = false;
										break;
									}
								}
							}
						}
						else
						{
							flag13 = false;
						}
					}
					if (flag13 && !Main.tile[num227, num228].active())
					{
						int num231 = -1;
						if ((double)(((float)Main.mouseX + Main.screenPosition.X) / 16f) < Math.Round(((float)Main.mouseX + Main.screenPosition.X) / 16f))
						{
							num231 = 0;
						}
						int num232 = -1;
						if ((double)(((float)Main.mouseY + Main.screenPosition.Y) / 16f) < Math.Round(((float)Main.mouseY + Main.screenPosition.Y) / 16f))
						{
							num232 = 0;
						}
						for (int num233 = tileTargetX + num231; num233 <= tileTargetX + num231 + 1; num233++)
						{
							for (int num234 = tileTargetY + num232; num234 <= tileTargetY + num232 + 1; num234++)
							{
								if (!flag13)
								{
									continue;
								}
								num227 = num233;
								num228 = num234;
								if (Main.tile[num227, num228].wall <= 0)
								{
									continue;
								}
								if (!Main.wallHouse[Main.tile[num227, num228].wall])
								{
									for (int num235 = num227 - 1; num235 < num227 + 2; num235++)
									{
										for (int num236 = num228 - 1; num236 < num228 + 2; num236++)
										{
											if (Main.tile[num235, num236].wall != Main.tile[num227, num228].wall)
											{
												flag13 = false;
												break;
											}
										}
									}
								}
								else
								{
									flag13 = false;
								}
							}
						}
					}
					if (flag12 && Main.tile[num227, num228].wall > 0 && (!Main.tile[num227, num228].active() || num227 != tileTargetX || num228 != tileTargetY || (!Main.tileHammer[Main.tile[num227, num228].type] && !poundRelease)) && toolTime == 0 && itemAnimation > 0 && controlUseItem && ıtem.hammer > 0)
					{
						bool flag14 = true;
						if (!Main.wallHouse[Main.tile[num227, num228].wall])
						{
							flag14 = false;
							for (int num237 = num227 - 1; num237 < num227 + 2; num237++)
							{
								for (int num238 = num228 - 1; num238 < num228 + 2; num238++)
								{
									if (Main.tile[num237, num238].wall == 0 || Main.wallHouse[Main.tile[num237, num238].wall])
									{
										flag14 = true;
										break;
									}
								}
							}
						}
						if (flag14)
						{
							num215 = hitTile.HitObject(num227, num228, 2);
							num216 += (int)((float)ıtem.hammer * 1.5f);
							if (hitTile.AddDamage(num215, num216) >= 100)
							{
								hitTile.Clear(num215);
								WorldGen.KillWall(num227, num228);
								if (Main.netMode == 1)
								{
									NetMessage.SendData(17, -1, -1, "", 2, num227, num228);
								}
							}
							else
							{
								WorldGen.KillWall(num227, num228, true);
								if (Main.netMode == 1)
								{
									NetMessage.SendData(17, -1, -1, "", 2, num227, num228, 1f);
								}
							}
							if (num216 != 0)
							{
								hitTile.Prune();
							}
							itemTime = ıtem.useTime / 2;
						}
					}
				}
				if (Main.myPlayer == whoAmI && ıtem.type == 1326 && itemAnimation > 0 && itemTime == 0)
				{
					itemTime = ıtem.useTime;
					Vector2 vector19 = default(Vector2);
					vector19.X = (float)Main.mouseX + Main.screenPosition.X;
					if (gravDir == 1f)
					{
						vector19.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)height;
					}
					else
					{
						vector19.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
					}
					vector19.X -= width / 2;
					if (vector19.X > 50f && vector19.X < (float)(Main.maxTilesX * 16 - 50) && vector19.Y > 50f && vector19.Y < (float)(Main.maxTilesY * 16 - 50))
					{
						int num239 = (int)(vector19.X / 16f);
						int num240 = (int)(vector19.Y / 16f);
						if ((Main.tile[num239, num240].wall != 87 || !((double)num240 > Main.worldSurface) || NPC.downedPlantBoss) && !Collision.SolidCollision(vector19, width, height))
						{
							Teleport(vector19, 1);
							NetMessage.SendData(65, -1, -1, "", 0, whoAmI, vector19.X, vector19.Y, 1);
							if (chaosState)
							{
								statLife -= statLifeMax2 / 7;
								if (Lang.lang <= 1)
								{
									string deathText = Language.GetTextValue("DeathText.Teleport_1", Main.player[Main.myPlayer].name);
									if (Main.rand.Next(2) == 0)
									{
										deathText = ((!Male) ? Language.GetTextValue("DeathText.Teleport_2_Male", Main.player[Main.myPlayer].name) : Language.GetTextValue("DeathText.Teleport_2_Male", Main.player[Main.myPlayer].name));
									}
									if (statLife <= 0)
									{
										KillMe(1.0, 0, false, deathText);
									}
								}
								else if (statLife <= 0)
								{
									KillMe(1.0, 0, false, "");
								}
								lifeRegenCount = 0;
								lifeRegenTime = 0;
							}
							AddBuff(88, 360);
						}
					}
				}
				if (ıtem.type == 29 && itemAnimation > 0 && statLifeMax < 400 && itemTime == 0)
				{
					itemTime = ıtem.useTime;
					statLifeMax += 20;
					statLifeMax2 += 20;
					statLife += 20;
					if (Main.myPlayer == whoAmI)
					{
						HealEffect(20);
					}
					AchievementsHelper.HandleSpecialEvent(this, 0);
				}
				if (ıtem.type == 1291 && itemAnimation > 0 && statLifeMax >= 400 && statLifeMax < 500 && itemTime == 0)
				{
					itemTime = ıtem.useTime;
					statLifeMax += 5;
					statLifeMax2 += 5;
					statLife += 5;
					if (Main.myPlayer == whoAmI)
					{
						HealEffect(5);
					}
					AchievementsHelper.HandleSpecialEvent(this, 2);
				}
				if (ıtem.type == 109 && itemAnimation > 0 && statManaMax < 200 && itemTime == 0)
				{
					itemTime = ıtem.useTime;
					statManaMax += 20;
					statManaMax2 += 20;
					statMana += 20;
					if (Main.myPlayer == whoAmI)
					{
						ManaEffect(20);
					}
					AchievementsHelper.HandleSpecialEvent(this, 1);
				}
				if (ıtem.type == 3335 && itemAnimation > 0 && !extraAccessory && Main.expertMode && itemTime == 0)
				{
					itemTime = ıtem.useTime;
					extraAccessory = true;
					NetMessage.SendData(4, -1, -1, Main.player[whoAmI].name, whoAmI);
				}
				PlaceThing();
			}
			if (ıtem.type == 3542)
			{
				Vector2 value5 = Main.OffsetsPlayerOnhand[bodyFrame.Y / 56] * 2f;
				if (direction != 1)
				{
					value5.X = (float)bodyFrame.Width - value5.X;
				}
				if (gravDir != 1f)
				{
					value5.Y = (float)bodyFrame.Height - value5.Y;
				}
				value5 -= new Vector2(bodyFrame.Width - width, bodyFrame.Height - 42) / 2f;
				Vector2 position17 = RotatedRelativePoint(base.position + value5) - velocity;
				for (int num241 = 0; num241 < 4; num241++)
				{
					Dust dust = Main.dust[Dust.NewDust(base.Center, 0, 0, 242, direction * 2, 0f, 150, default(Color), 1.3f)];
					dust.position = position17;
					dust.velocity *= 0f;
					dust.noGravity = true;
					dust.fadeIn = 1f;
					dust.velocity += velocity;
					if (Main.rand.Next(2) == 0)
					{
						dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
						dust.scale += Main.rand.NextFloat();
						if (Main.rand.Next(2) == 0)
						{
							dust.customData = this;
						}
					}
				}
			}
			if (((ıtem.damage >= 0 && ıtem.type > 0 && !ıtem.noMelee) || ıtem.type == 1450 || ıtem.type == 1991 || ıtem.type == 3183 || ıtem.type == 3542) && itemAnimation > 0)
			{
				bool flag15 = false;
				Rectangle r2 = new Rectangle((int)itemLocation.X, (int)itemLocation.Y, 32, 32);
				if (!Main.dedServ)
				{
					r2 = new Rectangle((int)itemLocation.X, (int)itemLocation.Y, Main.itemTexture[ıtem.type].Width, Main.itemTexture[ıtem.type].Height);
				}
				r2.Width = (int)((float)r2.Width * ıtem.scale);
				r2.Height = (int)((float)r2.Height * ıtem.scale);
				if (direction == -1)
				{
					r2.X -= r2.Width;
				}
				if (gravDir == 1f)
				{
					r2.Y -= r2.Height;
				}
				if (ıtem.useStyle == 1)
				{
					if ((double)itemAnimation < (double)itemAnimationMax * 0.333)
					{
						if (direction == -1)
						{
							r2.X -= (int)((double)r2.Width * 1.4 - (double)r2.Width);
						}
						r2.Width = (int)((double)r2.Width * 1.4);
						r2.Y += (int)((double)r2.Height * 0.5 * (double)gravDir);
						r2.Height = (int)((double)r2.Height * 1.1);
					}
					else if (!((double)itemAnimation < (double)itemAnimationMax * 0.666))
					{
						if (direction == 1)
						{
							r2.X -= (int)((double)r2.Width * 1.2);
						}
						r2.Width *= 2;
						r2.Y -= (int)(((double)r2.Height * 1.4 - (double)r2.Height) * (double)gravDir);
						r2.Height = (int)((double)r2.Height * 1.4);
					}
				}
				else if (ıtem.useStyle == 3)
				{
					if ((double)itemAnimation > (double)itemAnimationMax * 0.666)
					{
						flag15 = true;
					}
					else
					{
						if (direction == -1)
						{
							r2.X -= (int)((double)r2.Width * 1.4 - (double)r2.Width);
						}
						r2.Width = (int)((double)r2.Width * 1.4);
						r2.Y += (int)((double)r2.Height * 0.6);
						r2.Height = (int)((double)r2.Height * 0.6);
					}
				}
				float gravDir2 = gravDir;
				float num321 = -1f;
				if (ıtem.type == 1450 && Main.rand.Next(3) == 0)
				{
					int num242 = -1;
					float x6 = r2.X + Main.rand.Next(r2.Width);
					float y6 = r2.Y + Main.rand.Next(r2.Height);
					if (Main.rand.Next(500) == 0)
					{
						num242 = Gore.NewGore(new Vector2(x6, y6), default(Vector2), 415, (float)Main.rand.Next(51, 101) * 0.01f);
					}
					else if (Main.rand.Next(250) == 0)
					{
						num242 = Gore.NewGore(new Vector2(x6, y6), default(Vector2), 414, (float)Main.rand.Next(51, 101) * 0.01f);
					}
					else if (Main.rand.Next(80) == 0)
					{
						num242 = Gore.NewGore(new Vector2(x6, y6), default(Vector2), 413, (float)Main.rand.Next(51, 101) * 0.01f);
					}
					else if (Main.rand.Next(10) == 0)
					{
						num242 = Gore.NewGore(new Vector2(x6, y6), default(Vector2), 412, (float)Main.rand.Next(51, 101) * 0.01f);
					}
					else if (Main.rand.Next(3) == 0)
					{
						num242 = Gore.NewGore(new Vector2(x6, y6), default(Vector2), 411, (float)Main.rand.Next(51, 101) * 0.01f);
					}
					if (num242 >= 0)
					{
						Main.gore[num242].velocity.X += direction * 2;
						Main.gore[num242].velocity.Y *= 0.3f;
					}
				}
				if (ıtem.type == 3542)
				{
					flag15 = true;
				}
				if (!flag15)
				{
					if (ıtem.type == 989 && Main.rand.Next(5) == 0)
					{
						int type5;
						switch (Main.rand.Next(3))
						{
						case 0:
							type5 = 15;
							break;
						case 1:
							type5 = 57;
							break;
						default:
							type5 = 58;
							break;
						}
						int num243 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, type5, direction * 2, 0f, 150, default(Color), 1.3f);
						Main.dust[num243].velocity *= 0.2f;
					}
					if (ıtem.type == 2880 && Main.rand.Next(2) == 0)
					{
						int type6 = Utils.SelectRandom<int>(Main.rand, 226, 229);
						int num244 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, type6, direction * 2, 0f, 150);
						Main.dust[num244].velocity *= 0.2f;
						Main.dust[num244].noGravity = true;
					}
					if ((ıtem.type == 44 || ıtem.type == 45 || ıtem.type == 46 || ıtem.type == 103 || ıtem.type == 104) && Main.rand.Next(15) == 0)
					{
						Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 14, direction * 2, 0f, 150, default(Color), 1.3f);
					}
					if (ıtem.type == 273 || ıtem.type == 675)
					{
						if (Main.rand.Next(5) == 0)
						{
							Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 14, direction * 2, 0f, 150, default(Color), 1.4f);
						}
						int num245 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 27, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 1.2f);
						Main.dust[num245].noGravity = true;
						Main.dust[num245].velocity.X /= 2f;
						Main.dust[num245].velocity.Y /= 2f;
					}
					if (ıtem.type == 723 && Main.rand.Next(2) == 0)
					{
						int num246 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 64, 0f, 0f, 150, default(Color), 1.2f);
						Main.dust[num246].noGravity = true;
					}
					if (ıtem.type == 65)
					{
						if (Main.rand.Next(5) == 0)
						{
							Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 58, 0f, 0f, 150, default(Color), 1.2f);
						}
						if (Main.rand.Next(10) == 0)
						{
							Gore.NewGore(new Vector2(r2.X, r2.Y), default(Vector2), Main.rand.Next(16, 18));
						}
					}
					if (ıtem.type == 3065)
					{
						int num247 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 58, 0f, 0f, 150, default(Color), 1.2f);
						Main.dust[num247].velocity *= 0.5f;
						if (Main.rand.Next(8) == 0)
						{
							int num248 = Gore.NewGore(new Vector2(r2.Center.X, r2.Center.Y), default(Vector2), 16);
							Main.gore[num248].velocity *= 0.5f;
							Main.gore[num248].velocity += new Vector2(direction, 0f);
						}
					}
					if (ıtem.type == 190)
					{
						int num249 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 40, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 0, default(Color), 1.2f);
						Main.dust[num249].noGravity = true;
					}
					else if (ıtem.type == 213)
					{
						int num250 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 3, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 0, default(Color), 1.2f);
						Main.dust[num250].noGravity = true;
					}
					if (ıtem.type == 121)
					{
						for (int num251 = 0; num251 < 2; num251++)
						{
							int num252 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 6, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 2.5f);
							Main.dust[num252].noGravity = true;
							Main.dust[num252].velocity.X *= 2f;
							Main.dust[num252].velocity.Y *= 2f;
						}
					}
					if (ıtem.type == 122 || ıtem.type == 217)
					{
						int num253 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 6, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 1.9f);
						Main.dust[num253].noGravity = true;
					}
					if (ıtem.type == 155)
					{
						int num254 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 172, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 0.9f);
						Main.dust[num254].noGravity = true;
						Main.dust[num254].velocity *= 0.1f;
					}
					if (ıtem.type == 676 && Main.rand.Next(3) == 0)
					{
						int num255 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 67, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 90, default(Color), 1.5f);
						Main.dust[num255].noGravity = true;
						Main.dust[num255].velocity *= 0.2f;
					}
					if (ıtem.type == 3063)
					{
						int num256 = Dust.NewDust(r2.TopLeft(), r2.Width, r2.Height, 66, 0f, 0f, 150, Color.Transparent, 0.85f);
						Main.dust[num256].color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
						Main.dust[num256].noGravity = true;
						Main.dust[num256].velocity /= 2f;
					}
					if (ıtem.type == 724 && Main.rand.Next(5) == 0)
					{
						int num257 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 67, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 90, default(Color), 1.5f);
						Main.dust[num257].noGravity = true;
						Main.dust[num257].velocity *= 0.2f;
					}
					if (ıtem.type >= 795 && ıtem.type <= 802 && Main.rand.Next(3) == 0)
					{
						int num258 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 115, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 140, default(Color), 1.5f);
						Main.dust[num258].noGravity = true;
						Main.dust[num258].velocity *= 0.25f;
					}
					if (ıtem.type == 367 || ıtem.type == 368 || ıtem.type == 674)
					{
						int num259 = 0;
						if (Main.rand.Next(3) == 0)
						{
							num259 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 57, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 1.1f);
							Main.dust[num259].noGravity = true;
							Main.dust[num259].velocity.X /= 2f;
							Main.dust[num259].velocity.Y /= 2f;
							Main.dust[num259].velocity.X += direction * 2;
						}
						if (Main.rand.Next(4) == 0)
						{
							num259 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 43, 0f, 0f, 254, default(Color), 0.3f);
							Main.dust[num259].velocity *= 0f;
						}
					}
					if (ıtem.type >= 198 && ıtem.type <= 203)
					{
						float num260 = 0.5f;
						float num261 = 0.5f;
						float num262 = 0.5f;
						if (ıtem.type == 198)
						{
							num260 *= 0.1f;
							num261 *= 0.5f;
							num262 *= 1.2f;
						}
						else if (ıtem.type == 199)
						{
							num260 *= 1f;
							num261 *= 0.2f;
							num262 *= 0.1f;
						}
						else if (ıtem.type == 200)
						{
							num260 *= 0.1f;
							num261 *= 1f;
							num262 *= 0.2f;
						}
						else if (ıtem.type == 201)
						{
							num260 *= 0.8f;
							num261 *= 0.1f;
							num262 *= 1f;
						}
						else if (ıtem.type == 202)
						{
							num260 *= 0.8f;
							num261 *= 0.9f;
							num262 *= 1f;
						}
						else if (ıtem.type == 203)
						{
							num260 *= 0.9f;
							num261 *= 0.9f;
							num262 *= 0.1f;
						}
						Lighting.AddLight((int)((itemLocation.X + 6f + velocity.X) / 16f), (int)((itemLocation.Y - 14f) / 16f), num260, num261, num262);
					}
					if (frostBurn && ıtem.melee && !ıtem.noMelee && !ıtem.noUseGraphic && Main.rand.Next(2) == 0)
					{
						int num263 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 135, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 2.5f);
						Main.dust[num263].noGravity = true;
						Main.dust[num263].velocity *= 0.7f;
						Main.dust[num263].velocity.Y -= 0.5f;
					}
					if (ıtem.melee && !ıtem.noMelee && !ıtem.noUseGraphic && meleeEnchant > 0)
					{
						if (meleeEnchant == 1)
						{
							if (Main.rand.Next(3) == 0)
							{
								int num264 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 171, 0f, 0f, 100);
								Main.dust[num264].noGravity = true;
								Main.dust[num264].fadeIn = 1.5f;
								Main.dust[num264].velocity *= 0.25f;
							}
						}
						else if (meleeEnchant == 2)
						{
							if (Main.rand.Next(2) == 0)
							{
								int num265 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 75, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 2.5f);
								Main.dust[num265].noGravity = true;
								Main.dust[num265].velocity *= 0.7f;
								Main.dust[num265].velocity.Y -= 0.5f;
							}
						}
						else if (meleeEnchant == 3)
						{
							if (Main.rand.Next(2) == 0)
							{
								int num266 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 6, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 2.5f);
								Main.dust[num266].noGravity = true;
								Main.dust[num266].velocity *= 0.7f;
								Main.dust[num266].velocity.Y -= 0.5f;
							}
						}
						else if (meleeEnchant == 4)
						{
							int num267 = 0;
							if (Main.rand.Next(2) == 0)
							{
								num267 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 57, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 1.1f);
								Main.dust[num267].noGravity = true;
								Main.dust[num267].velocity.X /= 2f;
								Main.dust[num267].velocity.Y /= 2f;
							}
						}
						else if (meleeEnchant == 5)
						{
							if (Main.rand.Next(2) == 0)
							{
								int num268 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 169, 0f, 0f, 100);
								Main.dust[num268].velocity.X += direction;
								Main.dust[num268].velocity.Y += 0.2f;
								Main.dust[num268].noGravity = true;
							}
						}
						else if (meleeEnchant == 6)
						{
							if (Main.rand.Next(2) == 0)
							{
								int num269 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 135, 0f, 0f, 100);
								Main.dust[num269].velocity.X += direction;
								Main.dust[num269].velocity.Y += 0.2f;
								Main.dust[num269].noGravity = true;
							}
						}
						else if (meleeEnchant == 7)
						{
							if (Main.rand.Next(20) == 0)
							{
								int type7 = Main.rand.Next(139, 143);
								int num270 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, type7, velocity.X, velocity.Y, 0, default(Color), 1.2f);
								Main.dust[num270].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
								Main.dust[num270].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
								Main.dust[num270].velocity.X += (float)Main.rand.Next(-50, 51) * 0.05f;
								Main.dust[num270].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.05f;
								Main.dust[num270].scale *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
							}
							if (Main.rand.Next(40) == 0)
							{
								int type8 = Main.rand.Next(276, 283);
								int num271 = Gore.NewGore(new Vector2(r2.X, r2.Y), velocity, type8);
								Main.gore[num271].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
								Main.gore[num271].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
								Main.gore[num271].scale *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
								Main.gore[num271].velocity.X += (float)Main.rand.Next(-50, 51) * 0.05f;
								Main.gore[num271].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.05f;
							}
						}
						else if (meleeEnchant == 8 && Main.rand.Next(4) == 0)
						{
							int num272 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 46, 0f, 0f, 100);
							Main.dust[num272].noGravity = true;
							Main.dust[num272].fadeIn = 1.5f;
							Main.dust[num272].velocity *= 0.25f;
						}
					}
					if (magmaStone && ıtem.melee && !ıtem.noMelee && !ıtem.noUseGraphic && Main.rand.Next(3) != 0)
					{
						int num273 = Dust.NewDust(new Vector2(r2.X, r2.Y), r2.Width, r2.Height, 6, velocity.X * 0.2f + (float)(direction * 3), velocity.Y * 0.2f, 100, default(Color), 2.5f);
						Main.dust[num273].noGravity = true;
						Main.dust[num273].velocity.X *= 2f;
						Main.dust[num273].velocity.Y *= 2f;
					}
					if (Main.myPlayer == i && (ıtem.type == 1991 || ıtem.type == 3183))
					{
						for (int num274 = 0; num274 < 200; num274++)
						{
							if (Main.npc[num274].active && Main.npc[num274].catchItem > 0)
							{
								Rectangle value6 = new Rectangle((int)Main.npc[num274].position.X, (int)Main.npc[num274].position.Y, Main.npc[num274].width, Main.npc[num274].height);
								if (r2.Intersects(value6) && (ıtem.type == 3183 || Main.npc[num274].noTileCollide || Collision.CanHit(base.position, width, height, Main.npc[num274].position, Main.npc[num274].width, Main.npc[num274].height)))
								{
									NPC.CatchNPC(num274, i);
								}
							}
						}
					}
					if (Main.myPlayer == i && (ıtem.damage > 0 || ıtem.type == 3183))
					{
						int num275 = (int)((float)ıtem.damage * meleeDamage);
						float knockBack = ıtem.knockBack;
						float num276 = 1f;
						if (kbGlove)
						{
							num276 += 1f;
						}
						if (kbBuff)
						{
							num276 += 0.5f;
						}
						knockBack *= num276;
						if (inventory[selectedItem].type == 3106)
						{
							knockBack += knockBack * (1f - stealth);
						}
						List<ushort> list2 = null;
						int type9 = ıtem.type;
						if (type9 == 213)
						{
							list2 = new List<ushort>(new ushort[17]
							{
								3,
								24,
								52,
								61,
								62,
								71,
								73,
								74,
								82,
								83,
								84,
								110,
								113,
								115,
								184,
								205,
								201
							});
						}
						int num277 = r2.X / 16;
						int num278 = (r2.X + r2.Width) / 16 + 1;
						int num279 = r2.Y / 16;
						int num280 = (r2.Y + r2.Height) / 16 + 1;
						for (int num281 = num277; num281 < num278; num281++)
						{
							for (int num282 = num279; num282 < num280; num282++)
							{
								if (Main.tile[num281, num282] == null || !Main.tileCut[Main.tile[num281, num282].type] || (list2 != null && list2.Contains(Main.tile[num281, num282].type)) || Main.tile[num281, num282 + 1] == null || Main.tile[num281, num282 + 1].type == 78 || Main.tile[num281, num282 + 1].type == 380)
								{
									continue;
								}
								if (ıtem.type == 1786)
								{
									int type10 = Main.tile[num281, num282].type;
									WorldGen.KillTile(num281, num282);
									if (!Main.tile[num281, num282].active())
									{
										int num283 = 0;
										if (type10 == 3 || type10 == 24 || type10 == 61 || type10 == 110 || type10 == 201)
										{
											num283 = Main.rand.Next(1, 3);
										}
										if (type10 == 73 || type10 == 74 || type10 == 113)
										{
											num283 = Main.rand.Next(2, 5);
										}
										if (num283 > 0)
										{
											int number = Item.NewItem(num281 * 16, num282 * 16, 16, 16, 1727, num283);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(21, -1, -1, "", number, 1f);
											}
										}
									}
									if (Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 0, num281, num282);
									}
								}
								else
								{
									WorldGen.KillTile(num281, num282);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 0, num281, num282);
									}
								}
							}
						}
						if (ıtem.type != 3183)
						{
							for (int num284 = 0; num284 < 200; num284++)
							{
								if (!Main.npc[num284].active || Main.npc[num284].immune[i] != 0 || attackCD != 0)
								{
									continue;
								}
								if (!Main.npc[num284].dontTakeDamage)
								{
									if (Main.npc[num284].friendly && (Main.npc[num284].type != 22 || !killGuide) && (Main.npc[num284].type != 54 || !killClothier))
									{
										continue;
									}
									Rectangle value7 = new Rectangle((int)Main.npc[num284].position.X, (int)Main.npc[num284].position.Y, Main.npc[num284].width, Main.npc[num284].height);
									if (!r2.Intersects(value7) || (!Main.npc[num284].noTileCollide && !Collision.CanHit(base.position, width, height, Main.npc[num284].position, Main.npc[num284].width, Main.npc[num284].height)))
									{
										continue;
									}
									bool flag16 = false;
									if (Main.rand.Next(1, 101) <= meleeCrit)
									{
										flag16 = true;
									}
									int num285 = Item.NPCtoBanner(Main.npc[num284].BannerID());
									if (num285 > 0 && NPCBannerBuff[num285])
									{
										num275 = ((!Main.expertMode) ? ((int)((double)num275 * 1.5)) : (num275 * 2));
									}
									int num286 = Main.DamageVar(num275);
									StatusNPC(ıtem.type, num284);
									OnHit(Main.npc[num284].Center.X, Main.npc[num284].Center.Y, Main.npc[num284]);
									if (armorPenetration > 0)
									{
										num286 += Main.npc[num284].checkArmorPenetration(armorPenetration);
									}
									int num287 = (int)Main.npc[num284].StrikeNPC(num286, knockBack, direction, flag16);
									if (inventory[selectedItem].type == 3211)
									{
										Vector2 vector20 = new Vector2(direction * 100 + Main.rand.Next(-25, 26), Main.rand.Next(-75, 76));
										vector20.Normalize();
										vector20 *= (float)Main.rand.Next(30, 41) * 0.1f;
										Vector2 value8 = new Vector2(r2.X + Main.rand.Next(r2.Width), r2.Y + Main.rand.Next(r2.Height));
										value8 = (value8 + Main.npc[num284].Center * 2f) / 3f;
										Projectile.NewProjectile(value8.X, value8.Y, vector20.X, vector20.Y, 524, (int)((double)num275 * 0.7), knockBack * 0.7f, whoAmI);
									}
									bool flag17 = !Main.npc[num284].immortal;
									if (beetleOffense && flag17)
									{
										beetleCounter += num287;
										beetleCountdown = 0;
									}
									if (ıtem.type == 1826 && (Main.npc[num284].value > 0f || (Main.npc[num284].damage > 0 && !Main.npc[num284].friendly)))
									{
										pumpkinSword(num284, (int)((double)num275 * 1.5), knockBack);
									}
									if (meleeEnchant == 7)
									{
										Projectile.NewProjectile(Main.npc[num284].Center.X, Main.npc[num284].Center.Y, Main.npc[num284].velocity.X, Main.npc[num284].velocity.Y, 289, 0, 0f, whoAmI);
									}
									if (inventory[selectedItem].type == 3106)
									{
										stealth = 1f;
										if (Main.netMode == 1)
										{
											NetMessage.SendData(84, -1, -1, "", whoAmI);
										}
									}
									if (ıtem.type == 1123 && flag17)
									{
										int num288 = Main.rand.Next(1, 4);
										if (strongBees && Main.rand.Next(3) == 0)
										{
											num288++;
										}
										for (int num289 = 0; num289 < num288; num289++)
										{
											float num290 = (float)(direction * 2) + (float)Main.rand.Next(-35, 36) * 0.02f;
											float num291 = (float)Main.rand.Next(-35, 36) * 0.02f;
											num290 *= 0.2f;
											num291 *= 0.2f;
											Projectile.NewProjectile(r2.X + r2.Width / 2, r2.Y + r2.Height / 2, num290, num291, beeType(), beeDamage(num286 / 3), beeKB(0f), i);
										}
									}
									if (Main.npc[num284].value > 0f && coins && Main.rand.Next(5) == 0)
									{
										int type11 = 71;
										if (Main.rand.Next(10) == 0)
										{
											type11 = 72;
										}
										if (Main.rand.Next(100) == 0)
										{
											type11 = 73;
										}
										int num292 = Item.NewItem((int)Main.npc[num284].position.X, (int)Main.npc[num284].position.Y, Main.npc[num284].width, Main.npc[num284].height, type11);
										Main.item[num292].stack = Main.rand.Next(1, 11);
										Main.item[num292].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
										Main.item[num292].velocity.X = (float)Main.rand.Next(10, 31) * 0.2f * (float)direction;
										if (Main.netMode == 1)
										{
											NetMessage.SendData(21, -1, -1, "", num292);
										}
									}
									int num293 = Item.NPCtoBanner(Main.npc[num284].BannerID());
									if (num293 >= 0)
									{
										lastCreatureHit = num293;
									}
									if (Main.netMode != 0)
									{
										if (flag16)
										{
											NetMessage.SendData(28, -1, -1, "", num284, num286, knockBack, direction, 1);
										}
										else
										{
											NetMessage.SendData(28, -1, -1, "", num284, num286, knockBack, direction);
										}
									}
									if (accDreamCatcher)
									{
										addDPS(num286);
									}
									Main.npc[num284].immune[i] = itemAnimation;
									attackCD = (int)((double)itemAnimationMax * 0.33);
								}
								else if (Main.npc[num284].type == 63 || Main.npc[num284].type == 64 || Main.npc[num284].type == 103 || Main.npc[num284].type == 242)
								{
									Rectangle value9 = new Rectangle((int)Main.npc[num284].position.X, (int)Main.npc[num284].position.Y, Main.npc[num284].width, Main.npc[num284].height);
									if (r2.Intersects(value9) && (Main.npc[num284].noTileCollide || Collision.CanHit(base.position, width, height, Main.npc[num284].position, Main.npc[num284].width, Main.npc[num284].height)))
									{
										Hurt((int)((double)Main.npc[num284].damage * 1.3), -direction, false, false, Lang.deathMsg(name, -1, -1, -1, 4));
										Main.npc[num284].immune[i] = itemAnimation;
										attackCD = (int)((double)itemAnimationMax * 0.33);
									}
								}
							}
							if (hostile)
							{
								for (int num294 = 0; num294 < 16; num294++)
								{
									if (num294 == i || !Main.player[num294].active || !Main.player[num294].hostile || Main.player[num294].immune || Main.player[num294].dead || (Main.player[i].team != 0 && Main.player[i].team == Main.player[num294].team))
									{
										continue;
									}
									Rectangle value10 = new Rectangle((int)Main.player[num294].position.X, (int)Main.player[num294].position.Y, Main.player[num294].width, Main.player[num294].height);
									if (!r2.Intersects(value10) || !Collision.CanHit(base.position, width, height, Main.player[num294].position, Main.player[num294].width, Main.player[num294].height))
									{
										continue;
									}
									bool flag18 = false;
									if (Main.rand.Next(1, 101) <= 10)
									{
										flag18 = true;
									}
									int num295 = Main.DamageVar(num275);
									StatusPvP(ıtem.type, num294);
									OnHit(Main.player[num294].Center.X, Main.player[num294].Center.Y, Main.player[num294]);
									int num296 = (int)Main.player[num294].Hurt(num295, direction, true, false, "", flag18);
									if (inventory[selectedItem].type == 3211)
									{
										Vector2 vector21 = new Vector2(direction * 100 + Main.rand.Next(-25, 26), Main.rand.Next(-75, 76));
										vector21.Normalize();
										vector21 *= (float)Main.rand.Next(30, 41) * 0.1f;
										Vector2 value11 = new Vector2(r2.X + Main.rand.Next(r2.Width), r2.Y + Main.rand.Next(r2.Height));
										value11 = (value11 + Main.player[num294].Center * 2f) / 3f;
										Projectile.NewProjectile(value11.X, value11.Y, vector21.X, vector21.Y, 524, (int)((double)num275 * 0.7), knockBack * 0.7f, whoAmI);
									}
									if (beetleOffense)
									{
										beetleCounter += num296;
										beetleCountdown = 0;
									}
									if (meleeEnchant == 7)
									{
										Projectile.NewProjectile(Main.player[num294].Center.X, Main.player[num294].Center.Y, Main.player[num294].velocity.X, Main.player[num294].velocity.Y, 289, 0, 0f, whoAmI);
									}
									if (ıtem.type == 1123)
									{
										int num297 = Main.rand.Next(1, 4);
										if (strongBees && Main.rand.Next(3) == 0)
										{
											num297++;
										}
										for (int num298 = 0; num298 < num297; num298++)
										{
											float num299 = (float)(direction * 2) + (float)Main.rand.Next(-35, 36) * 0.02f;
											float num300 = (float)Main.rand.Next(-35, 36) * 0.02f;
											num299 *= 0.2f;
											num300 *= 0.2f;
											Projectile.NewProjectile(r2.X + r2.Width / 2, r2.Y + r2.Height / 2, num299, num300, beeType(), beeDamage(num295 / 3), beeKB(0f), i);
										}
									}
									if (inventory[selectedItem].type == 3106)
									{
										stealth = 1f;
										if (Main.netMode == 1)
										{
											NetMessage.SendData(84, -1, -1, "", whoAmI);
										}
									}
									if (ıtem.type == 1826 && Main.npc[num294].value > 0f)
									{
										pumpkinSword(num294, (int)((double)num275 * 1.5), knockBack);
									}
									if (Main.netMode != 0)
									{
										if (flag18)
										{
											NetMessage.SendData(26, -1, -1, Lang.deathMsg(name, whoAmI), num294, direction, num295, 1f, 1);
										}
										else
										{
											NetMessage.SendData(26, -1, -1, Lang.deathMsg(name, whoAmI), num294, direction, num295, 1f);
										}
									}
									attackCD = (int)((double)itemAnimationMax * 0.33);
								}
							}
							if (ıtem.type == 787 && (itemAnimation == (int)((double)itemAnimationMax * 0.1) || itemAnimation == (int)((double)itemAnimationMax * 0.3) || itemAnimation == (int)((double)itemAnimationMax * 0.5) || itemAnimation == (int)((double)itemAnimationMax * 0.7) || itemAnimation == (int)((double)itemAnimationMax * 0.9)))
							{
								float num301 = 0f;
								float num302 = 0f;
								float num303 = 0f;
								float num304 = 0f;
								if (itemAnimation == (int)((double)itemAnimationMax * 0.9))
								{
									num301 = -7f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.7))
								{
									num301 = -6f;
									num302 = 2f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.5))
								{
									num301 = -4f;
									num302 = 4f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.3))
								{
									num301 = -2f;
									num302 = 6f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.1))
								{
									num302 = 7f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.7))
								{
									num304 = 26f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.3))
								{
									num304 -= 4f;
									num303 -= 20f;
								}
								if (itemAnimation == (int)((double)itemAnimationMax * 0.1))
								{
									num303 += 6f;
								}
								if (direction == -1)
								{
									if (itemAnimation == (int)((double)itemAnimationMax * 0.9))
									{
										num304 -= 8f;
									}
									if (itemAnimation == (int)((double)itemAnimationMax * 0.7))
									{
										num304 -= 6f;
									}
								}
								num301 *= 1.5f;
								num302 *= 1.5f;
								num304 *= (float)direction;
								num303 *= gravDir;
								Projectile.NewProjectile((float)(r2.X + r2.Width / 2) + num304, (float)(r2.Y + r2.Height / 2) + num303, (float)direction * num302, num301 * gravDir, 131, num275 / 2, 0f, i);
							}
						}
					}
				}
			}
			if (itemTime == 0 && itemAnimation > 0)
			{
				if (ıtem.hairDye >= 0)
				{
					itemTime = ıtem.useTime;
					if (whoAmI == Main.myPlayer)
					{
						hairDye = (byte)ıtem.hairDye;
						NetMessage.SendData(4, -1, -1, Main.player[whoAmI].name, whoAmI);
					}
				}
				if (ıtem.healLife > 0)
				{
					statLife += ıtem.healLife;
					itemTime = ıtem.useTime;
					if (Main.myPlayer == whoAmI)
					{
						HealEffect(ıtem.healLife);
					}
				}
				if (ıtem.healMana > 0)
				{
					statMana += ıtem.healMana;
					itemTime = ıtem.useTime;
					if (Main.myPlayer == whoAmI)
					{
						AddBuff(94, manaSickTime);
						ManaEffect(ıtem.healMana);
					}
				}
				if (ıtem.buffType > 0)
				{
					if (whoAmI == Main.myPlayer && ıtem.buffType != 90 && ıtem.buffType != 27)
					{
						AddBuff(ıtem.buffType, ıtem.buffTime);
					}
					itemTime = ıtem.useTime;
				}
				if (ıtem.type == 678)
				{
					itemTime = ıtem.useTime;
					if (whoAmI == Main.myPlayer)
					{
						AddBuff(20, 216000);
						AddBuff(22, 216000);
						AddBuff(23, 216000);
						AddBuff(24, 216000);
						AddBuff(30, 216000);
						AddBuff(31, 216000);
						AddBuff(32, 216000);
						AddBuff(33, 216000);
						AddBuff(35, 216000);
						AddBuff(36, 216000);
						AddBuff(68, 216000);
					}
				}
			}
			if (whoAmI == Main.myPlayer)
			{
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 361 && Main.CanStartInvasion(1, true))
				{
					itemTime = ıtem.useTime;
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					if (Main.netMode != 1)
					{
						if (Main.invasionType == 0)
						{
							Main.invasionDelay = 0;
							Main.StartInvasion();
						}
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -1f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 602 && Main.CanStartInvasion(2, true))
				{
					itemTime = ıtem.useTime;
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					if (Main.netMode != 1)
					{
						if (Main.invasionType == 0)
						{
							Main.invasionDelay = 0;
							Main.StartInvasion(2);
						}
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -2f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 1315 && Main.CanStartInvasion(3, true))
				{
					itemTime = ıtem.useTime;
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					if (Main.netMode != 1)
					{
						if (Main.invasionType == 0)
						{
							Main.invasionDelay = 0;
							Main.StartInvasion(3);
						}
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -3f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 1844 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon)
				{
					itemTime = ıtem.useTime;
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					if (Main.netMode != 1)
					{
						Main.NewText(Lang.misc[31].Value, 50, byte.MaxValue, 130);
						Main.startPumpkinMoon();
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -4f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 2767 && Main.dayTime && !Main.eclipse)
				{
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					itemTime = ıtem.useTime;
					if (Main.netMode == 0)
					{
						Main.eclipse = true;
						Main.NewText(Lang.misc[20].Value, 50, byte.MaxValue, 130);
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -6f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 3601 && NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
				{
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					itemTime = ıtem.useTime;
					if (Main.netMode == 0)
					{
						WorldGen.StartImpendingDoom();
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -8f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.type == 1958 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon)
				{
					itemTime = ıtem.useTime;
					Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
					if (Main.netMode != 1)
					{
						Main.NewText(Lang.misc[34].Value, 50, byte.MaxValue, 130);
						Main.startSnowMoon();
					}
					else
					{
						NetMessage.SendData(61, -1, -1, "", whoAmI, -5f);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && ıtem.makeNPC > 0 && controlUseItem && base.position.X / 16f - (float)tileRangeX - (float)ıtem.tileBoost <= (float)tileTargetX && (base.position.X + (float)width) / 16f + (float)tileRangeX + (float)ıtem.tileBoost - 1f >= (float)tileTargetX && base.position.Y / 16f - (float)tileRangeY - (float)ıtem.tileBoost <= (float)tileTargetY && (base.position.Y + (float)height) / 16f + (float)tileRangeY + (float)ıtem.tileBoost - 2f >= (float)tileTargetY)
				{
					int num305 = Main.mouseX + (int)Main.screenPosition.X;
					int num306 = Main.mouseY + (int)Main.screenPosition.Y;
					itemTime = ıtem.useTime;
					int i4 = num305 / 16;
					int j3 = num306 / 16;
					if (!WorldGen.SolidTile(i4, j3))
					{
						NPC.ReleaseNPC(num305, num306, ıtem.makeNPC, ıtem.placeStyle, whoAmI);
					}
				}
				if (itemTime == 0 && itemAnimation > 0 && (ıtem.type == 43 || ıtem.type == 70 || ıtem.type == 544 || ıtem.type == 556 || ıtem.type == 557 || ıtem.type == 560 || ıtem.type == 1133 || ıtem.type == 1331) && SummonItemCheck())
				{
					if (ıtem.type == 560)
					{
						itemTime = ıtem.useTime;
						Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
						if (Main.netMode != 1)
						{
							NPC.SpawnOnPlayer(i, 50);
						}
						else
						{
							NetMessage.SendData(61, -1, -1, "", whoAmI, 50f);
						}
					}
					else if (ıtem.type == 43)
					{
						if (!Main.dayTime)
						{
							itemTime = ıtem.useTime;
							Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 4);
							}
							else
							{
								NetMessage.SendData(61, -1, -1, "", whoAmI, 4f);
							}
						}
					}
					else if (ıtem.type == 70)
					{
						if (ZoneCorrupt)
						{
							itemTime = ıtem.useTime;
							Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 13);
							}
							else
							{
								NetMessage.SendData(61, -1, -1, "", whoAmI, 13f);
							}
						}
					}
					else if (ıtem.type == 544)
					{
						if (!Main.dayTime)
						{
							itemTime = ıtem.useTime;
							Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 125);
								NPC.SpawnOnPlayer(i, 126);
							}
							else
							{
								NetMessage.SendData(61, -1, -1, "", whoAmI, 125f);
								NetMessage.SendData(61, -1, -1, "", whoAmI, 126f);
							}
						}
					}
					else if (ıtem.type == 556)
					{
						if (!Main.dayTime)
						{
							itemTime = ıtem.useTime;
							Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 134);
							}
							else
							{
								NetMessage.SendData(61, -1, -1, "", whoAmI, 134f);
							}
						}
					}
					else if (ıtem.type == 557)
					{
						if (!Main.dayTime)
						{
							itemTime = ıtem.useTime;
							Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 127);
							}
							else
							{
								NetMessage.SendData(61, -1, -1, "", whoAmI, 127f);
							}
						}
					}
					else if (ıtem.type == 1133)
					{
						itemTime = ıtem.useTime;
						Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
						if (Main.netMode != 1)
						{
							NPC.SpawnOnPlayer(i, 222);
						}
						else
						{
							NetMessage.SendData(61, -1, -1, "", whoAmI, 222f);
						}
					}
					else if (ıtem.type == 1331 && ZoneCrimson)
					{
						itemTime = ıtem.useTime;
						Main.PlaySound(15, (int)base.position.X, (int)base.position.Y, 0);
						if (Main.netMode != 1)
						{
							NPC.SpawnOnPlayer(i, 266);
						}
						else
						{
							NetMessage.SendData(61, -1, -1, "", whoAmI, 266f);
						}
					}
				}
			}
			if ((ıtem.type == 50 || ıtem.type == 3124 || ıtem.type == 3199) && itemAnimation > 0)
			{
				if (Main.rand.Next(2) == 0)
				{
					Dust.NewDust(base.position, width, height, 15, 0f, 0f, 150, default(Color), 1.1f);
				}
				if (itemTime == 0)
				{
					itemTime = ıtem.useTime;
				}
				else if (itemTime == ıtem.useTime / 2)
				{
					for (int num307 = 0; num307 < 70; num307++)
					{
						Dust.NewDust(base.position, width, height, 15, velocity.X * 0.5f, velocity.Y * 0.5f, 150, default(Color), 1.5f);
					}
					grappling[0] = -1;
					grapCount = 0;
					for (int num308 = 0; num308 < 1000; num308++)
					{
						if (Main.projectile[num308].active && Main.projectile[num308].owner == i && Main.projectile[num308].aiStyle == 7)
						{
							Main.projectile[num308].Kill();
						}
					}
					Spawn();
					for (int num309 = 0; num309 < 70; num309++)
					{
						Dust.NewDust(base.position, width, height, 15, 0f, 0f, 150, default(Color), 1.5f);
					}
				}
			}
			if (ıtem.type == 2350 && itemAnimation > 0)
			{
				if (itemTime == 0)
				{
					itemTime = ıtem.useTime;
				}
				else if (itemTime == 2)
				{
					for (int num310 = 0; num310 < 70; num310++)
					{
						Main.dust[Dust.NewDust(base.position, width, height, 15, velocity.X * 0.2f, velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
					}
					grappling[0] = -1;
					grapCount = 0;
					for (int num311 = 0; num311 < 1000; num311++)
					{
						if (Main.projectile[num311].active && Main.projectile[num311].owner == i && Main.projectile[num311].aiStyle == 7)
						{
							Main.projectile[num311].Kill();
						}
					}
					bool flag19 = immune;
					int num312 = immuneTime;
					Spawn();
					immune = flag19;
					immuneTime = num312;
					for (int num313 = 0; num313 < 70; num313++)
					{
						Main.dust[Dust.NewDust(base.position, width, height, 15, 0f, 0f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
					}
					if (ıtem.stack > 0)
					{
						ıtem.stack--;
					}
				}
			}
			if (ıtem.type == 2351 && itemAnimation > 0)
			{
				if (itemTime == 0)
				{
					itemTime = ıtem.useTime;
				}
				else if (itemTime == 2)
				{
					if (Main.netMode == 0)
					{
						TeleportationPotion();
					}
					else if (Main.netMode == 1 && whoAmI == Main.myPlayer)
					{
						NetMessage.SendData(73);
					}
					if (ıtem.stack > 0)
					{
						ıtem.stack--;
					}
				}
			}
			if (ıtem.type == 2756 && itemAnimation > 0)
			{
				if (itemTime == 0)
				{
					itemTime = ıtem.useTime;
				}
				else if (itemTime == 2)
				{
					if (whoAmI == Main.myPlayer)
					{
						Male = !Male;
						if (Main.netMode == 1)
						{
							NetMessage.SendData(4, -1, -1, name, whoAmI);
						}
					}
					if (ıtem.stack > 0)
					{
						ıtem.stack--;
					}
				}
				else
				{
					float num314 = ıtem.useTime;
					num314 = (num314 - (float)itemTime) / num314;
					float x7 = 15f;
					float num315 = 44f;
					float num316 = (float)Math.PI * 3f;
					Vector2 vector22 = new Vector2(x7, 0f).RotatedBy(num316 * num314);
					vector22.X *= direction;
					for (int num317 = 0; num317 < 2; num317++)
					{
						int type12 = 221;
						if (num317 == 1)
						{
							vector22.X *= -1f;
							type12 = 219;
						}
						Vector2 position18 = new Vector2(vector22.X, num315 * (1f - num314) - num315 + (float)(height / 2));
						position18 += base.Center;
						int num318 = Dust.NewDust(position18, 0, 0, type12, 0f, 0f, 100);
						Main.dust[num318].position = position18;
						Main.dust[num318].noGravity = true;
						Main.dust[num318].velocity = Vector2.Zero;
						Main.dust[num318].scale = 1.3f;
						Main.dust[num318].customData = this;
					}
				}
			}
			if (i != Main.myPlayer)
			{
				return;
			}
			if (itemTime == (int)((float)ıtem.useTime * tileSpeed) && ıtem.tileWand > 0)
			{
				int tileWand2 = ıtem.tileWand;
				for (int num319 = 0; num319 < 58; num319++)
				{
					if (tileWand2 == inventory[num319].type && inventory[num319].stack > 0)
					{
						inventory[num319].stack--;
						if (inventory[num319].stack <= 0)
						{
							inventory[num319] = new Item();
						}
						break;
					}
				}
			}
			int num320 = (ıtem.createTile >= 0) ? ((int)((float)ıtem.useTime * tileSpeed)) : ((ıtem.createWall <= 0) ? ıtem.useTime : ((int)((float)ıtem.useTime * wallSpeed)));
			if (itemTime == num320 && ıtem.consumable)
			{
				bool flag20 = true;
				if (ıtem.type == 2350 || ıtem.type == 2351)
				{
					flag20 = false;
				}
				if (ıtem.type == 2756)
				{
					flag20 = false;
				}
				if (ıtem.ranged)
				{
					if (ammoCost80 && Main.rand.Next(5) == 0)
					{
						flag20 = false;
					}
					if (ammoCost75 && Main.rand.Next(4) == 0)
					{
						flag20 = false;
					}
				}
				if (ıtem.thrown)
				{
					if (thrownCost50 && Main.rand.Next(100) < 50)
					{
						flag20 = false;
					}
					if (thrownCost33 && Main.rand.Next(100) < 33)
					{
						flag20 = false;
					}
				}
				if (ıtem.type >= 71 && ıtem.type <= 74)
				{
					flag20 = true;
				}
				if (flag20)
				{
					if (ıtem.stack > 0)
					{
						ıtem.stack--;
					}
					if (ıtem.stack <= 0)
					{
						itemTime = itemAnimation;
						Main.blockMouse = true;
					}
				}
			}
			if (ıtem.stack <= 0 && itemAnimation == 0)
			{
				inventory[selectedItem] = new Item();
			}
			if (selectedItem == 58 && itemAnimation != 0)
			{
				Main.mouseItem = ıtem.Clone();
			}
		}

		public float GetWeaponKnockback(Item sItem, float KnockBack)
		{
			if (sItem.summon)
			{
				KnockBack += minionKB;
			}
			if (sItem.melee && kbGlove)
			{
				KnockBack *= 2f;
			}
			if (kbBuff)
			{
				KnockBack *= 1.5f;
			}
			if (sItem.ranged && shroomiteStealth)
			{
				KnockBack *= 1f + (1f - stealth) * 0.5f;
			}
			if (sItem.ranged && setVortex)
			{
				KnockBack *= 1f + (1f - stealth) * 0.5f;
			}
			return KnockBack;
		}

		public int GetWeaponDamage(Item sItem)
		{
			int num = sItem.damage;
			if (num > 0)
			{
				if (sItem.melee)
				{
					num = (int)((float)num * meleeDamage + 5E-06f);
				}
				else if (sItem.ranged)
				{
					num = (int)((float)num * rangedDamage + 5E-06f);
					if (sItem.useAmmo == 1 || sItem.useAmmo == 323)
					{
						num = (int)((float)num * arrowDamage + 5E-06f);
					}
					if (sItem.useAmmo == 14 || sItem.useAmmo == 311)
					{
						num = (int)((float)num * bulletDamage + 5E-06f);
					}
					if (sItem.useAmmo == 771 || sItem.useAmmo == 246 || sItem.useAmmo == 312 || sItem.useAmmo == 514)
					{
						num = (int)((float)num * rocketDamage + 5E-06f);
					}
				}
				else if (sItem.thrown)
				{
					num = (int)((float)num * thrownDamage + 5E-06f);
				}
				else if (sItem.magic)
				{
					num = (int)((float)num * magicDamage + 5E-06f);
				}
				else if (sItem.summon)
				{
					num = (int)((float)num * minionDamage);
				}
			}
			return num;
		}

		private void ApplyAnimation(Item sItem)
		{
			if (sItem.melee)
			{
				itemAnimation = (int)((float)sItem.useAnimation * meleeSpeed);
				itemAnimationMax = (int)((float)sItem.useAnimation * meleeSpeed);
			}
			else if (sItem.createTile >= 0)
			{
				itemAnimation = (int)((float)sItem.useAnimation * tileSpeed);
				itemAnimationMax = (int)((float)sItem.useAnimation * tileSpeed);
			}
			else if (sItem.createWall >= 0)
			{
				itemAnimation = (int)((float)sItem.useAnimation * wallSpeed);
				itemAnimationMax = (int)((float)sItem.useAnimation * wallSpeed);
			}
			else
			{
				itemAnimation = sItem.useAnimation;
				itemAnimationMax = sItem.useAnimation;
				reuseDelay = sItem.reuseDelay;
			}
		}

		public bool HasAmmo(Item sItem, bool canUse)
		{
			if (sItem.useAmmo > 0)
			{
				canUse = false;
				for (int i = 0; i < 58; i++)
				{
					if (inventory[i].ammo == sItem.useAmmo && inventory[i].stack > 0)
					{
						canUse = true;
						break;
					}
				}
			}
			return canUse;
		}

		public void PickAmmo(Item sItem, ref int shoot, ref float speed, ref bool canShoot, ref int Damage, ref float KnockBack, bool dontConsume = false)
		{
			Item ıtem = new Item();
			bool flag = false;
			for (int i = 54; i < 58; i++)
			{
				if (inventory[i].ammo == sItem.useAmmo && inventory[i].stack > 0)
				{
					ıtem = inventory[i];
					canShoot = true;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < 54; j++)
				{
					if (inventory[j].ammo == sItem.useAmmo && inventory[j].stack > 0)
					{
						ıtem = inventory[j];
						canShoot = true;
						break;
					}
				}
			}
			if (!canShoot)
			{
				return;
			}
			if (sItem.type == 1946)
			{
				shoot = 338 + ıtem.type - 771;
			}
			else if (sItem.useAmmo == 771)
			{
				shoot += ıtem.shoot;
			}
			else if (sItem.useAmmo == 780)
			{
				shoot += ıtem.shoot;
			}
			else if (ıtem.shoot > 0)
			{
				shoot = ıtem.shoot;
			}
			if (sItem.type == 3019 && shoot == 1)
			{
				shoot = 485;
			}
			if (sItem.type == 3052)
			{
				shoot = 495;
			}
			if (sItem.type == 3245 && shoot == 21)
			{
				shoot = 532;
			}
			if (shoot == 42)
			{
				if (ıtem.type == 370)
				{
					shoot = 65;
					Damage += 5;
				}
				else if (ıtem.type == 408)
				{
					shoot = 68;
					Damage += 5;
				}
				else if (ıtem.type == 1246)
				{
					shoot = 354;
					Damage += 5;
				}
			}
			if (inventory[selectedItem].type == 2888 && shoot == 1)
			{
				shoot = 469;
			}
			if (magicQuiver && (sItem.useAmmo == 1 || sItem.useAmmo == 323))
			{
				KnockBack = (int)((double)KnockBack * 1.1);
				speed *= 1.1f;
			}
			speed += ıtem.shootSpeed;
			if (ıtem.ranged)
			{
				if (ıtem.damage > 0)
				{
					Damage += (int)((float)ıtem.damage * rangedDamage);
				}
			}
			else
			{
				Damage += ıtem.damage;
			}
			if (sItem.useAmmo == 1 && archery)
			{
				if (speed < 20f)
				{
					speed *= 1.2f;
					if (speed > 20f)
					{
						speed = 20f;
					}
				}
				Damage = (int)((double)(float)Damage * 1.2);
			}
			KnockBack += ıtem.knockBack;
			bool flag2 = dontConsume;
			if (sItem.type == 3245)
			{
				if (Main.rand.Next(3) == 0)
				{
					flag2 = true;
				}
				else if (thrownCost33 && Main.rand.Next(100) < 33)
				{
					flag2 = true;
				}
				else if (thrownCost50 && Main.rand.Next(100) < 50)
				{
					flag2 = true;
				}
			}
			if (sItem.type == 3475 && Main.rand.Next(3) != 0)
			{
				flag2 = true;
			}
			if (sItem.type == 3540 && Main.rand.Next(3) != 0)
			{
				flag2 = true;
			}
			if (magicQuiver && sItem.useAmmo == 1 && Main.rand.Next(5) == 0)
			{
				flag2 = true;
			}
			if (ammoBox && Main.rand.Next(5) == 0)
			{
				flag2 = true;
			}
			if (ammoPotion && Main.rand.Next(5) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 1782 && Main.rand.Next(3) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 98 && Main.rand.Next(3) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 2270 && Main.rand.Next(2) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 533 && Main.rand.Next(2) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 1929 && Main.rand.Next(2) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 1553 && Main.rand.Next(2) == 0)
			{
				flag2 = true;
			}
			if (sItem.type == 434 && itemAnimation < sItem.useAnimation - 2)
			{
				flag2 = true;
			}
			if (ammoCost80 && Main.rand.Next(5) == 0)
			{
				flag2 = true;
			}
			if (ammoCost75 && Main.rand.Next(4) == 0)
			{
				flag2 = true;
			}
			if (shoot == 85 && itemAnimation < itemAnimationMax - 6)
			{
				flag2 = true;
			}
			if ((shoot == 145 || shoot == 146 || shoot == 147 || shoot == 148 || shoot == 149) && itemAnimation < itemAnimationMax - 5)
			{
				flag2 = true;
			}
			if (!flag2 && ıtem.consumable)
			{
				ıtem.stack--;
				if (ıtem.stack <= 0)
				{
					ıtem.active = false;
					ıtem.type = 0;
				}
			}
		}

		public void PickTile(int x, int y, int pickPower)
		{
			int num = 0;
			int tileId = hitTile.HitObject(x, y, 1);
			Tile tile = Main.tile[x, y];
			if (Main.tileNoFail[tile.type])
			{
				num = 100;
			}
			num = ((!Main.tileDungeon[tile.type] && tile.type != 25 && tile.type != 58 && tile.type != 117 && tile.type != 203) ? ((tile.type != 48 && tile.type != 232) ? ((tile.type == 226) ? (num + pickPower / 4) : ((tile.type != 107 && tile.type != 221) ? ((tile.type != 108 && tile.type != 222) ? ((tile.type == 111 || tile.type == 223) ? (num + pickPower / 4) : ((tile.type != 211) ? (num + pickPower) : (num + pickPower / 5))) : (num + pickPower / 3)) : (num + pickPower / 2))) : (num + pickPower / 4)) : (num + pickPower / 2));
			if (tile.type == 211 && pickPower < 200)
			{
				num = 0;
			}
			if ((tile.type == 25 || tile.type == 203) && pickPower < 65)
			{
				num = 0;
			}
			else if (tile.type == 117 && pickPower < 65)
			{
				num = 0;
			}
			else if (tile.type == 37 && pickPower < 50)
			{
				num = 0;
			}
			else if (tile.type == 404 && pickPower < 65)
			{
				num = 0;
			}
			else if ((tile.type == 22 || tile.type == 204) && (double)y > Main.worldSurface && pickPower < 55)
			{
				num = 0;
			}
			else if (tile.type == 56 && pickPower < 65)
			{
				num = 0;
			}
			else if (tile.type == 58 && pickPower < 65)
			{
				num = 0;
			}
			else if ((tile.type == 226 || tile.type == 237) && pickPower < 210)
			{
				num = 0;
			}
			else if (Main.tileDungeon[tile.type] && pickPower < 65)
			{
				if ((double)x < (double)Main.maxTilesX * 0.35 || (double)x > (double)Main.maxTilesX * 0.65)
				{
					num = 0;
				}
			}
			else if (tile.type == 107 && pickPower < 100)
			{
				num = 0;
			}
			else if (tile.type == 108 && pickPower < 110)
			{
				num = 0;
			}
			else if (tile.type == 111 && pickPower < 150)
			{
				num = 0;
			}
			else if (tile.type == 221 && pickPower < 100)
			{
				num = 0;
			}
			else if (tile.type == 222 && pickPower < 110)
			{
				num = 0;
			}
			else if (tile.type == 223 && pickPower < 150)
			{
				num = 0;
			}
			if (tile.type == 147 || tile.type == 0 || tile.type == 40 || tile.type == 53 || tile.type == 57 || tile.type == 59 || tile.type == 123 || tile.type == 224 || tile.type == 397)
			{
				num += pickPower;
			}
			if (tile.type == 165 || Main.tileRope[tile.type] || tile.type == 199 || Main.tileMoss[tile.type])
			{
				num = 100;
			}
			if (hitTile.AddDamage(tileId, num, false) >= 100 && (tile.type == 2 || tile.type == 23 || tile.type == 60 || tile.type == 70 || tile.type == 109 || tile.type == 199 || Main.tileMoss[tile.type]))
			{
				num = 0;
			}
			if (tile.type == 128 || tile.type == 269)
			{
				if (tile.frameX == 18 || tile.frameX == 54)
				{
					x--;
					tile = Main.tile[x, y];
					hitTile.UpdatePosition(tileId, x, y);
				}
				if (tile.frameX >= 100)
				{
					num = 0;
					Main.blockMouse = true;
				}
			}
			if (tile.type == 334)
			{
				if (tile.frameY == 0)
				{
					y++;
					tile = Main.tile[x, y];
					hitTile.UpdatePosition(tileId, x, y);
				}
				if (tile.frameY == 36)
				{
					y--;
					tile = Main.tile[x, y];
					hitTile.UpdatePosition(tileId, x, y);
				}
				int frameX = tile.frameX;
				bool flag = frameX >= 5000;
				bool flag2 = false;
				if (!flag)
				{
					int num2 = frameX / 18;
					num2 %= 3;
					x -= num2;
					tile = Main.tile[x, y];
					if (tile.frameX >= 5000)
					{
						flag = true;
					}
				}
				if (flag)
				{
					frameX = tile.frameX;
					int num3 = 0;
					while (frameX >= 5000)
					{
						frameX -= 5000;
						num3++;
					}
					if (num3 != 0)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					num = 0;
					Main.blockMouse = true;
				}
			}
			if (!WorldGen.CanKillTile(x, y))
			{
				num = 0;
			}
			if (hitTile.AddDamage(tileId, num) >= 100)
			{
				AchievementsHelper.CurrentlyMining = true;
				hitTile.Clear(tileId);
				if (Main.netMode == 1 && Main.tileContainer[Main.tile[x, y].type])
				{
					WorldGen.KillTile(x, y, true);
					NetMessage.SendData(17, -1, -1, "", 0, x, y, 1f);
					if (Main.tile[x, y].type == 21)
					{
						NetMessage.SendData(34, -1, -1, "", 1, x, y);
					}
					if (Main.tile[x, y].type == 88)
					{
						NetMessage.SendData(34, -1, -1, "", 3, x, y);
					}
				}
				else
				{
					int num4 = y;
					Main.tile[x, num4].active();
					WorldGen.KillTile(x, num4);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, num4);
					}
				}
				AchievementsHelper.CurrentlyMining = false;
			}
			else
			{
				WorldGen.KillTile(x, y, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(17, -1, -1, "", 0, x, y, 1f);
				}
			}
			if (num != 0)
			{
				hitTile.Prune();
			}
		}

		public bool ItemFitsWeaponRack(Item i)
		{
			bool flag = false;
			if (i.fishingPole > 0)
			{
				flag = true;
			}
			int netID = i.netID;
			if (netID == 905 || netID == 1326)
			{
				flag = true;
			}
			if ((i.damage > 0 || flag) && i.useStyle > 0)
			{
				return i.stack > 0;
			}
			return false;
		}

		public void PlaceWeapon(int x, int y)
		{
			if (Main.tile[x, y].active() && Main.tile[x, y].type == 334)
			{
				int frameY = Main.tile[x, y].frameY;
				int num = 1;
				frameY /= 18;
				while (num > frameY)
				{
					y++;
					frameY = Main.tile[x, y].frameY;
					frameY /= 18;
				}
				while (num < frameY)
				{
					y--;
					frameY = Main.tile[x, y].frameY;
					frameY /= 18;
				}
				int num2 = Main.tile[x, y].frameX;
				int num3 = 0;
				while (num2 >= 5000)
				{
					num2 -= 5000;
					num3++;
				}
				if (num3 != 0)
				{
					num2 = (num3 - 1) * 18;
				}
				bool flag = false;
				if (num2 >= 54)
				{
					num2 -= 54;
					flag = true;
				}
				x -= num2 / 18;
				int num4 = Main.tile[x, y].frameX;
				WorldGen.KillTile(x, y, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(17, -1, -1, "", 0, x, y, 1f);
				}
				if (Main.netMode == 1)
				{
					NetMessage.SendData(17, -1, -1, "", 0, x + 1, y, 1f);
				}
				while (num4 >= 5000)
				{
					num4 -= 5000;
				}
				Main.blockMouse = true;
				int num5 = 5000;
				int num6 = 10000;
				if (flag)
				{
					num5 = 20000;
					num6 = 25000;
				}
				Main.tile[x, y].frameX = (short)(inventory[selectedItem].netID + num5 + 100);
				Main.tile[x + 1, y].frameX = (short)(inventory[selectedItem].prefix + num6);
				if (Main.netMode == 1)
				{
					NetMessage.SendTileSquare(-1, x, y, 1);
				}
				if (Main.netMode == 1)
				{
					NetMessage.SendTileSquare(-1, x + 1, y, 1);
				}
				inventory[selectedItem].stack--;
				if (inventory[selectedItem].stack <= 0)
				{
					inventory[selectedItem].SetDefaults();
					Main.mouseItem.SetDefaults();
				}
				if (selectedItem == 58)
				{
					Main.mouseItem = inventory[selectedItem].Clone();
				}
				releaseUseItem = false;
				mouseInterface = true;
			}
		}

		public bool ItemFitsItemFrame(Item i)
		{
			return i.stack > 0;
		}

		public void PlaceItemInFrame(int x, int y)
		{
			if (Main.tile[x, y].frameX % 36 != 0)
			{
				x--;
			}
			if (Main.tile[x, y].frameY % 36 != 0)
			{
				y--;
			}
			int num = TEItemFrame.Find(x, y);
			if (num == -1)
			{
				return;
			}
			if (((TEItemFrame)TileEntity.ByID[num]).item.stack > 0)
			{
				WorldGen.KillTile(x, y, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(17, -1, -1, "", 0, tileTargetX, y, 1f);
				}
			}
			if (Main.netMode == 1)
			{
				NetMessage.SendData(89, -1, -1, "", x, y, selectedItem, whoAmI);
			}
			else
			{
				TEItemFrame.TryPlacing(x, y, inventory[selectedItem].netID, inventory[selectedItem].prefix, inventory[selectedItem].stack);
			}
			inventory[selectedItem].stack--;
			if (inventory[selectedItem].stack <= 0)
			{
				inventory[selectedItem].SetDefaults();
				Main.mouseItem.SetDefaults();
			}
			if (selectedItem == 58)
			{
				Main.mouseItem = inventory[selectedItem].Clone();
			}
			releaseUseItem = false;
			mouseInterface = true;
		}

		public Color GetImmuneAlpha(Color newColor, float alphaReduction)
		{
			float num = (float)(255 - immuneAlpha) / 255f;
			if (alphaReduction > 0f)
			{
				num *= 1f - alphaReduction;
			}
			if (immuneAlpha > 125)
			{
				return Color.Transparent;
			}
			return Color.Multiply(newColor, num);
		}

		public Color GetImmuneAlphaPure(Color newColor, float alphaReduction)
		{
			float num = (float)(255 - immuneAlpha) / 255f;
			if (alphaReduction > 0f)
			{
				num *= 1f - alphaReduction;
			}
			return Color.Multiply(newColor, num);
		}

		public Color GetDeathAlpha(Color newColor)
		{
			int r = newColor.R + (int)((double)immuneAlpha * 0.9);
			int g = newColor.G + (int)((double)immuneAlpha * 0.5);
			int b = newColor.B + (int)((double)immuneAlpha * 0.5);
			int num = newColor.A + (int)((double)immuneAlpha * 0.4);
			if (num < 0)
			{
				num = 0;
			}
			if (num > 255)
			{
				num = 255;
			}
			return new Color(r, g, b, num);
		}

		public void addDPS(int dmg)
		{
			if (dpsStarted)
			{
				dpsLastHit = DateTime.Now;
				dpsDamage += dmg;
				dpsEnd = DateTime.Now;
			}
			else
			{
				dpsStarted = true;
				dpsStart = DateTime.Now;
				dpsEnd = DateTime.Now;
				dpsLastHit = DateTime.Now;
				dpsDamage = dmg;
			}
		}

		public void checkDPSTime()
		{
			int num = 3;
			if (dpsStarted && (DateTime.Now - dpsLastHit).Seconds >= num)
			{
				dpsStarted = false;
			}
		}

		public int getDPS()
		{
			TimeSpan timeSpan = dpsEnd - dpsStart;
			float num = (float)timeSpan.Milliseconds / 1000f;
			num += (float)timeSpan.Seconds;
			num += (float)timeSpan.Minutes / 60f;
			if (num >= 3f)
			{
				dpsStart = DateTime.Now;
				dpsStart = dpsStart.AddSeconds(-1.0);
				dpsDamage = (int)((float)dpsDamage / num);
				timeSpan = dpsEnd - dpsStart;
				num = (float)timeSpan.Milliseconds / 1000f;
				num += (float)timeSpan.Seconds;
				num += (float)timeSpan.Minutes / 60f;
			}
			if (num < 1f)
			{
				num = 1f;
			}
			float num2 = (float)dpsDamage / num;
			return (int)num2;
		}

		public int DropCoins()
		{
			int num = 0;
			for (int i = 0; i < 59; i++)
			{
				if (inventory[i].type >= 71 && inventory[i].type <= 74)
				{
					int num2 = Item.NewItem((int)position.X, (int)position.Y, width, height, inventory[i].type);
					int num3 = inventory[i].stack / 2;
					if (Main.expertMode)
					{
						num3 = (int)((double)inventory[i].stack * 0.25);
					}
					num3 = inventory[i].stack - num3;
					inventory[i].stack -= num3;
					if (inventory[i].type == 71)
					{
						num += num3;
					}
					if (inventory[i].type == 72)
					{
						num += num3 * 100;
					}
					if (inventory[i].type == 73)
					{
						num += num3 * 10000;
					}
					if (inventory[i].type == 74)
					{
						num += num3 * 1000000;
					}
					if (inventory[i].stack <= 0)
					{
						inventory[i] = new Item();
					}
					Main.item[num2].stack = num3;
					Main.item[num2].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
					Main.item[num2].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
					Main.item[num2].noGrabDelay = 100;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num2);
					}
					if (i == 58)
					{
						Main.mouseItem = inventory[i].Clone();
					}
				}
			}
			lostCoins = num;
			lostCoinString = Main.ValueToCoins(lostCoins);
			return num;
		}

		public void DropItems()
		{
			for (int i = 0; i < 59; i++)
			{
				if (inventory[i].stack > 0 && inventory[i].type != 3509 && inventory[i].type != 3506 && inventory[i].type != 3507)
				{
					int num = Item.NewItem((int)position.X, (int)position.Y, width, height, inventory[i].type);
					Main.item[num].netDefaults(inventory[i].netID);
					Main.item[num].Prefix(inventory[i].prefix);
					Main.item[num].stack = inventory[i].stack;
					Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
					Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
					Main.item[num].noGrabDelay = 100;
					Main.item[num].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num);
					}
				}
				inventory[i] = new Item();
				if (i < armor.Length)
				{
					if (armor[i].stack > 0)
					{
						int num2 = Item.NewItem((int)position.X, (int)position.Y, width, height, armor[i].type);
						Main.item[num2].netDefaults(armor[i].netID);
						Main.item[num2].Prefix(armor[i].prefix);
						Main.item[num2].stack = armor[i].stack;
						Main.item[num2].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
						Main.item[num2].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
						Main.item[num2].noGrabDelay = 100;
						Main.item[num2].newAndShiny = false;
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", num2);
						}
					}
					armor[i] = new Item();
				}
				if (i < dye.Length)
				{
					if (dye[i].stack > 0)
					{
						int num3 = Item.NewItem((int)position.X, (int)position.Y, width, height, dye[i].type);
						Main.item[num3].netDefaults(dye[i].netID);
						Main.item[num3].Prefix(dye[i].prefix);
						Main.item[num3].stack = dye[i].stack;
						Main.item[num3].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
						Main.item[num3].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
						Main.item[num3].noGrabDelay = 100;
						Main.item[num3].newAndShiny = false;
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", num3);
						}
					}
					dye[i] = new Item();
				}
				if (i < miscEquips.Length)
				{
					if (miscEquips[i].stack > 0)
					{
						int num4 = Item.NewItem((int)position.X, (int)position.Y, width, height, miscEquips[i].type);
						Main.item[num4].netDefaults(miscEquips[i].netID);
						Main.item[num4].Prefix(miscEquips[i].prefix);
						Main.item[num4].stack = miscEquips[i].stack;
						Main.item[num4].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
						Main.item[num4].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
						Main.item[num4].noGrabDelay = 100;
						Main.item[num4].newAndShiny = false;
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, "", num4);
						}
					}
					miscEquips[i] = new Item();
				}
				if (i >= miscDyes.Length)
				{
					continue;
				}
				if (miscDyes[i].stack > 0)
				{
					int num5 = Item.NewItem((int)position.X, (int)position.Y, width, height, miscDyes[i].type);
					Main.item[num5].netDefaults(miscDyes[i].netID);
					Main.item[num5].Prefix(miscDyes[i].prefix);
					Main.item[num5].stack = miscDyes[i].stack;
					Main.item[num5].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
					Main.item[num5].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
					Main.item[num5].noGrabDelay = 100;
					Main.item[num5].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num5);
					}
				}
				miscDyes[i] = new Item();
			}
			inventory[0].SetDefaults(3507);
			inventory[0].Prefix(-1);
			inventory[1].SetDefaults(3509);
			inventory[1].Prefix(-1);
			inventory[2].SetDefaults(3506);
			inventory[2].Prefix(-1);
			Main.mouseItem = new Item();
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public object clientClone()
		{
			Player player = new Player();
			player.zone1 = zone1;
			player.zone2 = zone2;
			player.extraAccessory = extraAccessory;
			player.MinionTargetPoint = MinionTargetPoint;
			player.direction = direction;
			player.selectedItem = selectedItem;
			player.controlUp = controlUp;
			player.controlDown = controlDown;
			player.controlLeft = controlLeft;
			player.controlRight = controlRight;
			player.controlJump = controlJump;
			player.controlUseItem = controlUseItem;
			player.statLife = statLife;
			player.statLifeMax = statLifeMax;
			player.statMana = statMana;
			player.statManaMax = statManaMax;
			player.position.X = position.X;
			player.chest = chest;
			player.talkNPC = talkNPC;
			player.hideVisual = hideVisual;
			player.hideMisc = hideMisc;
			for (int i = 0; i < 59; i++)
			{
				player.inventory[i] = inventory[i].Clone();
				if (i < armor.Length)
				{
					player.armor[i] = armor[i].Clone();
				}
				if (i < dye.Length)
				{
					player.dye[i] = dye[i].Clone();
				}
				if (i < miscEquips.Length)
				{
					player.miscEquips[i] = miscEquips[i].Clone();
				}
				if (i < miscDyes.Length)
				{
					player.miscDyes[i] = miscDyes[i].Clone();
				}
				if (i < bank.item.Length)
				{
					player.bank.item[i] = bank.item[i].Clone();
				}
				if (i < bank2.item.Length)
				{
					player.bank2.item[i] = bank2.item[i].Clone();
				}
			}
			player.trashItem = trashItem.Clone();
			for (int j = 0; j < 22; j++)
			{
				player.buffType[j] = buffType[j];
				player.buffTime[j] = buffTime[j];
			}
			return player;
		}

		public static bool CheckSpawn(int x, int y)
		{
			if (x < 10 || x > Main.maxTilesX - 10 || y < 10 || y > Main.maxTilesX - 10)
			{
				return false;
			}
			if (Main.tile[x, y - 1] == null)
			{
				return false;
			}
			if (!Main.tile[x, y - 1].active() || Main.tile[x, y - 1].type != 79)
			{
				return false;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 3; j < y; j++)
				{
					if (Main.tile[i, j] == null)
					{
						return false;
					}
					if (Main.tile[i, j].nactive() && Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						Main.NewText(Language.GetTextValue("Game.BedObstructed"), byte.MaxValue, 240, 20);
						return false;
					}
				}
			}
			if (!WorldGen.StartRoomCheck(x, y - 1))
			{
				return false;
			}
			return true;
		}

		public void FindSpawn()
		{
			int num = 0;
			while (true)
			{
				if (num < 200)
				{
					if (spN[num] == null)
					{
						SpawnX = -1;
						SpawnY = -1;
						return;
					}
					if (spN[num] == Main.worldName && spI[num] == Main.worldID)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			SpawnX = spX[num];
			SpawnY = spY[num];
		}

		public void RemoveSpawn()
		{
			SpawnX = -1;
			SpawnY = -1;
			int num = 0;
			while (true)
			{
				if (num >= 200 || spN[num] == null)
				{
					return;
				}
				if (spN[num] == Main.worldName && spI[num] == Main.worldID)
				{
					break;
				}
				num++;
			}
			for (int i = num; i < 199; i++)
			{
				spN[i] = spN[i + 1];
				spI[i] = spI[i + 1];
				spX[i] = spX[i + 1];
				spY[i] = spY[i + 1];
			}
			spN[199] = null;
			spI[199] = 0;
			spX[199] = 0;
			spY[199] = 0;
		}

		public void ChangeSpawn(int x, int y)
		{
			for (int i = 0; i < 200 && spN[i] != null; i++)
			{
				if (spN[i] == Main.worldName && spI[i] == Main.worldID)
				{
					for (int num = i; num > 0; num--)
					{
						spN[num] = spN[num - 1];
						spI[num] = spI[num - 1];
						spX[num] = spX[num - 1];
						spY[num] = spY[num - 1];
					}
					spN[0] = Main.worldName;
					spI[0] = Main.worldID;
					spX[0] = x;
					spY[0] = y;
					return;
				}
			}
			for (int num2 = 199; num2 > 0; num2--)
			{
				if (spN[num2 - 1] != null)
				{
					spN[num2] = spN[num2 - 1];
					spI[num2] = spI[num2 - 1];
					spX[num2] = spX[num2 - 1];
					spY[num2] = spY[num2 - 1];
				}
			}
			spN[0] = Main.worldName;
			spI[0] = Main.worldID;
			spX[0] = x;
			spY[0] = y;
		}

		public static void SavePlayer(PlayerFileData playerFile, bool skipMapSave = false)
		{
			Main.Achievements.Save();
			string path = playerFile.Path;
			Player player = playerFile.Player;
			bool ısCloudSave = playerFile.IsCloudSave;
			if (!skipMapSave)
			{
				try
				{
					if (Main.mapEnabled)
					{
						Main.Map.Save();
					}
				}
				catch
				{
				}
				try
				{
					if (!ısCloudSave)
					{
						Directory.CreateDirectory(Main.PlayerPath);
					}
				}
				catch
				{
				}
			}
			if (!Main.ServerSideCharacter && path != null && !(path == ""))
			{
				if (FileUtilities.Exists(path, ısCloudSave))
				{
					FileUtilities.Copy(path, path + ".bak", ısCloudSave);
				}
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				using (Stream stream = ısCloudSave ? ((Stream)new MemoryStream(2000)) : ((Stream)new FileStream(path, FileMode.Create)))
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, rijndaelManaged.CreateEncryptor(ENCRYPTION_KEY, ENCRYPTION_KEY), CryptoStreamMode.Write))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
						{
							binaryWriter.Write(Main.curRelease);
							playerFile.Metadata.Write(binaryWriter);
							binaryWriter.Write(player.name);
							binaryWriter.Write(player.difficulty);
							binaryWriter.Write(playerFile.GetPlayTime().Ticks);
							binaryWriter.Write(player.hair);
							binaryWriter.Write(player.hairDye);
							BitsByte bb = (byte)0;
							for (int i = 0; i < 8; i++)
							{
								bb[i] = player.hideVisual[i];
							}
							binaryWriter.Write(bb);
							bb = (byte)0;
							for (int j = 0; j < 2; j++)
							{
								bb[j] = player.hideVisual[j + 8];
							}
							binaryWriter.Write(bb);
							binaryWriter.Write(player.hideMisc);
							binaryWriter.Write((byte)player.skinVariant);
							binaryWriter.Write(player.statLife);
							binaryWriter.Write(player.statLifeMax);
							binaryWriter.Write(player.statMana);
							binaryWriter.Write(player.statManaMax);
							binaryWriter.Write(player.extraAccessory);
							binaryWriter.Write(player.taxMoney);
							binaryWriter.Write(player.hairColor.R);
							binaryWriter.Write(player.hairColor.G);
							binaryWriter.Write(player.hairColor.B);
							binaryWriter.Write(player.skinColor.R);
							binaryWriter.Write(player.skinColor.G);
							binaryWriter.Write(player.skinColor.B);
							binaryWriter.Write(player.eyeColor.R);
							binaryWriter.Write(player.eyeColor.G);
							binaryWriter.Write(player.eyeColor.B);
							binaryWriter.Write(player.shirtColor.R);
							binaryWriter.Write(player.shirtColor.G);
							binaryWriter.Write(player.shirtColor.B);
							binaryWriter.Write(player.underShirtColor.R);
							binaryWriter.Write(player.underShirtColor.G);
							binaryWriter.Write(player.underShirtColor.B);
							binaryWriter.Write(player.pantsColor.R);
							binaryWriter.Write(player.pantsColor.G);
							binaryWriter.Write(player.pantsColor.B);
							binaryWriter.Write(player.shoeColor.R);
							binaryWriter.Write(player.shoeColor.G);
							binaryWriter.Write(player.shoeColor.B);
							for (int k = 0; k < player.armor.Length; k++)
							{
								binaryWriter.Write(player.armor[k].netID);
								binaryWriter.Write(player.armor[k].prefix);
							}
							for (int l = 0; l < player.dye.Length; l++)
							{
								binaryWriter.Write(player.dye[l].netID);
								binaryWriter.Write(player.dye[l].prefix);
							}
							for (int m = 0; m < 58; m++)
							{
								binaryWriter.Write(player.inventory[m].netID);
								binaryWriter.Write(player.inventory[m].stack);
								binaryWriter.Write(player.inventory[m].prefix);
								binaryWriter.Write(player.inventory[m].favorited);
							}
							for (int n = 0; n < player.miscEquips.Length; n++)
							{
								binaryWriter.Write(player.miscEquips[n].netID);
								binaryWriter.Write(player.miscEquips[n].prefix);
								binaryWriter.Write(player.miscDyes[n].netID);
								binaryWriter.Write(player.miscDyes[n].prefix);
							}
							for (int num = 0; num < 40; num++)
							{
								binaryWriter.Write(player.bank.item[num].netID);
								binaryWriter.Write(player.bank.item[num].stack);
								binaryWriter.Write(player.bank.item[num].prefix);
							}
							for (int num2 = 0; num2 < 40; num2++)
							{
								binaryWriter.Write(player.bank2.item[num2].netID);
								binaryWriter.Write(player.bank2.item[num2].stack);
								binaryWriter.Write(player.bank2.item[num2].prefix);
							}
							for (int num3 = 0; num3 < 22; num3++)
							{
								if (Main.buffNoSave[player.buffType[num3]])
								{
									binaryWriter.Write(0);
									binaryWriter.Write(0);
								}
								else
								{
									binaryWriter.Write(player.buffType[num3]);
									binaryWriter.Write(player.buffTime[num3]);
								}
							}
							for (int num4 = 0; num4 < 200; num4++)
							{
								if (player.spN[num4] == null)
								{
									binaryWriter.Write(-1);
									break;
								}
								binaryWriter.Write(player.spX[num4]);
								binaryWriter.Write(player.spY[num4]);
								binaryWriter.Write(player.spI[num4]);
								binaryWriter.Write(player.spN[num4]);
							}
							binaryWriter.Write(player.hbLocked);
							for (int num5 = 0; num5 < player.hideInfo.Length; num5++)
							{
								binaryWriter.Write(player.hideInfo[num5]);
							}
							binaryWriter.Write(player.anglerQuestsFinished);
							binaryWriter.Flush();
							cryptoStream.FlushFinalBlock();
							stream.Flush();
							if (ısCloudSave && SocialAPI.Cloud != null)
							{
								SocialAPI.Cloud.Write(playerFile.Path, ((MemoryStream)stream).GetBuffer(), (int)((MemoryStream)stream).Length);
							}
						}
					}
				}
			}
		}

		public static PlayerFileData LoadPlayer(string playerPath, bool cloudSave)
		{
			PlayerFileData playerFileData = new PlayerFileData(playerPath, cloudSave);
			if (cloudSave && SocialAPI.Cloud == null)
			{
				return playerFileData;
			}
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			Player player = new Player();
			try
			{
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				rijndaelManaged.Padding = PaddingMode.None;
				byte[] buffer = FileUtilities.ReadAllBytes(playerPath, cloudSave);
				using (MemoryStream stream = new MemoryStream(buffer))
				{
					using (CryptoStream input = new CryptoStream(stream, rijndaelManaged.CreateDecryptor(ENCRYPTION_KEY, ENCRYPTION_KEY), CryptoStreamMode.Read))
					{
						using (BinaryReader binaryReader = new BinaryReader(input))
						{
							int num = binaryReader.ReadInt32();
							if (num >= 135)
							{
								playerFileData.Metadata = FileMetadata.Read(binaryReader, FileType.Player);
							}
							else
							{
								playerFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.Player);
							}
							if (num > Main.maxSupportSaveRelease)
							{
								player.loadStatus = 1;
								player.name = binaryReader.ReadString();
								playerFileData.Player = player;
								return playerFileData;
							}
							player.name = binaryReader.ReadString();
							if (num >= 10)
							{
								if (num >= 17)
								{
									player.difficulty = binaryReader.ReadByte();
								}
								else if (binaryReader.ReadBoolean())
								{
									player.difficulty = 2;
								}
							}
							if (num >= 138)
							{
								playerFileData.SetPlayTime(new TimeSpan(binaryReader.ReadInt64()));
							}
							else
							{
								playerFileData.SetPlayTime(TimeSpan.Zero);
							}
							player.hair = binaryReader.ReadInt32();
							if (num >= 82)
							{
								player.hairDye = binaryReader.ReadByte();
							}
							if (num >= 124)
							{
								BitsByte bitsByte = binaryReader.ReadByte();
								for (int i = 0; i < 8; i++)
								{
									player.hideVisual[i] = bitsByte[i];
								}
								bitsByte = binaryReader.ReadByte();
								for (int j = 0; j < 2; j++)
								{
									player.hideVisual[j + 8] = bitsByte[j];
								}
							}
							else if (num >= 83)
							{
								BitsByte bitsByte2 = binaryReader.ReadByte();
								for (int k = 0; k < 8; k++)
								{
									player.hideVisual[k] = bitsByte2[k];
								}
							}
							if (num >= 119)
							{
								player.hideMisc = binaryReader.ReadByte();
							}
							if (num <= 17)
							{
								if (player.hair == 5 || player.hair == 6 || player.hair == 9 || player.hair == 11)
								{
									player.Male = false;
								}
								else
								{
									player.Male = true;
								}
							}
							else if (num < 107)
							{
								player.Male = binaryReader.ReadBoolean();
							}
							else
							{
								player.skinVariant = binaryReader.ReadByte();
							}
							player.statLife = binaryReader.ReadInt32();
							player.statLifeMax = binaryReader.ReadInt32();
							if (player.statLifeMax > 500)
							{
								player.statLifeMax = 500;
							}
							player.statMana = binaryReader.ReadInt32();
							player.statManaMax = binaryReader.ReadInt32();
							if (player.statManaMax > 200)
							{
								player.statManaMax = 200;
							}
							if (player.statMana > 400)
							{
								player.statMana = 400;
							}
							if (num >= 125)
							{
								player.extraAccessory = binaryReader.ReadBoolean();
							}
							if (num >= 128)
							{
								player.taxMoney = binaryReader.ReadInt32();
							}
							player.hairColor = binaryReader.ReadRGB();
							player.skinColor = binaryReader.ReadRGB();
							player.eyeColor = binaryReader.ReadRGB();
							player.shirtColor = binaryReader.ReadRGB();
							player.underShirtColor = binaryReader.ReadRGB();
							player.pantsColor = binaryReader.ReadRGB();
							player.shoeColor = binaryReader.ReadRGB();
							Main.player[Main.myPlayer].shirtColor = player.shirtColor;
							Main.player[Main.myPlayer].pantsColor = player.pantsColor;
							Main.player[Main.myPlayer].hairColor = player.hairColor;
							if (num >= 38)
							{
								if (num < 124)
								{
									int num2 = 11;
									if (num >= 81)
									{
										num2 = 16;
									}
									for (int l = 0; l < num2; l++)
									{
										int num3 = l;
										if (num3 >= 8)
										{
											num3 += 2;
										}
										player.armor[num3].netDefaults(binaryReader.ReadInt32());
										player.armor[num3].Prefix(binaryReader.ReadByte());
									}
								}
								else
								{
									int num4 = 20;
									for (int m = 0; m < num4; m++)
									{
										player.armor[m].netDefaults(binaryReader.ReadInt32());
										player.armor[m].Prefix(binaryReader.ReadByte());
									}
								}
								if (num >= 47)
								{
									int num5 = 3;
									if (num >= 81)
									{
										num5 = 8;
									}
									if (num >= 124)
									{
										num5 = 10;
									}
									for (int n = 0; n < num5; n++)
									{
										int num6 = n;
										player.dye[num6].netDefaults(binaryReader.ReadInt32());
										player.dye[num6].Prefix(binaryReader.ReadByte());
									}
								}
								if (num >= 58)
								{
									for (int num7 = 0; num7 < 58; num7++)
									{
										int num8 = binaryReader.ReadInt32();
										if (num8 >= 3602)
										{
											player.inventory[num7].netDefaults(0);
										}
										else
										{
											player.inventory[num7].netDefaults(num8);
											player.inventory[num7].stack = binaryReader.ReadInt32();
											player.inventory[num7].Prefix(binaryReader.ReadByte());
											if (num >= 114)
											{
												player.inventory[num7].favorited = binaryReader.ReadBoolean();
											}
										}
									}
								}
								else
								{
									for (int num9 = 0; num9 < 48; num9++)
									{
										int num10 = binaryReader.ReadInt32();
										if (num10 >= 3602)
										{
											player.inventory[num9].netDefaults(0);
										}
										else
										{
											player.inventory[num9].netDefaults(num10);
											player.inventory[num9].stack = binaryReader.ReadInt32();
											player.inventory[num9].Prefix(binaryReader.ReadByte());
										}
									}
								}
								if (num >= 117)
								{
									if (num < 136)
									{
										for (int num11 = 0; num11 < 5; num11++)
										{
											if (num11 != 1)
											{
												int num12 = binaryReader.ReadInt32();
												if (num12 >= 3602)
												{
													player.miscEquips[num11].netDefaults(0);
												}
												else
												{
													player.miscEquips[num11].netDefaults(num12);
													player.miscEquips[num11].Prefix(binaryReader.ReadByte());
												}
												num12 = binaryReader.ReadInt32();
												if (num12 >= 3602)
												{
													player.miscDyes[num11].netDefaults(0);
												}
												else
												{
													player.miscDyes[num11].netDefaults(num12);
													player.miscDyes[num11].Prefix(binaryReader.ReadByte());
												}
											}
										}
									}
									else
									{
										for (int num13 = 0; num13 < 5; num13++)
										{
											int num14 = binaryReader.ReadInt32();
											if (num14 >= 3602)
											{
												player.miscEquips[num13].netDefaults(0);
											}
											else
											{
												player.miscEquips[num13].netDefaults(num14);
												player.miscEquips[num13].Prefix(binaryReader.ReadByte());
											}
											num14 = binaryReader.ReadInt32();
											if (num14 >= 3602)
											{
												player.miscDyes[num13].netDefaults(0);
											}
											else
											{
												player.miscDyes[num13].netDefaults(num14);
												player.miscDyes[num13].Prefix(binaryReader.ReadByte());
											}
										}
									}
								}
								if (num >= 58)
								{
									for (int num15 = 0; num15 < 40; num15++)
									{
										player.bank.item[num15].netDefaults(binaryReader.ReadInt32());
										player.bank.item[num15].stack = binaryReader.ReadInt32();
										player.bank.item[num15].Prefix(binaryReader.ReadByte());
									}
									for (int num16 = 0; num16 < 40; num16++)
									{
										player.bank2.item[num16].netDefaults(binaryReader.ReadInt32());
										player.bank2.item[num16].stack = binaryReader.ReadInt32();
										player.bank2.item[num16].Prefix(binaryReader.ReadByte());
									}
								}
								else
								{
									for (int num17 = 0; num17 < 20; num17++)
									{
										player.bank.item[num17].netDefaults(binaryReader.ReadInt32());
										player.bank.item[num17].stack = binaryReader.ReadInt32();
										player.bank.item[num17].Prefix(binaryReader.ReadByte());
									}
									for (int num18 = 0; num18 < 20; num18++)
									{
										player.bank2.item[num18].netDefaults(binaryReader.ReadInt32());
										player.bank2.item[num18].stack = binaryReader.ReadInt32();
										player.bank2.item[num18].Prefix(binaryReader.ReadByte());
									}
								}
							}
							else
							{
								for (int num19 = 0; num19 < 8; num19++)
								{
									player.armor[num19].SetDefaults(Item.VersionName(binaryReader.ReadString(), num));
									if (num >= 36)
									{
										player.armor[num19].Prefix(binaryReader.ReadByte());
									}
								}
								if (num >= 6)
								{
									for (int num20 = 8; num20 < 11; num20++)
									{
										player.armor[num20].SetDefaults(Item.VersionName(binaryReader.ReadString(), num));
										if (num >= 36)
										{
											player.armor[num20].Prefix(binaryReader.ReadByte());
										}
									}
								}
								for (int num21 = 0; num21 < 44; num21++)
								{
									player.inventory[num21].SetDefaults(Item.VersionName(binaryReader.ReadString(), num));
									player.inventory[num21].stack = binaryReader.ReadInt32();
									if (num >= 36)
									{
										player.inventory[num21].Prefix(binaryReader.ReadByte());
									}
								}
								if (num >= 15)
								{
									for (int num22 = 44; num22 < 48; num22++)
									{
										player.inventory[num22].SetDefaults(Item.VersionName(binaryReader.ReadString(), num));
										player.inventory[num22].stack = binaryReader.ReadInt32();
										if (num >= 36)
										{
											player.inventory[num22].Prefix(binaryReader.ReadByte());
										}
									}
								}
								for (int num23 = 0; num23 < 20; num23++)
								{
									player.bank.item[num23].SetDefaults(Item.VersionName(binaryReader.ReadString(), num));
									player.bank.item[num23].stack = binaryReader.ReadInt32();
									if (num >= 36)
									{
										player.bank.item[num23].Prefix(binaryReader.ReadByte());
									}
								}
								if (num >= 20)
								{
									for (int num24 = 0; num24 < 20; num24++)
									{
										player.bank2.item[num24].SetDefaults(Item.VersionName(binaryReader.ReadString(), num));
										player.bank2.item[num24].stack = binaryReader.ReadInt32();
										if (num >= 36)
										{
											player.bank2.item[num24].Prefix(binaryReader.ReadByte());
										}
									}
								}
							}
							if (num < 58)
							{
								for (int num25 = 40; num25 < 48; num25++)
								{
									player.inventory[num25 + 10] = player.inventory[num25].Clone();
									player.inventory[num25].SetDefaults();
								}
							}
							if (num >= 11)
							{
								int num26 = 22;
								if (num < 74)
								{
									num26 = 10;
								}
								for (int num27 = 0; num27 < num26; num27++)
								{
									player.buffType[num27] = binaryReader.ReadInt32();
									player.buffTime[num27] = binaryReader.ReadInt32();
									if (player.buffType[num27] == 0)
									{
										num27--;
										num26--;
									}
								}
							}
							for (int num28 = 0; num28 < 200; num28++)
							{
								int num29 = binaryReader.ReadInt32();
								if (num29 == -1)
								{
									break;
								}
								player.spX[num28] = num29;
								player.spY[num28] = binaryReader.ReadInt32();
								player.spI[num28] = binaryReader.ReadInt32();
								player.spN[num28] = binaryReader.ReadString();
							}
							if (num >= 16)
							{
								player.hbLocked = binaryReader.ReadBoolean();
							}
							if (num >= 115)
							{
								int num30 = 13;
								for (int num31 = 0; num31 < num30; num31++)
								{
									player.hideInfo[num31] = binaryReader.ReadBoolean();
								}
							}
							if (num >= 98)
							{
								player.anglerQuestsFinished = binaryReader.ReadInt32();
							}
							player.skinVariant = (int)MathHelper.Clamp(player.skinVariant, 0f, 7f);
							for (int num32 = 3; num32 < 8 + player.extraAccessorySlots; num32++)
							{
								int type = player.armor[num32].type;
								if (type == 908)
								{
									player.lavaMax += 420;
								}
								if (type == 906)
								{
									player.lavaMax += 420;
								}
								if (player.wingsLogic == 0 && player.armor[num32].wingSlot >= 0)
								{
									player.wingsLogic = player.armor[num32].wingSlot;
								}
								if (type == 158 || type == 396 || type == 1250 || type == 1251 || type == 1252)
								{
									player.noFallDmg = true;
								}
								player.lavaTime = player.lavaMax;
							}
						}
					}
				}
				player.PlayerFrame();
				player.loadStatus = 0;
				playerFileData.Player = player;
				return playerFileData;
			}
			catch
			{
			}
			Player player2 = new Player();
			player2.loadStatus = 2;
			if (player.name != "")
			{
				player2.name = player.name;
			}
			else
			{
				string[] array = playerPath.Split(Path.DirectorySeparatorChar);
				player.name = array[array.Length - 1].Split('.')[0];
			}
			playerFileData.Player = player2;
			return playerFileData;
		}

		public static PlayerFileData GetFileData(string file, bool cloudSave)
		{
			if (file == null || (cloudSave && SocialAPI.Cloud == null))
			{
				return null;
			}
			PlayerFileData playerFileData = LoadPlayer(file, cloudSave);
			if (playerFileData.Player != null)
			{
				if (playerFileData.Player.loadStatus != 0 && playerFileData.Player.loadStatus != 1)
				{
					if (FileUtilities.Exists(file + ".bak", cloudSave))
					{
						FileUtilities.Move(file + ".bak", file, cloudSave);
					}
					playerFileData = LoadPlayer(file, cloudSave);
					if (playerFileData.Player == null)
					{
						return null;
					}
				}
				return playerFileData;
			}
			return null;
		}

		public Color GetHairColor(bool useLighting = true)
		{
			Color color = Lighting.GetColor((int)((double)position.X + (double)width * 0.5) / 16, (int)(((double)position.Y + (double)height * 0.25) / 16.0));
			return GameShaders.Hair.GetColor(hairDye, this, useLighting ? color : Color.White);
		}

		public bool HasItem(int type)
		{
			for (int i = 0; i < 58; i++)
			{
				if (type == inventory[i].type && inventory[i].stack > 0)
				{
					return true;
				}
			}
			return false;
		}

		public int FindItem(int netid)
		{
			for (int i = 0; i < 58; i++)
			{
				if (netid == inventory[i].netID && inventory[i].stack > 0)
				{
					return i;
				}
			}
			return -1;
		}

		public int FindItem(List<int> netids)
		{
			for (int i = 0; i < 58; i++)
			{
				if (inventory[i].stack > 0 && netids.Contains(inventory[i].netID))
				{
					return i;
				}
			}
			return -1;
		}

		public int FindItem(bool[] validtypes)
		{
			for (int i = 0; i < 58; i++)
			{
				if (inventory[i].stack > 0 && validtypes[inventory[i].type])
				{
					return i;
				}
			}
			return -1;
		}

		public Player()
		{
			width = 20;
			height = 42;
			name = string.Empty;
			for (int i = 0; i < 59; i++)
			{
				if (i < armor.Length)
				{
					armor[i] = new Item();
				}
				inventory[i] = new Item();
			}
			for (int j = 0; j < 40; j++)
			{
				bank.item[j] = new Item();
				bank2.item[j] = new Item();
			}
			for (int k = 0; k < dye.Length; k++)
			{
				dye[k] = new Item();
			}
			for (int l = 0; l < miscEquips.Length; l++)
			{
				miscEquips[l] = new Item();
			}
			for (int m = 0; m < miscDyes.Length; m++)
			{
				miscDyes[m] = new Item();
			}
			trashItem = new Item();
			grappling[0] = -1;
			inventory[0].SetDefaults(3507);
			inventory[1].SetDefaults(3509);
			inventory[2].SetDefaults(3506);
			statManaMax = 20;
			extraAccessory = false;
			if (Main.cEd)
			{
				inventory[3].SetDefaults(603);
			}
			for (int n = 0; n < 419; n++)
			{
				adjTile[n] = false;
				oldAdjTile[n] = false;
			}
			hitTile = new HitTile();
			mount = new Mount();
		}

		public void TeleportationPotion()
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int width = base.width;
			Vector2 vector = new Vector2(num2, num3) * 16f + new Vector2(-width / 2 + 8, -height);
			while (!flag && num < 1000)
			{
				num++;
				num2 = 100 + Main.rand.Next(Main.maxTilesX - 200);
				num3 = 100 + Main.rand.Next(Main.maxTilesY - 200);
				vector = new Vector2(num2, num3) * 16f + new Vector2(-width / 2 + 8, -height);
				if (Collision.SolidCollision(vector, width, height))
				{
					continue;
				}
				if (Main.tile[num2, num3] == null)
				{
					Main.tile[num2, num3] = new Tile();
				}
				if ((Main.tile[num2, num3].wall == 87 && (double)num3 > Main.worldSurface && !NPC.downedPlantBoss) || (Main.wallDungeon[Main.tile[num2, num3].wall] && (double)num3 > Main.worldSurface && !NPC.downedBoss3))
				{
					continue;
				}
				int num4 = 0;
				while (num4 < 100)
				{
					if (Main.tile[num2, num3 + num4] == null)
					{
						Main.tile[num2, num3 + num4] = new Tile();
					}
					Tile tile = Main.tile[num2, num3 + num4];
					vector = new Vector2(num2, num3 + num4) * 16f + new Vector2(-width / 2 + 8, -height);
					Vector4 vector2 = Collision.SlopeCollision(vector, velocity, width, height, gravDir);
					bool flag2 = !Collision.SolidCollision(vector, width, height);
					if (vector2.Z == velocity.X)
					{
						float y = velocity.Y;
					}
					if (flag2)
					{
						num4++;
						continue;
					}
					if (tile.active() && !tile.inActive() && Main.tileSolid[tile.type])
					{
						break;
					}
					num4++;
				}
				if (Collision.LavaCollision(vector, width, height) || Collision.HurtTiles(vector, velocity, width, height).Y > 0f)
				{
					continue;
				}
				Collision.SlopeCollision(vector, velocity, width, height, gravDir);
				if (!Collision.SolidCollision(vector, width, height) || num4 >= 99)
				{
					continue;
				}
				Vector2 vector3 = Vector2.UnitX * 16f;
				if (Collision.TileCollision(vector - vector3, vector3, base.width, height, false, false, (int)gravDir) != vector3)
				{
					continue;
				}
				vector3 = -Vector2.UnitX * 16f;
				if (Collision.TileCollision(vector - vector3, vector3, base.width, height, false, false, (int)gravDir) != vector3)
				{
					continue;
				}
				vector3 = Vector2.UnitY * 16f;
				if (!(Collision.TileCollision(vector - vector3, vector3, base.width, height, false, false, (int)gravDir) != vector3))
				{
					vector3 = -Vector2.UnitY * 16f;
					if (!(Collision.TileCollision(vector - vector3, vector3, base.width, height, false, false, (int)gravDir) != vector3))
					{
						flag = true;
						num3 += num4;
						break;
					}
				}
			}
			if (flag)
			{
				Vector2 newPos = vector;
				Teleport(newPos, 2);
				velocity = Vector2.Zero;
				if (Main.netMode == 2)
				{
					RemoteClient.CheckSection(whoAmI, position);
					NetMessage.SendData(65, -1, -1, "", 0, whoAmI, newPos.X, newPos.Y, 3);
				}
			}
		}

		public void GetAnglerReward()
		{
			Item ıtem = new Item();
			ıtem.type = 0;
			float num = 1f;
			if (anglerQuestsFinished <= 50)
			{
				num -= (float)anglerQuestsFinished * 0.01f;
			}
			else if (anglerQuestsFinished <= 100)
			{
				num = 0.5f - (float)(anglerQuestsFinished - 50) * 0.005f;
			}
			else if (anglerQuestsFinished <= 150)
			{
				num = 0.25f - (float)(anglerQuestsFinished - 100) * 0.002f;
			}
			if (anglerQuestsFinished == 5)
			{
				ıtem.SetDefaults(2428);
			}
			else if (anglerQuestsFinished == 10)
			{
				ıtem.SetDefaults(2367);
			}
			else if (anglerQuestsFinished == 15)
			{
				ıtem.SetDefaults(2368);
			}
			else if (anglerQuestsFinished == 20)
			{
				ıtem.SetDefaults(2369);
			}
			else if (anglerQuestsFinished == 30)
			{
				ıtem.SetDefaults(2294);
			}
			else if (anglerQuestsFinished > 75 && Main.rand.Next((int)(250f * num)) == 0)
			{
				ıtem.SetDefaults(2294);
			}
			else if (Main.hardMode && anglerQuestsFinished > 25 && Main.rand.Next((int)(100f * num)) == 0)
			{
				ıtem.SetDefaults(2422);
			}
			else if (Main.hardMode && anglerQuestsFinished > 10 && Main.rand.Next((int)(70f * num)) == 0)
			{
				ıtem.SetDefaults(2494);
			}
			else if (Main.hardMode && anglerQuestsFinished > 10 && Main.rand.Next((int)(70f * num)) == 0)
			{
				ıtem.SetDefaults(3031);
			}
			else if (Main.hardMode && anglerQuestsFinished > 10 && Main.rand.Next((int)(70f * num)) == 0)
			{
				ıtem.SetDefaults(3032);
			}
			else if (Main.rand.Next((int)(80f * num)) == 0)
			{
				ıtem.SetDefaults(3183);
			}
			else if (Main.rand.Next((int)(60f * num)) == 0)
			{
				ıtem.SetDefaults(2360);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(2373);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(2374);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(2375);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(3120);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(3037);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(3096);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(2417);
			}
			else if (Main.rand.Next((int)(40f * num)) == 0)
			{
				ıtem.SetDefaults(2498);
			}
			else
			{
				switch (Main.rand.Next(70))
				{
				case 0:
					ıtem.SetDefaults(2442);
					break;
				case 1:
					ıtem.SetDefaults(2443);
					break;
				case 2:
					ıtem.SetDefaults(2444);
					break;
				case 3:
					ıtem.SetDefaults(2445);
					break;
				case 4:
					ıtem.SetDefaults(2497);
					break;
				case 5:
					ıtem.SetDefaults(2495);
					break;
				case 6:
					ıtem.SetDefaults(2446);
					break;
				case 7:
					ıtem.SetDefaults(2447);
					break;
				case 8:
					ıtem.SetDefaults(2448);
					break;
				case 9:
					ıtem.SetDefaults(2449);
					break;
				case 10:
					ıtem.SetDefaults(2490);
					break;
				case 11:
					ıtem.SetDefaults(2435);
					ıtem.stack = Main.rand.Next(50, 151);
					break;
				case 12:
					ıtem.SetDefaults(2496);
					break;
				default:
					switch (Main.rand.Next(3))
					{
					case 0:
						ıtem.SetDefaults(2354);
						ıtem.stack = Main.rand.Next(2, 6);
						break;
					case 1:
						ıtem.SetDefaults(2355);
						ıtem.stack = Main.rand.Next(2, 6);
						break;
					default:
						ıtem.SetDefaults(2356);
						ıtem.stack = Main.rand.Next(2, 6);
						break;
					}
					break;
				}
			}
			ıtem.position = base.Center;
			Item ıtem2 = GetItem(whoAmI, ıtem, true);
			if (ıtem2.stack > 0)
			{
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
			}
			if (ıtem.type == 2417)
			{
				Item ıtem3 = new Item();
				Item ıtem4 = new Item();
				ıtem3.SetDefaults(2418);
				ıtem3.position = base.Center;
				ıtem2 = GetItem(whoAmI, ıtem3, true);
				if (ıtem2.stack > 0)
				{
					int number2 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number2, 1f);
					}
				}
				ıtem4.SetDefaults(2419);
				ıtem4.position = base.Center;
				ıtem2 = GetItem(whoAmI, ıtem4, true);
				if (ıtem2.stack > 0)
				{
					int number3 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number3, 1f);
					}
				}
			}
			else if (ıtem.type == 2498)
			{
				Item ıtem5 = new Item();
				Item ıtem6 = new Item();
				ıtem5.SetDefaults(2499);
				ıtem5.position = base.Center;
				ıtem2 = GetItem(whoAmI, ıtem5, true);
				if (ıtem2.stack > 0)
				{
					int number4 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number4, 1f);
					}
				}
				ıtem6.SetDefaults(2500);
				ıtem6.position = base.Center;
				ıtem2 = GetItem(whoAmI, ıtem6, true);
				if (ıtem2.stack > 0)
				{
					int number5 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", number5, 1f);
					}
				}
			}
			Item ıtem7 = new Item();
			int num2 = (anglerQuestsFinished + 50) / 2;
			num2 = (int)((float)(num2 * Main.rand.Next(50, 201)) * 0.015f);
			num2 = (int)((double)num2 * 1.5);
			if (Main.expertMode)
			{
				num2 *= 2;
			}
			if (num2 > 100)
			{
				num2 /= 100;
				if (num2 > 10)
				{
					num2 = 10;
				}
				if (num2 < 1)
				{
					num2 = 1;
				}
				ıtem7.SetDefaults(73);
				ıtem7.stack = num2;
			}
			else
			{
				if (num2 > 99)
				{
					num2 = 99;
				}
				if (num2 < 1)
				{
					num2 = 1;
				}
				ıtem7.SetDefaults(72);
				ıtem7.stack = num2;
			}
			ıtem7.position = base.Center;
			ıtem2 = GetItem(whoAmI, ıtem7, true);
			if (ıtem2.stack > 0)
			{
				int number6 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number6, 1f);
				}
			}
			if (Main.rand.Next((int)(100f * num)) > 50)
			{
				return;
			}
			Item ıtem8 = new Item();
			if (Main.rand.Next((int)(15f * num)) == 0)
			{
				ıtem8.SetDefaults(2676);
			}
			else if (Main.rand.Next((int)(5f * num)) == 0)
			{
				ıtem8.SetDefaults(2675);
			}
			else
			{
				ıtem8.SetDefaults(2674);
			}
			if (Main.rand.Next(25) <= anglerQuestsFinished)
			{
				ıtem8.stack++;
			}
			if (Main.rand.Next(50) <= anglerQuestsFinished)
			{
				ıtem8.stack++;
			}
			if (Main.rand.Next(100) <= anglerQuestsFinished)
			{
				ıtem8.stack++;
			}
			if (Main.rand.Next(150) <= anglerQuestsFinished)
			{
				ıtem8.stack++;
			}
			if (Main.rand.Next(200) <= anglerQuestsFinished)
			{
				ıtem8.stack++;
			}
			if (Main.rand.Next(250) <= anglerQuestsFinished)
			{
				ıtem8.stack++;
			}
			ıtem8.position = base.Center;
			ıtem2 = GetItem(whoAmI, ıtem8, true);
			if (ıtem2.stack > 0)
			{
				int number7 = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number7, 1f);
				}
			}
		}

		public void GetDyeTraderReward()
		{
			int num = -1;
			List<int> list = new List<int>();
			list.Add(3560);
			list.Add(3028);
			list.Add(3041);
			list.Add(3040);
			list.Add(3025);
			list.Add(3190);
			list.Add(3027);
			list.Add(3026);
			list.Add(3554);
			list.Add(3553);
			list.Add(3555);
			list.Add(2872);
			list.Add(3534);
			list.Add(2871);
			List<int> list2 = list;
			if (Main.hardMode)
			{
				list2.Add(3039);
				list2.Add(3038);
				list2.Add(3598);
				list2.Add(3597);
				list2.Add(3600);
				list2.Add(3042);
				list2.Add(3533);
				list2.Add(3561);
				if (NPC.downedMechBossAny)
				{
					list2.Add(2883);
					list2.Add(2869);
					list2.Add(2873);
					list2.Add(2870);
				}
				if (NPC.downedPlantBoss)
				{
					list2.Add(2878);
					list2.Add(2879);
					list2.Add(2884);
					list2.Add(2885);
				}
				if (NPC.downedMartians)
				{
					list2.Add(2864);
					list2.Add(3556);
				}
				if (NPC.downedMoonlord)
				{
					list2.Add(3024);
				}
			}
			num = list2[Main.rand.Next(list2.Count)];
			Item ıtem = new Item();
			ıtem.SetDefaults(num);
			ıtem.stack = 3;
			ıtem.position = base.Center;
			Item ıtem2 = GetItem(whoAmI, ıtem, true);
			if (ıtem2.stack > 0)
			{
				int number = Item.NewItem((int)position.X, (int)position.Y, width, height, ıtem2.type, ıtem2.stack, false, 0, true);
				if (Main.netMode == 1)
				{
					NetMessage.SendData(21, -1, -1, "", number, 1f);
				}
			}
		}

		public bool CheckMana(int amount, bool pay = false, bool blockQuickMana = false)
		{
			int num = (int)((float)amount * manaCost);
			if (statMana >= num)
			{
				if (pay)
				{
					statMana -= num;
				}
				return true;
			}
			if (manaFlower && !blockQuickMana)
			{
				QuickMana();
				if (statMana >= num)
				{
					if (pay)
					{
						statMana -= num;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		public void TryPortalJumping()
		{
			if (!mount.Active && !dead)
			{
				PortalHelper.TryGoingThroughPortals(this);
			}
		}

		public bool ConsumeSolarFlare()
		{
			if (setSolar && solarShields > 0)
			{
				if (Main.netMode == 1 && whoAmI != Main.myPlayer)
				{
					return true;
				}
				solarShields--;
				for (int i = 0; i < 22; i++)
				{
					if (buffType[i] >= 170 && buffType[i] <= 172)
					{
						DelBuff(i);
					}
				}
				if (solarShields > 0)
				{
					AddBuff(170 + solarShields - 1, 5, false);
				}
				solarCounter = 0;
				return true;
			}
			return false;
		}

		public void KeyDoubleTap(int keyDir)
		{
			int num = 0;
			if (Main.ReversedUpDownArmorSetBonuses)
			{
				num = 1;
			}
			if (keyDir == num)
			{
				if (setVortex && !mount.Active)
				{
					vortexStealthActive = !vortexStealthActive;
				}
				if (setStardust)
				{
					MinionTargetAim();
				}
			}
		}

		public void KeyHoldDown(int keyDir, int holdTime)
		{
			int num = 0;
			if (Main.ReversedUpDownArmorSetBonuses)
			{
				num = 1;
			}
			if (keyDir == num && setStardust && holdTime >= 60)
			{
				MinionTargetPoint = Vector2.Zero;
			}
		}

		public void MinionTargetAim()
		{
			Vector2 mouseWorld = Main.MouseWorld;
			float y = mouseWorld.Y;
			int num = (int)mouseWorld.X / 16;
			int num2 = (int)y / 16;
			int num3 = 0;
			if (Main.tile[num, num2].nactive() && Main.tileSolid[Main.tile[num, num2].type] && !Main.tileSolidTop[Main.tile[num, num2].type])
			{
				int num4 = 0;
				int num5 = 0;
				while (num5 > -20 && num2 + num5 > 1)
				{
					int num6 = num2 + num5;
					if (Main.tile[num, num6].nactive() && Main.tileSolid[Main.tile[num, num6].type] && !Main.tileSolidTop[Main.tile[num, num6].type])
					{
						num4 = num5;
						num5--;
						continue;
					}
					num4 = num5;
					break;
				}
				int num7 = 0;
				for (int i = 0; i < 20 && num2 + i < Main.maxTilesY; i++)
				{
					int num8 = num2 + i;
					if (Main.tile[num, num8].nactive() && Main.tileSolid[Main.tile[num, num8].type] && !Main.tileSolidTop[Main.tile[num, num8].type])
					{
						num7 = i;
						continue;
					}
					num7 = i;
					break;
				}
				num3 = ((num7 <= -num4) ? (num7 + 3) : (num4 - 2));
			}
			int num9 = num2 + num3;
			bool flag = false;
			for (int j = num9; j < num9 + 5; j++)
			{
				if (WorldGen.SolidTileAllowBottomSlope(num, j))
				{
					flag = true;
				}
			}
			while (!flag)
			{
				num9++;
				for (int k = num9; k < num9 + 5; k++)
				{
					if (WorldGen.SolidTileAllowBottomSlope(num, k))
					{
						flag = true;
					}
				}
			}
			Vector2 vector = new Vector2(num * 16 + 8, num9 * 16);
			if (Distance(vector) <= 1000f)
			{
				MinionTargetPoint = vector;
			}
		}

		public void UpdateMinionTarget()
		{
			if (whoAmI != Main.myPlayer)
			{
				return;
			}
			if (Distance(MinionTargetPoint) > 1000f)
			{
				MinionTargetPoint = Vector2.Zero;
			}
			if (stardustGuardian && HasMinionTarget)
			{
				Vector2 minionTargetPoint = MinionTargetPoint;
				float num = (float)miscCounter / 150f;
				float num2 = (float)Math.PI * 2f / 3f;
				for (int i = 0; i < 3; i++)
				{
					int num3 = Dust.NewDust(minionTargetPoint, 0, 0, 135, 0f, 0f, 100, default(Color), 1.5f);
					Main.dust[num3].noGravity = true;
					Main.dust[num3].velocity = Vector2.Zero;
					Main.dust[num3].noLight = true;
					Main.dust[num3].position = minionTargetPoint + (num * ((float)Math.PI * 2f) + num2 * (float)i).ToRotationVector2() * 4f;
					Main.dust[num3].shader = GameShaders.Armor.GetSecondaryShader(cPet, this);
				}
			}
		}

		public void NebulaLevelup(int type)
		{
			if (whoAmI != Main.myPlayer)
			{
				return;
			}
			int time = 480;
			for (int i = 0; i < 22; i++)
			{
				if (buffType[i] >= type && buffType[i] < type + 3)
				{
					DelBuff(i);
				}
			}
			switch (type)
			{
			case 173:
				nebulaLevelLife = (int)MathHelper.Clamp(nebulaLevelLife + 1, 0f, 3f);
				AddBuff(type + nebulaLevelLife - 1, time);
				break;
			case 176:
				nebulaLevelMana = (int)MathHelper.Clamp(nebulaLevelMana + 1, 0f, 3f);
				AddBuff(type + nebulaLevelMana - 1, time);
				break;
			case 179:
				nebulaLevelDamage = (int)MathHelper.Clamp(nebulaLevelDamage + 1, 0f, 3f);
				AddBuff(type + nebulaLevelDamage - 1, time);
				break;
			}
		}

		public void UpdateTouchingTiles()
		{
			TouchedTiles.Clear();
			List<Point> list = null;
			List<Point> list2 = null;
			if (!Collision.IsClearSpotHack(position + velocity, 16f, width, height, false, false, (int)gravDir, true, true))
			{
				list = Collision.FindCollisionTile((Math.Sign(velocity.Y) == 1) ? 2 : 3, position + velocity, 16f, width, height, false, false, (int)gravDir);
			}
			if (!Collision.IsClearSpotHack(position, Math.Abs(velocity.Y), width, height, false, false, (int)gravDir, true, true))
			{
				list2 = Collision.FindCollisionTile((Math.Sign(velocity.Y) == 1) ? 2 : 3, position, Math.Abs(velocity.Y), width, height, false, false, (int)gravDir, true, true);
			}
			if (list != null && list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (!list.Contains(list2[i]))
					{
						list.Add(list2[i]);
					}
				}
			}
			if (list == null && list2 != null)
			{
				list = list2;
			}
			if (list != null)
			{
				TouchedTiles = list;
			}
		}
	}
}
