import { useState, useEffect } from 'react';
import Task from '../task/Task';

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const [currTask, setCurrTask] = useState(0);
  const fetchTasks = async () => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/get-tasks`);
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
    <div className='flex flex-col w-full h-[60.5rem] p-5'>
      {currTask != 0 ? (
        <>
          <h1 className='section-title'>EV1 Past Tasks</h1>
        </>
      ) : (
        <></>
      )}
      {currTask <= tasks.length - 1 ? (
        <>
          <h1 className='section-title'>EV1 Current Task</h1>
          <Task info={tasks[currTask]} current={true} />
        </>
      ) : (
        <></>
      )}
      {currTask < tasks.length - 1 ? (
        <>
          <h1 className='section-title'>EV1 Next Tasks</h1>
        </>
      ) : (
        <></>
      )}
    </div>
  );
}

export default Tasks;
