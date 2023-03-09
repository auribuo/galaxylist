<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import type {Config, Data, Layout, PlotData, PlotlyHTMLElement} from "plotly.js-basic-dist-min";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios, {AxiosError} from "axios";
    import {groupGalaxies} from "../shared/Plot";
    import {Galaxy} from "../shared/Galaxy";
    import GalaxyDetail from "./GalaxyDetail.svelte";

    import {Viewport} from "../shared/Viewport";
    import Aladin from "./Aladin.svelte";
    import {CalculateResponse} from "../shared/CalculateResponse";
    import {round} from "../shared/TimeZoneCache.js";


    const loadingText = "Lade..."

    export let apiEndpoint: string = ""

    let loading: string = ""

    let plotVisible: boolean = false

    let galaxies: CalculateResponse | null;

    let isFovShown: boolean = false;

    let detailGalaxy: Galaxy | null = null

    const DEF_DELAY = 1000;

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms || DEF_DELAY));
    }

    function createFovTrace(viewport: Viewport, color: string, text?: string): Partial<PlotData> {
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
            line: {
                color: color,
            },

            showlegend: !!text

        }
    }

    async function getGalaxies(calculateRequest: CalculateRequest): Promise<CalculateResponse | null> {
        try {
            const resp = await axios.post<CalculateResponse>(apiEndpoint + "/calculate/alg", calculateRequest)
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

        let data: Partial<PlotData>[] = groupGalaxies(galaxies.galaxyPath, event.detail.type)
        let traces: Partial<PlotData>[] = [{
            x: galaxies.galaxyPath.map(g => g.azimuthalCoordinate.azimuth),
            y: galaxies.galaxyPath.map(g => g.azimuthalCoordinate.height),
            text: galaxies.galaxyPath.map(g => g.ugcNumber),
            type: 'scatter',
            mode: "lines+markers",
            name: "Galaxien",
            "line.color": "blue",
            marker: {
                size: 5
            },
            showlegend: false,
        } as Partial<PlotData>]

        // galaxies.galaxyPathViewports != null
        if (false) {
            let threshold = 150
            for (let viewport of galaxies.galaxyPathViewports) {
                if (
                    Math.abs(viewport.topLeft.azimuth - viewport.topRight.azimuth) > threshold ||
                    Math.abs(viewport.topLeft.azimuth - viewport.bottomLeft.azimuth) > threshold ||
                    Math.abs(viewport.topLeft.azimuth - viewport.bottomRight.azimuth) > threshold ||
                    galaxies.galaxyPathViewports.length < 1 ||
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

        const config: Partial<Config> = {responsive: true}
        const plot = await Plotly.newPlot('plot', traces, layout, config);
        plot.on("plotly_click", (data) => {
            const x = data.points[0].x
            const y = data.points[0].y
            detailGalaxy = galaxies.galaxyPath.find(g => g.azimuthalCoordinate.azimuth == x && g.azimuthalCoordinate.height == y)
        })
        
        
        
        loading = ""

        plotVisible = true
    }

    async function animateViewports(plotDiv: string, data: CalculateResponse) {

        console.log("animate")
        let fTrace = createFovTrace(data.viewportPath[0], "yellow", "Start")
        let lTrace = createFovTrace(data.viewportPath[data.viewportPath.length - 1], "green", "Ende")
        await Plotly.addTraces(plotDiv, fTrace, 0)
        await Plotly.addTraces(plotDiv, lTrace, 0)

        let firstTrace: Boolean = true
        for (let viewport of data.viewportPath) {
            if (firstTrace) {
                firstTrace = false
            } else {
                await Plotly.deleteTraces(plotDiv, 0)
            }
            let trace = createFovTrace(viewport, "red")
            await Plotly.addTraces(plotDiv, trace, 0)
            await sleep(1000);
        }
        await Plotly.deleteTraces(plotDiv, 0)

    }


    function handleCloseDetailPanel(type: CustomEvent<"type" | "quality">) {
        detailGalaxy = null
    }
</script>
<div id="galaxyView">
    <InputFields on:submitted={displayGalaxies}>
    </InputFields>
    {#if loading === loadingText}
        <div class="loading">
            <div class="loadingText">
                {loading}
            </div>
        </div>
    {/if}
    {#if plotVisible}
        <h2>
            Errechete Qualität: {round(galaxies.totalQuality, 2)} aus {round(galaxies.galaxyPath.length, 2)} Galaxien
        </h2>
    {/if}
    <br/>
    <div id="plotArea">
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