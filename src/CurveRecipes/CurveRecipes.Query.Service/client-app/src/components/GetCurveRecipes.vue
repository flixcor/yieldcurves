<template>
  <md-card>
    <md-card-header>
      <div class="md-title">Curve Recipes</div>
      <md-button
        class="md-primary md-fab md-fab-top-right md-mini"
        v-on:click="emitCreateClicked()"
      >
        <md-icon>add</md-icon>
      </md-button>
    </md-card-header>
    <md-card-content>
      <md-table v-if="recipes && recipes.length">
        <md-table-row>
          <md-table-head>Name</md-table-head>
        </md-table-row>
        <md-table-row
          v-for="curve of recipes"
          :key="curve.id"
          v-on:click="emitDetailClicked(curve.id)"
        >
          <md-table-cell>{{curve.name}}</md-table-cell>
        </md-table-row>
      </md-table>
    </md-card-content>
  </md-card>
</template>

<script>

export default {
  name: 'GetCurveRecipes',
  props: ['recipes'],
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
