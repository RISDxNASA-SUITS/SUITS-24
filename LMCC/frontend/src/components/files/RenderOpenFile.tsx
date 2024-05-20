import {useEffect, useState} from "react";
import {FileType} from "./FileTypes.ts";
import backIcon from "../../assets/icons/back.png"

const backend_url = "http://localhost:5000"

interface RenderFileProps{
    fType:FileType;
}
interface BackendFileList{
    files:string[]
}
const geo_stations = ["A"]

interface FileProps{
    name:string,
    image?:string,
    location:string,
    parent?:string,

}



const  ShowFile = ({name,image,location}:FileProps)=> {

    return <div className={"h-full w-full flex-col"}><span className={"text-2xl"}>{name}</span> {image && <img src={backend_url +image} alt={"rock"}/>}</div>
}


export default function RenderOpenFile({fType}:RenderFileProps){
    const [stationFiles,setStationFiles] = useState<FileProps[]>([])
    const [stationFile,setStationFile] = useState(0);
    const [geoFile, setGeoFile] = useState(0);
    const [geoFiles,setGeoFiles] = useState<FileProps[]>([]);
    const [childShowing,setChildShowing] = useState(false);
    useEffect( ()  =>{
        const getFiles = async()=>{
           setGeoFiles(await handleFetchGeoData(FileType.Geo));
           setStationFiles(await handleFetchStationData());
        }
        getFiles().catch(()=>{console.log("something went horribly wrong :(")});
    },[])
    const handleDecrement = () =>{
        if(childShowing){
            setGeoFile(Math.min(geoFile - 1,0));
        } else {
            setStationFile(Math.min(stationFile -1 ,0));
        }
    }
    const handleIncrement = () =>{
        if(childShowing){
            setGeoFile(Math.max(geoFile + 1,geoFiles.length - 1));
        } else {
            setStationFile(Math.max(stationFile + 1,stationFiles.length - 1));
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
                {childShowing?<ShowFile {...geoFiles[geoFile]}/>:<ShowFile{...stationFiles[stationFile]}/>}
            </div>
        <button className='tss-button tss-button-primary absolute h-10 w-20 left-2 bottom-2' onClick={handleDecrement}>Prev</button>
        <button className={'tss-button tss-button-primary absolute right-2 bottom-2 h-5 w-14'} onClick={handleIncrement}>Next</button>
        </div>
            <div className={"min-h-max min-w-1/2 w-1/2 bg-black"}> lkjhkhjgkhv</div>
    </div>)
}



const handleFetchGeoData = async (fType:FileType):Promise<FileProps[]>=>{
    let geo_files_to_fetch:string[] = []
    switch(fType){
        case FileType.Geo:
            for (const x of geo_stations) {
                let data:BackendFileList|null = null
                const fetchBackend = async()=>{
                    const res = await fetch(backend_url + `/files?directory=geosample/${x}`);
                    console.log(res);
                    data = await res.json()
                    console.log("data is",data)
                }

                await fetchBackend();
                const new_files = data!.files!.map((y)=>{
                    return y.trim().replace(".txt","");
                })

                new_files.forEach((y)=>{
                    if(y!= "info"){
                        console.log(y);
                        geo_files_to_fetch.push(`${x}/${y}`);
                    }

                })
                geo_files_to_fetch = geo_files_to_fetch.filter((x)=>{console.log(x);return x != "info"})

            }


    }
    const tmp_files:FileProps[] = []
    for (const file of geo_files_to_fetch) {

        const fetchFile = async ():Promise<FileProps> =>{
            const data = await fetch(backend_url + `/get-samples/?station_num=${file.split("/")[0]}&rock_id=${file.split("/")[1]}`)
            return await data.json()

        }
        const fetchedFile:FileProps = await fetchFile()
        fetchedFile.parent = file.split("/")[0];
        tmp_files.push(fetchedFile);
        //}
    }
    return tmp_files


}


const handleFetchStationData = async():Promise<FileProps[]> =>{
    const tmp_files:FileProps[] = []
    for (const x of geo_stations) {
        const data = await fetch(backend_url + `/get-station/?station_num=${x}`);
        const stationInfo:FileProps = await data.json();
        tmp_files.push(stationInfo)

    }
    return tmp_files;

}


