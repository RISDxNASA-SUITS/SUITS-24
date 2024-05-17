import MapImage from '../../assets/map.png';
import PenIcon from '../../assets/icons/pen.png';
import HistoryIcon from '../../assets/icons/history.png';
import POIIcon from '../../assets/icons/poi.png';
import HazardIcon from '../../assets/icons/hazard.png';
import GeoIcon from '../../assets/icons/geo.png';
import './Map.css';

function Map() {
  return (
    <div className='flex w-full h-full p-5'>
      <img
        src={MapImage}
        alt='Rockyard map'
        className='w-5/6 h-full'
        draggable='false'
      />
      <div className='flex flex-col w-1/6 h-full p-5 justify-between'>
        <button className='map-button'>
          <img src={PenIcon} alt='Draw Path' className='map-button-image' />
          <span>Draw Path</span>
        </button>
        <button className='map-button'>
          <img
            src={HistoryIcon}
            alt='Show History'
            className='map-button-image'
          />
          <span>Show History</span>
        </button>
        <button className='map-button'>
          <img src={POIIcon} alt='Add POI' className='map-button-image' />
          <span>Add POI</span>
        </button>
        <button className='map-button'>
          <img src={HazardIcon} alt='Add Hazard' className='map-button-image' />
          <span>Add Hazard</span>
        </button>
        <button className='map-button'>
          <img src={GeoIcon} alt='Add Geo' className='map-button-image' />
          <span>Add Geo</span>
        </button>
      </div>
    </div>
  );
}

export default Map;
