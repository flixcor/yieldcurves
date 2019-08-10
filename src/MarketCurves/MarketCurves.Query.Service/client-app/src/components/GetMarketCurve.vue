<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <template v-if="curve">
      <md-card-header>
        <div class="md-title">Detail for {{name}}</div>
        <md-button
          class="md-primary md-fab md-fab-top-right md-mini"
          v-on:click="emitCreateClicked()"
        >
          <md-icon>add</md-icon>
        </md-button>
      </md-card-header>
      <md-card-content>
        <md-table>
          <md-table-row>
            <md-table-head>Tenor</md-table-head>
            <md-table-head>Vendor</md-table-head>
            <md-table-head>Name</md-table-head>
            <md-table-head>DateLag</md-table-head>
            <md-table-head>IsMandatory</md-table-head>
            <md-table-head>PriceType</md-table-head>
          </md-table-row>

          <md-table-row v-for="point of curvePoints" :key="point.tenor">
            <md-table-cell>{{point.tenor}}</md-table-cell>
            <md-table-cell>{{point.vendor}}</md-table-cell>
            <md-table-cell>{{point.name}}</md-table-cell>
            <md-table-cell>{{point.dateLag}}</md-table-cell>
            <md-table-cell>{{point.isMandatory}}</md-table-cell>
            <md-table-cell>{{point.priceType}}</md-table-cell>
          </md-table-row>
        </md-table>

      </md-card-content>
    </template>
    <md-progress-bar v-else md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>

<script>
export default {
  name: 'get-market-curve',
  props: {
    'name': {
      required: true,
      type: String
    },
    'curvePoints': {
      required: true,
      type: Array
    }
  },
  methods: {
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
