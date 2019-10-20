<template>
  <div id="app">
    <md-tabs
      md-sync-route
      class="md-primary"
    >
      <md-tab
        id="tab-instruments"
        to="/instruments"
        md-label="Instruments"
      />
      <md-tab
        id="tab-marketcurves"
        to="/marketcurves"
        md-label="marketcurves"
      />
      <md-tab
        id="tab-recipes"
        to="/recipes"
        md-label="recipes"
      />
      <md-tab
        id="tab-prices"
        to="/prices"
        md-label="prices"
      />
      <md-tab
        id="tab-calculatedcurves"
        to="/calculatedcurves"
        md-label="calculatedcurves"
      />
    </md-tabs>
    <router-view />
  </div>
</template>

<script>
export default {
  name: "App"
};
</script>
<style>
.comp {
  margin: 25px;
}
</style>