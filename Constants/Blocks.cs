using System;

namespace SpacecraftGT
{
	public enum Block : byte
	{
		Air = 0,
		Rock = 1,
		Grass = 2,
		Dirt = 3,
		Cobblestone = 4,
		Wood = 5,
		Sapling = 6,
		Adminium = 7,
		Water = 8,
		StillWater = 9,
		Lava = 10,
		StillLava = 11,
		Sand = 12,
		Gravel = 13,
		GoldOre = 14,
		IronOre = 15,
		CoalOre = 16,
		Log = 17,
		Leaves = 18,
		Sponge = 19,
		Glass = 20,

		LapisOre = 21,
		LapisBlock = 22,
		Dispenser = 23,
		Sandstone = 24,
		NoteBlock = 25,
		/* Unused space */
		
		Cloth = 35,
		/* Unused space */
		
		YellowFlower = 37,
		RedFlower = 38,
		RedMushroom = 39,
		BrownMushroom = 40,

		Gold = 41,
		Iron = 42,
		DoubleStair = 43,
		Stair = 44,
		Brick = 45,
		TNT = 46,
		Books = 47,
		MossyCobblestone = 48,
		Obsidian = 49,
		
		Torch = 50,
		Fire = 51,
		MobSpawner = 52,
		WoodenStairs = 53,
		Chest = 54,
		RedstoneWire = 55,
		DiamondOre = 56,
		DiamondBlock = 57,
		Workbench = 58,
		Crops = 59,
		Soil = 60,
		Furnace = 61,
		BurningFurnace = 62,
		
		Signpost = 63,
		WoodenDoor = 64,
		Ladder = 65,
		MinecartTracks = 66,
		CobblestoneStairs = 67,
		WallSign = 68,
		
		Lever = 69,
		StonePressurePlate = 70,
		IronDoor = 71,
		WoodenPressurePlate = 72,
		RedstoneOre = 73,
		GlowingRedstoneOre = 74,
		RedstoneTorchOff = 75,
		RedstoneTorchOn = 76,
		StoneButton = 77,
		
		SnowSurface = 78,
		Ice = 79,
		SnowBlock = 80,
		Cactus = 81,
		Clay = 82,
		Reed = 83,
		Jukebox = 84,
		Fence = 85,
		
		Pumpkin = 86,
		Netherstone = 87,
		Nethermud = 88,
		Lightstone = 89,
		Portal = 90,
		LitPumpkin = 91,
		CakeBlock = 92
	}
	
	public enum Item : short
	{
		IronShovel = 256,
		IronPick = 257,
		IronAxe = 258,
		FlintAndSteel = 259,
		Apple = 260,
		Bow = 261,
		Arrow = 262,
		Coal = 263,
		Diamond = 264,
		Iron = 265,
		Gold = 266,
		IronSword = 267,
		WoodenSword = 268,
		WoodenShovel = 269,
		WoodenPick = 270,
		WoodenAxe = 271,
		StoneSword = 272,
		StoneShovel = 273,
		StonePick = 274,
		StoneAxe = 275,
		DiamondSword = 276,
		DiamondShovel = 277,
		DiamondPick = 278,
		DiamondAxe = 279,
		Stick = 280,
		Bowl = 281,
		MushroomSoup = 282,
		GoldSword = 283,
		GoldShovel = 284,
		GoldPickaxe = 285,
		GoldAxe = 286,
		String = 287,
		Feather = 288,
		Gunpowder = 289,
		WoodenHoe = 290,
		StoneHoe = 291,
		IronHoe = 292,
		DiamondHoe = 293,
		GoldHoe = 294,
		Seeds = 295,
		Wheat = 296,
		Bread = 297,
		LeatherHelmet = 298,
		LeatherChestplate = 299,
		LeatherPants = 300,
		LeatherBoots = 301,
		ChainmailHelmet = 302,
		ChainmailChestplate = 303,
		ChainmailPants = 304,
		ChainmailBoots = 305,
		IronHelmet = 306,
		IronChestplate = 307,
		IronPants = 308,
		IronBoots = 309,
		DiamondHelmet = 310,
		DiamondChestplate = 311,
		DiamondPants = 312,
		DiamondBoots = 313,
		GoldHelmet = 314,
		GoldChestplate = 315,
		GoldPants = 316,
		GoldBoots = 317,
		Flint = 318,
		Pork = 319,
		GrilledPork = 320,
		Painting = 321,
		GoldenApple = 322,
		Sign = 323,
		WoodenDoor = 324,
		Bucket = 325,
		WaterBucket = 326,
		LavaBucket = 327,
		Minecart = 328,
		Saddle = 329,
		IronDoor = 330,
		Redstone = 331,
		Snowball = 332,
		Boat = 333,
		Leather = 334,
		MilkBucket = 335,
		ClayBrick = 336,
		Clay = 337,
		Reed = 338,
		Paper = 339,
		Book = 340,
		Slime = 341,
		StorageMinecart = 342,
		PoweredMinecart = 343,
		Egg = 344,
		Compass = 345,
		FishingRod = 346,
		Watch = 347,
		Lightstone = 348,
		RawFish = 349,
		CookedFish = 350,
		InkSack = 351,
		Bone = 352,
		Sugar = 353,
		Cake = 354
	}
	
	public enum Record : short
	{
		Gold = 2256,
		Green = 2257
	}
	
	public static class Metadata
	{
		public enum Wood : byte {
			Normal, Redwood, Birch
		}
		public enum Liquid : byte {
			Full = 0, LavaMax = 3, WaterMax = 7, Falling = 8
		}
		public enum Wool : byte {
			White, Orange, Magenta, LightBlue, Yellow, LightGreen, Pink, Gray,
			LightGray, Cyan, Purple, Blue, Brown, DarkGreen, Red, Black
		}
		public enum Dyes : byte {
			InkSack, RoseRed, CactusGreen, CocoBeans, LapisLazuli, Purple, Cyan, LightGray,
			Gray, Pink, Lime, DandelionYellow, LightBlue, Magenta, Orange, BoneMeal
		}
		public enum Torch : byte {
			South = 1, North, East, West, Standing
		}
		public enum Tracks : byte {
			EastWest, NorthSouth, RiseSouth, RiseNorth, RiseEast, RiseWest,
			NECorner, SECorner, SWCorner, NWCorner
		}
		public enum Ladders : byte {
			East = 2, West, North, South
		}
		public enum Stairs : byte {
			South, North, West, East
		}
		public enum Lever : byte {
			SouthWall = 1, NorthWall, WestWall, EastWall, EWGround, NSGround,
			IsFlipped = 8
		}
		public enum Door : byte {
			Northeast, Southeast, Southwest, Northwest,
			IsOpen = 4, IsTopHalf = 8
		}
		public enum Button : byte {
			SouthWall = 1, NorthWall, WestWall, EastWall,
			IsPressed = 8
		}
		public enum Sign : byte {
			West = 0, North = 4, East = 8, South = 0xC,
			WallEast = 2, WallWest = 3, WallNorth = 4, WallSouth = 5
		}
		public enum Furnace : byte {
			East = 2, West, North, South
		}
		public enum Pumpkin : byte {
			East, South, West, North
		}
	}
}