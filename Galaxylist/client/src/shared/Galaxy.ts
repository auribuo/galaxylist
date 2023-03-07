import type {RightAscention} from "./RightAscention";
import type {Declination} from "./Declination";
import type {EquatorialCoordinate} from "./EquatorialCoordinate";
import type {AzimuthalCoordinate} from "./AzimuthalCoordinate";


export class Galaxy {
   hubbleType: string ="";
   ugcNumber: number =0;
   magnitude: number=0;
   equatorialCoordinate: EquatorialCoordinate; 
   azimuthalCoordinate: AzimuthalCoordinate;
   semiMajorAxis: number;
   semiMinorAxis: number;
   positionAngle: number;
};