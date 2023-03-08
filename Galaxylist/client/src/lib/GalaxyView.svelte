<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios, {AxiosError} from "axios";
    import type {Data, Layout} from "plotly.js-basic-dist-min";
    import {groupGalaxies} from "../shared/Plot";
    import {FovViewPort} from "../shared/FovViewPort";

    export let apiEndpoint: string = ""

    let loading: string = ""

    let galaxies: GalaxyResponse | null;
    let isFovShown: boolean = false;
  

    async function getGalaxies(calculateRequest: CalculateRequest): Promise<GalaxyResponse> {
        try {
            const resp = await axios.post<GalaxyResponse>(apiEndpoint, calculateRequest)
            return resp.data as GalaxyResponse
        } catch (e) {
            window.alert("Fehler beim Laden der Galaxien: " + (e as AxiosError).message)
            return {total: 0, galaxies: []}
        }
    }

    const displayGalaxies = async (event: CustomEvent<CalculateRequest>) => {
        galaxies = await getGalaxies(event.detail);

        const typeData = groupGalaxies(galaxies, "type")
        const qualityData = groupGalaxies(galaxies, "quality")
        
        let layout: Partial<Layout>  = {
            xaxis: {
                range: event.detail.hemisphere == "E" ? [0, 180] : [180, 360]
                //range: [0, 360]
            },
            yaxis: {
                range: [0, 90]
            },
            title: 'Galaxien in Auswahl'
        };

        const config = {responsive: true}
        loading = "Loading..."
        await Plotly.newPlot('typePlot', typeData, layout, config);
        await Plotly.newPlot('qualityPlot', qualityData, layout, config);
        loading = ""
    }
    const  updateFov = async (event: CustomEvent<FovViewPort>) => {
        let coord = event.detail;
        let trace: Data = {
            x: [
                coord.pos.azimuth - coord.fov.width/2,
                coord.pos.azimuth + coord.fov.width/2,
                coord.pos.azimuth + coord.fov.width/2,
                coord.pos.azimuth - coord.fov.width/2,
                coord.pos.azimuth - coord.fov.width/2,
            ],
            y: [
                coord.pos.height + coord.fov.height/2,
                coord.pos.height + coord.fov.height/2,
                coord.pos.height - coord.fov.height/2,
                coord.pos.height - coord.fov.height/2,
                coord.pos.height + coord.fov.height/2,
            ],
            type: 'scatter',
            name: 'FOV'
        }
        if(isFovShown){
            await Plotly.deleteTraces('galaxyPlot',0)
        }
        await Plotly.addTraces('galaxyPlot',[trace],0)
        isFovShown=true
    };
</script>
<div id="galaxyView">
    <InputFields
            on:submitted={displayGalaxies}
            on:updateFov={updateFov}
    ></InputFields>
    <div>{loading}</div>
    <br/>
    <div id="plotContainer">
        <div id="typePlot" class="galaxyPlot"></div>
        <div id="qualityPlot" class="galaxyPlot"></div>
    </div>

</div>

<style>
    #galaxyView {
        display: flex;
        flex-direction: column;

        height: 100%;
        align-items: center;
        justify-content: center;

        width: 100%;
    }

    #plotContainer {
        display: flex;
        flex-direction: column;
        width: 100%;
        height: 100%;
    }

    .galaxyPlot {
        height: 100%;
        width: 100%;
        margin: 10px;
        background-color: black;
    }


</style>