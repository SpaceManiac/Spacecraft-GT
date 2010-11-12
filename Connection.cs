using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Net;

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
		
		private void Transmit(byte[] packet)
		{
			_TransmitQueue.Enqueue(packet);
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
				int length = CheckCompletePacket();
				if (length > 0) {
					byte[] packet = new byte[length];
					Array.Copy(_Buffer, packet, length);
					
					byte[] newBuffer = new byte[_Buffer.Length - length];
					Array.Copy(_Buffer, length, newBuffer, 0, _Buffer.Length - length);
					_Buffer = newBuffer;
					
					ProcessPacket(packet);
				} else {
					break;
				}
			}
		}
		
		private int CheckCompletePacket()
		{
			PacketType type = (PacketType) _Buffer[0];
			string structure = PacketStructure.Data[_Buffer[0]];
			int bufPos = 0;
			
			Spacecraft.Log("Checking for packet completeness");
			for (int i = 0; i < structure.Length; ++i) {
				switch (structure[i]) {
					case 'b':		// byte(1)
						if ((bufPos += 1) > _Buffer.Length) return 0;
						break;
					
					case 's':		// short(2)
						if ((bufPos += 2) > _Buffer.Length) return 0;
						break;
					
					case 'f':		// float(4)
					case 'i':		// int(4)
						if ((bufPos += 4) > _Buffer.Length) return 0;
						break;
					
					case 'd':		// double(8)
					case 'l':		// long(8)
						if ((bufPos += 8) > _Buffer.Length) return 0;
						break;
					
					case 't':		// string
						if ((bufPos + 2) > _Buffer.Length) return 0;
						int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_Buffer, bufPos));
						if ((bufPos += (2 + len)) > _Buffer.Length) return 0;
						break;
					
					case 'x':		// onos!
						// TODO
						return 0;
						//break;
				}
			}
			
			return bufPos;
		}
		
		private void ProcessPacket(byte[] packet)
		{
			// By this point, we know the packet is complete. It's safe to read everything
			// TODO
		}
	}
}
