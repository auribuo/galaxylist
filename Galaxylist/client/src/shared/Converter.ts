import type {Declination} from "./Declination";
import type {RightAscension} from "./RightAscension";
import  Degree from "./Degree";

export let Converter= {
    declinationToDegree(rightAscension: RightAscension) {
        return (rightAscension.hours * 3600 + rightAscension.minutes * 60 + rightAscension.S) * 360 / 86400
    },
    degreeComponentsToDegree(degree: Degree){
        if (degree.degrees < 0) {
            return degree.degrees - (degree.minutes / 60 + degree.S / 3600)
        } else {
            return degree.degrees + degree.minutes / 60 + degree.S / 3600;
        }
    },
    degreeToDegreeComponents(degree: number){
        let deg: Degree = new Degree();
        deg.degrees = Math.floor(degree)
        deg.minutes = Math.floor(((degree-deg.degrees)*60))
        deg.S = Math.floor((degree-deg.degrees - deg.minutes)*360)
        return deg;
    },

    rightAscensionToDegree(declination: Declination) {
        return this.degreeComponentsToDegree(declination as Degree)
    }
}