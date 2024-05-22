import React, {useEffect, useState} from "react";
import {mapPosToUtm,utmToMapPos,location,UserPosition} from "./utmToMap.ts";

interface FetchFormat{
    imu:LocationWrapper
}

interface LocationWrapper{
    eva1:LocationWrapperLocation,
    eva2:LocationWrapperLocation,
}

interface LocationWrapperLocation{
    posx:number,
    posy:number,

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
            const loc = utmToMapPos({x:data.imu.eva2.posx,y: data.imu.eva2.posy})


            setLoc(loc);
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

    return <img className={"absolute h-10 w-10 bottom-6 left-6 z-10"} style={{left: `${loc.leftOffset * 100}%`, bottom:`${loc.bottomOffset * 100}%`}} src={img} alt={"image for user/rover"}/>
}