<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Market Curves</div>
      <md-button
        class="md-primary md-fab md-fab-top-right md-mini"
        v-on:click="emitCreateClicked()"
      >
        <md-icon>add</md-icon>
      </md-button>
    </md-card-header>
    <md-card-content>
      <md-table v-if="curves && curves.length">
        <md-table-row>
          <md-table-head>Name</md-table-head>
        </md-table-row>

        <md-table-row
          v-for="curve of curves"
          :key="curve.id"
          v-on:click="emitDetailClicked(curve.id)"
        >
          <md-table-cell>{{curve.name}}</md-table-cell>
        </md-table-row>
      </md-table>

      <ul v-if="errors && errors.length">
        <li v-for="(error,index) of errors" :key="index">{{error.message}}</li>
      </ul>
    </md-card-content>
  </md-card>
</template>

<script>
import axios from 'axios';

export default {
  name: 'get-market-curves',
  data() {
    return {
      curves: [],
      errors: [],
    };
  },
  created() {
    axios
      .get('https://localhost:5003/api')
      .then((response) => {
        this.curves = response.data;
      })
      .catch((e) => {
        this.errors.push(e);
      });
  },
  methods: {
    emitDetailClicked(id) {
      this.$emit('detailClicked', id);
    },
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
