<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import type {Data, Layout} from "plotly.js-basic-dist-min";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios, {AxiosError} from "axios";
    import {groupGalaxies} from "../shared/Plot";
    import {FovViewPort} from "../shared/FovViewPort";
    import {Galaxy} from "../shared/Galaxy";
    import GalaxyDetail from "./GalaxyDetail.svelte";
    import {AzimuthalCoordinate} from "../shared/AzimuthalCoordinate";
    import {Fov} from "../shared/Fov";
    import {Viewport} from "../shared/Viewport";

    const loadingText = "Lade..."
    
    export let apiEndpoint: string = ""

    let loading: string = ""

    let galaxies: GalaxyResponse | null;
    let isFovShown: boolean = false;

    let typeDetailGalaxy: Galaxy | null;
    let qualityDetailGalaxy: Galaxy | null;

    function createFovTrace(viewport: Viewport): Data {
        console.log(viewport)
        /*return {
            y: [
                viewport.topLeft.height,
                viewport.topRight.height,
                viewport.bottomLeft.height,
                viewport.bottomRight.height,
                viewport.topLeft.height
            ],
            x: [
                viewport.topLeft.azimuth,
                viewport.topRight.azimuth,
                viewport.bottomLeft.azimuth,
                viewport.bottomRight.azimuth,
                viewport.bottomLeft.azimuth,
            ],
            type: 'scatter',
            showlegend: false
            
        }*/
        console.log(viewport.galaxies)
        return {
            y: [
                viewport.bottomRight.height,
                viewport.bottomLeft.height,
                viewport.topLeft.height,
                viewport.topRight.height,
                viewport.bottomRight.height,
            ],
            x: [
                viewport.bottomRight.azimuth,
                viewport.bottomLeft.azimuth,
                viewport.topLeft.azimuth,
                viewport.topRight.azimuth,
                viewport.bottomRight.azimuth,
            ],
            type: 'scatter',
     
            showlegend: false
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
        loading = loadingText
        galaxies = await getGalaxies(event.detail);

        const typeData = groupGalaxies(galaxies, "type")
        
        const qualityData = groupGalaxies(galaxies, "quality")
            const threshold = 150;
            if(galaxies.viewports != null){
            for(let viewport of galaxies.viewports){
             
                    if (
                        Math.abs(viewport.topLeft.azimuth - viewport.topRight.azimuth) > threshold ||
                        Math.abs(viewport.topLeft.azimuth - viewport.bottomLeft.azimuth) > threshold ||
                        Math.abs(viewport.topLeft.azimuth - viewport.bottomRight.azimuth) > threshold ||
                        galaxies.viewports.length<1 ||
                        viewport.topLeft.height > 80 
                    ) {

                    } else {
                        typeData.push(createFovTrace(viewport))
                    }
            }
        }
        let layout: Partial<Layout> = {
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

        const config = {responsive: true}
        const typePlot = await Plotly.newPlot('typePlot', typeData, layout, config);
        typePlot.on("plotly_click", (data) => {
            const x = data.points[0].x
            const y = data.points[0].y
            typeDetailGalaxy = galaxies.galaxies.find(g => g.azimuthalCoordinate.azimuth == x && g.azimuthalCoordinate.height == y)
        })
        const qualityPlot = await Plotly.newPlot('qualityPlot', qualityData, layout, config);
        qualityPlot.on("plotly_click", (data) => {
            const x = data.points[0].x
            const y = data.points[0].y
            qualityDetailGalaxy = galaxies.galaxies.find(g => g.azimuthalCoordinate.azimuth == x && g.azimuthalCoordinate.height == y)
        })
        loading = ""
    }
    const updateFov = async (event: CustomEvent<FovViewPort>) => {
        let coord = event.detail;
        /*let trace: Data = createFovTrace(coord.pos, coord.fov)

       if(isFovShown){
            await Plotly.deleteTraces('galaxyPlot',0)
        }
        await Plotly.addTraces('galaxyPlot',[trace],0)
        isFovShown=true*/
    };

    function handleCloseDetailPanel(type: CustomEvent<"type" | "quality">) {
        if (type.detail === "type") {
            typeDetailGalaxy = null
        } else {
            qualityDetailGalaxy = null
        }
    }
</script>
<div id="galaxyView">
    <InputFields
            on:submitted={displayGalaxies}
            on:updateFov={updateFov}>
    </InputFields>
    {#if loading === loadingText}
        <div class="loading">
            <div class="loadingText">
                {loading}
            </div>
        </div>
    {/if}
    <br/>
    <div id="plotArea">
        <div class="plotContainer">
            <div id="typePlot" class="galaxyPlot"></div>
            {#if typeDetailGalaxy != null}
                <div class="galaxyInfo">
                    <GalaxyDetail galaxy="{typeDetailGalaxy}" type="type" on:closePanel={handleCloseDetailPanel}>
                    </GalaxyDetail>
                </div>
            {/if}
        </div>
        <div class="plotContainer">
            <div id="qualityPlot" class="galaxyPlot"></div>
            {#if qualityDetailGalaxy != null}
                <div class="galaxyInfo">
                    <GalaxyDetail galaxy="{qualityDetailGalaxy}" type="quality" on:closePanel={handleCloseDetailPanel}>
                    </GalaxyDetail>
                </div>
            {/if}
        </div>
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
    
    .loading {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        height: 100%;
        width: 100%;
    }
    
    .loadingText {
        font-size: 2em;
    }

    #plotArea {
        display: flex;
        flex-direction: column;
        width: 100%;
        height: 100%;
    }

    .plotContainer {
        display: flex;
        flex-direction: row;
        flex-wrap: nowrap;
        align-items: center;
        justify-content: center;
    }

    .galaxyInfo {
        height: 100%;
        width: 25%;
    }

    .galaxyPlot {
        aspect-ratio: 1/1;
        width: 100%;
        margin: 10px;
        background-color: black;
    }


</style>