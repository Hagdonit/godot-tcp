/* eslint-disable prettier/prettier */
import * as os from 'os';
import { Injectable } from '@nestjs/common';
import { createServer, Socket } from 'net';

@Injectable()
export class TcpServerService {
  private server;

  onModuleInit() {
    this.server = createServer((socket: Socket) => {
      console.log('Client connected. with listener count:', this.server.listenerCount('connection'));

      this.processMessage(socket, 'Hello from server!');

      socket.on('data', (data) => {
        try {
          const receivedMessage = data.toString();
          console.log('Received from client:', receivedMessage);

          // Xử lý dữ liệu nhận được từ client và phản hồi lại
          const response = this.processData(receivedMessage);
          socket.write(response);

          // Ghi log thông tin RAM và CPU sau khi xử lý
          this.logSystemUsage();
        } catch (err) {
          console.error('Error processing data:', err.message);
        }
      });

      socket.on('end', () => {
        console.log('Client disconnected.');
        // Hủy cấp phát tài nguyên khi client đóng kết nối
        this.cleanupSocket(socket);
      });

      socket.on('error', ({ code }: NodeJS.ErrnoException) => {
        if (code === 'ECONNRESET') {
          console.log('Connection reset by client.');
          // Xử lý việc client đóng kết nối không đúng cách
        } else {
          console.error('Socket error:', code);
        }
        // Hủy cấp phát tài nguyên khi có lỗi
        this.cleanupSocket(socket);
      });
    });

    this.server.listen(8123, () => {
      console.log('TCP server is listening on port 8123');

    });
  }

  processData(message: string): string {
    return `Server received: ${message}`;
  }

  processMessage(socket: Socket, message: string) {
    try {
      socket.write(message);
    } catch (error) {
      console.error('Error processing message:', error);
    }
  }

  cleanupSocket(socket: Socket) {
    try {
      socket.destroy();  // Hủy bỏ socket để giải phóng tài nguyên
      console.log('Socket destroyed and resources released.');
    } catch (err) {
      console.error('Error while cleaning up socket:', err);
    }
  }

  logSystemUsage() {
    const memoryUsage = process.memoryUsage();
    const freeMemory = os.freemem();
    const totalMemory = os.totalmem();

    console.log(`Memory Usage: RSS = ${memoryUsage.rss}, Heap Used = ${memoryUsage.heapUsed}`);
    console.log(`Free Memory: ${freeMemory}, Total Memory: ${totalMemory}`);
  }

  closeServer() {
    this.server.close(() => {
      console.log('TCP server closed.');
    });
  }

}
