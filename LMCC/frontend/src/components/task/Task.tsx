import { useState } from 'react';
import BuildingIcon from '../../assets/icons/building.png';
import UpArrowIcon from '../../assets/icons/up.png';
import DownArrowIcon from '../../assets/icons/down.png';
import './Task.css';

interface TaskProps {
  title: string;
  location: string;
  current: boolean;
}

function Task({ title, location, current }: TaskProps) {
  const [expanded, setExpended] = useState(false);

  return (
    <div
      className={`task-card${current ? ' current' : ''}${
        expanded ? ' expanded' : ''
      }`}
    >
      <h1 className='text-xl'>{title}</h1>
      <span className='text-base'>
        <img src={BuildingIcon} alt='Location' className='inline-block pr-1' />
        {location}
      </span>
      <img
        className='expand-icon'
        src={expanded ? UpArrowIcon : DownArrowIcon}
        onClick={() => setExpended(!expanded)}
      />
    </div>
  );
}

Task.defaultProps = {
  expanded: false,
  current: false,
};

export default Task;
