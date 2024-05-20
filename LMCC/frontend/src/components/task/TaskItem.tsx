import { Instruction } from './TaskTypes';
import Inprogress from '../../assets/symbols/inprogress_symbol.png';
import Complete from '../../assets/symbols/complete_symbol.png';
import './TaskItem.css';

interface TaskItemProps {
  instruction: Instruction;
}

function TaskItem({ instruction }: TaskItemProps) {
  return (
    <div className='flex flex-col pb-5'>
      <h2 className='text-lg pb-2'>{instruction.name}</h2>
      {instruction.steps.map((step, index) => (
        <div key={`${step.name}-${index}`} className='flex items-center py-2'>
          <div className='w-10'>
            {step.status == 'complete' || step.status == 'inprogress' ? (
              <img
                src={step.status == 'complete' ? Complete : Inprogress}
                className={`h-auto${
                  step.status == 'inprogress' ? ' spinning' : ''
                }`}
              />
            ) : (
              <></>
            )}
          </div>
          <span className='relative'>{step.name}</span>
        </div>
      ))}
    </div>
  );
}

export default TaskItem;
