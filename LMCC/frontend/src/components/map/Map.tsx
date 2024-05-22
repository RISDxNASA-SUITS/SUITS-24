import MapImage from '../../assets/rockyard_map_geo_coords.png';
import PenIcon from '../../assets/icons/pen.png';
import HistoryIcon from '../../assets/icons/history.png';
import POIIcon from '../../assets/icons/poi.png';
import HazardIcon from '../../assets/icons/hazard.png';
import GeoIcon from '../../assets/icons/geo.png';
import HazardImage from '../../assets/icons/hazardImage.png'
import geoImage from '../../assets/icons/geo_marker.png'
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
import {User} from "./UserComponent.tsx";
import UserCircle from "../../assets/icons/UserCircle.png"
import RoverCircle from "../../assets/icons/RoverCircle.png"
import RockJson from "../../../../backend/RockData.json"
import {location, mapPosToUtm} from "./utmToMap.ts";


function Map() {
  const [poi,setPoi] = useState(false);
  const [poiList, setPoiList] = useState<JSX.Element[]>([]);
  const [hazard,setHazard] = useState<boolean>(false)
  const [hazardList,setHazardList] = useState<JSX.Element[]>([]);
  const UserRoverList = [<User endPoint={"/get-imu"} img={UserCircle}></User>,<User endPoint={"/get-rover"} img={RoverCircle}></User>]

  const vLineWidth = useRef<number>(0)
  const wLineWidth = useRef<number>(0)
  const top = useRef<number>(0);
  const bottom = useRef<number>(0);
  const left = useRef<number>(0);
  const right = useRef<number>(0);
  const [geoMapMarkers,setGeoMapMarkers] = useState<JSX.Element[]>([]);
  const map = useRef<HTMLImageElement | null>(null)
  const canvasRef = useRef<HTMLCanvasElement|null>(null);
  const [geoMarker,setGeoMarker] = useState(false);
  const canvasContextRef = useRef<CanvasRenderingContext2D | null>(null);
  // When true, moving the mouse draws on the canvas
  const isDrawing = useRef(false);
  const x = useRef(0);
  const y = useRef(0);
  const [pos,setPos] = useState({x:0,y:0});
  useEffect(()=>{
    if(!canvasRef.current){
      return;
    }
    const new_x = pos.x / canvasRef.current?.width
    const new_y = pos.y / canvasRef.current?.height
    const new_loc:location = mapPosToUtm({leftOffset:new_x,bottomOffset:new_y})
    fetch(`${import.meta.env.VITE_API_URL}/draw-end?x=${new_loc.x}&y=${new_loc.y}`)
  },[pos])
  useEffect(()=> {
    const l = new Promise(r => setTimeout(r, 1000)).then(()=>
    {
      canvasContextRef.current = canvasRef.current!.getContext("2d");
      let refCleanup = canvasRef.current;
      const mouseDownHandler = (e)=>{
        console.log("we be clickin")
        const target:HTMLCanvasElement = e.target as HTMLCanvasElement
        const dim = target.getBoundingClientRect();
        const x1 = e.clientX - dim.left;
        const y1 = e.clientY - dim.top;
        x.current = x1;
        y.current = y1;
        isDrawing.current = true;
        e.preventDefault();
      }
      canvasRef.current!.addEventListener('mousedown', mouseDownHandler)

      const mouseMoveHandler = (e)=> {
        if (isDrawing.current) {
          const target: HTMLCanvasElement = e.target as HTMLCanvasElement
          const dim = target.getBoundingClientRect();
          const x1 = e.clientX - dim.left;
          const y1 = e.clientY - dim.top;
          drawLine(canvasContextRef.current!, x.current, y.current, x1, y1);
          x.current = x1;
          y.current = y1;
          e.preventDefault();
        }
      }
        canvasRef.current!.addEventListener('mousemove', mouseMoveHandler);
        const mouseUpHandler = (e) => {
          if (isDrawing.current) {
            isDrawing.current = false;
            setPos({x:x.current,y:y.current})
            e.preventDefault();
          }
        }

        canvasRef.current!.addEventListener('mouseup', mouseUpHandler);

        return () => {
          refCleanup!.removeEventListener("mousedown", mouseDownHandler);
          refCleanup!.removeEventListener("mouseup", mouseUpHandler);
          refCleanup!.removeEventListener("mousemove", mouseMoveHandler);
        }
      })},[])

// event.offsetX, event.offsetY gives the (x,y) offset from the edge of the canvas.

// Add the event listeners for mousedown, mousemove, and mouseup






  function drawLine(context:CanvasRenderingContext2D, x1:number, y1:number, x2:number, y2:number) {
    context.beginPath();
    context.strokeStyle = 'black';
    context.lineWidth = 1;
    context.moveTo(x1, y1);
    context.lineTo(x2, y2);
    context.stroke();
    context.closePath();
  }


    useEffect(()=>{
        const l = new Promise(r => setTimeout(r, 1000)).then(()=>{
        const rect = map.current?.getBoundingClientRect();


        canvasRef.current!.width = rect!.right - rect!.left;
        canvasRef.current!.height = rect!.bottom - rect!.top;
        top.current = 1.0125 * rect.top
        bottom.current = .9875 * rect.bottom
        left.current = 1.0125 * rect.left
        right.current = .9875 * rect.right
        const lrDist = right.current - left.current;
        const tbDist = bottom.current - top.current;
        vLineWidth.current = (lrDist / 26.71) * .925;
        wLineWidth.current = (tbDist / 25.87) * .925;

        console.log("mouse down",isDrawing)

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
      const new_x = x / canvasRef.current?.width
      const new_y = y / canvasRef.current?.height
      const new_loc:location = mapPosToUtm({leftOffset:new_x,bottomOffset:new_y})
      fetch(`${import.meta.env.VITE_API_URL}/send-poi?x=${new_loc.x}&y=${new_loc.y}`)
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
    if(geoMarker) {
      const target: HTMLImageElement = e.target as HTMLImageElement
      const dim = target.getBoundingClientRect();
      const x = e.clientX - dim.left;
      const y = e.clientY - dim.top;
      setPoiList([...poiList, <MapIcon id={uuidv4()} x={x} y={y} img={geoImage}/>]);
      setPoi(false);
    }
  }



  return (<>
    <div className={'flex  w-full h-full p-5'}>
        <img
          src={MapImage}
          alt='Rockyard map'
          className='w-5/6 h-full object-cover'
          draggable='false'

          ref={map}


      />
      <canvas className={"w-5/6 h-full absolute z-20"} onClick={(e:React.MouseEvent<HTMLCanvasElement, MouseEvent>)=>placeIcon(e)} ref={canvasRef}></canvas>
      {geoMapMarkers}
      {UserRoverList}

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
            onClick={()=>{
              if(canvasRef.current !== null) {
                canvasContextRef.current!.clearRect(0,0,canvasRef.current.width,canvasRef.current!.height)}}
              }


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
        <button className='map-button' onClick={()=>{
          setHazard(false)
          setPoi(false)
          setGeoMarker(true);
        }}>
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
