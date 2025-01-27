import { User } from "@/models/User";
import { Game } from "@/models/Game";

const users: User[] = [
  new User({userId: 1, username: "Natalia Bardzodlug ienazwisko"}),
  new User({userId: 2, username: "Adam Kaleta"}),
  new User({userId: 3, username: "Przemo"}),
];

export const games = [
  new Game('PIESEK1', true, new User(), users),
  new Game('PLACKI3', true, new User({userId: 1, username: "Aleks"}), users, 'oko'),
  new Game('UN2PUBLIC', false, new User({userId: 2, username: "Artur"}), users, 'oko'),
  new Game('HEHE54', true, new User({userId: 3, username: "Przemysław Bukowski"}), users),
  new Game('UN2PUBLIC', true, new User({userId: 4, username: "Mariuszek Corporation"}), users, 'oko'),
  new Game('UN2PUBLIC', true, new User({userId: 5, username: "MariuszekCorporationVersionBezSpacji"}), users, 'oko'),
];
