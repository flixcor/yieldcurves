<template>
  <v-text-field
    :value="value"
    :label="label"
    v-on="$listeners"
  />
</template>

<script>
export default {
  props: {
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
