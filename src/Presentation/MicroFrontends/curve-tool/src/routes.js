const Instruments = () => import(/* webpackChunkName: "instruments" */ './components/Instruments.vue');
const MarketCurves = () => import(/* webpackChunkName: "marketcurves" */ './components/MarketCurves.vue');
const Recipes = () => import(/* webpackChunkName: "recipes" */ './components/Recipes.vue');
const Prices = () => import(/* webpackChunkName: "prices" */ './components/Prices.vue');

const routes = [
    { path: '/', component: Instruments },
    { path: '/instruments', component: Instruments },
    { path: '/marketcurves', component: MarketCurves },
    { path: '/recipes', component: Recipes },
    { path: '/prices', component: Prices },
];

export default routes;