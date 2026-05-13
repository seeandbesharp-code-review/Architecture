// src/app/models/worker.model.ts

export enum WorkerPosition {
  Manager = 'Manager',
  Developer = 'Developer',
  Designer = 'Designer',
  Tester = 'Tester',
  Support = 'Support'
}

export enum Mine {
  Women = 'Women',
  Men = 'Men'
}

export interface WorkerDto {
  firstName: string;
  lastName: string;
  dateOfStart: string | Date;
}

export interface WorkerModelDto {
  workerId: number;
  firstName: string;
  lastName: string;
  dateOfStart: string | Date;
  birthday: string | Date;
  position: number; // 0 = Manager, 1 = Developer...
  mine: number; // 0 = Men, 1 = Women
}

