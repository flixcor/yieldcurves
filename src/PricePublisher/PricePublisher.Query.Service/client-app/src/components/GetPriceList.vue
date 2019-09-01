<template>
  <md-card>
      <md-card-header>
        <div class="md-title">Prices</div>
        <md-button class="md-primary md-fab md-fab-top-right md-mini" v-on:click="onCreateClick">
          <md-icon>add</md-icon>
        </md-button>
      </md-card-header>
      <md-card-content>
        <md-table>
          <md-table-row>
            <md-table-head>AsOfDate</md-table-head>
            <md-table-head>AsAtDate</md-table-head>
            <md-table-head>Vendor</md-table-head>
            <md-table-head>Instrument</md-table-head>
            <md-table-head>Currency</md-table-head>
            <md-table-head>Amount</md-table-head>
            <md-table-head>PriceType</md-table-head>
          </md-table-row>
          <md-table-row v-for="price of prices" :key="price.id">
            <md-table-cell>{{price.asOfDate}}</md-table-cell>
            <md-table-cell>{{price.asAtDate}}</md-table-cell>
            <md-table-cell>{{price.vendor}}</md-table-cell>
            <md-table-cell>{{price.instrument}}</md-table-cell>
            <md-table-cell>{{price.priceCurrency}}</md-table-cell>
            <md-table-cell>{{price.priceAmount}}</md-table-cell>
            <md-table-cell>{{price.priceType}}</md-table-cell>
          </md-table-row>
        </md-table>
      </md-card-content>
    </md-card>
</template>
<script>

export default {
  props: ['prices'],
  methods: {
    onCreateClick() {
      this.$emit("create");
    }
  }
};
</script>