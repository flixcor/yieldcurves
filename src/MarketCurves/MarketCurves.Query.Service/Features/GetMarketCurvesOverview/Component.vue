<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Market Curves
      </span>
      <ct-spacer />
      <ct-button
        class="primary"
        fab
        @click="emitCreateClicked()"
      >
        <v-icon>mdi-plus</v-icon>
      </ct-button>
    </template>

    <template v-slot:content>
      <ct-table>
        <thead>
          <tr>
            <th>Name</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="curve of entities"
            :key="curve.id"
            @click="emitDetailClicked(curve.id)"
          >
            <td>{{ curve.name }}</td>
          </tr>
        </tbody>
      </ct-table>
    </template>
  </ct-card>
</template>

<script>
export default {
  name: 'GetMarketCurves',
  props: {
    entities: {
      required: true,
      type: Array,
    },
  },
  methods: {
    emitDetailClicked(id) {
      this.$emit('detailClicked', id);
    },
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
