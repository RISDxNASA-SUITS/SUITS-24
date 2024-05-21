import React, {useEffect, useState} from "react";
import {mapPosToUtm,utmToMapPos,Location,UserPosition} from "./utmToMap.ts";

interface FetchFormat{
    imu:LocationWrapper
}

interface LocationWrapper{
    eva1:ApiLoc,
    eva2:ApiLoc,
}

class ApiLoc{
    public x: number;
    public y:number;

    constructor(heading:number,posx:number,posy:number){
        this.x = posx;
        this.y = posy;

    }

    convert(){
        return utmToMapPos({x:this.x,y:this.y});
    }
}


interface UserProps{
    endPoint:string,
    img:string,
}



export const User = ({endPoint,img}:UserProps):JSX.Element=>{
    const[loc,setLoc] = useState<UserPosition>({leftOffset:0,bottomOffset:0})
    const fetchUserLoc = async () => {
        try {
            const res = await fetch(`${import.meta.env.VITE_API_URL}` + endPoint, {
                method: 'GET',
            });
            const data:FetchFormat = await res.json();

            setLoc(data.imu.eva2.convert());
        } catch (err) {
            console.log('Failed to fetch TSS:', err);
        }
    };

    useEffect(() => {
        fetchUserLoc();
        const interval = setInterval(fetchUserLoc, 1000);
        return () => {
            clearInterval(interval);
        };
    }, []);


    return <img className={"absolute h-10 w-10 bottom-6 left-6"} style={{paddingLeft:`${loc!.leftOffset * 100}%`, paddingBottom:`${loc!.leftOffset * 100}%`}} src={img} alt={"image for user/rover"}/>
}