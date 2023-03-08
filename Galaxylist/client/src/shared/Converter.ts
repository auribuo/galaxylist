import type {Declination} from "./Declination";
import type {RightAscension} from "./RightAscension";
import  Degree from "./Degree";

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
    degreeToDegreeComponents(degree: number){
        let deg: Degree = new Degree();
        deg.D = Math.floor(degree)
        deg.M = Math.floor(((degree-deg.D)*60))
        deg.S = Math.floor((degree-deg.D - deg.M)*360)
        return deg;
    },

    rightAscensionToDegree(declination: Declination) {
        return this.degreeComponentsToDegree(declination as Degree)
    }
}