<script setup lang="ts">
import { ref } from "vue";
import HubInputWithBtn from "@/components/hubComponents/HubInputWithBtn.vue";
import HubPopup from "@/components/hubComponents/HubPopup.vue";
import QuestionCreator from "@/components/QuestionCreator.vue";
import HubAccordion from "@/components/hubComponents/HubAccordion.vue";
import HubAccordionElement from "@/components/hubComponents/HubAccordionElement.vue";
import { Catalog } from "@/models/Catalog";
import { ICON } from "@/enums/iconsEnum";
import CatalogCreator from "@/components/CatalogCreator.vue";
import { useUserStore } from "@/stores/userStore";
import NavigationBtns from "@/components/NavigationBtns.vue";
import WhiteSelectList from "@/components/selectLists/WhiteSelectList.vue";
import {
  convertCatalogsToListElement,
  convertQuestionToListElement,
  ListElement,
} from "@/components/selectLists/ListElement";
import ScrollSelectList from "@/components/selectLists/ScrollSelectList.vue";
import { Question } from "@/models/Question";
import { userService } from "@/api/services/UserService";

const userStore = useUserStore();

const addQuestion = async (catalogs: Catalog[]) => {
  event.preventDefault();
  if (addManyQuestonsToCatalogs.value && catalogs != null && catalogs.length) {
    return assignedQuestionsToCatalogs(catalogs);
  }

  if (!newQuestion.value) {
    return;
  }

  const questionToStore = new Question(
    null,
    newQuestion.value,
    userStore.user.userId
  );

  const response = await userService.addQuestion(questionToStore);
  if (!response) {
    return;
  }

  let catalogIds: number[] = [];
  if (catalogs && catalogs.length > 0) {
    catalogIds = catalogs
      .map((catalog) => catalog.catalogId)
      .filter((id): id is number => id !== null);
    questionToStore.addCatalogs(catalogIds);
  }

  questionToStore.id = response;
  userStore.addQuestion(questionToStore);
  if (catalogIds.length) {
    userStore.assignedQuestionsToCatalogs([response], catalogIds);
    refreshCatalogList();
  }

  isQuestionCreatorOpen.value = false;
  newQuestion.value = "";
  actualQuestions.value = userStore.user.questions.map(
    convertQuestionToListElement
  );
};

const deleteQuestions = (questions: ListElement[]) => {
  const questionsIds = questions.map((question) => question.id);
  questionsIds.forEach((questionId) => {
    userService.removeQuestion(questionId); // TODO: zrobic zeby nie szly requesty w pętli. i dodac await
    userStore.removeQuestion(questionId);
    refreshCatalogList();
  });
};

const assignedQuestionsToCatalogs = async (catalogs: Catalog[]) => {
  const selectedActualQuestionsIds = questionsToSelect.value.map(
    (question) => question.id
  );
  const selectedActualCatalogsIds = catalogs
    .map((catalog) => catalog.catalogId)
    .filter((catalogId) => catalogId !== null);

  const response = await userService.assignedQuestionsToCatalogs(
    selectedActualQuestionsIds,
    selectedActualCatalogsIds
  );

  if (!response) {
    return;
  }

  userStore.assignedQuestionsToCatalogs(
    selectedActualQuestionsIds,
    selectedActualCatalogsIds
  );

  refreshCatalogList();
  isQuestionCreatorOpen.value = false;
  addManyQuestonsToCatalogs.value = false;
  questionsToSelect.value = [];
  actualQuestions.value.forEach((question) => (question.isSelected = false));
};

const addNewCatalog = () => {
  editCatalogMode.value = false;
  isCatalogCreatorOpen.value = true;
  currentCatalog.value = new Catalog();
};

const addQuestionsToCatalog = (questions: ListElement[]) => {
  addManyQuestonsToCatalogs.value = true;
  isQuestionCreatorOpen.value = true;
  questionsToSelect.value = questions;
  const selectedQuestions = questionsToSelect.value.filter(
    (question) => question.isSelected
  );

  if (selectedQuestions.length === 1) {
    const question = userStore.user.questions.find(
      (q) => q.id === questionsToSelect.value[0].id
    );
    catalogsIdsFromSelectedQuestion.value = question?.catalogIds ?? [];
  } else {
    catalogsIdsFromSelectedQuestion.value = [];
  }
};

const showCatalogsList = () => {
  catalogsIdsFromSelectedQuestion.value = [];
  addManyQuestonsToCatalogs.value = false;
  isQuestionCreatorOpen.value = true;
};

const showCatalogDetails = (catalog: ListElement) => {
  currentCatalog.value = userStore.user.catalogs.find(
    (actualCatalog) => actualCatalog.catalogId === catalog.id
  )!;
  editCatalogMode.value = true;
  isCatalogCreatorOpen.value = true;
};

const closePopup = (refresh: boolean = false) => {
  isCatalogCreatorOpen.value = false;

  if (refresh) {
    setOpenTab.value = "yourCatalogs";
    refreshCatalogList();
  }
};

const refreshCatalogList = () => {
  actualCatalogs.value = userStore.user.catalogs.map(
    convertCatalogsToListElement
  );
};

const isCatalogCreatorOpen = ref<boolean>(false);
const isQuestionCreatorOpen = ref<boolean>(false);
const setOpenTab = ref<string>("addQuestion");
const newQuestion = ref<string>("");
const currentCatalog = ref<Catalog>(new Catalog());
const editCatalogMode = ref<boolean>(true);
const addManyQuestonsToCatalogs = ref<boolean>(false);
const actualQuestions = ref<ListElement[]>(
  userStore.user.questions.map(convertQuestionToListElement)
);
const questionsToSelect = ref<ListElement[]>([]);
const catalogsIdsFromSelectedQuestion = ref<number[]>([]);
const actualCatalogs = ref<ListElement[]>(
  userStore.user.catalogs.map(convertCatalogsToListElement)
);
const addQuestionBtn = {
  text: "add",
  isOrange: true,
  action: addQuestion,
};
</script>

<template>
  <div class="libraryView">
    <HubPopup v-model="isQuestionCreatorOpen">
      <QuestionCreator
        :newQuestion="newQuestion"
        @addQuestion="addQuestion"
        :isQuestionCreatorOpen="isQuestionCreatorOpen"
        :addMany="addManyQuestonsToCatalogs"
        :selectedCatalogsIds="catalogsIdsFromSelectedQuestion"
      />
    </HubPopup>
    <HubPopup v-model="isCatalogCreatorOpen">
      <CatalogCreator
        v-model="currentCatalog"
        :editMode="editCatalogMode"
        @closePopup="closePopup"
      />
    </HubPopup>
    <HubAccordionElement
      @click="addNewCatalog"
      title="addCatalog"
      isSmallerTitle
    />
    <HubAccordion
      v-model="setOpenTab"
      :slotNames="['yourCatalogs', 'addQuestion']"
      isSmallerTitle
    >
      <template #yourCatalogs>
        <WhiteSelectList
          class="yourCatalogs"
          v-model="actualCatalogs"
          :height="146"
          :itemsPerPage="3"
          :fontSize="14"
          emptyDataText="psych.noDirectoryHasBeenCreatedYet"
          multiple
          showPagination
          showElementsCountInItem
          showItemDetailsBtn
          @showDetails="showCatalogDetails"
        />
      </template>
      <template #addQuestion>
        <HubInputWithBtn
          v-model="newQuestion"
          class="addQuestionToLibrary"
          :btnAction="addQuestionBtn.action"
          :btnText="addQuestionBtn.text"
          :extraBtnIcon="ICON.ADD_TO_COLLECTION"
          :extraBtnAction="showCatalogsList"
          :btnIsOrange="addQuestionBtn.isOrange"
          isTextarea
        />
      </template>
    </HubAccordion>
    <HubAccordionElement
      @click="setOpenTab = ''"
      class="manageLibrary"
      title="yourQuestions"
      isOpen
      isSmallerTitle
      centerContent
    >
      <ScrollSelectList
        v-model="actualQuestions"
        addCutomText="psych.addToCatalog"
        emptyDataText="psych.noQuestionsHasBeenCreatedYet"
        @addItems="addQuestionsToCatalog"
        @deleteItems="deleteQuestions"
      />
    </HubAccordionElement>
    <NavigationBtns btn="back" btn2="shop" btn2Disabled />
  </div>
</template>

<style lang="scss" scoped>
@import "@/assets/styles/variables";

.libraryView {
  background: $mainBackground;
  height: 100%;
  padding: 12px;
  display: flex;
  flex-direction: column;
  gap: 12px;

  .yourCatalogs {
    padding: 8px;
  }

  .addQuestionToLibrary {
    padding: 12px;
  }

  .manageLibrary {
    flex-grow: 1;
  }
}
</style>
