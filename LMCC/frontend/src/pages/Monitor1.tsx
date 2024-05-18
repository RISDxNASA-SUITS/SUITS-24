import Window from '../components/window/Window';
import Telemetry from '../components/tss/TSS';
import Files from '../components/files/Files';
import Map from '../components/map/Map';

function Monitor1() {
  return (
    <div className='w-screen h-screen m-0'>
      <div className='flex w-full h-full m-0'>
        <div className='flex-col w-[36%] pl-12 pr-6 mt-20'>
          <Window title='TSS'>
            <Telemetry />
          </Window>
          <Window title='Files'>
            <Files />
          </Window>
        </div>
        <div className='flex-col w-[64%] pr-12 pl-6 mt-20'>
          <Window title='Map'>
            <Map />
          </Window>
        </div>
      </div>
    </div>
  );
}

export default Monitor1;
