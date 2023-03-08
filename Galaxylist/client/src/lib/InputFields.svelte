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
    export let calculateRequest: CalculateRequest = new CalculateRequest()

    export let latitude: Degree = Converter.degreeToDegreeComponents(calculateRequest.location.latitude);
    export let longitude: Degree = Converter.degreeToDegreeComponents(calculateRequest.location.longitude);

    async function handleSubmitClick() {
        calculateRequest.observationStart = await TimeZoneConverter.toUtc(calculateRequest.observationStartDate, calculateRequest.location)

        calculateRequest.location.longitude = Converter.degreeComponentsToDegree(longitude)
        calculateRequest.location.latitude = Converter.degreeComponentsToDegree(latitude)

        dispatch('submitted', calculateRequest);
    }

</script>

<div id="inputField">
    <label class="formLabel">Längengrad</label>
    <DegreeInput bind:degree={longitude}></DegreeInput>

    <label class="formLabel">Breitengrad</label>
    <DegreeInput bind:degree={latitude}></DegreeInput>

    <label class="formLabel">Minimale Höhe</label>
    <div class="inputUnitDiv"><input min="0" max="180" class="input " type="number"
                                     bind:value="{calculateRequest.minimumHeight}"/> <span class="unit">°</span></div>

    <label class="formLabel">Start der Observation</label>
    <div id="dateInput">
        <DateInput bind:value={calculateRequest.observationStartDate}></DateInput>
    </div>

    <label class="formLabel">Hemisphäre</label>
    <select bind:value={calculateRequest.hemisphere}>
        <option value="E">Osten</option>
        <option value="W">Westen</option>
    </select>

    <label class="formLabel">Brennweite</label>
    <div><input class="input inMM" min="0" type="number" id="focalLInput"
                bind:value="{calculateRequest.telescope.focalLength}"/><span class="unit mm">mm</span></div>

    <label class="formLabel">FOV Höhe</label>
    <div class="inputUnitDiv"><input min="0" max="180" class="input " id="fovHeight" type="number"
                                     bind:value="{calculateRequest.fov.height}"/><span class="unit">°</span></div>

    <label class="formLabel">FOV Breite</label>
    <div class="inputUnitDiv"><input min="0" max="180" class="input " id="fovWidth" type="number"
                                     bind:value="{calculateRequest.fov.width}"/><span class="unit">°</span></div>


    <Button on:click="{handleSubmitClick}">Hole Galaxien</Button>

</div>

<style>
    .formLabel {
        flex-direction: row;

    }

    #inputField {
        display: grid;
        grid-template-columns: 200px auto;

        grid-gap: 5px;
        align-items: start;
        justify-items: start;
        height: fit-content;
        margin-right: 20px;
    }

    .inputUnitDiv {
        width: fit-content;
        display: flex;
    }

    #dateInput {
        --date-input-width: 100%;
    }


    .symbol {
        display: flex;
        flex-wrap: nowrap;
        padding-right: 10px;
        width: 100%;
    }

    .input {
        width: 50px;
        padding-right: 20px;
    }

    .input.inMM {
        padding-right: 40px;
    }

    .unit {
        margin-left: -20px;
        margin-right: 5px;
    }

    .unit.mm {
        margin-left: -40px;
        margin-right: 10px;
    }

</style>