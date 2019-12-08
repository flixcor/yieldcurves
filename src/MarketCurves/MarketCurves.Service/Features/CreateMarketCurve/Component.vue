<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Create new market curve
      </span>
    </template>
    <template v-slot:content>
      <ct-multiple-choice
        v-model="command.country"
        label="Country"
        :options="countries"
      />
      <ct-multiple-choice
        v-model="command.curveType"
        label="Curve Type"
        :options="curveTypes"
      />
      <ct-multiple-choice
        v-model="command.floatingLeg"
        label="Floating Leg"
        :options="['N/A', ...floatingLegs]"
      />
      <ul v-if="errors.length">
        <li
          v-for="error in errors"
          :key="error"
        >
          {{ error }}
        </li>
      </ul>
    </template>
    <template v-slot:actions>
      <ct-spacer />
      <ct-btn
        class="md-raised md-primary"
        @click="submit"
      >
        Submit
      </ct-btn>
    </template>
  </ct-card>
</template>

<script>
import axios from 'axios';

const endpoint = 'https://localhost:5001/features/create-market-curve';

export default {
  props: {
    command: {
      type: Object,
      required: true
    },
    countries: {
      type: Array,
      required: true
    },
    floatingLegs: {
      type: Array,
      required: true
    },
    curveTypes: {
      type: Array,
      required: true
    }
  },
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
        .post(endpoint, this.command)
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
  },
};
</script>
