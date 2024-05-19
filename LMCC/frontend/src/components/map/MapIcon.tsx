

interface position{
    x:number,
    y:number,
    img:string,
    id:string,
}
export default function MapIcon({x,y,img,id}:position){

    return (<img key={id} className={"h-8 w-8 absolute"} style={{left:x,top:y}} src={img}
                 alt={"poi"}></img>)
}

