import Window from '../components/window/Window';
import Notifications from '../components/notifications/Notifications';
import Tasks from '../components/tasks/Tasks';
import RoverImage from '../assets/rover-placeholder.jpg';

function Monitor2() {
  return (
    <div className='w-screen h-screen m-0'>
      <div className='flex w-full h-full m-0'>
        <div className='flex-col w-[36%] pl-12 pr-6 my-20'>
          <Window title='Camera'>
            <img src={"http://192.168.51.195:5000/native_feed"} alt='Rover video feed' draggable='false' />
            <img src={RoverImage} alt='Rover video feed' draggable='false' />
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
