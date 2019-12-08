<template>
  <ct-table>
    <thead>
      <tr>
        <th>As Of</th>
        <th>As At</th>
        <th>Vendor</th>
        <th>Instrument</th>
        <th>Currency</th>
        <th>Amount</th>
        <th>Price Type</th>
      </tr>
    </thead>
    <tbody>
      <tr
        v-for="price of entities"
        :key="price.id"
      >
        <td>{{ price.asOfDate.substring(0,10) }}</td>
        <td>{{ price.asAtDate.substring(0,16) }}</td>
        <td>{{ price.vendor }}</td>
        <td>{{ price.instrument }}</td>
        <td>{{ price.priceCurrency }}</td>
        <td>{{ price.priceAmount }}</td>
        <td>{{ price.priceType }}</td>
      </tr>
    </tbody>
  </ct-table>
</template>
<script>

export default {
  props: {
    entities: {
      type: Array,
      required: true
    }
  }
};
</script>