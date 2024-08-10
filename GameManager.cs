using System;
using System.Collections.Generic;
using Godot;

namespace GodotTCP;

[Tool]
public partial class GameManager : Node
{
	
	[Export] public TextEdit ChatInput;
	[Export] public Button SendButton;
	[Export] public Button ConnectButton;
	[Export] public Button DisconnectButton;
	[Export] public RichTextLabel ChatOutput;
	private readonly List<string> _chatMessages = new();

	private StreamPeerTcp _tcpClient = new();
	private bool _connected = false;
	private string _deviceId;

	public override void _Ready()
	{
		_deviceId = OS.GetUniqueId();
		ConnectToServer("localhost", 8123);
	}
	
	public override void _Process(double delta)
	{
		if (!_connected || _tcpClient.GetAvailableBytes() <= 0) return;
		var dataBytes = _tcpClient.GetAvailableBytes();
		var data = _tcpClient.GetData(dataBytes);
		var message = data.ToString();
		// string message = System.Text.Encoding.UTF8.GetString(data.);
		GD.Print("Received from server: " + message);
	}

	public override void _ExitTree()
	{
		if (_connected)
		{
			try
			{
				_tcpClient.DisconnectFromHost();
				GD.Print("Disconnected from server.");
			}
			catch (Exception ex)
			{
				GD.Print("Exception during disconnect: " + ex.Message);
			}
			finally
			{
				_connected = false;
			}
		}

	}


	public void ConnectToServer(string ipAddress, int port)
	{
		var err = _tcpClient.ConnectToHost(ipAddress, port);
		
		GD.Print("Connecting to server... " + ipAddress + ":" + port, ",  Error:", err.ToString());
		
		if (err == Error.Ok)
		{
			GD.Print("Connected to server.");
			_connected = true;
		}
		else
		{
			GD.Print("Failed to connect: " + err);
		}
	}

	public void SendMessage(string message)
	{
		if (_connected)
		{
			byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
			_tcpClient.PutData(data);
			GD.Print("Sent to server: " + message);
		}
		else
		{
			GD.Print("Not connected to the server.");
		}
	}

	public override string[] _GetConfigurationWarnings()
	{
		if (ChatInput == null)
			return new[] { "ChatInput is not assigned" };
		if (SendButton == null)
			return new[] { "SendButton is not assigned" };
		if (ConnectButton == null)
			return new[] { "ConnectButton is not assigned" };
		if (DisconnectButton == null)
			return new[] { "DisconnectButton is not assigned" };
		if (ChatOutput == null)
			return new[] { "ChatOutput is not assigned" };
		return base._GetConfigurationWarnings();
	}
}
