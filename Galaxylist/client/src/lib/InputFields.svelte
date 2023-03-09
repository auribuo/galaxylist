<script lang="ts">
    import Button from '@smui/button'
    import DegreeInput from "./DegreeInput.svelte";


    import {DateInput} from 'date-picker-svelte'
    import {CalculateRequest} from "../shared/CalculateRequest";
    import {createEventDispatcher} from "svelte";
    import {TimeZoneConverter} from "../shared/TimeZoneConverter";

    import Degree from "../shared/Degree";
    import {Converter} from "../shared/Converter";
    import {AzimuthalCoordinate} from "../shared/AzimuthalCoordinate";
    import {FovViewPort} from "../shared/FovViewPort";

    const dispatch = createEventDispatcher<{
        submitted: CalculateRequest,
        updateFov: FovViewPort
    }>();
    export let calculateRequest: CalculateRequest = new CalculateRequest()

    export let latitude: Degree = Converter.degreeToDegreeComponents(calculateRequest.location.latitude);
    export let longitude: Degree = Converter.degreeToDegreeComponents(calculateRequest.location.longitude);
    export let fovPos: FovViewPort = new  FovViewPort();
    
    
    
    async function handleSubmitClick() {
        calculateRequest.observationStartDate = new Date(dateStr)
        calculateRequest.observationStart = await TimeZoneConverter.toUtc(calculateRequest.observationStartDate, calculateRequest.location)

        calculateRequest.location.longitude = Converter.degreeComponentsToDegree(longitude)
        calculateRequest.location.latitude = Converter.degreeComponentsToDegree(latitude)
        
        dispatch('submitted', calculateRequest);
    }
    
    async function handleUpdateFovRect(){
        fovPos.fov.height = calculateRequest.fov.height;
        fovPos.fov.width = calculateRequest.fov.width;
        dispatch('updateFov',fovPos)
    }
    /*
    TODO
    Löcher in Galaxien
    Position der Kamera nach nachführung
    Distanz mit spärischer Trigonometrie
    Viewport korrigieren auf Ausrichtung
    Kreisbogen berechnung in Dokumentation 
     */
    
    let dateStr = calculateRequest.observationStartDate.toISOString().slice(0, 16);
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
        <input type="datetime-local" bind:value={dateStr}>
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


    <label class="formLabel">FOV Azimut Position</label>
    <div class="inputUnitDiv"><input min="0" max="180" class="input " id="fovAzimut" type="number"
                                     bind:value="{fovPos.pos.azimuth}"/><span class="unit">°</span></div>


    <label class="formLabel">FOV Höhe Position</label>
    <div class="inputUnitDiv"><input min="0" max="90" class="input " id="fovWeight" type="number"
                                     bind:value="{fovPos.pos.height}"/><span class="unit">°</span></div>

    
    <label class="formLabel">Hole Viewports</label>
    <div class="inputUnitDiv">
        <input class="input "  type="checkbox"
                                     bind:checked="{calculateRequest.sendViewports}"/><span class="unit"></span>
        <input class="input "  type="number"
               bind:value="{calculateRequest.sendViewports}"/><span class="unit"></span>
        <input class="input "  type="number"   placeholder="x"
               bind:value="{calculateRequest.sendViewports}"/><span class="unit"></span>
    
    
    </div>
    
    <label class="formLabel">Belichtungszeit für UGC1</label>
    <div class="inputUnitDiv"><input min="0" class="input " id="refExposure" type="number"
                                     bind:value="{calculateRequest.refExposure}"/><span class="unit">s</span></div>


    <Button on:click="{handleSubmitClick}">Hole Galaxien</Button>
    <Button on:click="{handleUpdateFovRect}">Aktualisiere Fov</Button>

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