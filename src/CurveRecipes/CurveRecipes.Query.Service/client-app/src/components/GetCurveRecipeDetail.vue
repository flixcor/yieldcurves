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
          <md-table-head>Transformation</md-table-head>
          <md-table-head>Parameter</md-table-head>
          <md-table-head>Value</md-table-head>
        </md-table-row>
        <template v-for="(transformation, i) of transformations">
          <md-table-row v-for="(parameter, j) of transformation.parameters" :key="i + j">
            <md-table-cell>{{transformation.name}}</md-table-cell>
            <md-table-cell>{{parameter.name}}</md-table-cell>
            <md-table-cell>{{parameter.value}}</md-table-cell>
          </md-table-row>
        </template>
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
