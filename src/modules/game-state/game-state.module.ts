/* eslint-disable prettier/prettier */
import { Module } from '@nestjs/common';
import { GameStateController } from './game-state.controller';
// import { GameStateService } from './game-state.service';

@Module({
  controllers: [GameStateController],
  providers: [],
  exports: [],
})
export class GameStateModule { }
