<template>
  <md-card>
    <md-card-header>
      <div class="md-title">Instruments</div>
      <md-button class="md-primary md-fab md-fab-top-right md-mini" v-on:click="onCreateClick">
        <md-icon>add</md-icon>
      </md-button>
    </md-card-header>
    <md-card-content>
      <md-table>
        <md-table-row>
          <md-table-head>Vendor</md-table-head>
          <md-table-head>Description</md-table-head>
        </md-table-row>
        <md-table-row v-for="instrument of instruments" :key="instrument.id">
          <md-table-cell>{{instrument.vendor}}</md-table-cell>
          <md-table-cell>{{instrument.description}}</md-table-cell>
        </md-table-row>
      </md-table>
    </md-card-content>
  </md-card>
</template>
<script>
export default {
  props: ["instruments"],
  methods: {
    onCreateClick() {
      this.$emit("create");
    },
    insert(e) {
      this.instruments = [e, ...this.instruments];
    },
  }
};
</script>