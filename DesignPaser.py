# Interprets PacketDesign.txt, and spits out packet classes to Packets.cs
from __future__ import print_function
import sys, time, collections
sys.stdout = open('Packets.cs', 'w');

ReadFile = open("PacketDesign.txt")
Lines = ReadFile.readlines()

def debug(*a):
	for i in a:
		print(i, sep="", file=sys.__stdout__)
			
packets = collections.OrderedDict()

currentName = ""

for L in Lines:
	if L.find("0x") != -1:
		name = L.split("(")[0]
		number = L.split("(")[1]
		name = name.replace(" ", "")
		number = number.replace(")", "")
		number = int(number, 16)
		debug(name, number)
		currentName = name
		packets[name] =  collections.OrderedDict()
		packets
		packets[name]["ID"] = number
	else:
		if L.find("\t") == -1:
			continue
		L = L[1:]
		field = L.split("\t")[0].strip()
		type = L.split("\t")[1].strip()
		packets[currentName][field.replace(" ", "")] = type.replace(" ", "")
		
debug(packets)

print ('''// This file is auto-generated by DesignPaser.py. Manual edits will be lost on regeneration.
// Last generated on ''' + time.ctime(time.time()) + "\n" + '''using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SpacecraftGT
{
    public abstract partial class Packet
    {
        public abstract byte[] Save();
        public static Packet Load(byte[] data)
        {
			Packet Output = null;
			int BufferPos = 0;
			switch (data[0]) {''')

for Packet in packets:
	print("\t\t\t\tcase ", hex(packets[Packet]["ID"]),  ": { //", Packet, sep="") # Print the case ID: statement, in hex for readability. 
	print("\t\t\t\t\t", "var Packet = Output as " + Packet + "Packet;", sep="") # Get a reference to the specific type, rather than the general.
	print("\t\t\t\t\t", "Packet = new " + Packet + "Packet();", sep="") # Declare using New. 
	
	for property in packets[Packet]:
		Type = packets[Packet][property]
		if property == "ID":
			Type = "byte"
		Type = Type.replace("[]", " Array")
		Type = Type.title()
		Type = Type.replace(" ", "");
		print("\t\t\t\t\tPacket." + property + " = GetNext", Type, "(data, ref BufferPos);", sep="")
		#print("\t\t\t\t\tBufferPos +=", Lengths[Type], ";", sep="")
		
	print("\t\t\t\t\tbreak; }\n")

	
print('''			}
			return Output;
		}
	}''')	
	
	
for Packet in packets:
	print("\tpublic class " + Packet + "Packet : Packet {", sep="")
	for item in packets[Packet]:
		if (item == "ID"):
			print("\t\t", "public byte ID = ", hex(packets[Packet][item]), ";", sep="")
		else:
			print("\t\tpublic ", packets[Packet][item], " ", item, ";", sep="")
			
	print("\n\t\tpublic override byte[] Save() { ")
	print("\t\t\tBuilder<Byte> Build = new Builder<byte>();")
	for item in packets[Packet]:
		if packets[Packet][item] in ["int", "short", "long"]:
			print("\t\t\tBuild.Append( IPAddress.HostToNetworkOrder(", item + " ).ToBytes()", ");")
		else:
			print("\t\t\tBuild.Append( ", item + ".ToBytes()", ");")
	print("\t\t\treturn Build.ToArray();\n\t\t}")
			
	print("\t}")
	print("")
	
print("}")
	
	