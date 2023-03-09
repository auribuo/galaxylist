<script lang="ts">
    import GalaxyView from "./lib/GalaxyView.svelte";

    let apiEndpoint = "http://localhost:5244/api"
</script>

<main>
    <GalaxyView bind:apiEndpoint={apiEndpoint} ></GalaxyView>
</main>
