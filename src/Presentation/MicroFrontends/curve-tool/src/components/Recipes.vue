<template>
  <div>
    <get-curve-recipes class="comp" v-on:createClicked="onCreateClicked()" />
    <component v-bind:is="detailComponent" v-if="detailComponent" class="comp" />
  </div>
</template>

<script>
import externalComponent from "../utils/external-component";

export default {
  name: "Recipes",
  components: {
    GetCurveRecipes: () =>
      externalComponent("https://localhost:5005/get-curve-recipes.umd.js")
  },
  data() {
    return {
      detailComponent: false
    };
  },
  methods: {
    onCreateClicked() {
      this.detailComponent = () =>
        externalComponent("https://localhost:5007/create-curve-recipe.umd.js");
    }
  }
};
</script>