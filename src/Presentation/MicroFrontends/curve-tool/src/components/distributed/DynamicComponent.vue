<template>
  <Component
    :is="computedComponent"
    v-bind="props"
    v-on="$listeners"
  />
</template>

<script>
import externalComponent from '../../utils/external-component';
export default {
  name: `DynamicComponent`,
  props: {
    component: {
      required: true,
      type: String,
    },
    props: {
      default: () => ({}),
      type: Object,
    },
  },
  data() {
    return {
      computedComponent: null,
    };
  },
  watch: {
    component: {
      immediate: true,
      handler(newComponent, prevComponent = ``) {
        if (newComponent === prevComponent) return;
        this.computedComponent = () => externalComponent(this.component);
      },
    },
  },
};
</script>