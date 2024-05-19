import MapImage from '../../assets/empty_rockyard_map.png';
import PenIcon from '../../assets/icons/pen.png';
import HistoryIcon from '../../assets/icons/history.png';
import POIIcon from '../../assets/icons/poi.png';
import HazardIcon from '../../assets/icons/hazard.png';
import GeoIcon from '../../assets/icons/geo.png';
import HazardImage from '../../assets/icons/hazardImage.png'
import { v4 as uuidv4 } from 'uuid';
import './Map.css';
import React, {useState} from "react";
import MapIcon from "./MapIcon.tsx";
import GeoPinA from "../../assets/icons/geoPinA.png";
import GeoPinB from "../../assets/icons/GeoPinB.png";
import GeoPinC from "../../assets/icons/GeoPinC.png";
import GeoPinD from "../../assets/icons/GeoPinD.png";
import GeoPinE from "../../assets/icons/GeoPinE.png";
import GeoPinF from "../../assets/icons/GeoPinF.png";
import GeoPinG from "../../assets/icons/GeoPinG.png";
function Map() {
  const [poi,setPoi] = useState(false);
  const [poiList, setPoiList] = useState<JSX.Element[]>([]);
  const [hazard,setHazard] = useState<boolean>(false)
  const [hazardList,setHazardList] = useState<JSX.Element[]>([]);
  const geoMapMarkers:JSX.Element[] = [
      <img src={GeoPinA} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[10.6rem] left-[7.93rem] hover:-translate-y-1 hover:scale-[1.15]'} />,
      <img src={GeoPinB} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[13.8rem] left-[14.48rem] hover:-translate-y-1 hover:scale-[1.15]'} />,
      <img src={GeoPinC} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[17.15rem] left-[11.26rem] hover:-translate-y-1 hover:scale-[1.15]'} />,
      <img src={GeoPinD} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[20.43rem] left-[14.48rem] hover:-translate-y-1 hover:scale-[1.15]'} />,
    <img src={GeoPinE} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[18.8rem] left-[21.06rem] hover:-translate-y-1 hover:scale-[1.15]'} />,
    <img src={GeoPinF} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[25.35rem] left-[29.25rem] hover:-translate-y-1 hover:scale-[1.15]'} />,
    <img src={GeoPinG} alt={"geo location pin"} className={'absolute h-20 w-12 bottom-[25.35rem] left-[21.06rem] hover:-translate-y-1 hover:scale-[1.15]'} />,

  ]
  const placeIcon = (e:React.MouseEvent<HTMLImageElement, MouseEvent>)=>{
    if(hazard){
      const target:HTMLImageElement = e.target as HTMLImageElement
      const dim = target.getBoundingClientRect();
      const x = e.clientX - dim.left;
      const y = e.clientY - dim.top;
      setHazardList([...hazardList,<MapIcon id={uuidv4()} x={x} y={y} img={HazardImage}/>]);
      setHazard(false);
    }
    if(poi){
      const target:HTMLImageElement = e.target as HTMLImageElement
      const dim = target.getBoundingClientRect();
      const x = e.clientX - dim.left;
      const y = e.clientY - dim.top;
      setPoiList([...poiList,<MapIcon id={uuidv4()} x={x} y={y} img={POIIcon}/>]);
      setPoi(false);
    }
  }

  return (<>
    <div className='flex w-full h-full p-5'>
      <img
        src={MapImage}
        alt='Rockyard map'
        className='w-5/6 h-full object-cover'
        draggable='false'
        onClick={(e:React.MouseEvent<HTMLImageElement, MouseEvent>)=>placeIcon(e)}

      />
      {geoMapMarkers}

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
        <button onClick={()=>{
          setHazard(false)
          setPoi(true)
        }} className='map-button'>
          <img src={POIIcon} alt='Add POI' className='map-button-image' />
          <span >Add POI</span>
        </button>

        <button onClick={()=>{
          setHazard(true)
          setPoi(false)
        }}
          className='map-button'>
          <img src={HazardIcon} alt='Add Hazard' className='map-button-image' />
          <span>Add Hazard</span>
        </button>
        <button className='map-button'>
          <img src={GeoIcon} alt='Add Geo' className='map-button-image' />
          <span>Add Geo</span>
        </button>
      </div>
    </div>
        {poiList}
        {hazardList}
      </>
  );
}

export default Map;
