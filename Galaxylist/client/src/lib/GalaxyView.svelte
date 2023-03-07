<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios from "axios";
    import type {Data} from "plotly.js-basic-dist-min";

    export let apiEndpoint: string = ""

    let loading: string = ""

    let galaxies: GalaxyResponse | null;

    async function getGalaxies(calculateRequest: CalculateRequest): Promise<GalaxyResponse> {
        const resp = await axios.post<GalaxyResponse>(apiEndpoint, calculateRequest)
        return resp.data as GalaxyResponse
    }

    const displayGalaxies = async (event: CustomEvent<CalculateRequest>) => {
        galaxies = await getGalaxies(event.detail);

        const traces: Data[] = galaxies.galaxies.map(galaxy => {
            return {
                x: [galaxy.azimuthalCoordinate.azimuth],
                y: [galaxy.azimuthalCoordinate.height],
                mode: 'markers',
                type: 'scatter',
                name: 'Galaxies',
                text: [galaxy.toString()],
                marker: {size: 10}
            }
        })

        const trace: Data = {
            x: galaxies.galaxies.map(galaxy => galaxy.azimuthalCoordinate.azimuth),
            y: galaxies.galaxies.map(galaxy => galaxy.azimuthalCoordinate.height),
            mode: 'markers',
            type: 'scatter',
            name: 'Galaxies',
            text: galaxies.galaxies.map(galaxy => galaxy.ugcNumber.toString()),
            marker: {size: 10}
        }

        const layout = {
            xaxis: {
                range: event.detail.hemisphere == "E" ? [0, 180] : [180, 360]
            },
            yaxis: {
                range: [0, 90]
            },
            title: 'Data Labels Hover'
        };
        loading = "Loading..."
        await Plotly.newPlot('galaxyPlot', [trace], layout);
        loading = ""
    }
</script>

<div id="galaxyView">
    <InputFields
            on:submitted={displayGalaxies}
    ></InputFields>
    <div>{loading}</div>
    <div id="galaxyPlot"></div>
</div>

<style>
    #galaxyView {
        display: flex;
        flex-direction: row;
    }

    #galaxyPlot {
        height: 100%;
        background-color: red;
    }


</style>