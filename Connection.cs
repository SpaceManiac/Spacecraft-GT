using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
		
		public Connection(TcpClient client)
		{
			_Client = client;
			IPString = _Client.Client.RemoteEndPoint.ToString();
			
			_Thread = new Thread(ConnectionThread);
			_Thread.Name = "SC-GT Player " + _Client.GetHashCode();
			_Thread.Start();
			
			_TransmitQueue = new Queue<byte[]>();
			_Running = true;
			_Buffer = new byte[0];
		}
		
		public void Stop()
		{
			_Running = false;
		}
		
		public void Transmit(PacketType type, params object[] args)
		{
			Builder<Byte> packet = new Builder<Byte>();
			string structure = PacketStructure.Data[_Buffer[0]];
			byte[] bytes;
			for (int i = 0; i < structure.Length; ++i) {
				switch (structure[i]) {
					case 'b':		// byte(1)
						packet.Append((byte) args[i-1]);
						break;
						
					case 's':		// short(2)
						packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) args[i-1])));
						break;
						
					case 'f':		// float(4)
						bytes = BitConverter.GetBytes((float) args[i-1]);
						//for (int j = 3; j >= 0; --j) {
						//	packet.Append(bytes[j]);
						//}
						packet.Append(bytes);
						break;
						
					case 'i':		// int(4)
						packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int) args[i-1])));
						break;
						
					case 'd':		// double(8)
						bytes = BitConverter.GetBytes((double) args[i-1]);
						//for (int j = 7; j >= 0; --j) {
						//	packet.Append(bytes[j]);
						//}
						packet.Append(bytes);
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
			_TransmitQueue.Enqueue(packet.ToArray());
		}
		
		private void ConnectionThread()
		{
			Spacecraft.Log("Connection thread " + _Client.GetHashCode() + " running.");
			while (_Running) {
				while (_TransmitQueue.Count > 0) {
					TransmitRaw(_TransmitQueue.Dequeue());
				}
				
				if (_Client.GetStream().DataAvailable) {
					IncomingData();
				}
				
				Thread.Sleep(10);
			}
		}
		
		private void TransmitRaw(byte[] packet)
		{
			_Client.GetStream().Write(packet, 0, packet.Length);
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
			PacketType type = (PacketType) _Buffer[0];
			string structure = PacketStructure.Data[_Buffer[0]];
			int bufPos = 0;
			
			Pair<int, object[]> nPair = new Pair<int, object[]>(0, null);
			Builder<object> data = new Builder<object>();
			
			for (int i = 0; i < structure.Length; ++i) {
				switch (structure[i]) {
					case 'b':		// byte(1)
						if ((bufPos + 1) > _Buffer.Length) return nPair;
						data.Append((byte) _Buffer[bufPos]);
						bufPos += 1;
						break;
					
					case 's':		// short(2)
						if ((bufPos + 2) > _Buffer.Length) return nPair;
						data.Append((short) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_Buffer, bufPos)));
						bufPos += 2;
						break;
					
					case 'f':		// float(4)
						if ((bufPos + 4) > _Buffer.Length) return nPair;
						data.Append((float) BitConverter.ToSingle(_Buffer, bufPos));
						bufPos += 4;
						break;
					case 'i':		// int(4)
						if ((bufPos + 4) > _Buffer.Length) return nPair;
						data.Append((int) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_Buffer, bufPos)));
						bufPos += 4;
						break;
					
					case 'd':		// double(8)
						if ((bufPos + 8) > _Buffer.Length) return nPair;
						data.Append((double) BitConverter.ToDouble(_Buffer, bufPos));
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
		
		private void ProcessPacket(object[] packet)
		{
			PacketType type = (PacketType) (byte) packet[0];
			Spacecraft.Log("Packet (" + type + ") length = " + packet.Length + " [0] = " + (string)packet[1]);
		}
	}
}
