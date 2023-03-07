<script lang="ts">
    import Button from '@smui/button'
    import DegreeInput from "./DegreeInput.svelte";

    import {DateInput} from 'date-picker-svelte'
    import {CalculateRequest} from "../shared/CalculateRequest";
    import {createEventDispatcher} from "svelte";
    import {TimeZoneConverter} from "../shared/TimeZoneConverter";

    import Degree from "../shared/Degree";
    import {Converter} from "../shared/Converter";
    
    const dispatch = createEventDispatcher<{
        submitted: CalculateRequest
    }>();



    export let latitude: Degree = new Degree();
    export let longitude: Degree = new Degree();
    
    async function handleSubmitClick() {
        calculateRequest.observationStart = await TimeZoneConverter.toUtc(calculateRequest.observationStartDate, calculateRequest.location)

        calculateRequest.location.longitude = Converter.degreeComponentsToDegree(longitude)
        calculateRequest.location.latitude = Converter.degreeComponentsToDegree(latitude)
        
        dispatch('submitted', calculateRequest);
    }

    

    export let calculateRequest: CalculateRequest = new CalculateRequest()
</script>

<div id="inF">
    <div class="inputField degreeField">
        <label class="inputLabel degLabel">Längengrad </label>
        <DegreeInput bind:degree={longitude}></DegreeInput>
    </div>
    <div class="inputField degreeField">
        <label class="inputLabel degLabel">Breitengrad </label>
        <DegreeInput bind:degree={latitude}></DegreeInput>
    </div>
    


    <div class="inputField">
        <label class="inputLabel">Minimale Höhe</label>
        <span class="symbol"><input min="0" max="180" class="input" id="minHeight" type="number" bind:value="{calculateRequest.minimumHeight}"/> </span>
    </div>

    <div class="inputField">
        <label class="inputLabel">Start der Observation</label>
        <DateInput bind:value={calculateRequest.observationStartDate}></DateInput>
    </div>

    <div class="inputField">
        <label class="inputLabel">Hemisphäre</label>
        <select id="selectH" bind:value={calculateRequest.hemisphere}>
            <option value="E">Osten</option>
            <option value="W">Westen</option>
        </select>
    </div>

    <div class="inputField">
        <label class="inputLabel">Brennweite</label>
        <div  class="symbol"><input class="input"  type="number" id="focalLInput" bind:value="{calculateRequest.telescope.focalLength}"/>mm</div>
    </div>

    <div class="inputField">
        <label class="inputLabel">FOV Höhe</label>
        <span class="symbol"><input min="0" max="180" class="input fov"   id="fovHeight" type="number" bind:value="{calculateRequest.fov.height}"/> </span>
    </div>

    <div class="inputField">
        <label class="inputLabel">FOV Breite</label>
        <span class="symbol"><input min="0" max="180" class="input fov" id="fovWidth" type="number" bind:value="{calculateRequest.fov.width}"/> </span>
    </div>
        

    <Button on:click="{handleSubmitClick}">Hole Galaxien</Button>

</div>

<style>
    #focalLInput {
        width: 100%;
        margin-right: 5px;
        margin-left: 5px;
    }

    #selectH {
        width: 100%;
        margin-right: 15px;
    }

    #inF {
        justify-content: flex-start;
        align-items: flex-start;
    }

    .fov {
        width: 100% !important;
    }

    .inputField {
        display: flex;
        flex-direction: row;
        margin-bottom: 5px;
        margin-right: 5px;
        white-space: nowrap;
    }
    .inputLabel {
        margin-right: 20px;
    }
    .input {
        width: 50px;
    }

    .symbol {
        display: flex;
        flex-wrap: nowrap;
        padding-right: 10px;
        width: 100%;
    }

    #minHeight {
        width: 100%;
    }
</style>