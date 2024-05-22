import {useEffect, useState} from "react";
import {FileType} from "./FileTypes.ts";
import backIcon from "../../assets/icons/back.png"
import { v4 as uuidv4 } from 'uuid';
import bookmarkEmpty from "../../assets/icons/Unbookmark.png"
import bookmarkFull from "../../assets/icons/Bookmarked.png"
import "axios"
import axios from "axios";
import RockJson from "../../../../backend/RockData.json"

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
interface Rock{
    SiO2:number,
    TiO2:number,
    Al2O3 :number,
    FeO: number,
    MnO:number,
    MgO:number,
    CaO:number,
    K2O:number,
    P2O3:number,
    other:number,
}
interface FileInfo{
    name?:string,
    image?:string,
    location?:string,
    parent?:string,
    id:string,
    rockId?:number,
    flagged?:boolean,
    elements?:Rock

}

interface StationInfoWrapper{
    station_info:FileInfo
}

interface GeoInfoWrapper{
    sample:FileInfo
}



let iCount = 0;
const rockIndex = new Map<string,number>();

export default function RenderOpenFile({fType}:RenderFileInfo){
    const [stationFiles,setStationFiles] = useState<FileInfo[]>([])
    const [stationFile,setStationFile] = useState(0);
    const [geoFile, setGeoFile] = useState(0);
    const [geoFiles,setGeoFiles] = useState<FileInfo[]>([]);
    const [childShowing,setChildShowing] = useState(false);
    const [parentCounts,setParentCounts] = useState<number[]>([]);
    console.log(geoFile,"is geoFile")
    console.log(geoFiles,"are geofiles");
    const  ShowFile = ({name,location,parent,elements}:FileInfo)=> {
        return !parent?<div className={"h-full w-full flex flex-col"}>
            <span className={"text-2xl"}>{name}</span>
            <span className={"text-xl"}> Location is: {location}</span>
            <span className={" mt-5 text-1xl"}> Num Samples Scanned: {`${parentCounts[stationFile]?parentCounts[stationFile]:0}`} </span></div>:
            <div className={'flex flex-row gap-10'}>
                <div className={'flex flex-col flex-wrap'}>
                    <div className={'text-l'}>SiO2:{elements?.SiO2} </div>
                    <div className={'text-l'}>TiO2:{elements?.SiO2} </div>
                    <div className={'text-l'}>Al2O3:{elements!.Al2O3} </div>
                    <div className={'text-l'}>FeO:{elements!.FeO}</div>
                    <div className={'text-l'}>MnO:{elements!.MnO}</div>
                </div>
                <div className={'flex flex-col'}>
                <div className={'text-l'}>MgO:{elements!.MgO}</div>
                <div className={'text-l'}>CaO:{elements!.CaO}</div>
                <div className={'text-l'}>K2O:{elements!.K2O}</div>
                <div className={'text-l'}>P2O3:{elements!.P2O3}</div>
                <div className={'text-l'}>Other:{elements!.other}</div>
                </div>

            </div>

    }
    const fetchRover= async () => {
        try {
            const res = await fetch(`${import.meta.env.VITE_API_URL}/get-rover`, {
                method: 'GET',
            });
            const data = await res.json();
            const qr_id = data['qr_id'];
            if(qr_id === 0){
                return
            }
            RockJson.ROCKS.forEach((file)=>{
                if(file.id === qr_id){
                    axios.post(`${import.meta.env.VITE_API_URL}/make-rock`,{
                        "station_id":'G',
                        "rock":file.data,
                    })
                }
            })


        } catch (err) {
            console.log('Failed to fetch TSS:', err);
        }
    };

    useEffect(() => {
        fetchRover();
        const toCall = async () => setGeoFiles(await handleFetchGeoData())
        handleFetchGeoData().then((x)=>setGeoFiles(x))
        const interval = setInterval(fetchRover, 1000);
        const interval1 = setInterval(toCall,5000)
        return () => {
            clearInterval(interval);
            clearInterval(interval1);
        };
    }, []);
    useEffect(()=>{
        const tmp_parent_counts = Array(6).fill(0);
        geoFiles.forEach((file:FileInfo)=>{
            if(file.parent){
                tmp_parent_counts[geo_stations.indexOf(file.parent)]++;
            }

        })

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

            setStationFile(Math.min(stationFile + 1,stationFiles.length - 1));
        }
    }
    // @ts-ignore
    // @ts-ignore
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
        <button className={'tss-button tss-button-primary absolute right-2 bottom-2 h-10 w-20'} onClick={handleIncrement}>Next</button>
        </div>
            <div className={"h-[40vh] w-1/2 flex flex-column justify-end"}>
                {!childShowing && geoFiles.filter((x)=>{
                return x.parent === geo_stations[stationFile]}).map((x)=><div className={"flex flex-row justify-start"}><button onClick={()=>{
                    if(rockIndex.has(x.id)){
                        // @ts-expect-error
                        setGeoFile(x.rockId - 1)
                        setChildShowing(true);
                    }}} className={'tss-button tss-button-primary h-10 w-20'} style={x.flagged?{color:"green"}:{}}>{!x.flagged?x.rockId:x.rockId!.toString() +"(flagged)" }
                </button></div>)}
                {childShowing && <img src={geoFiles[geoFile].flagged?bookmarkFull:bookmarkEmpty}  className={'mt-3 mr-3 h-8 w-6'} onClick={
                    ()=>setGeoFiles(geoFiles.map((x,i)=> {
                    if (i === geoFile) {
                        x.flagged = !x.flagged

                    }
                    axios.post(`${import.meta.env.VITE_API_URL}` + "/flag-rock",{
                        flagged:x.flagged,
                        station_id:geo_stations[stationFile],
                        rock_id:geoFile
                    })
                    return x;
                }
                ))} alt={"picture"}/>}
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
                    const file:FileInfo = ((await sample_data.json()) as GeoInfoWrapper).sample

                    file.id = uuidv4()
                    file.parent = x;
                    console.log(iCount,"is icount");
                    rockIndex.set(file.id,file.rockId!);


                    tmp_files.push(file)

                }

            }
            console.log(tmp_files);
            return tmp_files

}


const handleFetchStationData = async():Promise<FileInfo[]> =>{
    const tmp_files:FileInfo[] = []
    for (const x of geo_stations) {
        const data = await fetch(backend_url + `/get-station?station_num=${x}`);
        const stationInfo:StationInfoWrapper = await data.json();

        stationInfo.station_info.id = uuidv4()
        tmp_files.push(stationInfo.station_info)

    }

    return tmp_files;

}


