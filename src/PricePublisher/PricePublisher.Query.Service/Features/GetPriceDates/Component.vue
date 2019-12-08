<template>
  <ct-date-picker
    :value="selectedDate"
    :dates="dates"
    label="As-of date"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: "GetPriceDates",
  props: {
    entities: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      selectedDate: null
    };
  },
  computed: {
    dates() {
      if (!this.entities) {
        return [];
      }
      return this.entities.map(x => x.asOfDate);
    }
  }
};
</script>