using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace NBT
{
	public enum TagType : byte
	{
		End = 0,
		Byte = 1,
		Short = 2, 
		Int = 3,
		Long = 4,
		Float = 5,
		Double = 6,
		ByteArray = 7,
		String = 8,
		List = 9,
		Compound = 10,
	}

	public class BinaryTag
	{
		public TagType _ListType;
		public TagType Type;
		public string Name;
		public object Payload;

		public BinaryTag(TagType T)
		{
			this.Type = T;
			this.Name = "";
			this.Payload = null;
		}

		public BinaryTag(TagType T, object payload)
		{
			this.Type = T;
			this.Name = "";
			this.Payload = payload;
		}

        public BinaryTag this[string Key]
        {
            get {
                if (Type == TagType.Compound) {
                    foreach (BinaryTag Item in (BinaryTag[])this.Payload)
                        if (Item.Name == Key)
                            return Item;
                    return null;
                } else {
                    return null;
                }
            }
        }

        public BinaryTag this[int Index]
        {
            get {
                if (Type == TagType.List) {
                    return (BinaryTag)((BinaryTag[])Payload)[Index];
                } else {
                    return null;
                }
            }
        }
		
		private string NameString (string name)
		{
			if (name == "") {
				return "";
			} else {
				return "(\"" + name + "\")";
			}
		}
		
		private string CompoundToString (string type, string indent)
		{
			BinaryTag[] List = (BinaryTag[]) Payload;
			if (Payload == null) {
				List = new BinaryTag[0];
			}
			StringBuilder Builder = new StringBuilder();
			Builder.Append("TAG_").Append(type).Append(NameString(Name)).Append(": ");
			Builder.Append(List.Length).Append(" entries\n");
			Builder.Append(indent).Append("{\n");
			foreach(BinaryTag Tag in List) {
				Builder.Append(indent).Append("  ");
				if(Tag.Type == TagType.Compound) {
					Builder.Append(Tag.CompoundToString("Compound", indent + "  "));
				} else if(Tag.Type == TagType.List) {
					Builder.Append(Tag.CompoundToString("List", indent + "  "));
				} else {
					Builder.Append(Tag);
				}
				Builder.Append("\n");
			}
			Builder.Append(indent).Append("}");
			return Builder.ToString();
		}

		public override string ToString ()
		{
			switch(Type) {
				case TagType.End:
					return "TAG_End";
				case TagType.Byte:
					return "TAG_Byte" + NameString(Name) + ": " + (byte) Payload;
				case TagType.Short:
					return "TAG_Short" + NameString(Name) + ": " + (short) Payload;
				case TagType.Int:
					return "TAG_Int" + NameString(Name) + ": " + (int) Payload;
				case TagType.Long:
					return "TAG_Long" + NameString(Name) + ": " + (long) Payload;
				case TagType.Float:
					return "TAG_Float" + NameString(Name) + ": " + (float) Payload;
				case TagType.Double:
					return "TAG_Double" + NameString(Name) + ": " + (double) Payload;
				case TagType.ByteArray:
					return "TAG_ByteArray" + NameString(Name) + ": [" + ((byte[]) Payload).Length + " bytes]";
				case TagType.String:
					return "TAG_String" + NameString(Name) + ": " + (string) Payload;
				case TagType.List:
					return CompoundToString("List", "");
				case TagType.Compound:
					return CompoundToString("Compound", "");
				default:
					return "TAG_ERROR";
			}
		}
	}
	
	public static class NbtParser
	{
		private static short ntoh(short x) {
			return IPAddress.NetworkToHostOrder(x);
		}
		private static int ntoh(int x) {
			return IPAddress.NetworkToHostOrder(x);
		}
		private static long ntoh(long x) {
			return IPAddress.NetworkToHostOrder(x);
		}

		public static BinaryTag ParseTagStream(Stream ByteStream)
		{
			return ReadNamedTag(ByteStream);
		}
		
		private static BinaryTag ReadNamedTag(Stream ByteStream)
		{
			TagType Type = (TagType) ByteStream.ReadByte();
			string Name = "";
			if (Type != TagType.End) {
				Name = (string) ReadTag(ByteStream, TagType.String).Payload;
			}
			BinaryTag Tag = ReadTag(ByteStream, Type);
			Tag.Name = Name;
			return Tag;
		}
		
		private static BinaryTag ReadTag(Stream ByteStream, TagType Type)
		{
			BinaryTag Tag = new BinaryTag(Type);
			byte[] buffer;
			
			switch(Type) {
				case TagType.End:
					Tag.Payload = null;
					return Tag;
				
				case TagType.Byte:
					Tag.Payload = (byte) ByteStream.ReadByte();
					return Tag;
				
				case TagType.Short:
					buffer = new byte[2];
					ByteStream.Read(buffer, 0, 2);
					Tag.Payload = ntoh(BitConverter.ToInt16(buffer, 0));
					return Tag;
				
				case TagType.Int:
					buffer = new byte[4];
					ByteStream.Read(buffer, 0, 4);
					Tag.Payload = ntoh(BitConverter.ToInt32(buffer, 0));
					return Tag;
				
				case TagType.Long:
					buffer = new byte[8];
					ByteStream.Read(buffer, 0, 8);
					Tag.Payload = ntoh(BitConverter.ToInt64(buffer, 0));
					return Tag;
				
				case TagType.Float:
					buffer = new byte[4];
					ByteStream.Read(buffer, 0, 4);
					buffer = BitConverter.GetBytes(ntoh(BitConverter.ToInt32(buffer, 0)));
					Tag.Payload = BitConverter.ToSingle(buffer, 0);
					return Tag;
				
				case TagType.Double:
					buffer = new byte[8];
					ByteStream.Read(buffer, 0, 8);
					buffer = BitConverter.GetBytes(ntoh(BitConverter.ToInt64(buffer, 0)));
					Tag.Payload = BitConverter.ToDouble(buffer, 0);
					return Tag;
				
				case TagType.ByteArray: {
					int length = (int) ReadTag(ByteStream, TagType.Int).Payload;
					buffer = new byte[length];
					ByteStream.Read(buffer, 0, length);
					Tag.Payload = buffer;
					return Tag;
				}
				
				case TagType.String: {
					short length = (short) ReadTag(ByteStream, TagType.Short).Payload;
					buffer = new byte[length];
					ByteStream.Read(buffer, 0, length);
					Tag.Payload = UTF8Encoding.UTF8.GetString(buffer);
					return Tag;
				}
				
				case TagType.List: {
					TagType type = (TagType) (byte) ReadTag(ByteStream, TagType.Byte).Payload;
					int length = (int) ReadTag(ByteStream, TagType.Int).Payload;
					BinaryTag[] list = new BinaryTag[length];
					Tag._ListType = type;
					for (int i = 0; i < length; ++i) {
						list[i] = ReadTag(ByteStream, type);
					}
					Tag.Payload = list;
					return Tag;
				}
				
				case TagType.Compound:
					List<BinaryTag> tags = new List<BinaryTag>();
					while (true) {
						BinaryTag tag = ReadNamedTag(ByteStream);
						if (tag.Type == TagType.End) {
							break;
						}
						tags.Add(tag);
					}
					Tag.Payload = tags.ToArray();
					return Tag;
				
				default:
					throw new IOException("Explosion in NBT reader.");
			}
	   	}
	}
	
	public static class NbtWriter
	{
		private static short hton(short x) {
			return IPAddress.HostToNetworkOrder(x);
		}
		private static int hton(int x) {
			return IPAddress.HostToNetworkOrder(x);
		}
		private static long hton(long x) {
			return IPAddress.HostToNetworkOrder(x);
		}

		public static void WriteTagStream(BinaryTag tag, Stream ByteStream)
		{
			WriteNamedTag(tag, ByteStream);
		}
		
		private static void WriteNamedTag(BinaryTag Tag, Stream ByteStream)
		{
			ByteStream.WriteByte((byte) Tag.Type);
			if (Tag.Type != TagType.End) {
				WriteTag(new BinaryTag(TagType.String, Tag.Name), ByteStream);
			}
			WriteTag(Tag, ByteStream);
		}
		
		private static void WriteTag(BinaryTag Tag, Stream ByteStream)
		{
			byte[] buffer;
			
			switch(Tag.Type) {
				case TagType.End:
					break;
				
				case TagType.Byte:
					ByteStream.WriteByte((byte) Tag.Payload);
					break;
				
				case TagType.Short:
					buffer = BitConverter.GetBytes(hton((short) Tag.Payload));
					ByteStream.Write(buffer, 0, 2);
					break;
				
				case TagType.Int:
					buffer = BitConverter.GetBytes(hton((int) Tag.Payload));
					ByteStream.Write(buffer, 0, 4);
					break;
				
				case TagType.Long:
					buffer = BitConverter.GetBytes(hton((long) Tag.Payload));
					ByteStream.Write(buffer, 0, 8);
					break;
				
				case TagType.Float:
					buffer = BitConverter.GetBytes((float) Tag.Payload);
					buffer = BitConverter.GetBytes(hton(BitConverter.ToInt32(buffer, 0)));
					ByteStream.Write(buffer, 0, 4);
					break;
				
				case TagType.Double:
					buffer = BitConverter.GetBytes((double) Tag.Payload);
					buffer = BitConverter.GetBytes(hton(BitConverter.ToInt64(buffer, 0)));
					ByteStream.Write(buffer, 0, 8);
					break;
				
				case TagType.ByteArray: {
					byte[] array = (byte[]) Tag.Payload;
					WriteTag(new BinaryTag(TagType.Int, array.Length), ByteStream);
					ByteStream.Write(array, 0, array.Length);
					break;
				}
				
				case TagType.String: {
					byte[] data = UTF8Encoding.UTF8.GetBytes((string) Tag.Payload);
					WriteTag(new BinaryTag(TagType.Short, (short) data.Length), ByteStream);
					ByteStream.Write(data, 0, data.Length);
					break; 
				}
				
				case TagType.List: {
					BinaryTag[] list = (BinaryTag[]) Tag.Payload;
					if (list.Length > 0) {
						Tag._ListType = list[0].Type;
					}
					WriteTag(new BinaryTag(TagType.Byte, (byte) Tag._ListType), ByteStream);
					WriteTag(new BinaryTag(TagType.Int, (int) list.Length), ByteStream);
					for (int i = 0; i < list.Length; ++i) {
						WriteTag(list[i], ByteStream);
					}
					break;
				}
				
				case TagType.Compound: {
					BinaryTag[] list = (BinaryTag[]) Tag.Payload;
					for (int i = 0; i < list.Length; ++i) {
						WriteNamedTag(list[i], ByteStream);
					}
					WriteNamedTag(new BinaryTag(TagType.End), ByteStream);
					break;
				}
				
				default:
					throw new IOException("Explosion in NBT serializer.");
			}
	   	}
	}
}
