import type {Declination} from "./Declination";
import type {RightAscension} from "./RightAscension";
import type Degree from "./Degree";

export let Converter= {
     declinationToDegree(rightAscension: RightAscension) {
        return (rightAscension.H * 3600 + rightAscension.M * 60 + rightAscension.S) * 360 / 86400
    },
    degreeComponentsToDegree(degree: Degree){
        if (degree.D < 0) {
            return degree.D - (degree.M / 60 + degree.S / 3600)
        } else {
            return degree.D + degree.M / 60 + degree.S / 3600;
        }
    },

    rightAscensionToDegree(declination: Declination) {
        return this.degreeComponentsToDegree(declination as Degree)
    }
}