import type {LocationCoordinates} from "./LocationCoordinates";
import type {Telescope} from "./Telescope";
import type {Fov} from "./Fov";
import type {Viewport} from "./Viewport";


export class CalculateRequest {
    observationStart: string = "2023-03-09T01:00:00Z"
    observationStartDate: Date = new Date("2023-03-09T01:00:00Z");
    location: LocationCoordinates = {
        latitude: 47,
        longitude: 12
    };
    telescope: Telescope = {
        focalLength: 0,
        aperture: 0
    }
    fov: Fov = {
        width: 0,
        height: 0,
        positionAngle: 0
    };
    minimumHeight: number = 30;

    minimumMajorAxis: number = 10;
    minimumMinorAxis: number = 10;
    hemisphere: "W" | "E" = "W";
    
    rasterApprox: number = 0.5;
    
    searchRadius: number = 20;
    
    maxSearchSeconds: number = 4 * 60 * 60;
    
    startPointCount: number = 10;
    
    sendViewports: boolean = false;
    
    refExposure: number = 60
}