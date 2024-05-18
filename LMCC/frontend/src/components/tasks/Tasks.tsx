import { useState, useEffect } from 'react';
import Task from '../task/Task';

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const fetchTasks = async () => {
    try {
      const res = await fetch('http://localhost:5000/get-tasks');
      const data = await res.json();
      console.log(data);
      setTasks(data);
    } catch (err) {
      console.log('Failed to fetch tasks:', err);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, []);

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
