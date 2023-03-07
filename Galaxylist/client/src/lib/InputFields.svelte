<script lang="ts">
    import Button from '@smui/button'
    import TextField from '@smui/textfield'
    import {Galaxy} from "../shared/Galaxy";
    import DegreeInput from "./DegreeInput.svelte";
    import Degree from "../shared/Degree";
    import { DateInput } from 'date-picker-svelte'
    
    async function getGalaxies(): Promise<Galaxy[]> {
        //@ts-ignore
        return await (await fetch("http://localhost:5244/api/v1/galaxies")).json()
    }

    function displayGalaxies() {

        getGalaxies().then((galaxies: Galaxy[]) => {
            console.log(galaxies)
            console.log(galaxies[0].declination.M)
        }).catch((error) => {
            console.error(error)
        })

    }
    
    let hemispheres = ["E", "W"]
    
    let longitude: Degree = new Degree();
    let altitude: Degree = new Degree();
    let minHeight: number=30;
    let observationStart: Date = new Date();
    let hemisphere: "E"|"W" = "E";
    let focalLength: number=200;
    let fovHeight: number = 0.5;
    let fovWidth: number = 0.8;
</script>


<!-- 


{
   "observationStart": "2023-08-21T00:00:00+02:00",
   "minimumHeight": 30,
   "hemisphere": "E",
   "location": {
       "longitude": 12,
       "latitude": 47
   },
   "telescope": {
       "focalLength": 0,
       "aperture": 0
   },
   "fov": {
       "width": 10,
       "height": 10
   }
}


-->

<div id="inF">

    <div class="inputField degreeField">
        <label class="inputLabel degLabel">Längengrad: </label>
        <DegreeInput bind:degree={longitude}></DegreeInput>
    </div>
    <div class="inputField degreeField">
        <label class="inputLabel degLabel">Breitengrad: </label>
        <DegreeInput bind:degree={altitude}></DegreeInput>
    </div>

    
    <div class="inputField">
        <label class="inputLabel">Minimale Höhe</label>
        <span class="symbol"><input min="0" max="180" class="input" id="minHeight" type="number" value="{minHeight}"/> </span>
    </div>

    <div class="inputField">
        <label class="inputLabel">Start der Observation</label>
        <DateInput bind:value={observationStart}></DateInput>
    </div>

    <div class="inputField">
        <label class="inputLabel">Hemisphäre</label>
        <select id="selectH" bind:value={hemisphere}>
            <option value="E">Osten</option>
            <option value="W">Westen</option>
        </select>
    </div>

    <div class="inputField">
        <label class="inputLabel">Brennweite</label>
        <div  class="symbol"><input class="input"  type="number" id="focalLInput" value="{focalLength}"/>mm</div>
    </div>

    <div class="inputField">
        <label class="inputLabel">FOV Höhe</label>
        <span class="symbol"><input min="0" max="180" class="input fov"   id="fovHeight" type="number" value="{fovHeight}"/> </span>
    </div>

    <div class="inputField">
        <label class="inputLabel">FOV Breite</label>
        <span class="symbol"><input min="0" max="180" class="input fov" id="fovWidth" type="number" value="{fovWidth}"/> </span>
    </div>
    
    
    <Button on:click="{displayGalaxies}">Hole Galaxien</Button>
    
</div>

<style>
    #focalLInput{
        width: 100%;
        margin-right: 5px;
        margin-left: 5px;
    }
    #selectH{
        width: 100%;
        margin-right: 15px;
    }
    #inF {
        justify-content: flex-start;
        align-items: flex-start;
    }
    .fov{
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
    .input{
        width: 50px;
    }
    .symbol{
        display: flex;
        flex-wrap: nowrap;
        padding-right: 10px;
        width: 100%;
    }
    #minHeight{
        width: 100%;
    }
</style>