<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Create new market curve</div>
    </md-card-header>
    <md-card-content>
      <mt-select
        id="countryDropdown"
        v-model="command.country"
        label="Country"
        :options="countries"
      />
      <mt-select
        id="curveTypeDropdown"
        v-model="command.curveType"
        label="Curve Type"
        :options="curveTypes"
      />
      <mt-select
        id="floatingLegDropdown"
        v-model="command.floatingLeg"
        label="Floating Leg"
        :options="['N/A', ...floatingLegs]"
      />
      <ul v-if="errors.length">
        <li v-for="error in errors" :key="error">
          {{error}}
        </li>
      </ul>
      <md-button v-on:click="this.submit" class="md-raised md-primary">Submit</md-button>
    </md-card-content>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from '../Common/Material/MtSelect.vue';

const endpoint = 'https://localhost:5001';

export default {
  components: {
    MtSelect,
  },
  props: ['command', 'countries', 'floatingLegs', 'curveTypes'],
  data() {
    return {
      errors: [],
    };
  },
  methods: {
    submit() {
      if (this.command.floatingLeg === 'N/A') {
        this.command.floatingLeg = null;
      }

      axios
        .post(`${endpoint}/api`, this.command)
        .then(() => this.$emit('success'))
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
