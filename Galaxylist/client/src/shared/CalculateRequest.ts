import type {LocationCoordinates} from "./LocationCoordinates";
import type {Telescope} from "./Telescope";
import type {Fov} from "./Fov";



export class CalculateRequest {
    observationStart: Date = new Date();
    location: LocationCoordinates ={
        latitude:0,
        longitude: 0
    };
    telescope: Telescope={
        focalLength:0,
        aperture:0
    }
    fov: Fov={
        width: 0,
        height: 0,
        positionAngle: 0
    };
    minimumHeight: number;
    hemisphere: "W"| "E";
}