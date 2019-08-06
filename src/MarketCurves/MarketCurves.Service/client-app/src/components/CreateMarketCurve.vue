<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Create new market curve</div>
    </md-card-header>
    <md-card-content v-if="commandViewModel">
      <mt-select
        id="countryDropdown"
        v-model="commandViewModel.command.country"
        label="Country"
        :options="commandViewModel.countries"
      />
      <mt-select
        id="curveTypeDropdown"
        v-model="commandViewModel.command.curveType"
        label="Curve Type"
        :options="commandViewModel.curveTypes"
      />
      <mt-select
        id="floatingLegDropdown"
        v-model="commandViewModel.command.floatingLeg"
        label="Floating Leg"
        :options="['N/A', ...commandViewModel.floatingLegs]"
      />
      <ul v-if="errors.length">
        <li v-for="error in errors" :key="error">
          {{error}}
        </li>
      </ul>
      <md-button v-on:click="this.submit" class="md-raised md-primary">Submit</md-button>
    </md-card-content>
    <md-progress-bar v-else md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from './Material/MtSelect.vue';

const endpoint = 'https://localhost:5001';

export default {
  components: {
    MtSelect,
  },
  data() {
    return {
      commandViewModel: null,
      errors: [],
    };
  },
  methods: {
    submit() {
      if (this.commandViewModel.command.floatingLeg === 'N/A') {
        this.commandViewModel.command.floatingLeg = null;
      }

      axios
        .post(`${endpoint}/api`, this.commandViewModel.command)
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
    initialize() {
      axios
        .get(`${endpoint}/api`)
        .then((response) => {
          // JSON responses are automatically parsed.
          this.commandViewModel = response.data;
        })
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
  },
  created() {
    this.initialize();
  },
};
</script>
