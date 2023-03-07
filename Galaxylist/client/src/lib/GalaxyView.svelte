<script lang="ts">
    import InputFields from "./InputFields.svelte";
    import {onMount} from "svelte";
    import {CalculateRequest} from "../shared/CalculateRequest";
    import * as  Plotly from 'plotly.js-basic-dist-min'
    import type  {GalaxyResponse} from "../shared/GalaxyResponse";
    
    let calculateRequest = new CalculateRequest();
    

    async function getGalaxies(): Promise<GalaxyResponse> {
        //@ts-ignore
        return await (await fetch("http://localhost:5244/api/galaxies")).json()
    }

    const displayGalaxies = ()=> {
        getGalaxies().then((galaxies: GalaxyResponse) => {
            console.log(galaxies.galaxies)
        }).catch((error) => {
            console.error(error)
        })
    }
    
    var trace1 = {
        x: [1, 2, 3, 4, 5],
        y: [1, 6, 3, 6, 1],
        mode: 'markers',
        type: 'scatter',
        name: 'Team A',
        text: ['A-1', 'A-2', 'A-3', 'A-4', 'A-5'],
        marker: { size: 12 }
    };
    
    var trace2 = {
        x: [1.5, 2.5, 3.5, 4.5, 5.5],
        y: [4, 1, 7, 1, 4],
        mode: 'markers',
        type: 'scatter',
        name: 'Team B',
        text: ['B-a', 'B-b', 'B-c', 'B-d', 'B-e'],
        marker: { size: 12 }
    };


    var data = [ trace1, trace2 ] as any; 

    var layout = {
        xaxis: {
            range: [ 0.75, 5.25 ]
        },
        yaxis: {
            range: [0, 8]
        },
        title:'Data Labels Hover'
    };
    onMount(()=>{
        Plotly.newPlot('galaxyPlot', data, layout);
    })
</script>

<div id="galaxyView">
    <InputFields 
            bind:calculateRequest = {calculateRequest}
            displayGalaxies = {displayGalaxies}
    ></InputFields>
    
    <div id="galaxyPlot"></div>

</div>

<style>
    #galaxyView{
        display: flex;
        flex-direction: row;
    }
    #galaxyPlot{
        height: 100%;
        background-color: red;
    }
    
    
</style>