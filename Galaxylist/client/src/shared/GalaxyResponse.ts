import type {Galaxy} from "./Galaxy";
import type {Viewport} from "./Viewport";

export class GalaxyResponse {
    total: number;
    galaxies: Galaxy[];
    viewports: Viewport[];
}