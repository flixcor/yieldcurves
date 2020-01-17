<template>
  <ct-table>
    <thead>
      <tr>
        <th scope="col">
          As Of
        </th>
        <th scope="col">
          As At
        </th>
        <th scope="col">
          Vendor
        </th>
        <th scope="col">
          Instrument
        </th>
        <th scope="col">
          Currency
        </th>
        <th scope="col">
          Amount
        </th>
        <th scope="col">
          Price Type
        </th>
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