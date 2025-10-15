# FoxTales — Be as sly as a fox!

**FoxTales CI/CD build status**:  
[![FoxTales CI/CD](https://github.com/HubertGorski/foxtales/actions/workflows/foxtales.yml/badge.svg)](https://github.com/HubertGorski/foxtales/actions/workflows/foxtales.yml)

## Table of Contents
- [Overview](#-overview)
- [Gameplay](#-gameplay)
- [Guest Mode](#-guest-mode)
- [Room Options](#-room-options)
- [Example Scenarios](#-example-scenarios)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Author](#-author)

## 🎮 Overview

FoxTales is a browser-based social party game that requires no installation — simply visit https://foxtales.cc and start playing.

Designed with simplicity and fun in mind, it's perfect for parties, online gatherings, or team-building events.

## 🎯 Gameplay
FoxTales revolves around creativity and getting to know each other better through a simple 5-step process:
- 🎲 **Question Draw** — The game selects a random question about one of the players in the room
- 💭 **Answers** — Each player anonymously writes their response
- 🗳️ **Voting** — Everyone votes for the best answer (authors remain hidden)
- 🏆 **Scoring** — Players earn points for votes received on their answers
- 📊 **Summary** — After each round, results are displayed and a new round begins

## 👤 Guest Mode
Don't want to register? **No problem!**

You can play as a **guest** with full functionality.

_Your progress and resources will be lost after logging out._


## ⚙️ Room Options
Room owners have full control over the game experience:

### 🎯 Question Types
Choose where your game questions come from:
- 🌍 **Public Questions** — Use built-in question sets from FoxTales
- 🔒 **Private Questions** — Use your own custom questions and categories
- 🔄 **Mixed Mode** — Combine both public and private questions

### 👥 Player Permissions
- ✅ **Allow Custom Categories** — Let other players add their question categories to your room

### 🔗 Room Access
Control who can join your game:
- 🔐 **Private Room** — Share access code with specific players
- 📋 **Public Listing** — Room appears in public game list, anyone can join
- 🌐 **Password Protected** — Room is public but requires access password

## 📖 Example Scenarios
### Example 1: Star Wars Themed Party
#### The Setup:
Anna and Mark are heading to Chris's Star Wars themed house party.

#### Preparation:
- 🪐 Anna creates a _"Star Wars"_ category with the question:
"If **** had a lightsaber, what would they use it for?"
- 🎉 Mark creates a _"Party Time"_ category with the question:
"**** wins a million dollars! What do they do with it?"

#### At the Party:
- 🏠 Chris creates a game room with the option to allow custom question categories
- 👥 Anna adds her "Star Wars" category to the room
- 👥 Mark adds his "Party Time" category to the room
- ⚙️ Chris disables both public questions and his own personal categories

#### The Game:
- 🎲 Questions are randomly drawn from both "Party Time" and "Star Wars" categories
- 🎯 First drawn question: "Chris wins a million dollars! What do they do with it?"


## ✨ Features

🕹️ **Browser-based**	No installation required

👥 **Unlimited Players**	No room size limits

🧑‍🚀 **Guest Mode**	Play without registration

🔐 **Flexible Access**	Multiple room sharing options

💬 **Custom Content**	Create your own questions and categories

🎨 **Avatar Selection**	Choose your player avatar

⚡ **Real-time**	Instant gameplay experience

## 🛠️ Tech Stack
- **Frontend:** Vue3
- **Backend:** .NET 9, C#
- **Architecture:**	Clean Architecture (Onion)
- **Communication:** SignalR + MediatR
- **Authentication:**	Access & Refresh Tokens
- **Background Processing:**	Background Services

## 👨‍💻 Author
_FoxTales was created out of passion for technology and social games._

**Hubert Górski**

---
Ready to play? Visit https://foxtales.cc and start your adventure!
