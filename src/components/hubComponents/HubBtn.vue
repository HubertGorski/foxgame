<script setup lang="ts">
import { ref } from "vue";

const props = defineProps({
  text: {
    type: String,
    required: false,
  },
  icon: {
    type: String,
    required: false,
  },
  isOrange: {
    type: Boolean,
    default: false,
  },
  isSwitch: {
    type: Boolean,
    default: false,
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  action: {
    type: Function,
    required: true,
  },
});

const isBtnClicked = ref(false);

const handleAction = () => {
  if (props.isSwitch) {
    isBtnClicked.value = !isBtnClicked.value;
  }
  props.action();
};
</script>

<template>
  <div
    @click="handleAction()"
    class="hubBtn"
    :class="[
      { disabled },
      isBtnClicked
        ? isOrange
          ? 'hubBtn--darkOrange'
          : 'hubBtn--darkBrown'
        : isOrange
          ? 'hubBtn--orange'
          : 'hubBtn--brown',
    ]"
  >
    <p v-if="text">{{ $t(text) }}</p>
    <v-icon v-if="icon">{{ icon }}</v-icon>
  </div>
</template>

<style lang="scss" scoped>
@import "@/assets/styles/variables";

.hubBtn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  max-width: 300px;
  padding: 20px;
  border-radius: 10px;
  color: white;
  font-size: 18px;
  font-weight: bold;
  text-transform: uppercase;
  cursor: pointer;
  box-shadow:
    0 4px 6px rgba(0, 0, 0, 0.1),
    0 1px 3px rgba(0, 0, 0, 0.06);
  white-space: nowrap;

  &--brown {
    background-color: $mainBrownColor;
  }

  &--darkBrown {
    background-color: $lightBrownColor;
    transform: scale(0.98);
    box-shadow: rgba(50, 50, 93, 0.25) 0px 50px 100px -20px, rgba(0, 0, 0, 0.3) 0px 30px 60px -30px, rgba(10, 37, 64, 0.35) 0px -2px 6px 0px inset;
  }

  &--orange {
    background-color: $mainOrangeColor;
  }

  &.disabled {
    pointer-events: none;
    background-color: $lightGrayColor;
  }
}
</style>
