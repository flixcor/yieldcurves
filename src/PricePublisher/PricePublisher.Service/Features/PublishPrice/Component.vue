<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Publish a price
      </span>
    </template>

    <template v-slot:content>
      <ct-date-picker
        v-model="asOfDate"
        label="As-of date"
      />
      <ct-multiple-choice
        v-model="command.instrumentId"
        label="Instrument"
        :options="instrumentOptions"
      />
      <ct-multiple-choice
        v-if="hasPriceType"
        v-model="command.priceType"
        label="Price type"
        :options="priceTypes"
      />
      <ct-input
        v-model="command.priceCurrency"
        label="Price currency (ISO3)"
      />
      <ct-input
        v-model="command.priceAmount"
        label="Price amount"
      />
      <ul v-if="errors.length">
        <li
          v-for="error in errors"
          :key="error"
        >
          {{ error }}
        </li>
      </ul>

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
    </template>
  </ct-card>
</template>

<script>
import axios from 'axios';

const endpoint = 'https://localhost:5013/features/publish-price';

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
    }
  },
  data() {
    return {
      loading: false,
      errors: [],
    };
  },
  computed: {
    hasPriceType() {
      const match = this.instruments
        .find(x => x.id === this.command.instrumentId);

      const hasPriceType = match && match.hasPriceType;

      return hasPriceType;
    },
    asOfDate: {
      get() {
        return new Date(this.command.asOfDate).toISOString().split('T')[0];
      },
      set(newVal) {
        this.command.asOfDate = new Date(newVal).toJSON();
      },
    },
    instrumentOptions() {
      return this.instruments.map(i=> {
        return { key: i.id, value: i.name };
      });
    }
  },
  methods: {
    submit() {
      if (!this.hasPriceType) {
        this.command.priceType = null;
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
