import {useEffect, useState} from "react";
import {FileType} from "./FileTypes.ts";
import backIcon from "../../assets/icons/back.png"
import { v4 as uuidv4 } from 'uuid';

const backend_url = "http://localhost:5000"

interface RenderFileInfo{
    fType:FileType;
}
interface BackendNumFiles{
    num_samples:number;
}
interface BackendFileList{
    files:string[]
}
const geo_stations = ["A","B","C","D","E","F","G"]

interface FileInfo{
    name?:string,
    image?:string,
    location?:string,
    parent?:string,
    id?:string,
    rockId?:number

}

interface StationInfoWrapper{
    station_info:FileInfo
}






export default function RenderOpenFile({fType}:RenderFileInfo){
    const [stationFiles,setStationFiles] = useState<FileInfo[]>([])
    const [stationFile,setStationFile] = useState(0);
    const [geoFile, setGeoFile] = useState(0);
    const [geoFiles,setGeoFiles] = useState<FileInfo[]>([]);
    const [childShowing,setChildShowing] = useState(false);
    const [parentCounts,setParentCounts] = useState<number[]>([]);
    const  ShowFile = ({name,image,location,parent}:FileInfo)=> {
        return <div className={"h-full w-full flex flex-col"}>
            <span className={"text-2xl"}>{name}</span>
            <span className={"text-xl"}> Location is: {location}</span>
            {!parent && <span className={" mt-5 text-1xl"}> Num Samples Scanned: {`${parentCounts[stationFile]?parentCounts[stationFile]:0}`} </span>} </div>
    }
    useEffect(()=>{
        const tmp_parent_counts = Array(6).fill(0);
        geoFiles.forEach((file:FileInfo)=>{
            if(file.parent){
                tmp_parent_counts[geo_stations.indexOf(file.parent)]++;
            }

        })
        console.log(stationFiles);
        setParentCounts(tmp_parent_counts)
    },[geoFiles])
    useEffect( ()  =>{
        const getFiles = async()=>{
           setGeoFiles(await handleFetchGeoData());
           setStationFiles(await handleFetchStationData());



        }
        getFiles().catch(()=>{console.log("something went horribly wrong :(")});
    },[])
    const handleDecrement = () =>{
        if(childShowing){
            setGeoFile(Math.max(geoFile - 1,0));
        } else {
            setStationFile(Math.max(stationFile -1 ,0));

        }
    }
    const handleIncrement = () =>{
        if(childShowing){
            setGeoFile(Math.min(geoFile + 1,geoFiles.length - 1));
        } else {
            console.log(stationFile,stationFiles[stationFile])
            setStationFile(Math.min(stationFile + 1,stationFiles.length - 1));
        }
    }
    return (
        <div className={"flex flex-row h-full w-full gap-10"}>
            <div className={"min-h-max h-[40vh] w-1/2"}>
                <div className={"flex flex-row  w-full items-center mt-2"}>
                    <img className={"ml-5 h-full w-3"} src={backIcon} alt={"back button"} onClick={()=>{childShowing?setChildShowing(false):setChildShowing(false)}}/>
                    <span className={"ml-5 text-lg"}>Back to files</span>
                </div>
            <div className={"mt-5 ml-5"}>
                {childShowing?<ShowFile {...geoFiles[geoFile]} />:<ShowFile {...stationFiles[stationFile]}/>}
            </div>
        <button className='tss-button tss-button-primary absolute h-10 w-20 left-2 bottom-2' onClick={handleDecrement}>Prev</button>
        <button className={'tss-button tss-button-primary absolute right-2 bottom-2 h-5 w-14'} onClick={handleIncrement}>Next</button>
        </div>
        </div>)
}



const handleFetchGeoData = async ():Promise<FileInfo[]> =>{
    const tmp_files:FileInfo[] = []


            for (const x of geo_stations) {
                const fetchBackend = async () => {
                    const url_test = backend_url + "/num-samples/" +x

                    const res = await fetch(url_test);
                    return await res.json()
                }

                const data: BackendNumFiles = await fetchBackend();

                if (data === null) {

                    return[];
                }
                for (let i = 0; i < data!.num_samples - 1; i++) {
                    const sample_data = await fetch(backend_url + "/get-sample?sample_site=" + x + "&rock_id=" + (i + 1).toString());
                    const file:FileInfo = await sample_data.json()
                    file.id = uuidv4()
                    file.parent = x;
                    tmp_files.push(file)
                }

            }
            return tmp_files

}


const handleFetchStationData = async():Promise<FileInfo[]> =>{
    const tmp_files:FileInfo[] = []
    for (const x of geo_stations) {
        const data = await fetch(backend_url + `/get-station?station_num=${x}`);
        const stationInfo:StationInfoWrapper = await data.json();
        console.log(stationInfo);
        stationInfo.station_info.id = uuidv4()
        tmp_files.push(stationInfo.station_info)

    }
    console.log(tmp_files);
    return tmp_files;

}


