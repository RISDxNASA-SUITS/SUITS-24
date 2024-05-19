export interface Step {
  name: string;
  status: string;
}

export interface Instruction {
  name: string;
  steps: Step[];
}

export interface TaskInfo {
  name: string;
  location: string;
  instructions: Instruction[];
}
