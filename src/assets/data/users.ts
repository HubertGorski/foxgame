import { User } from "@/models/User";
import { achievements } from "./achievements";

export const users = [
  new User({userId: 1, username: "Natalia Bardzodlug ienazwisko"}),
  new User({userId: 2, username: "Adam Kaleta"}),
  new User({userId: 3, username: "Przemo"}),
  new User({userId: 4, username: "Mariuszek DobryCzłowiek"}),
  new User({userId: 5, username: "Hubert Bardzodlugienazwisko"}),
  new User({userId: 6, username: "Marta"}),
  new User({userId: 7, username: "Wojtek"}),
  new User({userId: 8, username: "Natalia", achievements: achievements})
];
