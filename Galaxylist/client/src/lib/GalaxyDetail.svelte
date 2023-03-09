<script lang="ts">
    import {Galaxy} from "../shared/Galaxy.js";
    import {createEventDispatcher, onMount} from "svelte";
    import * as Plotly from "plotly.js-basic-dist-min"
    import {round} from "../shared/TimeZoneCache.js";

    export let galaxy: Galaxy;
    export let type: "type" | "quality"

    const dispatch = createEventDispatcher<{ closePanel: "type" | "quality" }>()

    function handleClosePanel() {
        dispatch("closePanel", type)
    }

    Plotly.redraw("plot")
</script>
<h2>
    Details fuer UGC{galaxy.ugcNumber} ({galaxy.preferredName})
</h2>
<table style="width: 100%">
    <tr>
        <td>UGC Number</td>
        <td>{galaxy.ugcNumber}</td>
    </tr>
    <tr>
        <td>Preferred Name</td>
        <td>{galaxy.preferredName}</td>
    </tr>
    <tr>
        <td>Distance</td>
        <td>{round(galaxy.distance, 2)} Mpc</td>
    </tr>
    <tr>
        <td>Magnitude</td>
        <td>{galaxy.magnitude}</td>
    </tr>
    <tr>
        <td>Type</td>
        <td>{galaxy.morphology}</td>
    </tr>
    <tr>
        <td>Quality</td>
        <td>{round(galaxy.quality, 2)}</td>
    </tr>
    <tr>
        <td>Azimuth</td>
        <td>{round(galaxy.azimuthalCoordinate.azimuth, 2)}°</td>
    </tr>
    <tr>
        <td>Elevation</td>
        <td>{round(galaxy.azimuthalCoordinate.height, 2)}°</td>
    </tr>
    <tr>
        <td colspan="2">
            <button on:click={handleClosePanel} style="width: 100%">Close</button>
        </td>
    </tr>
</table>