using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Godotwebsocket;

[Tool]
public partial class GameManager : Node
{
	private ClientWebSocket _webSocket = new();
	private CancellationTokenSource _cancellationTokenSource = new();
	
	[Export] public TextEdit ChatInput;
	[Export] public Button SendButton;
	[Export] public Button ConnectButton;
	[Export] public Button DisconnectButton;
	[Export] public RichTextLabel ChatOutput;
	private readonly List<string> _chatMessages = new();

	public override void _Ready()
	{
		if (ConnectButton != null)
			ConnectButton.Pressed += OnEventConnectWebSocket;
		
		if (SendButton != null)
			SendButton.Pressed += OnSendButtonPressed;
		
	}

	public override void _ExitTree()
	{
		_cancellationTokenSource.Cancel();
		_webSocket?.Dispose();
	}
	
	private void RenderMessages()
	{
		ChatOutput.Text = "";
		
		foreach (var message in _chatMessages)
		{
			ChatOutput.Text += $"{message}\n";
		}
	}
	
	private async void ConnectToWebSocket()
	{
		try
		{
			_webSocket = new ClientWebSocket();
			if (_webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
			{
				GD.Print("WebSocket is already connected or in the process of connecting.");
				return;
			}
			// Khởi tạo WebSocket client
			GD.Print("Connecting to WebSocket server...");
			await _webSocket.ConnectAsync(new Uri("ws://localhost:8080"), _cancellationTokenSource.Token);
			GD.Print("Connected to WebSocket server");

			// Đọc tin nhắn từ server
			await ReceiveMessages();
		}
		catch (Exception e)
		{
			GD.PrintErr($"WebSocket connection failed: {e.Message}");
		}
	}
	
	private void OnEventConnectWebSocket()
	{
		if (_webSocket.State == WebSocketState.Open)
		{
			GD.Print("WebSocket is already connected");
			Task.Run(CloseConnection);
			_chatMessages.Add("Say goodbye...");
			RenderMessages();
			return;
		}
		
		// Kết nối tới WebSocket server
		ConnectToWebSocket();
	}
	
	private async Task CloseConnection()
	{
		if (_webSocket.State != WebSocketState.Open) return;
		
		await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the client", CancellationToken.None);
		
		return;
	}

	private void OnSendButtonPressed()
	{
		if (ChatInput.Text.Length <= 0) return;
		var text = ChatInput.Text;
		
		SendMessage(text);
		ChatInput.Text = "";
		text = "*Me*: " + text;
		_chatMessages.Add(text);
	}
	
	private async Task ReceiveMessages()
	{
		var buffer = new byte[1024];

		try
		{
			while (_webSocket.State is WebSocketState.Open)
			{
				var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

				switch (result.MessageType)
				{
					case WebSocketMessageType.Text:
					{
						var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
						GD.Print($"Message received: {message}");
					
						_chatMessages.Add(message);
						RenderMessages();
						break;
					}
					// case WebSocketMessageType.Close:
					// 	await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the client", CancellationToken.None);
					// 	GD.Print("WebSocket connection closed");
					// 	break;
					case WebSocketMessageType.Binary:
					default:
						break;
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"WebSocket receive error: {e.Message}");
		}
	}

	public async void SendMessage(string message)
	{
		if (_webSocket.State != WebSocketState.Open) return;
		
		GD.Print($"Sending message: {message}");
		var messageBuffer = Encoding.UTF8.GetBytes(message);
		await _webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
		RenderMessages();
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
