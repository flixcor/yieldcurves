<template>
  <md-card>
    <md-card-header>
      <div class="md-title">Transformations for {{name}}</div>
      <md-button
        class="md-primary md-fab md-fab-top-right md-mini"
        v-on:click="emitCreateClicked()"
      >
        <md-icon>add</md-icon>
      </md-button>
    </md-card-header>
    <md-card-content>
      <md-table v-if="transformations && transformations.length">
        <md-table-row>
          <md-table-head>Parameter</md-table-head>
          <md-table-head>Value</md-table-head>
        </md-table-row>
        <md-table-row
          v-for="(transformation, index) of transformations"
          :key="index"
        >
          <md-table-cell>{{transformation.name}}</md-table-cell>
          <md-table-cell>{{transformation.value}}</md-table-cell>
        </md-table-row>
      </md-table>
    </md-card-content>
  </md-card>
</template>

<script>

export default {
  name: 'GetCurveRecipes',
  props: ['name', 'transformations'],
  methods: {
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
