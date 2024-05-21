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
    const left_distance = Math.max(TOP_LEFT.y - user.x,0);
    const up_distance = Math.max(BOTTOM_RIGHT.y - user.y,0);

    return {leftOffset:left_distance / lr_dist,bottomOffset:up_distance / vertical_dist}

}