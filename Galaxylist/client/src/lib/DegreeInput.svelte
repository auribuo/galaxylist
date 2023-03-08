<script lang="ts">
    import TextField from "@smui/textfield"
    import Degree from "../shared/Degree";

    export let degree: Degree = new Degree();
</script>


<div class="inputGroup">
    <input min="-360" max="359" class="input" type="number" bind:value="{degree.D}"/> <span class="unit">°</span>
    <input min="0" max="59" class="input" type="number" bind:value="{degree.M}"/>   <span class="unit">'</span>
    <input min="0" max="59" class="input" type="number" bind:value="{degree.S}"/>  <span class="unit">''</span>
</div>

<style>
    .inputGroup{
        display: flex;
        flex-direction: row;
    }
    .input{
        width: 50px;
        padding-right: 20px;
    }
    .unit{
        margin-left: -20px;
        margin-right: 5px;
    }


</style>