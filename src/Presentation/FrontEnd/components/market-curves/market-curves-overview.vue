<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Market Curves
      </span>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="$emit('createClicked')"
      >
        <v-icon>mdi-plus</v-icon>
      </ct-btn>
    </template>

    <template v-if="state.marketCurves" v-slot:content>
      <ct-table>
        <thead>
          <tr>
            <th scope="col">
              Name
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="curve of state.marketCurves"
            :key="curve.aggregateId"
            @click="$emit('detailClicked', curve.aggregateId)"
          >
            <td>{{ getName(curve) }}</td>
          </tr>
        </tbody>
      </ct-table>
    </template>
  </ct-card>
</template>

<script>
const appendNonEmpty = (seperator, items) => items.filter(x => x).join(seperator)

export default {
  name: 'GetMarketCurves',
  props: {
    state: {
      type: Object,
      default: () => ({
        marketCurves: []
      })
    }
  },
  methods: {
    getName (curve) {
      const { content } = curve
      return appendNonEmpty('_', [content.country, content.curveType, content.floatingLeg])
    }
  }
}
</script>
