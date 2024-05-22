import { useState } from 'react';
import BuildingIcon from '../../assets/icons/building.png';
import UpArrowIcon from '../../assets/icons/up.png';
import DownArrowIcon from '../../assets/icons/down.png';
import { TaskInfo } from './TaskTypes';
import TaskItem from './TaskItem';
import './Task.css';

interface TaskProps {
  info: TaskInfo;
  current: boolean;
}

function Task({ info, current }: TaskProps) {
  const [expanded, setExpended] = useState(false);

  return (
    <div
      className={`task-card${current ? ' current' : ''}${
        expanded ? ' expanded' : ''
      }`}
    >
      <h1 className='text-xl'>{info.name}</h1>
      <span className='text-base'>
        <img src={BuildingIcon} alt='Location' className='inline-block pr-1' />
        {info.location}
      </span>
      <img
        className='expand-icon'
        src={expanded ? UpArrowIcon : DownArrowIcon}
        onClick={() => setExpended(!expanded)}
      />
      {expanded ? (
        <div className='pt-3'>
          {info.instructions?.map((ins, index) => (
            <TaskItem instruction={ins} key={`${ins.name}-${index}`} />
          ))}
        </div>
      ) : (
        <></>
      )}
    </div>
  );
}

Task.defaultProps = {
  expanded: false,
  current: false,
};

export default Task;
