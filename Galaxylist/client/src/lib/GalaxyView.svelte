<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import type {Config, Data, Layout, PlotlyHTMLElement} from "plotly.js-basic-dist-min";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios, {AxiosError} from "axios";
    import {groupGalaxies} from "../shared/Plot";
    import {Galaxy} from "../shared/Galaxy";
    import GalaxyDetail from "./GalaxyDetail.svelte";

    import {Viewport} from "../shared/Viewport";
    import Aladin from "./Aladin.svelte";
    import {CalculateResponse} from "../shared/CalculateResponse";

    
    const loadingText = "Lade..."

    export let apiEndpoint: string = ""

    let loading: string = ""

    let plotVisible: boolean = false

    let galaxies: GalaxyResponse | null;
    let isFovShown: boolean = false;

    let detailGalaxy: Galaxy | null = null

    const DEF_DELAY = 1000;

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms || DEF_DELAY));
    }

    function createFovTrace(viewport: Viewport, color: string, text?: string): Data {
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
            mode: "lines",
            name: text,
            line:{
                color: color,
            },
            
            showlegend: text? true: false

        }
    }
   
    async function getGalaxies(calculateRequest: CalculateRequest): Promise<GalaxyResponse | null> {
        try {
            const resp = await axios.post<GalaxyResponse>(apiEndpoint+ "/galaxies", calculateRequest)
            return resp.data as GalaxyResponse
        } catch (e) {
            window.alert("Fehler beim Laden der Galaxien: " + (e as AxiosError).message)
            return null
        }
    }

 
    async function calculateGalaxies(calculateRequest: CalculateRequest){
        try {
            const resp = await axios.post<CalculateResponse>(apiEndpoint+ "/galaxies", calculateRequest)
            return resp.data as CalculateResponse
        } catch (e) {
            window.alert("Fehler beim Laden der Galaxien: " + (e as AxiosError).message)
            return null
        }
    }
    
    
    const displayGalaxies = async (event: CustomEvent<{ data: CalculateRequest, type: "type" | "quality" }>) => {

        loading = loadingText
        galaxies = await getGalaxies(event.detail.data);
        if (galaxies == null) {
            loading = ""
            return
        }

        const data = groupGalaxies(galaxies, event.detail.type)

        if(galaxies.viewports != null) {
            let threshold = 150
            for (let viewport of galaxies.viewports) {

                if (
                    Math.abs(viewport.topLeft.azimuth - viewport.topRight.azimuth) > threshold ||
                    Math.abs(viewport.topLeft.azimuth - viewport.bottomLeft.azimuth) > threshold ||
                    Math.abs(viewport.topLeft.azimuth - viewport.bottomRight.azimuth) > threshold ||
                    galaxies.viewports.length < 1 ||
                    viewport.topLeft.height > 80
                ) {

                } else {
                    data.push(createFovTrace(viewport, "red"))
                }
            }
        }
        let layout: Partial<Layout> = {
            xaxis: {
                range: event.detail.data.hemisphere == "E" ? [0, 180] : [180, 360]
            },
            yaxis: {
                scaleanchor: "x",
                range: [0, 90]
            },
            title: 'Galaxien in Auswahl'
        };

        const config: Partial<Config> = {responsive: true, autosizable: true}
        const plot = await Plotly.newPlot('plot', data, layout, config);
        plot.on("plotly_click", (data) => {
            const x = data.points[0].x
            const y = data.points[0].y
            detailGalaxy = galaxies.galaxies.find(g => g.azimuthalCoordinate.azimuth == x && g.azimuthalCoordinate.height == y)
        })
        loading = ""
        
        plotVisible = true
    }
    async function animateViewports(plotDiv: string, data: CalculateResponse) {
        let fTrace = createFovTrace(data.viewports[0], "yellow", "Start")
        let lTrace = createFovTrace(data.viewports[data.viewports.length-1], "green", "Ende")
        Plotly.addTraces(plotDiv,fTrace,0)
        Plotly.addTraces(plotDiv,lTrace,0)
        
        let firstTrace: Boolean = true
        for(let viewport of data.viewports){
            console.log(viewport)
            if(firstTrace){
               firstTrace=false 
            }else{
                Plotly.deleteTraces(plotDiv,0)
            }
            let trace = createFovTrace(viewport, "red")
            Plotly.addTraces(plotDiv,trace,0)
            await sleep(1000);
        }
        Plotly.deleteTraces(plotDiv, 0)
        
    }


    function handleCloseDetailPanel(type: CustomEvent<"type" | "quality">) {
        detailGalaxy = null
    }

     async function  handleCalculateGalaxies(event: CustomEvent<CalculateRequest>){
        try{
            Plotly.purge('plot')
        }catch (e) {
            
        }
        
        loading = loadingText
         event.detail.sendViewports = true
        let result: CalculateResponse = await calculateGalaxies(event.detail);
                 
      
         const data = groupGalaxies(result, "quality")


         let layout: Partial<Layout> = {
             xaxis: {
                 range: event.detail.hemisphere == "E" ? [0, 180] : [180, 360]
             },
             yaxis: {
                 scaleanchor: "x",
                 range: [0, 90]
             },
             title: 'Galaxien in Auswahl'
         };

         const config: Partial<Config> = {responsive: true, autosizable: true}
         const plot = await Plotly.newPlot('plot', data, layout, config);
         
         plot.on("plotly_click", (data) => {
             const x = data.points[0].x
             const y = data.points[0].y
             detailGalaxy = galaxies.galaxies.find(g => g.azimuthalCoordinate.azimuth == x && g.azimuthalCoordinate.height == y)
         })
         
         animateViewports('plot',result);
         
        loading = ""
        plotVisible = true
    };
</script>
<div id="galaxyView">
    <InputFields
            on:submitted={displayGalaxies}
            on:calculate={handleCalculateGalaxies}>
    </InputFields>
    {#if loading === loadingText}
        <div class="loading">
            <div class="loadingText">
                {loading}
            </div>
        </div>
    {/if}
    <br/>
    <div id="plotArea" >
        <div class="plotContainer">
            <div id="plot" class="galaxyPlot"></div>
        </div>
    </div>
    
   
    {#if detailGalaxy != null}
        <div class="galaxyInfo">
            <GalaxyDetail galaxy="{detailGalaxy}" type="quality"
                          on:closePanel={handleCloseDetailPanel}>
            </GalaxyDetail>
        </div>
    
    {/if}
</div>


{#if detailGalaxy != null }
    <Aladin ugcNumber="{detailGalaxy.ugcNumber}"></Aladin>
{/if}

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
        width: 100%;
    }

    .galaxyPlot {
        aspect-ratio: 1/1;
        width: 100%;
        margin: 10px;
    }


</style>