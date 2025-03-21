<script setup lang="ts">
import type { Deck } from '@/models/Deck';
import { useUserStore } from '@/stores/userStore';
import { ref, watch } from 'vue';
import { convertDecksToListElement, type ListElement } from '../whiteSelectList/ListElement';
import WhiteCard from '../WhiteCard.vue';
import WhiteSelectList from '../whiteSelectList/WhiteSelectList.vue';
import HubBtn from '../hubComponents/HubBtn.vue';
import { DylematyCard } from '@/models/DylematyCard';

const userStore = useUserStore();

const props = defineProps({
  newCard: {
    type: DylematyCard,
    required: true,
  },
});

const emit = defineEmits<{
  (e: "addCard", decks: Deck[]): void;
}>();

const actualDecks = ref<ListElement[]>(
  userStore.user.decks.map(convertDecksToListElement)
);

watch(userStore.user.decks, (newDecks) => {
  actualDecks.value = newDecks.map(convertDecksToListElement);
});

const addCard = () => {
  const selectedActualDeckIds = new Set(
    actualDecks.value
      .filter((deck) => deck.isSelected)
      .map((c) => c.id)
  );

  const selectedUserDecks = userStore.user.decks.filter((deck) =>
    selectedActualDeckIds.has(deck.id)
  );

  actualDecks.value = userStore.user.decks.map(
    convertDecksToListElement
  );

  emit("addCard", selectedUserDecks);
};

const addCardBtn = {
  text: "add",
  isOrange: true,
  action: addCard,
};
</script>

<template>
  <div class="cardCreator creamCard">
    <div class="cardCreator_title">Dodaj do katalogu</div>
    <WhiteCard header="Utworzone pytanie:">
      <div class="card">
        {{ newCard.text }}
      </div>
    </WhiteCard>
    <WhiteSelectList
      v-model="actualDecks"
      customSelectedCountTitle="selectedDecks"
      :height="198"
      showSelectedCount
      multiple
      showPagination
    />
    <HubBtn
      class="cardCreator_btn"
      :action="addCardBtn.action"
      :text="addCardBtn.text"
      :isOrange="addCardBtn.isOrange"
    />
  </div>
</template>

<style lang="scss" scoped>
@import "@/assets/styles/variables";

.cardCreator {
  width: 324px;
  padding: 12px;

  &_title {
    color: $grayColor;
    font-size: 24px;
    font-weight: 600;
  }

  .card {
    font-size: 14px;
    font-style: italic;
    color: $lightGrayColor;
    letter-spacing: 0.5px;
  }

  &_btn {
    padding: 8px;
    margin-top: 16px;
  }
}
</style>
