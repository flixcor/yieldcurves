<template>
  <ct-input
    v-model="datasource.command.name"
    label="Name"
  />
</template>


<script>
export default {
  props: {
    datasource: {
      type: Object,
      required: true
    }
  }
};
</script>
