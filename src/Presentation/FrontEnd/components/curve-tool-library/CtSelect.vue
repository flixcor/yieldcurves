<template>
  <v-select
    :value="value"
    :items="options"
    :label="label"
    v-on="$listeners"
  />
</template>

<script>
export default {
  props: {
    options: {
      type: Array,
      required: true
    },
    label: {
      type: String,
      required: true
    },
    value: {
      type: [String, Number, Object],
      required: true
    }
  }
}
</script>
