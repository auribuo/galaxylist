<script lang="ts">
    import TextField from "@smui/textfield"
    import Degree from "../shared/Degree";

    export let degree: Degree = new Degree();
</script>


<div class="inputGroup">
    <input min="-360" max="359" class="input" type="number" bind:value="{degree.degrees}"/>
    <input min="0" max="59" class="input" type="number" bind:value="{degree.minutes}"/> 
    <input min="0" max="59" class="input" type="number" bind:value="{degree.S}"/>  
</div>

<style>
    .inputGroup{
        display: flex;
        flex-direction: row;
    }
    .input{
        width: 80px;
    }



</style>