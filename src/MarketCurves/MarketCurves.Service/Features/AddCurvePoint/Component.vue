<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Add new curve point
      </span>
    </template>

    <template v-slot:content>
      <ct-multiple-choice
        v-model="command.tenor"
        label="Tenor"
        :options="tenors"
      />
      <ct-multiple-choice
        v-model="command.instrumentId"
        label="Instrument"
        :options="instrumentOptions"
      />
      <ct-input
        v-model="command.dateLag"
        type="number"
        max="0"
        label="DateLag"
      />
      <ct-switch
        v-model="command.isMandatory"
        label="Mandatory"
      />
      <br>
      <ct-multiple-choice
        v-if="hasPriceType"
        id="priceTypeDropdown"
        v-model="command.priceType"
        label="Price Type"
        :options="priceTypes"
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
        class="primary"
        fab
        @click="submit"
      >
        <v-icon>mdi-send</v-icon>
      </ct-btn>
    </template>
  </ct-card>
</template>

<script>
import axios from 'axios';

const endpoint = 'https://localhost:5001/features/add-curve-point';

export default {
  props: {
    command: {
      type: Object,
      required: true
    },
    priceTypes: {
      type: Array,
      required: true
    },
    instruments: {
      type: Array,
      required: true
    },
    tenors: {
      type: Array,
      required: true
    }
  },
  data () {
    return {
      errors: [],
    };
  },
  computed: {
    hasPriceType () {
      const match = this.instruments
        .find(x => x.id === this.command.instrumentId);

      const hasPriceType = match && match.hasPriceType;

      return hasPriceType;
    },
    instrumentOptions () {
      return this.instruments.map(x => {
        return {
          key: x.id, value: x.name
        };
      });
    }
  },
  methods: {
    submit () {
      if (!this.hasPriceType) {
        this.command.priceType = null;
      }

      axios.post(endpoint, this.command)
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
  },
};
</script>
