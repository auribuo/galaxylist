<script lang="ts">
    import TextField from "@smui/textfield"
    import Degree from "../shared/Degree";
    
    export let degree: Degree = new Degree();
</script>


<div class="inputGroup">
    <span class="symbol"><input min="0" max="359" class="input" type="number" bind:value="{degree.D}"/>  °</span>
    <span class="symbol"><input min="0" max="59" class="input" type="number" bind:value="{degree.M}"/>  '</span>
    <span class="symbol"><input min="0" max="59" class="input" type="number" bind:value="{degree.S}"/>  ''</span>
</div>

<style>
    .inputGroup{
        display: flex;
        flex-direction: row;
    }
    .input{
        width: 50px;
    }
    .symbol{
        padding-right: 10px
    }
     
</style>