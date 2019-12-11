<template>
  <ct-table>
    <thead>
      <tr>
        <th>Maturity</th>
        <th>Currency</th>
        <th>Value</th>
      </tr>
    </thead>
    <tbody>
      <tr
        v-for="point of points"
        :key="point.id"
      >
        <td>{{ point.maturity }}</td>
        <td>{{ point.currency }}</td>
        <td>{{ point.value }}</td>
      </tr>
    </tbody>
  </ct-table>
</template>

<script>
export default {
  props: {
    points: {
      type: Array,
      required: true
    }
  },
};
</script>