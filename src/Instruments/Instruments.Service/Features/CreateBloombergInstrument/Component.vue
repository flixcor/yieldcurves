<template>
  <div>
    <p>Pricing source</p>
    <v-btn-toggle
      v-model="togglePricingSource"
      mandatory
    >
      <v-btn
        v-for="(pricingSource, index) in datasource.pricingSources"
        :key="index"
      >
        {{ pricingSource }}
      </v-btn>
    </v-btn-toggle>

    <p>Yellow key</p>
    <v-btn-toggle
      v-model="toggleYellowKey"
      mandatory
    >
      <v-btn
        v-for="(yellowKey, index) in datasource.yellowKeys"
        :key="index"
      >
        {{ yellowKey }}
      </v-btn>
    </v-btn-toggle>

    <v-text-field
      v-model="datasource.command.ticker"
      label="Ticker"
    />
  </div>
</template>


<script>
export default {
  props: {
    datasource: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      togglePricingSource: 0,
      toggleYellowKey: 0
    };
  },
  computed: {
    currentPricingSource () {
      return this.datasource &&
        this.datasource.pricingSources[this.togglePricingSource]
    },
    currentYellowKey () {
      return this.datasource &&
        this.datasource.yellowKeys[this.toggleYellowKey]
    }
  },
  watch: {
    togglePricingSource () {
      this.datasource.command.pricingSource = this.currentPricingSource
    },
    toggleYellowKey () {
      this.datasource.command.yellowKey = this.currentYellowKey
    }
  },
};
</script>
