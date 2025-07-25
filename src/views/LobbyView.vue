<script setup lang="ts">
import UserListElement from "@/components/UserListElement.vue";
import { computed, ref, toRef, watch } from "vue";
import NavigationBtns from "@/components/NavigationBtns.vue";
import { useSignalRStore } from "@/stores/signalRStore";
import { useRouter } from "vue-router";
import { ROUTE_PATH } from "@/router/routeEnums";
import { Game } from "@/models/Game";
import { useUserStore } from "@/stores/userStore";
import { NO_ACCESS_REASON } from "@/enums/noAccessReasonEnum";

const router = useRouter();
const signalRStore = useSignalRStore();
const userStore = useUserStore();
const isZoom = ref<boolean>(false);

const leaveRoom = async () => {
  await signalRStore.leaveRoom(userStore.user.userId);
  router.push(ROUTE_PATH.MENU);
};

const goToSettings = async () => {
  router.push(ROUTE_PATH.CREATE_GAME_PSYCH);
};

const goToGame = async () => {
  await signalRStore.removeRoom(); // TODO: usuanc jak zaczne robic gre
  router.push(ROUTE_PATH.GAME_PSYCH);
};

const setReady = async () => {
  userStore.user.isReady = !userStore.user.isReady;
  await signalRStore.setStatus(userStore.user.userId, userStore.user.isReady);
};

const optionBtnAction = async () => {
  if (isOwner.value) {
    goToSettings();
  } else {
    await leaveRoom();
  }
};

const startBtnAction = async () => {
  if (isOwner.value) {
    goToGame();
  } else {
    setReady();
  }
};

const game = computed<Game>(
  () => toRef(signalRStore, "game").value ?? new Game()
);

const customCodeLabel = computed(() => {
  return isZoom.value ? game.value.code : `Kod dostępu: ${game.value.code}`;
});

const isMinimalViewMode = computed(() => {
  return game.value.users.length > 4;
});

const isOwner = computed(() => {
  return game.value.owner.userId === userStore.user.userId;
});

const optionBtn = computed(() => {
  return {
    text: isOwner.value ? "settings" : "back",
    isOrange: false,
    action: optionBtnAction,
  };
});

const startBtn = computed(() => {
  return {
    text: isOwner.value ? "start" : "ready",
    action: startBtnAction,
    tooltipText: isOwner.value ? "tooltip.startNewGame" : "",
    disabled:
      game.value.areUsersUnready &&
      game.value.owner.userId === userStore.user.userId,
  };
});

if (!game.value.users.length) {
  router.push(ROUTE_PATH.JOIN_GAME);
}

watch(game, (game: Game | null) => {
  if (game == null || game.code == null) {
    router.push({
      path: ROUTE_PATH.NO_ACCESS,
      query: { reason: NO_ACCESS_REASON.GAME_CLOSED },
    });
  }
});
</script>

<template>
  <div class="lobbyView">
    <div class="lobbyView_header">
      <p class="title">Oczekiwanie na graczy</p>
      <p class="counter">
        ({{ game.readyUsersCount }} / {{ game.usersCount }})
      </p>
    </div>
    <img
      v-if="game.users.length === 0"
      src="@/assets/imgs/fox3.png"
      alt="Lisek"
      class="lobbyView_emptyUserList"
    />
    <div
      v-else
      :class="{ isMinimalViewMode: isMinimalViewMode }"
      class="lobbyView_userList"
    >
      <div v-for="user in game.users" :key="user.userId">
        <user-list-element :user="user" />
      </div>
    </div>
    <div @click="isZoom = !isZoom" :class="{ mask: isZoom }"></div>
    <div @click="isZoom = !isZoom" :class="{ zoomCustomCodeSection: isZoom }">
      <span v-if="!isZoom" class="icon">!</span>
      <span class="customCodeSection">{{ customCodeLabel }}</span>
    </div>
    <span v-if="isZoom" class="isZoom"></span>
    <NavigationBtns
      :btn="optionBtn.text"
      :btnIsOrange="optionBtn.isOrange"
      :btnCustomAction="optionBtn.action"
      :btn2="startBtn.text"
      :btn2Disabled="startBtn.disabled"
      :btn2TooltipText="startBtn.tooltipText"
      :btn2CustomAction="startBtn.action"
      :btn2isSwitch="!isOwner"
    />
  </div>
</template>

<style lang="scss" scoped>
@import "@/assets/styles/variables";
.lobbyView {
  background: $mainBackground;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  padding: 16px;

  .zoomCustomCodeSection {
    transform: translate(5px, -276px) scale(3);
    z-index: 3;
    transition: all 0.4s;
  }
  .isZoom {
    height: 24px;
  }
  .mask {
    content: "";
    position: absolute;
    background-color: rgba(0, 0, 0, 0.6);
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    z-index: 2;
    transition: all 0.4s;
  }

  .icon {
    position: relative;
    top: 4px;
    right: 4px;
    font-size: 32px;
    font-weight: 900;
    color: $mainBrownColor;
  }

  .customCodeSection {
    background-color: $background;
    color: $mainBrownColor;
    font-size: 24;
    font-weight: 600;
    text-align: center;
    padding: 8px 20px;
    border-radius: 12px;
    border: 2px solid $mainBrownColor;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    margin: 10px auto;
    width: fit-content;
  }

  &_header {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: $mainBrownColor;
    font-weight: 600;

    .title {
      font-size: 24px;
    }

    .counter {
      font-size: 18px;
    }
  }

  &_emptyUserList {
    height: 450px;
  }

  &_userList {
    display: flex;
    flex-direction: column;
    height: 450px;
    justify-content: center;

    &.isMinimalViewMode {
      justify-content: space-between;
      overflow-y: scroll;
    }
  }
}
</style>
