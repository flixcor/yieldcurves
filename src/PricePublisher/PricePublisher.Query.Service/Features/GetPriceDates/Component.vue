<template>
  <md-datepicker
    id="asOfDatePicker"
    v-model="selectedDate"
    md-immediately
    :md-disabled-dates="disabledDates"
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
  data () {
    return {
      selectedDate: this.toDateString(this.getMaxDate()),
      showCreate: false,
      disabledDates: date => {
        const dateString = this.toDateString(date);
        const dates = this.entities;
        const isMissing = !dates.some(x => x === dateString);

        return isMissing;
      }
    };
  },
  methods: {
    getMaxDate () {
      const dates = this.getDates();
      if (dates.length) {
        const maxValue = Math.max.apply(null, dates);
        return new Date(maxValue);
      }
      return null;
    },
    toDateString (date) {
      if (!date) {
        return null;
      }

      var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
      const offSetDate = (new Date(date - tzoffset));

      return offSetDate.toISOString().split('T')[0]
    },
    getDates () {
      return this.entities.map(x => new Date(x.asOfDate));
    }
  }
};
</script>