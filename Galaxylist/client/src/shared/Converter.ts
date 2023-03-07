import type {Declination} from "./Declination";
import type {RightAscention} from "./RightAscention";

export class Converter {
    declinationToDegree(rightAscention: RightAscention){
        return (rightAscention.H * 3600 + rightAscention.M * 60 + rightAscention.S) * 360 / 86400
    }
    rightAscentionToDegree(declination: Declination){
        if(declination.D <0){
            return declination.D - (declination.M / 60 + declination.S / 3600)
        }else{
            return declination.D + declination.M / 60 + declination.S / 3600;
        }
    }
}