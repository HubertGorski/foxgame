import { defineStore } from "pinia";
import { User } from "@/models/User";
import { users } from "@/assets/data/users";
import type { ROLE } from "@/enums/rolesEnum";

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
    setUserPermissions(role: ROLE) {
      this.user.role = role;
    },
  },
});
