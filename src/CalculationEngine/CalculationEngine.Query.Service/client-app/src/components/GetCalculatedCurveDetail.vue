<template>
  <md-table>
    <md-table-row>
      <md-table-head>Maturity</md-table-head>
      <md-table-head>Currency</md-table-head>
      <md-table-head>Value</md-table-head>
    </md-table-row>
    <md-table-row v-for="point of points" :key="point.id">
      <md-table-cell>{{point.maturity}}</md-table-cell>
      <md-table-cell>{{point.currency}}</md-table-cell>
      <md-table-cell>{{point.value}}</md-table-cell>
    </md-table-row>
  </md-table>
</template>

<script>
export default {
  props: ['points'],
};
</script>