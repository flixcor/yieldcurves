<template>
  <ct-card>
    <template v-slot:title>
      <span>Instruments</span>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="$emit('create')"
      >
        <v-icon>mdi-plus</v-icon>
      </ct-btn>
    </template>
    <template v-slot:content>
      <ct-table>
        <thead>
          <tr>
            <th>Vendor</th>
            <th>Description</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="instrument of state.instruments"
            :key="instrument.aggregateId"
          >
            <td>{{ instrument.vendor }}</td>
            <td>{{ instrument.description }}</td>
          </tr>
        </tbody>
      </ct-table>
    </template>
  </ct-card>
</template>
<script>
export default {
  props: {
    state: {
      type: Object,
      default: () => ({
        instruments: []
      })
    }
  }
}
</script>
