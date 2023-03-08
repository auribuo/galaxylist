<script lang="ts">
    import svelteLogo from './assets/svelte.svg'
    import Counter from './lib/Counter.svelte'
    import GalaxyView from "./lib/GalaxyView.svelte";
    import InputFields from "./lib/InputFields.svelte";
    let apiEndpoint = "http://localhost:5244/api/calculate"
</script>

<main>
    <GalaxyView bind:apiEndpoint={apiEndpoint}></GalaxyView>
</main>

<style>
    main{
        width: 100%;
        height: 100%;
        display: flex;
        justify-content: center;
    }
</style>