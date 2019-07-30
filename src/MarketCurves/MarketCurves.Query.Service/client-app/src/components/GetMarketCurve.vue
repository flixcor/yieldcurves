<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <template v-if="curve">
      <md-card-header>
        <div class="md-title">Detail for {{curve.name}}</div>
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

          <md-table-row v-for="point of curve.curvePoints" :key="point.tenor">
            <md-table-cell>{{point.tenor}}</md-table-cell>
            <md-table-cell>{{point.vendor}}</md-table-cell>
            <md-table-cell>{{point.name}}</md-table-cell>
            <md-table-cell>{{point.dateLag}}</md-table-cell>
            <md-table-cell>{{point.isMandatory}}</md-table-cell>
            <md-table-cell>{{point.priceType}}</md-table-cell>
          </md-table-row>
        </md-table>

        <ul v-if="errors && errors.length">
          <li v-for="(error,index) of errors" :key="index">{{error.message}}</li>
        </ul>
      </md-card-content>
    </template>
    <md-progress-bar v-else md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>

<script>
import axios from 'axios';

export default {
  name: 'get-market-curve',
  props: ['id'],
  data() {
    return {
      curve: null,
      errors: [],
    };
  },
  created() {
    axios
      .get(`https://localhost:44347/api/${this.id}`)
      .then((response) => {
        this.curve = response.data;
      })
      .catch((e) => {
        this.errors.push(e);
      });
  },
  methods: {
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
