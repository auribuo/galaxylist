import type {Declination} from "./Declination";
import type {RightAscension} from "./RightAscension";

export class Converter {
    declinationToDegree(rightAscension: RightAscension) {
        return (rightAscension.H * 3600 + rightAscension.M * 60 + rightAscension.S) * 360 / 86400
    }

    rightAscensionToDegree(declination: Declination) {
        if (declination.D < 0) {
            return declination.D - (declination.M / 60 + declination.S / 3600)
        } else {
            return declination.D + declination.M / 60 + declination.S / 3600;
        }
    }
}