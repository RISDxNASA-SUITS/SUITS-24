import {useEffect, useState} from "react";
import {FileType} from "./FileTypes.ts";

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
    image:string,
    location:string,

}

enum TypeFileShowing{
    parent,
    child,
}

const  ShowFile = ({name,image,location}:FileProps)=> {
    console.log("image is",image)
    return <div className={"h-full w-full"}> location{location} {name} <img src={backend_url +image} alt={"rock"}/></div>
}


export default function RenderOpenFile({fType}:RenderFileProps){
    const [files,setFiles] = useState<JSX.Element[]>([])
    const [curFile,setCurFile] = useState(0);
    useEffect( ()  =>{
        const getFiles = async()=>{
           setFiles(await handleFetchData(fType))
        }
        getFiles().catch(()=>{console.log("something went horribly wrong :(")});
    },[])
    return <div className={"h-full w-full"}><button className='tss-button tss-button-primary absolute left-2 bottom-2' onClick={()=>setCurFile(Math.min(0,curFile - 1))}>Prev File</button>{files[curFile]}<button className='tss-button tss-button-primary absolute right-2 bottom-2' onClick={()=>setCurFile(Math.max(curFile + 1,files.length - 1))}>Next File</button></div>;
}



const handleFetchData = async (fType:FileType):Promise<JSX.Element[]>=>{
    const files_to_fetch:string[] = []
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
                    files_to_fetch.push(`${x}/${y}`);
                })

            }


    }
    const tmp_files:JSX.Element[] = []
    for (const file of files_to_fetch) {

        const fetchFile = async ():Promise<FileProps> =>{
            const data = await fetch(backend_url + `/get-samples/?station_num=${file.split("/")[0]}&rock_id=${file.split("/")[1]}`)
            const fetchedFile:FileProps = await data.json()
            return fetchedFile
        }
        const fetchedFile:FileProps = await fetchFile()
        tmp_files.push(<ShowFile name={fetchedFile.name} location={fetchedFile.location} image={fetchedFile.image}/>)
        //}
    }
    return tmp_files


}


