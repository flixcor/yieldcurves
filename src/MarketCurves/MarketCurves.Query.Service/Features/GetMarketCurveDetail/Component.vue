<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Detail for {{ name }}
      </span>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="emitCreateClicked()"
      >
        <v-icon>mdi-plus</v-icon>
      </ct-btn>
    </template>

    <template v-slot:content>
      <ct-table>
        <thead>
          <tr>
            <td>Tenor</td>
            <td>Vendor</td>
            <td>Name</td>
            <td>DateLag</td>
            <td>IsMandatory</td>
            <td>PriceType</td>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="point of curvePoints"
            :key="point.tenor"
          >
            <td>{{ point.tenor }}</td>
            <td>{{ point.vendor }}</td>
            <td>{{ point.name }}</td>
            <td>{{ point.dateLag }}</td>
            <td>{{ point.isMandatory }}</td>
            <td>{{ point.priceType }}</td>
          </tr>
        </tbody>
      </ct-table>
    </template>
  </ct-card>
</template>

<script>
export default {
  name: 'GetMarketCurve',
  props: {
    name: {
      required: true,
      type: String,
    },
    curvePoints: {
      required: true,
      type: Array,
    },
  },
  methods: {
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
