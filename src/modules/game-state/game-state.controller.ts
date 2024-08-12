/* eslint-disable prettier/prettier */
import { Controller, Logger } from '@nestjs/common';
import { MessagePattern } from '@nestjs/microservices';

@Controller('game-state')
export class GameStateController {
  private readonly logger = new Logger(GameStateController.name);

  @MessagePattern("resources")
  handleMessage(data: any) {
    this.logger.log('Received data: ' + JSON.stringify(data));

    return { result: data };
  }

}
