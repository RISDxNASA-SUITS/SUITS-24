import Window from '../components/window/Window';
import Notifications from '../components/notifications/Notifications';
import Tasks from '../components/tasks/Tasks';
import { useState } from 'react';

enum CamMode {
  NORMAL,
  THERMAL,
}

function Monitor2() {
  const [camMode, setCamMode] = useState<CamMode>(CamMode.NORMAL);

  return (
    <div className='w-screen h-screen m-0'>
      <div className='flex w-full h-full m-0'>
        <div className='flex-col w-[36%] pl-12 pr-6 my-20'>
          <Window title='Camera'>
            <img
              src={`${import.meta.env.VITE_ROVER_URL}/${
                camMode == CamMode.NORMAL ? 'native_feed' : 'thermal_feed'
              }`}
              className='min-h-[25.5rem]'
              alt='Rover video feed'
              draggable='false'
              onClick={() => {
                setCamMode(
                  camMode == CamMode.NORMAL ? CamMode.THERMAL : CamMode.NORMAL
                );
              }}
            />
            <img
              src={`${import.meta.env.VITE_EV_URL}/stream.mjpg`}
              className='w-full min-h-[25.5rem]'
              alt='EV video feed'
              draggable='false'
            ></img>
          </Window>
          <Window title='Notifications'>
            <Notifications />
          </Window>
        </div>
        <div className='flex-col w-[64%] pr-12 pl-6 my-20'>
          <Window title='Tasks'>
            <Tasks />
          </Window>
        </div>
      </div>
    </div>
  );
}

export default Monitor2;
