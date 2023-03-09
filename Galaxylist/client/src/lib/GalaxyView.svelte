<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import type {Config, Data, Layout, PlotlyHTMLElement} from "plotly.js-basic-dist-min";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type {GalaxyResponse} from "../shared/GalaxyResponse";
    import axios, {AxiosError} from "axios";
    import {groupGalaxies} from "../shared/Plot";
    import {FovViewPort} from "../shared/FovViewPort";
    import {Galaxy} from "../shared/Galaxy";
    import GalaxyDetail from "./GalaxyDetail.svelte";
    import {AzimuthalCoordinate} from "../shared/AzimuthalCoordinate";
    import {Fov} from "../shared/Fov";
    import Aladin from "./Aladin.svelte";

    const loadingText = "Lade..."

    export let apiEndpoint: string = ""

    let loading: string = ""

    let plotVisible: boolean = false

    let galaxies: GalaxyResponse | null;
    let isFovShown: boolean = false;

    let detailGalaxy: Galaxy | null = null

    function createFovTrace(pos: AzimuthalCoordinate, fov: Fov): Data {
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
            type: 'scatter',
            showlegend: false

        }
    }

    async function getGalaxies(calculateRequest: CalculateRequest): Promise<GalaxyResponse | null> {
        try {
            const resp = await axios.post<GalaxyResponse>(apiEndpoint, calculateRequest)
            return resp.data as GalaxyResponse
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

        if (galaxies.viewports != null) {
            for (let viewport of galaxies.viewports) {
                data.push(createFovTrace(viewport.pos, event.detail.data.fov))
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
    const updateFov = async (event: CustomEvent<FovViewPort>) => {
        let coord = event.detail;
        let trace: Data = createFovTrace(coord.pos, coord.fov)

        if (isFovShown) {
            await Plotly.deleteTraces('typePlot', 0)
        }
        await Plotly.addTraces('typePlot', [trace], 0)
        isFovShown = true
    };

    function handleCloseDetailPanel(type: CustomEvent<"type" | "quality">) {
        detailGalaxy = null
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