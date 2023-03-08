import type {LocationCoordinates} from "./LocationCoordinates";
import axios from "axios";
import type {TimeZoneInfo} from "./TimeZoneInfo";

export class TimeZoneCache {
    private static cache: { [key: string]: number } = {};

    static async getOffset(location: LocationCoordinates, date: Date): Promise<number> {
        const key = `${location.latitude},${location.longitude}`;
        if (key in this.cache) {
            console.log("Cache hit")
            return this.cache[key];
        }
        console.log("Cache miss")
        const resp = await axios.get<TimeZoneInfo>(
            `http://api.timezonedb.com/v2.1/get-time-zone?key=7YGYYSZKV0RU&format=json&by=position&lat=${location.latitude}&lng=${location.longitude}&time=${Math.floor(date.getTime() / 1000)}`
        )
        const timeZoneInfo = resp.data;
        this.cache[key] = timeZoneInfo.gmtOffset;
        return timeZoneInfo.gmtOffset;
    }
}