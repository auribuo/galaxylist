<script lang="ts">
    import Button from '@smui/button'
    import DegreeInput from "./DegreeInput.svelte";

    import {DateInput} from 'date-picker-svelte'
    import {CalculateRequest} from "../shared/CalculateRequest";
    import {createEventDispatcher} from "svelte";

    const dispatch = createEventDispatcher<{
        submitted: CalculateRequest
    }>();

    function handleSubmitClick() {
        dispatch('submitted', calculateRequest);
    }
    
    export let calculateRequest: CalculateRequest = new CalculateRequest()
</script>

<div id="inF">
    <div class="inputField degreeField">
        <label class="inputLabel degLabel">Längengrad: </label>
        <DegreeInput bind:degree={calculateRequest.location.longitude}></DegreeInput>
    </div>
    <div class="inputField degreeField">
        <label class="inputLabel degLabel">Breitengrad: </label>
        <DegreeInput bind:degree={calculateRequest.location.latitude}></DegreeInput>
    </div>


    <div class="inputField">
        <label class="inputLabel">Minimale Höhe</label>
        <span class="symbol"><input min="0" max="180" class="input" id="minHeight" type="number"
                                    value="{calculateRequest.minimumHeight}"/> </span>
    </div>

    <div class="inputField">
        <label class="inputLabel">Start der Observation</label>
        <DateInput bind:value={calculateRequest.observationStart}></DateInput>
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
        <div class="symbol"><input class="input" type="number" id="focalLInput"
                                   value="{calculateRequest.telescope.focalLength}"/>mm
        </div>
    </div>

    <div class="inputField">
        <label class="inputLabel">FOV Höhe</label>
        <span class="symbol"><input min="0" max="180" class="input fov" id="fovHeight" type="number"
                                    value="{calculateRequest.fov.height}"/> </span>
    </div>

    <div class="inputField">
        <label class="inputLabel">FOV Breite</label>
        <span class="symbol"><input min="0" max="180" class="input fov" id="fovWidth" type="number"
                                    value="{calculateRequest.fov.width}"/> </span>
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