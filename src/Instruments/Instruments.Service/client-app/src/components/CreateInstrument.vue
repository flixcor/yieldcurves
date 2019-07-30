<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Create new instrument</div>
    </md-card-header>
    <md-card-content v-if="commands.regular">
      <mt-select
        id="vendorDropdown"
        v-model="commands.regular.command.vendor"
        label="Vendor"
        :options="commands.regular.vendors"
      />
      <bloomberg-instrument v-if="isBloomberg" v-model="commands.bloomberg" />
      <regular-instrument v-else v-model="commands.regular" />
      <md-button v-on:click="this.submit" class="md-raised md-primary">Submit</md-button>
    </md-card-content>
    <md-progress-bar v-else md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from './Material/MtSelect.vue';

const endpoint = 'https://localhost:44346';

export default {
  components: {
    BloombergInstrument: () => import('./BloombergInstrument.vue'),
    RegularInstrument: () => import('./RegularInstrument.vue'),
    MtSelect,
  },
  computed: {
    isBloomberg() {
      if (!this.commands.regular) return false;
      return this.commands.regular.command.vendor === 'Bloomberg';
    },
  },
  data() {
    return {
      commands: {},
      errors: [],
    };
  },
  methods: {
    submit() {
      const isBB = () => this.commands.regular.command.vendor === 'Bloomberg';

      const obj = isBB()
        ? this.commands.bloomberg.command
        : this.commands.regular.command;

      const route = isBB() ? '/api/bloomberg' : '/api';

      axios
        .post(endpoint + route, obj)
        .catch((e) => {
          this.errors.push(e);
        });
    },
    initialize() {
      axios
        .get(`${endpoint}/api`)
        .then((response) => {
          // JSON responses are automatically parsed.
          this.commands = response.data;
        })
        .catch((e) => {
          this.errors.push(e);
        });
    },
  },
  created() {
    this.initialize();
  },
};
</script>
