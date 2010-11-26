using System;

namespace SpacecraftGT
{
	public enum PacketType : byte
	{
		KeepAlive = 0x00,           // c <-> s
		LoginDetails = 0x01,        //   <->
		Handshake = 0x02,           //   <->
		Message = 0x03,             //   <->
		PlayerInventory = 0x05,     //   <->
		SpawnPosition = 0x06,       //   <--
		Player = 0x0A,              //   -->
		PlayerPosition = 0x0B,      //   -->
		PlayerLook = 0x0C,          //   -->
		PlayerPositionLook = 0x0D,  //   <->
		PlayerDigging = 0x0E,       //   -->
		PlayerBlockPlace = 0x0F,    //   -->
		PlayerHolding = 0x10,       //   -->
		AddToInventory = 0x11,      //   <--
		ArmAnimation = 0x12,        //   <--
		NamedEntitySpawn = 0x14,    //   <--
		PickupSpawn = 0x15,         //   <--
		CollectItem = 0x16,         //   <--
		AddObjectVehicle = 0x17,    //   <--
		MobSpawn = 0x18,            //   <--
		DestroyEntity = 0x1D,       //   <--
		Entity = 0x1E,              //   <--
		EntityRelativeMove = 0x1F,  //   <--
		EntityLook = 0x20,          //   <--
		EntityLookAndMove = 0x21,   //   <--
		EntityTeleport = 0x22,      //   <--
		PreChunk = 0x32,            //   <--
		MapChunk = 0x33,            //   <--
		MultiBlockChange = 0x34,    //   <--
		BlockChange = 0x35,         //   <--
		ComplexEntity = 0x3B,       //   <--
		Disconnect = 0xFF           //   <->
	}
	
	public static class PacketStructure
	{
		// b - byte(1)
		// s - short(2)
		// i - int(4)
		// l - long(8)
		// f - float(4)
		// d - double(8)
		// t - string - short-prefixed
		// x - bytearray - special handling
		
		public static string[] Data = {
			"b",               // keep alive - 0x00
			"bittlb",          // login request
			"bt",              // handshake
			"bt",              // chat message
			"bisx",            // player inventory
			"biii",            // spawn position
			"", "", "", "",
			"bb",              // player - 0x0A
			"bddddb",          // player position
			"bffb",            // player look
			"bddddffb",        // player position/look
			"bbibib",          // player digging
			"bsibib",          // player block placement
			"bis",             // holding change
			"bsbs",            // add to inventory
			"bib",             // arm animation
			"",
			"bitiiibbs",       // named entity spawn - 0x14
			"bisbiiibbb",      // pickup spawn
			"bii",             // collect item
			"bibiii",          // add object/vehicle
			"bibiiibb",        // mob spawn
			"", "", "", "",
			"bi",              // destroy entity - 0x1D
			"bi",              // entity
			"bibbb",           // entity relative move
			"bibb",            // entity look
			"bibbbbb",         // entity look and relative move
			"biiiibb",         // entity teleport
			"", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
			"biib",            // prechunk - 0x32
			"bisibbbix",       // map chunk
			"biisxxx",         // multi block change
			"bibibb",          // block change
			"bisisx"           // complex entities
			// special handling for disconnect
		};
	}
}