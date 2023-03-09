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
        submitted: { data: CalculateRequest, type: string }
        calculate: CalculateRequest
    }>();
    export let calculateRequest: CalculateRequest = new CalculateRequest()
    let type: "type" | "quality" = "type";
    export let latitude: Degree = Converter.degreeToDegreeComponents(calculateRequest.location.latitude);
    export let longitude: Degree = Converter.degreeToDegreeComponents(calculateRequest.location.longitude);
    export let fovPos: FovViewPort = new FovViewPort();

    calculateRequest.fov.height = 0.5
    calculateRequest.fov.width = 0.8

    async function handleSubmitClick() {
        calculateRequest.observationStartDate = new Date(dateStr)
        calculateRequest.observationStart = await TimeZoneConverter.toUtc(calculateRequest.observationStartDate, calculateRequest.location)

        calculateRequest.location.longitude = Converter.degreeComponentsToDegree(longitude)
        calculateRequest.location.latitude = Converter.degreeComponentsToDegree(latitude)

        dispatch('submitted', {data: calculateRequest, type});
    }


    async function handleCalculateClick() {

        dispatch('calculate', calculateRequest)
    }


    let dateStr = calculateRequest.observationStartDate.toISOString().slice(0, 16);
</script>

<div id="inputField">
    <div id="inputs">
        <div id="locationFields">
            <div class="inputWrapper">
                <label class="formLabel">Längengrad [°/'/'']</label>
                <DegreeInput bind:degree={longitude}></DegreeInput>
            </div>

            <div class="inputWrapper">
                <label class="formLabel">Breitengrad [°]</label>
                <DegreeInput bind:degree={latitude}></DegreeInput>
            </div>

            <div class="inputWrapper">
                <label class="formLabel">Minimale Höhe [°]</label>
                <div class="inputUnitDiv">
                    <input min="0" max="180" class="input " type="number"
                           bind:value="{calculateRequest.minimumHeight}"/>
                </div>
            </div>

            <div class="inputWrapper">
                <label class="formLabel">Start der Observation</label>
                <div id="dateInput">
                    <input type="datetime-local" bind:value={dateStr}>
                </div>
            </div>


            <div class="inputWrapper">
                <label class="formLabel">Hemisphäre</label>
                <select bind:value={calculateRequest.hemisphere}>
                    <option value="E">Osten</option>
                    <option value="W">Westen</option>
                </select></div>
        </div>

        <div id="viewFields">
            <div class="inputWrapper">
                <label class="formLabel">Brennweite [mm]</label>
                <div>
                    <input class="input inMM" min="0" type="number" id="focalLInput"
                           bind:value="{calculateRequest.telescope.focalLength}"/>
                </div>
            </div>

            <div class="inputWrapper">
                <label class="formLabel">FOV Höhe [°]</label>
                <div class="inputUnitDiv">
                    <input min="0" max="180" class="input " id="fovHeight" type="number"
                           bind:value="{calculateRequest.fov.height}"/>
                    <span class="unit"></span>
                </div>
            </div>

            <div class="inputWrapper">
                <label class="formLabel">FOV Breite [°]</label>
                <div class="inputUnitDiv">
                    <input min="0" max="180" class="input " id="fovWidth" type="number"
                           bind:value="{calculateRequest.fov.width}"/>
                    <span class="unit"></span>
                </div>
            </div>


            <!--TODO Remove-->
            <div class="inputWrapper">
                <label class="formLabel">FOV Azimut Position [°]</label>
                <div class="inputUnitDiv">
                    <input min="0" max="180" class="input " id="fovAzimut" type="number"
                           bind:value="{fovPos.pos.azimuth}"/><span
                        class="unit"></span>
                </div>
            </div>


            <div class="inputWrapper">
                <label class="formLabel">FOV Höhe Position [°]</label>
                <div class="inputUnitDiv"><input min="0" max="90" class="input " id="fovWeight" type="number"
                                                 bind:value="{fovPos.pos.height}"/><span class="unit"></span>
                </div>
            </div>

            <div class="inputWrapper">
                <label class="formLabel">Belichtungszeit für UGC1 [s]</label>
                <div class="inputUnitDiv">
                    <input min="0" class="input" id="refExposure" type="number"
                           bind:value="{calculateRequest.refExposure}"/>
                </div>
            </div>
        </div>
    </div>

    <div class="inputWrapper-nospace">
        <label class="formLabel">Hole Viewports</label>
        <div class="inputUnitDiv"><input class="input " type="checkbox"
                                         bind:checked="{calculateRequest.sendViewports}"/><span class="unit"></span>
        </div>
    </div>

    <div class="inputWrapper-nospace" id="plotType">
        <label class="formLabel">Plot typ</label>
        <select bind:value={type}>
            <option value="type">Morphologie</option>
            <option value="quality">Qualitaet</option>
        </select>
    </div>

    <Button on:click="{handleSubmitClick}">Hole Galaxien</Button>
</div>

<style>
    .formLabel {
        flex-direction: row;
        margin-right: 10px;
    }

    #inputField {
        width: 90%;
    }

    #locationFields {
        width: 50%;
    }

    #viewFields {
        width: 50%;
    }

    #inputs {
        display: flex;
        flex-direction: row;
        flex-wrap: nowrap;
        align-items: center;
        justify-content: center;
    }

    #plotType {
        margin-top: 12px;
    }


    .inputWrapper {
        display: flex;
        justify-content: space-between;
        margin: 10px;
    }

    .inputWrapper-nospace {
        display: flex;
        margin-right: 10px;

    }

    .input {
        width: 100px;

    }


</style>