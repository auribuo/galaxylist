import {GalaxyResponse} from "./GalaxyResponse";
import type {Viewport} from "./Viewport";
import type {Galaxy} from "./Galaxy";

export class CalculateResponse {
    total: number = 0;
    totalQuality: number = 0;
    path: Galaxy[] = [];

    galaxyPath: Galaxy[] = []
    galaxyPathViewports: Viewport[] = [];
    viewportPath: Viewport[] = [];
}