import { defineStore } from "pinia";
import { User } from "@/models/User";
import { users } from "@/assets/data/users";
import type { Avatar } from "@/models/Avatar";

interface UserState {
  user: User;
}

export const useUserStore = defineStore({
  id: "userStore",
  state: (): UserState => ({
    user: users[7],
  }),

  getters: {
    getUserRole: (state) => {
      return state.user.role;
    },
  },

  actions: {
    setUsername(newUsername: string) {
      this.user.username = newUsername;
    },

    setAvatar(newAvatar: Avatar) {
      this.user.avatar = newAvatar;
    },
  },
});
