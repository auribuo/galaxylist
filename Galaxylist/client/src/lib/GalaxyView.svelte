<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios from "axios";
    import type {Data, Layout} from "plotly.js-basic-dist-min";

    export let apiEndpoint: string = ""

    let loading: string = ""

    let galaxies: GalaxyResponse | null;
    let trace: Data | null;
    let layout: Partial<Layout> | null;


    async function getGalaxies(calculateRequest: CalculateRequest): Promise<GalaxyResponse> {
        const resp = await axios.post<GalaxyResponse>(apiEndpoint, calculateRequest)
        return resp.data as GalaxyResponse
    }

    const displayGalaxies = async (event: CustomEvent<CalculateRequest>) => {
        galaxies = await getGalaxies(event.detail);

        trace = {
            x: galaxies.galaxies.map(galaxy => galaxy.azimuthalCoordinate.azimuth),
            y: galaxies.galaxies.map(galaxy => galaxy.azimuthalCoordinate.height),
            mode: 'markers',
            type: 'scatter',
            name: 'Galaxies',
            text: galaxies.galaxies.map(galaxy => galaxy.ugcNumber.toString()),
            marker: {size: 10}
        }
        layout = {
            xaxis: {
                range: event.detail.hemisphere == "E" ? [0, 180] : [180, 360]
            },
            yaxis: {
                range: [0, 90]
            },
            title: 'Galaxien in Auswahl'
        };

        const config = {responsive: true}
        loading = "Loading..."
        await Plotly.newPlot('galaxyPlot', [trace], layout, config);
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

        height: 100%;
        align-items: center;
        justify-content: center;

        width: 100%;
    }

    #galaxyPlot {
        height: 100%;
        width: 100%;
        background-color: black;
    }


</style>