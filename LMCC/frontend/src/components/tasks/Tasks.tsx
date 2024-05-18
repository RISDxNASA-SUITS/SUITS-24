import Task from '../task/Task';

function Tasks() {
  return (
    <div className='flex flex-col w-full h-[59.5rem] p-5'>
      <h1 className='section-title'>EV1 Current Task</h1>
      <Task title='Egress' location='UIA Panel' current={true} />
      <h1 className='section-title'>EV1 Next Tasks</h1>
      <Task title='Navigation' location='Comms Tower' />
      <Task title='Repair' location='Comms Tower' />
      <Task title='Inspect Comms Tower Worksite' location='Comms Tower' />
    </div>
  );
}

export default Tasks;
