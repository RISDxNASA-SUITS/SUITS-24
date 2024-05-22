export interface location{
    x:number
    y:number,
}


export interface UserPosition{
    leftOffset:number,
    bottomOffset:number,
}

const TOP_LEFT:location = {x:298305,y:3272438}

const TOP_RIGHT:location = {x:298405,y:3272438}

const BOTTOM_LEFT:location= {x:298305,y:3272330}

const BOTTOM_RIGHT:location={x:298405,y:3272330}
const lr_dist =  100;
const vertical_dist = 108;


export function mapPosToUtm(user:UserPosition):location{


    return {x:user.leftOffset * lr_dist + TOP_LEFT.x, y:user.bottomOffset * vertical_dist + BOTTOM_LEFT.y}

}


export function utmToMapPos(user:location):UserPosition{
    const left_distance = Math.max(user.x - TOP_LEFT.x,0);
    const up_distance = Math.max(user.y - BOTTOM_RIGHT.y,0);
    console.log(user,up_distance,left_distance,"are all of the distances in utm")

    return {leftOffset:(left_distance / lr_dist) * 5/6 - ((3/8) * 1/28),bottomOffset:(up_distance / vertical_dist) * 5/6 + (1/27)}

}