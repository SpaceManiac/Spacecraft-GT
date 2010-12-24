using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO.Compression;
using System.IO;
using zlib;
using System.Diagnostics;

namespace SpacecraftGT
{
	public class Connection
	{
		public string IPString;
		
		private Thread _Thread;
		private TcpClient _Client;
		private Queue<byte[]> _TransmitQueue;
		private bool _Running;
		private byte[] _Buffer;
		private Player _Player;
		
		public Connection(TcpClient client, Player player)
		{
			_Client = client;
			IPString = _Client.Client.RemoteEndPoint.ToString();
			
			_Running = true;
			_TransmitQueue = new Queue<byte[]>();
			_Buffer = new byte[0];
			_Player = player;
			
			_Thread = new Thread(ConnectionThread);
			_Thread.Name = "SC-Player " + _Client.GetHashCode();
			_Thread.Start();
		}
		
		public void Stop()
		{
			_Running = false;
		}
		
		#region Network code

		public void Transmit(PacketType type, params object[] args)
		{
			// Spacecraft.Log("Transmitting: " + type + "(" + (byte)type + ")");
			string structure = (type == PacketType.Disconnect ? "bt" : PacketStructure.Data[(byte) type]);
			
			Builder<Byte> packet = new Builder<Byte>();
			packet.Append((byte) type);
			
			byte[] bytes;
			int current = 1;
			try {
				for (int i = 1; i < structure.Length; ++i) {
					current = i;
					switch (structure[i]) {
						case 'b':		// sbyte(1)
							packet.Append((byte) (sbyte) args[i-1]);
							break;
							
						case 's':		// short(2)
							packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) args[i-1])));
							break;
							
						case 'f':		// float(4)
							bytes = BitConverter.GetBytes((float) args[i-1]);
							for (int j = 3; j >= 0; --j) {
								packet.Append(bytes[j]);
							}
							//packet.Append(bytes);
							break;
							
						case 'i':		// int(4)
							packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int) args[i-1])));
							break;
							
						case 'd':		// double(8)
							bytes = BitConverter.GetBytes((double) args[i-1]);
							for (int j = 7; j >= 0; --j) {
								packet.Append(bytes[j]);
							}
							//packet.Append(bytes);
							break;
							
						case 'l':		// long(8)
							packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long) args[i-1])));
							break;
						
						case 't':		// string
							bytes = Encoding.UTF8.GetBytes((string) args[i-1]);
							packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) bytes.Length)));
							packet.Append(bytes);
							break;
						
						case 'x':		// byte array
							packet.Append((byte[]) args[i-1]);
							break;
					}
				}
			}
			catch (InvalidCastException) {
				Spacecraft.Log("Error transmitting " + type + ": expected '" + structure[current] +
					"', got " + args[current - 1].GetType().ToString() + " for argument " + current + " (format: " + structure + ")");
				throw;
			}
			_TransmitQueue.Enqueue(packet.ToArray());
		}
		
		private void ConnectionThread()
		{
			Spacecraft.Log("Connection thread " + _Client.GetHashCode() + " running.");
			
			Stopwatch clock = new Stopwatch();
			clock.Start();
			double lastKeepAlive = 0;
			
			while (_Running) {
				try {
					while (_TransmitQueue.Count > 0) {
						byte[] next = _TransmitQueue.Dequeue();
						TransmitRaw(next);
						if (next.Length > 0 && next[0] == (byte) PacketType.Disconnect) {
							_Client.GetStream().Flush();
							_Client.Close();
						}
					}
					
					if (!_Client.Connected) {
						_Client.Close();
						_Running = false;
						break;
					}
					
					if (_Client.GetStream().DataAvailable) {
						IncomingData();
					}
					
					if (lastKeepAlive + 20 < clock.Elapsed.TotalSeconds) {
						Transmit(PacketType.KeepAlive);
						lastKeepAlive = clock.Elapsed.TotalSeconds;
					}
					
					Thread.Sleep(30);
				}
				catch (Exception) {
					_Running = false;
				}
			}
			if (_Player.Spawned) {
				_Player.Despawn();
			} else {
				Spacecraft.Log("Anonymous connection thread stopped.");
			}
		}
		
		private void TransmitRaw(byte[] packet)
		{
			try {
				_Client.GetStream().Write(packet, 0, packet.Length);
			}
			catch (Exception) {
				_Client.Close();
				Spacecraft.Log("Disconnected on exception");
				_Running = false;
			}
		}
		
		public void Disconnect(string message)
		{
			Transmit(PacketType.Disconnect, message);
		}
		
		private void IncomingData()
		{
			NetworkStream stream = _Client.GetStream();
			Builder<byte> buffer = new Builder<byte>();
			buffer.Append(_Buffer);
			
			while (stream.DataAvailable) {
				buffer.Append((byte) stream.ReadByte());
			}
			
			_Buffer = buffer.ToArray();
			buffer = null;
			
			while (_Buffer.Length > 0) {
				Pair<int, object[]> pair = CheckCompletePacket();
				int length = pair.First;
				if (length > 0) {
					//byte[] packet = new byte[length];
					//Array.Copy(_Buffer, packet, length);
					
					byte[] newBuffer = new byte[_Buffer.Length - length];
					Array.Copy(_Buffer, length, newBuffer, 0, _Buffer.Length - length);
					_Buffer = newBuffer;
					
					ProcessPacket(pair.Second);
				} else {
					break;
				}
			}
		}
		
		private Pair<int, object[]> CheckCompletePacket()
		{
			Pair<int, object[]> nPair = new Pair<int, object[]>(0, null);
			
			PacketType type = (PacketType) _Buffer[0];
			if (_Buffer[0] >= PacketStructure.Data.Length && _Buffer[0] != 0xFF) {
				Spacecraft.Log("Got invalid packet: " + _Buffer[0]);
				return nPair;
			} 
			
			string structure = (type == PacketType.Disconnect ? "bt" : PacketStructure.Data[_Buffer[0]]);
			int bufPos = 0;
			Builder<object> data = new Builder<object>();
			byte[] bytes = new byte[8];
			
			for (int i = 0; i < structure.Length; ++i) {
				switch (structure[i]) {
					case 'b':		// sbyte(1)
						if ((bufPos + 1) > _Buffer.Length) return nPair;
						if (i == 0)
							data.Append((byte) _Buffer[bufPos]);
						else
							data.Append((sbyte) _Buffer[bufPos]);
						bufPos += 1;
						break;
					
					case 's':		// short(2)
						if ((bufPos + 2) > _Buffer.Length) return nPair;
						data.Append((short) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_Buffer, bufPos)));
						bufPos += 2;
						break;
					
					case 'f':		// float(4)
						if ((bufPos + 4) > _Buffer.Length) return nPair;
						for (int j = 0; j < 4; ++j) {
							bytes[j] = _Buffer[bufPos + 3 - j];
						}
						data.Append((float) BitConverter.ToSingle(bytes, 0));
						bufPos += 4;
						break;
					case 'i':		// int(4)
						if ((bufPos + 4) > _Buffer.Length) return nPair;
						data.Append((int) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_Buffer, bufPos)));
						bufPos += 4;
						break;
					
					case 'd':		// double(8)
						if ((bufPos + 8) > _Buffer.Length) return nPair;
						for (int j = 0; j < 8; ++j) {
							bytes[j] = _Buffer[bufPos + 7 - j];
						}
						data.Append((double) BitConverter.ToDouble(bytes, 0));
						bufPos += 8;
						break;
					case 'l':		// long(8)
						if ((bufPos + 8) > _Buffer.Length) return nPair;
						data.Append((long) IPAddress.NetworkToHostOrder(BitConverter.ToInt64(_Buffer, bufPos)));
						bufPos += 8;
						break;
					
					case 't':		// string
						if ((bufPos + 2) > _Buffer.Length) return nPair;
						int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_Buffer, bufPos));
						if ((bufPos + 2 + len) > _Buffer.Length) return nPair;
						data.Append((string) Encoding.UTF8.GetString(_Buffer, bufPos + 2, len));
						bufPos += (2 + len);
						break;
					
					case 'x':		// onos!
						// TODO
						return nPair;
						//break;
				}
			}
			
			return new Pair<int, object[]>(bufPos, data.ToArray());
		}
		
		public void SendChunk(Chunk chunk)
		{
			Transmit(PacketType.PreChunk, chunk.ChunkX, chunk.ChunkZ, (sbyte) 1);
			
			byte[] uncompressed = chunk.GetBytes();
			MemoryStream mem = new MemoryStream();
			ZOutputStream stream = new ZOutputStream(mem, zlibConst.Z_BEST_COMPRESSION);
			stream.Write(uncompressed, 0, uncompressed.Length);
			stream.Close();
			byte[] data = mem.ToArray();
			
			Transmit(PacketType.MapChunk, 16 * chunk.ChunkX, (short) 0, 16 * chunk.ChunkZ,
				(sbyte) 15, (sbyte) 127, (sbyte) 15, data.Length, data);
		}
		
		#endregion
		
		private void ProcessPacket(object[] packet)
		{
			PacketType type = (PacketType) (byte) packet[0];
			
			// Spacecraft.Log("Packet received: " + type);
			
			switch(type) {
				case PacketType.Handshake: {
					_Player.Username = (string) packet[1];
					Transmit(PacketType.Handshake, Spacecraft.Server.ServerHash);
					break;
				}
				case PacketType.LoginDetails: {
					int protocol = (int) packet[1];
					if (protocol != Spacecraft.ProtocolVersion) {
						Spacecraft.Log("Expecting protocol v" + Spacecraft.ProtocolVersion + ", got v" + (int) packet[1]);
						if (protocol > Spacecraft.ProtocolVersion) {
							Disconnect("Outdated server!");
						} else {
							Disconnect("Outdated client!");
						}
						break;
					}
					if ((string) packet[2] != _Player.Username) {
						Disconnect("Sent invalid username");
						break;
					}
					
					// TODO: Implement name verification
					
					Transmit(PacketType.LoginDetails, _Player.EntityID,
						Spacecraft.Server.Name, Spacecraft.Server.Motd,
						/* World.Seed */ (long) 0, /* World.Dimension */ (sbyte) 0);
					_Player.Spawn();
					
					break;
				}
				
				case PacketType.Message: {
					_Player.RecvMessage((string) packet[1]);
					break;
				}
				
				case PacketType.Player: {
					// Ignore.
					break;
				}
				case PacketType.PlayerPosition: {
					_Player.X = (double) packet[1];
					_Player.Y = (double) packet[2];
					//
					_Player.Z = (double) packet[4];
					//
					break;
				}
				case PacketType.PlayerLook: {
					// TODO: Handle PlayerLook
					float yaw = (float) packet[1], pitch = (float) packet[2];
					_Player.Yaw = (sbyte) (yaw * 256 / 360);
					_Player.Pitch = (sbyte) (pitch * 256 / 360);
					break;
				}
				case PacketType.PlayerPositionLook: {
					// TODO: Handle PlayerPositionLook
					_Player.X = (double) packet[1];
					_Player.Y = (double) packet[2];
					//
					_Player.Z = (double) packet[4];
					float yaw = (float) packet[5], pitch = (float) packet[6];
					_Player.Yaw = (sbyte) (yaw * 256 / 360);
					_Player.Pitch = (sbyte) (pitch * 256 / 360);
					break;
				}
				
				case PacketType.Disconnect: {
					Disconnect("Quitting");
					break;
				}
				
				default: {
					Spacecraft.Log("[Packet] " + _Player.Username + " sent " + type);
					break;
				}
			}
		}
	}
}
