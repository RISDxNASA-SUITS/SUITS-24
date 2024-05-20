import { useState, useEffect } from 'react';
import Task from '../task/Task';

function Tasks() {
  const [tasks, setTasks] = useState([]);
  // const [currTask, setCurrTask] = useState(1);
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
    const interval = setInterval(fetchTasks, 1000);
    return () => {
      clearInterval(interval);
    };
  }, []);

  return (
    <div className='flex flex-col w-full h-[60.5rem] p-5'>
      <h1 className='section-title'>EV1 Tasks</h1>
      {tasks.map((task, index) => (
        <Task info={task} key={`next-task-${index}`} />
      ))}
      {/* {currTask <= tasks.length - 1 ? (
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
          {tasks.slice(currTask + 1, tasks.length).map((task, index) => (
            <Task info={task} key={`next-task-${index}`} />
          ))}
        </>
      ) : (
        <></>
      )}
      {currTask != 0 ? (
        <>
          <h1 className='section-title'>EV1 Past Tasks</h1>
          {tasks.slice(0, currTask).map((task, index) => (
            <Task info={task} key={`next-task-${index}`} />
          ))}
        </>
      ) : (
        <></>
      )} */}
    </div>
  );
}

export default Tasks;
