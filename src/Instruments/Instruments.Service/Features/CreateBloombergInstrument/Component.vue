<template>
  <div>
    <ct-multiple-choice
      v-model="datasource.command.pricingSource"
      label="Pricing source"
      :options="datasource.pricingSources"
    />

    <ct-multiple-choice
      v-model="datasource.command.yellowKey"
      label="Yellow key"
      :options="datasource.yellowKeys"
    />

    <ct-input
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
  }
};
</script>
