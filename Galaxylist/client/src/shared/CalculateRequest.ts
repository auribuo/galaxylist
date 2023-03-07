import type {LocationCoordinates} from "./LocationCoordinates";
import type {Telescope} from "./Telescope";
import type {Fov} from "./Fov";


export class CalculateRequest {
    observationStart: string = ""
    observationStartDate: Date = new Date("2023-02-21T00:00:00Z");
    location: LocationCoordinates = {
        latitude: -34,
        longitude: 151
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