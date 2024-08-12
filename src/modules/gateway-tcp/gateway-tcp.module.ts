/* eslint-disable prettier/prettier */
import { Module } from '@nestjs/common';
import { GatewayTcpService } from './gateway-tcp.service';

@Module({
  providers: [
    GatewayTcpService,
  ],
  exports: [
    GatewayTcpService,
  ],
})
export class GatewayTcpModule { }
