const WebSocket = require('ws');

// Tạo WebSocket server chạy trên cổng 8080
const wss = new WebSocket.Server({ port: 8080, host: '0.0.0.0' });

wss.on('connection', (ws) => {
  console.log('Client connected');

  // Gửi thông báo chào mừng khi client kết nối
  ws.send('Welcome to WebSocket server');

  // Xử lý khi nhận được tin nhắn từ client
  ws.on('message', (message) => {
    console.log(`Received: ${message}`);

    // Gửi lại thông điệp đã nhận từ client
    ws.send(`Server received: ${message}`);
  });

  // Xử lý khi client ngắt kết nối
  ws.on('close', () => {
    console.log('Client disconnected');
    ws.send(`Say goodbye to client`);

  });

  // Xử lý lỗi
  ws.on('error', (error) => {
    console.error('WebSocket error:', error);
  });
});

console.log('WebSocket server is running on ws://localhost:8080');
