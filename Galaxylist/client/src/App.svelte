<script lang="ts">
    import GalaxyView from "./lib/GalaxyView.svelte";

    let apiEndpoint = "http://172.31.98.254:5244/api"
</script>

<main>
    <GalaxyView bind:apiEndpoint={apiEndpoint} ></GalaxyView>
</main>
