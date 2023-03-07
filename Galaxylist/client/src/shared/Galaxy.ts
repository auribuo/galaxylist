import type {RightAscention} from "./RightAscention";
import type {Declination} from "./Declination";


export class Galaxy {
   magnitude: number;
   semiMajorAxis: number;
   semiMinorAxis: number;
   rightAscension: RightAscention;
   declination: Declination;
   positionAngle: number;
};