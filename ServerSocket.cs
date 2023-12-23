using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkManager
{
	public class ServerSocket
	{
		private string _interfaceAddress;
		private int _port;
		private string _data = string.Empty;

		public ServerSocket(string interfaceAddress, int port)
		{
			_interfaceAddress = interfaceAddress;
			_port = port;
		}

		public int GetPort() { return _port; }
		public string GetInterfaceAddress() { return _interfaceAddress; }

		public string GetLastData() { return _data; }
		public void StartListener()
		{
			byte[] bytes = new byte[1024];
			IPAddress ipAddress = IPAddress.Parse(_interfaceAddress);

			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);
			Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				listener.Bind(localEndPoint);
				listener.Listen(10);
				Logging.Info($"Server is listening on {ipAddress}:{_port}");
				while (true)
				{
					Socket handler = listener.Accept();
					Logging.Info($"Client connection: {handler.RemoteEndPoint}");

					_data = string.Empty;
					while (true)
					{
						int byteReceive = handler.Receive(bytes);
						_data += Encoding.UTF8.GetString(bytes, 0, byteReceive);
						Logging.Debug($"New bytes receive: {byteReceive}");
						if (_data.IndexOf("<EOF>") > -1)
						{
							break;
						}
					}
					_data = _data.Replace("<EOF>", "");
					Logging.Info($"Received: {_data}");

					string response = "OK";
					Logging.Info($"Response: {response}");

					//Logging.Debug("End of data");

					byte[] bDataToClient = Encoding.UTF8.GetBytes(response);
					handler.Send(bDataToClient);
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex.Message);
			}
		}
	}
}
