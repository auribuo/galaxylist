<svelte:head>

    <!-- our code needs jQuery library -->
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.9.1.min.js" charset="utf-8"></script>

    <!-- Aladin Lite JS code -->
    <script type="text/javascript" src="https://aladin.cds.unistra.fr/AladinLite/api/v3/latest/aladin.js"
            charset="utf-8"></script>

    <!-- Creation of Aladin Lite instance with initial parameters -->
    <script type="text/javascript">
        A.init.then(() => {
            window.aladin = A.aladin('#aladin-lite-div', {
                survey: "P/DSS2/color",
                fov: 0.1,
                target: "UGC" + window.ugcNumber
            });
            window.aladinReady = true;
        });
    </script>
</svelte:head>

<script>
    export let ugcNumber = 0;

    function scrollToAladin() {
        const el = document.getElementById('aladin-lite-div');
        if (!el) return;
        el.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
    }

    $: {
        if (window.aladinReady) {
            window.aladin.gotoObject(`UGC${ugcNumber}`);
            scrollToAladin();
        }
    }
</script>

<div id="aladin-lite-div" style="height: 400px; margin-top: 50px"></div>