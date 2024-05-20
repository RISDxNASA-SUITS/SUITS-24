import MapImage from '../../assets/rockyard_map_geo_coords.png';
import PenIcon from '../../assets/icons/pen.png';
import HistoryIcon from '../../assets/icons/history.png';
import POIIcon from '../../assets/icons/poi.png';
import HazardIcon from '../../assets/icons/hazard.png';
import GeoIcon from '../../assets/icons/geo.png';
import HazardImage from '../../assets/icons/hazardImage.png'
import { v4 as uuidv4 } from 'uuid';
import './Map.css';
import React, {useCallback, useEffect, useRef, useState} from "react";
import MapIcon from "./MapIcon.tsx";
import GeoPinA from "../../assets/icons/geoPinA.png";
import GeoPinB from "../../assets/icons/GeoPinB.png";
import GeoPinC from "../../assets/icons/GeoPinC.png";
import GeoPinD from "../../assets/icons/GeoPinD.png";
import GeoPinE from "../../assets/icons/GeoPinE.png";
import GeoPinF from "../../assets/icons/GeoPinF.png";
import GeoPinG from "../../assets/icons/GeoPinG.png";
import UiaPin from "../../assets/icons/UiaPin.png";
import CommPin from "../../assets/icons/CommPin.png";
function Map() {
  const [poi,setPoi] = useState(false);
  const [poiList, setPoiList] = useState<JSX.Element[]>([]);
  const [hazard,setHazard] = useState<boolean>(false)
  const [hazardList,setHazardList] = useState<JSX.Element[]>([]);

  const vLineWidth = useRef<number>(0)
  const wLineWidth = useRef<number>(0)
  const top = useRef<number>(0);
  const bottom = useRef<number>(0);
  const left = useRef<number>(0);
  const right = useRef<number>(0);
  const [geoMapMarkers,setGeoMapMarkers] = useState<JSX.Element[]>([]);
  const [hasLoaded,setHasLoaded] = useState(false);
  const [timer,setTimer] = useState(1);
  const map = useRef<HTMLImageElement | null>(null)

    useEffect(()=>{
        const l = new Promise(r => setTimeout(r, 1000)).then(()=>{
        const rect = map.current?.getBoundingClientRect();



        top.current = 1.0125 * rect.top
        bottom.current = .9875 * rect.bottom
        left.current = 1.0125 * rect.left
        right.current = .9875 * rect.right
        const lrDist = right.current - left.current;
        const tbDist = bottom.current - top.current;
        vLineWidth.current = (lrDist / 26.71) * .925;
        wLineWidth.current = (tbDist / 25.87) * .925;

        setGeoMapMarkers([
          <img key={uuidv4()} src={GeoPinA} alt={"get a location pin"} className={`absolute h-20 w-12] hover:-translate-y-1 hover:scale-[1.15]`} style={{left:5.85*vLineWidth.current, bottom:7.5*wLineWidth.current}}/>,
          <img key={uuidv4()} src={GeoPinB} alt={"geo b location pin"} className={'absolute h-20 w-12 hover:-translate-y-1 hover:scale-[1.15]'} style={{left:9.9*vLineWidth.current, bottom:9.5*wLineWidth.current}}/>,
          <img key={uuidv4()} src={GeoPinC} alt={"geo c location pin"} className={'absolute h-20 w-12 bottom-[17.15rem] left-[11.26rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:7.9*vLineWidth.current, bottom:10.65*wLineWidth.current}}/>,
          <img key={uuidv4()} src={GeoPinD} alt={"geo d location pin"} className={'absolute h-20 w-12 bottom-[20.43rem] left-[14.48rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:9.9*vLineWidth.current, bottom:12.6*wLineWidth.current}}/>,
          <img key={uuidv4()} src={GeoPinE} alt={"geo e location pin"} className={'absolute h-20 w-12 bottom-[18.8rem] left-[21.06rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:14.9*vLineWidth.current, bottom:11.6*wLineWidth.current}}/>,
          <img key={uuidv4()} src={GeoPinF} alt={"geo f location pin"} className={'absolute h-20 w-12 bottom-[25.35rem] left-[29.25rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:18.9*vLineWidth.current, bottom:15.6*wLineWidth.current}}/>,
          <img key={uuidv4()} src={GeoPinG} alt={"geo g location pin"} className={'absolute h-20 w-12 bottom-[25.35rem] left-[21.06rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:14.9*vLineWidth.current, bottom:15.6*wLineWidth.current}}/>,
          <img key={uuidv4()} src={UiaPin} alt={"Uia pin location"} className={'absolute h-20 w-16 bottom-[8.85rem] left-[22.25rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:15.6*vLineWidth.current, bottom:6.5*wLineWidth.current}}/>,
          <img key={uuidv4()} src={CommPin} alt={"Comm pin location"} className={'absolute h-20 w-20 bottom-[23.7rem] left-[31.5rem] hover:-translate-y-1 hover:scale-[1.15]'} style={{left:19.25*vLineWidth.current, bottom:13.7*wLineWidth.current}}/>
        ])
      });

    },[])




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
        ref={map}


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
            onLoad={()=>{
              if(!hasLoaded) {
                setHasLoaded(true)
              }}}
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
