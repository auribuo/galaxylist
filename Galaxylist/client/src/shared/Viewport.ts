import type {Galaxy} from "./Galaxy";
import {AzimuthalCoordinate} from "./AzimuthalCoordinate";

export class Viewport {
    public pos: AzimuthalCoordinate = new AzimuthalCoordinate();
    public galaxies: Galaxy[] = []
}