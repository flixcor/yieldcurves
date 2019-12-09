<template>
  <v-menu
    v-model="menu"
    :nudge-right="40"
    transition="scale-transition"
    offset-y
    min-width="290px"
  >
    <template v-slot:activator="{ on }">
      <v-text-field
        v-model="selectedDate"
        :label="label"
        readonly
        v-on="on"
      />
    </template>
    <v-date-picker
      v-model="selectedDate"
      :allowed-dates="getAllowedDates"
    />
  </v-menu>
</template>

<script>
export default {
  props: {
    value: {
      type: String,
      default: null
    },
    dates: {
      type: Array,
      default: null
    },
    label: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      selectedDate: null,
      menu: false
    }
  },
  watch: {
    dates: {
      deep: true,
      handler () {
        if (!this.selectedDate) { this.selectedDate = this.getMaxDate() }
      }
    },
    selectedDate (val) {
      this.$emit('input', val)
    }
  },
  mounted () {
    this.selectedDate = this.value || this.getMaxDate()
  },
  methods: {
    getMaxDate () {
      const dates = this.dates
      if (!dates || !dates.length) {
        return null
      }
      dates.sort()
      dates.reverse()
      return dates[0]
    },
    getAllowedDates (val) {
      return this.dates && this.dates.length
        ? this.dates.includes(val)
        : true
    },
    onInput (e) {
      this.menu = false
      this.$emit('input', e)
    }
  }
}
</script>
