/* eslint-disable prettier/prettier */
import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { GameStateModule } from './modules/game-state/game-state.module';
// import { GatewayTcpModule } from './modules/gateway-tcp/gateway-tcp.module';

@Module({
  imports: [GameStateModule],
  controllers: [
    AppController
  ],
  providers: [
    AppService,
  ],
})

export class AppModule { }
