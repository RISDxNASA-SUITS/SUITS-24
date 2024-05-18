import FCGeo from '../../assets/icons/fc-geo.png';
import FCHazard from '../../assets/icons/fc-hazard.png';
import FCPOI from '../../assets/icons/fc-poi.png';
import FCStations from '../../assets/icons/fc-stations.png';
import './Files.css';

function Files() {
  return (
    <div className='flex flex-col w-full h-full pt-7 justify-between'>
      <div className='flex w-full justify-around pb-5'>
        <img
          className='file-card'
          src={FCGeo}
          alt='Geo Samples'
          draggable='false'
        />
        <img
          className='file-card'
          src={FCHazard}
          alt='Geo Samples'
          draggable='false'
        />
      </div>
      <div className='flex w-full justify-around pb-10'>
        <img
          className='file-card'
          src={FCPOI}
          alt='Geo Samples'
          draggable='false'
        />
        <img
          className='file-card'
          src={FCStations}
          alt='Geo Samples'
          draggable='false'
        />
      </div>
    </div>
  );
}

export default Files;
