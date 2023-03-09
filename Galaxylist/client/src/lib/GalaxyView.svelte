<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import type {Data, Layout} from "plotly.js-basic-dist-min";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios, {AxiosError} from "axios";
    import {groupGalaxies} from "../shared/Plot";
    import {FovViewPort} from "../shared/FovViewPort";
    import {AzimuthalCoordinate} from "../shared/AzimuthalCoordinate";
    import {Fov} from "../shared/Fov";

    export let apiEndpoint: string = ""

    let loading: string = ""

    let galaxies: GalaxyResponse | null;
    let isFovShown: boolean = false;
  
    
    function createFovTrace(pos: AzimuthalCoordinate, fov: Fov): Data{
        return {
            y: [
                pos.azimuth - fov.width / 2,
                pos.azimuth + fov.width / 2,
                pos.azimuth + fov.width / 2,
                pos.azimuth - fov.width / 2,
                pos.azimuth - fov.width / 2,
            ],
            x: [
                pos.height + fov.height / 2,
                pos.height + fov.height / 2,
                pos.height - fov.height / 2,
                pos.height - fov.height / 2,
                pos.height + fov.height / 2,
            ],
            type: 'scatter'
        }
    }

    async function getGalaxies(calculateRequest: CalculateRequest): Promise<GalaxyResponse> {
        try {
            const resp = await axios.post<GalaxyResponse>(apiEndpoint, calculateRequest)
            return resp.data as GalaxyResponse
        } catch (e) {
            window.alert("Fehler beim Laden der Galaxien: " + (e as AxiosError).message)
            return {total: 0, galaxies: [], viewports: []}
        }
    }

    const displayGalaxies = async (event: CustomEvent<CalculateRequest>) => {
        galaxies = await getGalaxies(event.detail);

        const typeData = groupGalaxies(galaxies, "type")
        
        const qualityData = groupGalaxies(galaxies, "quality")
        
        if(galaxies.viewports != null){
            for(let viewport of galaxies.viewports){
                typeData.push(createFovTrace(viewport.pos, event.detail.fov))
            }    
        }
        
        let layout: Partial<Layout>  = {
            xaxis: {
                range: event.detail.hemisphere == "E" ? [0, 180] : [180, 360]
                //range: [0, 360]
            },
            yaxis: {
                scaleanchor: "x",
                range: [0, 90]
            },
            title: 'Galaxien in Auswahl'
        };

        const config = {responsive: true, }
        loading = "Loading..."
        await Plotly.newPlot('typePlot', typeData, layout, config);
        await Plotly.newPlot('qualityPlot', qualityData, layout, config);
        loading = ""
    }
    const  updateFov = async (event: CustomEvent<FovViewPort>) => {
        let coord = event.detail;
        let trace: Data = createFovTrace(coord.pos, coord.fov)
        if(isFovShown){
            await Plotly.deleteTraces('typePlot',0)
        }
        await Plotly.addTraces('typePlot',[trace],0)
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
        width: 50%;
    }
    .galaxyPlot {
        aspect-ratio: 1/1;
        width: 100%;
        margin: 10px;
        background-color: black;
    }
</style>