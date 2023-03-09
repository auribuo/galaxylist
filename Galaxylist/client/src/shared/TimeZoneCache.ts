import type {LocationCoordinates} from "./LocationCoordinates";
import axios from "axios";
import type {TimeZoneInfo} from "./TimeZoneInfo";
import Hashids from "hashids";

export class TimeZoneCache {
    private static cache: { [key: string]: number } = {};

    static async getOffset(location: LocationCoordinates, date: Date): Promise<number> {
        const dateEpoch = Math.floor(date.getTime() / 1000)
        const key = TimeZoneCache.hashKeyFor(location, date)
        if (key in this.cache) {
            console.debug(`Cache hit for ${key}`)
            return this.cache[key];
        }
        console.debug(`Cache miss for ${key}. Fetching data`)
        const resp = await axios.get<TimeZoneInfo>(
            `http://api.timezonedb.com/v2.1/get-time-zone?key=7YGYYSZKV0RU&format=json&by=position&lat=${location.latitude}&lng=${location.longitude}&time=${dateEpoch}`
        )
        const timeZoneInfo = resp.data;
        this.cache[key] = timeZoneInfo.gmtOffset;
        return timeZoneInfo.gmtOffset;
    }

    private static hashKeyFor(location: LocationCoordinates, date: Date): string {
        const hashId = new Hashids("galaxylist", 8);
        const key = `${round(location.latitude, 2)}${round(location.longitude, 2)}${Math.floor(date.getTime() / 1000000)}`;
        return hashId.encode(key);
    }
}

export function round(number: number, precision: number): number {
    const factor = Math.pow(10, precision);
    return Math.round(number * factor) / factor;
}