import type {LocationCoordinates} from './LocationCoordinates';
import axios from "axios";
import type {TimeZoneInfo} from "./TimeZoneInfo";

export class TimeZoneConverter {
    static async toUtc(date: Date, location: LocationCoordinates): Promise<string> {
        const resp = await axios.get<TimeZoneInfo>(
            `http://api.timezonedb.com/v2.1/get-time-zone?key=7YGYYSZKV0RU&format=json&by=position&lat=${location.latitude}&lng=${location.longitude}&time=${Math.floor(date.getTime() / 1000)}`
        )
        const timeZoneInfo = resp.data;
        console.log(timeZoneInfo);
        date.setMinutes(date.getMinutes() + date.getTimezoneOffset())

        const year = date.getFullYear().toString();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        const seconds = date.getSeconds().toString().padStart(2, '0');
        const offsetSign = timeZoneInfo.gmtOffset > 0 ? '+' : '-';
        const offsetHours = Math.floor(timeZoneInfo.gmtOffset / 3600).toString().padStart(2, '0');
        const offsetMinutes = (Math.floor(timeZoneInfo.gmtOffset / 60) % 60).toString().padStart(2, '0');
        const str = `${year}-${month}-${day}T${hours}:${minutes}:${seconds}${offsetSign}${offsetHours}:${offsetMinutes}`;

        console.log(str)
        return str;
    }
}