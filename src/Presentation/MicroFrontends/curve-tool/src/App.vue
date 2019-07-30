<template>
  <div id="app">
    <md-tabs  md-sync-route class="md-primary">
      <md-tab to="/instruments" id="tab-instruments" md-label="Instruments"></md-tab>
      <md-tab to="/marketcurves" id="tab-marketcurves" md-label="marketcurves"></md-tab>
      <md-tab to="/recipes" id="tab-recipes" md-label="recipes"></md-tab>
    </md-tabs>
    <router-view />
  </div>
</template>

<script>
export default {
  name: "app"
};
</script>
<style>
.comp {
  margin: 25px;
}
</style>