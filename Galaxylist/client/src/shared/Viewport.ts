import type {Galaxy} from "./Galaxy";
import {AzimuthalCoordinate} from "./AzimuthalCoordinate";
import {EquatorialCoordinate} from "./EquatorialCoordinate";

export class Viewport {
    public pos: EquatorialCoordinate = new EquatorialCoordinate();
    public galaxies: Galaxy[] = [];
    public topLeft: AzimuthalCoordinate = new AzimuthalCoordinate()
    public topRight: AzimuthalCoordinate = new AzimuthalCoordinate()
    public bottomLeft: AzimuthalCoordinate = new AzimuthalCoordinate()
    public bottomRight: AzimuthalCoordinate = new AzimuthalCoordinate()
    public azimuthalPos: AzimuthalCoordinate = new AzimuthalCoordinate()
}