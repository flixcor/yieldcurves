<template>
  <ct-date-picker
    :dates="dates"
    label="As-of date"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: "GetCalculationDates",
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