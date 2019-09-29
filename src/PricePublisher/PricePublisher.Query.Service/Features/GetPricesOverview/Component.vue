<template>
  <md-table>
    <md-table-row>
      <md-table-head>As Of</md-table-head>
      <md-table-head>As At</md-table-head>
      <md-table-head>Vendor</md-table-head>
      <md-table-head>Instrument</md-table-head>
      <md-table-head>Currency</md-table-head>
      <md-table-head>Amount</md-table-head>
      <md-table-head>Price Type</md-table-head>
    </md-table-row>
    <md-table-row v-for="price of prices" :key="price.id">
      <md-table-cell>{{price.asOfDate.substring(0,10)}}</md-table-cell>
      <md-table-cell>{{price.asAtDate.substring(0,16)}}</md-table-cell>
      <md-table-cell>{{price.vendor}}</md-table-cell>
      <md-table-cell>{{price.instrument}}</md-table-cell>
      <md-table-cell>{{price.priceCurrency}}</md-table-cell>
      <md-table-cell>{{price.priceAmount}}</md-table-cell>
      <md-table-cell>{{price.priceType}}</md-table-cell>
    </md-table-row>
  </md-table>
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