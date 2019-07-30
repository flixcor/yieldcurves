const Instruments = () => import(/* webpackChunkName: "instruments" */ './components/Instruments.vue');
const MarketCurves = () => import(/* webpackChunkName: "marketcurves" */ './components/MarketCurves.vue');
const Recipes = () => import(/* webpackChunkName: "recipes" */ './components/Recipes.vue');

const routes = [
    { path: '/', component: Instruments },
    { path: '/instruments', component: Instruments },
    { path: '/marketcurves', component: MarketCurves },
    { path: '/recipes', component: Recipes }
];

export default routes;