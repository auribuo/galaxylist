import './app.css'
import App from './App.svelte'

// A is a global variable from aladin lite source code.
// It is declared for the typescript checker to understand it.
declare var A: any;
// Defining interface objects for the different events
// linked to Aladin-Lite.
interface AddHiPSCatalogData {
    ID: string;
    hips_service_url: string;
}
interface AddHiPSSurveyData {
    ID: string;
    hips_service_url: string;
    hips_order: string;
    hips_tile_format: string;
}
interface AddMOCData {
    moc_access_url: string;
}
interface AddVizierCatalogData {
    ID: string;
    obs_id: string;
}

const app = new App({
    target: document.getElementById('app'),
})

export default app
