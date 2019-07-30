<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
      <md-card-header>
        <div class="md-title">Instruments</div>
        <md-button class="md-primary md-fab md-fab-top-right md-mini" v-on:click="onCreateClick">
          <md-icon>add</md-icon>
        </md-button>
      </md-card-header>
      <md-card-content>
        <md-table v-if="instruments && instruments.length">
          <md-table-row>
            <md-table-head>Vendor</md-table-head>
            <md-table-head>Description</md-table-head>
          </md-table-row>
          <md-table-row v-for="instrument of instruments" :key="instrument.id">
            <md-table-cell>{{instrument.vendor}}</md-table-cell>
            <md-table-cell>{{instrument.description}}</md-table-cell>
          </md-table-row>
        </md-table>
        <ul v-if="errors && errors.length">
          <li v-for="(error,index) of errors" :key="index">{{error.message}}</li>
        </ul>
      </md-card-content>
    </md-card>
</template>
<script>
import axios from "axios";

export default {
  data() {
    return {
      instruments: [],
      errors: []
    };
  },
  methods: {
    onCreateClick() {
      this.$emit("create");
    }
  },
  created() {
    axios
      .get("https://localhost:44345/api")
      .then(response => {
        // JSON responses are automatically parsed.
        this.instruments = response.data;
      })
      .catch(e => {
        this.errors.push(e);
      });
  }
};
</script>