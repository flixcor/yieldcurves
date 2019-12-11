<template>
  <ct-multiple-choice
    v-model="currentRecipe"
    label="Recipe"
    :options="recipeNames"
  />
</template>

<script>
export default {
  props: {
    recipes: {
      type: Array,
      required: true
    }
  },
  data () {
    return {
      value: this.recipes.map(x => x.id)[0],
    };
  },
  computed: {
    recipeNames () {
      return this.recipes.map(x => x.name)
    },
    currentRecipe: {
      get () {
        return this.value
          ? this.recipes.find(x => x.id === this.value).name
          : null;
      },
      set (val) {
        if (val) {
          this.value = this.recipes.find(x => x.name === val).id;
        }
        else {
          this.value = null;
        }

        this.$emit('change', this.value);
      }
    }
  },
  mounted() {
    this.$emit('change', this.value);
  },
};
</script>
