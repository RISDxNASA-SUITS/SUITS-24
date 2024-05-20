import FCGeo from '../../assets/icons/fc-geo.png';
import FCHazard from '../../assets/icons/fc-hazard.png';
import FCPOI from '../../assets/icons/fc-poi.png';
import FCStations from '../../assets/icons/fc-stations.png';
import './Files.css';
import {useState} from "react";

import RenderOpenFile from "./RenderOpenFile.tsx";
import {FileType} from "./FileTypes.ts";


function Files() {
    const[openFile,setOpenFile] = useState<FileType | null>(FileType.Geo)
  return (

      openFile !== null? <div className='flex-row w-[30rem] h-80 justify-between'>  <RenderOpenFile fType={openFile}/> </div>:
          <div className='flex flex-col w-full h-full pt-7 justify-between'>
              <div className='flex w-full justify-around pb-5' onClick={() => {console.log(openFile);setOpenFile(FileType.Geo)}}>
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
                      onClick={() => setOpenFile(FileType.Poi)}/>
              </div>
              <div className='flex w-full justify-around pb-10'>
                  <img
                      className='file-card'
                      src={FCPOI}
                      alt='Geo Samples'
                      draggable='false'
                      onClick={() => setOpenFile(FileType.Rover)}/>
                  <img
                      className='file-card'
                      src={FCStations}
                      alt='Geo Samples'
                      draggable='false'
                      onClick={() => setOpenFile(FileType.Stations)}/>
              </div>
          </div>

  );
}

export default Files;
