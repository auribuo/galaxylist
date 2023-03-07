import type {LocationCoordinates} from "./LocationCoordinates";
import type {Telescope} from "./Telescope";
import type {Fov} from "./Fov";


export class CalculateRequest {
    observationStart: Date = new Date("2023-08-21T00:00:00+02:00");
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
    hemisphere: "W" | "E" = "W";
}