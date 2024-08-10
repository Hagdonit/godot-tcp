/* eslint-disable prettier/prettier */
import { TcpServerService } from './tcp-server.service';
import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';

@Module({
  imports: [],
  controllers: [
    AppController
  ],
  providers: [
    AppService,
    TcpServerService,
  ],
})

export class AppModule { }
