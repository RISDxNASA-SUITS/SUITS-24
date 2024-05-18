import React from 'react';
import header from '../../assets/header.svg';
import './Window.css';

interface WindowProps {
  children: React.ReactNode;
  title: string;
}

function Window({ children, title }: WindowProps) {
  return (
    <div className='container p-0 relative'>
      <img
        src={header}
        alt='window header'
        className='w-40 absolute -top-10 left-2 opacity-75'
      />
      <div className='absolute w-40 -top-8 left-2 text-center text-white text-base'>
        {title}
      </div>
      <div className='window-cover' />
      <div className='window'>{children}</div>
    </div>
  );
}

export default Window;
