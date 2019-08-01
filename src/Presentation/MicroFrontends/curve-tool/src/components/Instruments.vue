<template>
  <div>
    <instruments-view v-on:create="showCreate = !showCreate" class="comp" />
    <create-instrument v-if="showCreate" class="comp" />
  </div>
</template>

<script>
import externalComponent from "../utils/external-component";

export default {
  name: "Instruments",
  components: {
    CreateInstrument: () =>
      externalComponent("https://localhost:5011/create-instrument.umd.js"),
    InstrumentsView: () =>
      externalComponent("https://localhost:5009/instruments-view.umd.js")
  },
  data() {
    return {
      showCreate: false
    };
  }
};
</script>